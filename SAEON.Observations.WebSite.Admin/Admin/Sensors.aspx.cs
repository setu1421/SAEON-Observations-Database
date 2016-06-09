using Ext.Net;
using SAEON.Observations.Data;
using Serilog;
using SubSonic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin_Sensors : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!X.IsAjaxRequest)
        {
            InstrumentStore.DataSource = new InstrumentCollection().OrderByAsc(Instrument.Columns.Name).Load();
            InstrumentStore.DataBind();

            var store = sbStation.GetStore();
            SubSonic.SqlQuery q = new Select(Station.Columns.Id, Station.Columns.Name)
                                    .From(Station.Schema)
                                    .Where(Station.Columns.Id).IsNotNull()
                                    .OrderAsc(DataSchema.Columns.Name);
            System.Data.DataSet ds = q.ExecuteDataSet();
            store.DataSource = ds.Tables[0];
            store.DataBind();

            store = sbPhenomenon.GetStore();
            q = new Select(Phenomenon.Columns.Id, Phenomenon.Columns.Name)
                       .From(Phenomenon.Schema)
                       .Where(Phenomenon.Columns.Id).IsNotNull()
                       .OrderAsc(Phenomenon.Columns.Name);
            ds = q.ExecuteDataSet();
            store.DataSource = ds.Tables[0];
            store.DataBind();

            store = cbDataSource.GetStore();
            q = new Select(DataSource.Columns.Id, DataSource.Columns.Name, DataSource.Columns.DataSchemaID)
                    .From(DataSource.Schema)
                    .Where(DataSource.Columns.Id).IsNotNull()
                    .OrderAsc(DataSource.Columns.Name);
            ds = q.ExecuteDataSet();
            store.DataSource = ds.Tables[0];
            store.DataBind();


            cbDataSchema.GetStore().DataSource = new DataSchemaCollection().OrderByAsc(DataSchema.Columns.Name).Load();
            cbDataSchema.GetStore().DataBind();
        }
    }

    #region Sensors
    protected void SensorsGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        this.SensorsGrid.GetStore().DataSource = SensorRepository.GetPagedList(e, e.Parameters[this.GridFilters1.ParamPrefix]);
    }

    protected void ValidateField(object sender, RemoteValidationEventArgs e)
    {

        SensorCollection col = new SensorCollection();

        string checkColumn = String.Empty,
               errorMessage = String.Empty;

        if (e.ID == "tfCode" || e.ID == "tfName")
        {
            if (e.ID == "tfCode")
            {
                checkColumn = Sensor.Columns.Code;
                errorMessage = "The specified Sensor Procedure Code already exists";
            }
            else if (e.ID == "tfName")
            {
                checkColumn = Sensor.Columns.Name;
                errorMessage = "The specified Sensor Procedure Name already exists";

            }

            if (String.IsNullOrEmpty(tfID.Text.ToString()))
                col = new SensorCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
            else
                col = new SensorCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(Offering.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();

            if (col.Count > 0)
            {
                e.Success = false;
                e.ErrorMessage = errorMessage;
            }
            else
                e.Success = true;
        }
    }

    protected void Save(object sender, DirectEventArgs e)
    {

        Sensor sens = new Sensor();

        if (String.IsNullOrEmpty(tfID.Text))
            sens.Id = Guid.NewGuid();
        else
            sens = new Sensor(tfID.Text.Trim());

        sens.Code = tfCode.Text.Trim();
        sens.Name = tfName.Text.Trim();
        sens.Description = tfDescription.Text.Trim();
        sens.UserId = AuthHelper.GetLoggedInUserId;
        sens.Url = tfUrl.Text.Trim();
        sens.StationID = Guid.Parse(sbStation.SelectedItem.Value);
        sens.PhenomenonID = Guid.Parse(sbPhenomenon.SelectedItem.Value);
        sens.DataSourceID = Guid.Parse(cbDataSource.SelectedItem.Value);

        if (cbDataSchema.SelectedItem.Value != null)
        {
            //test if dataschema is valid (linked to test in datasource files)
            bool isValid = true;
            string dataSourceName = "";

            if (sens.DataSource.DataSchemaID != null)
            {
                isValid = false;
                dataSourceName = sens.DataSource.Name;
            }

            //
            if (isValid)
            {
                sens.DataSchemaID = Guid.Parse(cbDataSchema.SelectedItem.Value);
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Invalid Data Source",
                    //Message = "The selected data schema is already linked to a sensor that is linked to this data source.",
                    Message = "This sensor procedure cant have a data schema because its data source (" + dataSourceName + ") is already linked to a data schema",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR
                });

                return;
            }
        }
        else
            sens.DataSchemaID = null;

        sens.Save();

        SensorsGrid.DataBind();

        DetailWindow.Hide();
    }

    protected void SensorsGridStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();
        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        string js = BaseRepository.BuildExportQ("VSensor", gridData, visCols, sortCol, sortDir);

        BaseRepository.doExport(type, js);
    }
    #endregion

    #region Instruments
    protected void InstrumentLinksGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["SensorID"] != null && e.Parameters["SensorID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["SensorID"].ToString());
            try
            {
                VInstrumentSensorCollection col = new VInstrumentSensorCollection()
                    .Where(VInstrumentSensor.Columns.SensorID, Id)
                    .OrderByAsc(VInstrumentSensor.Columns.StartDate)
                    .OrderByAsc(VInstrumentSensor.Columns.EndDate)
                    .OrderByAsc(VInstrumentSensor.Columns.InstrumentName)
                    .Load();
                InstrumentLinksGrid.GetStore().DataSource = col;
                InstrumentLinksGrid.GetStore().DataBind();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Sensors.InstrumentLinksGridStore_RefreshData");
                MessageBoxes.Error(ex, "Error", "Unable to refresh instruments grid");
            }
        }
    }

    protected void LinkInstrument_Click(object sender, DirectEventArgs e)
    {
        try
        {
            RowSelectionModel masterRow = SensorsGrid.SelectionModel.Primary as RowSelectionModel;
            var masterID = new Guid(masterRow.SelectedRecordID);
            InstrumentSensor instrumentSensor = new InstrumentSensor(Utilities.MakeGuid(InstrumentLinkID.Value));
            instrumentSensor.SensorID = masterID;
            instrumentSensor.InstrumentID = new Guid(cbInstrumentLink.SelectedItem.Value.Trim());
            if (!String.IsNullOrEmpty(dfInstrumentStartDate.Text) && (dfInstrumentStartDate.SelectedDate.Year >= 1900))
                instrumentSensor.StartDate = dfInstrumentStartDate.SelectedDate;
            else
                instrumentSensor.StartDate = null;
            if (!String.IsNullOrEmpty(dfInstrumentEndDate.Text) && (dfInstrumentEndDate.SelectedDate.Year >= 1900))
                instrumentSensor.EndDate = dfInstrumentEndDate.SelectedDate;
            else
                instrumentSensor.EndDate = null;
            instrumentSensor.UserId = AuthHelper.GetLoggedInUserId;
            instrumentSensor.Save();
            Auditing.Log("Sensors.AddInstrumentLink", new Dictionary<string, object> {
                { "SensorID", instrumentSensor.SensorID },
                { "SensorCode", instrumentSensor.Sensor.Code },
                { "InstrumentID", instrumentSensor.InstrumentID},
                { "InstrumentCode", instrumentSensor.Instrument.Code},
                { "StartDate", instrumentSensor.StartDate },
                { "EndDate", instrumentSensor.EndDate}
            });
            InstrumentLinksGrid.DataBind();
            InstrumentLinkWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Sensors.LinkInstrument_Click");
            MessageBoxes.Error(ex, "Error", "Unable to link sensor");
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
            new InstrumentSensorController().Delete(aID);
            Auditing.Log("Sensors.DeleteInstrumentLink", new Dictionary<string, object> { { "ID", aID } });
            InstrumentLinksGrid.DataBind();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Sensors.DeleteInstrumentLink({aID})", aID);
            MessageBoxes.Error(ex, "Error", "Unable to delete instrument link");
        }
    }

    [DirectMethod]
    public void AddInstrumentClick(object sender, DirectEventArgs e)
    {
        //X.Redirect(X.ResourceManager.ResolveUrl("Admin/Sites"));
    }
    #endregion
}