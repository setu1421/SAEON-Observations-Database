//#define IsTest
//#define CosmosDb
using Ext.Net;
#if UseCosmosDb
using SAEON.Azure.CosmosDB;
#endif
using SAEON.Core;
using SAEON.Logs;
#if UseCosmosDb
sing SAEON.Observations.Azure;
#endif
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
using System.Threading.Tasks;
using System.Transactions;

public partial class Admin_ImportBatches : System.Web.UI.Page
{
    private bool BulkInsert = false;
    private int BulkInsertBatchSize = 25000;
    private bool DeleteIndivdualObservations = false;
    private bool LogBadValues = false;
    private bool UseParallel = true;

    protected void Page_Load(object sender, EventArgs e)
    {
        using (SAEONLogs.MethodCall(GetType()))
        {
            try
            {
                SAEONLogs.Verbose("PageLoad Start");
                DeleteIndivdualObservations = ConfigurationManager.AppSettings["DeleteIndivdualObservations"].IsTrue();
                BulkInsert = ConfigurationManager.AppSettings["BulkInsert"].IsTrue();
                BulkInsertBatchSize = int.Parse(ConfigurationManager.AppSettings["BulkInsertBatchSize"] ?? "25000");
                LogBadValues = ConfigurationManager.AppSettings["LogBadValues"].IsTrue();
                UseParallel = ConfigurationManager.AppSettings["UseParallel"].IsTrue();
                SAEONLogs.Verbose("IsPostBack: {IsPostBack} IsAjaxRequest: {IsAjax}", IsPostBack, X.IsAjaxRequest);
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
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                throw;
            }
            finally
            {
                SAEONLogs.Verbose("PageLoad End");
            }
        }
    }

    #region ImportBatches

    protected void ImportBatchesGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        using (SAEONLogs.MethodCall(GetType()))
        {
            try
            {
                SAEONLogs.Verbose("ImportBatchesGridStore_RefreshData Start");
                ImportBatchesGridStore.DataSource = ImportBatchRepository.GetPagedList(e, e.Parameters[GridFilters1.ParamPrefix]);
                ImportBatchesGridStore.DataBind();
                //(ImportBatchesGridStore.Proxy[0] as PageProxy).Total = ImportBatchesGridStore.DataSource
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                throw;
            }
            finally
            {
                SAEONLogs.Verbose("ImportBatchesGridStore_RefreshData End");
            }
        }
    }

    protected void DataLogGrid_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        using (SAEONLogs.MethodCall(GetType()))
        {
            try
            {
                SAEONLogs.Verbose("DataLogGrid_RefreshData Start");
                if (e.Parameters["ImportBatchID"] != null && e.Parameters["ImportBatchID"].ToString() != "-1")
                {
                    Guid BatchId = Utilities.MakeGuid(e.Parameters["ImportBatchID"].ToString());
                    DataLogGridStore.DataSource = DataLogRepository.GetPagedListByBatch(e, e.Parameters[GridFiltersDataLog.ParamPrefix], BatchId);
                    DataLogGridStore.DataBind();
                }
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                throw;
            }
            finally
            {
                SAEONLogs.Verbose("DataLogGrid_RefreshData End");
            }
        }
    }

    private void DeleteObservations(SharedDbConnectionScope connScope, Guid importBatchId)
    {
        using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "ImportBatchID", importBatchId } }))
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            SAEONLogs.Information("Deleting observations for ImportBatch {ImportBatchID}", importBatchId);
            if (DeleteIndivdualObservations)
            {
                SAEONLogs.Information("Deleting observations for ImportBatch {ImportBatchID}", importBatchId);
                Observation.Delete(Observation.Columns.ImportBatchID, importBatchId);
                SAEONLogs.Information("Deleted observations for ImportBatch {ImportBatchID} in {Elapsed}", importBatchId, stopwatch.Elapsed.TimeStr());
            }
            else
            {
                using (var cmd = connScope.CurrentConnection.CreateCommand())
                {
                    cmd.CommandText = "Delete Observation where (ImportBatchId = @ImportBatchID)";
                    cmd.CommandTimeout = Convert.ToInt32(TimeSpan.Parse(ConfigurationManager.AppSettings["TransactionTimeout"]).TotalSeconds);
                    var param = cmd.CreateParameter();
                    param.DbType = DbType.Guid;
                    param.ParameterName = "@ImportBatchID";
                    param.Value = importBatchId;
                    cmd.Parameters.Add(param);
                    var count = cmd.ExecuteNonQuery();
                    SAEONLogs.Information("Deleted {count} observations for ImportBatch {ImportBatchID} in {Elapsed}", count, importBatchId, stopwatch.Elapsed.TimeStr());
                }
            }
        }
    }

    private void CreateSummary(SharedDbConnectionScope connScope, Guid importBatchId)
    {
        using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "ImportBatchID", importBatchId } }))
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            SAEONLogs.Information("Creating Summary");
            ImportBatchSummary.Delete("ImportBatchID", importBatchId);
            var sqlTest =
                "Select" + Environment.NewLine +
                "  ImportBatchID, SensorID, InstrumentID, StationID, SiteID, PhenomenonOfferingID, PhenomenonUOMID, COUNT(ID) Count, Min(ValueDate) StartDate, Max(ValueDate) EndDate" + Environment.NewLine +
                "from" + Environment.NewLine +
                "  vObservationExpansion" + Environment.NewLine +
                "where" + Environment.NewLine +
                "  (ImportBatchID = @ImportBatchID)" + Environment.NewLine +
                "group by" + Environment.NewLine +
                "  ImportBatchID, SensorID, InstrumentID, StationID, SiteID, PhenomenonOfferingID, PhenomenonUOMID";
            using (var cmd = connScope.CurrentConnection.CreateCommand())
            {
                cmd.CommandText = sqlTest;
                var param = cmd.CreateParameter();
                param.DbType = DbType.Guid;
                param.ParameterName = "@ImportBatchID";
                param.Value = importBatchId;
                cmd.Parameters.Add(param);
                var reader = cmd.ExecuteReader();
                var dt = new DataTable();
                dt.Load(reader);
                SAEONLogs.Information("ImportBatchSummaries: {ImportBatchSummaries}", dt.DumpCSV());
            }
            var sql =
                "Insert Into ImportBatchSummary" + Environment.NewLine +
                "  (ImportBatchID, SensorID, InstrumentID, StationID, SiteID, PhenomenonOfferingID, PhenomenonUOMID, Count, ValueCount, Minimum, Maximum, Average, StandardDeviation, Variance," + Environment.NewLine +
                "   LatitudeNorth, LatitudeSouth, LongitudeWest, LongitudeEast, ElevationMinimum, ElevationMaximum, StartDate, EndDate, VerifiedCount, UnverifiedCount)" + Environment.NewLine +
                "Select" + Environment.NewLine +
                "  ImportBatchID, SensorID, InstrumentID, StationID, SiteID, PhenomenonOfferingID, PhenomenonUOMID, COUNT(ID) Count, COUNT(DataValue) ValueCount, " + Environment.NewLine +
                "  MIN(DataValue) Minimum, MAX(DataValue) Maximum, AVG(DataValue) Average, STDEV(DataValue) StandardDeviation, VAR(DataValue) Variance, " + Environment.NewLine +
                "  Max(Latitude) LatitudeNorth, Min(Latitude) LatitudeSouth, Min(Longitude) LongitudeWest, Max(Longitude) LongitudeEast, " + Environment.NewLine +
                "  Min(Elevation) ElevationMinimum, Max(Elevation) ElevationMaximum, Min(ValueDate) StartDate, Max(ValueDate) EndDate, " + Environment.NewLine +
                "  (" + Environment.NewLine +
                "  Select" + Environment.NewLine +
                "    Count(*)" + Environment.NewLine +
                "  from" + Environment.NewLine +
                "    Observation" + Environment.NewLine +
                "    left join Status" + Environment.NewLine +
                "      on (Observation.StatusID = Status.ID)" + Environment.NewLine +
                "  where" + Environment.NewLine +
                "    ((Observation.ImportBatchID = vObservationExpansion.ImportBatchID) and" + Environment.NewLine +
                "     (Observation.SensorID = vObservationExpansion.SensorID) and" + Environment.NewLine +
                "     (Observation.PhenomenonOfferingID = vObservationExpansion.PhenomenonOfferingID) and" + Environment.NewLine +
                "     (Observation.PhenomenonUOMID = vObservationExpansion.PhenomenonUOMID) and" + Environment.NewLine +
                "     ((Observation.StatusID is null) or (Status.Name = 'Verified')))" + Environment.NewLine +
                "  ) VerifiedCount," + Environment.NewLine +
                "  (" + Environment.NewLine +
                "  Select" + Environment.NewLine +
                "    Count(*)" + Environment.NewLine +
                "  from" + Environment.NewLine +
                "    Observation" + Environment.NewLine +
                "    inner join Status" + Environment.NewLine +
                "      on (Observation.StatusID = Status.ID)" + Environment.NewLine +
                "  where" + Environment.NewLine +
                "    ((Observation.ImportBatchID = vObservationExpansion.ImportBatchID) and" + Environment.NewLine +
                "     (Observation.SensorID = vObservationExpansion.SensorID) and" + Environment.NewLine +
                "     (Observation.PhenomenonOfferingID = vObservationExpansion.PhenomenonOfferingID) and" + Environment.NewLine +
                "     (Observation.PhenomenonUOMID = vObservationExpansion.PhenomenonUOMID) and" + Environment.NewLine +
                "     (Status.Name <> 'Verified'))" + Environment.NewLine +
                "  ) UnverifiedCount" + Environment.NewLine +
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
                SAEONLogs.Information("Created Summary {count:N0} in {time}", n, stopwatch.Elapsed.TimeStr());
            }
        }
    }

