using Ext.Net;
using da = SAEON.ObservationsDB.Data;
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
            SiteStore.DataSource = new da.SiteCollection().OrderByAsc(da.Site.Columns.Name).Load();
            SiteStore.DataBind();
            OrganisationStore.DataSource = new da.OrganisationCollection().OrderByAsc(da.Organisation.Columns.Name).Load();
            OrganisationStore.DataBind();
            OrganisationRoleStore.DataSource = new da.OrganisationRoleCollection().OrderByAsc(da.OrganisationRole.Columns.Name).Load();
            OrganisationRoleStore.DataBind();
        }
    }

    #region Stations
    protected void StationsGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        StationsGrid.GetStore().DataSource = StationRepository.GetPagedList(e, e.Parameters[GridFilters1.ParamPrefix]);
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
            da.Station station = new da.Station();

            if (String.IsNullOrEmpty(tfID.Text))
                station.Id = Guid.NewGuid();
            else
                station = new da.Station(tfID.Text.Trim());

            if (!string.IsNullOrEmpty(tfCode.Text.Trim()))
                station.Code = tfCode.Text.Trim();
            if (!string.IsNullOrEmpty(tfName.Text.Trim()))
                station.Name = tfName.Text.Trim();
            station.Description = tfDescription.Text.Trim();
            station.SiteID = new Guid(cbSite.SelectedItem.Value.Trim());

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

    #region DataSources

    protected void DataSourcesGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["StationID"] != null && e.Parameters["StationID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["StationID"].ToString());
            da.DataSourceCollection col = new da.DataSourceCollection()
                .Where(da.DataSource.Columns.StationID, Id)
                .OrderByAsc(da.DataSource.Columns.Code)
                .Load();
            DataSourcesGrid.GetStore().DataSource = col;
            DataSourcesGrid.GetStore().DataBind();
        }
    }

    protected void AvailableDataSourcesStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
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
            AvailableDataSourcesGrid.GetStore().DataSource = col;
            AvailableDataSourcesGrid.GetStore().DataBind();
        }
    }

    protected void LinkDataSources_Click(object sender, DirectEventArgs e)
    {
        try
        {
            RowSelectionModel sm = AvailableDataSourcesGrid.SelectionModel.Primary as RowSelectionModel;
            RowSelectionModel masterRow = StationsGrid.SelectionModel.Primary as RowSelectionModel;

            var masterID = new Guid(masterRow.SelectedRecordID);
            if (sm.SelectedRows.Count > 0)
            {
                foreach (SelectedRow row in sm.SelectedRows)
                {
                    da.DataSource dataSource = new da.DataSource(row.RecordID);
                    if (dataSource != null)
                    {
                        dataSource.StationID = masterID;
                        dataSource.UserId = AuthHelper.GetLoggedInUserId;
                        dataSource.Save();
                        Auditing.Log("Stations.AddDataSourceLink", new Dictionary<string, object> {
                            { "StationID", masterID }, { "ID", dataSource.Id }, { "Code", dataSource.Code }, { "Name", dataSource.Name } });
                    }
                }
                DataSourcesGrid.DataBind();
                AvailableDataSourcesWindow.Hide();
            }
            else
            {
                MessageBoxes.Info("Invalid Selection", "Select at least one instrument");
            }

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Stations.LinkDataSources_Click");
            MessageBoxes.Error(ex, "Error", "Unable to link data sources");
        }
    }

    protected void DataSourceLink(object sender, DirectEventArgs e)
    {
        string actionType = e.ExtraParams["type"];
        string recordID = e.ExtraParams["id"];
        try
        {
            if (actionType == "Edit")
            {
            }
            else if (actionType == "Delete")
            {
                da.DataSource dataSource = new da.DataSource(recordID);
                if (dataSource != null)
                {
                    dataSource.StationID = null;
                    dataSource.UserId = AuthHelper.GetLoggedInUserId;
                    dataSource.Save();
                    Auditing.Log("Stations.DeleteDataSourceLink", new Dictionary<string, object> {
                        { "ID", dataSource.Id }, { "Code", dataSource.Code }, { "Name", dataSource.Name } });
                    DataSourcesGrid.DataBind();
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Stations.DataSourceLink({ActionType},{RecordID})", actionType, recordID);
            MessageBoxes.Error(ex, "Unable to {0} data source link", actionType);
        }
    }
    #endregion

    #region Organisations

    protected void OrganisationLinksGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
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
            da.StationOrganisation stationOrganisation = new da.StationOrganisation();
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
                { "StationID", masterID },
                { "OrganisationID", stationOrganisation.OrganisationID},
                { "OrganisationCode", stationOrganisation.Organisation.Code},
                { "RoleID", stationOrganisation.OrganisationRoleID },
                { "RoleCode", stationOrganisation.OrganisationRole.Code},
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

    protected void OrganisationLink(object sender, DirectEventArgs e)
    {
        string actionType = e.ExtraParams["type"];
        string recordID = e.ExtraParams["id"];
        try
        {
            if (actionType == "Edit")
            {
                OrganisationLinkFormPanel.SetValues(new da.StationOrganisation(recordID));
                OrganisationLinkWindow.Show();
            }
            else if (actionType == "Delete")
            {
                new da.StationOrganisationController().Delete(recordID);
                Auditing.Log("Station.DeleteOrganisationLink", new Dictionary<string, object> { { "ID", recordID } });
                OrganisationLinksGrid.DataBind();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Stations.OrganisationLink({ActionType},{RecordID})", actionType, recordID);
            MessageBoxes.Error(ex, "Unable to {0} organisation link", actionType);
        }
    }

    #endregion
}