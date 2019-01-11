using Ext.Net;
using NCalc;
using SAEON.Logs;
using SAEON.Observations.Data;
using SubSonic;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Admin_DataSources : System.Web.UI.Page
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

            InstrumentStore.DataSource = new InstrumentCollection().OrderByAsc(Instrument.Columns.Name).Load();
            InstrumentStore.DataBind();
        }
    }

    #region Data Sources
    protected void DataSourcesGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        DataSourcesGridStore.DataSource = DataSourceRepository.GetPagedList(e, e.Parameters[GridFilters1.ParamPrefix]);
    }

    protected void ValidateField(object sender, RemoteValidationEventArgs e)
    {
        DataSourceCollection col = new DataSourceCollection();
        string checkColumn = String.Empty;
        string errorMessage = String.Empty;
        e.Success = true;

        if (e.ID == "tfCode" || e.ID == "tfName")
        {
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
            if (tfID.IsEmpty)
            {
                col = new DataSourceCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
            }
            else
            {
                col = new DataSourceCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(DataSource.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();
            }

            if (col.Count > 0)
            {
                e.Success = false;
                e.ErrorMessage = errorMessage;
            }
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
        using (Logging.MethodCall(GetType()))
        {
            try
            {
                DataSource ds = new DataSource();

                if (String.IsNullOrEmpty(tfID.Text))
                {
                    ds.Id = Guid.NewGuid();
                }
                else
                {
                    ds = new DataSource(tfID.Text.Trim());
                }

                ds.Code = tfCode.Text.Trim();
                ds.Name = tfName.Text.Trim();
                ds.Description = tfDescription.Text.Trim();
                ds.Url = tfUrl.Text;
                //ds.DataSourceTypeID = new Guid(cbDataSourceType.SelectedItem.Value);
                //ds.DefaultNullValue = Int64.Parse(nfDefaultValue.Value.ToString());

                ds.UpdateFreq = int.Parse(cbUpdateFrequency.SelectedItem.Value);

                if (cbDataSchema.IsEmpty)
                {
                    ds.DataSchemaID = null;
                }
                else
                {
                    SensorCollection col = new Select().From(Sensor.Schema)
                        .Where(Sensor.Columns.DataSourceID).IsEqualTo(ds.Id)
                        .And(Sensor.Columns.DataSchemaID).IsNotNull()
                        .ExecuteAsCollection<SensorCollection>();

                    if (!col.Any())
                    {
                        ds.DataSchemaID = Guid.Parse(cbDataSchema.SelectedItem.Value);
                    }
                    else
                    {
                        //Logging.Verbose($"DirectCall.DeleteSensorSchemas(\"{ds.Id.ToString()}\",{{ eventMask: {{ showMask: true}}}});");
                        MessageBoxes.Confirm("Confirm",
                            $"DirectCall.DeleteSensorSchemas(\"{ds.Id.ToString()}\",{{ eventMask: {{ showMask: true}}}});",
                            $"This data source can't have a data schema because sensor{(col.Count > 1 ? "s" : "")} {string.Join(", ", col)} are already linked to a data schema. Clear the schema from these sensor{(col.Count > 1 ? "s" : "")}?");
                        return;
                    }
                }


                if (dfStartDate.SelectedDate.Date.Year < 1900)
                {
                    ds.StartDate = null;
                }
                else
                {
                    ds.StartDate = dfStartDate.SelectedDate;
                }

                if (dfEndDate.SelectedDate.Date.Year < 1900)
                {
                    ds.EndDate = null;
                }
                else
                {
                    ds.EndDate = dfEndDate.SelectedDate;
                }

                ds.UserId = AuthHelper.GetLoggedInUserId;

                ds.Save();
                Auditing.Log(GetType(), new ParameterList {
                { "ID", ds.Id }, { "Code", ds.Code }, { "Name", ds.Name } });
                DataSourcesGrid.DataBind();

                DetailWindow.Hide();
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
                throw;
            }
        }
    }


    [DirectMethod]
    public void DeleteSensorSchemas(Guid aID)
    {
        using (Logging.MethodCall(GetType(), new ParameterList { { "aID", aID } }))
        {
            try
            {
                SensorCollection col = new Select().From(Sensor.Schema)
                    .Where(Sensor.Columns.DataSourceID).IsEqualTo(aID)
                    .And(Sensor.Columns.DataSchemaID).IsNotNull()
                    .ExecuteAsCollection<SensorCollection>();
                foreach (var sensor in col)
                {
                    sensor.DataSchemaID = null;
                    sensor.Save();
                }
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
                throw;
            }
        }
    }

    protected void DataSourcesGridStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();
        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        //string js = BaseRepository.BuildExportQ("VDataSource", gridData, visCols, sortCol, sortDir);
        //BaseRepository.doExport(type, js);
        BaseRepository.Export("vDataSource", gridData, visCols, sortCol, sortDir, type, "Data Sources", Response);
    }

    #endregion

    #region Data Source Transformations
    protected void TransformationsGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        using (Logging.MethodCall(GetType()))
        {
            if (e.Parameters["DataSourceID"] != null && e.Parameters["DataSourceID"].ToString() != "-1")
            {

                try
                {
                    Guid Id = Guid.Parse(e.Parameters["DataSourceID"].ToString());

                    VDataSourceTransformationCollection trCol = new VDataSourceTransformationCollection()
                        .Where(VDataSourceTransformation.Columns.DataSourceID, Id)
                        .OrderByAsc(VDataSourceTransformation.Columns.Iorder)
                        .OrderByAsc(VDataSourceTransformation.Columns.Rank)
                        .Load();

                    TransformationsGrid.GetStore().DataSource = trCol;
                    TransformationsGrid.GetStore().DataBind();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }
    }

    protected void SaveTransformation(object sender, DirectEventArgs e)
    {
        using (Logging.MethodCall(GetType()))
        {
            try
            {

                RowSelectionModel masterRow = DataSourcesGrid.SelectionModel.Primary as RowSelectionModel;
                var masterID = new Guid(masterRow.SelectedRecordID);
                DataSourceTransformation dstransform = null;
                if (String.IsNullOrEmpty(tfTransID.Text))
                {
                    dstransform = new DataSourceTransformation();
                }
                else
                {
                    dstransform = new DataSourceTransformation(tfTransID.Text.Trim());
                }

                // --> Removed 20170126 TimPN
                ////Check for outdated Transforms
                //SqlQuery q = new Select().From<DataSourceTransformation>()
                //            .Where(DataSourceTransformation.TransformationTypeIDColumn).IsEqualTo(cbTransformType.SelectedItem.Value)
                //            .And(DataSourceTransformation.PhenomenonIDColumn).IsEqualTo(cbPhenomenon.SelectedItem.Value)
                //            .And(DataSourceTransformation.DataSourceIDColumn).IsEqualTo(datasourceRow.SelectedRecordID)
                //            .AndExpression(DataSourceTransformation.EndDateColumn.QualifiedName).IsNull().Or(DataSourceTransformation.EndDateColumn).IsGreaterThanOrEqualTo(dfTransStart.SelectedDate).CloseExpression();

                //if (String.IsNullOrEmpty(tfTransID.Text))
                //    dstransform.Id = Guid.NewGuid();
                //else
                //{
                //    dstransform = new DataSourceTransformation(tfTransID.Text.Trim());
                //    q = q.And(DataSourceTransformation.IdColumn).IsNotEqualTo(dstransform.Id);
                //}
                //DataSourceTransformationCollection OutdatedItems = q.ExecuteAsCollection<DataSourceTransformationCollection>();
                // --< Removed 20170126 TimPN

                // --> Removed 20170126 TimPN
                //foreach (var item in OutdatedItems)
                //{
                //    item.EndDate = dfTransStart.SelectedDate.Date;
                //    item.Save();
                //}
                // --< Removed 20170126 TimPN

                dstransform.DataSourceID = masterID;
                dstransform.TransformationTypeID = new Guid(cbTransformType.SelectedItem.Value);
                dstransform.PhenomenonID = new Guid(cbPhenomenon.SelectedItem.Value);
                dstransform.PhenomenonOfferingID = cbOffering.IsEmpty ? null : new Guid?(new Guid(cbOffering.SelectedItem.Value));
                dstransform.PhenomenonUOMID = cbUnitofMeasure.IsEmpty ? null : new Guid?(new Guid(cbUnitofMeasure.SelectedItem.Value));

                var transformType = new TransformationType(new Guid(cbTransformType.SelectedItem.Value));
                if (transformType.Code == TransformationType.QualityControlValues)
                {
                    dstransform.NewPhenomenonID = null;
                    dstransform.NewPhenomenonOfferingID = null;
                    dstransform.NewPhenomenonUOMID = null;
                }
                else
                {
                    dstransform.NewPhenomenonID = sbNewPhenomenon.IsEmpty ? null : new Guid?(new Guid(sbNewPhenomenon.SelectedItem.Value));
                    dstransform.NewPhenomenonOfferingID = sbNewOffering.IsEmpty ? null : new Guid?(new Guid(sbNewOffering.SelectedItem.Value));
                    dstransform.NewPhenomenonUOMID = sbNewUoM.IsEmpty ? null : new Guid?(new Guid(sbNewUoM.SelectedItem.Value));
                }
                //

                if (dfTransStart.IsEmpty || (dfTransStart.SelectedDate.Year < 1900))
                {
                    dstransform.StartDate = null;
                }
                else
                {
                    dstransform.StartDate = DateTime.Parse(dfTransStart.RawText);
                }

                if (dfTransEnd.IsEmpty || (dfTransEnd.SelectedDate.Year < 1900))
                {
                    dstransform.EndDate = null;
                }
                else
                {
                    dstransform.EndDate = DateTime.Parse(dfTransEnd.RawText);
                }

                dstransform.Definition = tfDefinition.Text.Trim().ToLower();
                dstransform.Rank = (int)tfRank.Number;
                dstransform.ParamA = tfParamA.IsEmpty ? null : (double?)tfParamA.Number;
                dstransform.ParamB = tfParamB.IsEmpty ? null : (double?)tfParamB.Number;
                dstransform.ParamC = tfParamC.IsEmpty ? null : (double?)tfParamC.Number;
                dstransform.ParamD = tfParamD.IsEmpty ? null : (double?)tfParamD.Number;
                dstransform.ParamE = tfParamE.IsEmpty ? null : (double?)tfParamE.Number;
                dstransform.ParamF = tfParamF.IsEmpty ? null : (double?)tfParamF.Number;
                dstransform.ParamG = tfParamG.IsEmpty ? null : (double?)tfParamG.Number;
                dstransform.ParamH = tfParamH.IsEmpty ? null : (double?)tfParamH.Number;
                dstransform.ParamI = tfParamI.IsEmpty ? null : (double?)tfParamI.Number;
                dstransform.ParamJ = tfParamJ.IsEmpty ? null : (double?)tfParamJ.Number;
                dstransform.ParamK = tfParamK.IsEmpty ? null : (double?)tfParamK.Number;
                dstransform.ParamL = tfParamL.IsEmpty ? null : (double?)tfParamL.Number;
                dstransform.ParamM = tfParamM.IsEmpty ? null : (double?)tfParamM.Number;
                dstransform.ParamN = tfParamN.IsEmpty ? null : (double?)tfParamN.Number;
                dstransform.ParamO = tfParamO.IsEmpty ? null : (double?)tfParamO.Number;
                dstransform.ParamP = tfParamP.IsEmpty ? null : (double?)tfParamP.Number;
                dstransform.ParamQ = tfParamQ.IsEmpty ? null : (double?)tfParamQ.Number;
                dstransform.ParamR = tfParamR.IsEmpty ? null : (double?)tfParamR.Number;
                dstransform.ParamSX = tfParamS.IsEmpty ? null : (double?)tfParamS.Number;
                dstransform.ParamT = tfParamT.IsEmpty ? null : (double?)tfParamT.Number;
                dstransform.ParamU = tfParamU.IsEmpty ? null : (double?)tfParamU.Number;
                dstransform.ParamV = tfParamV.IsEmpty ? null : (double?)tfParamV.Number;
                dstransform.ParamW = tfParamW.IsEmpty ? null : (double?)tfParamW.Number;
                dstransform.ParamX = tfParamX.IsEmpty ? null : (double?)tfParamX.Number;
                dstransform.ParamY = tfParamY.IsEmpty ? null : (double?)tfParamY.Number;
                dstransform.Save();
                Auditing.Log(GetType(), new ParameterList {
                { "DataSourceID", dstransform.DataSourceID},
                { "DataSourceCode", dstransform.DataSource.Code},
                { "TransformationTypeID", dstransform.TransformationTypeID},
                { "TransformationTypeName", dstransform.TransformationType.Name },
                { "PhenomenonID", dstransform.PhenomenonID},
                {"PhenomenonOfferingID", dstransform.PhenomenonOfferingID },
                {"PhenomenonUnitOfMeasureID", dstransform.PhenomenonUOMID },
                {"NewPhenomenonID" , dstransform.NewPhenomenonID},
                {"NewPhenomenonOfferingID", dstransform.NewPhenomenonOfferingID },
                {"NewPhenomenonUnitOfMeasureID", dstransform.NewPhenomenonUOMID },
                {"StartDate", dstransform?.StartDate },
                {"EndDate", dstransform?.EndDate },
                {"Rank", dstransform.Rank },
                {"ParamA", dstransform.ParamA},
                {"ParamB", dstransform.ParamB},
                {"ParamC", dstransform.ParamC},
                {"ParamD", dstransform.ParamD},
                {"ParamE", dstransform.ParamE},
                {"ParamF", dstransform.ParamF},
                {"ParamG", dstransform.ParamG},
                {"ParamH", dstransform.ParamH},
                {"ParamI", dstransform.ParamI},
                {"ParamJ", dstransform.ParamJ},
                {"ParamK", dstransform.ParamK},
                {"ParamL", dstransform.ParamL},
                {"ParamM", dstransform.ParamM},
                {"ParamN", dstransform.ParamN},
                {"ParamO", dstransform.ParamO},
                {"ParamP", dstransform.ParamP},
                {"ParamQ", dstransform.ParamQ},
                {"ParamR", dstransform.ParamR},
                {"ParamS", dstransform.ParamSX},
                {"ParamT", dstransform.ParamT},
                {"ParamU", dstransform.ParamU},
                {"ParamV", dstransform.ParamV},
                {"ParamW", dstransform.ParamW},
                {"ParamX", dstransform.ParamX},
                {"ParamY", dstransform.ParamY},
                });

                TransformationsGrid.GetStore().DataBind();

                TransformationDetailWindow.Hide();
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
                MessageBoxes.Error(ex, "Error", "Unable to save transformation");
                throw;
            }
        }
    }

    [DirectMethod]
    public void ConfirmDeleteTransformation(Guid aID)
    {
        MessageBoxes.Confirm(
            "Confirm Delete",
            String.Format("DirectCall.DeleteTransformation(\"{0}\",{{ eventMask: {{ showMask: true}}}});", aID.ToString()),
            "Are you sure you want to delete this transformation?");
    }

    [DirectMethod]
    public void DeleteTransformation(Guid aID)
    {
        using (Logging.MethodCall(GetType(), new ParameterList { { "aID", aID } }))
        {
            try
            {
                DataSourceTransformation.Delete(aID);
                Auditing.Log(GetType(), new ParameterList { { "ID", aID } });
                TransformationsGrid.GetStore().DataBind();
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
                MessageBoxes.Error(ex, "Error", "Unable to delete transformation");
            }
        }
    }

    [DirectMethod]
#pragma warning disable IDE1006 // Naming Styles
    public void cbPhenomenonSelect(object sender, DirectEventArgs e)
#pragma warning restore IDE1006 // Naming Styles
    {
        cbOffering.GetStore().DataSource = new Select(PhenomenonOffering.IdColumn, Offering.NameColumn)
                 .From(Offering.Schema)
                 .InnerJoin(PhenomenonOffering.OfferingIDColumn, Offering.IdColumn)
                 .Where(PhenomenonOffering.Columns.PhenomenonID).IsEqualTo(cbPhenomenon.Value)
                 .ExecuteDataSet().Tables[0];
        cbOffering.GetStore().DataBind();
        cbUnitofMeasure.GetStore().DataSource = new Select(PhenomenonUOM.IdColumn, UnitOfMeasure.UnitColumn)
               .From(UnitOfMeasure.Schema)
               .InnerJoin(PhenomenonUOM.UnitOfMeasureIDColumn, UnitOfMeasure.IdColumn)
               .Where(PhenomenonUOM.Columns.PhenomenonID).IsEqualTo(cbPhenomenon.Value)
               .ExecuteDataSet().Tables[0];
        cbUnitofMeasure.GetStore().DataBind();
        cbOffering.Clear();
        cbOffering.ClearInvalid();
        cbUnitofMeasure.Clear();
        cbUnitofMeasure.ClearInvalid();
    }

    protected void OnDefinitionValidation(object sender, RemoteValidationEventArgs e)
    {
        using (Logging.MethodCall(GetType()))
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
                    var transformationType = new TransformationType(cbTransformType.SelectedItem.Value);
                    switch (transformationType.Code)
                    {
                        case TransformationType.RatingTable:
                            JSON.Deserialize<Dictionary<double, double>>(json);
                            e.Success = true;
                            break;
                        case TransformationType.QualityControlValues:
                            JSON.Deserialize<Dictionary<string, double>>(json);
                            e.Success = true;
                            break;
                        case TransformationType.Lookup:
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
                                {
                                    e.Success = true;
                                }
                            }
                            break;
                        case TransformationType.Expression:
                            e.Success = false;
                            Expression expr = new Expression(e.Value.ToString(), EvaluateOptions.IgnoreCase);
                            expr.Parameters["value"] = 1.0;
                            if (!tfParamA.IsEmpty)
                            {
                                expr.Parameters["a"] = tfParamA.Number;
                            }
                            if (!tfParamB.IsEmpty)
                            {
                                expr.Parameters["b"] = tfParamB.Number;
                            }
                            if (!tfParamC.IsEmpty)
                            {
                                expr.Parameters["c"] = tfParamC.Number;
                            }
                            if (!tfParamD.IsEmpty)
                            {
                                expr.Parameters["d"] = tfParamD.Number;
                            }
                            if (!tfParamE.IsEmpty)
                            {
                                expr.Parameters["e"] = tfParamE.Number;
                            }
                            if (!tfParamF.IsEmpty)
                            {
                                expr.Parameters["f"] = tfParamF.Number;
                            }
                            if (!tfParamG.IsEmpty)
                            {
                                expr.Parameters["g"] = tfParamG.Number;
                            }
                            if (!tfParamH.IsEmpty)
                            {
                                expr.Parameters["h"] = tfParamH.Number;
                            }
                            if (!tfParamI.IsEmpty)
                            {
                                expr.Parameters["i"] = tfParamI.Number;
                            }
                            if (!tfParamJ.IsEmpty)
                            {
                                expr.Parameters["j"] = tfParamJ.Number;
                            }
                            if (!tfParamK.IsEmpty)
                            {
                                expr.Parameters["k"] = tfParamK.Number;
                            }
                            if (!tfParamL.IsEmpty)
                            {
                                expr.Parameters["l"] = tfParamL.Number;
                            }
                            if (!tfParamM.IsEmpty)
                            {
                                expr.Parameters["m"] = tfParamM.Number;
                            }
                            if (!tfParamN.IsEmpty)
                            {
                                expr.Parameters["n"] = tfParamN.Number;
                            }
                            if (!tfParamO.IsEmpty)
                            {
                                expr.Parameters["o"] = tfParamO.Number;
                            }
                            if (!tfParamP.IsEmpty)
                            {
                                expr.Parameters["p"] = tfParamP.Number;
                            }
                            if (!tfParamQ.IsEmpty)
                            {
                                expr.Parameters["q"] = tfParamQ.Number;
                            }
                            if (!tfParamR.IsEmpty)
                            {
                                expr.Parameters["r"] = tfParamR.Number;
                            }
                            if (!tfParamS.IsEmpty)
                            {
                                expr.Parameters["s"] = tfParamS.Number;
                            }
                            if (!tfParamT.IsEmpty)
                            {
                                expr.Parameters["t"] = tfParamT.Number;
                            }
                            if (!tfParamU.IsEmpty)
                            {
                                expr.Parameters["u"] = tfParamU.Number;
                            }
                            if (!tfParamV.IsEmpty)
                            {
                                expr.Parameters["v"] = tfParamV.Number;
                            }
                            if (!tfParamW.IsEmpty)
                            {
                                expr.Parameters["w"] = tfParamW.Number;
                            }
                            if (!tfParamX.IsEmpty)
                            {
                                expr.Parameters["x"] = tfParamX.Number;
                            }
                            if (!tfParamY.IsEmpty)
                            {
                                expr.Parameters["y"] = tfParamY.Number;
                            }
                            if (expr.HasErrors())
                            {
                                e.ErrorMessage = $"Error in expression - {expr.Error}";
                                Logging.Error("Error in expression {Error}", expr.Error);
                                break;
                            }
                            try
                            {
                                var valueStr = expr.Evaluate();
                                Logging.Verbose("ValueStr: {value}", valueStr);
                                var value = double.Parse(valueStr.ToString());
                                Logging.Verbose("Value: {value}", value);
                                e.Success = true;
                            }
                            catch (EvaluationException ex)
                            {
                                e.ErrorMessage = $"Error evaluating expression - {ex.Message}";
                                Logging.Exception(ex, "Error evaluating expression {exception}", ex.Message);
                            }
                            catch (Exception ex)
                            {
                                e.ErrorMessage = $"Exception evaluating expression - {ex.Message}";
                                Logging.Exception(ex, "Exception evaluating expression {exception}", ex.Message);

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
    }

    [DirectMethod]
    public void LoadCombos(string transformationTypeID, string phenomenonID, string offeringID, string unitOfMeasureID, string newPhenomenonID, string newOfferingID, string newUnitOfMeasureID)
    {
        using (Logging.MethodCall(GetType(), new ParameterList { { "TransformationTypeID", transformationTypeID },
            { "PhenomenonID", phenomenonID }, { "OfferingID", offeringID }, {"UnitOfMeasureID", unitOfMeasureID },
            { "NewPhenomenonID", newPhenomenonID }, { "NewOfferingID", newOfferingID }, { "NewUnitOfMeasureID", newUnitOfMeasureID } }))
        {
            try
            {
                cbTransformType.Value = transformationTypeID;
                cbTransformTypeSelect(cbTransformType, new DirectEventArgs(null));
                cbPhenomenon.Value = phenomenonID;
                cbPhenomenonSelect(cbPhenomenon, new DirectEventArgs(null));
                cbOffering.Value = string.IsNullOrWhiteSpace(offeringID) ? null : offeringID;
                cbUnitofMeasure.Value = string.IsNullOrWhiteSpace(unitOfMeasureID) ? null : unitOfMeasureID;
                sbNewPhenomenon.Value = string.IsNullOrWhiteSpace(newPhenomenonID) ? null : newPhenomenonID;
                sbNewPhenomenonSelect(sbNewPhenomenon, new DirectEventArgs(null));
                sbNewOffering.Value = string.IsNullOrWhiteSpace(newOfferingID) ? null : newOfferingID;
                sbNewUoM.Value = string.IsNullOrWhiteSpace(newUnitOfMeasureID) ? null : newUnitOfMeasureID;
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
                throw;
            }
        }

    }

    [DirectMethod]
    public void SetFields()
    {
        void SetField(NumberField field)
        {
            field.ClearInvalid();
            field.MarkAsValid();
        }

        using (Logging.MethodCall(GetType()))
        {
            SetField(tfParamA);
            SetField(tfParamB);
            SetField(tfParamC);
            SetField(tfParamD);
            SetField(tfParamE);
            SetField(tfParamF);
            SetField(tfParamG);
            SetField(tfParamH);
            SetField(tfParamI);
            SetField(tfParamJ);
            SetField(tfParamK);
            SetField(tfParamL);
            SetField(tfParamM);
            SetField(tfParamN);
            SetField(tfParamO);
            SetField(tfParamP);
            SetField(tfParamQ);
            SetField(tfParamR);
            SetField(tfParamS);
            SetField(tfParamT);
            SetField(tfParamU);
            SetField(tfParamV);
            SetField(tfParamW);
            SetField(tfParamX);
            SetField(tfParamY);
        }
    }

    protected void OnParamValidation(object sender, RemoteValidationEventArgs e)
    {
        tfDefinition.ClearInvalid();
        try
        {
            OnDefinitionValidation(sender, e);
            if (e.Success)
            {
                tfDefinition.MarkAsValid();
            }
            else
            {
                tfDefinition.MarkInvalid();
            }
        }
        catch (Exception)
        {
        }
        e.Success = true;
    }

    [DirectMethod]
#pragma warning disable IDE1006 // Naming Styles
    public void cbTransformTypeSelect(object sender, DirectEventArgs e)
#pragma warning restore IDE1006 // Naming Styles
    {
        using (Logging.MethodCall(GetType()))
        {
            try
            {
                TransformationType transformType = new TransformationType(cbTransformType.Value);
                ContainerParameters.Hidden = (transformType.Code != TransformationType.Expression);
                if (transformType.Code == TransformationType.QualityControlValues)
                {
                    cbOffering.ReadOnly = true;
                    cbOffering.Clear();
                    cbOffering.ClearInvalid();
                    cbUnitofMeasure.ReadOnly = true;
                    cbUnitofMeasure.Clear();
                    cbUnitofMeasure.ClearInvalid();
                    sbNewPhenomenon.ReadOnly = true;
                    sbNewPhenomenon.Clear();
                    sbNewPhenomenon.ClearInvalid();
                    sbNewOffering.ReadOnly = true;
                    sbNewOffering.Clear();
                    sbNewOffering.ClearInvalid();
                    sbNewUoM.ReadOnly = true;
                    sbNewUoM.Clear();
                    sbNewUoM.ClearInvalid();
                }
                else
                {
                    cbOffering.ReadOnly = false;
                    cbUnitofMeasure.ReadOnly = false;
                    sbNewPhenomenon.ReadOnly = false;
                    sbNewOffering.ReadOnly = false;
                    sbNewUoM.ReadOnly = false;
                }
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
                throw;
            }
        }
    }

    private void LoadNewComboBoxes()
    {
        var Id = sbNewPhenomenon.SelectedItem.Value;
        sbNewOffering.GetStore().DataSource = new Select(PhenomenonOffering.IdColumn, Offering.NameColumn)
                 .From(Offering.Schema)
                 .InnerJoin(PhenomenonOffering.OfferingIDColumn, Offering.IdColumn)
                 .Where(PhenomenonOffering.Columns.PhenomenonID).IsEqualTo(Id)
                 .ExecuteDataSet().Tables[0];
        sbNewOffering.GetStore().DataBind();
        sbNewUoM.GetStore().DataSource = new Select(PhenomenonUOM.IdColumn, UnitOfMeasure.UnitColumn)
               .From(UnitOfMeasure.Schema)
               .InnerJoin(PhenomenonUOM.UnitOfMeasureIDColumn, UnitOfMeasure.IdColumn)
               .Where(PhenomenonUOM.Columns.PhenomenonID).IsEqualTo(Id)
               .ExecuteDataSet().Tables[0];
        sbNewUoM.GetStore().DataBind();
    }

    [DirectMethod]
#pragma warning disable IDE1006 // Naming Styles
    public void sbNewPhenomenonSelect(object sender, DirectEventArgs e)
#pragma warning restore IDE1006 // Naming Styles
    {
        LoadNewComboBoxes();
        sbNewOffering.Clear();
        sbNewOffering.ClearInvalid();
        sbNewUoM.Clear();
        sbNewUoM.ClearInvalid();
    }

    [DirectMethod]
#pragma warning disable IDE1006 // Naming Styles
    public void sbNewPhenomenonClear(object sender, DirectEventArgs e)
#pragma warning restore IDE1006 // Naming Styles
    {
        sbNewPhenomenon.Clear();
        sbNewPhenomenon.ClearInvalid();
        sbNewOffering.Clear();
        sbNewOffering.ClearInvalid();
        sbNewUoM.Clear();
        sbNewUoM.ClearInvalid();
        LoadNewComboBoxes();
    }

    [DirectMethod]
#pragma warning disable IDE1006 // Naming Styles
    public void sbNewOfferingClear(object sender, DirectEventArgs e)
#pragma warning restore IDE1006 // Naming Styles
    {
        sbNewOffering.Clear();
        sbNewOffering.ClearInvalid();
    }

    [DirectMethod]
#pragma warning disable IDE1006 // Naming Styles
    public void sbNewUoMClear(object sender, DirectEventArgs e)
#pragma warning restore IDE1006 // Naming Styles
    {
        sbNewUoM.Clear();
        sbNewUoM.ClearInvalid();
    }

    #endregion

}