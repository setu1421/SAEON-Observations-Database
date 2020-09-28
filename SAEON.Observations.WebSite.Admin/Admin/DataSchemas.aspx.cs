using Ext.Net;
using SAEON.Logs;
using SAEON.Observations.Data;
using SubSonic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Transactions;
using System.Web.UI.WebControls;

public partial class Admin_DataSchemas : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var showValidate = ConfigurationManager.AppSettings["ShowValidateButton"] == "true" && Request.IsLocal;
        btnValidate.Hidden = !showValidate;
        btnValidateSchemaColumn.Hidden = !showValidate;
        if (!X.IsAjaxRequest)
        {
            DataSourceTypeStore.DataSource = new DataSourceTypeCollection().OrderByAsc(DataSourceType.Columns.Code).Load();
            DataSourceTypeStore.DataBind();
            SchemaColumnTypeStore.DataSource = new SchemaColumnTypeCollection().OrderByAsc(SchemaColumnType.Columns.Name).Load();
            SchemaColumnTypeStore.DataBind();
            PhenomenonStore.DataSource = new PhenomenonCollection().OrderByAsc(Phenomenon.Columns.Name).Load();
            PhenomenonStore.DataBind();
            SchemaPickerStore.DataSource = new DataSchemaCollection().OrderByAsc(DataSchema.Columns.Name).Load();
            SchemaPickerStore.DataBind();
        }
    }

    #region Data Schemas
    protected void DataSchemasGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        DataSchemasGridStore.DataSource = DataSchemRepository.GetPagedList(e, e.Parameters[DataSchemasGridFilters.ParamPrefix]);
        SchemaPickerStore.DataSource = new DataSchemaCollection().OrderByAsc(DataSchema.Columns.Name).Load();
        SchemaPickerStore.DataBind();
    }

    protected void ValidateField(object sender, RemoteValidationEventArgs e)
    {
        //SAEONLogs.Information("ValidateField: {ID}", e.ID);
        DataSchemaCollection col = new DataSchemaCollection();
        string checkColumn = String.Empty;
        string errorMessage = String.Empty;
        e.Success = true;
        tfCode.HasValue();
        tfName.HasValue();

        if (e.ID == "tfCode" || e.ID == "tfName")
        {
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

            if (tfID.IsEmpty)
            {
                col = new DataSchemaCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
            }
            else
            {
                col = new DataSchemaCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(DataSchema.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();
            }

            if (col.Count > 0)
            {
                e.Success = false;
                e.ErrorMessage = errorMessage;
            }
        }
    }

    protected void DataSchemasGridStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();
        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        //string js = BaseRepository.BuildExportQ("VDataSchema", gridData, visCols, sortCol, sortDir);
        //BaseRepository.doExport(type, js);

        BaseRepository.Export("VDataSchema", gridData, visCols, sortCol, sortDir, type, "DataSchemas", Response);
    }

    protected void Save(object sender, DirectEventArgs e)
    {
        using (SAEONLogs.MethodCall(GetType()))
        {
            try
            {

                DataSchema schema = new DataSchema();

                if (!tfID.HasValue())
                    schema.Id = Guid.NewGuid();
                else
                    schema = new DataSchema(tfID.Text);
                if (tfCode.HasValue())
                    schema.Code = tfCode.Text;
                if (tfName.HasValue())
                    schema.Name = tfName.Text;
                if (tfDescription.HasValue())
                    schema.Description = tfDescription.Text;

                if (!nfIgnoreFirst.IsEmpty)
                {
                    schema.IgnoreFirst = Int32.Parse(nfIgnoreFirst.Text);
                }
                else
                {
                    schema.IgnoreFirst = 0;
                }

                if (!nfIgnoreLast.IsEmpty)
                {
                    schema.IgnoreLast = Int32.Parse(nfIgnoreLast.Text);
                }
                else
                {
                    schema.IgnoreLast = 0;
                }

                schema.Condition = Utilities.NullIfEmpty(tfCondition.Text);

                if (tfSplit.HasValue())
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
                schema.HasColumnNames = cbHasColumnNames.Checked;
                schema.Delimiter = cbDelimiter.SelectedItem.Value;

                schema.UserId = AuthHelper.GetLoggedInUserId;
                schema.Save();
                Auditing.Log(GetType(), new MethodCallParameters {
                { "ID", schema.Id }, { "Code", schema.Code }, { "Name", schema.Name } });
                DataSchemasGrid.DataBind();
                SchemaPickerStore.DataBind();

                DetailWindow.Hide();
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                MessageBoxes.Error(ex, "Error", "Unable to save instrument");
            }
        }
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
        SchemaPickerStore.DataSource = new DataSchemaCollection().Where(DataSchema.Columns.Id, SubSonic.Comparison.NotEquals, masterID).OrderByAsc(DataSchema.Columns.Name).Load();
        SchemaPickerStore.DataBind();
    }

    [DirectMethod]
