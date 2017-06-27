using Ext.Net;
using SAEON.Observations.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Admin_Instruments : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
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

    //protected void MasterRowSelect(object sender, DirectEventArgs e)
    //{
    //    RowSelectionModel masterRow = InstrumentsGrid.SelectionModel.Primary as RowSelectionModel;
    //    var masterID = new Guid(masterRow.SelectedRecordID);
    //    SensorCollection sensors = new Select()
    //        .From(Sensor.Schema)
    //        .Where(Sensor.StationIDColumn)
    //        .In(new Select(new string[] { StationInstrument.Columns.StationID })
    //            .From(StationInstrument.Schema)
    //            .Where(StationInstrument.InstrumentIDColumn)
    //            .IsEqualTo(masterID))
    //        .Or(Sensor.StationIDColumn)
    //        .IsNull()
    //        .OrderAsc(Sensor.Columns.Name)
    //        .ExecuteAsCollection<SensorCollection>();
    //    SensorStore.DataSource = sensors;
    //    SensorStore.DataBind();
    //}


    protected void ValidateField(object sender, RemoteValidationEventArgs e)
    {
        InstrumentCollection col = new InstrumentCollection();

        string checkColumn = String.Empty,
               errorMessage = String.Empty;

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

        if (!string.IsNullOrEmpty(checkColumn))
            if (String.IsNullOrEmpty(tfID.Text.ToString()))
                col = new InstrumentCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
            else
                col = new InstrumentCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(Instrument.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();

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
            Instrument instrument = new Instrument();

            if (String.IsNullOrEmpty(tfID.Text))
                instrument.Id = Guid.NewGuid();
            else
                instrument = new Instrument(tfID.Text.Trim());

            if (!string.IsNullOrEmpty(tfCode.Text.Trim()))
                instrument.Code = tfCode.Text.Trim();
            if (!string.IsNullOrEmpty(tfName.Text.Trim()))
                instrument.Name = tfName.Text.Trim();
            instrument.Description = tfDescription.Text.Trim();

            if (!string.IsNullOrEmpty(tfUrl.Text))
                instrument.Url = tfUrl.Text;

            if (!String.IsNullOrEmpty(dfStartDate.Text) && (dfStartDate.SelectedDate.Year >= 1900))
                instrument.StartDate = dfStartDate.SelectedDate;
            else
                instrument.StartDate = null;
            if (!String.IsNullOrEmpty(dfEndDate.Text) && (dfEndDate.SelectedDate.Year >= 1900))
                instrument.EndDate = dfEndDate.SelectedDate;
            else
                instrument.EndDate = null;

            instrument.UserId = AuthHelper.GetLoggedInUserId;

            instrument.Save();
            Auditing.Log("Instruments.Save", new Dictionary<string, object> {
                { "ID", instrument.Id }, { "Code", instrument.Code }, { "Name", instrument.Name } });

            InstrumentsGrid.DataBind();

            DetailWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Instruments.Save");
            MessageBoxes.Error(ex, "Error", "Unable to save instrument");
        }
    }

    protected void InstrumentsGridStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();
        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        string js = BaseRepository.BuildExportQ("Instrument", gridData, visCols, sortCol, sortDir);

        BaseRepository.doExport(type, js);
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
        return !col.Any();
    }

    protected void OrganisationLinkSave(object sender, DirectEventArgs e)
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
            OrganisationInstrument organisationInstrument = new OrganisationInstrument(Utilities.MakeGuid(OrganisationLinkID.Value));
            organisationInstrument.InstrumentID = masterID;
            organisationInstrument.OrganisationID = new Guid(cbOrganisation.SelectedItem.Value.Trim());
            organisationInstrument.OrganisationRoleID = new Guid(cbOrganisationRole.SelectedItem.Value.Trim());
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
            Auditing.Log("Instruments.AddOrganisationLink", new Dictionary<string, object> {
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
            Log.Error(ex, "Instruments.LinkOrganisation_Click");
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
            OrganisationInstrument.Delete(aID);
            Auditing.Log("Instruments.DeleteOrganisationLink", new Dictionary<string, object> { { "ID", aID } });
            OrganisationLinksGrid.DataBind();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Instruments.DeleteOrganisationLink({aID})", aID);
            MessageBoxes.Error(ex, "Error", "Unable to delete organisation link");
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
                Log.Error(ex, "Instruments.StationLinksGridStore_RefreshData");
                MessageBoxes.Error(ex, "Error", "Unable to refresh sensors grid");
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
        if (!String.IsNullOrEmpty(dfStationStartDate.Text) && (dfStationStartDate.SelectedDate.Year >= 1900))
            col.Where(StationInstrument.Columns.StartDate, dfStationStartDate.SelectedDate);
        if (!String.IsNullOrEmpty(dfStationEndDate.Text) && (dfStationEndDate.SelectedDate.Year >= 1900))
            col.Where(StationInstrument.Columns.EndDate, dfStationEndDate.SelectedDate);
        col.Load();
        return !col.Any();
    }

    protected void StationLinkSave(object sender, DirectEventArgs e)
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
            StationInstrument stationInstrument = new StationInstrument(Utilities.MakeGuid(StationLinkID.Value));
            stationInstrument.InstrumentID = masterID;
            stationInstrument.StationID = new Guid(cbStation.SelectedItem.Value.Trim());
            if (!String.IsNullOrEmpty(dfStationStartDate.Text) && (dfStationStartDate.SelectedDate.Year >= 1900))
                stationInstrument.StartDate = dfStationStartDate.SelectedDate;
            else
                stationInstrument.StartDate = null;
            if (!String.IsNullOrEmpty(dfStationEndDate.Text) && (dfStationEndDate.SelectedDate.Year >= 1900))
                stationInstrument.EndDate = dfStationEndDate.SelectedDate;
            else
                stationInstrument.EndDate = null;
            stationInstrument.UserId = AuthHelper.GetLoggedInUserId;
            stationInstrument.Save();
            Auditing.Log("Instruments.AddStationLink", new Dictionary<string, object> {
                { "InstrumentID", stationInstrument.InstrumentID },
                { "InstrumentCode", stationInstrument.Instrument.Code },
                { "StationID", stationInstrument.StationID},
                { "StationCode", stationInstrument.Station.Code},
                { "StartDate", stationInstrument?.StartDate },
                { "EndDate", stationInstrument?.EndDate}
            });
            StationLinksGrid.DataBind();
            StationLinkWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Instruments.LinkStation_Click");
            MessageBoxes.Error(ex, "Error", "Unable to link station");
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
        try
        {
            StationInstrument.Delete(aID);
            Auditing.Log("Instruments.DeleteStationLink", new Dictionary<string, object> { { "ID", aID } });
            StationLinksGrid.DataBind();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Instruments.DeleteStationLink({aID})", aID);
            MessageBoxes.Error(ex, "Error", "Unable to delete station link");
        }
    }

    [DirectMethod]
    public void AddStationClick(object sender, DirectEventArgs e)
    {
        //X.Redirect(X.ResourceManager.ResolveUrl("Admin/Sites"));
    }
    #endregion

    #region DataSources
    protected void DataSourceLinksGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["InstrumentID"] != null && e.Parameters["InstrumentID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["InstrumentID"].ToString());
            try
            {
                VInstrumentDataSourceCollection col = new VInstrumentDataSourceCollection()
                    .Where(VInstrumentDataSource.Columns.InstrumentID, Id)
                    .OrderByAsc(VInstrumentDataSource.Columns.StartDate)
                    .OrderByAsc(VInstrumentDataSource.Columns.EndDate)
                    .OrderByAsc(VInstrumentDataSource.Columns.DataSourceName)
                    .Load();
                DataSourceLinksGrid.GetStore().DataSource = col;
                DataSourceLinksGrid.GetStore().DataBind();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Instruments.DataSourceLinksGridStore_RefreshData");
                MessageBoxes.Error(ex, "Error", "Unable to refresh DataSources grid");
            }
        }
    }

    private bool DataSourceLinkOk()
    {
        RowSelectionModel masterRow = InstrumentsGrid.SelectionModel.Primary as RowSelectionModel;
        var masterID = new Guid(masterRow.SelectedRecordID);
        InstrumentDataSourceCollection col = new InstrumentDataSourceCollection()
            .Where(InstrumentDataSource.Columns.InstrumentID, masterID)
            .Where(InstrumentDataSource.Columns.DataSourceID, cbDataSource.SelectedItem.Value);
        if (!String.IsNullOrEmpty(dfDataSourceStartDate.Text) && (dfDataSourceStartDate.SelectedDate.Year >= 1900))
            col.Where(InstrumentDataSource.Columns.StartDate, dfDataSourceStartDate.SelectedDate);
        if (!String.IsNullOrEmpty(dfDataSourceEndDate.Text) && (dfDataSourceEndDate.SelectedDate.Year >= 1900))
            col.Where(InstrumentDataSource.Columns.EndDate, dfDataSourceEndDate.SelectedDate);
        col.Load();
        return !col.Any();
    }

    protected void DataSourceLinkSave(object sender, DirectEventArgs e)
    {
        try
        {
            if (!DataSourceLinkOk())
            {
                MessageBoxes.Error("Error", "Data Source is already linked");
                return;
            }
            RowSelectionModel masterRow = InstrumentsGrid.SelectionModel.Primary as RowSelectionModel;
            var masterID = new Guid(masterRow.SelectedRecordID);
            InstrumentDataSource instrumentDataSource = new InstrumentDataSource(Utilities.MakeGuid(DataSourceLinkID.Value));
            instrumentDataSource.InstrumentID = masterID;
            instrumentDataSource.DataSourceID = new Guid(cbDataSource.SelectedItem.Value.Trim());
            if (!String.IsNullOrEmpty(dfDataSourceStartDate.Text) && (dfDataSourceStartDate.SelectedDate.Year >= 1900))
                instrumentDataSource.StartDate = dfDataSourceStartDate.SelectedDate;
            else
                instrumentDataSource.StartDate = null;
            if (!String.IsNullOrEmpty(dfDataSourceEndDate.Text) && (dfDataSourceEndDate.SelectedDate.Year >= 1900))
                instrumentDataSource.EndDate = dfDataSourceEndDate.SelectedDate;
            else
                instrumentDataSource.EndDate = null;
            instrumentDataSource.UserId = AuthHelper.GetLoggedInUserId;
            instrumentDataSource.Save();
            Auditing.Log("Instruments.AddDataSourceLink", new Dictionary<string, object> {
                { "InstrumentID", instrumentDataSource.InstrumentID },
                { "InstrumentCode", instrumentDataSource.Instrument.Code },
                { "DataSourceID", instrumentDataSource.DataSourceID},
                { "DataSourceCode", instrumentDataSource.DataSource.Code},
                { "StartDate", instrumentDataSource?.StartDate },
                { "EndDate", instrumentDataSource?.EndDate}
            });
            DataSourceLinksGrid.DataBind();
            DataSourceLinkWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Instruments.LinkDataSource_Click");
            MessageBoxes.Error(ex, "Error", "Unable to link DataSource");
        }
    }

    [DirectMethod]
    public void ConfirmDeleteDataSourceLink(Guid aID)
    {
        MessageBoxes.Confirm(
            "Confirm Delete",
            String.Format("DirectCall.DeleteDataSourceLink(\"{0}\",{{ eventMask: {{ showMask: true}}}});", aID.ToString()),
            "Are you sure you want to delete this Data Source link?");
    }

    [DirectMethod]
    public void DeleteDataSourceLink(Guid aID)
    {
        try
        {
            InstrumentDataSource.Delete(aID);
            Auditing.Log("Instruments.DeleteDataSourceLink", new Dictionary<string, object> { { "ID", aID } });
            DataSourceLinksGrid.DataBind();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Instruments.DeleteDataSourceLink({aID})", aID);
            MessageBoxes.Error(ex, "Error", "Unable to delete DataSource link");
        }
    }

    [DirectMethod]
    public void AddDataSourceClick(object sender, DirectEventArgs e)
    {
        //X.Redirect(X.ResourceManager.ResolveUrl("Admin/Sites"));
    }
    #endregion

    #region Sensors
    protected void SensorLinksGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
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
                Log.Error(ex, "Instruments.SensorLinksGridStore_RefreshData");
                MessageBoxes.Error(ex, "Error", "Unable to refresh sensors grid");
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
        return !col.Any();
    }

    protected void SensorLinkSave(object sender, DirectEventArgs e)
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
            InstrumentSensor instrumentSensor = new InstrumentSensor(Utilities.MakeGuid(SensorLinkID.Value));
            instrumentSensor.InstrumentID = masterID;
            instrumentSensor.SensorID = new Guid(cbSensor.SelectedItem.Value.Trim());
            if (!String.IsNullOrEmpty(dfSensorStartDate.Text) && (dfSensorStartDate.SelectedDate.Year >= 1900))
                instrumentSensor.StartDate = dfSensorStartDate.SelectedDate;
            else
                instrumentSensor.StartDate = null;
            if (!String.IsNullOrEmpty(dfSensorEndDate.Text) && (dfSensorEndDate.SelectedDate.Year >= 1900))
                instrumentSensor.EndDate = dfSensorEndDate.SelectedDate;
            else
                instrumentSensor.EndDate = null;
            instrumentSensor.UserId = AuthHelper.GetLoggedInUserId;
            instrumentSensor.Save();
            Auditing.Log("Instruments.AddSensorLink", new Dictionary<string, object> {
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
            Log.Error(ex, "Instruments.LinkSensor_Click");
            MessageBoxes.Error(ex, "Error", "Unable to link sensor");
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
        try
        {
            InstrumentSensor.Delete(aID);
            Auditing.Log("Instruments.DeleteSensorLink", new Dictionary<string, object> { { "ID", aID } });
            SensorLinksGrid.DataBind();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Instruments.DeleteSensorLink({aID})", aID);
            MessageBoxes.Error(ex, "Error", "Unable to delete sensor link");
        }
    }

    [DirectMethod]
    public void AddSensorClick(object sender, DirectEventArgs e)
    {
        //X.Redirect(X.ResourceManager.ResolveUrl("Admin/Sites"));
    }
    #endregion

}