using Ext.Net;
using da = SAEON.ObservationsDB.Data;
using System;
using System.Linq;
using SubSonic;
using Serilog;

public partial class Admin_StationV2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!X.IsAjaxRequest)
        {
            ProjectSiteStore.DataSource = new da.ProjectSiteCollection().OrderByAsc(da.ProjectSite.Columns.Name).Load();
            ProjectSiteStore.DataBind();
            SiteStore.DataSource = new da.SiteCollection().OrderByAsc(da.Site.Columns.Name).Load();
            SiteStore.DataBind();
            OrganisationStore.DataSource = new da.OrganisationCollection().OrderByAsc(da.Organisation.Columns.Name).Load();
            OrganisationStore.DataBind();
            OrganisationRoleStore.DataSource = new da.OrganisationRoleCollection().OrderByAsc(da.OrganisationRole.Columns.Name).Load();
            OrganisationRoleStore.DataBind();
        }
    }

    #region Stations
    protected void StationGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        StationGrid.GetStore().DataSource = StationRepository.GetPagedList(e, e.Parameters[GridFilters1.ParamPrefix]);
    }


    protected void ValidateField(object sender, RemoteValidationEventArgs e)
    {
        da.StationCollection col = new da.StationCollection();

        string checkColumn = String.Empty,
               errorMessage = String.Empty;

        if (e.ID == "tfCode")
        {
            checkColumn = da.Station.Columns.Code;
            errorMessage = "The specified Station Code already exists";
        }
        else if (e.ID == "tfName")
        {
            checkColumn = da.Station.Columns.Name;
            errorMessage = "The specified Station Name already exists";

        }

        if (String.IsNullOrEmpty(tfID.Text.ToString()))
            col = new da.StationCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
        else
            col = new da.StationCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(da.Station.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();

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
            da.Station stat = new da.Station();

            if (String.IsNullOrEmpty(tfID.Text))
                stat.Id = Guid.NewGuid();
            else
                stat = new da.Station(tfID.Text.Trim());

            if (!string.IsNullOrEmpty(tfCode.Text.Trim()))
                stat.Code = tfCode.Text.Trim();
            if (!string.IsNullOrEmpty(tfName.Text.Trim()))
                stat.Name = tfName.Text.Trim();
            stat.Description = tfDescription.Text.Trim();
            stat.ProjectSiteID = new Guid(cbProjectSite.SelectedItem.Value.Trim());
            stat.SiteID = new Guid(cbSite.SelectedItem.Value.Trim());

            if (!string.IsNullOrEmpty(nfLatitude.Text))
                stat.Latitude = Double.Parse(nfLatitude.Text);

            if (!string.IsNullOrEmpty(nfLongitude.Text))
                stat.Longitude = Double.Parse(nfLongitude.Text);

            if (!string.IsNullOrEmpty(nfElevation.Text))
                stat.Elevation = Int32.Parse(nfElevation.Text);

            if (!string.IsNullOrEmpty(tfUrl.Text))
                stat.Url = tfUrl.Text;

            stat.UserId = AuthHelper.GetLoggedInUserId;

            stat.Save();

            StationGrid.DataBind();

            DetailWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Save");
            MessageBoxes.Error(ex, "Unable to save station");
        }
    }

    protected void StationStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();
        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        string js = BaseRepository.BuildExportQ("VStation", gridData, visCols, sortCol, sortDir);

        BaseRepository.doExport(type, js);
    }

    protected void DoDelete(object sender, DirectEventArgs e)
    {
        string ActionType = e.ExtraParams["type"];
        string recordID = e.ExtraParams["id"];
        try
        {
            if (ActionType == "RemoveInstrument")
            {
                da.DataSource dataSource = new da.DataSource(recordID);
                if (dataSource != null)
                {
                    dataSource.StationID = null;
                    dataSource.UserId = AuthHelper.GetLoggedInUserId;
                    dataSource.Save();
                    StationGrid.DataBind();
                }
            }
            else if (ActionType == "RemoveOrganisation")
            {
                new da.StationOrganisationController().Delete(recordID);
                OrganisationGrid.DataBind();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "DoDelete({ActionType},{RecordID})", ActionType, recordID);
            MessageBoxes.Error(ex, "Unable to delete {0}", ActionType == "RemoveInstrument" ? "instrument" : "organisation");
        }
    }
    #endregion

    #region Instruments

    protected void InstrumentGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["StationID"] != null && e.Parameters["StationID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["StationID"].ToString());
            da.DataSourceCollection col = new da.DataSourceCollection()
                .Where(da.DataSource.Columns.StationID, Id)
                .OrderByAsc(da.DataSource.Columns.Code)
                .Load();
            InstrumentGrid.GetStore().DataSource = col;
            InstrumentGrid.GetStore().DataBind();
        }
    }

    protected void AvailableInstrumentsStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["StationID"] != null && e.Parameters["StationID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["StationID"].ToString());
            da.DataSourceCollection col = new Select()
                .From(da.DataSource.Schema)
                .Where(da.DataSource.IdColumn)
                .NotIn(new Select(new string[] { da.DataSource.Columns.Id }).From(da.DataSource.Schema).Where(da.DataSource.IdColumn).IsEqualTo(Id))
                .And(da.DataSource.StationIDColumn)
                .IsNull()
                .OrderAsc(da.DataSource.Columns.Code)
                .ExecuteAsCollection<da.DataSourceCollection>();
            AvailableInstrumentsGrid.GetStore().DataSource = col;
            AvailableInstrumentsGrid.GetStore().DataBind();
        }
    }

    protected void AcceptInstruments_Click(object sender, DirectEventArgs e)
    {
        try
        {
            RowSelectionModel sm = AvailableInstrumentsGrid.SelectionModel.Primary as RowSelectionModel;
            RowSelectionModel masterRow = InstrumentGrid.SelectionModel.Primary as RowSelectionModel;

            var masterID = masterRow.SelectedRecordID;
            if (sm.SelectedRows.Count > 0)
            {
                foreach (SelectedRow row in sm.SelectedRows)
                {
                    da.DataSource dataSource = new da.DataSource(row.RecordID);
                    if (dataSource != null)
                    {
                        dataSource.StationID = new Guid(masterID);
                        dataSource.UserId = AuthHelper.GetLoggedInUserId;
                        dataSource.Save();
                    }
                }
                InstrumentGrid.DataBind();
                AvailableInstrumentsWindow.Hide();
            }
            else
            {
                MessageBoxes.Info("Invalid Selection", "Select at least one instrument");
            }

        }
        catch (Exception ex)
        {
            Log.Error(ex, "AcceptInstruments_Click");
            MessageBoxes.Error(ex, "Unable to save instruments");
        }
    }
    #endregion

    #region Organisations

    protected void OrganisationGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["StationID"] != null && e.Parameters["StationID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["StationID"].ToString());
            da.VStationOrganisationCollection StationOrganisationCol = new da.VStationOrganisationCollection()
                .Where(da.VStationOrganisation.Columns.StationID, Id)
                .OrderByAsc(da.VStationOrganisation.Columns.StartDate)
                .OrderByAsc(da.VStationOrganisation.Columns.EndDate)
                .OrderByAsc(da.VStationOrganisation.Columns.OrganisationName)
                .OrderByAsc(da.VStationOrganisation.Columns.OrganisationRoleName)
                .Load();
            OrganisationGrid.GetStore().DataSource = StationOrganisationCol;
            OrganisationGrid.GetStore().DataBind();
        }
    }

    protected void AcceptOrganisation_Click(object sender, DirectEventArgs e)
    {
        try
        {
            RowSelectionModel masterRow = StationGrid.SelectionModel.Primary as RowSelectionModel;
            var stationID = masterRow.SelectedRecordID;
            da.StationOrganisation stationOrganisation = new da.StationOrganisation();
            stationOrganisation.StationID = new Guid(stationID);
            stationOrganisation.OrganisationID = new Guid(cbOrganisation.SelectedItem.Value.Trim());
            stationOrganisation.OrganisationRoleID = new Guid(cbOrganisationRole.SelectedItem.Value.Trim());
            if (!String.IsNullOrEmpty(dfOrganisationStartDate.Text) && (dfOrganisationStartDate.SelectedDate.Year >= 1900))
                stationOrganisation.StartDate = dfOrganisationStartDate.SelectedDate;
            if (!String.IsNullOrEmpty(dfOrganisationEndDate.Text) && (dfOrganisationEndDate.SelectedDate.Year >= 1900))
                stationOrganisation.EndDate = dfOrganisationEndDate.SelectedDate;
            stationOrganisation.UserId = AuthHelper.GetLoggedInUserId;
            stationOrganisation.Save();
            OrganisationGrid.DataBind();
            OrganisationWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "AcceptOrganisation_Click");
            MessageBoxes.Error(ex, "Unable to save organisation");
        }
    }
    #endregion
}