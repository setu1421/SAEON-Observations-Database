using Ext.Net;
using SAEON.Observations.Data;
using Serilog;
using Serilog.Context;
using SubSonic;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin_ImportBatches : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!X.IsAjaxRequest)
        {
            Store store = cbDataSource.GetStore();
            SqlQuery q = new Select(DataSource.IdColumn).From(DataSource.Schema).Distinct()
                            .InnerJoin(Sensor.DataSourceIDColumn, DataSource.IdColumn);
            SqlQuery q1 = new Select(DataSourceRole.DataSourceIDColumn).From(DataSourceRole.Schema).Distinct()
                .Where(DataSourceRole.RoleNameColumn).In(System.Web.Security.Roles.GetRolesForUser())
                .And(DataSourceRole.IsRoleReadOnlyColumn).IsEqualTo(false)
                .And(DataSourceRole.DataSourceIDColumn).In(q);
            SqlQuery sourceQuery = new Select(DataSource.IdColumn, DataSource.NameColumn)
                        .From(DataSource.Schema).Where(DataSource.IdColumn).In(q1).OrderAsc(DataSource.NameColumn.QualifiedName);
            DataSet ds = sourceQuery.ExecuteDataSet();
            store.DataSource = ds.Tables[0];

            store.DataBind();

            cbSensor.GetStore().DataSource = new Select(Sensor.IdColumn, Sensor.NameColumn)
                    .From(Sensor.Schema).ExecuteDataSet().Tables[0];

            cbSensor.GetStore().DataBind();

            cbOffering.GetStore().DataSource = new Select(PhenomenonOffering.IdColumn, Offering.NameColumn)
               .From(Offering.Schema)
               .InnerJoin(PhenomenonOffering.OfferingIDColumn, Offering.IdColumn).ExecuteDataSet().Tables[0];

            cbOffering.GetStore().DataBind();

            this.cbUnitofMeasure.GetStore().DataSource = new Select(PhenomenonUOM.IdColumn, UnitOfMeasure.UnitColumn)
              .From(UnitOfMeasure.Schema)
              .InnerJoin(PhenomenonUOM.UnitOfMeasureIDColumn, UnitOfMeasure.IdColumn)
              .ExecuteDataSet().Tables[0];
            this.cbUnitofMeasure.GetStore().DataBind();

            StatusStore.DataSource = new StatusCollection().OrderByAsc(Status.Columns.Name).Load();
            StatusStore.DataBind();
            StatusReasonStore.DataSource = new StatusReasonCollection().OrderByAsc(StatusReason.Columns.Name).Load();
            StatusReasonStore.DataBind();
        }
    }

    #region Import Batches
    protected void ImportBatchesGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        this.ImportBatchesGridStore.DataSource = ImportBatchRepository.GetPagedList(e, e.Parameters[this.GridFilters1.ParamPrefix]);
    }

    protected void DSLogGrid_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["ImportBatchID"] != null && e.Parameters["ImportBatchID"].ToString() != "-1")
        {
            Guid BatchId = Utilities.MakeGuid(e.Parameters["ImportBatchID"].ToString());

            this.DSLogGrid.GetStore().DataSource = DataLogRepository.GetPagedListByBatch(e, e.Parameters[this.GridFilters1.ParamPrefix], BatchId);
            this.DSLogGrid.GetStore().DataBind();
        }
    }


    protected void UploadClick(object sender, DirectEventArgs e)
    {
        using (LogContext.PushProperty("Method", "UploadClick"))
            try
            {
                Guid DataSourceId = new Guid(cbDataSource.SelectedItem.Value);

                //
                //add check to see that either datasource or linked sensor has a dataschema
                bool isThereADataSchema = false;
                DataSource tempDS = new DataSource(DataSourceId);
                if (tempDS.DataSchemaID != null)
                {
                    isThereADataSchema = true;
                }
                else
                {
                    SensorCollection tempSPCol = new SensorCollection()
                        .Where(Sensor.Columns.DataSourceID, DataSourceId)
                        .Where(Sensor.Columns.DataSchemaID, SubSonic.Comparison.IsNot, null)
                        .Load();

                    if (tempSPCol.Count != 0)
                    {
                        isThereADataSchema = true;
                    }
                }

                if (!isThereADataSchema)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "Error",
                        Message = "There is no data schema linked to the data source or any of its sensors."
                    });

                    return;
                }
                //

                // Save file to FileStream container


                List<SchemaValue> values = Import(DataSourceId);



                if (values.Any())
                {
                    try
                    {
                        using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 15, 0)))
                        {
                            using (SharedDbConnectionScope connScope = new SharedDbConnectionScope())
                            {
                                ImportBatch batch = new ImportBatch();
                                //batch.Guid = Guid.NewGuid();
                                batch.Id = Guid.NewGuid();
                                batch.DataSourceID = DataSourceId;
                                batch.ImportDate = DateTime.Now;

                                FileInfo fi;
                                if (!string.IsNullOrEmpty(LogFileUpload.PostedFile.FileName))
                                {
                                    fi = new FileInfo(LogFileUpload.PostedFile.FileName);
                                    batch.LogFileName = fi.Name;
                                }

                                fi = new FileInfo(DataFileUpload.PostedFile.FileName);
                                batch.FileName = fi.Name;


                                if (values.FirstOrDefault(t => t.IsValid) == null)
                                    batch.Status = (int)ImportBatchStatus.DatalogWithErrors;
                                else
                                    batch.Status = (int)ImportBatchStatus.NoLogErrors;

                                batch.UserId = AuthHelper.GetLoggedInUserId;
                                batch.Save();

                                for (int i = 0; i < values.Count; i++)
                                {
                                    SchemaValue schval = values[i];

                                    SqlQuery q = new Select(Aggregate.Count(Observation.IdColumn)).From(Observation.Schema)
                                        .Where(Observation.Columns.SensorID).IsEqualTo(schval.SensorID)
                                        .And(Observation.Columns.ValueDate).IsEqualTo(schval.DateValue)
                                        .And(Observation.Columns.RawValue).IsEqualTo(schval.RawValue)
                                        .And(Observation.Columns.PhenomenonOfferingID).IsEqualTo(schval.PhenomenonOfferingID)
                                        .And(Observation.Columns.PhenomenonUOMID).IsEqualTo(schval.PhenomenonUOMID);
                                    //add offering

                                    int totalDuplicate = q.ExecuteScalar<int>();


                                    if (schval.IsValid && totalDuplicate == 0)
                                    {

                                        if (!isDuplicateOfNull(schval, batch.Id))
                                        {
                                            Observation Obrecord = new Observation();
                                            Obrecord.SensorID = schval.SensorID.Value;
                                            Obrecord.ValueDate = schval.DateValue;
                                            Obrecord.RawValue = schval.RawValue;
                                            Obrecord.DataValue = schval.DataValue;
                                            Obrecord.PhenomenonOfferingID = schval.PhenomenonOfferingID.Value;
                                            Obrecord.PhenomenonUOMID = schval.PhenomenonUOMID.Value;
                                            Obrecord.ImportBatchID = batch.Id;
                                            Obrecord.UserId = AuthHelper.GetLoggedInUserId;

                                            Obrecord.AddedDate = DateTime.Now;

                                            if (schval.Comment.Length > 0)
                                                Obrecord.Comment = schval.Comment;

                                            Obrecord.CorrelationID = schval.CorrelationID;
                                            Obrecord.Save();
                                        }

                                    }
                                    else
                                    {
                                        //
                                        batch.Status = (int)ImportBatchStatus.DatalogWithErrors;
                                        batch.Save();
                                        //

                                        DataLog logrecord = new DataLog();

                                        logrecord.SensorID = schval.SensorID;

                                        if (schval.DateValueInvalid)
                                            logrecord.InvalidDateValue = schval.InvalidDateValue;
                                        else if (schval.DateValue != DateTime.MinValue)
                                            logrecord.ValueDate = schval.DateValue;

                                        if (schval.TimeValueInvalid)
                                            logrecord.InvalidTimeValue = schval.InvalidTimeValue;

                                        if (schval.TimeValue.HasValue && schval.TimeValue != DateTime.MinValue)
                                            logrecord.ValueTime = schval.TimeValue;

                                        if (schval.RawValueInvalid)
                                            logrecord.ValueText = schval.InvalidRawValue;
                                        else
                                            logrecord.RawValue = schval.RawValue;

                                        if (schval.DataValueInvalid)
                                            logrecord.TransformValueText = schval.InvalidDataValue;
                                        else
                                            logrecord.DataValue = schval.DataValue;

                                        if (schval.InValidOffering)
                                            logrecord.InvalidOffering = schval.PhenomenonOfferingID.Value.ToString();
                                        else
                                            logrecord.PhenomenonOfferingID = schval.PhenomenonOfferingID.Value;

                                        if (schval.InValidUOM)
                                            logrecord.InvalidUOM = schval.PhenomenonUOMID.Value.ToString();
                                        else
                                            logrecord.PhenomenonUOMID = schval.PhenomenonUOMID.Value;

                                        logrecord.RawFieldValue = String.IsNullOrEmpty(schval.FieldRawValue) ? "" : schval.FieldRawValue;
                                        logrecord.ImportDate = DateTime.Now;
                                        logrecord.ImportBatchID = batch.Id;

                                        logrecord.DataSourceTransformationID = schval.DataSourceTransformationID;
                                        if (totalDuplicate > 0)
                                        {
                                            logrecord.ImportStatus = "This record is valid, but it is already in the Observations table. " + String.Join(",", schval.InvalidStatuses.ToArray());
                                            logrecord.StatusID = new Guid("0b03dcbf-ddd1-4fe1-95d9-3c80bef2d643");  //duplicate - Record is already in live system, not uploaded
                                        }
                                        else
                                        {
                                            logrecord.ImportStatus = String.Join(",", schval.InvalidStatuses.ToArray());
                                            logrecord.StatusID = new Guid(schval.InvalidStatuses[0]);
                                        }

                                        logrecord.UserId = AuthHelper.GetLoggedInUserId;

                                        if (schval.Comment.Length > 0)
                                            logrecord.Comment = schval.Comment;

                                        logrecord.CorrelationID = schval.CorrelationID;
                                        logrecord.Save();
                                    }
                                }
                            }
                            ts.Complete();
                        }

                        this.ImportBatchesGrid.GetStore().DataBind();
                        this.DSLogGrid.GetStore().DataBind();
                        ObservationsGridStore.DataBind();

                        ImportWindow.Hide();

                        X.Msg.Hide();
                    }
                    catch (Exception Ex)
                    {
                        Log.Error(Ex, "An error occured while importing values");
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Title = "Warning",
                            Message = Ex.Message + "|"
                        });
                    }//"An error occured while importing values."
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.WARNING,
                        Title = "Warning",
                        Message = "No values have been imported."
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unable to save import batch");
                List<object> errors = new List<object>();
                errors.Add(new { ErrorMessage = ex.Message, LineNo = 1, RecordString = "" });

                ErrorGridStore.DataSource = errors;
                ErrorGridStore.DataBind();
                X.Msg.Hide();
            }
    }

    protected bool isDuplicateOfNull(SchemaValue schval, Guid batchid)
    {
        SqlQuery q = new Select().From(Observation.Schema)
            .Where(Observation.Columns.SensorID).IsEqualTo(schval.SensorID)
            .And(Observation.Columns.ValueDate).IsEqualTo(schval.DateValue)
            .And(Observation.Columns.RawValue).IsNull()
            .And(Observation.Columns.PhenomenonOfferingID).IsEqualTo(schval.PhenomenonOfferingID)
            .And(Observation.Columns.PhenomenonUOMID).IsEqualTo(schval.PhenomenonUOMID);

        ObservationCollection oCol = q.ExecuteAsCollection<ObservationCollection>();

        if (oCol.Count != 0)
        {
            //delete this observation and move it do the datalog with the new value in and a status saying its a duplicate of a null that now has a value. 
            //check if this works with transformations 
            DataLog d = new DataLog();
            d.SensorID = oCol[0].SensorID;
            d.ImportDate = oCol[0].AddedDate;
            d.ValueDate = oCol[0].ValueDate;
            d.RawValue = schval.DataValue;
            if (d.DataValue != null)
                d.RawFieldValue = schval.DataValue.Value.ToString();

            d.DataValue = schval.DataValue;
            d.Comment = oCol[0].Comment;
            d.PhenomenonOfferingID = oCol[0].PhenomenonOfferingID;
            d.PhenomenonUOMID = oCol[0].PhenomenonUOMID;
            d.ImportBatchID = batchid;
            d.UserId = oCol[0].UserId;

            Status s = new Status("edb0a37c-f68d-4693-8ba6-d14d1b4fabe8");
            d.StatusID = s.Id;
            d.ImportStatus = s.Name;
            //edb0a37c-f68d-4693-8ba6-d14d1b4fabe8	QA-99	Duplicate of a previous empty value	Duplicate of a previous empty value

            d.Save();

            new Delete().From(Observation.Schema).Where(Observation.Columns.Id).IsEqualTo(oCol[0].Id).Execute();
            return true;


        }
        return false;
    }

    protected void cbOffering_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        var Id = this.cbSensor.SelectedItem.Value;

        this.cbOffering.GetStore().DataSource = new Select(PhenomenonOffering.IdColumn, Offering.DescriptionColumn)
                 .From(Offering.Schema)
                 .InnerJoin(PhenomenonOffering.OfferingIDColumn, Offering.IdColumn)
                 .InnerJoin(Sensor.PhenomenonIDColumn, PhenomenonOffering.PhenomenonIDColumn)
                 .Where(Sensor.IdColumn.QualifiedName).IsEqualTo(Id)
                 .ExecuteDataSet().Tables[0];
        this.cbOffering.GetStore().DataBind();
    }

    protected void cbUnitofMeasure_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        var Id = this.cbSensor.SelectedItem.Value;

        this.cbUnitofMeasure.GetStore().DataSource = new Select(PhenomenonUOM.IdColumn, UnitOfMeasure.UnitColumn)
               .From(UnitOfMeasure.Schema)
               .InnerJoin(PhenomenonUOM.UnitOfMeasureIDColumn, UnitOfMeasure.IdColumn)
               .InnerJoin(Sensor.PhenomenonIDColumn, PhenomenonUOM.PhenomenonIDColumn)
               .Where(Sensor.IdColumn.QualifiedName).IsEqualTo(Id)
               .ExecuteDataSet().Tables[0];
        this.cbUnitofMeasure.GetStore().DataBind();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    List<SchemaValue> Import(Guid DataSourceId)
    {
        DataSource ds = new DataSource(DataSourceId);
        List<SchemaValue> ImportValues = new List<SchemaValue>();

        List<DateTime> recordgaps = new List<DateTime>();
        List<DateTime> datagaps = new List<DateTime>();

        string Data = String.Empty;
        //if (!ds.DataSchemaID.HasValue)
        //{

        SensorCollection col = new SensorCollection().Where(Sensor.Columns.DataSourceID, DataSourceId)
                                                                       .Where(Sensor.Columns.DataSchemaID, SubSonic.Comparison.IsNot, null).Load();

        ImportLogHelper logHelper = new ImportLogHelper();

        if (LogFileUpload.PostedFile.ContentLength > 0)
        {
            using (StreamReader reader = new StreamReader(LogFileUpload.PostedFile.InputStream))
            {
                logHelper.ReadLog(reader.ReadToEnd());
            }
        }

        using (StreamReader reader = new StreamReader(DataFileUpload.PostedFile.InputStream))
        {

            if (col.Count > 0)
            {
                foreach (var sp in col)
                {
                    DataFileUpload.PostedFile.InputStream.Seek(0, SeekOrigin.Begin);

                    DataSchema schema = sp.DataSchema;

                    Data = ImportSchemaHelper.GetWorkingStream(schema, reader);

                    //                   using (ImportSchemaHelper helper = new ImportSchemaHelper(ds, schema, Data, sp, logHelper))
                    ImportSchemaHelper helper = new ImportSchemaHelper(ds, schema, Data, sp, logHelper);
                    {
                        if (helper.Errors.Count > 0)
                        {
                            ErrorGrid.GetStore().DataSource = helper.Errors;
                            ErrorGrid.GetStore().DataBind();
                            break;
                        }
                        else
                        {
                            helper.ProcessSchema();

                            ImportValues.AddRange(helper.SchemaValues);
                        }
                    }
                }
            }
            else
            {
                DataSchema schema = ds.DataSchema;

                Data = ImportSchemaHelper.GetWorkingStream(schema, reader);

                //using (ImportSchemaHelper helper = new ImportSchemaHelper(ds, schema, Data, null, logHelper))
                ImportSchemaHelper helper = new ImportSchemaHelper(ds, schema, Data, null, logHelper);
                {
                    if (helper.Errors.Count > 0)
                    {
                        ErrorGrid.GetStore().DataSource = helper.Errors;
                        ErrorGrid.GetStore().DataBind();
                    }
                    else
                    {
                        helper.ProcessSchema();

                        ImportValues.AddRange(helper.SchemaValues);
                    }
                }
            }
            //}
        }

        return ImportValues;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void SaveObservation(object sender, DirectEventArgs e)
    {
        using (LogContext.PushProperty("Method", "SaveObservation"))
            try
            {
                RowSelectionModel batchRow = this.ImportBatchesGrid.SelectionModel.Primary as RowSelectionModel;
                ImportBatch batch = new ImportBatch(batchRow.SelectedRecordID);

                //DataLog log = new DataLog();

                Observation obs = new Observation();

                obs.SensorID = new Guid(cbSensor.SelectedItem.Value);

                DateTime datevalue = (DateTime)ValueDate.Value;

                if (TimeValue.Value.ToString() != "-10675199.02:48:05.4775808")
                    datevalue = datevalue.AddMilliseconds(((TimeSpan)TimeValue.Value).TotalMilliseconds);

                obs.ValueDate = datevalue;
                obs.RawValue = Double.Parse(RawValue.Value.ToString());
                obs.DataValue = Double.Parse(DataValue.Value.ToString());

                obs.PhenomenonOfferingID = new Guid(cbOffering.SelectedItem.Value);
                obs.PhenomenonUOMID = new Guid(cbUnitofMeasure.SelectedItem.Value);

                obs.ImportBatchID = batch.Id;
                obs.UserId = AuthHelper.GetLoggedInUserId;

                obs.Comment = String.IsNullOrEmpty(tfComment.Text) ? null : tfComment.Text;

                SqlQuery q = new Select(Aggregate.Count("ID")).From(DataLog.Schema).Where(DataLog.ImportBatchIDColumn).IsEqualTo(batchRow.SelectedRecordID);

                //DataLogCollection batchcol = new DataLogCollection().Where(DataLog.Columns.ImportBatchID, batchRow.SelectedRecordID).Load();

                bool islast = q.ExecuteScalar<int>() == 1;

                //try
                //{

                q = new Select("ID").From(Observation.Schema)
                    .Where(Observation.Columns.SensorID).IsEqualTo(obs.SensorID)
                    .And(Observation.Columns.ValueDate).IsEqualTo(obs.ValueDate)
                    .And(Observation.Columns.RawValue).IsEqualTo(obs.RawValue);

                if (q.GetRecordCount() == 0)
                {
                    using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
                    {
                        using (SharedDbConnectionScope connScope = new SharedDbConnectionScope())
                        {
                            obs.AddedDate = DateTime.Now;
                            obs.Save();

                            if (islast)
                            {
                                batch.Status = (int)ImportBatchStatus.NoLogErrors;
                                batch.Save();
                            }


                            DataLog.Delete(tfID.Text);
                        }

                        ts.Complete();
                    }

                    DetailWindow.Hide();

                    this.ImportBatchesGrid.GetStore().DataBind();
                    this.DSLogGrid.GetStore().DataBind();
                    ObservationsGridStore.DataBind();
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "Warning",
                        Message = "New values will cause a duplicate entry to be made, data not saved."
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unable to save observation");
                throw;
            }

        //}
        //catch (Exception Ex)
        //{
        //    X.Msg.Show(new MessageBoxConfig
        //    {
        //        Buttons = MessageBox.Button.OK,
        //        Icon = MessageBox.Icon.ERROR,
        //        Title = "Warning",
        //        Message = "An error occured while processing the batch."
        //    });
        //}

        //if (log.DataValueInvalid == 1)
        //    obs.       
    }


    [DirectMethod]
    public void ConfirmDeleteBatch(Guid ImportBatchId)
    {
        X.Msg.Confirm("Confirm Delete", "Are you sure you want to Delete this Batch?", new MessageBoxButtonsConfig
        {

            Yes = new MessageBoxButtonConfig
            {
                Handler = String.Concat("DirectCall.DeleteBatch('", ImportBatchId, "',{ eventMask: { showMask: true}});"),
                Text = "Yes",
            },
            No = new MessageBoxButtonConfig
            {
                Text = "No"
            }
        }).Show();
    }

    [DirectMethod]
    public void DeleteBatch(Guid ImportBatchId)
    {
        using (LogContext.PushProperty("Method", "DeleteBatch"))
            try
            {
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 15, 0)))
                {
                    using (SharedDbConnectionScope connScope = new SharedDbConnectionScope())
                    {
                        DataLog.Delete(DataLog.Columns.ImportBatchID, ImportBatchId);
                        Observation.Delete(Observation.Columns.ImportBatchID, ImportBatchId);
                        ImportBatch.Delete(ImportBatchId);
                    }

                    ts.Complete();
                }

                ImportBatchesGrid.GetStore().DataBind();
                DSLogGrid.GetStore().DataBind();
                ObservationsGridStore.DataBind();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unable to delete batch");
                throw;
            }
    }


    [DirectMethod]
    public void ConfirmMoveBatch(Guid ImportBatchId)
    {
        X.Msg.Confirm("Confirm Move", "Are you sure you want to Move this Batch to the DataLog?", new MessageBoxButtonsConfig
        {

            Yes = new MessageBoxButtonConfig
            {
                Handler = String.Concat("DirectCall.MoveBatch('", ImportBatchId, "',{ eventMask: { showMask: true}});"),
                Text = "Yes",
            },
            No = new MessageBoxButtonConfig
            {
                Text = "No"
            }
        }).Show();
    }

    [DirectMethod]
    public void MoveBatch(Guid ImportBatchId)
    {
        using (LogContext.PushProperty("Method", "MoveBatch"))
            try
            {
                ObservationCollection col = new ObservationCollection().Where(Observation.Columns.ImportBatchID, ImportBatchId).Load();
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
                {
                    using (SharedDbConnectionScope connScope = new SharedDbConnectionScope())
                    {
                        for (int i = 0; i < col.Count; i++)
                        {
                            Observation ob = col[i];
                            DataLog log = new DataLog();
                            log.ValueDate = ob.ValueDate;
                            log.ValueTime = ob.ValueDate;
                            log.SensorID = ob.SensorID;
                            log.PhenomenonOfferingID = ob.PhenomenonOfferingID;
                            log.PhenomenonUOMID = ob.PhenomenonUOMID;
                            log.ImportBatchID = ob.ImportBatchID;
                            log.RawValue = ob.RawValue;
                            log.DataValue = ob.DataValue;
                            log.UserId = AuthHelper.GetLoggedInUserId;
                            log.StatusID = new Guid(Status.BatchRetracted);
                            log.CorrelationID = ob.CorrelationID;
                            log.Save();
                            Observation.Delete(ob.Id);
                        }
                    }

                    ts.Complete();
                }

                ImportBatchesGrid.GetStore().DataBind();
                DSLogGrid.GetStore().DataBind();
                ObservationsGridStore.DataBind();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unable to move batch");
                throw;
            }
    }

    [DirectMethod]
    public void ConfirmDeleteEntry(Guid Id)
    {
        X.Msg.Confirm("Confirm Delete", "Are you sure you want to Delete this entry?", new MessageBoxButtonsConfig
        {

            Yes = new MessageBoxButtonConfig
            {
                Handler = String.Concat("DirectCall.DeleteEntry('", Id, "',{ eventMask: { showMask: true}});"),
                Text = "Yes",
            },
            No = new MessageBoxButtonConfig
            {
                Text = "No"
            }
        }).Show();
    }

    [DirectMethod]
    public void DeleteEntry(Guid Id)
    {
        using (LogContext.PushProperty("Method", "DeleteEntry"))
            try
            {
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
                {
                    using (SharedDbConnectionScope connScope = new SharedDbConnectionScope())
                    {
                        Guid BatchId = new DataLog("ID", Id).ImportBatchID;
                        DataLog.Delete(DataLog.Columns.Id, Id);

                        SqlQuery q = new Select("ID").From(DataLog.Schema)
                            .Where(DataLog.Columns.ImportBatchID).IsEqualTo(BatchId);

                        if (q.GetRecordCount() == 0)
                        {
                            ImportBatch iB = new ImportBatch("ID", BatchId);
                            iB.Status = (int)ImportBatchStatus.NoLogErrors;
                            iB.Save();
                        }

                    }

                    ts.Complete();
                }

                ImportBatchesGrid.GetStore().DataBind();
                DSLogGrid.GetStore().DataBind();
                ObservationsGridStore.DataBind();

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unable to delete entry");
                throw;
            }
    }

    protected void ImportBatchesGridStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();
        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        string js = BaseRepository.BuildExportQ("VImportBatch", gridData, visCols, sortCol, sortDir);

        BaseRepository.doExport(type, js);
    }

    [DirectMethod]
    public void ConfirmMoveToObservation(Guid Id)
    {
        X.Msg.Confirm("Confirm Move", "Are you sure you want to move this entry to the observations?", new MessageBoxButtonsConfig
        {

            Yes = new MessageBoxButtonConfig
            {
                Handler = String.Concat("DirectCall.MoveToObservation('", Id, "',{ eventMask: { showMask: true}});"),
                Text = "Yes",
            },
            No = new MessageBoxButtonConfig
            {
                Text = "No"
            }
        }).Show();
    }

    [DirectMethod]
    public void MoveToObservation(Guid Id)
    {
        using (LogContext.PushProperty("Method", "MoveToObservation"))
            try
            {
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
                {
                    using (SharedDbConnectionScope connScope = new SharedDbConnectionScope())
                    {
                        DataLog d = new DataLog(Id);

                        Observation Obrecord = new Observation();
                        Obrecord.SensorID = d.SensorID.Value;
                        Obrecord.ValueDate = d.ValueDate.Value;
                        Obrecord.RawValue = d.RawValue;
                        Obrecord.DataValue = d.DataValue;
                        Obrecord.PhenomenonOfferingID = d.PhenomenonOfferingID.Value;
                        Obrecord.PhenomenonUOMID = d.PhenomenonUOMID.Value;
                        Obrecord.ImportBatchID = d.ImportBatchID;
                        Obrecord.UserId = AuthHelper.GetLoggedInUserId;
                        Obrecord.AddedDate = d.ImportDate;
                        Obrecord.Comment = d.Comment;

                        Obrecord.Save();

                        new Delete().From(DataLog.Schema).Where(DataLog.Columns.Id).IsEqualTo(d.Id).Execute();

                    }

                    ts.Complete();
                }

                ImportBatchesGrid.GetStore().DataBind();
                DSLogGrid.GetStore().DataBind();
                ObservationsGridStore.DataBind();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unable to move to observation");
                throw;
            }
    }
    #endregion

    #region Observations
    protected void ObservationsGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["ImportBatchID"] != null && e.Parameters["ImportBatchID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["ImportBatchID"].ToString());
            btnSetSelected.Disabled = true;
            btnSetWithout.Disabled = true;
            btnSetAll.Disabled = true;
            btnClearSelected.Disabled = true;
            btnClearAll.Disabled = true;
            try
            {
                ObservationsGridStore.DataSource = ObservationRepository.GetPagedListByBatch(e, e.Parameters[GridFilters1.ParamPrefix], Id);
                ObservationsGridStore.DataBind();
                EnableButtons();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ImportBatches.ObservationsGridStore_RefreshData");
                MessageBoxes.Error(ex, "Error", "Unable to refresh Observations grid");
            }
        }
    }

    protected void SetSelectedClick(object sender, DirectEventArgs e)
    {
        var sm = ObservationsGrid.SelectionModel.Primary as CheckboxSelectionModel;
        if (cbStatus.SelectedItem.Text == "Verified")
        {
            MessageBoxes.Confirm("Confirm",
            "DirectCall.SetSelected({eventMask: { showMask: true}});",
            $"Set status '{cbStatus.SelectedItem.Text}' to the {sm.SelectedRows.Count:N0} selected observations?");

        }
        else
        {
            MessageBoxes.Confirm("Confirm",
            "DirectCall.SetSelected({eventMask: { showMask: true}});",
            $"Set status '{cbStatus.SelectedItem.Text}' and reason '{cbStatusReason.SelectedItem.Text}' to the {sm.SelectedRows.Count:N0} selected observations?");

        }
    }

    [DirectMethod]
    public void SetSelected()
    {
        using (LogContext.PushProperty("Method", "SetSelected"))
        {
            try
            {
                var sm = ObservationsGrid.SelectionModel.Primary as CheckboxSelectionModel;
                foreach (SelectedRow row in sm.SelectedRows)
                {
                    Observation obs = new Observation(row.RecordID);
                    obs.StatusID = Utilities.MakeGuid(cbStatus.SelectedItem.Value);
                    if (cbStatus.SelectedItem.Text == "Verified")
                        obs.StatusReasonID = null;
                    else
                        obs.StatusReasonID = Utilities.MakeGuid(cbStatusReason.SelectedItem.Value);
                    obs.Save();
                }
                ObservationsGrid.DataBind();
                EnableButtons();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unable to set status and reason to the selected observations");
                MessageBoxes.Error(ex, "Error", "Unable to set status and reason to the selected observations");
            }
        }
    }

    protected void SetWithoutClick(object sender, DirectEventArgs e)
    {
        if (cbStatus.SelectedItem.Text == "Verified")
        {
            MessageBoxes.Confirm("Confirm",
            "DirectCall.SetWithout({eventMask: { showMask: true}});",
            $"Set status '{cbStatus.SelectedItem.Text}' to the observations without a status?");

        }
        else
        {
            MessageBoxes.Confirm("Confirm",
            "DirectCall.SetWithout({eventMask: { showMask: true}});",
            $"Set status '{cbStatus.SelectedItem.Text}' and reason '{cbStatusReason.SelectedItem.Text}' to the observations without status?");

        }
    }

    [DirectMethod]
    public void SetWithout()
    {
        using (LogContext.PushProperty("Method", "SetWithout"))
        {
            try
            {
                RowSelectionModel batchRow = this.ImportBatchesGrid.SelectionModel.Primary as RowSelectionModel;
                ImportBatch batch = new ImportBatch(batchRow.SelectedRecordID);
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
                {
                    using (SharedDbConnectionScope connScope = new SharedDbConnectionScope())
                    {
                        if (cbStatus.SelectedItem.Text == "Verified")
                        {
                            new Update(Observation.Schema)
                                .Set(Observation.Columns.StatusID).EqualTo(Utilities.MakeGuid(cbStatus.SelectedItem.Value))
                                .Set(Observation.Columns.StatusReasonID).EqualTo(null)
                                .Where(Observation.Columns.ImportBatchID).IsEqualTo(batch.Id)
                                .And(Observation.Columns.StatusID).IsNull()
                                .Execute();
                        }
                        else
                        {
                            new Update(Observation.Schema)
                                .Set(Observation.Columns.StatusID).EqualTo(Utilities.MakeGuid(cbStatus.SelectedItem.Value))
                                .Set(Observation.Columns.StatusReasonID).EqualTo(Utilities.MakeGuid(cbStatusReason.SelectedItem.Value))
                                .Where(Observation.Columns.ImportBatchID).IsEqualTo(batch.Id)
                                .And(Observation.Columns.StatusID).IsNull()
                                .Execute();
                        }
                    }
                    ts.Complete();
                }
                ObservationsGrid.DataBind();
                EnableButtons();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unable to set status and reason to the observations without status");
                MessageBoxes.Error(ex, "Error", "Unable to set status and reason to the observations without status");
            }
        }
    }

    protected void SetAllClick(object sender, DirectEventArgs e)
    {
        if (cbStatus.SelectedItem.Text == "Verified")
        {
            MessageBoxes.Confirm("Confirm",
            "DirectCall.SetAll({eventMask: { showMask: true}});",
            $"Set status '{cbStatus.SelectedItem.Text}' to all the observations?");

        }
        else
        {
            MessageBoxes.Confirm("Confirm",
            "DirectCall.SetAll({eventMask: { showMask: true}});",
            $"Set status '{cbStatus.SelectedItem.Text}' and reason '{cbStatusReason.SelectedItem.Text}' to all the observations?");

        }
    }

    [DirectMethod]
    public void SetAll()
    {
        using (LogContext.PushProperty("Method", "SetAll"))
        {
            try
            {
                RowSelectionModel batchRow = this.ImportBatchesGrid.SelectionModel.Primary as RowSelectionModel;
                ImportBatch batch = new ImportBatch(batchRow.SelectedRecordID);
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
                {
                    using (SharedDbConnectionScope connScope = new SharedDbConnectionScope())
                    {
                        if (cbStatus.SelectedItem.Text == "Verified")
                        {
                            new Update(Observation.Schema)
                                .Set(Observation.Columns.StatusID).EqualTo(Utilities.MakeGuid(cbStatus.SelectedItem.Value))
                                .Set(Observation.Columns.StatusReasonID).EqualTo(null)
                                .Where(Observation.Columns.ImportBatchID).IsEqualTo(batch.Id)
                                .Execute();
                        }
                        else
                        {
                            new Update(Observation.Schema)
                                .Set(Observation.Columns.StatusID).EqualTo(Utilities.MakeGuid(cbStatus.SelectedItem.Value))
                                .Set(Observation.Columns.StatusReasonID).EqualTo(Utilities.MakeGuid(cbStatusReason.SelectedItem.Value))
                                .Where(Observation.Columns.ImportBatchID).IsEqualTo(batch.Id)
                                .Execute();
                        }
                    }
                    ts.Complete();
                }
                ObservationsGrid.DataBind();
                EnableButtons();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unable to set status and reason to all observations");
                MessageBoxes.Error(ex, "Error", "Unable to set status and reason to all observations");
            }
        }
    }

    protected void ClearSelectedClick(object sender, DirectEventArgs e)
    {
        var sm = ObservationsGrid.SelectionModel.Primary as CheckboxSelectionModel;
        MessageBoxes.Confirm("Confirm",
        "DirectCall.ClearSelected({eventMask: { showMask: true}});",
        $"Clear status and reason on the {sm.SelectedRows.Count:N0} selected observations?");
    }

    [DirectMethod]
    public void ClearSelected()
    {
        using (LogContext.PushProperty("Method", "ClearSelected"))
        {
            try
            {
                var sm = ObservationsGrid.SelectionModel.Primary as CheckboxSelectionModel;
                foreach (SelectedRow row in sm.SelectedRows)
                {
                    Observation obs = new Observation(row.RecordID);
                    obs.StatusID = null;
                    obs.StatusReasonID = null;
                    obs.Save();
                }
                ObservationsGrid.DataBind();
                EnableButtons();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unable to clear status and reason on the selected observations");
                MessageBoxes.Error(ex, "Error", "Unable to clear status and reason on the selected observations");
            }
        }
    }

    protected void ClearAllClick(object sender, DirectEventArgs e)
    {
        var sm = ObservationsGrid.SelectionModel.Primary as CheckboxSelectionModel;
        MessageBoxes.Confirm("Confirm",
        "DirectCall.ClearAll({eventMask: { showMask: true}});",
        $"Clear status and reason on all the observations?");
    }

    [DirectMethod]
    public void ClearAll()
    {
        using (LogContext.PushProperty("Method", "ClearAll"))
        {
            try
            {
                RowSelectionModel batchRow = this.ImportBatchesGrid.SelectionModel.Primary as RowSelectionModel;
                ImportBatch batch = new ImportBatch(batchRow.SelectedRecordID);
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
                {
                    using (SharedDbConnectionScope connScope = new SharedDbConnectionScope())
                    {
                        new Update(Observation.Schema)
                            .Set(Observation.Columns.StatusID).EqualTo(null)
                            .Set(Observation.Columns.StatusReasonID).EqualTo(null)
                            .Where(Observation.Columns.ImportBatchID).IsEqualTo(batch.Id)
                            .Execute();
                    }
                    ts.Complete();
                }
                ObservationsGrid.DataBind();
                EnableButtons();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unable to clear status and reason on all observations");
                MessageBoxes.Error(ex, "Error", "Unable to clear status and reason on all observations");
            }
        }
    }

    protected void EnableButtons()
    {
        var sm = ObservationsGrid.SelectionModel.Primary as CheckboxSelectionModel;
        if (cbStatus.SelectedItem.Text == "Verified")
        {
            cbStatusReason.SelectedIndex = -1;
            cbStatusReason.SetValue(null);
            cbStatusReason.Disabled = true;
            btnSetSelected.Disabled = (cbStatus.SelectedIndex == -1) || (sm.SelectedRows.Count == 0);
            btnSetWithout.Disabled = (cbStatus.SelectedIndex == -1);
            btnSetAll.Disabled = (cbStatus.SelectedIndex == -1);
            btnClearSelected.Disabled = (sm.SelectedRows.Count == 0);
            btnClearAll.Disabled = false;
        }
        else
        {
            cbStatusReason.Disabled = false;
            btnSetSelected.Disabled = (cbStatus.SelectedIndex == -1) || (cbStatusReason.SelectedIndex == -1) || (sm.SelectedRows.Count == 0);
            btnSetWithout.Disabled = (cbStatus.SelectedIndex == -1) || (cbStatusReason.SelectedIndex == -1);
            btnSetAll.Disabled = (cbStatus.SelectedIndex == -1) || (cbStatusReason.SelectedIndex == -1);
            btnClearSelected.Disabled = (sm.SelectedRows.Count == 0);
            btnClearAll.Disabled = false;
        }
    }

    protected void EnableButtons(object sender, DirectEventArgs e)
    {
        EnableButtons();
    }
    #endregion
}