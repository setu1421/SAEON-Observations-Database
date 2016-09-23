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

public partial class Admin_DataSchemas : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!X.IsAjaxRequest)
        {
            DataSourceTypeStore.DataSource = new DataSourceTypeCollection().OrderByAsc(DataSourceType.Columns.Code).Load();
            DataSourceTypeStore.DataBind();
            SchemaColumnTypeStore.DataSource = new SchemaColumnTypeCollection().OrderByAsc(SchemaColumnType.Columns.Name).Load();
            SchemaColumnTypeStore.DataBind();
            PhenomenonStore.DataSource = new PhenomenonCollection().OrderByAsc(Phenomenon.Columns.Name).Load();
            PhenomenonStore.DataBind();
        }
    }

    #region Data Schemas
    protected void DataSchemasGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        this.DataSchemasGrid.GetStore().DataSource = DataSchemRepository.GetPagedList(e, e.Parameters[this.GridFilters1.ParamPrefix]);
    }

    protected void ValidateField(object sender, RemoteValidationEventArgs e)
    {

        DataSchemaCollection col = new DataSchemaCollection();

        string checkColumn = String.Empty,
               errorMessage = String.Empty;

        if (e.ID == "tfCode")
        {
            checkColumn = Instrument.Columns.Code;
            errorMessage = "The specified Data Schema Code already exists";
        }
        else if (e.ID == "tfName")
        {
            checkColumn = Instrument.Columns.Name;
            errorMessage = "The specified Data Schema Name already exists";

        }

        if (!string.IsNullOrEmpty(checkColumn))
            if (String.IsNullOrEmpty(tfID.Text.ToString()))
                col = new DataSchemaCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
            else
                col = new DataSchemaCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(DataSchema.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();

        if (col.Count > 0)
        {
            e.Success = false;
            e.ErrorMessage = errorMessage;
        }
        else
            e.Success = true;
    }

    protected void DataSchemasGridStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();
        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        string js = BaseRepository.BuildExportQ("VDataSchema", gridData, visCols, sortCol, sortDir);

        BaseRepository.doExport(type, js);
    }

    protected void Save(object sender, DirectEventArgs e)
    {

        DataSchema schema = new DataSchema();

        if (String.IsNullOrEmpty(tfID.Text))
            schema.Id = Guid.NewGuid();
        else
            schema = new DataSchema(tfID.Text.Trim());

        schema.Code = tfCode.Text.Trim();
        schema.Name = tfName.Text.Trim();
        schema.Description = tfDescription.Text.Trim();

        if (!String.IsNullOrEmpty(nfIgnoreFirst.Text))
            schema.IgnoreFirst = Int32.Parse(nfIgnoreFirst.Text);
        else
            schema.IgnoreFirst = 0;

        if (!String.IsNullOrEmpty(nfIgnoreLast.Text))
            schema.IgnoreLast = Int32.Parse(nfIgnoreLast.Text);
        else
            schema.IgnoreLast = 0;

        if (!String.IsNullOrEmpty(tfCondition.Text))
            schema.Condition = tfCondition.Text;
        else
            schema.Condition = null;


        if (!String.IsNullOrEmpty(tfSplit.Text))
        {
            schema.SplitSelector = tfSplit.Text;
            schema.SplitIndex = int.Parse(nfSplitIndex.Value.ToString());
        }
        else
        {
            schema.SplitSelector = null;
            schema.SplitIndex = null;
        }

        schema.DataSourceTypeID = new Guid(cbDataSourceType.SelectedItem.Value);
        schema.Delimiter = cbDelimiter.SelectedItem.Value;

        schema.UserId = AuthHelper.GetLoggedInUserId;
        schema.Save();
        DataSchemasGrid.DataBind();

        this.DetailWindow.Hide();
    }

    protected void MasterRowSelect(object sender, DirectEventArgs e)
    {
        RowSelectionModel masterRow = DataSchemasGrid.SelectionModel.Primary as RowSelectionModel;
        var masterID = new Guid(masterRow.SelectedRecordID);
        DataSchema schema = new DataSchema(masterID);
        if (schema.DataSourceTypeID == new Guid(DataSourceType.CSV))
        {
            nfWidth.Visible = false;
        }
        else
        {
            nfWidth.Visible = true;
        }
    }

    #endregion

    #region Schema Columns
    protected void SchemaColumnsGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["DataSchemaID"] != null && e.Parameters["DataSchemaID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["DataSchemaID"].ToString());
            try
            {
                VSchemaColumnCollection col = new VSchemaColumnCollection()
                    .Where(VSchemaColumn.Columns.DataSchemaID, Id)
                    .OrderByAsc(VSchemaColumn.Columns.Number)
                    .Load();
                SchemaColumnsGrid.GetStore().DataSource = col;
                SchemaColumnsGrid.GetStore().DataBind();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DataSchemas.SchemaColumnsGridStore_RefreshData");
                MessageBoxes.Error(ex, "Error", "Unable to refresh SchemaColumns grid");
            }
        }
    }

    protected void SchemaColumnAddSave(object sender, DirectEventArgs e)
    {
        try
        {
            RowSelectionModel masterRow = DataSchemasGrid.SelectionModel.Primary as RowSelectionModel;
            var masterID = new Guid(masterRow.SelectedRecordID);
            SchemaColumn schemaColumn = new SchemaColumn(Utilities.MakeGuid(SchemaColumnAddID.Value));
            schemaColumn.DataSchemaID = masterID;
            //schemaColumn.SchemaColumnTypeID = new Guid(cbSchemaColumnType.SelectedItem.Value.Trim());
            //if (!String.IsNullOrEmpty(dfSchemaColumnStartDate.Text) && (dfSchemaColumnStartDate.SelectedDate.Year >= 1900))
            //    DataSchemaSchemaColumn.StartDate = dfSchemaColumnStartDate.SelectedDate;
            //else
            //    DataSchemaSchemaColumn.StartDate = null;
            //if (!String.IsNullOrEmpty(dfSchemaColumnEndDate.Text) && (dfSchemaColumnEndDate.SelectedDate.Year >= 1900))
            //    DataSchemaSchemaColumn.EndDate = dfSchemaColumnEndDate.SelectedDate;
            //else
            //    DataSchemaSchemaColumn.EndDate = null;
            //DataSchemaSchemaColumn.UserId = AuthHelper.GetLoggedInUserId;
            //DataSchemaSchemaColumn.Save();
            Auditing.Log("DataSchemas.AddSchemaColumn", new Dictionary<string, object> {
                { "DataSchemaID", schemaColumn.DataSchemaID },
                { "DataSchemaCode", schemaColumn.DataSchema.Code },
                //{ "SchemaColumnID", DataSchemaSchemaColumn.SchemaColumnID},
                //{ "SchemaColumnCode", DataSchemaSchemaColumn.SchemaColumn.Code},
                //{ "StartDate", DataSchemaSchemaColumn.StartDate },
                //{ "EndDate", DataSchemaSchemaColumn.EndDate}
            });
            SchemaColumnsGrid.DataBind();
            SchemaColumnAddWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "DataSchemas.AddSchemaColumn_Click");
            MessageBoxes.Error(ex, "Error", "Unable to add SchemaColumn");
        }
    }

    [DirectMethod]
    public void ConfirmDeleteSchemaColumn(Guid aID)
    {
        MessageBoxes.Confirm(
            "Confirm Delete",
            String.Format("DirectCall.DeleteSchemaColumn(\"{0}\",{{ eventMask: {{ showMask: true}}}});", aID.ToString()),
            "Are you sure you want to delete this column?");
    }

    [DirectMethod]
    public void DeleteSchemaColumn(Guid aID)
    {
        try
        {
            new SchemaColumnController().Delete(aID);
            Auditing.Log("DataSchemas.DeleteSchemaColumn", new Dictionary<string, object> { { "ID", aID } });
            SchemaColumnsGrid.DataBind();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "DataSchemas.DeleteSchemaColumn({aID})", aID);
            MessageBoxes.Error(ex, "Error", "Unable to delete SchemaColumn");
        }
    }

    [DirectMethod]
    public void AddSchemaColumnClick(object sender, DirectEventArgs e)
    {
        //X.Redirect(X.ResourceManager.ResolveUrl("Admin/Sites"));
    }

    [DirectMethod]
    public void cbPhenomenonSelect(object sender, DirectEventArgs e)
    {
        var Id = cbPhenomenon.SelectedItem.Value;
        cbOffering.GetStore().DataSource = new Select(PhenomenonOffering.IdColumn, Offering.NameColumn)
            .From(Offering.Schema)
            .InnerJoin(PhenomenonOffering.OfferingIDColumn, Offering.IdColumn)
            .Where(PhenomenonOffering.Columns.PhenomenonID).IsEqualTo(Id)
            .OrderAsc(Offering.Columns.Name)
            .ExecuteDataSet().Tables[0];
        cbOffering.GetStore().DataBind();
        cbUnitOfMeasure.GetStore().DataSource = new Select(PhenomenonUOM.IdColumn, UnitOfMeasure.UnitColumn)
            .From(UnitOfMeasure.Schema)
            .InnerJoin(PhenomenonUOM.UnitOfMeasureIDColumn, UnitOfMeasure.IdColumn)
            .Where(PhenomenonUOM.Columns.PhenomenonID).IsEqualTo(Id)
            .OrderAsc(UnitOfMeasure.Columns.Unit)
            .ExecuteDataSet().Tables[0];
        cbUnitOfMeasure.GetStore().DataBind();
        cbOffering.Clear();
        cbUnitOfMeasure.Clear();
    }

    [DirectMethod]
    public void cbSchemaColumnTypeSelect(object sender, DirectEventArgs e)
    {
    }

    #endregion
}