using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Observations.Data;
using Ext.Net;
using FileHelpers.Dynamic;
using FileHelpers;
using SubSonic;
using System.IO;
using System.Data;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

public partial class _DataSchema : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!X.IsAjaxRequest)
        {
            this.cbDataSourceType.GetStore().DataSource = new DataSourceTypeCollection().OrderByAsc(DataSourceType.Columns.Code).Load();
            this.cbDataSourceType.GetStore().DataBind();

            this.cbPhenomenon.GetStore().DataSource = new PhenomenonCollection().OrderByAsc(Phenomenon.Columns.Name).Load();
            this.cbPhenomenon.DataBind();

        }
    }

    protected void DataSchemaStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        this.DataSchemaGrid.GetStore().DataSource = DataSchemRepository.GetPagedList(e, e.Parameters[this.GridFilters1.ParamPrefix]);
    }

    protected void Store22_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {

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
        else if (e.ID == "DateFieldFormat" || e.ID == "TimeFieldFormat")
        {
            try
            {
                DateTime.Now.ToString(e.Value.ToString());
                e.Success = true;
            }
            catch
            {
                e.Success = false;
                e.ErrorMessage = "The format specified is invalid.";
            }
        }
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

        if (!String.IsNullOrEmpty(schema.DataSchemaX))
        {
            ClassBuilder cb;
            if (schema.DataSourceTypeID == new Guid(DataSourceType.CSV))
                cb = (DelimitedClassBuilder)ClassBuilder.LoadFromXmlString(schema.DataSchemaX);
            else
                cb = (FixedLengthClassBuilder)ClassBuilder.LoadFromXmlString(schema.DataSchemaX);

            string cs = cb.GetClassSourceCode(NetLanguage.CSharp);

            Type builder = ClassBuilder.ClassFromString(cs);

            var engine = new FileHelperEngine(cb.CreateRecordClass());

            schema.DataSchemaX = cb.SaveToXmlString();
        }

        schema.Save();

        DataSchemaGrid.DataBind();

        this.DetailWindow.Hide();
    }

    protected void onCommand(object sender, DirectEventArgs e)
    {

        string ActionType = e.ExtraParams["type"];
        string recordID = e.ExtraParams["id"];

        if (ActionType == "Fields")
        {
            hfSchemaID.Text = recordID;

            DataSchema ds = new DataSchema(recordID);
            List<FieldDefinition> fields = new List<FieldDefinition>();

            if (!String.IsNullOrEmpty(ds.DataSchemaX))
            {

                if (ds.DataSourceTypeID == new Guid(DataSourceType.CSV))
                {
                    DelimitedClassBuilder cb = (DelimitedClassBuilder)ClassBuilder.LoadFromXmlString(ds.DataSchemaX);

                    for (int i = 0; i < cb.Fields.Length; i++)
                    {

                        FieldDefinition field = new FieldDefinition();

                        DelimitedFieldBuilder bd = cb.Fields[i];

                        field.id = Guid.NewGuid().ToString();
                        field.Name = bd.FieldName;

                        if (bd.ValueDate)
                        {
                            field.Datefield = bd.ValueDate;
                            field.Dateformat = bd.ValueDateformat;
                        }
                        else if (bd.ValueTime)
                        {
                            field.Timefield = bd.ValueTime;
                            field.Timeformat = bd.ValueTimeformat;
                        }
                        else if (bd.FieldValueDiscarded)
                            field.Ignorefield = true;
                        else if (bd.ValueComment)
                            field.Commentfield = true;
                        else
                        {
                            if (bd.ValueEmpty)
                                field.EmptyValue = bd.EmptyValue;

                            field.OfferingID = bd.PhenomenonOfferingID;
                            PhenomenonOffering off = new PhenomenonOffering(bd.PhenomenonOfferingID);
                            if (off != null)
                            {
                                field.Offeringfield = true;
                                field.PhenomenonID = off.PhenomenonID;

                                if (!String.IsNullOrEmpty(bd.FixedTime))
                                {
                                    field.FixedTimeField = true;
                                    field.FixedTime = bd.FixedTime;
                                    FieldFixedTimeValue.AllowBlank = false;
                                    FixedTimePanel.Show();
                                }
                            }

                            field.UnitofMeasureID = bd.PhenomenonUOMID;
                        }

                        fields.Add(field);
                    }
                }
                else if (ds.DataSourceTypeID == new Guid(DataSourceType.Flat))
                {
                    FixedLengthClassBuilder cb = (FixedLengthClassBuilder)ClassBuilder.LoadFromXmlString(ds.DataSchemaX);

                    for (int i = 0; i < cb.Fields.Length; i++)
                    {
                        FieldDefinition field = new FieldDefinition();

                        FixedFieldBuilder bd = cb.Fields[i];

                        field.id = Guid.NewGuid().ToString();
                        field.Name = bd.FieldName;
                        field.FieldLength = bd.FieldLength;
                        if (bd.ValueDate)
                        {
                            field.Datefield = bd.ValueDate;
                            field.Dateformat = bd.ValueDateformat;
                        }
                        else if (bd.ValueTime)
                        {
                            field.Timefield = bd.ValueTime;
                            field.Timeformat = bd.ValueTimeformat;
                        }
                        else if (bd.FieldValueDiscarded)
                            field.Ignorefield = true;
                        else if (bd.ValueComment)
                            field.Commentfield = true;
                        else
                        {
                            if (bd.ValueEmpty)
                                field.EmptyValue = bd.EmptyValue;

                            field.OfferingID = bd.PhenomenonOfferingID;
                            PhenomenonOffering off = new PhenomenonOffering(bd.PhenomenonOfferingID);
                            if (off != null)
                            {
                                field.Offeringfield = true;
                                field.PhenomenonID = off.PhenomenonID;
                                
                                if (!String.IsNullOrEmpty(bd.FixedTime))
                                {
                                    field.FixedTimeField = true;
                                    field.FixedTime = bd.FixedTime;
                                    FieldFixedTimeValue.AllowBlank = false;
                                    FixedTimePanel.Show();
                                }
                            }

                            field.UnitofMeasureID = bd.PhenomenonUOMID;
                        }

                        fields.Add(field);
                    }
                }

                FieldsStore.DataSource = fields;
                FieldsStore.DataBind();
            }

            if (ds.DataSourceTypeID == new Guid(DataSourceType.CSV))
            {
                DateFieldLength.AllowBlank = true;
                DateFieldLengthPanel.Hide();

                TimeFieldLength.AllowBlank = true;
                TimeFieldLengthPanel.Hide();

                IgnoreFieldLength.AllowBlank = true;
                IgnoreFieldLengthPanel.Hide();

                OfferingFieldLength.AllowBlank = true;
                OfferingFieldLengthPanel.Hide();

                CommentFieldLength.AllowBlank = true;
                CommentFieldLengthPanel.Hide();

                DelimetedFieldsGrid.ColumnModel.SetHidden(1, true);
            }
            else if (ds.DataSourceTypeID == new Guid(DataSourceType.Flat))
            {
                DateFieldLength.AllowBlank = false;
                DateFieldLengthPanel.Show();

                TimeFieldLength.AllowBlank = false;
                TimeFieldLengthPanel.Show();

                IgnoreFieldLength.AllowBlank = false;
                IgnoreFieldLengthPanel.Show();

                OfferingFieldLength.AllowBlank = false;
                OfferingFieldLengthPanel.Show();

                CommentFieldLength.AllowBlank = false;
                CommentFieldLengthPanel.Show();

                DelimetedFieldsGrid.ColumnModel.SetHidden(1, false);
            }

            DefinitionWindow.Show();
        }
        else if (ActionType == "Preview")
        {
            DataSchema ds = new DataSchema(recordID);

            if (!String.IsNullOrEmpty(ds.DataSchemaX))
            {
                try
                {
                    Type type = ClassBuilder.LoadFromXmlString(ds.DataSchemaX).CreateRecordClass();

                    PreviewSchemaID.Text = recordID;
                    PreviewWindow.Show();
                }
                catch
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "Success",
                        Message = "The selected data Schema in invalid."
                    });
                }
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "Success",
                    Message = "No fields have been defined in this schema."
                });
            }
        }
    }

    protected void SubmitFieldsData(object sender, StoreSubmitDataEventArgs e)
    {

        DataSchema schema = new DataSchema(hfSchemaID.Text);

        if (schema.DataSourceTypeID == new Guid(DataSourceType.CSV))	//comma sep value
        {
            DelimitedClassBuilder cb = new DelimitedClassBuilder(schema.Name, schema.Delimiter);

            cb.IgnoreEmptyLines = true;

            cb.IgnoreFirstLines = schema.IgnoreFirst;
            cb.IgnoreLastLines = schema.IgnoreLast;

            if (!String.IsNullOrEmpty(schema.Condition))
                schema.Condition = schema.Condition;

            if (!String.IsNullOrEmpty(schema.SplitSelector))
            {
                cb.SplitSelector = schema.SplitSelector;
                cb.SplitIndex = schema.SplitIndex.Value;
            }

            //cb.GeneraProperties = true; 

            List<FieldDefinition> fields = e.Object<FieldDefinition>();

            foreach (var field in fields)
            {
                cb.AddField(field.Name.Trim(), typeof(string));

                if (field.Datefield)
                {
                    cb.LastField.ValueDate = true;
                    cb.LastField.ValueDateformat = field.Dateformat;
                }
                else if (field.Timefield)
                {
                    cb.LastField.ValueTime = true;
                    cb.LastField.ValueTimeformat = field.Timeformat;
                }
                else
                {
                    if (field.Ignorefield)
                        cb.LastField.FieldValueDiscarded = true;
                    else if(field.Commentfield)
                        cb.LastField.ValueComment = true;
                    else if (field.OfferingID.HasValue)
                    {
                        if (!String.IsNullOrEmpty(field.EmptyValue))
                        {
                            cb.LastField.ValueEmpty = true;
                            cb.LastField.EmptyValue = field.EmptyValue;
                        }

                        cb.LastField.PhenomenonOfferingID = field.OfferingID.Value;
                        cb.LastField.PhenomenonUOMID = field.UnitofMeasureID.Value;

                        if (!String.IsNullOrEmpty(field.FixedTime))
                            cb.LastField.FixedTime = field.FixedTime;
                    }
                }
            }


            string cs = cb.GetClassSourceCode(NetLanguage.CSharp);

            Type builder = ClassBuilder.ClassFromString(cs);

            var engine = new FileHelperEngine(cb.CreateRecordClass());

            string dataSchema = cb.SaveToXmlString();

            schema.DataSchemaX = dataSchema;

            schema.Save();
        }
        else if (schema.DataSourceTypeID == new Guid(DataSourceType.Flat))
        {
            FixedLengthClassBuilder cb = new FixedLengthClassBuilder(schema.Name, FixedMode.AllowVariableLength);

            cb.IgnoreEmptyLines = true;
            cb.IgnoreFirstLines = schema.IgnoreFirst;
            cb.IgnoreLastLines = schema.IgnoreLast;

            if (!String.IsNullOrEmpty(schema.Condition))
                schema.Condition = schema.Condition;

            //cb.GeneraProperties = true; 

            if (!String.IsNullOrEmpty(schema.SplitSelector))
            {
                cb.SplitSelector = schema.SplitSelector;
                cb.SplitIndex = schema.SplitIndex.Value;
            }

            List<FieldDefinition> fields = e.Object<FieldDefinition>();

            foreach (var field in fields)
            {
                cb.AddField(field.Name.Trim(), field.FieldLength.Value, typeof(string));

                if (field.Datefield)
                {
                    cb.LastField.ValueDate = true;
                    cb.LastField.ValueDateformat = field.Dateformat;
                }
                else if (field.Timefield)
                {
                    cb.LastField.ValueTime = true;
                    cb.LastField.ValueTimeformat = field.Timeformat;
                }
                else
                {
                    if (field.Ignorefield)
                        cb.LastField.FieldValueDiscarded = true;
                    else if (field.Commentfield)
                        cb.LastField.ValueComment = true;
                    else if (field.OfferingID.HasValue)
                    {
                        if (!String.IsNullOrEmpty(field.EmptyValue))
                        {
                            cb.LastField.ValueEmpty = true;
                            cb.LastField.EmptyValue = field.EmptyValue;
                        }

                        cb.LastField.PhenomenonOfferingID = field.OfferingID.Value;
                        cb.LastField.PhenomenonUOMID = field.UnitofMeasureID.Value;

                        if (!String.IsNullOrEmpty(field.FixedTime))
                            cb.LastField.FixedTime = field.FixedTime;
                    }
                }
            }


            string cs = cb.GetClassSourceCode(NetLanguage.CSharp);

            Type builder = ClassBuilder.ClassFromString(cs);

            var engine = new FileHelperEngine(cb.CreateRecordClass());

            string dataSchema = cb.SaveToXmlString();

            schema.DataSchemaX = dataSchema;

            schema.Save();
        }

        DefinitionWindow.Hide();
    }

    protected void cbOffering_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        var Id = this.cbPhenomenon.SelectedItem.Value;

        this.cbOffering.GetStore().DataSource = new Select(PhenomenonOffering.IdColumn, Offering.NameColumn)
                 .From(Offering.Schema)
                 .InnerJoin(PhenomenonOffering.OfferingIDColumn, Offering.IdColumn)
                 .Where(PhenomenonOffering.Columns.PhenomenonID).IsEqualTo(Id)
                 .ExecuteDataSet().Tables[0];
        this.cbOffering.GetStore().DataBind();

        //this.cbUnitofMeasure.GetStore().DataSource = new Select(PhenomenonUOM.IdColumn, UnitOfMeasure.UnitColumn)
        //        .From(UnitOfMeasure.Schema)
        //        .InnerJoin(PhenomenonUOM.UnitOfMeasureIDColumn, UnitOfMeasure.IdColumn)
        //        .Where(PhenomenonUOM.Columns.PhenomenonID).IsEqualTo(Id)
        //        .ExecuteDataSet().Tables[0];
        //this.cbUnitofMeasure.GetStore().DataBind();
    }

    protected void cbUnitofMeasure_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        var Id = this.cbPhenomenon.SelectedItem.Value;

        this.cbUnitofMeasure.GetStore().DataSource = new Select(PhenomenonUOM.IdColumn, UnitOfMeasure.UnitColumn)
               .From(UnitOfMeasure.Schema)
               .InnerJoin(PhenomenonUOM.UnitOfMeasureIDColumn, UnitOfMeasure.IdColumn)
               .Where(PhenomenonUOM.Columns.PhenomenonID).IsEqualTo(Id)
               .ExecuteDataSet().Tables[0];
        this.cbUnitofMeasure.GetStore().DataBind();
    }

    protected void UploadClick(object sender, DirectEventArgs e)
    {

        if (this.PreviewFileUpload.HasFile)
        {
            DataSchema ds = new DataSchema(PreviewSchemaID.Text);

            Type type = ClassBuilder.LoadFromXmlString(ds.DataSchemaX).CreateRecordClass();
            e.Success = true;

            FileHelperEngine eng = new FileHelperEngine(type);

            eng.ErrorMode = ErrorMode.SaveAndContinue;

            var errors = new List<object>();

            string Data = String.Empty;

            using (StreamReader sr = new StreamReader(PreviewFileUpload.PostedFile.InputStream))
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    if (!String.IsNullOrEmpty(ds.SplitSelector))
                    {

                        string line;

                        int index = 0;

                        while ((line = sr.ReadLine()) != null && index <= ds.SplitIndex)
                        {
                            if (line.StartsWith(ds.SplitSelector))
                                index++;

                            if (index == ds.SplitIndex)
                            {
                                sb.AppendLine(line);
                                break;
                            }
                        }

                        while ((line = sr.ReadLine()) != null && index <= ds.SplitIndex)
                        {
                            if (line.StartsWith(ds.SplitSelector))
                                index++;

                            if (index <= ds.SplitIndex)
                            {
                                sb.AppendLine(line);
                            }
                        }

                        Data = sb.ToString();

                    }
                    else
                        Data = sr.ReadToEnd();
                    

                    DataTable dt = eng.ReadStringAsDT(Data, 100);

                    Store store = new Store();
                    store.ID = "tempstore";
                    JsonReader read = new JsonReader();
                    ColumnModel cmodel = PreviewGrid.ColumnModel;
                    foreach (DataColumn col in dt.Columns)
                    {
                        read.Fields.Add(new RecordField(col.ColumnName, RecordFieldType.String));

                        cmodel.Columns.Add(new Column() { DataIndex = col.ColumnName, Header = col.ColumnName, Width = Unit.Pixel(100) });
                    }

                    store.Reader.Add(read);
                    store.IDMode = IDMode.Static;
                    store.DataSource = dt;
                    store.Render();

                    this.PreviewGrid.Reconfigure(store.ID, cmodel);


                    if (eng.ErrorManager.ErrorCount > 0)
                    {
                        foreach (ErrorInfo errinfo in eng.ErrorManager.Errors)
                        {
                            errors.Add(new { ErrorMessage = errinfo.ExceptionInfo.Message, LineNo = errinfo.LineNumber, RecordString = errinfo.RecordString });
                        }

                        ErrorGridStore.DataSource = errors;
                        ErrorGridStore.DataBind();

                    }

                    X.Msg.Hide();
                }
                catch (Exception ex)
                {

                    errors.Add(new { ErrorMessage = ex.Message, LineNo = 1, RecordString = "" });

                    ErrorGridStore.DataSource = errors;
                    ErrorGridStore.DataBind();
                    X.Msg.Hide();

                    //X.Msg.Show(new MessageBoxConfig
                    //{
                    //    Buttons = MessageBox.Button.OK,
                    //    Icon = MessageBox.Icon.ERROR,
                    //    Title = "Fail",
                    //    Message = "An Error occured while executing the schema"
                    //});
                }
                finally
                {

                }
            }
        }
        else
        {
            X.Msg.Show(new MessageBoxConfig
            {
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.ERROR,
                Title = "Fail",
                Message = "No file uploaded"
            });

            e.Success = false;
        }
    }

    protected void DataSchemaStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();
        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        string js = BaseRepository.BuildExportQ("VDataSchema", gridData, visCols, sortCol, sortDir);

        BaseRepository.doExport(type, js);
    }

}