#if UseCosmosDb
    private void CreateCosmosDBItems(SharedDbConnectionScope connScope, Guid importBatchId)
    {
        if (!(ObservationsAzure.Enabled && ObservationsAzure.CosmosDBEnabled)) return;
        using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "ImportBatchID", importBatchId } }))
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var azure = new ObservationsAzure();
            try
            {
                SAEONLogs.Information("Adding CosmosDB items for ImportBatch {ImportBatchId} BulkEnabled: {BulkEnabled}", importBatchId, ObservationsAzure.CosmosDBBulkEnabled);
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
                    var cost = new CosmosDBCost<ObservationItem>();
                    var items = new List<ObservationItem>();
                    var batchSize = azure.BatchSize;
                    foreach (DataRow row in table.Rows)
                    {
                        var item = new ObservationItem
                        {
                            Id = row.GetValue<int>("ID").ToString(),
                            ValueDate = new EpochDate(row.GetValue<DateTime>("ValueDate")),
                            ValueDay = new EpochDate(row.GetValue<DateTime>("ValueDay")),
                            ValueYear = row.GetValue<int>("ValueYear"),
                            ValueDecade = row.GetValue<int>("ValueDecade"),
                            TextValue = row.GetValue<string>("TextValue"),
                            RawValue = row.GetValue<double?>("RawValue"),
                            DataValue = row.GetValue<double?>("DataValue"),
                            Comment = row.GetValue<string>("Comment"),
                            CorrelationId = row.GetValue<Guid?>("CorrelationId"),
                            Latitude = row.GetValue<double?>("Latitude"),
                            Longitude = row.GetValue<double?>("Longitude"),
                            Elevation = row.GetValue<double?>("Elevation"),
                            UserId = row.GetValue<Guid>("UserId"),
                            AddedDate = new EpochDate(row.GetValue<DateTime>("AddedDate")),
                            AddedAt = new EpochDate(row.GetValue<DateTime>("AddedAt")),
                            UpdatedAt = new EpochDate(row.GetValue<DateTime>("UpdatedAt")),
                            ImportBatch = new ObservationImportBatch { Id = row.GetValue<Guid>("ImportBatch.ID"), Code = row.GetValue<int>("ImportBatch.Code"), Date = new EpochDate(row.GetValue<DateTime>("ImportBatch.Date")) },
                            Site = new ObservationSite { Id = row.GetValue<Guid>("Site.ID"), Code = row.GetValue<string>("Site.Code"), Name = row.GetValue<string>("Site.Name") },
                            Station = new ObservationStation { Id = row.GetValue<Guid>("Station.ID"), Code = row.GetValue<string>("Station.Code"), Name = row.GetValue<string>("Station.Name") },
                            Instrument = new ObservationInstrument { Id = row.GetValue<Guid>("Instrument.ID"), Code = row.GetValue<string>("Instrument.Code"), Name = row.GetValue<string>("Instrument.Name") },
                            Sensor = new ObservationSensor { Id = row.GetValue<Guid>("Sensor.ID"), Code = row.GetValue<string>("Sensor.Code"), Name = row.GetValue<string>("Sensor.Name") },
                            Phenomenon = new ObservationPhenomenon { Id = row.GetValue<Guid>("Phenomenon.ID"), Code = row.GetValue<string>("Phenomenon.Code"), Name = row.GetValue<string>("Phenomenon.Name") },
                            Offering = new ObservationOffering { Id = row.GetValue<Guid>("Offering.ID"), Code = row.GetValue<string>("Offering.Code"), Name = row.GetValue<string>("Offering.Name") },
                            Unit = new ObservationUnit { Id = row.GetValue<Guid>("Unit.ID"), Code = row.GetValue<string>("Unit.Code"), Name = row.GetValue<string>("Unit.Name") },
                            Status = row.IsNull("Status.ID") ? null : new ObservationStatus { Id = row.GetValue<Guid>("Status.ID"), Code = row.GetValue<string>("Status.Code"), Name = row.GetValue<string>("Status.Name") },
                            StatusReason = row.IsNull("StatusReason.ID") ? null : new ObservationStatusReason { Id = row.GetValue<Guid>("StatusReason.ID"), Code = row.GetValue<string>("StatusReason.Code"), Name = row.GetValue<string>("StatusReason.Name") },
                        };
                        //SAEONLogs.Verbose("Adding {@Observation}", observation);
                        items.Add(item);
                        if (items.Count >= batchSize)
                        {
                            var batchCost = azure.UpsertObservations(items);
                            cost += batchCost;
                            SAEONLogs.Information("Added batch of {BatchSize:N0} for ImportBatch {ImportBatchId} Cost: {BatchCost}", items.Count, importBatchId, batchCost);
                            items.Clear();
                        }
                    }
                    if (items.Any())
                    {
                        var batchCost = azure.UpsertObservations(items);
                        cost += batchCost;
                        SAEONLogs.Information("Added batch of {BatchSize:N0} for ImportBatch {ImportBatchId} Cost: {BatchCost}", items.Count, importBatchId, batchCost);
                    }
                    SAEONLogs.Information("Added {Items:N0} CosmosDB items for ImportBatch {ImportBatchId} in {elapsed}, Cost: {Cost}", table.Rows.Count, importBatchId, stopwatch.Elapsed.TimeStr(), cost);
                }
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                try
                {
                    azure.DeleteImportBatch(importBatchId);
                }
                catch (Exception)
                {
                }
                throw;
            }
        }
    }

    private void DeleteCosmosDBItems(Guid importBatchId)
    {
        if (!(ObservationsAzure.Enabled && ObservationsAzure.CosmosDBEnabled)) return;
        using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "ImportBatchID", importBatchId } }))
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var azure = new ObservationsAzure();
            SAEONLogs.Information("Deleting CosmosDB items for ImportBatch {importBatchId} BulkEnabled: {BulkEnabled}", importBatchId, ObservationsAzure.CosmosDBBulkEnabled);
            var (totalCost, readCost, deleteCost) = azure.DeleteImportBatch(importBatchId);
            SAEONLogs.Information("Deleted CosmosDB items for ImportBatch {importBatchId} in {elapsed}, ReadCost: {ReadCost} DeleteCost: {DeleteCost} TotalCost: {TotalCost}", importBatchId, stopwatch.Elapsed.TimeStr(), readCost, deleteCost, totalCost);
        }
    }