#pragma warning disable IDE1006 // Naming Styles
    public void cbDataSourceTypeSelect(object sender, DirectEventArgs e)
#pragma warning restore IDE1006 // Naming Styles
    {
        DataSourceType dsType = new DataSourceType(cbDataSourceType.Value);
        if (dsType.Code == "CSV")
        {
            cbDelimiter.AllowBlank = false;
        }
        else
        {
            cbDelimiter.AllowBlank = true;
            cbDelimiter.ClearValue();
            cbDelimiter.ClearInvalid();
            cbDelimiter.MarkInvalid();
        }
    }

    [DirectMethod]
    public void ConfirmDeleteSchema(Guid aID)
    {
        List<string> dataSources = new DataSourceCollection().Where(DataSource.Columns.DataSchemaID, aID).OrderByAsc(DataSource.Columns.Name).Load().Select(ds => ds.Name).ToList();
        List<string> sensors = new SensorCollection().Where(Sensor.Columns.DataSchemaID, aID).OrderByAsc(Sensor.Columns.Name).Load().Select(s => s.Name).ToList();
        if (dataSources.Any() && sensors.Any())
        {
            MessageBoxes.Error("Error", $"Cannot delete data schema as it is used in data source(s) {string.Join(", ", dataSources)} and sensor(s) {string.Join(", ", sensors)}");
        }
        else if (dataSources.Any())
        {
            MessageBoxes.Error("Error", $"Cannot delete data schema as it is used in data source(s) {string.Join(", ", dataSources)}");
        }
        else if (sensors.Any())
        {
            MessageBoxes.Error("Error", $"Cannot delete data schema as it is used in sensor(s) {string.Join(", ", sensors)}");
        }
        else
        {
            MessageBoxes.Confirm(
                "Confirm Delete",
                String.Format("DirectCall.DeleteSchema(\"{0}\",{{ eventMask: {{ showMask: true}}}});", aID.ToString()),
                "Are you sure you want to delete this schema?");
        }
    }

    [DirectMethod]
    public void DeleteSchema(Guid aID)
    {
        using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "aID", aID } }))
        {
            try
            {
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 15, 0)))
                {
                    using (SharedDbConnectionScope connScope = new SharedDbConnectionScope())
                    {
                        SchemaColumnCollection cols = new SchemaColumnCollection().Where(SchemaColumn.Columns.DataSchemaID, aID).Load();
                        foreach (var col in cols)
                        {
                            SchemaColumn.Delete(col.Id);
                        }

                        DataSchema.Delete(aID);
                    }
                    ts.Complete();
                }
                Auditing.Log(GetType(), new MethodCallParameters { { "ID", aID } });
                DataSchemasGrid.DataBind();
                SchemaColumnsGrid.DataBind();
                SchemaPickerStore.DataBind();
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                MessageBoxes.Error(ex, "Error", "Unable to delete Schema");
            }
        }
    }

    #endregion

    #region Schema Columns
    protected void SchemaColumnsGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        using (SAEONLogs.MethodCall(GetType()))
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
                    SAEONLogs.Exception(ex);
                    MessageBoxes.Error(ex, "Error", "Unable to refresh SchemaColumns grid");
                }
            }
        }
    }

    protected void ValidateColumnField(object sender, RemoteValidationEventArgs e)
    {
        using (SAEONLogs.MethodCall(GetType()))
        {
            e.Success = true;
            SAEONLogs.Verbose("Column: {ID}", e.ID);
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
            else if (e.ID == "cbSchemaColumnType")
            {
                var columnType = new SchemaColumnType(cbSchemaColumnType.SelectedItem.Value);
                if ((columnType.Name != "Ignore") && (columnType.Name != "Comment") && (columnType.Name != "Offering"))
                {
                    RowSelectionModel masterRow = DataSchemasGrid.SelectionModel.Primary as RowSelectionModel;
                    var masterID = new Guid(masterRow.SelectedRecordID);
                    SchemaColumnCollection col = new SchemaColumnCollection()
                        .Where(SchemaColumn.Columns.DataSchemaID, masterID)
                        .Where(SchemaColumn.Columns.SchemaColumnTypeID, cbSchemaColumnType.SelectedItem.Value)
                        .Load();
                    if (col.Any())
                    {
                        e.Success = false;
                        e.ErrorMessage = $"Schema already has a {columnType.Name} column";
                    }
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
            else if ((e.ID == "cbOffering") || (e.ID == "cbUnitOfMeasure"))
            {
                RowSelectionModel masterRow = DataSchemasGrid.SelectionModel.Primary as RowSelectionModel;
                var masterID = new Guid(masterRow.SelectedRecordID);
                var q = new Select().From(SchemaColumn.Schema)
                    .Where(SchemaColumn.Columns.DataSchemaID).IsEqualTo(masterID)
                    .And(SchemaColumn.Columns.SchemaColumnTypeID).IsEqualTo(cbSchemaColumnType.SelectedItem.Value)
                    .AndExpression(SchemaColumn.Columns.PhenomenonOfferingID).IsNull().Or(SchemaColumn.Columns.PhenomenonOfferingID).IsEqualTo(cbOffering.SelectedItem.Value).CloseExpression()
                    .AndExpression(SchemaColumn.Columns.PhenomenonUOMID).IsNull().Or(SchemaColumn.Columns.PhenomenonUOMID).IsEqualTo(cbUnitOfMeasure.SelectedItem.Value).CloseExpression();
                var col = q.ExecuteAsCollection<SchemaColumnCollection>();
                //SAEONLogs.Verbose("Count: {count} Any: {any} SQL: {sql}", col.Count, col.Any(), q.BuildSqlStatement());
                if (col.Any())
                {
                    e.Success = false;
                    e.ErrorMessage = $"Schema already has such an offering";
                }
            }
        }
    }

    protected void SchemaColumnSave(object sender, DirectEventArgs e)
    {
        using (SAEONLogs.MethodCall(GetType()))
        {
            try
            {
                RowSelectionModel masterRow = DataSchemasGrid.SelectionModel.Primary as RowSelectionModel;
                var masterID = new Guid(masterRow.SelectedRecordID);
                SchemaColumn schemaColumn = new SchemaColumn(Utilities.MakeGuid(SchemaColumnID.Value))
                {
                    DataSchemaID = masterID
                };
                SqlQuery qry = new Select(Aggregate.Max(SchemaColumn.Columns.Number))
                    .From(SchemaColumn.Schema)
                    .Where(SchemaColumn.Columns.DataSchemaID).IsEqualTo(masterID);
                if (Utilities.MakeGuid(SchemaColumnID.Value) == Guid.Empty)
                {
                    schemaColumn.Number = qry.ExecuteScalar<int>() + 1;
                }

                schemaColumn.Name = Utilities.NullIfEmpty(tfColumnName.Text);
                schemaColumn.SchemaColumnTypeID = new Guid(cbSchemaColumnType.SelectedItem.Value.Trim());
                DataSchema schema = new DataSchema(masterID);
                DataSourceType dataSourceType = new DataSourceType(schema.DataSourceTypeID);
                if (dataSourceType.Code == "CSV")
                {
                    schemaColumn.Width = null;
                }
                else if (string.IsNullOrEmpty(nfWidth.Text.Trim()))
                {
                    schemaColumn.Width = null;
                }
                else
                {
                    schemaColumn.Width = int.Parse(nfWidth.Text.Trim());
                }

                schemaColumn.Format = null;
                schemaColumn.PhenomenonID = null;
                schemaColumn.PhenomenonOfferingID = null;
                schemaColumn.PhenomenonUOMID = null;
                schemaColumn.EmptyValue = null;
                schemaColumn.FixedTime = null;
                switch (cbSchemaColumnType.SelectedItem.Text)
                {
                    case "Date":
                    case "Time":
                        schemaColumn.Format = cbFormat.SelectedItem.Value.Trim();
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
                        {
                            schemaColumn.FixedTime = ttFixedTime.SelectedTime.ToString();
                        }

                        break;
                }
                schemaColumn.UserId = AuthHelper.GetLoggedInUserId;
                schemaColumn.Save();
                Auditing.Log(GetType(), new MethodCallParameters {
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
                SchemaColumnWindow.Hide();
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                MessageBoxes.Error(ex, "Error", "Unable to save SchemaColumn");
            }
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
        using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "aID", aID } }))
        {
            try
            {
                SchemaColumn.Delete(aID);
                Auditing.Log(GetType(), new MethodCallParameters { { "ID", aID } });
                SchemaColumnsGrid.DataBind();
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                MessageBoxes.Error(ex, "Error", "Unable to delete SchemaColumn");
            }
        }
    }

    [DirectMethod]
    public void AddSchemaColumnClick(object sender, DirectEventArgs e)
    {
        //X.Redirect(X.ResourceManager.ResolveUrl("Admin/Sites"));
    }

    [DirectMethod]
#pragma warning disable IDE1006 // Naming Styles
    public void cbPhenomenonSelect(object sender, DirectEventArgs e)
#pragma warning restore IDE1006 // Naming Styles
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
        cbSchemaColumnType.MarkAsValid();
        SetFields();
        cbPhenomenon.Value = phenomenonID;
        cbPhenomenonSelect(cbPhenomenon, new DirectEventArgs(null));
        cbPhenomenon.MarkAsValid();
        cbOffering.Value = offeringID;
        cbOffering.MarkAsValid();
        cbUnitOfMeasure.Value = unitOfMeasureID;
        cbUnitOfMeasure.MarkAsValid();
        tfColumnName.Disabled = true;
    }

    [DirectMethod]
    public void SetFields()
    {
        SchemaColumnFormPanel.ClearInvalid();
        bool hidden = true;
        cbFormat.AllowBlank = true;
        cbFormat.ClearInvalid();
        cbFormat.MarkAsValid();
        cbFormat.Hidden = hidden;
        cbPhenomenon.AllowBlank = true;
        cbPhenomenon.ForceSelection = false;
        cbPhenomenon.ClearInvalid();
        cbPhenomenon.MarkAsValid();
        cbPhenomenon.Hidden = hidden;
        cbOffering.AllowBlank = true;
        cbOffering.ForceSelection = false;
        cbOffering.ClearInvalid();
        cbOffering.MarkAsValid();
        cbOffering.Hidden = hidden;
        cbUnitOfMeasure.AllowBlank = true;
        cbUnitOfMeasure.ForceSelection = false;
        cbUnitOfMeasure.ClearInvalid();
        cbUnitOfMeasure.MarkAsValid();
        cbUnitOfMeasure.Hidden = hidden;
        tfEmptyValue.Hidden = hidden;
        ttFixedTime.AllowBlank = true;
        ttFixedTime.ClearInvalid();
        ttFixedTime.MarkAsValid();
        ttFixedTime.Hidden = hidden;
        switch (cbSchemaColumnType.SelectedItem.Text)
        {
            case "Date":
            case "Time":
                cbFormat.AllowBlank = false;
                cbFormat.Hidden = false;
                break;
            case "Ignore":
            case "Elevation":
            case "Latitude":
            case "Longitude":
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
#pragma warning disable IDE1006 // Naming Styles
    public void cbSchemaColumnTypeSelect(object sender, DirectEventArgs e)
#pragma warning restore IDE1006 // Naming Styles
    {
        SetFields();
    }

    [DirectMethod]
    public void SchemaColumnUp(Guid aID)
    {
        using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "aID", aID } }))
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
                {
                    using (var ts = new TransactionScope())
                    {
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
                        }
                        ts.Complete();
                    }
                }

                Auditing.Log(GetType(), new MethodCallParameters { { "ID", aID } });
                SchemaColumnsGrid.DataBind();
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                MessageBoxes.Error(ex, "Error", "Unable to move SchemaColumn up");
            }
        }
    }

    [DirectMethod]
    public void SchemaColumnDown(Guid aID)
    {
        using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "aID", aID } }))
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
                {
                    using (var ts = new TransactionScope())
                    {
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
                        }
                        ts.Complete();
                    }
                }

                Auditing.Log(GetType(), new MethodCallParameters { { "ID", aID } });
                SchemaColumnsGrid.DataBind();
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                MessageBoxes.Error(ex, "Error", "Unable to move SchemaColumn down");
            }
        }
    }
    #endregion

    #region Data Sources
    protected void DataSourcesGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        using (SAEONLogs.MethodCall(GetType()))
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
                    SAEONLogs.Exception(ex);
                    MessageBoxes.Error(ex, "Error", "Unable to refresh DataSources grid");
                }
            }
        }
    }

    [DirectMethod]
    public void ConfirmDeleteDataSource(Guid aID)
    {
        MessageBoxes.Confirm(
            "Confirm Delete",
            $"DirectCall.DeleteDataSource(\"{aID.ToString()}\",{{ eventMask: {{ showMask: true}}}});",
            "Are you sure you want to delete this Data Source?");
    }

    [DirectMethod]
    public void DeleteDataSource(Guid aID)
    {
        using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "aID", aID } }))
        {
            try
            {
                DataSource dataSource = new DataSource(aID)
                {
                    DataSchemaID = null
                };
                dataSource.Save();
                Auditing.Log(GetType(), new MethodCallParameters { { "ID", aID } });
                DataSourcesGrid.DataBind();
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                MessageBoxes.Error(ex, "Error", "Unable to delete DataSource");
            }
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
        using (SAEONLogs.MethodCall(GetType()))
        {
            RowSelectionModel sm = AvailableDataSourcesGrid.SelectionModel.Primary as RowSelectionModel;
            RowSelectionModel DataSchemaRow = DataSchemasGrid.SelectionModel.Primary as RowSelectionModel;

            string DataSchemaID = DataSchemaRow.SelectedRecordID;
            if (sm.SelectedRows.Count > 0)
            {
                foreach (SelectedRow row in sm.SelectedRows)
                {
                    DataSource dataSource = new DataSource(row.RecordID);
                    if (dataSource != null)
                    {
                        try
                        {
                            dataSource.DataSchemaID = new Guid(DataSchemaID);
                            dataSource.UserId = AuthHelper.GetLoggedInUserId;
                            dataSource.Save();
                            Auditing.Log(GetType(), new MethodCallParameters {
                                { "DataSchemaID", dataSource.DataSchemaID},
                                { "DataSchemaCode", dataSource.DataSchema.Code},
                                { "DataSourceID", dataSource.Id },
                                { "DataSourceCode", dataSource.Code }
                            });
                        }
                        catch (Exception ex)
                        {
                            SAEONLogs.Exception(ex);
                            MessageBoxes.Error(ex, "Error", "Unable to link DataSource");
                        }
                    }
                }
                DataSourcesGridStore.DataBind();
                AvailableDataSourcesWindow.Hide();
                tfColumnName.Disabled = false;
            }
            else
            {
                MessageBoxes.Error("Invalid Selection", "Select at least one DataSource");
            }
        }
    }

    #endregion

    #region SchemaPicker
    public void SchemaPickerOk(object sender, DirectEventArgs e)
    {
        RowSelectionModel masterRow = DataSchemasGrid.SelectionModel.Primary as RowSelectionModel;
        var masterID = new Guid(masterRow.SelectedRecordID);
        var schema = new DataSchema(masterID);
        MessageBoxes.Confirm(
           "Confirm Copy",
           $"DirectCall.CopyDataSchema(\"{cbSchemaPickerID.SelectedItem.Value}\",{{ eventMask: {{ showMask: true}}}});",
           $"Are you sure you want delete all {schema.Name} columns and copy columns from {cbSchemaPickerID.SelectedItem.Text}?");
    }

    [DirectMethod]
    public void CopyDataSchema(Guid aId)
    {
        using (SAEONLogs.MethodCall(GetType()))
        {
            try
            {
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 15, 0)))
                {
                    using (SharedDbConnectionScope connScope = new SharedDbConnectionScope())
                    {
                        RowSelectionModel masterRow = DataSchemasGrid.SelectionModel.Primary as RowSelectionModel;
                        var masterID = new Guid(masterRow.SelectedRecordID);
                        SchemaColumn.Delete(SchemaColumn.Columns.DataSchemaID, masterID);
                        var oldSchemaCols = new SchemaColumnCollection().Where(SchemaColumn.Columns.DataSchemaID, aId).Load();
                        foreach (var oldSchemaCol in oldSchemaCols)
                        {
                            var newSchemaCol = new SchemaColumn
                            {
                                DataSchemaID = masterID,
                                Number = oldSchemaCol.Number,
                                Name = oldSchemaCol.Name,
                                SchemaColumnTypeID = oldSchemaCol.SchemaColumnTypeID,
                                Width = oldSchemaCol.Width,
                                Format = oldSchemaCol.Format,
                                PhenomenonID = oldSchemaCol.PhenomenonID,
                                PhenomenonOfferingID = oldSchemaCol.PhenomenonOfferingID,
                                PhenomenonUOMID = oldSchemaCol.PhenomenonUOMID,
                                EmptyValue = oldSchemaCol.EmptyValue,
                                FixedTime = oldSchemaCol.FixedTime,
                                UserId = AuthHelper.GetLoggedInUserId
                            };
                            newSchemaCol.Save();
                            Auditing.Log(GetType(), new MethodCallParameters {
                                { "DataSchemaID", newSchemaCol.DataSchemaID },
                                { "DataSchemaCode", newSchemaCol.DataSchema.Code },
                                { "Name", newSchemaCol.Name },
                                { "newSchemaColType", newSchemaCol.SchemaColumnType.Name },
                                { "Width", newSchemaCol?.Width },
                                { "Format", newSchemaCol.Format },
                                { "PhenomenonID", newSchemaCol.PhenomenonID },
                                { "PhenomenonCode", newSchemaCol?.Phenomenon?.Code },
                                { "PhenomenonOfferingID", newSchemaCol.PhenomenonOfferingID },
                                { "PhenomenonOfferingCode", newSchemaCol?.PhenomenonOffering?.Offering.Code },
                                { "PhenomenonUnitOfMeasureID", newSchemaCol.PhenomenonUOMID },
                                { "PhenomenonUnitOfMeasureCode", newSchemaCol?.PhenomenonUOM?.UnitOfMeasure.Code },
                                { "EmptyValue", newSchemaCol.EmptyValue },
                                { "FixedTime", newSchemaCol.FixedTime }
                            });
                        }
                    }
                    ts.Complete();
                }
                SchemaColumnsGrid.DataBind();
                SchemaPickerWindow.Hide();
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                MessageBoxes.Error("Error", "Unable to copy schema");
            }
        }
    }
    #endregion

    #region SchemaCopy
    protected void ValidateSchemaCopyField(object sender, RemoteValidationEventArgs e)
    {
        DataSchemaCollection col = new DataSchemaCollection();
        string checkColumn = String.Empty;
        string errorMessage = String.Empty;
        e.Success = true;

        if (e.ID == "tfSchemaCopyCode" || e.ID == "tfSchemaCopyName")
        {
            if (e.ID == "tfSchemaCopyCode")
            {
                checkColumn = DataSchema.Columns.Code;
                errorMessage = "The specified Data Schema Code already exists";
            }
            else if (e.ID == "tfSchemaCopyName")
            {
                checkColumn = DataSchema.Columns.Name;
                errorMessage = "The specified Data Schema Name already exists";
            }

            col = new DataSchemaCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
            if (col.Count > 0)
            {
                e.Success = false;
                e.ErrorMessage = errorMessage;
            }
        }
    }

    [DirectMethod]
    public void SetSchemaCopyFields()
    {
        SchemaCopyFormPanel.ClearInvalid();
        tfSchemaCopyCode.MarkInvalid();
        tfSchemaCopyName.MarkInvalid();
        tfSchemaCopyDescription.MarkAsValid();
    }

    public void SchemaCopySave(object sender, DirectEventArgs e)
    {
        using (SAEONLogs.MethodCall(GetType()))
        {
            try
            {
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 15, 0)))
                {
                    using (SharedDbConnectionScope connScope = new SharedDbConnectionScope())
                    {
                        var masterID = new Guid(tfSchemaCopyID.Text);
                        var oldSchema = new DataSchema(masterID);
                        var newSchema = new DataSchema
                        {
                            Code = tfSchemaCopyCode.Text.Trim(),
                            Name = tfSchemaCopyName.Text.Trim(),
                            Description = tfSchemaCopyDescription.Text.Trim(),
                            DataSourceTypeID = oldSchema.DataSourceTypeID,
                            Delimiter = oldSchema.Delimiter,
                            IgnoreFirst = oldSchema.IgnoreFirst,
                            IgnoreLast = oldSchema.IgnoreLast,
                            Condition = oldSchema.Condition,
                            SplitIndex = oldSchema.SplitIndex,
                            SplitSelector = oldSchema.SplitSelector,
                            HasColumnNames = oldSchema.HasColumnNames,
                            UserId = AuthHelper.GetLoggedInUserId
                        };
                        newSchema.Save();
                        Auditing.Log(GetType(), new MethodCallParameters { { "ID", newSchema.Id }, { "Code", newSchema.Code }, { "Name", newSchema.Name } });
                        var oldSchemaCols = new SchemaColumnCollection().Where(SchemaColumn.Columns.DataSchemaID, masterID).Load();
                        foreach (var oldSchemaCol in oldSchemaCols)
                        {
                            var newSchemaCol = new SchemaColumn
                            {
                                DataSchemaID = newSchema.Id,
                                Number = oldSchemaCol.Number,
                                Name = oldSchemaCol.Name,
                                SchemaColumnTypeID = oldSchemaCol.SchemaColumnTypeID,
                                Width = oldSchemaCol.Width,
                                Format = oldSchemaCol.Format,
                                PhenomenonID = oldSchemaCol.PhenomenonID,
                                PhenomenonOfferingID = oldSchemaCol.PhenomenonOfferingID,
                                PhenomenonUOMID = oldSchemaCol.PhenomenonUOMID,
                                EmptyValue = oldSchemaCol.EmptyValue,
                                FixedTime = oldSchemaCol.FixedTime,
                                UserId = AuthHelper.GetLoggedInUserId
                            };
                            newSchemaCol.Save();
                            Auditing.Log(GetType(), new MethodCallParameters {
                                { "DataSchemaID", newSchemaCol.DataSchemaID },
                                { "DataSchemaCode", newSchemaCol.DataSchema.Code },
                                { "Name", newSchemaCol.Name },
                                { "newSchemaColType", newSchemaCol.SchemaColumnType.Name },
                                { "Width", newSchemaCol?.Width },
                                { "Format", newSchemaCol.Format },
                                { "PhenomenonID", newSchemaCol.PhenomenonID },
                                { "PhenomenonCode", newSchemaCol?.Phenomenon?.Code },
                                { "PhenomenonOfferingID", newSchemaCol.PhenomenonOfferingID },
                                { "PhenomenonOfferingCode", newSchemaCol?.PhenomenonOffering?.Offering.Code },
                                { "PhenomenonUnitOfMeasureID", newSchemaCol.PhenomenonUOMID },
                                { "PhenomenonUnitOfMeasureCode", newSchemaCol?.PhenomenonUOM?.UnitOfMeasure.Code },
                                { "EmptyValue", newSchemaCol.EmptyValue },
                                { "FixedTime", newSchemaCol.FixedTime }
                            });
                        }
                        ts.Complete();
                    }
                    DataSchemasGrid.DataBind();
                    SchemaCopyWindow.Hide();
                }
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                MessageBoxes.Error("Error", "Unable to copy schema");
            }
        }
    }
    #endregion
}