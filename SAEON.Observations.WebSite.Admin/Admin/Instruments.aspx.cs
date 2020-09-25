using Ext.Net;
using SAEON.Logs;
using SAEON.Observations.Data;
using System;
using System.Configuration;
using System.Linq;

public partial class Admin_Instruments : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var showValidate = ConfigurationManager.AppSettings["ShowValidateButton"] == "true" && Request.IsLocal;
        btnValidate.Hidden = !showValidate;
        if (!X.IsAjaxRequest)
        {
            OrganisationStore.DataSource = new OrganisationCollection().OrderByAsc(Organisation.Columns.Name).Load();
            OrganisationStore.DataBind();
            OrganisationRoleStore.DataSource = new OrganisationRoleCollection().OrderByAsc(OrganisationRole.Columns.Name).Load();
            OrganisationRoleStore.DataBind();
            StationStore.DataSource = new StationCollection().OrderByAsc(Station.Columns.Name).Load();
            StationStore.DataBind();
            DataSourceStore.DataSource = new DataSourceCollection().OrderByAsc(DataSource.Columns.Name).Load();
            DataSourceStore.DataBind();
            SensorStore.DataSource = new SensorCollection().OrderByAsc(Sensor.Columns.Name).Load();
            SensorStore.DataBind();
        }
    }

    #region Instrument
    protected void InstrumentsGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        InstrumentsGrid.GetStore().DataSource = InstrumentRepository.GetPagedList(e, e.Parameters[GridFilters1.ParamPrefix]);
    }

    protected void ValidateField(object sender, RemoteValidationEventArgs e)
    {
        InstrumentCollection col = new InstrumentCollection();
        string checkColumn = String.Empty;
        string errorMessage = String.Empty;
        e.Success = true;
        tfCode.HasValue();
        tfName.HasValue();

        if (e.ID == "tfCode" || e.ID == "tfName")
        {
            if (e.ID == "tfCode")
            {
                checkColumn = Instrument.Columns.Code;
                errorMessage = "The specified Instrument Code already exists";
            }
            else if (e.ID == "tfName")
            {
                checkColumn = Instrument.Columns.Name;
                errorMessage = "The specified Instrument Name already exists";

            }

            if (tfID.IsEmpty)
                col = new InstrumentCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
            else
                col = new InstrumentCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(Instrument.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();

            if (col.Count > 0)
            {
                e.Success = false;
                e.ErrorMessage = errorMessage;
            }
        }
    }

    protected void Save(object sender, DirectEventArgs e)
    {
        using (SAEONLogs.MethodCall(GetType()))
        {
            try
            {
                Instrument instrument = new Instrument();

                if (!tfID.HasValue())
                    instrument.Id = Guid.NewGuid();
                else
                    instrument = new Instrument(tfID.Text);

                if (tfCode.HasValue())
                    instrument.Code = tfCode.Text;
                if (tfName.HasValue())
                    instrument.Name = tfName.Text;
                if (tfDescription.HasValue())
                    instrument.Description = tfDescription.Text;

                if (tfUrl.HasValue())
                    instrument.Url = tfUrl.Text;
                else
                    instrument.Url = null;

                if (nfLatitude.IsEmpty)
                    instrument.Latitude = null;
                else
                    instrument.Latitude = nfLatitude.Number;
                if (nfLongitude.IsEmpty)
                    instrument.Longitude = null;
                else
                    instrument.Longitude = nfLongitude.Number;
                if (nfElevation.IsEmpty)
                    instrument.Elevation = null;
                else
                    instrument.Elevation = nfElevation.Number;

                if (dfStartDate.HasValue())
                    instrument.StartDate = dfStartDate.SelectedDate;
                else
                    instrument.StartDate = null;
                if (dfEndDate.HasValue())
                    instrument.EndDate = dfEndDate.SelectedDate;
                else
                    instrument.EndDate = null;

                instrument.UserId = AuthHelper.GetLoggedInUserId;

                instrument.Save();
                Auditing.Log(GetType(), new MethodCallParameters {
                    { "ID", instrument.Id },
                    { "Code", instrument.Code },
                    { "Name", instrument.Name },
                    { "Latitude", instrument?.Latitude},
                    { "Longitude", instrument?.Longitude },
                    { "Elevation", instrument?.Elevation },
                    { "StartDate", instrument?.StartDate },
                    { "EndDate", instrument?.EndDate}
                });

                InstrumentsGrid.DataBind();

                DetailWindow.Hide();
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                MessageBoxes.Error(ex, "Error", "Unable to save instrument");
            }
        }
    }

    protected void InstrumentsGridStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();
        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        //string js = BaseRepository.BuildExportQ("Instrument", gridData, visCols, sortCol, sortDir);
        //BaseRepository.doExport(type, js);
        BaseRepository.Export("Instrument", gridData, visCols, sortCol, sortDir, type, "Instruments", Response);
    }

    #endregion

    #region Organisations
    protected void OrganisationLinksGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["InstrumentID"] != null && e.Parameters["InstrumentID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["InstrumentID"].ToString());
            VInstrumentOrganisationCollection col = new VInstrumentOrganisationCollection()
                .Where(VInstrumentOrganisation.Columns.InstrumentID, Id)
                .OrderByAsc(VStationOrganisation.Columns.Weight)
                .OrderByAsc(VInstrumentOrganisation.Columns.StartDate)
                .OrderByAsc(VInstrumentOrganisation.Columns.EndDate)
                .OrderByAsc(VInstrumentOrganisation.Columns.OrganisationName)
                .OrderByAsc(VInstrumentOrganisation.Columns.OrganisationRoleName)
                .Load();
            //VOrganisationInstrumentCollection col = new VOrganisationInstrumentCollection()
            //    .Where(VOrganisationInstrument.Columns.InstrumentID, Id)
            //    .OrderByAsc(VOrganisationInstrument.Columns.StartDate)
            //    .OrderByAsc(VOrganisationInstrument.Columns.EndDate)
            //    .OrderByAsc(VOrganisationInstrument.Columns.OrganisationName)
            //    .OrderByAsc(VOrganisationInstrument.Columns.OrganisationRoleName)
            //    .Load();
            OrganisationLinksGrid.GetStore().DataSource = col;
            OrganisationLinksGrid.GetStore().DataBind();
        }
    }

    private bool OrganisationLinkOk()
    {
        RowSelectionModel masterRow = InstrumentsGrid.SelectionModel.Primary as RowSelectionModel;
        var masterID = new Guid(masterRow.SelectedRecordID);
        OrganisationInstrumentCollection col = new OrganisationInstrumentCollection()
            .Where(OrganisationInstrument.Columns.InstrumentID, masterID)
            .Where(OrganisationInstrument.Columns.OrganisationID, cbOrganisation.SelectedItem.Value);
        if (!String.IsNullOrEmpty(dfOrganisationStartDate.Text) && (dfOrganisationStartDate.SelectedDate.Year >= 1900))
            col.Where(OrganisationInstrument.Columns.StartDate, dfOrganisationStartDate.SelectedDate);
        if (!String.IsNullOrEmpty(dfOrganisationEndDate.Text) && (dfOrganisationEndDate.SelectedDate.Year >= 1900))
            col.Where(OrganisationInstrument.Columns.EndDate, dfOrganisationEndDate.SelectedDate);
        col.Load();
        var id = Utilities.MakeGuid(OrganisationLinkID.Value);
        return !col.Any(i => i.Id != id);
    }

    protected void OrganisationLinkSave(object sender, DirectEventArgs e)
    {
        using (SAEONLogs.MethodCall(GetType()))
        {
            try
            {
                if (!OrganisationLinkOk())
                {
                    MessageBoxes.Error("Error", "Organisation is already linked");
                    return;
                }
                RowSelectionModel masterRow = InstrumentsGrid.SelectionModel.Primary as RowSelectionModel;
                var masterID = new Guid(masterRow.SelectedRecordID);
                OrganisationInstrument organisationInstrument = new OrganisationInstrument(Utilities.MakeGuid(OrganisationLinkID.Value))
                {
                    InstrumentID = masterID,
                    OrganisationID = new Guid(cbOrganisation.SelectedItem.Value.Trim()),
                    OrganisationRoleID = new Guid(cbOrganisationRole.SelectedItem.Value.Trim())
                };
                if (!String.IsNullOrEmpty(dfOrganisationStartDate.Text) && (dfOrganisationStartDate.SelectedDate.Year >= 1900))
                    organisationInstrument.StartDate = dfOrganisationStartDate.SelectedDate;
                else
                    organisationInstrument.StartDate = null;
                if (!String.IsNullOrEmpty(dfOrganisationEndDate.Text) && (dfOrganisationEndDate.SelectedDate.Year >= 1900))
                    organisationInstrument.EndDate = dfOrganisationEndDate.SelectedDate;
                else
                    organisationInstrument.EndDate = null;
                organisationInstrument.UserId = AuthHelper.GetLoggedInUserId;
                organisationInstrument.Save();
                Auditing.Log(GetType(), new MethodCallParameters {
                { "InstrumentID", organisationInstrument.InstrumentID },
                { "InstrumentCode", organisationInstrument.Instrument.Code },
                { "OrganisationID", organisationInstrument.OrganisationID},
                { "OrganisationCode", organisationInstrument.Organisation.Code},
                { "RoleID", organisationInstrument.OrganisationRoleID },
                { "RoleCode", organisationInstrument.OrganisationRole.Code},
                { "StartDate", organisationInstrument?.StartDate },
                { "EndDate", organisationInstrument?.EndDate}
            });
                OrganisationLinksGrid.DataBind();
                OrganisationLinkWindow.Hide();
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
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
        using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "ID", aID } }))
        {
            try
            {
                OrganisationInstrument.Delete(aID);
                Auditing.Log(GetType(), new MethodCallParameters { { "ID", aID } });
                OrganisationLinksGrid.DataBind();
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
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

    #region Stations
    protected void StationLinksGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        using (SAEONLogs.MethodCall(GetType()))
        {
            if (e.Parameters["InstrumentID"] != null && e.Parameters["InstrumentID"].ToString() != "-1")
            {
                Guid Id = Guid.Parse(e.Parameters["InstrumentID"].ToString());
                try
                {
                    VStationInstrumentCollection col = new VStationInstrumentCollection()
                        .Where(VStationInstrument.Columns.InstrumentID, Id)
                        .OrderByAsc(VStationInstrument.Columns.StartDate)
                        .OrderByAsc(VStationInstrument.Columns.EndDate)
                        .OrderByAsc(VStationInstrument.Columns.StationName)
                        .Load();
                    StationLinksGrid.GetStore().DataSource = col;
                    StationLinksGrid.GetStore().DataBind();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    MessageBoxes.Error(ex, "Error", "Unable to refresh sensors grid");
                }
            }
        }
    }

    private bool StationLinkOk()
    {
        RowSelectionModel masterRow = InstrumentsGrid.SelectionModel.Primary as RowSelectionModel;
        var masterID = new Guid(masterRow.SelectedRecordID);
        StationInstrumentCollection col = new StationInstrumentCollection()
            .Where(StationInstrument.Columns.InstrumentID, masterID)
            .Where(StationInstrument.Columns.StationID, cbStation.SelectedItem.Value);
        if (!dfStationStartDate.IsEmpty && (dfStationStartDate.SelectedDate.Year >= 1900))
            col.Where(StationInstrument.Columns.StartDate, dfStationStartDate.SelectedDate);
        if (!dfStationEndDate.IsEmpty && (dfStationEndDate.SelectedDate.Year >= 1900))
            col.Where(StationInstrument.Columns.EndDate, dfStationEndDate.SelectedDate);
        col.Load();
        var id = Utilities.MakeGuid(StationLinkID.Value);
        return !col.Any(i => i.Id != id);
    }

    protected void StationLinkSave(object sender, DirectEventArgs e)
    {
        using (SAEONLogs.MethodCall(GetType()))
        {
            try
            {
                if (!StationLinkOk())
                {
                    MessageBoxes.Error("Error", "Station is already linked");
                    return;
                }
                RowSelectionModel masterRow = InstrumentsGrid.SelectionModel.Primary as RowSelectionModel;
                var masterID = new Guid(masterRow.SelectedRecordID);
                StationInstrument stationInstrument = new StationInstrument(Utilities.MakeGuid(StationLinkID.Value))
                {
                    InstrumentID = masterID,
                    StationID = new Guid(cbStation.SelectedItem.Value.Trim())
                };
                if (nfStationLatitude.IsEmpty)
                    stationInstrument.Latitude = null;
                else
                    stationInstrument.Latitude = nfStationLatitude.Number;
                if (nfStationLongitude.IsEmpty)
                    stationInstrument.Longitude = null;
                else
                    stationInstrument.Longitude = nfStationLongitude.Number;
                if (nfStationElevation.IsEmpty)
                    stationInstrument.Elevation = null;
                else
                    stationInstrument.Elevation = nfStationElevation.Number;
                if (!dfStationStartDate.IsEmpty && (dfStationStartDate.SelectedDate.Year >= 1900))
                    stationInstrument.StartDate = dfStationStartDate.SelectedDate;
                else
                    stationInstrument.StartDate = null;
                if (!dfStationEndDate.IsEmpty && (dfStationEndDate.SelectedDate.Year >= 1900))
                    stationInstrument.EndDate = dfStationEndDate.SelectedDate;
                else
                    stationInstrument.EndDate = null;
                stationInstrument.UserId = AuthHelper.GetLoggedInUserId;
                stationInstrument.Save();
                Auditing.Log(GetType(), new MethodCallParameters {
                { "InstrumentID", stationInstrument.InstrumentID },
                { "InstrumentCode", stationInstrument.Instrument.Code },
                { "StationID", stationInstrument.StationID},
                { "StationCode", stationInstrument.Station.Code},
                { "Latitude", stationInstrument?.Latitude},
                { "Longitude", stationInstrument?.Longitude },
                { "Elevation", stationInstrument?.Elevation },
                { "StartDate", stationInstrument?.StartDate },
                { "EndDate", stationInstrument?.EndDate}
            });
                StationLinksGrid.DataBind();
                StationLinkWindow.Hide();
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                MessageBoxes.Error(ex, "Error", "Unable to link station");
            }
        }
    }

    [DirectMethod]
    public void ConfirmDeleteStationLink(Guid aID)
    {
        MessageBoxes.Confirm(
            "Confirm Delete",
            String.Format("DirectCall.DeleteStationLink(\"{0}\",{{ eventMask: {{ showMask: true}}}});", aID.ToString()),
            "Are you sure you want to delete this station link?");
    }

    [DirectMethod]
    public void DeleteStationLink(Guid aID)
    {
        using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "ID", aID } }))
        {
            try
            {
                StationInstrument.Delete(aID);
                Auditing.Log(GetType(), new MethodCallParameters { { "ID", aID } });
                StationLinksGrid.DataBind();
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                MessageBoxes.Error(ex, "Error", "Unable to delete station link");
            }
        }
    }

    [DirectMethod]
    public void AddStationClick(object sender, DirectEventArgs e)
    {
        //X.Redirect(X.ResourceManager.ResolveUrl("Admin/Sites"));
    }
    #endregion


    #region Sensors
    protected void SensorLinksGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        using (SAEONLogs.MethodCall(GetType()))
        {
            if (e.Parameters["InstrumentID"] != null && e.Parameters["InstrumentID"].ToString() != "-1")
            {
                Guid Id = Guid.Parse(e.Parameters["InstrumentID"].ToString());
                try
                {
                    VInstrumentSensorCollection col = new VInstrumentSensorCollection()
                        .Where(VInstrumentSensor.Columns.InstrumentID, Id)
                        .OrderByAsc(VInstrumentSensor.Columns.StartDate)
                        .OrderByAsc(VInstrumentSensor.Columns.EndDate)
                        .OrderByAsc(VInstrumentSensor.Columns.SensorName)
                        .Load();
                    SensorLinksGrid.GetStore().DataSource = col;
                    SensorLinksGrid.GetStore().DataBind();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    MessageBoxes.Error(ex, "Error", "Unable to refresh sensors grid");
                }
            }
        }
    }

    private bool SensorLinkOk()
    {
        RowSelectionModel masterRow = InstrumentsGrid.SelectionModel.Primary as RowSelectionModel;
        var masterID = new Guid(masterRow.SelectedRecordID);
        InstrumentSensorCollection col = new InstrumentSensorCollection()
            .Where(InstrumentSensor.Columns.InstrumentID, masterID)
            .Where(InstrumentSensor.Columns.SensorID, cbSensor.SelectedItem.Value);
        if (!String.IsNullOrEmpty(dfSensorStartDate.Text) && (dfSensorStartDate.SelectedDate.Year >= 1900))
            col.Where(InstrumentSensor.Columns.StartDate, dfSensorStartDate.SelectedDate);
        if (!String.IsNullOrEmpty(dfSensorEndDate.Text) && (dfSensorEndDate.SelectedDate.Year >= 1900))
            col.Where(InstrumentSensor.Columns.EndDate, dfSensorEndDate.SelectedDate);
        col.Load();
        var id = Utilities.MakeGuid(SensorLinkID.Value);
        return !col.Any(i => i.Id != id);
    }

    protected void SensorLinkSave(object sender, DirectEventArgs e)
    {
        using (SAEONLogs.MethodCall(GetType()))
        {
            try
            {
                if (!SensorLinkOk())
                {
                    MessageBoxes.Error("Error", "Sensor is already linked");
                    return;
                }

                RowSelectionModel masterRow = InstrumentsGrid.SelectionModel.Primary as RowSelectionModel;
                var masterID = new Guid(masterRow.SelectedRecordID);
                InstrumentSensor instrumentSensor = new InstrumentSensor(Utilities.MakeGuid(SensorLinkID.Value))
                {
                    InstrumentID = masterID,
                    SensorID = new Guid(cbSensor.SelectedItem.Value.Trim())
                };
                if (dfSensorStartDate.IsEmpty || (dfSensorStartDate.SelectedDate.Year < 1900))
                    instrumentSensor.StartDate = null;
                else
                {
                    instrumentSensor.StartDate = DateTime.Parse(dfSensorStartDate.RawText);
                }
                if (dfSensorEndDate.IsEmpty || (dfSensorEndDate.SelectedDate.Year < 1900))
                    instrumentSensor.EndDate = null;
                else
                {
                    instrumentSensor.EndDate = DateTime.Parse(dfSensorEndDate.RawText);
                }
                instrumentSensor.UserId = AuthHelper.GetLoggedInUserId;
                instrumentSensor.Save();
                Auditing.Log(GetType(), new MethodCallParameters {
                { "InstrumentID", instrumentSensor.InstrumentID },
                { "InstrumentCode", instrumentSensor.Instrument.Code },
                { "SensorID", instrumentSensor.SensorID},
                { "SensorCode", instrumentSensor.Sensor.Code},
                { "StartDate", instrumentSensor?.StartDate },
                { "EndDate", instrumentSensor?.EndDate}
            });
                SensorLinksGrid.DataBind();
                SensorLinkWindow.Hide();
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                MessageBoxes.Error(ex, "Error", "Unable to link sensor");
            }
        }
    }

    [DirectMethod]
    public void ConfirmDeleteSensorLink(Guid aID)
    {
        MessageBoxes.Confirm(
            "Confirm Delete",
            String.Format("DirectCall.DeleteSensorLink(\"{0}\",{{ eventMask: {{ showMask: true}}}});", aID.ToString()),
            "Are you sure you want to delete this sensor link?");
    }

    [DirectMethod]
    public void DeleteSensorLink(Guid aID)
    {
        using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "ID", aID } }))
        {
            try
            {
                InstrumentSensor.Delete(aID);
                Auditing.Log(GetType(), new MethodCallParameters { { "ID", aID } });
                SensorLinksGrid.DataBind();
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                MessageBoxes.Error(ex, "Error", "Unable to delete sensor link");
            }
        }
    }

    [DirectMethod]
    public void AddSensorClick(object sender, DirectEventArgs e)
    {
        //X.Redirect(X.ResourceManager.ResolveUrl("Admin/Sites"));
    }
    #endregion
}