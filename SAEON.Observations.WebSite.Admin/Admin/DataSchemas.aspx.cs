﻿using Ext.Net;
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

    protected void SchemaColumnAddSave(object sender, DirectEventArgs e)
    {
        try
        {
            RowSelectionModel masterRow = DataSchemasGrid.SelectionModel.Primary as RowSelectionModel;
            var masterID = new Guid(masterRow.SelectedRecordID);
            SchemaColumn schemaColumn = new SchemaColumn(Utilities.MakeGuid(SchemaColumnAddID.Value));
            schemaColumn.DataSchemaID = masterID;
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
            switch (cbSchemaColumnType.SelectedItem.Text)
            {
                case "Date":
                    schemaColumn.Format = cbFormat.SelectedItem.Value.Trim();
                    schemaColumn.PhenomenonID = null;
                    schemaColumn.PhenomenonOfferingID = null;
                    schemaColumn.PhenomenonUOMID = null;
                    break;
                case "Time":
                    schemaColumn.Format = cbFormat.SelectedItem.Value.Trim();
                    break;
                case "Offering":
                    schemaColumn.Format = null;
                    schemaColumn.PhenomenonID = new Guid(cbPhenomenon.SelectedItem.Value);
                    schemaColumn.PhenomenonOfferingID = new Guid(cbOffering.SelectedItem.Value);
                    schemaColumn.PhenomenonUOMID = new Guid(cbUnitOfMeasure.SelectedItem.Value);
                    schemaColumn.EmptyValue = Utilities.NullIfEmpty(tfEmptyValue.Text);
                    schemaColumn.FixedTime = null;
                    break;
                case "Fixed Time":
                    schemaColumn.PhenomenonID = new Guid(cbPhenomenon.SelectedItem.Value);
                    schemaColumn.PhenomenonOfferingID = new Guid(cbOffering.SelectedItem.Value);
                    schemaColumn.PhenomenonUOMID = new Guid(cbUnitOfMeasure.SelectedItem.Value);
                    schemaColumn.EmptyValue = Utilities.NullIfEmpty(tfEmptyValue.Text);
                    if (string.IsNullOrEmpty(ttFixedTime.Text.Trim()))
                        schemaColumn.FixedTime = null;
                    else
                        schemaColumn.FixedTime = int.Parse(ttFixedTime.Text.Trim());
                    break;
                default:
                    schemaColumn.Format = null;
                    schemaColumn.PhenomenonID = null;
                    schemaColumn.PhenomenonOfferingID = null;
                    schemaColumn.PhenomenonUOMID = null;
                    schemaColumn.EmptyValue = null;
                    schemaColumn.FixedTime = null;
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
                { "PhenomenonCode", schemaColumn?.Phenomenon.Code },
                { "PhenomenonOfferingID", schemaColumn.PhenomenonOfferingID },
                { "PhenomenonOfferingCode", schemaColumn?.PhenomenonOffering.Offering.Code },
                { "PhenomenonUnitOfMeasureID", schemaColumn.PhenomenonUOMID },
                { "PhenomenonUnitOfMeasureCode", schemaColumn?.PhenomenonUOM.UnitOfMeasure.Code },
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

    private void SetFields()
    {
        cbFormat.AllowBlank = true;
        cbFormat.MarkAsValid();
        cbFormat.Hidden = true;
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
                cbFormat.AllowBlank = false;
                cbFormat.Hidden = false;
                break;
            case "Time":
                cbFormat.AllowBlank = false;
                cbFormat.Hidden = false;
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

    #endregion
}