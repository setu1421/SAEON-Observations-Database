using Ext.Net;
using SAEON.ObservationsDB.Data;
using System;
using System.Linq;
using SubSonic;
using Serilog;
using System.Collections.Generic;

public partial class Admin_Stations : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!X.IsAjaxRequest)
        {
            OrganisationStore.DataSource = new OrganisationCollection().OrderByAsc(Organisation.Columns.Name).Load();
            OrganisationStore.DataBind();
            OrganisationRoleStore.DataSource = new OrganisationRoleCollection().OrderByAsc(OrganisationRole.Columns.Name).Load();
            OrganisationRoleStore.DataBind();
            InstrumentStore.DataSource = new InstrumentCollection().OrderByAsc(Instrument.Columns.Name).Load();
            InstrumentStore.DataBind();
        }
    }

    #region Stations
    protected void StationsGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        StationsGrid.GetStore().DataSource = StationRepository.GetPagedList(e, e.Parameters[GridFilters1.ParamPrefix]);
    }


    protected void ValidateField(object sender, RemoteValidationEventArgs e)
    {
        StationCollection col = new StationCollection();

        string checkColumn = String.Empty,
               errorMessage = String.Empty;

        if (e.ID == "tfCode")
        {
            checkColumn = Station.Columns.Code;
            errorMessage = "The specified Station Code already exists";
        }
        else if (e.ID == "tfName")
        {
            checkColumn = Station.Columns.Name;
            errorMessage = "The specified Station Name already exists";

        }

        if (String.IsNullOrEmpty(tfID.Text.ToString()))
            col = new StationCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
        else
            col = new StationCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(Station.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();

        if (col.Count > 0)
        {
            e.Success = false;
            e.ErrorMessage = errorMessage;
        }
        else
            e.Success = true;
    }

    protected void Save(object sender, DirectEventArgs e)
    {
        try
        {
            Station station = new Station();

            if (String.IsNullOrEmpty(tfID.Text))
                station.Id = Guid.NewGuid();
            else
                station = new Station(tfID.Text.Trim());

            if (!string.IsNullOrEmpty(tfCode.Text.Trim()))
                station.Code = tfCode.Text.Trim();
            if (!string.IsNullOrEmpty(tfName.Text.Trim()))
                station.Name = tfName.Text.Trim();
            station.Description = tfDescription.Text.Trim();

            if (!string.IsNullOrEmpty(nfLatitude.Text))
                station.Latitude = Double.Parse(nfLatitude.Text);

            if (!string.IsNullOrEmpty(nfLongitude.Text))
                station.Longitude = Double.Parse(nfLongitude.Text);

            if (!string.IsNullOrEmpty(nfElevation.Text))
                station.Elevation = Int32.Parse(nfElevation.Text);

            if (!string.IsNullOrEmpty(tfUrl.Text))
                station.Url = tfUrl.Text;

            if (!String.IsNullOrEmpty(dfStartDate.Text) && (dfStartDate.SelectedDate.Year >= 1900))
                station.StartDate = dfStartDate.SelectedDate;
            if (!String.IsNullOrEmpty(dfEndDate.Text) && (dfEndDate.SelectedDate.Year >= 1900))
                station.EndDate = dfEndDate.SelectedDate;

            station.UserId = AuthHelper.GetLoggedInUserId;

            station.Save();
            Auditing.Log("Stations.Save", new Dictionary<string, object> {
                { "ID", station.Id }, { "Code", station.Code }, { "Name", station.Name } });

            StationsGrid.DataBind();

            DetailWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Stations.Save");
            MessageBoxes.Error(ex, "Error", "Unable to save station");
        }
    }

    protected void StationsGridStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();
        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        string js = BaseRepository.BuildExportQ("VStation", gridData, visCols, sortCol, sortDir);

        BaseRepository.doExport(type, js);
    }

    #endregion

    #region Organisations

    protected void OrganisationLinksGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["StationID"] != null && e.Parameters["StationID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["StationID"].ToString());
            VStationOrganisationCollection StationOrganisationCol = new VStationOrganisationCollection()
                .Where(VStationOrganisation.Columns.StationID, Id)
                .OrderByAsc(VStationOrganisation.Columns.StartDate)
                .OrderByAsc(VStationOrganisation.Columns.EndDate)
                .OrderByAsc(VStationOrganisation.Columns.OrganisationName)
                .OrderByAsc(VStationOrganisation.Columns.OrganisationRoleName)
                .Load();
            OrganisationLinksGrid.GetStore().DataSource = StationOrganisationCol;
            OrganisationLinksGrid.GetStore().DataBind();
        }
    }

    protected void LinkOrganisation_Click(object sender, DirectEventArgs e)
    {
        try
        {
            RowSelectionModel masterRow = StationsGrid.SelectionModel.Primary as RowSelectionModel;
            var masterID = new Guid(masterRow.SelectedRecordID);
            StationOrganisation stationOrganisation = new StationOrganisation();
            stationOrganisation.StationID = masterID;
            stationOrganisation.OrganisationID = new Guid(cbOrganisation.SelectedItem.Value.Trim());
            stationOrganisation.OrganisationRoleID = new Guid(cbOrganisationRole.SelectedItem.Value.Trim());
            if (!String.IsNullOrEmpty(dfOrganisationStartDate.Text) && (dfOrganisationStartDate.SelectedDate.Year >= 1900))
                stationOrganisation.StartDate = dfOrganisationStartDate.SelectedDate;
            if (!String.IsNullOrEmpty(dfOrganisationEndDate.Text) && (dfOrganisationEndDate.SelectedDate.Year >= 1900))
                stationOrganisation.EndDate = dfOrganisationEndDate.SelectedDate;
            stationOrganisation.UserId = AuthHelper.GetLoggedInUserId;
            stationOrganisation.Save();
            Auditing.Log("Stations.AddOrganisationLink", new Dictionary<string, object> {
                { "StationID", stationOrganisation.StationID },
                { "StationCode", stationOrganisation.Station.Code },
                { "OrganisationID", stationOrganisation.OrganisationID},
                { "OrganisationCode", stationOrganisation.Organisation.Code},
                { "RoleID", stationOrganisation.OrganisationRoleID },
                { "RoleCode", stationOrganisation.OrganisationRole.Code},
                { "StartDate", stationOrganisation.StartDate },
                { "EndDate", stationOrganisation.EndDate}
            });
            OrganisationLinksGrid.DataBind();
            OrganisationLinkWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Stations.LinkOrganisation_Click");
            MessageBoxes.Error(ex, "Error", "Unable to link organisation");
        }
    }

    [DirectMethod]
    public void ConfirmDeleteOrganisationLink(Guid aID)
    {
        MessageBoxes.Confirm(
            "Confirm Delete",
            String.Format("DirectCall.DeleteOrganisationLink(\"{0}\",{{ eventMask: {{ showMask: true}}}});", aID.ToString()),
            "Are you sure you want to delete this organisation link?");
    }

    [DirectMethod]
    public void DeleteOrganisationLink(Guid aID)
    {
        try
        {
            new StationOrganisationController().Delete(aID);
            Auditing.Log("Stations.DeleteOrganisationLink", new Dictionary<string, object> { { "ID", aID } });
            OrganisationLinksGrid.DataBind();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Stations.DeleteOrganisationLink({aID})", aID);
            MessageBoxes.Error(ex, "Error", "Unable to delete organisation link");
        }
    }

    #endregion

    #region Instruments

    protected void InstrumentLinksGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["StationID"] != null && e.Parameters["StationID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["StationID"].ToString());
            VStationInstrumentCollection col = new VStationInstrumentCollection()
                .Where(VStationInstrument.Columns.StationID, Id)
                .OrderByAsc(VStationInstrument.Columns.InstrumentName)
                .Load();
            InstrumentLinksGrid.GetStore().DataSource = col;
            InstrumentLinksGrid.GetStore().DataBind();
        }
    }

    protected void LinkInstrument_Click(object sender, DirectEventArgs e)
    {
        try
        {
            RowSelectionModel masterRow = StationsGrid.SelectionModel.Primary as RowSelectionModel;
            var masterID = new Guid(masterRow.SelectedRecordID);
            StationInstrument stationInstrument = new StationInstrument();
            stationInstrument.StationID = masterID;
            stationInstrument.InstrumentID = new Guid(cbInstrument.SelectedItem.Value.Trim());
            if (!String.IsNullOrEmpty(dfInstrumentStartDate.Text) && (dfInstrumentStartDate.SelectedDate.Year >= 1900))
                stationInstrument.StartDate = dfInstrumentStartDate.SelectedDate;
            if (!String.IsNullOrEmpty(dfInstrumentEndDate.Text) && (dfInstrumentEndDate.SelectedDate.Year >= 1900))
                stationInstrument.EndDate = dfInstrumentEndDate.SelectedDate;
            stationInstrument.UserId = AuthHelper.GetLoggedInUserId;
            stationInstrument.Save();
            Auditing.Log("Stations.AddInstrumentLink", new Dictionary<string, object> {
                { "StationID", stationInstrument.StationID },
                { "StationCode", stationInstrument.Station.Code },
                { "InstrumentID", stationInstrument.InstrumentID},
                { "InstrumentCode", stationInstrument.Instrument.Code},
                { "StartDate", stationInstrument.StartDate },
                { "EndDate", stationInstrument.EndDate}
            });
            InstrumentLinksGrid.DataBind();
            InstrumentLinkWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Stations.LinkInstrument_Click");
            MessageBoxes.Error(ex, "Error", "Unable to link instrument");
        }
    }

    [DirectMethod]
    public void ConfirmDeleteInstrumentLink(Guid aID)
    {
        MessageBoxes.Confirm(
            "Confirm Delete",
            String.Format("DirectCall.DeleteInstrumentLink(\"{0}\",{{ eventMask: {{ showMask: true}}}});", aID.ToString()),
            "Are you sure you want to delete this instrument link?");
    }

    [DirectMethod]
    public void DeleteInstrumentLink(Guid aID)
    {
        try
        {
            new StationInstrumentController().Delete(aID);
            Auditing.Log("Stations.DeleteInstrumentLink", new Dictionary<string, object> { { "ID", aID } });
            InstrumentLinksGrid.DataBind();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Stations.DeleteInstrumentLink({aID})", aID);
            MessageBoxes.Error(ex, "Error", "Unable to delete instrument link");
        }
    }

    #endregion

}