#endif

    //public void FileSelected(object sender, DirectEventArgs e)
    //{
    //    using (SAEONLogs.MethodCall(GetType()))
    //    {
    //        SAEONLogs.Information("Import file selected");
    //    }
    //}

#if IsTest
    [DirectMethod]
    public void UploadSAEONLogs(string msg)
    {
        using (SAEONLogs.MethodCall(GetType()))
        {
            SAEONLogs.Information(msg);
        }
    }

    protected void LogStartClick(object sender, DirectEventArgs e)
    {
        using (SAEONLogs.MethodCall(GetType()))
        {
            try
            {
                SAEONLogs.Information("LogStart");
                Session["TestStart"] = DateTime.Now;
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                throw;
            }
        }
    }

    protected void TestUploadClick(object sender, DirectEventArgs e)
    {
        using (SAEONLogs.MethodCall(GetType()))
        {
            if (!DataFileUpload.HasFile)
            {
                MessageBoxes.Error("Error", "No file uploaded. Make sure you click on Log Start first.");
                return;
            }
            try
            {
                var elapsed = DateTime.Now - (DateTime)Session["TestStart"];
                SAEONLogs.Information("HasFile: {HasFile} FileName: {FileName} FileSize: {FileSize:n0} Time: {Time}", DataFileUpload.HasFile, DataFileUpload?.PostedFile.FileName, DataFileUpload?.PostedFile.ContentLength, elapsed.TimeStr());
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                throw;
            }
        }
    }
