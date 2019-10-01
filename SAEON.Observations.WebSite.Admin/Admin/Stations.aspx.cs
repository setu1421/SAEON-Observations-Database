using Ext.Net;
using SAEON.Logs;
using SAEON.Observations.Data;
using System;
using System.Linq;

public partial class Admin_Stations : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!X.IsAjaxRequest)
        {
            SiteStore.DataSource = new SiteCollection().OrderByAsc(SAEON.Observations.Data.Site.Columns.Name).Load();
            SiteStore.DataBind();
            OrganisationStore.DataSource = new OrganisationCollection().OrderByAsc(Organisation.Columns.Name).Load();
            OrganisationStore.DataBind();
            OrganisationRoleStore.DataSource = new OrganisationRoleCollection().OrderByAsc(OrganisationRole.Columns.Name).Load();
            OrganisationRoleStore.DataBind();
            ProjectStore.DataSource = new ProjectCollection().OrderByAsc(Project.Columns.Name).Load();
            ProjectStore.DataBind();
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
        string checkColumn = String.Empty;
        string errorMessage = String.Empty;
        e.Success = true;
        tfCode.HasValue();
        tfName.HasValue();
        tfDescription.HasValue();
        if (e.ID == "tfCode" || e.ID == "tfName")
        {
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

            if (tfID.IsEmpty)
                col = new StationCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
            else
                col = new StationCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(Station.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();

            if (col.Count > 0)
            {
                e.Success = false;
                e.ErrorMessage = errorMessage;
            }
        }
    }
    protected void Save(object sender, DirectEventArgs e)
    {
        using (Logging.MethodCall(GetType()))
        {
            try
            {
                Station station = new Station();

                if (!tfID.HasValue())
                    station.Id = Guid.NewGuid();
                else
                    station = new Station(tfID.Text);

                if (tfCode.HasValue())
                    station.Code = tfCode.Text;
                if (tfName.HasValue())
                    station.Name = tfName.Text;
                station.SiteID = Utilities.MakeGuid(cbSite.SelectedItem.Value.Trim());
                if (tfDescription.HasValue())
                    station.Description = tfDescription.Text;

                //if (!string.IsNullOrEmpty(nfLatitude.Text))
                //    station.Latitude = double.Parse(nfLatitude.Text);

                //if (!string.IsNullOrEmpty(nfLongitude.Text))
                //    station.Longitude = double.Parse(nfLongitude.Text);

                //if (!string.IsNullOrEmpty(nfElevation.Text))
                //    station.Elevation = Int32.Parse(nfElevation.Text);

                if (nfLatitude.IsEmpty)
                    station.Latitude = null;
                else
                    station.Latitude = nfLatitude.Number;
                if (nfLongitude.IsEmpty)
                    station.Longitude = null;
                else
                    station.Longitude = nfLongitude.Number;
                if (nfElevation.IsEmpty)
                    station.Elevation = null;
                else
                    station.Elevation = nfElevation.Number;

                if (tfUrl.HasValue())
                    station.Url = tfUrl.Text;
                else
                    station.Url = null;

                if (dfStartDate.HasValue())
                    station.StartDate = dfStartDate.SelectedDate;
                else
                    station.StartDate = null;
                if (dfEndDate.HasValue())
                    station.EndDate = dfEndDate.SelectedDate;
                else
                    station.EndDate = null;
                station.UserId = AuthHelper.GetLoggedInUserId;

                station.Save();
                Auditing.Log(GetType(), new ParameterList {
                { "ID", station.Id }, { "Code", station.Code }, { "Name", station.Name } });

                StationsGrid.DataBind();

                DetailWindow.Hide();
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
                MessageBoxes.Error(ex, "Error", "Unable to save station");
            }
        }
    }

    protected void StationsGridStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();
        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        //string js = BaseRepository.BuildExportQ("VStation", gridData, visCols, sortCol, sortDir);
        //BaseRepository.doExport(type, js);
        BaseRepository.Export("vStation", gridData, visCols, sortCol, sortDir, type, "Stations", Response);
    }

    public void AddStationClick(object sender, DirectEventArgs e)
    {
        SiteStore.DataSource = new SiteCollection().OrderByAsc(SAEON.Observations.Data.Site.Columns.Name).Load();
        SiteStore.DataBind();
    }
    public void StationGridCommand(object sender, DirectEventArgs e)
    {
        SiteStore.DataSource = new SiteCollection().OrderByAsc(SAEON.Observations.Data.Site.Columns.Name).Load();
        SiteStore.DataBind();
    }
    #endregion

    #region Organisations

    protected void OrganisationLinksGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["StationID"] != null && e.Parameters["StationID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["StationID"].ToString());
            VStationOrganisationCollection col = new VStationOrganisationCollection()
                .Where(VStationOrganisation.Columns.StationID, Id)
                .OrderByAsc(VStationOrganisation.Columns.Weight)
                .OrderByAsc(VStationOrganisation.Columns.StartDate)
                .OrderByAsc(VStationOrganisation.Columns.EndDate)
                .OrderByAsc(VStationOrganisation.Columns.OrganisationName)
                .OrderByAsc(VStationOrganisation.Columns.OrganisationRoleName)
                .Load();
            //VOrganisationStationCollection OrganisationStationCol = new VOrganisationStationCollection()
            //    .Where(VOrganisationStation.Columns.StationID, Id)
            //    .OrderByAsc(VOrganisationStation.Columns.StartDate)
            //    .OrderByAsc(VOrganisationStation.Columns.EndDate)
            //    .OrderByAsc(VOrganisationStation.Columns.OrganisationName)
            //    .OrderByAsc(VOrganisationStation.Columns.OrganisationRoleName)
            //    .Load();
            OrganisationLinksGrid.GetStore().DataSource = col;
            OrganisationLinksGrid.GetStore().DataBind();
        }
    }

    private bool OrganisationLinkOk()
    {
        RowSelectionModel masterRow = StationsGrid.SelectionModel.Primary as RowSelectionModel;
        var masterID = new Guid(masterRow.SelectedRecordID);
        OrganisationStationCollection col = new OrganisationStationCollection()
            .Where(OrganisationStation.Columns.StationID, masterID)
            .Where(OrganisationStation.Columns.OrganisationID, cbOrganisation.SelectedItem.Value);
        if (!String.IsNullOrEmpty(dfOrganisationStartDate.Text) && (dfOrganisationStartDate.SelectedDate.Year >= 1900))
            col.Where(OrganisationStation.Columns.StartDate, dfOrganisationStartDate.SelectedDate);
        if (!String.IsNullOrEmpty(dfOrganisationEndDate.Text) && (dfOrganisationEndDate.SelectedDate.Year >= 1900))
            col.Where(OrganisationStation.Columns.EndDate, dfOrganisationEndDate.SelectedDate);
        col.Load();
        var id = Utilities.MakeGuid(OrganisationLinkID.Value);
        return !col.Any(i => i.Id != id);
    }

    protected void OrganisationLinkSave(object sender, DirectEventArgs e)
    {
        using (Logging.MethodCall(GetType()))
        {
            try
            {
                if (!OrganisationLinkOk())
                {
                    MessageBoxes.Error("Error", "Organisation is already linked");
                    return;
                }
                RowSelectionModel masterRow = StationsGrid.SelectionModel.Primary as RowSelectionModel;
                var masterID = new Guid(masterRow.SelectedRecordID);
                OrganisationStation stationOrganisation = new OrganisationStation(Utilities.MakeGuid(OrganisationLinkID.Value))
                {
                    StationID = masterID,
                    OrganisationID = new Guid(cbOrganisation.SelectedItem.Value.Trim()),
                    OrganisationRoleID = new Guid(cbOrganisationRole.SelectedItem.Value.Trim())
                };
                if (!String.IsNullOrEmpty(dfOrganisationStartDate.Text) && (dfOrganisationStartDate.SelectedDate.Year >= 1900))
                    stationOrganisation.StartDate = dfOrganisationStartDate.SelectedDate;
                if (!String.IsNullOrEmpty(dfOrganisationEndDate.Text) && (dfOrganisationEndDate.SelectedDate.Year >= 1900))
                    stationOrganisation.EndDate = dfOrganisationEndDate.SelectedDate;
                stationOrganisation.UserId = AuthHelper.GetLoggedInUserId;
                stationOrganisation.Save();
                Auditing.Log(GetType(), new ParameterList {
                { "StationID", stationOrganisation.StationID },
                { "StationCode", stationOrganisation.Station.Code },
                { "OrganisationID", stationOrganisation.OrganisationID},
                { "OrganisationCode", stationOrganisation.Organisation.Code},
                { "RoleID", stationOrganisation.OrganisationRoleID },
                { "RoleCode", stationOrganisation.OrganisationRole.Code},
                { "StartDate", stationOrganisation?.StartDate },
                { "EndDate", stationOrganisation?.EndDate}
            });
                OrganisationLinksGrid.DataBind();
                OrganisationLinkWindow.Hide();
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
                MessageBoxes.Error(ex, "Error", "Unable to link organisation");
            }
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
        using (Logging.MethodCall(GetType(), new ParameterList { { "ID", aID } }))
        {
            try
            {
                OrganisationStation.Delete(aID);
                Auditing.Log(GetType(), new ParameterList { { "ID", aID } });
                OrganisationLinksGrid.DataBind();
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
                MessageBoxes.Error(ex, "Error", "Unable to delete organisation link");
            }
        }
    }

    [DirectMethod]
    public void AddOrganisationClick(object sender, DirectEventArgs e)
    {
        //X.Redirect(X.ResourceManager.ResolveUrl("Admin/Sites"));
    }
    #endregion

    #region Projects

    protected void ProjectLinksGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["StationID"] != null && e.Parameters["StationID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["StationID"].ToString());
            VProjectStationCollection col = new VProjectStationCollection()
                .Where(VProjectStation.Columns.StationID, Id)
                .OrderByAsc(VProjectStation.Columns.StartDate)
                .OrderByAsc(VProjectStation.Columns.EndDate)
                .OrderByAsc(VProjectStation.Columns.ProjectName)
                .Load();
            ProjectLinksGrid.GetStore().DataSource = col;
            ProjectLinksGrid.GetStore().DataBind();
        }
    }

    private bool ProjectLinkOk()
    {
        RowSelectionModel masterRow = StationsGrid.SelectionModel.Primary as RowSelectionModel;
        var masterID = new Guid(masterRow.SelectedRecordID);
        ProjectStationCollection col = new ProjectStationCollection()
            .Where(ProjectStation.Columns.StationID, masterID)
            .Where(ProjectStation.Columns.ProjectID, cbProject.SelectedItem.Value);
        if (!String.IsNullOrEmpty(dfProjectStartDate.Text) && (dfProjectStartDate.SelectedDate.Year >= 1900))
            col.Where(ProjectStation.Columns.StartDate, dfProjectStartDate.SelectedDate);
        if (!String.IsNullOrEmpty(dfProjectEndDate.Text) && (dfProjectEndDate.SelectedDate.Year >= 1900))
            col.Where(ProjectStation.Columns.EndDate, dfProjectEndDate.SelectedDate);
        col.Load();
        var id = Utilities.MakeGuid(ProjectLinkID.Value);
        return !col.Any(i => i.Id != id);
    }

    protected void ProjectLinkSave(object sender, DirectEventArgs e)
    {
        using (Logging.MethodCall(GetType()))
        {
            try
            {
                if (!ProjectLinkOk())
                {
                    MessageBoxes.Error("Error", "Project is already linked");
                    return;
                }
                RowSelectionModel masterRow = StationsGrid.SelectionModel.Primary as RowSelectionModel;
                var masterID = new Guid(masterRow.SelectedRecordID);
                ProjectStation projectStation = new ProjectStation(Utilities.MakeGuid(ProjectLinkID.Value))
                {
                    StationID = masterID,
                    ProjectID = new Guid(cbProject.SelectedItem.Value.Trim())
                };
                if (!String.IsNullOrEmpty(dfProjectStartDate.Text) && (dfProjectStartDate.SelectedDate.Year >= 1900))
                    projectStation.StartDate = dfProjectStartDate.SelectedDate;
                else
                    projectStation.StartDate = null;
                if (!String.IsNullOrEmpty(dfProjectEndDate.Text) && (dfProjectEndDate.SelectedDate.Year >= 1900))
                    projectStation.EndDate = dfProjectEndDate.SelectedDate;
                else
                    projectStation.EndDate = null;
                projectStation.UserId = AuthHelper.GetLoggedInUserId;
                projectStation.Save();
                Auditing.Log(GetType(), new ParameterList {
                { "StationID", projectStation.StationID },
                { "StationCode", projectStation.Station.Code },
                { "ProjectID", projectStation.ProjectID},
                { "ProjectCode", projectStation.Project.Code},
                { "StartDate", projectStation?.StartDate },
                { "EndDate", projectStation?.EndDate}
            });
                ProjectLinksGrid.DataBind();
                ProjectLinkWindow.Hide();
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
                MessageBoxes.Error(ex, "Error", "Unable to link Project");
            }
        }
    }

    [DirectMethod]
    public void ConfirmDeleteProjectLink(Guid aID)
    {
        MessageBoxes.Confirm(
            "Confirm Delete",
            String.Format("DirectCall.DeleteProjectLink(\"{0}\",{{ eventMask: {{ showMask: true}}}});", aID.ToString()),
            "Are you sure you want to delete this Project link?");
    }

    [DirectMethod]
    public void DeleteProjectLink(Guid aID)
    {
        using (Logging.MethodCall(GetType(), new ParameterList { { "ID", aID } }))
        {
            try
            {
                ProjectStation.Delete(aID);
                Auditing.Log(GetType(), new ParameterList { { "ID", aID } });
                ProjectLinksGrid.DataBind();
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
                MessageBoxes.Error(ex, "Error", "Unable to delete Project link");
            }
        }
    }

    [DirectMethod]
    public void AddProjectClick(object sender, DirectEventArgs e)
    {
        //X.Redirect(X.ResourceManager.ResolveUrl("Admin/Stations"));
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
                .OrderByAsc(VStationInstrument.Columns.StartDate)
                .OrderByAsc(VStationInstrument.Columns.EndDate)
                .OrderByAsc(VStationInstrument.Columns.InstrumentName)
                .Load();
            InstrumentLinksGrid.GetStore().DataSource = col;
            InstrumentLinksGrid.GetStore().DataBind();
        }
    }

    private bool InstrumentLinkOk()
    {
        RowSelectionModel masterRow = StationsGrid.SelectionModel.Primary as RowSelectionModel;
        var masterID = new Guid(masterRow.SelectedRecordID);
        StationInstrumentCollection col = new StationInstrumentCollection()
            .Where(StationInstrument.Columns.StationID, masterID)
            .Where(StationInstrument.Columns.InstrumentID, cbInstrument.SelectedItem.Value);
        if (!dfInstrumentStartDate.IsEmpty && (dfInstrumentStartDate.SelectedDate.Year >= 1900))
            col.Where(StationInstrument.Columns.StartDate, dfInstrumentStartDate.SelectedDate);
        if (!dfInstrumentEndDate.IsEmpty && (dfInstrumentEndDate.SelectedDate.Year >= 1900))
            col.Where(StationInstrument.Columns.EndDate, dfInstrumentEndDate.SelectedDate);
        col.Load();
        var id = Utilities.MakeGuid(InstrumentLinkID.Value);
        return !col.Any(i => i.Id != id);
    }

    protected void InstrumentLinkSave(object sender, DirectEventArgs e)
    {
        using (Logging.MethodCall(GetType()))
        {
            try
            {
                if (!InstrumentLinkOk())
                {
                    MessageBoxes.Error("Error", "Instrument is already linked");
                    return;
                }
                RowSelectionModel masterRow = StationsGrid.SelectionModel.Primary as RowSelectionModel;
                var masterID = new Guid(masterRow.SelectedRecordID);
                StationInstrument stationInstrument = new StationInstrument(Utilities.MakeGuid(InstrumentLinkID.Value))
                {
                    StationID = masterID,
                    InstrumentID = new Guid(cbInstrument.SelectedItem.Value.Trim())
                };
                if (nfInstrumentLatitude.IsEmpty)
                    stationInstrument.Latitude = null;
                else
                    stationInstrument.Latitude = nfInstrumentLatitude.Number;
                if (nfInstrumentLongitude.IsEmpty)
                    stationInstrument.Longitude = null;
                else
                    stationInstrument.Longitude = nfInstrumentLongitude.Number;
                if (nfInstrumentElevation.IsEmpty)
                    stationInstrument.Elevation = null;
                else
                    stationInstrument.Elevation = nfInstrumentElevation.Number;
                if (!dfInstrumentStartDate.IsEmpty && (dfInstrumentStartDate.SelectedDate.Year >= 1900))
                    stationInstrument.StartDate = dfInstrumentStartDate.SelectedDate;
                else
                    stationInstrument.StartDate = null;
                if (!dfInstrumentEndDate.IsEmpty && (dfInstrumentEndDate.SelectedDate.Year >= 1900))
                    stationInstrument.EndDate = dfInstrumentEndDate.SelectedDate;
                else
                    stationInstrument.EndDate = null;
                stationInstrument.UserId = AuthHelper.GetLoggedInUserId;
                stationInstrument.Save();
                Auditing.Log(GetType(), new ParameterList {
                { "StationID", stationInstrument.StationID },
                { "StationCode", stationInstrument.Station.Code },
                { "InstrumentID", stationInstrument.InstrumentID},
                { "InstrumentCode", stationInstrument.Instrument.Code},
                { "Latitude", stationInstrument?.Latitude},
                { "Longitude", stationInstrument?.Longitude },
                { "Elevation", stationInstrument?.Elevation },
                { "StartDate", stationInstrument?.StartDate },
                { "EndDate", stationInstrument?.EndDate}
            });
                InstrumentLinksGrid.DataBind();
                InstrumentLinkWindow.Hide();
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
                MessageBoxes.Error(ex, "Error", "Unable to link instrument");
            }
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
        using (Logging.MethodCall(GetType(), new ParameterList { { "ID", aID } }))
        {
            try
            {
                StationInstrument.Delete(aID);
                Auditing.Log(GetType(), new ParameterList { { "ID", aID } });
                InstrumentLinksGrid.DataBind();
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
                MessageBoxes.Error(ex, "Error", "Unable to delete instrument link");
            }
        }
    }

    [DirectMethod]
    public void AddInstrumentClick(object sender, DirectEventArgs e)
    {
        //X.Redirect(X.ResourceManager.ResolveUrl("Admin/Sites"));
    }
    #endregion

}