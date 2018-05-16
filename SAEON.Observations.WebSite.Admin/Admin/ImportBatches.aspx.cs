using Ext.Net;
using SAEON.Logs;
using SAEON.Observations.Data;
using SubSonic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Transactions;

public partial class Admin_ImportBatches : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!X.IsAjaxRequest)
        {
            Store store = cbDataSource.GetStore();
            SqlQuery sourceQuery = new Select(DataSource.IdColumn, DataSource.NameColumn).From(DataSource.Schema).OrderAsc(DataSource.NameColumn.QualifiedName);
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

            cbUnitofMeasure.GetStore().DataSource = new Select(PhenomenonUOM.IdColumn, UnitOfMeasure.UnitColumn)
              .From(UnitOfMeasure.Schema)
              .InnerJoin(PhenomenonUOM.UnitOfMeasureIDColumn, UnitOfMeasure.IdColumn)
              .ExecuteDataSet().Tables[0];
            cbUnitofMeasure.GetStore().DataBind();

            StatusStore.DataSource = new StatusCollection().OrderByAsc(Status.Columns.Name).Load();
            StatusStore.DataBind();
            StatusReasonStore.DataSource = new StatusReasonCollection().OrderByAsc(StatusReason.Columns.Name).Load();
            StatusReasonStore.DataBind();
        }
    }

    #region Import Batches
    protected void ImportBatchesGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        using (Logging.MethodCall(GetType()))
        {
            try
            {
                //Log.Verbose("ImportBatchesGridStore_RefreshData");
                ImportBatchesGridStore.DataSource = ImportBatchRepository.GetPagedList(e, e.Parameters[GridFilters1.ParamPrefix]);
                ImportBatchesGridStore.DataBind();
                //(ImportBatchesGridStore.Proxy[0] as PageProxy).Total = ImportBatchesGridStore.DataSource
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
                throw;
            }
        }
    }

    protected void DataLogGrid_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["ImportBatchID"] != null && e.Parameters["ImportBatchID"].ToString() != "-1")
        {
            Guid BatchId = Utilities.MakeGuid(e.Parameters["ImportBatchID"].ToString());
            DataLogGridStore.DataSource = DataLogRepository.GetPagedListByBatch(e, e.Parameters[GridFiltersDataLog.ParamPrefix], BatchId);
            DataLogGridStore.DataBind();
        }
    }

    private int duplicates = 0;
    private int nullDuplicates = 0;

    private void CreateSummary(SharedDbConnectionScope connScope, Guid importBatchId)
    {
        var cmd = connScope.CurrentConnection.CreateCommand();
        ImportBatchSummary.Delete("ImportBatchID", importBatchId);
        var sql =
            "Insert Into ImportBatchSummary" + Environment.NewLine +
            "  (ImportBatchID, SensorID, PhenomenonOfferingID, PhenomenonUOMID, Count, Minimum, Maximum, Average, StandardDeviation, Variance)" + Environment.NewLine +
            "Select" + Environment.NewLine +
            "  ImportBatchID, SensorID, PhenomenonOfferingID, PhenomenonUOMID, COUNT(DataValue) Count, MIN(DataValue) Minimum, MAX(DataValue) Maximum, AVG(DataValue) Average, STDEV(DataValue) StandardDeviation, VAR(DataValue) Variance" + Environment.NewLine +
            "from" + Environment.NewLine +
            "  Observation" + Environment.NewLine +
            "where" + Environment.NewLine +
            "  (ImportBatchID = @ImportBatchID)" + Environment.NewLine +
            "group by" + Environment.NewLine +
            "  ImportBatchID, SensorID, PhenomenonOfferingID, PhenomenonUOMID";
        cmd.CommandText = sql;
        var param = cmd.CreateParameter();
        param.DbType = DbType.Guid;
        param.ParameterName = "@ImportBatchID";
        param.Value = importBatchId;
        cmd.Parameters.Add(param);
        var n = cmd.ExecuteNonQuery();
        Logging.Verbose("Added {Summaries} summaries", n);
    }

    protected void UploadClick(object sender, DirectEventArgs e)
    {
        using (Logging.MethodCall(GetType()))
        {
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

                ImportBatch batch = new ImportBatch()
                {
                    Id = Guid.NewGuid(),
                    DataSourceID = DataSourceId,
                    ImportDate = DateTime.Now
                };
                FileInfo fi;
                //if (!string.IsNullOrWhiteSpace(LogFileUpload.PostedFile.FileName))
                //{
                //    fi = new FileInfo(LogFileUpload.PostedFile.FileName);
                //    batch.LogFileName = fi.Name;
                //}

                fi = new FileInfo(DataFileUpload.PostedFile.FileName);
                batch.FileName = fi.Name;

                Logging.Information("Import Version: {version} DataSource: {dataSource} FileName: {fileName}", 1.30, batch.DataSource.Name, batch.FileName);
                List<SchemaValue> values = Import(DataSourceId, batch);

                if (values.Any())
                {
                    try
                    {
                        Logging.Information("Saving {count} observations.", values.Count);
                        duplicates = 0;
                        nullDuplicates = 0;
                        using (TransactionScope ts = Utilities.NewTransactionScope())
                        {
                            using (SharedDbConnectionScope connScope = new SharedDbConnectionScope())
                            {
                                //ImportBatch batch = new ImportBatch();
                                //batch.Guid = Guid.NewGuid();

                                if (values.FirstOrDefault(t => t.IsValid) == null)
                                {
                                    Logging.Verbose("Error: IsValid: {count}", values.Where(t => !t.IsValid).Count());
                                    batch.Status = (int)ImportBatchStatus.DatalogWithErrors;
                                }
                                else
                                    batch.Status = (int)ImportBatchStatus.NoLogErrors;
                                batch.UserId = AuthHelper.GetLoggedInUserId;
                                batch.Save();

                                for (int i = 0; i < values.Count; i++)
                                {
                                    SchemaValue schval = values[i];
                                    bool isDuplicate = false;

                                    if (schval.IsValid)
                                    {
                                        if (schval.RawValue.HasValue && IsDuplicateOfNull(schval, batch.Id))
                                        {
                                            if (batch.Status != (int)ImportBatchStatus.DatalogWithErrors)
                                            {
                                                batch.Status = (int)ImportBatchStatus.DatalogWithErrors;
                                                batch.Save();
                                            }
                                        }
                                        else
                                        {
                                            try
                                            {
                                                Observation Obrecord = new Observation()
                                                {
                                                    SensorID = schval.SensorID.Value,
                                                    ValueDate = schval.DateValue,
                                                    RawValue = schval.RawValue,
                                                    DataValue = schval.DataValue,
                                                    PhenomenonOfferingID = schval.PhenomenonOfferingID.Value,
                                                    PhenomenonUOMID = schval.PhenomenonUOMID.Value,
                                                    Latitude = schval.Latitude,
                                                    Longitude = schval.Longitude,
                                                    Elevation = schval.Elevation,
                                                    ImportBatchID = batch.Id,
                                                    CorrelationID = schval.CorrelationID,
                                                    UserId = AuthHelper.GetLoggedInUserId,
                                                    AddedDate = DateTime.Now
                                                };
                                                if (string.IsNullOrWhiteSpace(schval.Comment))
                                                    Obrecord.Comment = null;
                                                else
                                                    Obrecord.Comment = schval.Comment;

                                                if (string.IsNullOrWhiteSpace(schval.TextValue))
                                                    Obrecord.TextValue = null;
                                                else
                                                    Obrecord.TextValue = schval.TextValue;
                                                Obrecord.Save();
                                            }
                                            catch (SqlException ex) when (ex.Number == 2627)
                                            {
                                                isDuplicate = true;
                                                duplicates++;
                                            }
                                            //catch (SqlException ex) when (ex.Number == 55555)
                                            //{
                                            //}
                                            catch (SqlException ex)
                                            {
                                                Logging.Exception(ex, "Number: {num}", ex.Number);
                                                throw;
                                            }
                                        }
                                    }
                                    if (!schval.IsValid || isDuplicate)
                                    {
                                        Logging.Error("IsValid: {isValid} IsDuplicate: {isDuplicate} Duplicates: {duplicates}", schval.IsValid, isDuplicate, duplicates);
                                        if (batch.Status != (int)ImportBatchStatus.DatalogWithErrors)
                                        {
                                            batch.Status = (int)ImportBatchStatus.DatalogWithErrors;
                                            batch.Save();
                                        }
                                        //

                                        DataLog logrecord = new DataLog()
                                        {
                                            SensorID = schval.SensorID
                                        };
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

                                        if (schval.InvalidOffering)
                                            logrecord.InvalidOffering = schval.PhenomenonOfferingID.Value.ToString();
                                        else
                                            logrecord.PhenomenonOfferingID = schval.PhenomenonOfferingID.Value;

                                        if (schval.InvalidUOM)
                                            logrecord.InvalidUOM = schval.PhenomenonUOMID.Value.ToString();
                                        else
                                            logrecord.PhenomenonUOMID = schval.PhenomenonUOMID.Value;

                                        logrecord.RawFieldValue = String.IsNullOrWhiteSpace(schval.FieldRawValue) ? "" : schval.FieldRawValue;
                                        logrecord.ImportDate = DateTime.Now;
                                        logrecord.ImportBatchID = batch.Id;

                                        logrecord.DataSourceTransformationID = schval.DataSourceTransformationID;
                                        if (isDuplicate)
                                        {
                                            schval.InvalidStatuses.Insert(0, Status.Duplicate);
                                        }
                                        logrecord.ImportStatus = String.Join(",", schval.InvalidStatuses.Select(s => new Status(s).Name));
                                        logrecord.StatusID = new Guid(schval.InvalidStatuses[0]);
                                        logrecord.UserId = AuthHelper.GetLoggedInUserId;

                                        if (schval.Comment.Length > 0)
                                            logrecord.Comment = schval.Comment;

                                        logrecord.Latitude = schval.Latitude;
                                        logrecord.Longitude = schval.Longitude;
                                        logrecord.Elevation = schval.Elevation;
                                        logrecord.CorrelationID = schval.CorrelationID;
                                        Logging.Verbose("BatchID: {id} Status: {status} ImportStatus: {importstatus}", batch.Id, logrecord.StatusID, logrecord.ImportStatus);
                                        try
                                        {
                                            logrecord.Save();
                                        }
                                        catch (Exception ex)
                                        {
                                            Logging.Exception(ex, "Unable to add DataLog");
                                            throw;
                                        }
                                    }
                                }
                                // Summaries
                                CreateSummary(connScope, batch.Id);
                                Auditing.Log(GetType(), new ParameterList {
                                    { "ID", batch.Id }, { "Code", batch.Code }, { "Status", batch.Status} });
                            }
                            ts.Complete();
                            Logging.Information("Done");
                        }

                        ObservationsGridStore.DataBind();
                        ImportBatchesGrid.GetStore().DataBind();
                        SummaryGridStore.DataBind();
                        DataLogGrid.GetStore().DataBind();

                        ImportWindow.Hide();

                        X.Msg.Hide();
                    }
                    catch (Exception Ex)
                    {
                        Logging.Exception(Ex, "An error occurred while importing values");
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Title = "Warning",
                            Message = Ex.Message + "|"
                        });
                        throw;
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
                Logging.Exception(ex, "Unable to save import batch");
                List<object> errors = new List<object>
                {
                    new { ErrorMessage = ex.Message, LineNo = 1, RecordString = "" }
                };
                ErrorGridStore.DataSource = errors;
                ErrorGridStore.DataBind();
                X.Msg.Hide();
            }
        }
    }

    protected bool IsDuplicateOfNull(SchemaValue schval, Guid batchid)
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
            Logging.Error("nullDuplicates: {nullDuplicates}", ++nullDuplicates);
            //delete this observation and move it do the datalog with the new value in and a status saying its a duplicate of a null that now has a value. 
            //check if this works with transformations 
            DataLog d = new DataLog()
            {
                SensorID = oCol[0].SensorID,
                ImportDate = oCol[0].AddedDate,
                ValueDate = oCol[0].ValueDate,
                RawValue = schval.RawValue
            };
            if (d.DataValue != null)
                d.RawFieldValue = schval.DataValue.Value.ToString();

            d.DataValue = schval.DataValue;
            d.Comment = oCol[0].Comment;
            d.PhenomenonOfferingID = oCol[0].PhenomenonOfferingID;
            d.PhenomenonUOMID = oCol[0].PhenomenonUOMID;
            d.ImportBatchID = batchid;
            d.UserId = oCol[0].UserId;

            Status s = new Status(Status.DuplicateOfNull);
            d.StatusID = s.Id;
            d.ImportStatus = s.Name;
            d.Save();

            new Delete().From(Observation.Schema).Where(Observation.Columns.Id).IsEqualTo(oCol[0].Id).Execute();
            return true;


        }
        return false;
    }