#endif

    protected void UploadClick(object sender, DirectEventArgs e)
    {
        using (SAEONLogs.MethodCall(GetType()))
        {
            try
            {
                if (!DataFileUpload.HasFile)
                {
                    MessageBoxes.Error("Error", "Error uploading file!");
                    return;
                }
#if IsTest
                SAEONLogs.Information("HasFile: {HasFile} FileName: {FileName} FileSize: {FileSize:n0} Time: {Time}", DataFileUpload.HasFile, DataFileUpload?.PostedFile.FileName, DataFileUpload?.PostedFile.ContentLength, (DateTime.Now - (DateTime)Session["TestStart"]).TimeStr());
#endif

                SAEONLogs.Information("BulkInsert: {BulkInsert} BulkInsertBatchSize: {BulkInsertBatchSize}", BulkInsert, BulkInsertBatchSize);

                Guid DataSourceId = new Guid(cbDataSource.SelectedItem.Value);
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
                SAEONLogs.Information("Import Version: {version:F2} DataSource: {dataSource} FileName: {fileName}", 1.60, batch.DataSource.Name, batch.FileName);
                List<SchemaValue> values = Import(DataSourceId, batch);
                stageStopwatch.Stop();
                SAEONLogs.Information("Imported {count:N0} observations in {elapsed}", values.Count, stageStopwatch.Elapsed.TimeStr());

                if (!values.Any())
                {
                    MessageBoxes.Warning("Warning", "No observations have been imported.");
                }
                else
                {
                    SAEONLogs.Information("Saving {count:N0} observations", values.Count);
                    var lastProgress100 = -1;
                    var n = 1;
                    var nMax = 0;
                    var goodValues = values.Where(i => i.IsValid).OrderBy(i => i.RowNum).ToList();
                    var badValues = values.Where(i => !i.IsValid).OrderBy(i => i.RowNum).ToList();
                    var dtGoodValues = new DataTable("Observation");
                    var dtBadValues = new DataTable("DataLog");
                    var userId = AuthHelper.GetLoggedInUserId;
                    if (BulkInsert)
                    {
                        if (badValues.Any())
                        {
                            // Create DataTable from bad values
                            stageStopwatch.Restart();
                            SAEONLogs.Information("Creating DataTable for {count:n0} bad observations", badValues.Count);
                            dtBadValues.Columns.Add("ImportBatchID", typeof(Guid));
                            dtBadValues.Columns.Add("SensorID", typeof(Guid));
                            dtBadValues.Columns.Add("ImportDate", typeof(DateTime));
                            dtBadValues.Columns.Add("ValueDate", typeof(DateTime));
                            dtBadValues.Columns.Add("ValueTime", typeof(DateTime));
                            dtBadValues.Columns.Add("ValueText", typeof(string));
                            dtBadValues.Columns.Add("TransformValueText", typeof(string));
                            dtBadValues.Columns.Add("RawValue", typeof(double));
                            dtBadValues.Columns.Add("DataValue", typeof(double));
                            dtBadValues.Columns.Add("PhenomenonOfferingID", typeof(Guid));
                            dtBadValues.Columns.Add("PhenomenonUOMID", typeof(Guid));
                            dtBadValues.Columns.Add("Latitude", typeof(double));
                            dtBadValues.Columns.Add("Longitude", typeof(double));
                            dtBadValues.Columns.Add("Elevation", typeof(double));
                            dtBadValues.Columns.Add("Comment", typeof(string));
                            dtBadValues.Columns.Add("CorrelationID", typeof(Guid));
                            dtBadValues.Columns.Add("InvalidDateValue", typeof(string));
                            dtBadValues.Columns.Add("InvalidTimeValue", typeof(string));
                            dtBadValues.Columns.Add("InvalidOffering", typeof(string));
                            dtBadValues.Columns.Add("InvalidUOM", typeof(string));
                            dtBadValues.Columns.Add("DataSourceTransformationID", typeof(Guid));
                            dtBadValues.Columns.Add("StatusID", typeof(Guid));
                            //dtBadValues.Columns.Add("StatusReasonID", typeof(Guid));
                            dtBadValues.Columns.Add("ImportStatus", typeof(string));
                            //dtBadValues.Columns.Add("RawRecordData", typeof(string));
                            dtBadValues.Columns.Add("RawFieldValue", typeof(string));
                            dtBadValues.Columns.Add("UserId", typeof(Guid));
                            nMax = badValues.Count;
                            n = 1;
                            if (UseParallel)
                                try
                                {
                                    Parallel.For(1, nMax, i =>
                                    {
                                        lock (dtBadValues)
                                        {
                                            AddBadValue(dtBadValues, badValues[i - 1], batch, userId);
                                        }
                                    });
                                    n = nMax;
                                }
                                catch (Exception ex)
                                {
                                    SAEONLogs.Exception(ex, "Unable to create dtBadValues");
                                    throw;
                                }
                            else
                                foreach (var value in badValues)
                                {
                                    var progress = (double)n / nMax;
                                    var progress100 = (int)(progress * 100);
                                    var reportPorgress = (progress100 % 5 == 0) && (progress100 > 0) && (lastProgress100 != progress100);
                                    var elapsed = stageStopwatch.Elapsed;
                                    var total = TimeSpan.FromSeconds(elapsed.TotalSeconds / progress);
                                    if (reportPorgress)
                                    {
                                        SAEONLogs.Information("{progress:p0} {value:n0} of {values:n0} bad observations in {min} of {mins}, {rowTime}/row, {rowsPerSec:n3} rows/sec", progress, n, nMax, elapsed.TimeStr(), total.TimeStr(), TimeSpan.FromSeconds(elapsed.TotalSeconds / n).TimeStr(), n / elapsed.TotalSeconds);
                                        lastProgress100 = progress100;
                                    }
                                    try
                                    {
                                        var dataRow = AddBadValue(dtBadValues, value, batch, userId);
                                        if (reportPorgress)
                                        {
                                            SAEONLogs.Verbose("DataRow: {row}", dataRow.AsString());
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        SAEONLogs.Exception(ex, "Unable to add DataRow: {row}", value.RowNum);
                                        throw;
                                    }
                                    n++;
                                }
                            stageStopwatch.Stop();
                            SAEONLogs.Information("Created DataTable {count:n0} bad observations in {elapsed}, {rowTime}/row, {rowsPerSec:n3} rows/sec", nMax, stageStopwatch.Elapsed.TimeStr(), TimeSpan.FromSeconds(stageStopwatch.Elapsed.TotalSeconds / nMax).TimeStr(), nMax / stageStopwatch.Elapsed.TotalSeconds);
                            //SAEONLogs.Verbose("BadValues: {BadValues}", dtBadValues.Dump());
                        }
                        if (goodValues.Any())
                        {
                            // Create DataTable from good values
                            stageStopwatch.Restart();
                            SAEONLogs.Information("Creating DataTable for {count:n0} good observations", goodValues.Count);
                            dtGoodValues.Columns.Add("ImportBatchID", typeof(Guid));
                            dtGoodValues.Columns.Add("SensorID", typeof(Guid));
                            dtGoodValues.Columns.Add("ValueDate", typeof(DateTime));
                            dtGoodValues.Columns.Add("TextValue", typeof(string));
                            dtGoodValues.Columns.Add("RawValue", typeof(double));
                            dtGoodValues.Columns.Add("DataValue", typeof(double));
                            dtGoodValues.Columns.Add("PhenomenonOfferingID", typeof(Guid));
                            dtGoodValues.Columns.Add("PhenomenonUOMID", typeof(Guid));
                            dtGoodValues.Columns.Add("Latitude", typeof(double));
                            dtGoodValues.Columns.Add("Longitude", typeof(double));
                            dtGoodValues.Columns.Add("Elevation", typeof(double));
                            dtGoodValues.Columns.Add("Comment", typeof(string));
                            dtGoodValues.Columns.Add("CorrelationID", typeof(Guid));
                            //dtGoodValues.Columns.Add("StatusID", typeof(Guid));
                            //dtGoodValues.Columns.Add("StatusReasonID", typeof(Guid));
                            dtGoodValues.Columns.Add("AddedDate", typeof(DateTime));
                            dtGoodValues.Columns.Add("UserId", typeof(Guid));
                            nMax = goodValues.Count;
                            n = 1;
                            if (UseParallel)
                                try
                                {
                                    Parallel.For(1, nMax, i =>
                                    {
                                        lock (dtGoodValues)
                                        {
                                            AddGoodValue(dtGoodValues, goodValues[i - 1], batch, userId);
                                        }
                                    });
                                    n = nMax;
                                }
                                catch (Exception ex)
                                {
                                    SAEONLogs.Exception(ex, "Unable to create dtGoodValues");
                                    throw;
                                }
                            else
                                foreach (var value in goodValues)
                                {
                                    var progress = (double)n / nMax;
                                    var progress100 = (int)(progress * 100);
                                    var reportPorgress = (progress100 % 5 == 0) && (progress100 > 0) && (lastProgress100 != progress100);
                                    var elapsed = stageStopwatch.Elapsed;
                                    var total = TimeSpan.FromSeconds(elapsed.TotalSeconds / progress);
                                    if (reportPorgress)
                                    {
                                        SAEONLogs.Information("{progress:p0} {value:n0} of {values:n0} good observations in {min} of {mins}, {rowTime}/row, {rowsPerSec:n3} rows/sec", progress, n, nMax, elapsed.TimeStr(), total.TimeStr(), TimeSpan.FromSeconds(elapsed.TotalSeconds / n).TimeStr(), n / elapsed.TotalSeconds);
                                        lastProgress100 = progress100;
                                    }
                                    try
                                    {
                                        var dataRow = AddGoodValue(dtGoodValues, value, batch, userId);
                                        if (reportPorgress)
                                        {
                                            SAEONLogs.Verbose("DataRow: {row}", dataRow.AsString());
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        SAEONLogs.Exception(ex, "Unable to add DataRow: {row}", value.RowNum);
                                        throw;
                                    }
                                    n++;
                                }
                            stageStopwatch.Stop();
                            SAEONLogs.Information("Created DataTable {count:n0} good observations in {elapsed}, {rowTime}/row, {rowsPerSec:n3} rows/sec", nMax, stageStopwatch.Elapsed.TimeStr(), TimeSpan.FromSeconds(stageStopwatch.Elapsed.TotalSeconds / nMax).TimeStr(), nMax / stageStopwatch.Elapsed.TotalSeconds);
                            //SAEONLogs.Verbose("GoodValues: {GoodValues}", dtGoodValues.Dump());
                        }
                    }
                    try
                    {
                        using (TransactionScope tranScope = Utilities.NewTransactionScope())
                        {
                            using (SharedDbConnectionScope connScope = new SharedDbConnectionScope())
                            {
                                if (!values.Any(t => t.IsValid))
                                {
                                    SAEONLogs.Verbose("Error: IsValid: {count:N0}", values.Where(t => !t.IsValid).Count());
                                    batch.Status = (int)ImportBatchStatus.DatalogWithErrors;
                                }
                                else
                                {
                                    batch.Status = (int)ImportBatchStatus.NoLogErrors;
                                }

                                batch.UserId = AuthHelper.GetLoggedInUserId;
                                batch.Save();

                                if (badValues.Any())
                                {
                                    if (batch.Status != (int)ImportBatchStatus.DatalogWithErrors)
                                    {
                                        batch.Status = (int)ImportBatchStatus.DatalogWithErrors;
                                        batch.Save();
                                    }

                                    SAEONLogs.Information("Saving {count:n0} bad observations", badValues.Count);
                                    stageStopwatch.Restart();
                                    if (BulkInsert)
                                    {
                                        // Bulk insert
                                        SAEONLogs.Information("Starting bad observations bulk inserts");
                                        nMax = badValues.Count;
                                        using (var bulkInsert = new SqlBulkCopy((SqlConnection)connScope.CurrentConnection, SqlBulkCopyOptions.CheckConstraints | SqlBulkCopyOptions.FireTriggers, null))
                                        {
                                            bulkInsert.BatchSize = BulkInsertBatchSize;
                                            bulkInsert.BulkCopyTimeout = 15 * 60 * 60;
                                            bulkInsert.NotifyAfter = bulkInsert.BatchSize;
                                            bulkInsert.SqlRowsCopied += BulkInsert_SqlRowsCopied;
                                            bulkInsert.DestinationTableName = "DataLog";
                                            foreach (DataColumn col in dtBadValues.Columns)
                                            {
                                                bulkInsert.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                                            }
                                            bulkInsert.WriteToServer(dtBadValues);
                                        }
                                        stageStopwatch.Stop();
                                        //SAEONLogs.Information("Elapsed: {elapsed}", stageStopwatch.Elapsed.TimeStr());
                                        SAEONLogs.Information("Bulk inserted {count:n0} bad observations in {elapsed}, {rowTime}/row, {rowsPerSec:n3} rows/sec", nMax, stageStopwatch.Elapsed.TimeStr(), TimeSpan.FromSeconds(stageStopwatch.Elapsed.TotalSeconds / nMax).TimeStr(), nMax / stageStopwatch.Elapsed.TotalSeconds);
                                    }
                                    else
                                    {
                                        SAEONLogs.Information("Starting bad observations inserts");
                                        lastProgress100 = -1;
                                        nMax = badValues.Count;
                                        n = 1;
                                        foreach (var value in badValues)
                                        {
                                            var progress = (double)n / nMax;
                                            var progress100 = (int)(progress * 100);
                                            var reportPorgress = (progress100 % 5 == 0) && (progress100 > 0) && (lastProgress100 != progress100);
                                            var elapsed = stageStopwatch.Elapsed;
                                            var total = TimeSpan.FromSeconds(elapsed.TotalSeconds / progress);
                                            if (reportPorgress)
                                            {
                                                SAEONLogs.Information("{progress:p0} {value:n0} of {values:n0} bad observations in {min} of {mins}", progress, n, nMax, elapsed.TimeStr(), total.TimeStr());
                                                lastProgress100 = progress100;
                                            }

                                            if (LogBadValues)
                                            {
                                                SAEONLogs.Error("Bad Value: {@value}", value);
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
                                            else if (value.TimeValue.HasValue && value.TimeValue != DateTime.MinValue)
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

                                            if (value.DataSourceTransformationID.HasValue)
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
                                            //SAEONLogs.Verbose("BatchID: {id} Status: {status} ImportStatus: {importStatus}", batch.Id, logrecord.StatusID, logrecord.ImportStatus);
                                            try
                                            {
                                                logrecord.Save();
                                            }
                                            catch (Exception ex)
                                            {
                                                SAEONLogs.Exception(ex, "Unable to create ErroLog: {rowNum}", value.RowNum);
                                                throw;
                                            }
                                            n++;
                                        }

                                        stageStopwatch.Stop();
                                        SAEONLogs.Information("Inserted {count:N0} bad observations in {time}", nMax, stageStopwatch.Elapsed.TimeStr());
                                    }
                                }
                                if (goodValues.Any())
                                {
                                    SAEONLogs.Information("Saving {good} good observations", goodValues.Count);
                                    stageStopwatch.Restart();
                                    if (BulkInsert)
                                    {
                                        // Bulk insert
                                        SAEONLogs.Information("Starting good observations bulk inserts");
                                        nMax = goodValues.Count;
                                        using (var bulkInsert = new SqlBulkCopy((SqlConnection)connScope.CurrentConnection, SqlBulkCopyOptions.CheckConstraints | SqlBulkCopyOptions.FireTriggers, null))
                                        {
                                            bulkInsert.BatchSize = BulkInsertBatchSize;
                                            bulkInsert.BulkCopyTimeout = 15 * 60 * 60;
                                            bulkInsert.NotifyAfter = bulkInsert.BatchSize;
                                            bulkInsert.SqlRowsCopied += BulkInsert_SqlRowsCopied;
                                            bulkInsert.DestinationTableName = "Observation";
                                            foreach (DataColumn col in dtGoodValues.Columns)
                                            {
                                                bulkInsert.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                                            }
                                            bulkInsert.WriteToServer(dtGoodValues);
                                        }
                                        stageStopwatch.Stop();
                                        //SAEONLogs.Information("Elapsed: {elapsed}", stageStopwatch.Elapsed.TimeStr());
                                        SAEONLogs.Information("Bulk inserted {count:n0} good observations in {elapsed}, {rowTime}/row, {rowsPerSec:n3} rows/sec", nMax, stageStopwatch.Elapsed.TimeStr(), TimeSpan.FromSeconds(stageStopwatch.Elapsed.TotalSeconds / nMax).TimeStr(), nMax / stageStopwatch.Elapsed.TotalSeconds);
                                    }
                                    else
                                    {
                                        SAEONLogs.Information("Starting good observations inserts");
                                        lastProgress100 = -1;
                                        nMax = goodValues.Count;
                                        n = 1;
                                        foreach (var schval in goodValues)
                                        {
                                            var progress = (double)n / nMax;
                                            var progress100 = (int)(progress * 100);
                                            var reportPorgress = (progress100 % 5 == 0) && (progress100 > 0) && (lastProgress100 != progress100);
                                            var elapsed = stageStopwatch.Elapsed;
                                            var total = TimeSpan.FromSeconds(elapsed.TotalSeconds / progress);
                                            if (reportPorgress)
                                            {
                                                SAEONLogs.Information("{progress:p0} {value:n0} of {values:n0} good observations in {min} of {mins}, {rowTime}/row, {rowsPerSec:n3} rows/sec", progress, n, nMax, elapsed.TimeStr(), total.TimeStr(), TimeSpan.FromSeconds(elapsed.TotalSeconds / n).TimeStr(), n / elapsed.TotalSeconds);
                                                lastProgress100 = progress100;
                                            }
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
                                                {
                                                    Obrecord.Comment = null;
                                                }
                                                else
                                                {
                                                    Obrecord.Comment = schval.Comment;
                                                }

                                                if (string.IsNullOrWhiteSpace(schval.TextValue))
                                                {
                                                    Obrecord.TextValue = null;
                                                }
                                                else
                                                {
                                                    Obrecord.TextValue = schval.TextValue;
                                                }

                                                Obrecord.Save();
                                            }
                                            catch (SqlException ex) when (ex.Number == 2627)
                                            {
                                                SAEONLogs.Exception(ex, "Duplicate! Number: {num} Value: {@value}", ex.Number, schval);
                                                throw;
                                            }
                                            catch (SqlException ex)
                                            {
                                                SAEONLogs.Exception(ex, "Number: {num} Value: {@value}", ex.Number, schval);
                                                throw;
                                            }
                                            catch (Exception ex)
                                            {
                                                SAEONLogs.Exception(ex);
                                                throw;
                                            }
                                            n++;
                                        }
                                        stageStopwatch.Stop();
                                        SAEONLogs.Information("Inserted {count:n0} good observations in {elapsed}, {rowTime}/row, {rowsPerSec:n3} rows/sec", nMax, stageStopwatch.Elapsed.TimeStr(), TimeSpan.FromSeconds(stageStopwatch.Elapsed.TotalSeconds / nMax).TimeStr(), nMax / stageStopwatch.Elapsed.TotalSeconds);
                                    }
                                }
                                // Summaries
                                try
                                {
                                    CreateSummary(connScope, batch.Id);
                                }
                                catch (Exception ex)
                                {
                                    SAEONLogs.Exception(ex);
                                    throw;
                                }

                                // Documents
#if UseCosmosDb
                                CreateCosmosDBItems(connScope, batch.Id);
#endif
                                Auditing.Log(GetType(), new MethodCallParameters { { "ID", batch.Id }, { "Code", batch.Code }, { "Status", batch.Status } });
                                batch.DurationInSecs = (int)durationStopwatch.Elapsed.TotalSeconds;
                                batch.Save();
                            }
                            tranScope.Complete();
                            SAEONLogs.Information("Import: {count:N0} observations, {good:N0} good {bad:N0} bad, summary and documents in {duration}", values.Count, goodValues.Count, badValues.Count, durationStopwatch.Elapsed.TimeStr());
#if IsTest
                            SAEONLogs.Information("HasFile: {HasFile} FileName: {FileName} FileSize: {FileSize:n0} Time: {Time}", DataFileUpload.HasFile, DataFileUpload?.PostedFile.FileName, DataFileUpload?.PostedFile.ContentLength, (DateTime.Now - (DateTime)Session["TestStart"]).TimeStr());
#endif
                        }
                    }
                    catch (Exception ex)
                    {
                        SAEONLogs.Exception(ex, "Unable to save {count:N0} observations in {duration}", values.Count, durationStopwatch.Elapsed.TimeStr());
                        throw;
                    }
                    ObservationsGridStore.DataBind();
                    ImportBatchesGrid.GetStore().DataBind();
                    SummaryGridStore.DataBind();
                    DataLogGrid.GetStore().DataBind();
                    ImportWindow.Hide();
                }
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                List<object> errors = new List<object>
                {
                    new { ErrorMessage = ex.Message, LineNo = 1, RecordString = "" }
                };
                ErrorGridStore.DataSource = errors;
                ErrorGrid.DataBind();
                MessageBoxes.Error("Error", $"An error occurred while importing - {ex.Message}");
            }
            finally
            {
                X.Mask.Hide();
            }
        }

        DataRow AddBadValue(DataTable dtBadValues, SchemaValue value, ImportBatch batch, Guid userId)
        {
            if (LogBadValues)
            {
                SAEONLogs.Error("Bad Value: {@value}", value);
            }
            var dataRow = dtBadValues.NewRow();
            dataRow.SetValue<Guid>("ImportBatchID", batch.Id);
            dataRow.SetValue<Guid?>("SensorID", value.SensorID);
            if (value.DateValueInvalid)
                dataRow.SetValue<string>("InvalidDateValue", value.InvalidDateValue);
            else if (value.DateValue > DateTime.MinValue)
                dataRow.SetValue<DateTime>("ValueDate", value.DateValue);
            if (value.TimeValueInvalid)
                dataRow.SetValue<string>("InvalidTimeValue", value.InvalidTimeValue);
            else if (value.TimeValue.HasValue && value.TimeValue.Value > DateTime.MinValue)
                dataRow.SetValue<DateTime>("ValueTime", value.TimeValue.Value);
            if (value.RawValueInvalid)
                dataRow.SetValue<string>("ValueText", value.InvalidRawValue);
            else
            {
                dataRow.SetValue<string>("ValueText", value.FieldRawValue);
                dataRow.SetValue<double?>("RawValue", value.RawValue);
            }
            if (value.DataValueInvalid)
                dataRow.SetValue<string>("TransformValueText", value.InvalidRawValue);
            else
                dataRow.SetValue<double?>("DataValue", value.DataValue);
            if (value.InvalidOffering)
                dataRow.SetValue<string>("InvalidOffering", value.PhenomenonOfferingID.Value.ToString());
            else
                dataRow.SetValue<Guid>("PhenomenonOfferingID", value.PhenomenonOfferingID.Value);
            if (value.InvalidUOM)
                dataRow.SetValue<string>("InvalidUOM", value.PhenomenonUOMID.Value.ToString());
            else
                dataRow.SetValue<Guid>("PhenomenonUOMID", value.PhenomenonUOMID.Value);
            dataRow.SetValue<double?>("Latitude", value.Latitude);
            dataRow.SetValue<double?>("Longitude", value.Longitude);
            dataRow.SetValue<double?>("Elevation", value.Elevation);
            dataRow.SetValue<Guid>("CorrelationID", value.CorrelationID);
            dataRow.SetValue<string>("Comment", value.Comment);
            dataRow.SetValue<Guid>("UserId", userId);
            dataRow.SetValue<DateTime>("ImportDate", DateTime.Now);
            dataRow.SetValue<string>("RawFieldValue", String.IsNullOrWhiteSpace(value.FieldRawValue) ? "" : value.FieldRawValue);
            if (value.DataSourceTransformationID.HasValue)
                dataRow.SetValue<Guid>("DataSourceTransformationID", value.DataSourceTransformationID.Value);
            dataRow.SetValue<string>("ImportStatus", String.Join(",", value.InvalidStatuses.Select(s => new Status(s).Name)));
            dataRow.SetValue<Guid>("StatusID", new Guid(value.InvalidStatuses.First()));
            //dataRow.SetValue<>("", value.);
            dtBadValues.Rows.Add(dataRow);
            return dataRow;
        }

        DataRow AddGoodValue(DataTable dtGoodValues, SchemaValue value, ImportBatch batch, Guid userId)
        {
            var dataRow = dtGoodValues.NewRow();
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
            dataRow.SetValue<Guid>("UserId", userId);
            dataRow.SetValue<DateTime>("AddedDate", DateTime.Now);
            //dataRow.SetValue<>("", value.);
            dtGoodValues.Rows.Add(dataRow);
            return dataRow;
        }
    }

    private void BulkInsert_SqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
    {
        SAEONLogs.Information("Batch: {rows:n0}", e.RowsCopied);
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
        using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "ImportBatch", batch.Code } }))
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
                        ImportSchemaHelper helper = new ImportSchemaHelper(ds, schema, Data, batch, sp);
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

                    ImportSchemaHelper helper = new ImportSchemaHelper(ds, schema, Data, batch, null);
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
        using (SAEONLogs.MethodCall(GetType()))
        {
            try
            {
                RowSelectionModel batchRow = ImportBatchesGrid.SelectionModel.Primary as RowSelectionModel;
                ImportBatch batch = new ImportBatch(batchRow.SelectedRecordID);

                //DataLog log = new DataLog();
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                SAEONLogs.Information("Moving from DataLog to Observation");
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
                    obs.RawValue = Utilities.ParseDouble(RawValue.Value.ToString());
                    obs.DataValue = Utilities.ParseDouble(DataValue.Value.ToString());

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
                            SAEONLogs.Information("Moved from DataLog to Observation in {time}", stopwatch.Elapsed.TimeStr());
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
                    SAEONLogs.Exception(ex, "Unable to move from DataLog to Observation in {time}", stopwatch.Elapsed.TimeStr());
                    throw;
                }
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
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
                Handler = $"Ext.getBody().mask('Deleting batch...'); DirectCall.DeleteBatch('{ImportBatchId}');",
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
        using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "ImportBatchID", importBatchId } }))
        {
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                try
                {
                    SAEONLogs.Information("Deleting ImportBatch {ImportBatchID}", importBatchId);
                    using (TransactionScope ts = Utilities.NewTransactionScope())
                    {
                        using (SharedDbConnectionScope connScope = new SharedDbConnectionScope())
                        {
                            DataLog.Delete(DataLog.Columns.ImportBatchID, importBatchId);
                            DeleteObservations(connScope, importBatchId);
                            var t = stopwatch.Elapsed;
                            SAEONLogs.Information("Deleting summaries for ImportBatch {ImportBatchID}", importBatchId);
                            ImportBatchSummary.Delete(ImportBatchSummary.Columns.ImportBatchID, importBatchId);
                            t = stopwatch.Elapsed;
                            SAEONLogs.Information("Deleted summaries for ImportBatch {ImportBatchID} in {Elapsed}", importBatchId, (stopwatch.Elapsed - t).TimeStr());
                            ImportBatch.Delete(importBatchId);
#if UseCosmosDb
                            DeleteCosmosDBItems(importBatchId);
#endif
                        }
                        ts.Complete();
                        stopwatch.Stop();
                        SAEONLogs.Information("Deleted ImportBatch {ImportBatchID} in {time}", importBatchId, stopwatch.Elapsed.TimeStr());
                    }
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    SAEONLogs.Exception(ex, "Unable to delete ImportBatch {ImportBatchID} in {time}", importBatchId, stopwatch.Elapsed.TimeStr());
                }

                ImportBatchesGridStore.DataBind();
                DataLogGridStore.DataBind();
                SummaryGridStore.DataBind();
                ObservationsGridStore.DataBind();
            }

            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                throw;
            }
            finally
            {
                X.Mask.Hide();
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
        using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "ImportBatchID", ImportBatchId } }))
        {
            try
            {
                ObservationCollection col = new ObservationCollection().Where(Observation.Columns.ImportBatchID, ImportBatchId).Load();
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                try
                {
                    SAEONLogs.Information("Moving import batch to DataLog {Id}", ImportBatchId);
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
                        SAEONLogs.Information("Moved import batch to DataLog {Id} in {time}", ImportBatchId, stopwatch.Elapsed.TimeStr());
                    }
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    SAEONLogs.Exception(ex, "Unable to moved import batch to DataLog {Id} in {time}", ImportBatchId, stopwatch.Elapsed.TimeStr());
                }

                ImportBatchesGridStore.DataBind();
                DataLogGridStore.DataBind();
                SummaryGridStore.DataBind();
                ObservationsGridStore.DataBind();
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                throw;
            }
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
        using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "ID", Id } }))
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
                SAEONLogs.Exception(ex, "Unable to delete entry");
                throw;
            }
        }
    }

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
        using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "Id", Id } }))
        {
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                try
                {
                    SAEONLogs.Information("Moving from DataLog {Id}", Id);
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
                        SAEONLogs.Information("Moved from DataLog {Id} in {time}", Id, stopwatch.Elapsed.TimeStr());
                    }

                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    SAEONLogs.Exception(ex, "Unable to moved from DataLog {Id} in {time}", Id, stopwatch.Elapsed.TimeStr());
                }

                ImportBatchesGridStore.DataBind();
                DataLogGridStore.DataBind();
                SummaryGridStore.DataBind();
                ObservationsGridStore.DataBind();
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                throw;
            }
        }
    }

    #endregion ImportBatches

    #region Observations

    protected void ObservationsGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        using (SAEONLogs.MethodCall(GetType()))
        {
            try
            {
                SAEONLogs.Verbose("ObservationsGridStore_RefreshData Start");
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
                        //SAEONLogs.Information("List: {@list}", ObservationRepository.GetPagedListByBatch(e, e.Parameters[GridFiltersObservations.ParamPrefix], Id));
                        ObservationsGridStore.DataSource = ObservationRepository.GetPagedListByBatch(e, e.Parameters[GridFiltersObservations.ParamPrefix], Id);
                        ObservationsGridStore.DataBind();
                        EnableButtons();
                    }
                    catch (Exception ex)
                    {
                        SAEONLogs.Exception(ex);
                        MessageBoxes.Error(ex, "Error", "Unable to refresh Observations grid");
                    }
                }
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                throw;
            }
            finally
            {
                SAEONLogs.Verbose("ObservationsGridStore_RefreshData End");
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
        using (SAEONLogs.MethodCall(GetType()))
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
                SAEONLogs.Exception(ex, "Unable to set status and reason to the selected observations");
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
        using (SAEONLogs.MethodCall(GetType()))
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
                        SAEONLogs.Information("SetWithOut in {time}", stopwatch.Elapsed.TimeStr());
                    }

                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    SAEONLogs.Exception(ex, "Unable to SetWithOut in {time}", stopwatch.Elapsed.TimeStr());
                    throw;
                }
                var sm = ObservationsGrid.SelectionModel.Primary as CheckboxSelectionModel;
                sm.ClearSelections();
                ObservationsGrid.DataBind();
                EnableButtons();
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                MessageBoxes.Error(ex, "Error", "Unable to set status and reason to the observations without status");
            }
        }
    }

    protected void SetTestClick(object sender, DirectEventArgs e)
    {
        var sm = ObservationsGrid.SelectionModel.Primary as CheckboxSelectionModel;
        var count = int.Parse(e.ExtraParams["count"]);

        MessageBoxes.Info("Info", $"Count: {count:n0} Selected: {sm.SelectedRows.Count}");
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
        using (SAEONLogs.MethodCall(GetType()))
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
                        SAEONLogs.Information("SetAll in {time}", stopwatch.Elapsed.TimeStr());
                    }
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    SAEONLogs.Exception(ex, "Unable to SetAll in {time}", stopwatch.Elapsed.TimeStr());
                    throw;
                }
                var sm = ObservationsGrid.SelectionModel.Primary as CheckboxSelectionModel;
                sm.ClearSelections();
                ObservationsGrid.DataBind();
                EnableButtons();
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
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
        using (SAEONLogs.MethodCall(GetType()))
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
                SAEONLogs.Exception(ex, "Unable to clear status and reason on the selected observations");
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
        using (SAEONLogs.MethodCall(GetType()))
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
                        SAEONLogs.Information("ClearAll in {time}", stopwatch.Elapsed.TimeStr());
                    }
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    SAEONLogs.Exception(ex, "Unable to ClearAll in {time}", stopwatch.Elapsed.TimeStr());
                    throw;
                }
                var sm = ObservationsGrid.SelectionModel.Primary as CheckboxSelectionModel;
                sm.ClearSelections();
                ObservationsGrid.DataBind();
                EnableButtons();
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
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
        using (SAEONLogs.MethodCall(GetType()))
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
                SAEONLogs.Exception(ex);
                throw;
            }
        }
    }

    #endregion Summary
}