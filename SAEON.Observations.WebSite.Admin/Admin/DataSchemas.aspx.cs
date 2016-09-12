using Ext.Net;
using FileHelpers.Dynamic;
using SAEON.Observations.Data;
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
            //this.cbDataSourceType.GetStore().DataSource = new DataSourceTypeCollection().OrderByAsc(DataSourceType.Columns.Code).Load();
            //this.cbDataSourceType.GetStore().DataBind();

            //this.cbPhenomenon.GetStore().DataSource = new PhenomenonCollection().OrderByAsc(Phenomenon.Columns.Name).Load();
            //this.cbPhenomenon.DataBind();
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


        if (e.ExtraParams.Count > 0)
        {
            if (e.ExtraParams["Name"] != null)
            {
                try
                {
                    DelimitedClassBuilder cb = new DelimitedClassBuilder("ValidationTest");
                    cb.AddField(e.Value.ToString(), typeof(string));

                    e.Success = true;
                }
                catch
                {
                    e.Success = false;
                    e.ErrorMessage = "Name cannot start with a number and may not contain spaces.";
                }
            }
        }

        if (e.ID == "tfCode" || e.ID == "tfName")
        {
            if (e.ID == "tfCode")
            {
                checkColumn = DataSchema.Columns.Code;
                errorMessage = "The specified DataSchema Code already exists";
            }
            else if (e.ID == "tfName")
            {

                try
                {
                    DelimitedClassBuilder cb = new DelimitedClassBuilder(e.Value.ToString());

                    checkColumn = DataSchema.Columns.Name;
                    errorMessage = "The specified DataSchema Name already exists";

                }
                catch
                {
                    e.Success = false;
                    e.ErrorMessage = "Name cannot start with a number and may not contain spaces.";

                    return;
                }
            }

            //if (String.IsNullOrEmpty(tfID.Text.ToString()))
            //    col = new DataSchemaCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
            //else
            //    col = new DataSchemaCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(DataSchema.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();

            //if (col.Count > 0)
            //{
            //    e.Success = false;
            //    e.ErrorMessage = errorMessage;
            //}
            //else
                e.Success = true;
        }
        //else if (e.ID == "DateFieldFormat" || e.ID == "TimeFieldFormat")
        //{
        //    try
        //    {
        //        DateTime.Now.ToString(e.Value.ToString());
        //        e.Success = true;
        //    }
        //    catch
        //    {
        //        e.Success = false;
        //        e.ErrorMessage = "The format specified is invalid.";
        //    }
        //}
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

        Guid dsType = new Guid(cbDataSourceType.SelectedItem.Value);
        schema.DataSourceTypeID = dsType;

        schema.Delimiter = cbDelimiter.SelectedItem.Value;

        schema.UserId = AuthHelper.GetLoggedInUserId;

        //if (!String.IsNullOrEmpty(schema.DataSchemaX))
        //{
        //    ClassBuilder cb;
        //    if (schema.DataSourceTypeID == new Guid(DataSourceType.CSV))
        //        cb = (DelimitedClassBuilder)ClassBuilder.LoadFromXmlString(schema.DataSchemaX);
        //    else
        //        cb = (FixedLengthClassBuilder)ClassBuilder.LoadFromXmlString(schema.DataSchemaX);

        //    string cs = cb.GetClassSourceCode(NetLanguage.CSharp);

        //    Type builder = ClassBuilder.ClassFromString(cs);

        //    var engine = new FileHelperEngine(cb.CreateRecordClass());

        //    schema.DataSchemaX = cb.SaveToXmlString();
        //}

        schema.Save();

        DataSchemasGrid.DataBind();

        this.DetailWindow.Hide();
    }

    #endregion
}