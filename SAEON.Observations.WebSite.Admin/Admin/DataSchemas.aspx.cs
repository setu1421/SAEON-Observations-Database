using Ext.Net;
using SAEON.Observations.Data;
using Serilog;
using SubSonic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;
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
            checkColumn = DataSchema.Columns.Code;
            errorMessage = "The specified Data Schema Code already exists";
        }
        else if (e.ID == "tfName")
        {
            checkColumn = DataSchema.Columns.Name;
            errorMessage = "The specified Data Schema Name already exists";

        }

        if (!string.IsNullOrEmpty(checkColumn))
            if (string.IsNullOrEmpty(tfID.Text.ToString()))
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
        schema.Code = Utilities.NullIfEmpty(tfCode.Text);
        schema.Name = Utilities.NullIfEmpty(tfName.Text);
        schema.Description = Utilities.NullIfEmpty(tfDescription.Text);

        if (!String.IsNullOrEmpty(nfIgnoreFirst.Text))
            schema.IgnoreFirst = Int32.Parse(nfIgnoreFirst.Text);
        else
            schema.IgnoreFirst = 0;

        if (!String.IsNullOrEmpty(nfIgnoreLast.Text))
            schema.IgnoreLast = Int32.Parse(nfIgnoreLast.Text);
        else
            schema.IgnoreLast = 0;

        schema.Condition = Utilities.NullIfEmpty(tfCondition.Text);

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
        DataSourceType dataSourceType = new DataSourceType(schema.DataSourceTypeID);
        nfWidth.Hidden = (dataSourceType.Code == "CSV");
        nfWidth.AllowBlank = nfWidth.Hidden;
        var cm = SchemaColumnsGrid.ColumnModel.Columns.FirstOrDefault(c => c.DataIndex == "Width").Hidden = nfWidth.Hidden;
        SetFields();
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

    protected void ValidateColumnField(object sender, RemoteValidationEventArgs e)
    {
        e.Success = true;
        if (e.ID == "tfColumnName")
        {
            RowSelectionModel masterRow = DataSchemasGrid.SelectionModel.Primary as RowSelectionModel;
            var masterID = new Guid(masterRow.SelectedRecordID);
            SchemaColumnCollection col = new SchemaColumnCollection()
                .Where(SchemaColumn.Columns.DataSchemaID, masterID)
                .Where(SchemaColumn.Columns.Name, e.Value.ToString().Trim())
                .Load();
            if (col.Any())
            {
                e.Success = false;
                e.ErrorMessage = "The specified Schema Column Name already exists";
            }
        }
        else if (e.ID == "tfFormat")
        {
            try
            {
                DateTime dt = DateTime.Now;
                string format = e.Value.ToString().Trim();
                DateTime.ParseExact(dt.ToString(format), format, null);
            }
            catch
            {
                e.Success = false;
                e.ErrorMessage = "The format specified is invalid.";
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
            SqlQuery qry = new Select(Aggregate.Max(SchemaColumn.Columns.Number))
                .From(SchemaColumn.Schema)
                .Where(SchemaColumn.Columns.DataSchemaID).IsEqualTo(masterID);
            schemaColumn.Number = qry.ExecuteScalar<int>() + 1;
            schemaColumn.Name = Utilities.NullIfEmpty(tfColumnName.Text);
            schemaColumn.SchemaColumnTypeID = new Guid(cbSchemaColumnType.SelectedItem.Value.Trim());
            DataSchema schema = new DataSchema(masterID);
            DataSourceType dataSourceType = new DataSourceType(schema.DataSourceTypeID);
            if (dataSourceType.Code == "CSV")
                schemaColumn.Width = null;
            else if (string.IsNullOrEmpty(nfWidth.Text.Trim()))
                schemaColumn.Width = null;
            else
                schemaColumn.Width = int.Parse(nfWidth.Text.Trim());
            schemaColumn.Format = null;
            schemaColumn.PhenomenonID = null;
            schemaColumn.PhenomenonOfferingID = null;
            schemaColumn.PhenomenonUOMID = null;
            schemaColumn.EmptyValue = null;
            schemaColumn.FixedTime = null;
            switch (cbSchemaColumnType.SelectedItem.Text)
            {
                case "Date":
                    schemaColumn.Format = tfFormat.Text.Trim();
                    break;
                case "Time":
                    schemaColumn.Format = tfFormat.Text.Trim();
                    break;
                case "Offering":
                    schemaColumn.PhenomenonID = Utilities.MakeGuid(cbPhenomenon.Value);
                    schemaColumn.PhenomenonOfferingID = Utilities.MakeGuid(cbOffering.Value);
                    schemaColumn.PhenomenonUOMID = Utilities.MakeGuid(cbUnitOfMeasure.Value);
                    schemaColumn.EmptyValue = Utilities.NullIfEmpty(tfEmptyValue.Text);
                    break;
                case "Fixed Time":
                    schemaColumn.PhenomenonID = Utilities.MakeGuid(cbPhenomenon.Value);
                    schemaColumn.PhenomenonOfferingID = Utilities.MakeGuid(cbOffering.Value);
                    schemaColumn.PhenomenonUOMID = Utilities.MakeGuid(cbUnitOfMeasure.Value);
                    schemaColumn.EmptyValue = Utilities.NullIfEmpty(tfEmptyValue.Text);
                    if (!string.IsNullOrEmpty(ttFixedTime.Text.Trim()))
                        schemaColumn.FixedTime = ttFixedTime.SelectedTime.ToString();
                    break;
            }
            schemaColumn.UserId = AuthHelper.GetLoggedInUserId;
            schemaColumn.Save();
            Auditing.Log("DataSchemas.AddSchemaColumn", new Dictionary<string, object> {
                { "DataSchemaID", schemaColumn.DataSchemaID },
                { "DataSchemaCode", schemaColumn.DataSchema.Code },
                { "Name", schemaColumn.Name },
                { "SchemaColumnType", schemaColumn.SchemaColumnType.Name },
                { "Width", schemaColumn?.Width },
                { "Format", schemaColumn.Format },
                { "PhenomenonID", schemaColumn.PhenomenonID },
                { "PhenomenonCode", schemaColumn?.Phenomenon?.Code },
                { "PhenomenonOfferingID", schemaColumn.PhenomenonOfferingID },
                { "PhenomenonOfferingCode", schemaColumn?.PhenomenonOffering?.Offering.Code },
                { "PhenomenonUnitOfMeasureID", schemaColumn.PhenomenonUOMID },
                { "PhenomenonUnitOfMeasureCode", schemaColumn?.PhenomenonUOM?.UnitOfMeasure.Code },
                { "EmptyValue", schemaColumn.EmptyValue },
                { "FixedTime", schemaColumn.FixedTime }
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
            SchemaColumn.Delete(aID);
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
    public void cbDataSourceTypeSelect(object sender, DirectEventArgs e)
    {
        DataSourceType dsType = new DataSourceType(cbDataSourceType.Value);
        if (dsType.Code == "CSV")
            cbDelimiter.AllowBlank = false;
        else
        {
            cbDelimiter.AllowBlank = true;
            cbDelimiter.ClearValue();
            cbDelimiter.ClearInvalid();
            cbDelimiter.MarkInvalid();
        }
    }


    [DirectMethod]
    public void cbPhenomenonSelect(object sender, DirectEventArgs e)
    {
        OfferingStore.DataSource = new Select(PhenomenonOffering.IdColumn.QualifiedName + " Id", Phenomenon.NameColumn.QualifiedName + " PhenomenonName", Offering.NameColumn.QualifiedName + " OfferingName")
            .From(PhenomenonOffering.Schema)
            .InnerJoin(Phenomenon.IdColumn, PhenomenonOffering.PhenomenonIDColumn)
            .InnerJoin(Offering.IdColumn, PhenomenonOffering.OfferingIDColumn)
            .Where(PhenomenonOffering.Columns.PhenomenonID).IsEqualTo(cbPhenomenon.Value)
            .OrderAsc(Phenomenon.NameColumn.QualifiedName, Offering.NameColumn.QualifiedName)
            .ExecuteDataSet().Tables[0];
        OfferingStore.DataBind();
        UnitOfMeasureStore.DataSource = new Select(PhenomenonUOM.IdColumn.QualifiedName + " Id", Phenomenon.NameColumn.QualifiedName + " PhenomenonName", UnitOfMeasure.UnitColumn.QualifiedName + " UnitOfMeasureUnit")
           .From(PhenomenonUOM.Schema)
           .InnerJoin(Phenomenon.IdColumn, PhenomenonUOM.PhenomenonIDColumn)
           .InnerJoin(UnitOfMeasure.IdColumn, PhenomenonUOM.UnitOfMeasureIDColumn)
           .Where(PhenomenonOffering.Columns.PhenomenonID).IsEqualTo(cbPhenomenon.Value)
           .OrderAsc(Phenomenon.NameColumn.QualifiedName, UnitOfMeasure.UnitColumn.QualifiedName)
           .ExecuteDataSet().Tables[0];
        UnitOfMeasureStore.DataBind();
        cbOffering.Clear();
        cbOffering.ClearInvalid();
        cbOffering.MarkInvalid();
        cbUnitOfMeasure.Clear();
        cbUnitOfMeasure.ClearInvalid();
        cbUnitOfMeasure.MarkInvalid();
    }

    [DirectMethod]
    public void LoadCombos(string columnType, string phenomenonID, string offeringID, string unitOfMeasureID)
    {
        cbSchemaColumnType.Value = columnType;
        SetFields();
        cbPhenomenon.Value = phenomenonID;
        cbPhenomenonSelect(cbPhenomenon, new DirectEventArgs(null));
        cbOffering.Value = offeringID;
        cbUnitOfMeasure.Value = unitOfMeasureID;
    }

    private void SetFields()
    {
        tfFormat.AllowBlank = true;
        tfFormat.MarkAsValid();
        tfFormat.Hidden = true;
        cbPhenomenon.AllowBlank = true;
        cbPhenomenon.ForceSelection = false;
        cbPhenomenon.MarkAsValid();
        cbPhenomenon.Hidden = true;
        cbOffering.AllowBlank = true;
        cbOffering.ForceSelection = true;
        cbOffering.MarkAsValid();
        cbOffering.Hidden = true;
        cbUnitOfMeasure.AllowBlank = true;
        cbUnitOfMeasure.ForceSelection = true;
        cbUnitOfMeasure.MarkAsValid();
        cbUnitOfMeasure.Hidden = true;
        tfEmptyValue.Hidden = true;
        ttFixedTime.AllowBlank = true;
        ttFixedTime.MarkAsValid();
        ttFixedTime.Hidden = true;
        switch (cbSchemaColumnType.SelectedItem.Text)
        {
            case "Date":
                tfFormat.AllowBlank = false;
                tfFormat.Hidden = false;
                break;
            case "Time":
                tfFormat.AllowBlank = false;
                tfFormat.Hidden = false;
                break;
            case "Ignore":
                break;
            case "Offering":
                cbPhenomenon.Hidden = false;
                cbPhenomenon.AllowBlank = false;
                cbOffering.Hidden = false;
                cbOffering.AllowBlank = false;
                cbUnitOfMeasure.Hidden = false;
                cbUnitOfMeasure.AllowBlank = false;
                tfEmptyValue.Hidden = false;
                break;
            case "Fixed Time":
                cbPhenomenon.Hidden = false;
                cbPhenomenon.AllowBlank = false;
                cbOffering.Hidden = false;
                cbOffering.AllowBlank = false;
                cbUnitOfMeasure.Hidden = false;
                cbUnitOfMeasure.AllowBlank = false;
                tfEmptyValue.Hidden = false;
                ttFixedTime.AllowBlank = false;
                ttFixedTime.Hidden = false;
                break;
            case "Comment":
                break;
        }
    }

    [DirectMethod]
    public void cbSchemaColumnTypeSelect(object sender, DirectEventArgs e)
    {
        SetFields();
    }

    [DirectMethod]
    public void SchemaColumnUp(Guid aID)
    {
        try
        {
            SchemaColumnCollection col = new SchemaColumnCollection().Where("DataSchemaID", new SchemaColumn(aID).DataSchemaID).OrderByAsc("Number").Load();
            SchemaColumn col1 = col.FirstOrDefault(c => c.Id == aID);
            SchemaColumn col2 = col.ElementAtOrDefault(col.IndexOf(col1) - 1);
            if (col2 == null)
            {
                col1.Number--;
                col1.Save();
            }
            else
                using (var ts = new TransactionScope())
                using (new SharedDbConnectionScope())
                {
                    int old1 = col1.Number;
                    int old2 = col2.Number;
                    col1.Number = int.MinValue;
                    col1.Save();
                    col2.Number = int.MaxValue;
                    col2.Save();
                    col1.Number = old2;
                    col1.Save();
                    col2.Number = old1;
                    col2.Save();
                    ts.Complete();
            }
            Auditing.Log("DataSchemas.SchemaColumnUp", new Dictionary<string, object> { { "ID", aID } });
            SchemaColumnsGrid.DataBind();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "DataSchemas.SchemaColumnUp({aID})", aID);
            MessageBoxes.Error(ex, "Error", "Unable to move SchemaColumn up");
        }
    }

    [DirectMethod]
    public void SchemaColumnDown(Guid aID)
    {
        try
        {
            SchemaColumnCollection col = new SchemaColumnCollection().Where("DataSchemaID", new SchemaColumn(aID).DataSchemaID).OrderByAsc("Number").Load();
            SchemaColumn col1 = col.FirstOrDefault(c => c.Id == aID);
            SchemaColumn col2 = col.ElementAtOrDefault(col.IndexOf(col1) + 1);
            if (col2 == null)
            {
                col1.Number++;
                col1.Save();
            }
            else
                using (var ts = new TransactionScope())
                using (new SharedDbConnectionScope())
                {
                    int old1 = col1.Number;
                    int old2 = col2.Number;
                    col1.Number = int.MinValue;
                    col1.Save();
                    col2.Number = int.MaxValue;
                    col2.Save();
                    col1.Number = old2;
                    col1.Save();
                    col2.Number = old1;
                    col2.Save();
                    ts.Complete();
                }
            Auditing.Log("DataSchemas.SchemaColumnDown", new Dictionary<string, object> { { "ID", aID } });
            SchemaColumnsGrid.DataBind();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "DataSchemas.SchemaColumnDown({aID})", aID);
            MessageBoxes.Error(ex, "Error", "Unable to move SchemaColumn down");
        }
    }
    #endregion

    #region Data Sources
    protected void DataSourcesGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["DataSchemaID"] != null && e.Parameters["DataSchemaID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["DataSchemaID"].ToString());
            try
            {
                DataSourceCollection col = new DataSourceCollection()
                    .Where(DataSource.Columns.DataSchemaID, Id)
                    .OrderByAsc(DataSource.Columns.Name)
                    .Load();
                DataSourcesGrid.GetStore().DataSource = col;
                DataSourcesGrid.GetStore().DataBind();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DataSchemas.DataSourceGridStore_RefreshData");
                MessageBoxes.Error(ex, "Error", "Unable to refresh DataSources grid");
            }
        }
    }

    [DirectMethod]
    public void ConfirmDeleteDataSource(Guid aID)
    {
        MessageBoxes.Confirm(
            "Confirm Delete",
            String.Format("DirectCall.DeleteDataSource(\"{0}\",{{ eventMask: {{ showMask: true}}}});", aID.ToString()),
            "Are you sure you want to delete this Data Source?");
    }

    [DirectMethod]
    public void DeleteDataSource(Guid aID)
    {
        try
        {
            DataSource dataSource = new DataSource(aID);
            dataSource.DataSchemaID = null;
            dataSource.Save();
            Auditing.Log("DataSchemas.DeleteDataSource", new Dictionary<string, object> { { "ID", aID } });
            DataSourcesGrid.DataBind();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "DataSchemas.DeleteDataSource({aID})", aID);
            MessageBoxes.Error(ex, "Error", "Unable to delete DataSource");
        }
    }

    protected void AvailableDataSourcesGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["DataSchemaID"] != null && e.Parameters["DataSchemaID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["DataSchemaID"].ToString());
            DataSourceCollection col = new Select()
                .From(DataSource.Schema)
                .Where(DataSource.IdColumn)
                .NotIn(new Select(new string[] { DataSource.Columns.Id }).From(DataSource.Schema).Where(DataSource.DataSchemaIDColumn).IsEqualTo(Id))
                .And(DataSource.DataSchemaIDColumn)
                .IsNull()
                .OrderAsc(DataSource.Columns.StartDate)
                .OrderAsc(DataSource.Columns.Name)
                .ExecuteAsCollection<DataSourceCollection>();
            AvailableDataSourcesGrid.GetStore().DataSource = col;
            AvailableDataSourcesGrid.GetStore().DataBind();
        }
    }

    protected void DataSourceLinksSave(object sender, DirectEventArgs e)
    {
        RowSelectionModel sm = AvailableDataSourcesGrid.SelectionModel.Primary as RowSelectionModel;
        RowSelectionModel DataSchemaRow = DataSchemasGrid.SelectionModel.Primary as RowSelectionModel;

        string DataSchemaID = DataSchemaRow.SelectedRecordID;
        if (sm.SelectedRows.Count > 0)
        {
            foreach (SelectedRow row in sm.SelectedRows)
            {
                DataSource DataSource = new DataSource(row.RecordID);
                if (DataSource != null)
                    try
                    {
                        DataSource.DataSchemaID = new Guid(DataSchemaID);
                        DataSource.UserId = AuthHelper.GetLoggedInUserId;
                        DataSource.Save();
                        Auditing.Log("DataSchemas.AddDataSourceLink", new Dictionary<string, object> {
                                { "DataSchemaID", DataSource.DataSchemaID},
                                { "DataSchemaCode", DataSource.DataSchema.Code},
                                { "DataSourceID", DataSource.Id },
                                { "DataSourceCode", DataSource.Code }
                            });
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "DataSchemas.LinkDataSource_Click");
                        MessageBoxes.Error(ex, "Error", "Unable to link DataSource");
                    }
            }
            DataSourcesGridStore.DataBind();
            AvailableDataSourcesWindow.Hide();
        }
        else
        {
            MessageBoxes.Error("Invalid Selection", "Select at least one DataSource");
        }
    }

    #endregion
}