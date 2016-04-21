using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAEON.ObservationsDB.Data;
using Ext.Net;
using SubSonic;
using System.Transactions;
using System.Xml;
using System.Xml.Xsl;
using Newtonsoft.Json;
using NCalc;

/// <summary>
/// Summary description for DataSource
/// </summary>
public partial class _DataSource : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!X.IsAjaxRequest)
        {
            DataSchemaStore.DataSource = new DataSchemaCollection().OrderByAsc(DataSchema.Columns.Name).Load();
            DataSchemaStore.DataBind();

            Dictionary<int, string> frequencylist = listHelper.GetUpdateFrequencyList();

            foreach (var item in frequencylist)
            {
                cbUpdateFrequency.Items.Insert(cbUpdateFrequency.Items.ToArray().Length, new Ext.Net.ListItem(item.Value, item.Key.ToString()));
            }

            TransformationTypeStore.DataSource = new TransformationTypeCollection().OrderByAsc(TransformationType.Columns.Name).Load();
            TransformationTypeStore.DataBind();

            PhenomenonStore.DataSource = new PhenomenonCollection().OrderByAsc(Phenomenon.Columns.Name).Load();
            PhenomenonStore.DataBind();

        }
    }

    protected void DataSourceStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        DataSourceGrid.GetStore().DataSource = DataSourceRepository.GetPagedList(e, e.Parameters[GridFilters1.ParamPrefix]);
    }

    protected void ValidateField(object sender, RemoteValidationEventArgs e)
    {

        DataSourceCollection col = new DataSourceCollection();

        string checkColumn = String.Empty,
               errorMessage = String.Empty;

        if (e.ID == "tfCode")
        {
            checkColumn = DataSource.Columns.Code;
            errorMessage = "The specified DataSource Code already exists";
        }
        else if (e.ID == "tfName")
        {
            checkColumn = DataSource.Columns.Name;
            errorMessage = "The specified DataSource Name already exists";

        }
        //else if (e.ID == "ContentPlaceHolder1_tfUrl")
        //{
        //    checkColumn = DataSource.Columns.Url;
        //    errorMessage = "Url is required when update frequency is not ad-hoc";
        //}
        //else if (e.ID == "ContentPlaceHolder1_cbUpdateFrequency")
        //{
        //    checkColumn = DataSource.Columns.Url;
        //    errorMessage = "Url and start date is required when update frequency is not ad-hoc";
        //}
        //else if (e.ID == "ContentPlaceHolder1_StartDate")
        //{
        //    checkColumn = DataSource.Columns.StartDate;
        //    errorMessage = "Start date is required when update frequency is not ad-hoc";
        //}


        if (e.ID == "tfCode" || e.ID == "tfName")
        {
            if (String.IsNullOrEmpty(tfID.Text.ToString()))
                col = new DataSourceCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
            else
                col = new DataSourceCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(DataSource.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();

            if (col.Count > 0)
            {
                e.Success = false;
                e.ErrorMessage = errorMessage;
            }
            else
                e.Success = true;
        }
        //else if (e.ID == "ContentPlaceHolder1_tfUrl")
        //{
        //    if (cbUpdateFrequency.SelectedItem.Text == "Ad-Hoc")
        //    {
        //        e.Success = true; 
        //    }
        //    else
        //    {
        //        if (tfUrl.Text.Length != 0)
        //        {
        //            e.Success = true; 
        //        }
        //        else
        //        {
        //            e.Success = false;
        //            e.ErrorMessage = errorMessage; 
        //        }
        //    }
        //}
        //else if (e.ID == "ContentPlaceHolder1_StartDate")
        //{
        //    if (cbUpdateFrequency.SelectedItem.Text == "Ad-Hoc")
        //    {
        //        e.Success = true;
        //    }
        //    else
        //    {
        //        if (StartDate.Text != "0001/01/01 00:00:00")
        //        {
        //            e.Success = true;
        //        }
        //        else
        //        {
        //            e.Success = false;
        //            e.ErrorMessage = errorMessage;
        //        }
        //    }
        //}
        //else if (e.ID == "ContentPlaceHolder1_cbUpdateFrequency")
        //{
        //    if (cbUpdateFrequency.SelectedItem.Text == "Ad-Hoc")
        //    {
        //        e.Success = true;
        //    }
        //    else
        //    {
        //        if (tfUrl.Text.Length != 0 && StartDate.Text != "0001/01/01 00:00:00")
        //        {
        //            e.Success = true;
        //        }
        //        else
        //        {
        //            e.Success = false;
        //            e.ErrorMessage = errorMessage;
        //        }
        //    }
        //}


    }

    protected void Save(object sender, DirectEventArgs e)
    {

        DataSource ds = new DataSource();

        if (String.IsNullOrEmpty(tfID.Text))
            ds.Id = Guid.NewGuid();
        else
            ds = new DataSource(tfID.Text.Trim());

        ds.Code = tfCode.Text.Trim();
        ds.Name = tfName.Text.Trim();
        ds.Description = tfDescription.Text.Trim();
        ds.Url = tfUrl.Text;
        //ds.DataSourceTypeID = new Guid(cbDataSourceType.SelectedItem.Value);
        //ds.DefaultNullValue = Int64.Parse(nfDefaultValue.Value.ToString());

        ds.UpdateFreq = int.Parse(cbUpdateFrequency.SelectedItem.Value);

        if (cbDataSchema.SelectedItem.Value != null)
        {
            //test if dataschema is valid (linked to test in datasource files)
            bool isValid = true;
            string sensorName = "";

            SensorProcedureCollection spCol = new SensorProcedureCollection()
                .Where(SensorProcedure.Columns.DataSourceID, ds.Id)
                .Load();

            foreach (SensorProcedure sp in spCol)
            {
                if (sp.DataSchemaID != null)
                {
                    isValid = false;
                    sensorName = sp.Name;
                }
            }
                        
            //
            if (isValid)
            {
                ds.DataSchemaID = Guid.Parse(cbDataSchema.SelectedItem.Value);
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Invalid Data Source",
                    //Message = "The selected data schema is already linked to a sensor that is linked to this data source.",
                    Message = "This data source cant have a data schema because one of the sensors linked to it (" + sensorName + ") is already linked to a data schema",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR
                });

                return;
            }
        }
        else
            ds.DataSchemaID = null;


        if (!String.IsNullOrEmpty(StartDate.Text))
            ds.StartDate = StartDate.SelectedDate;
        if (StartDate.SelectedDate.Date.Year < 1900)
        {
            ds.StartDate = null;
        }

        ds.UserId = AuthHelper.GetLoggedInUserId;

        ds.Save();

        DataSourceGrid.DataBind();

        DetailWindow.Hide();
    }

    protected void DSTransformGrid_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["DataSourceID"] != null && e.Parameters["DataSourceID"].ToString() != "-1")
        {

            Guid Id = Guid.Parse(e.Parameters["DataSourceID"].ToString());

            VDataSourceTransformationCollection trCol = new VDataSourceTransformationCollection().Where(VDataSourceTransformation.Columns.DataSourceID, Id).Load();

            DSTransformGrid.GetStore().DataSource = trCol;
            DSTransformGrid.GetStore().DataBind();
        }
    }

    protected void SaveTransformation(object sender, DirectEventArgs e)
    {

        RowSelectionModel datasourceRow = DataSourceGrid.SelectionModel.Primary as RowSelectionModel;

        DataSourceTransformation dstransform = new DataSourceTransformation();

        //Check for outdated Transforms
        SqlQuery q = new Select().From<DataSourceTransformation>()
                    .Where(DataSourceTransformation.TransformationTypeIDColumn).IsEqualTo(cbTransformType.SelectedItem.Value)
                    .And(DataSourceTransformation.PhenomenonIDColumn).IsEqualTo(cbPhenomenon.SelectedItem.Value)
                    .And(DataSourceTransformation.DataSourceIDColumn).IsEqualTo(datasourceRow.SelectedRecordID)
                    .AndExpression(DataSourceTransformation.EndDateColumn.QualifiedName).IsNull().Or(DataSourceTransformation.EndDateColumn).IsGreaterThanOrEqualTo(dfTransStart.SelectedDate).CloseExpression();

        if (String.IsNullOrEmpty(tfTransID.Text))
            dstransform.Id = Guid.NewGuid();
        else
        {
            dstransform = new DataSourceTransformation(tfTransID.Text.Trim());
            q = q.And(DataSourceTransformation.IdColumn).IsNotEqualTo(dstransform.Id);
        }

        DataSourceTransformationCollection OutdatedItems = q.ExecuteAsCollection<DataSourceTransformationCollection>();

        using (TransactionScope ts = new TransactionScope())
        {
            using (SharedDbConnectionScope connScope = new SharedDbConnectionScope())
            {

                foreach (var item in OutdatedItems)
                {
                    item.EndDate = dfTransStart.SelectedDate.Date;
                    item.Save();
                }

                dstransform.DataSourceID = new Guid(datasourceRow.SelectedRecordID);
                dstransform.TransformationTypeID = new Guid(cbTransformType.SelectedItem.Value);
                dstransform.PhenomenonID = new Guid(cbPhenomenon.SelectedItem.Value);

                if (cbOffering.SelectedItem.Value != null)
                    dstransform.PhenomenonOfferingID = new Guid(cbOffering.SelectedItem.Value);
                else
                    dstransform.PhenomenonOfferingID = null;

                if (cbUnitofMeasure.SelectedItem.Value != null)
                    dstransform.PhenomenonUOMID = new Guid(cbUnitofMeasure.SelectedItem.Value);
                else
                    dstransform.PhenomenonUOMID = null;

                //9ca36c10-cbad-4862-9f28-591acab31237 = Quality Control on Values
                if (new Guid(cbTransformType.SelectedItem.Value) != new Guid("9ca36c10-cbad-4862-9f28-591acab31237"))
                {

                    if (sbNewOffering.SelectedItem.Value != null)
                        dstransform.NewPhenomenonOfferingID = new Guid(sbNewOffering.SelectedItem.Value);
                    else
                        dstransform.NewPhenomenonOfferingID = null;

                    if (sbNewUoM.SelectedItem.Value != null)
                        dstransform.NewPhenomenonUOMID = new Guid(sbNewUoM.SelectedItem.Value);
                    else
                        dstransform.NewPhenomenonUOMID = null; 
                }
                else
                {
                    dstransform.NewPhenomenonOfferingID = null;
                    dstransform.NewPhenomenonUOMID = null; 
                }
                //

                dstransform.StartDate = dfTransStart.SelectedDate;
                dstransform.Definition = tfDefinition.Text.Trim().ToLower();


                dstransform.Save();
            }

            ts.Complete();
        }

        DSTransformGrid.GetStore().DataBind();

        TransformationDetailWindow.Hide();
    }

    protected void SaveRoleDetail(object sender, DirectEventArgs e)
    {
        DataSourceRole dsRole = new DataSourceRole(hiddenRoleDetail.Text);

        dsRole.DateStart = DateTime.Parse(dfRoleDetailStart.Text);
        dsRole.DateEnd = DateTime.Parse(dfRoleDetailEnd.Text);
        dsRole.IsRoleReadOnly = bool.Parse(cbIsRoleReadOnly.Value.ToString());

        dsRole.Save();

        DataSourceRoleGrid.GetStore().DataBind();

        RoleDetailWindow.Hide();
    }

    protected void OnDefinitionValidation(object sender, RemoteValidationEventArgs e)
    {


        try
        {
            string json = String.Concat("{", e.Value.ToString(), "}");

            if (cbTransformType.SelectedItem.Value == null)
            {
                e.ErrorMessage = "Select a transformation type.";
                e.Success = false;
            }
            else
            {
                switch (cbTransformType.SelectedItem.Value)
                {
                    case TransformationType.RatingTable:
                        JSON.Deserialize<Dictionary<double, double>>(json);
                        e.Success = true;
                        break;
                    case TransformationType.QualityControlValues:
                        JSON.Deserialize<Dictionary<string, double>>(json);
                        e.Success = true;
                        break;
                    case TransformationType.CorrectionValues:
                        Dictionary<string, string> corrvals = JSON.Deserialize<Dictionary<string, string>>(json);

                        if (corrvals.Count > 1)
                        {
                            e.Success = false;
                            e.ErrorMessage = "Only one expression is currently supported";
                        }
                        else if (!corrvals.ContainsKey("eq"))
                        {
                            e.Success = false;
                            e.ErrorMessage = "The Key value of this value should be \" eq \"";
                        }
                        else
                        {
                            Expression exp = new Expression(corrvals["eq"]);

                            if (!corrvals["eq"].Contains("[value]"))
                            {
                                e.Success = false;
                                e.ErrorMessage = "The expression accepts only one one variable - [value]";
                            }
                            else if (exp.HasErrors())
                            {
                                e.Success = false;
                                e.ErrorMessage = String.Concat("Error in expression - ", exp.Error);
                            }
                            else
                                e.Success = true;

                        }

                        break;
                }
            }
        }
        catch
        {
            e.ErrorMessage = "Invalid Definition.";
            e.Success = false;
        }
    }

    protected void DataSourceStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();
        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        string js = BaseRepository.BuildExportQ("VDataSource", gridData, visCols, sortCol, sortDir);

        BaseRepository.doExport(type, js);
    }

    protected void DataSourceRoleGrid_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {

        if (e.Parameters["DataSourceID"] != null && e.Parameters["DataSourceID"].ToString() != "-1")
        {

            Guid Id = Guid.Parse(e.Parameters["DataSourceID"].ToString());

            DataSourceRoleCollection RoleCol = new Select()
                     .From(DataSourceRole.Schema)
                     .Where(DataSourceRole.Columns.DataSourceID).IsEqualTo(Id)
                     .ExecuteAsCollection<DataSourceRoleCollection>();

            DataSourceRoleGrid.GetStore().DataSource = RoleCol;
            DataSourceRoleGrid.GetStore().DataBind();
        }
    }

    protected void RoleGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["DataSourceID"] != null && e.Parameters["DataSourceID"].ToString() != "-1")
        {

            Guid Id = Guid.Parse(e.Parameters["DataSourceID"].ToString());

            AspnetRoleCollection RoleCol = new Select()
                    .From(AspnetRole.Schema)
                    .Where(AspnetRole.RoleIdColumn).NotIn(new Select(new String[] { DataSourceRole.Columns.RoleId })
                                                                    .From(DataSourceRole.Schema)
                                                                    .Where(DataSourceRole.DataSourceIDColumn).IsEqualTo(Id))
                    .ExecuteAsCollection<AspnetRoleCollection>();

            List<RoleAndDates> RoleDateList = new List<RoleAndDates>();
            foreach (AspnetRole role in RoleCol)
            {
                RoleDateList.Add(new RoleAndDates()
                {
                    RoleId = role.RoleId,
                    RoleName = role.RoleName,
                    Description = role.Description,
                    DateStart = new DateTime(1900, 01, 01),
                    DateEnd = new DateTime(1900, 01, 01)
                });
            }

            RoleGridStore.DataSource = RoleDateList.ToList();
            RoleGridStore.DataBind();

        }
    }

    protected class RoleAndDates
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
    }

    protected void AcceptRoleButton_Click(object sender, DirectEventArgs e)
    {

        RowSelectionModel sm = RoleGrid.SelectionModel.Primary as RowSelectionModel;
        RowSelectionModel DataSourcerow = DataSourceGrid.SelectionModel.Primary as RowSelectionModel;

        string DataSourceID = DataSourcerow.SelectedRecordID;

        //
        string data = e.ExtraParams["RoleValues"];
        JsonObject[] jObj = JSON.Deserialize<JsonObject[]>(data);
        int nrRoles = 0;
        foreach (JsonObject jsonObject in jObj)
        {
            nrRoles++;
            Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonObject.ToString());

            string[] colms = new string[values.Count()];
            int i = 0;
            foreach (var item in values)
            {
                colms[i] = item.Value;
                i++;
            }

            DataSourceRole role = new DataSourceRole();
            role.RoleId = new Guid(colms[0]);
            role.DataSourceID = new Guid(DataSourceID);
            role.DateStart = DateTime.Parse(colms[3]);
            role.DateEnd = DateTime.Parse(colms[4]);
            role.RoleName = colms[1];
            role.IsRoleReadOnly = bool.Parse(colms[5]);

            role.Save();
        }
        //

        if (nrRoles == 0)
        {
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Invalid Selection",
                Message = "Select at least one Role",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO
            });
        }
        else
        {
            DataSourceRoleGrid.GetStore().DataBind();
            RoleGrid.GetStore().DataBind();
            AvailableRoleWindow.Hide();
        }

    }

    [DirectMethod]
    public void ConfirmDeleteRole(Guid RoleId)
    {
        X.Msg.Confirm("Confirm Delete", "Are you sure you want to delete this role?", new MessageBoxButtonsConfig
        {

            Yes = new MessageBoxButtonConfig
            {
                Handler = String.Concat("DirectCall.DeleteRole(\"", RoleId.ToString(), "\",{ eventMask: { showMask: true}});"),
                Text = "Yes"
            },
            No = new MessageBoxButtonConfig
            {
                Text = "No"
            }
        }).Show();
    }

    [DirectMethod]
    public void DeleteRole(Guid RoleId)
    {
        using (TransactionScope ts = new TransactionScope())
        {
            using (SharedDbConnectionScope connScope = new SharedDbConnectionScope())
            {
                //DataLog.Delete(DataLog.Columns.ImportBatchID, RoleId);
                //Observation.Delete(Observation.Columns.ImportBatchID, RoleId);
                DataSourceRole.Delete(RoleId);
            }

            ts.Complete();
        }

        DataSourceRoleGrid.GetStore().DataBind();
        //DSLogGrid.GetStore().DataBind();
    }


    protected void cbOffering_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        var Id = cbPhenomenon.SelectedItem.Value;

        cbOffering.GetStore().DataSource = new Select(PhenomenonOffering.IdColumn, Offering.NameColumn)
                 .From(Offering.Schema)
                 .InnerJoin(PhenomenonOffering.OfferingIDColumn, Offering.IdColumn)
                 .Where(PhenomenonOffering.Columns.PhenomenonID).IsEqualTo(Id)
                 .ExecuteDataSet().Tables[0];
        cbOffering.GetStore().DataBind();
    }

    protected void cbUnitofMeasure_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        var Id = cbPhenomenon.SelectedItem.Value;

        cbUnitofMeasure.GetStore().DataSource = new Select(PhenomenonUOM.IdColumn, UnitOfMeasure.UnitColumn)
               .From(UnitOfMeasure.Schema)
               .InnerJoin(PhenomenonUOM.UnitOfMeasureIDColumn, UnitOfMeasure.IdColumn)
               .Where(PhenomenonUOM.Columns.PhenomenonID).IsEqualTo(Id)
               .ExecuteDataSet().Tables[0];
        cbUnitofMeasure.GetStore().DataBind();
    }

    //////////////
    protected void cbNewOffering_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        var Id = cbPhenomenon.SelectedItem.Value;

        sbNewOffering.GetStore().DataSource = new Select(PhenomenonOffering.IdColumn, Offering.NameColumn)
                 .From(Offering.Schema)
                 .InnerJoin(PhenomenonOffering.OfferingIDColumn, Offering.IdColumn)
                 .Where(PhenomenonOffering.Columns.PhenomenonID).IsEqualTo(Id)
                 .ExecuteDataSet().Tables[0];
        sbNewOffering.GetStore().DataBind();
    }

    protected void cbNewUnitofMeasure_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        var Id = cbPhenomenon.SelectedItem.Value;

        sbNewUoM.GetStore().DataSource = new Select(PhenomenonUOM.IdColumn, UnitOfMeasure.UnitColumn)
               .From(UnitOfMeasure.Schema)
               .InnerJoin(PhenomenonUOM.UnitOfMeasureIDColumn, UnitOfMeasure.IdColumn)
               .Where(PhenomenonUOM.Columns.PhenomenonID).IsEqualTo(Id)
               .ExecuteDataSet().Tables[0];
        sbNewUoM.GetStore().DataBind();
    }
}