#pragma warning disable IDE1006 // Naming Styles
    protected void cbOffering_RefreshData(object sender, StoreRefreshDataEventArgs e)
#pragma warning restore IDE1006 // Naming Styles
    {
        var Id = cbSensor.SelectedItem.Value;

        cbOffering.GetStore().DataSource = new Select(PhenomenonOffering.IdColumn, Offering.DescriptionColumn)
                 .From(Offering.Schema)
                 .InnerJoin(PhenomenonOffering.OfferingIDColumn, Offering.IdColumn)
                 .InnerJoin(Sensor.PhenomenonIDColumn, PhenomenonOffering.PhenomenonIDColumn)
                 .Where(Sensor.IdColumn.QualifiedName).IsEqualTo(Id)
                 .ExecuteDataSet().Tables[0];
        cbOffering.GetStore().DataBind();
    }

#pragma warning disable IDE1006 // Naming Styles
    protected void cbUnitofMeasure_RefreshData(object sender, StoreRefreshDataEventArgs e)
#pragma warning restore IDE1006 // Naming Styles
    {
        var Id = cbSensor.SelectedItem.Value;

        cbUnitofMeasure.GetStore().DataSource = new Select(PhenomenonUOM.IdColumn, UnitOfMeasure.UnitColumn)
               .From(UnitOfMeasure.Schema)
               .InnerJoin(PhenomenonUOM.UnitOfMeasureIDColumn, UnitOfMeasure.IdColumn)
               .InnerJoin(Sensor.PhenomenonIDColumn, PhenomenonUOM.PhenomenonIDColumn)
               .Where(Sensor.IdColumn.QualifiedName).IsEqualTo(Id)
               .ExecuteDataSet().Tables[0];
        cbUnitofMeasure.GetStore().DataBind();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    List<SchemaValue> Import(Guid DataSourceId, ImportBatch batch)
    {
        using (Logging.MethodCall(GetType(), new ParameterList { { "ImportBatch", batch.Code } }))
        {

            DataSource ds = new DataSource(DataSourceId);
            List<SchemaValue> ImportValues = new List<SchemaValue>();

            List<DateTime> recordgaps = new List<DateTime>();
            List<DateTime> datagaps = new List<DateTime>();

            string Data = String.Empty;
            //if (!ds.DataSchemaID.HasValue)
            //{

            SensorCollection col = new SensorCollection()
                .Where(Sensor.Columns.DataSourceID, DataSourceId)
                .Where(Sensor.Columns.DataSchemaID, SubSonic.Comparison.IsNot, null)
                .Load();

            //ImportLogHelper logHelper = new ImportLogHelper();

            //if (LogFileUpload.PostedFile.ContentLength > 0)
            //{
            //    using (StreamReader reader = new StreamReader(LogFileUpload.PostedFile.InputStream))
            //    {
            //        logHelper.ReadLog(reader.ReadToEnd());
            //    }
            //}

            using (StreamReader reader = new StreamReader(DataFileUpload.PostedFile.InputStream))
            {

                if (col.Count > 0)
                {
                    foreach (var sp in col)
                    {
                        DataFileUpload.PostedFile.InputStream.Seek(0, SeekOrigin.Begin);

                        DataSchema schema = sp.DataSchema;

                        Data = ImportSchemaHelper.GetWorkingStream(schema, reader);
                        //using (ImportSchemaHelper helper = new ImportSchemaHelper(ds, schema, Data, sp, logHelper))
                        ImportSchemaHelper helper = new ImportSchemaHelper(ds, schema, Data, batch, sp/*, logHelper*/);
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
                    ImportSchemaHelper helper = new ImportSchemaHelper(ds, schema, Data, batch, null/*, logHelper*/);
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
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void SaveObservation(object sender, DirectEventArgs e)
    {
        using (Logging.MethodCall(GetType()))
        {
            try
            {
                RowSelectionModel batchRow = ImportBatchesGrid.SelectionModel.Primary as RowSelectionModel;
                ImportBatch batch = new ImportBatch(batchRow.SelectedRecordID);

                //DataLog log = new DataLog();

                Observation obs = new Observation()
                {
                    SensorID = new Guid(cbSensor.SelectedItem.Value)
                };
                DateTime datevalue = (DateTime)ValueDate.Value;

                if (TimeValue.Value.ToString() != "-10675199.02:48:05.4775808")
                    datevalue = datevalue.AddMilliseconds(((TimeSpan)TimeValue.Value).TotalMilliseconds);

                obs.ValueDate = datevalue;
                obs.RawValue = double.Parse(RawValue.Value.ToString());
                obs.DataValue = double.Parse(DataValue.Value.ToString());

                obs.PhenomenonOfferingID = new Guid(cbOffering.SelectedItem.Value);
                obs.PhenomenonUOMID = new Guid(cbUnitofMeasure.SelectedItem.Value);

                obs.ImportBatchID = batch.Id;
                obs.UserId = AuthHelper.GetLoggedInUserId;

                obs.Comment = String.IsNullOrWhiteSpace(tfComment.Text) ? null : tfComment.Text;

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
                    using (TransactionScope ts = Utilities.NewTransactionScope())
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
                            CreateSummary(connScope, batch.Id);
                        }

                        ts.Complete();
                    }

                    DetailWindow.Hide();

                    ImportBatchesGrid.GetStore().DataBind();
                    DataLogGrid.GetStore().DataBind();
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
                Logging.Exception(ex, "Unable to save observation");
                throw;
            }
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
        using (Logging.MethodCall(GetType(), new ParameterList { { "ImportBatchID", ImportBatchId } }))
        {
            try
            {
                using (TransactionScope ts = Utilities.NewTransactionScope())
                {
                    using (SharedDbConnectionScope connScope = new SharedDbConnectionScope())
                    {
                        DataLog.Delete(DataLog.Columns.ImportBatchID, ImportBatchId);
                        Observation.Delete(Observation.Columns.ImportBatchID, ImportBatchId);
                        ImportBatchSummary.Delete(ImportBatchSummary.Columns.ImportBatchID, ImportBatchId);
                        ImportBatch.Delete(ImportBatchId);
                    }

                    ts.Complete();
                }

                ImportBatchesGridStore.DataBind();
                DataLogGridStore.DataBind();
                SummaryGridStore.DataBind();
                ObservationsGridStore.DataBind();
            }
            catch (Exception ex)
            {
                Logging.Exception(ex, "Unable to delete batch");
                throw;
            }
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
        using (Logging.MethodCall(GetType(), new ParameterList { { "ImportBatchID", ImportBatchId } }))
        {
            try
            {
                ObservationCollection col = new ObservationCollection().Where(Observation.Columns.ImportBatchID, ImportBatchId).Load();
                using (TransactionScope ts = Utilities.NewTransactionScope())
                {
                    using (SharedDbConnectionScope connScope = new SharedDbConnectionScope())
                    {
                        for (int i = 0; i < col.Count; i++)
                        {
                            Observation ob = col[i];
                            DataLog log = new DataLog()
                            {
                                ValueDate = ob.ValueDate,
                                ValueTime = ob.ValueDate,
                                SensorID = ob.SensorID,
                                PhenomenonOfferingID = ob.PhenomenonOfferingID,
                                PhenomenonUOMID = ob.PhenomenonUOMID,
                                ImportBatchID = ob.ImportBatchID,
                                RawValue = ob.RawValue,
                                DataValue = ob.DataValue,
                                UserId = AuthHelper.GetLoggedInUserId,
                                StatusID = new Guid(Status.BatchRetracted),
                                CorrelationID = ob.CorrelationID
                            };
                            log.Save();
                            Observation.Delete(ob.Id);
                        }
                        CreateSummary(connScope, ImportBatchId);
                    }

                    ts.Complete();
                }

                ImportBatchesGridStore.DataBind();
                DataLogGridStore.DataBind();
                SummaryGridStore.DataBind();
                ObservationsGridStore.DataBind();
            }
            catch (Exception ex)
            {
                Logging.Exception(ex, "Unable to move batch");
                throw;
            }
        }
    }

    /*
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
        using (Logging.MethodCall(GetType(), new ParameterList { { "ID", Id } }))
        {
            try
            {
                using (TransactionScope ts = Utilities.NewTransactionScope())
                {
                    using (SharedDbConnectionScope connScope = new SharedDbConnectionScope())
                    {
                        Guid BatchId = new DataLog("ID", Id).ImportBatchID;
                        DataLog.Delete(DataLog.Columns.Id, Id);

                        SqlQuery q = new Select("ID").From(DataLog.Schema)
                            .Where(DataLog.Columns.ImportBatchID).IsEqualTo(BatchId);

                        if (q.GetRecordCount() == 0)
                        {
                            ImportBatch iB = new ImportBatch("ID", BatchId)
                            {
                                Status = (int)ImportBatchStatus.NoLogErrors
                            };
                            iB.Save();
                        }

                    }

                    ts.Complete();
                }

                ImportBatchesGridStore.DataBind();
                DataLogGridStore.DataBind();
                SummaryGridStore.DataBind();
                ObservationsGridStore.DataBind();

            }
            catch (Exception ex)
            {
                Logging.Exception(ex, "Unable to delete entry");
                throw;
            }
        }
    }
    */

    protected void ImportBatchesGridStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();
        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        //string js = BaseRepository.BuildExportQ("VImportBatch", gridData, visCols, sortCol, sortDir);
        //BaseRepository.doExport(type, js);
        BaseRepository.Export("vImportBatch", gridData, visCols, sortCol, sortDir, type, "Import Batches", Response);
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
        using (Logging.MethodCall(GetType(), new ParameterList { { "Id", Id } }))
        {
            try
            {
                using (TransactionScope ts = Utilities.NewTransactionScope())
                {
                    using (SharedDbConnectionScope connScope = new SharedDbConnectionScope())
                    {
                        DataLog d = new DataLog(Id);

                        Observation Obrecord = new Observation()
                        {
                            SensorID = d.SensorID.Value,
                            ValueDate = d.ValueDate.Value,
                            RawValue = d.RawValue,
                            DataValue = d.DataValue,
                            PhenomenonOfferingID = d.PhenomenonOfferingID.Value,
                            PhenomenonUOMID = d.PhenomenonUOMID.Value,
                            ImportBatchID = d.ImportBatchID,
                            UserId = AuthHelper.GetLoggedInUserId,
                            AddedDate = d.ImportDate,
                            Comment = d.Comment
                        };
                        Obrecord.Save();

                        //new Delete().From(DataLog.Schema).Where(DataLog.Columns.Id).IsEqualTo(d.Id).Execute();
                        DataLog.Delete("ID", d.Id);
                        CreateSummary(connScope, d.ImportBatchID);
                    }

                    ts.Complete();
                }

                ImportBatchesGridStore.DataBind();
                DataLogGridStore.DataBind();
                SummaryGridStore.DataBind();
                ObservationsGridStore.DataBind();
            }
            catch (Exception ex)
            {
                Logging.Exception(ex, "Unable to move to observation");
                throw;
            }
        }
    }
    #endregion

    #region Observations
    protected void ObservationsGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        using (Logging.MethodCall(GetType()))
        {
            try
            {
                //Log.Verbose("ObservationsGridStore_RefreshData");
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
                        //Logging.Information("List: {@list}", ObservationRepository.GetPagedListByBatch(e, e.Parameters[GridFiltersObservations.ParamPrefix], Id));
                        ObservationsGridStore.DataSource = ObservationRepository.GetPagedListByBatch(e, e.Parameters[GridFiltersObservations.ParamPrefix], Id);
                        ObservationsGridStore.DataBind();
                        EnableButtons();
                    }
                    catch (Exception ex)
                    {
                        Logging.Exception(ex);
                        MessageBoxes.Error(ex, "Error", "Unable to refresh Observations grid");
                    }
                }

            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
                throw;
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
        using (Logging.MethodCall(GetType()))
        {
            try
            {
                var sm = ObservationsGrid.SelectionModel.Primary as CheckboxSelectionModel;
                foreach (SelectedRow row in sm.SelectedRows)
                {
                    Observation obs = new Observation(row.RecordID)
                    {
                        StatusID = Utilities.MakeGuid(cbStatus.SelectedItem.Value)
                    };
                    if (cbStatus.SelectedItem.Text == "Verified")
                        obs.StatusReasonID = null;
                    else
                        obs.StatusReasonID = Utilities.MakeGuid(cbStatusReason.SelectedItem.Value);
                    obs.Save();
                }
                sm.ClearSelections();
                ObservationsGrid.DataBind();
                EnableButtons();
            }
            catch (Exception ex)
            {
                Logging.Exception(ex, "Unable to set status and reason to the selected observations");
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
        using (Logging.MethodCall(GetType()))
        {
            try
            {
                RowSelectionModel batchRow = ImportBatchesGrid.SelectionModel.Primary as RowSelectionModel;
                ImportBatch batch = new ImportBatch(batchRow.SelectedRecordID);
                using (TransactionScope ts = Utilities.NewTransactionScope())
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
                var sm = ObservationsGrid.SelectionModel.Primary as CheckboxSelectionModel;
                sm.ClearSelections();
                ObservationsGrid.DataBind();
                EnableButtons();
            }
            catch (Exception ex)
            {
                Logging.Exception(ex, "Unable to set status and reason to the observations without status");
                MessageBoxes.Error(ex, "Error", "Unable to set status and reason to the observations without status");
            }
        }
    }

    protected void SetTestClick(object sender, DirectEventArgs e)
    {
        var sm = ObservationsGrid.SelectionModel.Primary as CheckboxSelectionModel;
        var count = int.Parse(e.ExtraParams["count"]);

        MessageBoxes.Info("Info", $"Count: {count} Selected: {sm.SelectedRows.Count}");
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
        using (Logging.MethodCall(GetType()))
        {
            try
            {
                RowSelectionModel batchRow = ImportBatchesGrid.SelectionModel.Primary as RowSelectionModel;
                ImportBatch batch = new ImportBatch(batchRow.SelectedRecordID);
                using (TransactionScope ts = Utilities.NewTransactionScope())
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
                var sm = ObservationsGrid.SelectionModel.Primary as CheckboxSelectionModel;
                sm.ClearSelections();
                ObservationsGrid.DataBind();
                EnableButtons();
            }
            catch (Exception ex)
            {
                Logging.Exception(ex, "Unable to set status and reason to all observations");
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
        using (Logging.MethodCall(GetType()))
        {
            try
            {
                var sm = ObservationsGrid.SelectionModel.Primary as CheckboxSelectionModel;
                foreach (SelectedRow row in sm.SelectedRows)
                {
                    Observation obs = new Observation(row.RecordID)
                    {
                        StatusID = null,
                        StatusReasonID = null
                    };
                    obs.Save();
                }
                sm.ClearSelections();
                ObservationsGrid.DataBind();
                EnableButtons();
            }
            catch (Exception ex)
            {
                Logging.Exception(ex, "Unable to clear status and reason on the selected observations");
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
        using (Logging.MethodCall(GetType()))
        {
            try
            {
                RowSelectionModel batchRow = ImportBatchesGrid.SelectionModel.Primary as RowSelectionModel;
                ImportBatch batch = new ImportBatch(batchRow.SelectedRecordID);
                using (TransactionScope ts = Utilities.NewTransactionScope())
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
                var sm = ObservationsGrid.SelectionModel.Primary as CheckboxSelectionModel;
                sm.ClearSelections();
                ObservationsGrid.DataBind();
                EnableButtons();
            }
            catch (Exception ex)
            {
                Logging.Exception(ex, "Unable to clear status and reason on all observations");
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

    #region Summary
    protected void SummaryGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        using (Logging.MethodCall(GetType()))
        {
            try
            {
                if (e.Parameters["ImportBatchID"] != null && e.Parameters["ImportBatchID"].ToString() != "-1")
                {
                    Guid Id = Guid.Parse(e.Parameters["ImportBatchID"].ToString());
                    VImportBatchSummaryCollection col = new VImportBatchSummaryCollection()
                        .Where(VImportBatchSummary.Columns.ImportBatchID, Id)
                        .OrderByAsc(VImportBatchSummary.Columns.PhenomenonName)
                        .OrderByAsc(VImportBatchSummary.Columns.OfferingName)
                        .OrderByAsc(VImportBatchSummary.Columns.UnitOfMeasureUnit)
                        .Load();
                    SummaryGridStore.DataSource = col;
                    SummaryGridStore.DataBind();
                }
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
                throw;
            }
        }
    }

    #endregion

}