using Ext.Net;
using SAEON.Azure.CosmosDB;
using SAEON.Logs;
using SAEON.Observations.Azure;
using SAEON.Observations.Data;
using SubSonic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
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

    private void CreateSummary(SharedDbConnectionScope connScope, Guid importBatchId)
    {
        using (Logging.MethodCall(GetType(), new MethodCallParameters { { "ImportBatchID", importBatchId } }))
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Logging.Information("Creating Summary");
            ImportBatchSummary.Delete("ImportBatchID", importBatchId);
            var sql =
                "Insert Into ImportBatchSummary" + Environment.NewLine +
                "  (ImportBatchID, SensorID, InstrumentID, StationID, SiteID, PhenomenonOfferingID, PhenomenonUOMID, Count, Minimum, Maximum, Average, StandardDeviation, Variance," + Environment.NewLine +
                "   LatitudeNorth, LatitudeSouth, LongitudeWest, LongitudeEast, ElevationMinimum, ElevationMaximum, StartDate, EndDate)" + Environment.NewLine +
                "Select" + Environment.NewLine +
                "  ImportBatchID, SensorID, InstrumentID, StationID, SiteID, PhenomenonOfferingID, PhenomenonUOMID, COUNT(DataValue) Count, MIN(DataValue) Minimum, MAX(DataValue) Maximum, AVG(DataValue) Average, " + Environment.NewLine +
                "  STDEV(DataValue) StandardDeviation, VAR(DataValue) Variance, " + Environment.NewLine +
                "  Max(Latitude) LatitudeNorth, Min(Latitude) LatitudeSouth, Min(Longitude) LongitudeWest, Max(Longitude) LongitudeEast, " + Environment.NewLine +
                "  Min(Elevation) ElevationMinimum, Max(Elevation) ElevationMaximum, Min(ValueDate) StartDate, Max(ValueDate) EndDate" + Environment.NewLine +
                "from" + Environment.NewLine +
                "  vObservationExpansion" + Environment.NewLine +
                "where" + Environment.NewLine +
                "  (ImportBatchID = @ImportBatchID)" + Environment.NewLine +
                "group by" + Environment.NewLine +
                "  ImportBatchID, SensorID, InstrumentID, StationID, SiteID, PhenomenonOfferingID, PhenomenonUOMID";
            using (var cmd = connScope.CurrentConnection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandTimeout = Convert.ToInt32(TimeSpan.Parse(ConfigurationManager.AppSettings["TransactionTimeout"]).TotalSeconds);
                var param = cmd.CreateParameter();
                param.DbType = DbType.Guid;
                param.ParameterName = "@ImportBatchID";
                param.Value = importBatchId;
                cmd.Parameters.Add(param);
                var n = cmd.ExecuteNonQuery();
                stopwatch.Stop();
                Logging.Information("Created Summary {count:N0} in {time}", n, stopwatch.Elapsed);
            }
        }
    }

    private void CreateDocuments(SharedDbConnectionScope connScope, Guid importBatchId)
    {
        T GetValue<T>(DataRow row, string colName)
        {
            if (row.IsNull(colName))
                return default;
            else
                return (T)row[colName];
        }

        if (!(Azure.Enabled && Azure.CosmosDBEnabled)) return;
        using (Logging.MethodCall(GetType(), new MethodCallParameters { { "ImportBatchID", importBatchId } }))
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var azure = new Azure();
            Logging.Information("Adding documents for ImportBatch {ImportBatchId}", importBatchId);
            var sql =
                "Select" + Environment.NewLine +
                "  *" + Environment.NewLine +
                "from" + Environment.NewLine +
                "  vObservationJSON" + Environment.NewLine +
                "where" + Environment.NewLine +
                "  ([ImportBatch.ID] = @ImportBatchID)";
            using (var cmd = connScope.CurrentConnection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandTimeout = Convert.ToInt32(TimeSpan.Parse(ConfigurationManager.AppSettings["TransactionTimeout"]).TotalSeconds);
                var param = cmd.CreateParameter();
                param.DbType = DbType.Guid;
                param.ParameterName = "@ImportBatchID";
                param.Value = importBatchId;
                cmd.Parameters.Add(param);
                var reader = cmd.ExecuteReader();
                var table = new DataTable("Observations");
                table.Load(reader);
                var cost = new AzureCost();
                var documents = new List<ObservationDocument>();
                var batchSize = azure.BatchSize;
                foreach (DataRow row in table.Rows)
                {
                    var document = new ObservationDocument
                    {
                        Id = GetValue<int>(row, "ID").ToString(),
                        ValueDate = new EpochDate(GetValue<DateTime>(row, "ValueDate")),
                        ValueDay = new EpochDate(GetValue<DateTime>(row, "ValueDay")),
                        ValueYear = GetValue<int>(row, "ValueYear"),
                        ValueDecade = GetValue<int>(row, "ValueDecade"),
                        TextValue = GetValue<string>(row, "TextValue"),
                        RawValue = GetValue<double?>(row, "RawValue"),
                        DataValue = GetValue<double?>(row, "DataValue"),
                        Comment = GetValue<string>(row, "Comment"),
                        CorrelationId = GetValue<Guid?>(row, "CorrelationId"),
                        Latitude = GetValue<double?>(row, "Latitude"),
                        Longitude = GetValue<double?>(row, "Longitude"),
                        Elevation = GetValue<double?>(row, "Elevation"),
                        UserId = GetValue<Guid>(row, "UserId"),
                        AddedDate = new EpochDate(GetValue<DateTime>(row, "AddedDate")),
                        AddedAt = new EpochDate(GetValue<DateTime>(row, "AddedAt")),
                        UpdatedAt = new EpochDate(GetValue<DateTime>(row, "UpdatedAt")),
                        ImportBatch = new ObservationImportBatch { Id = GetValue<Guid>(row, "ImportBatch.ID"), Code = GetValue<int>(row, "ImportBatch.Code"), Date = new EpochDate(GetValue<DateTime>(row, "ImportBatch.Date")) },
                        Site = new ObservationSite { Id = GetValue<Guid>(row, "Site.ID"), Code = GetValue<string>(row, "Site.Code"), Name = GetValue<string>(row, "Site.Name") },
                        Station = new ObservationStation { Id = GetValue<Guid>(row, "Station.ID"), Code = GetValue<string>(row, "Station.Code"), Name = GetValue<string>(row, "Station.Name") },
                        Instrument = new ObservationInstrument { Id = GetValue<Guid>(row, "Instrument.ID"), Code = GetValue<string>(row, "Instrument.Code"), Name = GetValue<string>(row, "Instrument.Name") },
                        Sensor = new ObservationSensor { Id = GetValue<Guid>(row, "Sensor.ID"), Code = GetValue<string>(row, "Sensor.Code"), Name = GetValue<string>(row, "Sensor.Name") },
                        Phenomenon = new ObservationPhenomenon { Id = GetValue<Guid>(row, "Phenomenon.ID"), Code = GetValue<string>(row, "Phenomenon.Code"), Name = GetValue<string>(row, "Phenomenon.Name") },
                        Offering = new ObservationOffering { Id = GetValue<Guid>(row, "Offering.ID"), Code = GetValue<string>(row, "Offering.Code"), Name = GetValue<string>(row, "Offering.Name") },
                        Unit = new ObservationUnit { Id = GetValue<Guid>(row, "Unit.ID"), Code = GetValue<string>(row, "Unit.Code"), Name = GetValue<string>(row, "Unit.Name") },
                        Status = row.IsNull("Status.ID") ? null : new ObservationStatus { Id = GetValue<Guid>(row, "Status.ID"), Code = GetValue<string>(row, "Status.Code"), Name = GetValue<string>(row, "Status.Name") },
                        StatusReason = row.IsNull("StatusReason.ID") ? null : new ObservationStatusReason { Id = GetValue<Guid>(row, "StatusReason.ID"), Code = GetValue<string>(row, "StatusReason.Code"), Name = GetValue<string>(row, "StatusReason.Name") },
                    };
                    //Logging.Verbose("Adding {@Observation}", observation);
                    documents.Add(document);
                    if (documents.Count >= batchSize)
                    {
                        var batchCost = azure.UpsertObservations(documents);
                        cost += batchCost;
                        Logging.Information("Added batch of {BatchSize:N0} for ImportBatch {ImportBatchId} Cost: {BatchCost}", documents.Count, importBatchId, batchCost);
                        documents.Clear();
                    }
                }
                if (documents.Any())
                {
                    var batchCost = azure.UpsertObservations(documents);
                    cost += batchCost;
                    Logging.Information("Added batch of {BatchSize:N0} for ImportBatch {ImportBatchId} Cost: {BatchCost}", documents.Count, importBatchId, batchCost);
                }
                Logging.Information("Added {Documents:N0} documents for ImportBatch {ImportBatchId} in {elapsed}, Cost: {Cost}", table.Rows.Count, importBatchId, stopwatch.Elapsed, cost);
            }
        }
    }

    private void DeleteDocuments(Guid importBatchId)
    {
        if (!(Azure.Enabled && Azure.CosmosDBEnabled)) return;
        using (Logging.MethodCall(GetType(), new MethodCallParameters { { "ImportBatchID", importBatchId } }))
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var azure = new Azure();
            Logging.Information("Deleting documents for ImportBatch {importBatchId}", importBatchId);
            var cost = azure.DeleteImportBatch(importBatchId);
            Logging.Information("Deleted documents for ImportBatch {importBatchId} in {elapsed}, Cost: {Cost}", importBatchId, stopwatch.Elapsed, cost);
        }
    }

    protected void FileSelected(object sender, DirectEventArgs e)
    {
        using (Logging.MethodCall(GetType()))
        {
            Logging.Information("Import file selected");
        }
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
                var fi = new FileInfo(DataFileUpload.PostedFile.FileName);
                batch.FileName = fi.Name;

                var durationStopwatch = new Stopwatch();
                durationStopwatch.Start();
                var stageStopwatch = new Stopwatch();
                stageStopwatch.Start();
                Logging.Information("Import Version: {version:F2} DataSource: {dataSource} FileName: {fileName}", 1.45, batch.DataSource.Name, batch.FileName);
                List<SchemaValue> values = Import(DataSourceId, batch);
                stageStopwatch.Stop();
                Logging.Information("Imported {count:N0} values in {elapsed}", values.Count, stageStopwatch.Elapsed);

                if (!values.Any())
                {
                    MessageBoxes.Warning("Warning", "No values have been imported.");
                }
                else
                {

                    Logging.Information("Saving {count:N0} observations.", values.Count);

                    // Create DataTable from good values
                    stageStopwatch.Start();
                    Logging.Information("Creating DataTable");
                    var dtObservations = new DataTable("Observations");
                    dtObservations.Columns.Add("ImportBatchID", typeof(Guid));
                    dtObservations.Columns.Add("SensorID", typeof(Guid));
                    dtObservations.Columns.Add("ValueDate", typeof(DateTime));
                    dtObservations.Columns.Add("TextValue", typeof(string));
                    dtObservations.Columns.Add("RawValue", typeof(double));
                    dtObservations.Columns.Add("DataValue", typeof(double));
                    dtObservations.Columns.Add("PhenomenonOfferingID", typeof(Guid));
                    dtObservations.Columns.Add("PhenomenonUOMID", typeof(Guid));
                    dtObservations.Columns.Add("Latitude", typeof(double));
                    dtObservations.Columns.Add("Longitude", typeof(double));
                    dtObservations.Columns.Add("Elevation", typeof(double));
                    dtObservations.Columns.Add("Comment", typeof(string));
                    dtObservations.Columns.Add("CorrelationID", typeof(Guid));
                    dtObservations.Columns.Add("AddedDate", typeof(DateTime));
                    dtObservations.Columns.Add("UserId", typeof(Guid));
                    //dtObservations.Columns.Add("", typeof());
                    var lastProgress100 = -1;
                    var goodValues = values.Where(i => i.IsValid).OrderBy(i => i.RowNum).ToList();
                    var nMax = goodValues.Count;
                    int n = 1;
                    foreach (var value in goodValues)
                    {
                        var progress = (double)n++ / nMax;
                        var progress100 = (int)(progress * 100);
                        var reportPorgress = (progress100 % 5 == 0) && (progress100 > 0) && (lastProgress100 != progress100);
                        var elapsed = stageStopwatch.Elapsed.TotalMinutes;
                        if (reportPorgress)
                        {
                            Logging.Information("{progress:p0} {value:n0} of {values:n0} values {min:n2} of {mins:n2} min", progress, n, nMax, elapsed, elapsed / progress);
                            lastProgress100 = progress100;
                        }
                        try
                        {
                            var dataRow = dtObservations.NewRow();
                            dataRow.SetValue<Guid>("ImportBatchID", batch.Id);
                            dataRow.SetValue<Guid>("SensorID", value.SensorID.Value);
                            dataRow.SetValue<DateTime>("ValueDate", value.DateValue);
                            dataRow.SetValue<string>("TextValue", value.TextValue);
                            dataRow.SetValue<double?>("RawValue", value.RawValue);
                            dataRow.SetValue<double?>("DataValue", value.DataValue);
                            dataRow.SetValue<Guid>("PhenomenonOfferingID", value.PhenomenonOfferingID.Value);
                            dataRow.SetValue<Guid>("PhenomenonUOMID", value.PhenomenonUOMID.Value);
                            dataRow.SetValue<double?>("Latitude", value.Latitude);
                            dataRow.SetValue<double?>("Longitude", value.Longitude);
                            dataRow.SetValue<double?>("Elevation", value.Elevation);
                            dataRow.SetValue<Guid>("CorrelationID", value.CorrelationID);
                            dataRow.SetValue<string>("Comment", value.Comment);
                            dataRow.SetValue<Guid>("UserId", AuthHelper.GetLoggedInUserId);
                            dataRow.SetValue<DateTime>("AddedDate", DateTime.Now);
                            //dataRow.SetValue<>("", value.);
                            dtObservations.Rows.Add(dataRow);
                            if (reportPorgress)
                            {
                                Logging.Information("DataRow: {row}", dataRow.AsString());
                            }

                        }
                        catch (Exception ex)
                        {
                            Logging.Exception(ex, "Unable to add DataRow: {row}", value.RowNum);
                            throw;
                        }
                    }
                    stageStopwatch.Stop();
                    Logging.Information("Created DataTable {count:n0} rows in {elapsed}", dtObservations.Rows.Count, stageStopwatch.Elapsed);

                    using (TransactionScope tranScope = Utilities.NewTransactionScope())
                    using (SharedDbConnectionScope connScope = new SharedDbConnectionScope())
                    {
                        try
                        {
                            if (values.FirstOrDefault(t => t.IsValid) == null)
                            {
                                Logging.Verbose("Error: IsValid: {count:N0}", values.Where(t => !t.IsValid).Count());
                                batch.Status = (int)ImportBatchStatus.DatalogWithErrors;
                            }
                            else
                            {
                                batch.Status = (int)ImportBatchStatus.NoLogErrors;
                            }

                            batch.UserId = AuthHelper.GetLoggedInUserId;
                            batch.Save();

                            Logging.Information("Creating error logs");
                            stageStopwatch.Start();
                            lastProgress100 = -1;
                            var badValues = values.Where(i => !i.IsValid).OrderBy(i => i.RowNum).ToList();
                            nMax = badValues.Count;
                            n = 1;
                            foreach (var value in badValues)
                            {
                                var progress = (double)n++ / nMax;
                                var progress100 = (int)(progress * 100);
                                var reportPorgress = (progress100 % 5 == 0) && (progress100 > 0) && (lastProgress100 != progress100);
                                var elapsed = stageStopwatch.Elapsed.TotalMinutes;
                                if (reportPorgress)
                                {
                                    Logging.Information("{progress:p0} {value:n0} of {values:n0} values {min:n2} of {mins:n2} min", progress, n, nMax, elapsed, elapsed / progress);
                                    lastProgress100 = progress100;
                                }

                                Logging.Error("RowNum: {RowNum:N0} IsValid: {isValid} IsDuplicate: {isDuplicate} IsDuplicateOfNull: {isDuplicateOfNull}", value.RowNum, value.IsValid, value.IsDuplicate, value.IsDuplicateOfNull);
                                if (batch.Status != (int)ImportBatchStatus.DatalogWithErrors)
                                {
                                    batch.Status = (int)ImportBatchStatus.DatalogWithErrors;
                                    batch.Save();
                                }

                                DataLog logrecord = new DataLog()
                                {
                                    SensorID = value.SensorID
                                };
                                if (value.DateValueInvalid)
                                {
                                    logrecord.InvalidDateValue = value.InvalidDateValue;
                                }
                                else if (value.DateValue != DateTime.MinValue)
                                {
                                    logrecord.ValueDate = value.DateValue;
                                }

                                if (value.TimeValueInvalid)
                                {
                                    logrecord.InvalidTimeValue = value.InvalidTimeValue;
                                }

                                if (value.TimeValue.HasValue && value.TimeValue != DateTime.MinValue)
                                {
                                    logrecord.ValueTime = value.TimeValue;
                                }

                                if (value.RawValueInvalid)
                                {
                                    logrecord.ValueText = value.InvalidRawValue;
                                }
                                else
                                {
                                    logrecord.RawValue = value.RawValue;
                                }

                                if (value.DataValueInvalid)
                                {
                                    logrecord.TransformValueText = value.InvalidDataValue;
                                }
                                else
                                {
                                    logrecord.DataValue = value.DataValue;
                                }

                                if (value.InvalidOffering)
                                {
                                    logrecord.InvalidOffering = value.PhenomenonOfferingID.Value.ToString();
                                }
                                else
                                {
                                    logrecord.PhenomenonOfferingID = value.PhenomenonOfferingID.Value;
                                }

                                if (value.InvalidUOM)
                                {
                                    logrecord.InvalidUOM = value.PhenomenonUOMID.Value.ToString();
                                }
                                else
                                {
                                    logrecord.PhenomenonUOMID = value.PhenomenonUOMID.Value;
                                }

                                logrecord.RawFieldValue = String.IsNullOrWhiteSpace(value.FieldRawValue) ? "" : value.FieldRawValue;
                                logrecord.ImportDate = DateTime.Now;
                                logrecord.ImportBatchID = batch.Id;

                                logrecord.DataSourceTransformationID = value.DataSourceTransformationID;
                                logrecord.ImportStatus = String.Join(",", value.InvalidStatuses.Select(s => new Status(s).Name));
                                logrecord.StatusID = new Guid(value.InvalidStatuses[0]);
                                logrecord.UserId = AuthHelper.GetLoggedInUserId;

                                if (!string.IsNullOrWhiteSpace(value.Comment))
                                {
                                    logrecord.Comment = value.Comment.TrimEnd();
                                }

                                logrecord.Latitude = value.Latitude;
                                logrecord.Longitude = value.Longitude;
                                logrecord.Elevation = value.Elevation;
                                logrecord.CorrelationID = value.CorrelationID;
                                Logging.Verbose("BatchID: {id} Status: {status} ImportStatus: {importStatus}", batch.Id, logrecord.StatusID, logrecord.ImportStatus);
                                try
                                {
                                    logrecord.Save();
                                }
                                catch (Exception ex)
                                {
                                    Logging.Exception(ex, "Unable to create ErroLog: {rowNum}", value.RowNum);
                                    throw;
                                }
                            }
                            stageStopwatch.Stop();
                            Logging.Information("Created {count:N0} error logs in {time}", nMax, stageStopwatch.Elapsed);
                            // Bulk insert
                            stageStopwatch.Start();
                            Logging.Information("Starting bulk insert");
                            using (var bulkInsert = new SqlBulkCopy((SqlConnection)connScope.CurrentConnection, SqlBulkCopyOptions.CheckConstraints | SqlBulkCopyOptions.FireTriggers, null))
                            {
                                bulkInsert.BatchSize = 10000;
                                bulkInsert.BulkCopyTimeout = 5 * 60 * 60;
                                bulkInsert.NotifyAfter = bulkInsert.BatchSize;
                                bulkInsert.SqlRowsCopied += BulkInsert_SqlRowsCopied;
                                bulkInsert.DestinationTableName = "Observation";
                                foreach (DataColumn col in dtObservations.Columns)
                                {
                                    bulkInsert.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                                }
                                bulkInsert.WriteToServer(dtObservations);
                            }
                            stageStopwatch.Stop();
                            Logging.Information("Bulk inserted {count:N0} observations in {time}", goodValues.Count, stageStopwatch.Elapsed);
                            // Summaries
                            CreateSummary(connScope, batch.Id);
                            // Documents
                            CreateDocuments(connScope, batch.Id);
                            Auditing.Log(GetType(), new MethodCallParameters { { "ID", batch.Id }, { "Code", batch.Code }, { "Status", batch.Status } });
                            batch.DurationInSecs = (int)durationStopwatch.Elapsed.TotalSeconds;
                            batch.Save();
                            tranScope.Complete();
                            Logging.Information("Import: {count:N0} observations, summary and documents in {duration}", values.Count, durationStopwatch.Elapsed);
                        }
                        catch (Exception ex)
                        {
                            Logging.Exception(ex, "Unable to save {count:N0} observations in {duration}", values.Count, durationStopwatch.Elapsed);
                            throw;
                        }
                    }

                    ObservationsGridStore.DataBind();
                    ImportBatchesGrid.GetStore().DataBind();
                    SummaryGridStore.DataBind();
                    DataLogGrid.GetStore().DataBind();
                    ImportWindow.Hide();
                    X.Msg.Hide();
                }
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
                List<object> errors = new List<object>
                {
                    new { ErrorMessage = ex.Message, LineNo = 1, RecordString = "" }
                };
                ErrorGridStore.DataSource = errors;
                ErrorGrid.DataBind();
                MessageBoxes.Error("Error", $"An error occurred while importing - {ex.Message}");
                X.Msg.Hide();
            }
        }
    }

    private void BulkInsert_SqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
    {
        Logging.Information("Batch: {rows}", e.RowsCopied);
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
    private List<SchemaValue> Import(Guid DataSourceId, ImportBatch batch)
    {
        using (Logging.MethodCall(GetType(), new MethodCallParameters { { "ImportBatch", batch.Code } }))
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
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                Logging.Information("Moving from DataLog to Observation");
                try
                {
                    Observation obs = new Observation()
                    {
                        SensorID = new Guid(cbSensor.SelectedItem.Value)
                    };
                    DateTime datevalue = (DateTime)ValueDate.Value;

                    if (TimeValue.Value.ToString() != "-10675199.02:48:05.4775808")
                    {
                        datevalue = datevalue.AddMilliseconds(((TimeSpan)TimeValue.Value).TotalMilliseconds);
                    }

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
                        using (TransactionScope tranScope = Utilities.NewTransactionScope())
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
                            tranScope.Complete();
                            stopwatch.Stop();
                            Logging.Information("Moved from DataLog to Observation in {time}", stopwatch.Elapsed);
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
                    stopwatch.Stop();
                    Logging.Exception(ex, "Unable to move from DataLog to Observation in {time}", stopwatch.Elapsed);
                    throw;
                }
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
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
    public void DeleteBatch(Guid importBatchId)
    {
        using (Logging.MethodCall(GetType(), new MethodCallParameters { { "ImportBatchID", importBatchId } }))
        {
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                try
                {
                    Logging.Information("Deleting ImportBatch {ImportBatchID}", importBatchId);
                    using (TransactionScope ts = Utilities.NewTransactionScope())
                    using (SharedDbConnectionScope connScope = new SharedDbConnectionScope())
                    {
                        DataLog.Delete(DataLog.Columns.ImportBatchID, importBatchId);
                        Logging.Information("Deleting observations for ImportBatch {ImportBatchID}", importBatchId);
                        var t = stopwatch.Elapsed;
                        Observation.Delete(Observation.Columns.ImportBatchID, importBatchId);
                        Logging.Information("Deleted observations for ImportBatch {ImportBatchID} in {Elapsed}", importBatchId, stopwatch.Elapsed - t);
                        t = stopwatch.Elapsed;
                        Logging.Information("Deleting summaries for ImportBatch {ImportBatchID}", importBatchId);
                        ImportBatchSummary.Delete(ImportBatchSummary.Columns.ImportBatchID, importBatchId);
                        t = stopwatch.Elapsed;
                        Logging.Information("Deleted summaries for ImportBatch {ImportBatchID} in {Elapsed}", importBatchId, stopwatch.Elapsed - t);
                        ImportBatch.Delete(importBatchId);
                        DeleteDocuments(importBatchId);
                        ts.Complete();
                        stopwatch.Stop();
                        Logging.Information("Deleted ImportBatch {ImportBatchID} in {time}", importBatchId, stopwatch.Elapsed);
                    }

                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    Logging.Exception(ex, "Unable to delete ImportBatch {ImportBatchID} in {time}", importBatchId, stopwatch.Elapsed);
                }

                ImportBatchesGridStore.DataBind();
                DataLogGridStore.DataBind();
                SummaryGridStore.DataBind();
                ObservationsGridStore.DataBind();

            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
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
        using (Logging.MethodCall(GetType(), new MethodCallParameters { { "ImportBatchID", ImportBatchId } }))
        {
            try
            {
                ObservationCollection col = new ObservationCollection().Where(Observation.Columns.ImportBatchID, ImportBatchId).Load();
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                try
                {
                    Logging.Information("Moving import batch to DataLog {Id}", ImportBatchId);
                    using (TransactionScope tranScope = Utilities.NewTransactionScope())
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
                        tranScope.Complete();
                        stopwatch.Stop();
                        Logging.Information("Moved import batch to DataLog {Id} in {time}", ImportBatchId, stopwatch.Elapsed);
                    }
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    Logging.Exception(ex, "Unable to moved import batch to DataLog {Id} in {time}", ImportBatchId, stopwatch.Elapsed);
                }

                ImportBatchesGridStore.DataBind();
                DataLogGridStore.DataBind();
                SummaryGridStore.DataBind();
                ObservationsGridStore.DataBind();
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
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
        using (Logging.MethodCall(GetType(), new MethodCallParameters { { "ID", Id } }))
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
        using (Logging.MethodCall(GetType(), new MethodCallParameters { { "Id", Id } }))
        {
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                try
                {
                    Logging.Information("Moving from DataLog {Id}", Id);
                    using (TransactionScope tranScope = Utilities.NewTransactionScope())
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
                        tranScope.Complete();
                        stopwatch.Stop();
                        Logging.Information("Moved from DataLog {Id} in {time}", Id, stopwatch.Elapsed);
                    }

                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    Logging.Exception(ex, "Unable to moved from DataLog {Id} in {time}", Id, stopwatch.Elapsed);
                }

                ImportBatchesGridStore.DataBind();
                DataLogGridStore.DataBind();
                SummaryGridStore.DataBind();
                ObservationsGridStore.DataBind();
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
                throw;
            }
        }
    }

    #endregion Import Batches

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
                    {
                        obs.StatusReasonID = null;
                    }
                    else
                    {
                        obs.StatusReasonID = Utilities.MakeGuid(cbStatusReason.SelectedItem.Value);
                    }

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
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                try
                {
                    using (TransactionScope ts = Utilities.NewTransactionScope())
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
                        ts.Complete();
                        stopwatch.Stop();
                        Logging.Information("SetWithOut in {time}", stopwatch.Elapsed);
                    }

                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    Logging.Exception(ex, "Unable to SetWithOut in {time}", stopwatch.Elapsed);
                    throw;
                }
                var sm = ObservationsGrid.SelectionModel.Primary as CheckboxSelectionModel;
                sm.ClearSelections();
                ObservationsGrid.DataBind();
                EnableButtons();
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
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
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                try
                {
                    ImportBatch batch = new ImportBatch(batchRow.SelectedRecordID);
                    using (TransactionScope ts = Utilities.NewTransactionScope())
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
                        ts.Complete();
                        stopwatch.Stop();
                        Logging.Information("SetAll in {time}", stopwatch.Elapsed);
                    }
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    Logging.Exception(ex, "Unable to SetAll in {time}", stopwatch.Elapsed);
                    throw;
                }
                var sm = ObservationsGrid.SelectionModel.Primary as CheckboxSelectionModel;
                sm.ClearSelections();
                ObservationsGrid.DataBind();
                EnableButtons();
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
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
        //var sm = ObservationsGrid.SelectionModel.Primary as CheckboxSelectionModel;
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
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                try
                {
                    RowSelectionModel batchRow = ImportBatchesGrid.SelectionModel.Primary as RowSelectionModel;
                    ImportBatch batch = new ImportBatch(batchRow.SelectedRecordID);
                    using (TransactionScope ts = Utilities.NewTransactionScope())
                    using (SharedDbConnectionScope connScope = new SharedDbConnectionScope())
                    {
                        new Update(Observation.Schema)
                            .Set(Observation.Columns.StatusID).EqualTo(null)
                            .Set(Observation.Columns.StatusReasonID).EqualTo(null)
                            .Where(Observation.Columns.ImportBatchID).IsEqualTo(batch.Id)
                            .Execute();
                        ts.Complete();
                        stopwatch.Stop();
                        Logging.Information("ClearAll in {time}", stopwatch.Elapsed);
                    }
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    Logging.Exception(ex, "Unable to ClearAll in {time}", stopwatch.Elapsed);
                    throw;
                }
                var sm = ObservationsGrid.SelectionModel.Primary as CheckboxSelectionModel;
                sm.ClearSelections();
                ObservationsGrid.DataBind();
                EnableButtons();
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
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

    #endregion Observations

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

    #endregion Summary
}