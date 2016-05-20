using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAEON.Observations.Data;
using Ext.Net;
using SubSonic;
using System.Xml.Xsl;
using System.Xml;

/// <summary>
/// Summary description for Sensor
/// </summary>
public partial class _Sensor : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!X.IsAjaxRequest)
        {
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

    protected void SensorStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        this.SensorGrid.GetStore().DataSource = SensorRepository.GetPagedList(e, e.Parameters[this.GridFilters1.ParamPrefix]);
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

        SensorGrid.DataBind();

        this.DetailWindow.Hide();
    }

    protected void SensorStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();
        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        string js = BaseRepository.BuildExportQ("VSensor", gridData, visCols, sortCol, sortDir);

        BaseRepository.doExport(type, js);
    }


}