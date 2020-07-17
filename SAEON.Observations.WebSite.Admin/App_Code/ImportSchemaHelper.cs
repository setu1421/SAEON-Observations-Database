//#define DetailedLogging
//#define VeryDetailedLogging
#define UseParallel
using FileHelpers;
using FileHelpers.Dynamic;
using NCalc;
using SAEON.Core;
using SAEON.Logs;
using SAEON.Observations.Azure;
using SAEON.Observations.Data;
using Serilog.Events;
using SubSonic;
using System;
#if UseParallel
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
#if UseParallel
using System.Threading.Tasks;
#endif
using System.Web.Hosting;

public class SchemaDefinition
{
    public int Index { get; set; }
    public string FieldName { get; set; }
    public bool IsDate { get; set; }
    public string Dateformat { get; set; }
    public bool IsTime { get; set; }
    public string Timeformat { get; set; }
    public bool IsFixedTime { get; set; }
    public TimeSpan FixedTimeValue { get; set; }
    public bool IsIgnored { get; set; }
    public Guid? PhenomenonOfferingID { get; set; }
    public PhenomenonOffering PhenomenonOffering { get; set; } = null;
    public bool InValidOffering { get; set; }
    public Guid? PhenomenonUOMID { get; set; }
    public PhenomenonUOM PhenomenonOUM { get; set; } = null;
    public bool InValidUOM { get; set; }
    //public List<Guid> DataSourceTransformationIDs { get; set; } = new List<Guid>();
    public List<DataSourceTransformation> DataSourceTransformations { get; set; } = new List<DataSourceTransformation>();
    public bool IsEmptyValue { get; set; }
    public string EmptyValue { get; set; }
    public bool IsOffering { get; set; }
    public bool IsElevation { get; set; }
    public bool IsLatitude { get; set; }
    public bool IsLongitude { get; set; }

    public bool IsComment { get; set; }

    //public Guid? SensorID { get; set; }
    public List<Sensor> Sensors { get; set; } = new List<Sensor>();
    public List<VSensorDate> SensorDates { get; set; } = new List<VSensorDate>();
    public bool SensorNotFound { get; set; }
}

/// <summary>
///
/// </summary>
public class SchemaValue
{
    public DateTime DateValue { get; set; }
    public DateTime? TimeValue { get; set; }
    public string InvalidDateValue { get; set; }
    public string InvalidTimeValue { get; set; }
    public bool DateValueInvalid { get; set; }
    public bool TimeValueInvalid { get; set; }
    public Guid? PhenomenonOfferingID { get; set; }
    public bool InvalidOffering { get; set; }
    public Guid? PhenomenonUOMID { get; set; }
    public bool InvalidUOM { get; set; }
    public double? RawValue { get; set; }
    public double? DataValue { get; set; }
    public bool RawValueInvalid { get; set; }
    public string InvalidRawValue { get; set; }
    public bool DataValueInvalid { get; set; }
    public string InvalidDataValue { get; set; }
    public List<string> InvalidStatuses { get; private set; } = new List<string>();
    public Guid? DataSourceTransformationID { get; set; }
    public Guid? SensorID { get; set; }
    public bool SensorNotFound { get; set; }
    public string FieldRawValue { get; set; }
    public string Comment { get; set; } = null;

    public Guid? RawPhenomenonOfferingID { get; set; }
    public Guid? RawPhenomenonUOMID { get; set; }

    public Guid CorrelationID { get; set; }
    public string TextValue { get; set; } = null;

    public double? Latitude { get; set; } = null;
    public double? Longitude { get; set; } = null;
    public double? Elevation { get; set; } = null;
    public int RowNum { get; set; }

    public bool IsDuplicate { get; set; } = false;
    public bool IsDuplicateOfNull { get; set; } = false;
    public bool IsDuplicateInBatch { get; set; } = false;

    /// <summary>
    ///
    /// </summary>
    public bool IsValid
    {
        get
        {
            return !(DateValueInvalid || TimeValueInvalid || InvalidOffering || InvalidUOM || RawValueInvalid || DataValueInvalid || SensorNotFound || IsDuplicate || IsDuplicateOfNull || IsDuplicateInBatch);
        }
    }
}

/// <summary>
/// Summary description for ImportSchema
/// </summary>
public class ImportSchemaHelper : IDisposable
{
    private bool disposed = false;
    private readonly FileHelperEngine engine;
    private readonly DataTable dtResults;
    private readonly DataSource dataSource;
    private readonly DataSchema dataSchema;
    //private readonly List<DataSourceTransformation> transformations;
    private readonly List<SchemaDefinition> schemaDefs;
#if UseParallel
    public BlockingCollection<SchemaValue> SchemaValues;
#else
    public List<SchemaValue> SchemaValues;
#endif
    private readonly Sensor Sensor = null;

    /// <summary>
    /// Gap Record Helper
    /// </summary>
    //ImportLogHelper LogHelper = null;

    //public SchemaValues

    private bool concatedatetime = false;

    private readonly ObservationsAzure Azure = new ObservationsAzure();

    private readonly bool LogBadValues = false;

    private List<string> LoadColumnNamesDelimited(DataSchema schema, string data)
    {
        List<string> result = new List<string>();
        string[] lines = data.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        int columnNamesLine = schema.IgnoreFirst;
        if (columnNamesLine >= lines.Length)
        {
            throw new IndexOutOfRangeException("Column Names line greater than lines in source file");
        }
        List<string> columnNames = lines[columnNamesLine]
            .Split(new string[] { schema.Delimiter.Replace("\\t", "\t") }, StringSplitOptions.None)
            .Select(i => i.RemoveQuotes())
            .ToList();
        List<string> badColumnNames = columnNames
            .GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(y => y.Key)
            .ToList();
        if (badColumnNames.Any())
        {
            throw new InvalidOperationException("Duplicate column names found " + string.Join(", ", columnNames));
        }
        result.AddRange(columnNames);
        return result;
    }

    //private List<string> LoadColumnNamesFixedWidth(DataSchema schema, string data)
    //{
    //    List<string> result = new List<string>();
    //    string[] lines = data.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
    //    int columnNamesLine = schema.IgnoreFirst;
    //    if (columnNamesLine >= lines.Length)
    //    {
    //        throw new IndexOutOfRangeException("Column Names line greater than lines in source file");
    //    }
    //    List<string> columnNames = lines[columnNamesLine]
    //        .Split(new string[] { schema.Delimiter.Replace("\\t", "\t") }, StringSplitOptions.None)
    //        .Select(i => i.RemoveQuotes())
    //        .ToList();
    //    List<string> badColumnNames = columnNames
    //        .GroupBy(x => x)
    //        .Where(g => g.Count() > 1)
    //        .Select(y => y.Key)
    //        .ToList();
    //    if (badColumnNames.Any())
    //    {
    //        throw new InvalidOperationException("Duplicate column names found " + string.Join(", ", columnNames));
    //    }
    //    result.AddRange(columnNames);
    //    return result;
    //}

    /// <summary>
    ///
    /// </summary>
    /// <param name="schema"></param>
    /// <param name="InputStream"></param>
    public ImportSchemaHelper(DataSource ds, DataSchema schema, string data, ImportBatch batch, Sensor sensor = null)
    {
        using (Logging.MethodCall(GetType(), new MethodCallParameters { { "DataSource", ds.Name }, { "Schema", schema.Name }, { "ImportBatch", batch.Code }, { "Sensor", sensor?.Name } }))
        {
            dataSource = ds;
            dataSchema = schema;
            Sensor = sensor;
            LogBadValues = ConfigurationManager.AppSettings["LogBadValues"].IsTrue();
            Logging.Information("Checking Schema");
            if (schema.SchemaColumnRecords().Any(i => i.SchemaColumnType.Name == "Time") && !schema.SchemaColumnRecords().Any(i => i.SchemaColumnType.Name == "Date"))
            {
                throw new Exception("Schema has a Time but no Date column");
            }
            if (!schema.SchemaColumnRecords().Any(i => i.SchemaColumnType.Name == "Offering"))
            {
                throw new Exception("Schema has no Offering column(s)");
            }
            if (schema.SchemaColumnRecords().Any(i => i.SchemaColumnType.Name == "Latitude") && !schema.SchemaColumnRecords().Any(i => i.SchemaColumnType.Name == "Longitude"))
            {
                throw new Exception("Schema has a Latitude but no Longitude column");
            }
            if (schema.SchemaColumnRecords().Any(i => i.SchemaColumnType.Name == "Longitude") && !schema.SchemaColumnRecords().Any(i => i.SchemaColumnType.Name == "Latitude"))
            {
                throw new Exception("Schema has a Longitude but no Latitude column");
            }
            Logging.Information("Create ClassBuilder");
            Type recordType;
            if (schema.DataSourceTypeID == new Guid(DataSourceType.CSV))
            {
                DelimitedClassBuilder cb = new DelimitedClassBuilder("ImportBatches", schema.Delimiter)
                {
                    IgnoreEmptyLines = true,
                    IgnoreFirstLines = schema.IgnoreFirst
                };
                if (schema.HasColumnNames.HasValue && schema.HasColumnNames.Value)
                {
                    cb.IgnoreFirstLines++;
                }
                cb.IgnoreLastLines = schema.IgnoreLast;
                if (!String.IsNullOrEmpty(schema.Condition))
                {
                    schema.Condition = schema.Condition;
                }

                if (!(schema.HasColumnNames.HasValue && schema.HasColumnNames.Value))
                {
                    foreach (var col in schema.SchemaColumnRecords().OrderBy(sc => sc.Number))
                    {
                        cb.AddField(col.Name);
                    }
                }
                else
                {
                    // Load column names from file
                    List<string> columnNames = LoadColumnNamesDelimited(schema, data);
                    // Loop through and if in schema add else ignore
                    List<string> columnsNotInSchema = new List<string>();
                    foreach (var columnName in columnNames)
                    {
                        var col = schema.SchemaColumnRecords().Where(c => c.Name.Equals(columnName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                        cb.AddField(columnName);
                        if (col == null)
                        {
                            columnsNotInSchema.Add(columnName);
                            cb.LastField.FieldValueDiscarded = true;
                        }
                    }
                    if (columnsNotInSchema.Any())
                    {
                        batch.Issues += "Columns in data file but not in schema - " + string.Join(", ", columnsNotInSchema) + Environment.NewLine;
                        Logging.Warning("Columns in data file but not in schema: {columns}", columnsNotInSchema);
                    }
                    var columnsNotInDataFile = schema.SchemaColumnRecords().Select(c => c.Name.ToLower()).Except(cb.Fields.Select(f => f.FieldName.ToLower()));
                    if (columnsNotInDataFile.Any())
                    {
                        batch.Issues += "Columns in schema but not in data file - " + string.Join(", ", columnsNotInDataFile) + Environment.NewLine;
                        Logging.Warning("Columns in schema but not in data file: {columns}", columnsNotInDataFile);
                    }
                }
                //Logging.Information("Class: {class}", cb.GetClassSourceCode(NetLanguage.CSharp));
                recordType = cb.CreateRecordClass();
                engine = new DelimitedFileEngine(recordType);
            }
            else
            {
                FixedLengthClassBuilder cb = new FixedLengthClassBuilder(schema.Name, FixedMode.AllowVariableLength)
                {
                    IgnoreEmptyLines = true,
                    IgnoreFirstLines = schema.IgnoreFirst
                };
                //if (schema.HasColumnNames.HasValue && schema.HasColumnNames.Value)
                //{
                //    cb.IgnoreFirstLines++;
                //}
                cb.IgnoreLastLines = schema.IgnoreLast;
                if (!String.IsNullOrEmpty(schema.Condition))
                {
                    schema.Condition = schema.Condition;
                }
                //if (!(schema.HasColumnNames.HasValue && schema.HasColumnNames.Value))
                {
                    foreach (var col in schema.SchemaColumnRecords().OrderBy(sc => sc.Number))
                    {
                        cb.AddField(col.Name, col.Width.Value, typeof(string));
                    }
                }
                //else
                //{
                //    // Load column names from file
                //    List<string> columnNames = LoadColumnNamesFixedWidth(schema, data);
                //    // Loop through and if in schema add else ignore
                //    List<string> columnsNotInSchema = new List<string>();
                //    foreach (var columnName in columnNames)
                //    {
                //        var col = schema.SchemaColumnRecords().Where(c => c.Name.Equals(columnName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                //        cb.AddField(columnName);
                //        if (col == null)
                //        {
                //            columnsNotInSchema.Add(columnName);
                //            cb.LastField.FieldValueDiscarded = true;
                //        }
                //    }
                //    if (columnsNotInSchema.Any())
                //    {
                //        batch.Issues += "Columns in data file but not in schema - " + string.Join(", ", columnsNotInSchema) + Environment.NewLine;
                //        Logging.Warning("Columns in data file but not in schema: {columns}", columnsNotInSchema);
                //    }
                //    var columnsNotInDataFile = schema.SchemaColumnRecords().Select(c => c.Name.ToLower()).Except(cb.Fields.Select(f => f.FieldName.ToLower()));
                //    if (columnsNotInDataFile.Any())
                //    {
                //        batch.Issues += "Columns in schema but not in data file - " + string.Join(", ", columnsNotInDataFile) + Environment.NewLine;
                //        Logging.Warning("Columns in schema but not in data file: {columns}", columnsNotInDataFile);
                //    }
                //}
                //Logging.Information("Class: {class}", cb.GetClassSourceCode(NetLanguage.CSharp));
                recordType = cb.CreateRecordClass();
                engine = new FixedFileEngine(recordType);
            }
            Logging.Information("Create Engine");
            if (engine == null)
            {
                throw new NullReferenceException("Engine cannot be null");
            }

            engine.ErrorMode = ErrorMode.SaveAndContinue;

            //List<object> list = engine.ReadStringAsList(data);

            var fileName = $"{ds.Name}~~{DateTime.Now:yyyyMMdd HHmmss}~~{Path.GetFileName(batch.FileName)}";
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }

            foreach (var c in Path.GetInvalidPathChars())
            {
                fileName = fileName.Replace(c, '_');
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Logging.Information("Saving import file");
            SaveDocument(fileName, data);
            stopwatch.Stop();
            Logging.Information("Saved import file in {time}", stopwatch.Elapsed.TimeStr());
            stopwatch.Restart();
            Logging.Information("Reading DataTable");
            //dtResults = engine.ReadStringAsDT(data);
            dtResults = CommonEngine.RecordsToDataTable(engine.ReadString(data), recordType);
            //Logging.Information(dtResults.Dump());
            stopwatch.Stop();
            Logging.Information("Read DataTable in {time}", stopwatch.Elapsed.TimeStr());
            dtResults.TableName = ds.Name + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            schemaDefs = new List<SchemaDefinition>();
#if UseParallel
            SchemaValues = new BlockingCollection<SchemaValue>();
#else
            SchemaValues = new List<SchemaValue>();
#endif
        }
    }

    public void SaveDocument(string fileName, string fileContents)
    {
        string docPath = HostingEnvironment.MapPath(Path.Combine(ConfigurationManager.AppSettings["DocumentsPath"], "Uploads"));
        var yearMonth = DateTime.Now.ToString("yyyyMM");
        Directory.CreateDirectory(Path.Combine(docPath, yearMonth));
        File.WriteAllText(Path.Combine(docPath, yearMonth, fileName), fileContents);
        Azure.Upload($"Uploads/{yearMonth}", fileName, fileContents);
    }

    /// <summary>
    ///
    /// </summary>
    private void BuildSchemaDefinition()
    {
        using (Logging.MethodCall(GetType()))
        {
            for (int i = 0; i < dtResults.Columns.Count; i++)
            {
                DataColumn dtcol = dtResults.Columns[i];

                SchemaDefinition def = new SchemaDefinition()
                {
                    Index = i,
                    FieldName = dtcol.ColumnName
                };
                var schemaCol = dataSchema.SchemaColumnRecords().FirstOrDefault(sc => sc.Name.Equals(def.FieldName, StringComparison.CurrentCultureIgnoreCase));
                if (schemaCol == null)
                {
                    if ((dataSchema.DataSourceTypeID == new Guid(DataSourceType.CSV)) && dataSchema.HasColumnNames.HasValue && dataSchema.HasColumnNames.Value)
                    {
                        def.IsIgnored = true;
                    }
                    else
                    {
                        throw new ArgumentNullException($"Unable to find schema column with name {def.FieldName}");
                    }
                }
                else
                {
                    switch (schemaCol.SchemaColumnType.Name)
                    {
                        case "Date":
                            def.IsDate = true;
                            def.Dateformat = schemaCol.Format;
                            break;

                        case "Time":
                            def.IsTime = true;
                            def.Timeformat = schemaCol.Format;
                            break;

                        case "Ignore":
                            def.IsIgnored = true;
                            break;

                        case "Comment":
                            def.IsComment = true;
                            break;

                        case "Elevation":
                            def.IsElevation = true;
                            break;

                        case "Latitude":
                            def.IsLatitude = true;
                            break;

                        case "Longitude":
                            def.IsLongitude = true;
                            break;

                        case "Offering":
                        case "Fixed Time":
                            def.IsOffering = true;
                            def.PhenomenonOfferingID = schemaCol.PhenomenonOfferingID;
                            def.PhenomenonOffering = new PhenomenonOffering(def.PhenomenonOfferingID);
                            if (def.PhenomenonOffering == null)
                            {
                                def.InValidOffering = true;
                            }
                            else
                            {
                                def.DataSourceTransformations = LoadTransformations(def.PhenomenonOfferingID.Value);
                            }

                            def.PhenomenonUOMID = schemaCol.PhenomenonUOMID;
                            def.PhenomenonOUM = new PhenomenonUOM(def.PhenomenonUOMID);
                            if (def.PhenomenonOUM == null)
                            {
                                def.InValidUOM = true;
                            }

                            if (Sensor != null)
                            {
                                //def.SensorID = Sensor.Id;
                                def.Sensors.Clear();
                                def.Sensors.Add(Sensor);
                                def.SensorDates.Clear();
                                def.SensorDates.Add(new VSensorDateCollection().Where(VSensorDate.Columns.SensorID, Sensor.Id).Load().FirstOrDefault());
                            }
                            else
                            {
                                SensorCollection colsens = new Select()
                                                                      .From(Sensor.Schema)
                                                                      .Where(Sensor.PhenomenonIDColumn).IsEqualTo(def.PhenomenonOffering.PhenomenonID)
                                                                      .And(Sensor.DataSourceIDColumn).IsEqualTo(dataSource.Id)
                                                                      .ExecuteAsCollection<SensorCollection>();
                                if (colsens.Count() == 0)
                                {
                                    def.SensorNotFound = true;
                                }
                                else
                                {
                                    //def.SensorID = colsens[0].Id;
                                    def.Sensors.Clear();
                                    def.Sensors.AddRange(colsens.ToList());
                                    def.SensorDates.Clear();
                                    foreach (var sensor in def.Sensors)
                                    {
                                        def.SensorDates.Add(new VSensorDateCollection().Where(VSensorDate.Columns.SensorID, sensor.Id).Load().FirstOrDefault());
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(schemaCol.EmptyValue))
                            {
                                def.IsEmptyValue = true;
                                def.EmptyValue = schemaCol.EmptyValue;
                            }
                            if (schemaCol.SchemaColumnType.Name == "Fixed Time")
                            {
                                def.IsFixedTime = true;
                                def.FixedTimeValue = TimeSpan.Parse(schemaCol.FixedTime);
                            }
                            break;
                    }
                }

                schemaDefs.Add(def);
            }

            if (schemaDefs.FirstOrDefault(t => t.IsDate) != null && (schemaDefs.FirstOrDefault(t => t.IsTime) != null) || (schemaDefs.FirstOrDefault(t => t.IsFixedTime) != null))
            {
                concatedatetime = true;
            }

            Logging.Verbose("Schema: {Count:n0} Columns: {Columns:n0}", schemaDefs.Count, schemaDefs.Select(i => i.FieldName).ToList());
        }
    }

    private List<DataSourceTransformation> LoadTransformations(Guid offId)
    {
        DataSourceTransformationCollection col = new Select()
                                                .From(DataSourceTransformation.Schema)
                                                .InnerJoin(Phenomenon.IdColumn, DataSourceTransformation.PhenomenonIDColumn)
                                                .InnerJoin(PhenomenonOffering.PhenomenonIDColumn, Phenomenon.IdColumn)
                                                .InnerJoin(TransformationType.IdColumn, DataSourceTransformation.TransformationTypeIDColumn)
                                                .Where(PhenomenonOffering.IdColumn).IsEqualTo(offId)
                                                .And(DataSourceTransformation.DataSourceIDColumn).IsEqualTo(this.dataSource.Id)
                                                //.AndExpression(DataSourceTransformation.Columns.StartDate).IsNull()
                                                //    .Or(DataSourceTransformation.StartDateColumn).IsLessThanOrEqualTo(DateTime.Now.Date)
                                                //.CloseExpression()
                                                //.AndExpression(DataSourceTransformation.Columns.EndDate).IsNull()
                                                //    .Or(DataSourceTransformation.EndDateColumn).IsGreaterThanOrEqualTo(DateTime.Now)
                                                //.CloseExpression()
                                                .OrderAsc(TransformationType.IorderColumn.QualifiedName)
                                                .OrderAsc(DataSourceTransformation.RankColumn.QualifiedName)
                                                .ExecuteAsCollection<DataSourceTransformationCollection>();
        return col.Distinct().ToList();
    }

    /// <summary>
    ///
    /// </summary>
    public void ProcessSchema()
    {
        using (Logging.MethodCall(GetType()))
        {
            try
            {
                Logging.Information("Processing schema");
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var stageStopwatch = new Stopwatch();
                stageStopwatch.Start();
                Logging.Information("Building schema definition");
                BuildSchemaDefinition();
                Logging.Information("Built schema definition in {time}", stageStopwatch.Elapsed.TimeStr());
#if !UseParallel
                var lastProgress100 = -1;
#endif
                var nMax = dtResults.Rows.Count;
                var n = 1;
                Logging.Information("Processing {count:n0} rows", nMax);
                stageStopwatch.Restart();
#if UseParallel
                try
                {
                    Parallel.For(1, nMax, i => ProcessRow(dtResults.Rows[i - 1], i));
                    n = nMax;
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
#else
                foreach (DataRow row in dtResults.Rows)
                {
                    var progress = (double)n / nMax;
                    var progress100 = (int)(progress * 100);
                    var reportPorgress = (progress100 % 5 == 0) && (progress100 > 0) && (lastProgress100 != progress100);
                    var elapsed = stageStopwatch.Elapsed;
                    var total = TimeSpan.FromSeconds(elapsed.TotalSeconds / progress);
                    if (reportPorgress)
                    {
                        Logging.Information("{progress:p0} {row:n0} of {rows:n0} rows in {min} of {mins}, {rowTime}/row, {numRowsInSec:n3} rows/sec", progress, n, nMax, elapsed.TimeStr(), total.TimeStr(), TimeSpan.FromSeconds(elapsed.TotalSeconds / n).TimeStr(), n / elapsed.TotalSeconds);
                        lastProgress100 = progress100;
                    }
                    ProcessRow(row, n);
                    n++;
                }
#endif
                stageStopwatch.Stop();
                Logging.Information("Processed {count:n0} rows in {elapsed}, {rowTime}/row, {numRowsInSec:n3} rows/sec", nMax, stageStopwatch.Elapsed.TimeStr(), TimeSpan.FromSeconds(stageStopwatch.Elapsed.TotalSeconds / nMax).TimeStr(), nMax / stageStopwatch.Elapsed.TotalSeconds);
                Logging.Information("Checking for duplicates in batch");
                stageStopwatch.Restart();
                var dupGroups = SchemaValues.GroupBy(i => new { i.SensorID, i.DateValue, i.DataValue, i.PhenomenonOfferingID, i.PhenomenonUOMID, i.Elevation }).Where(g => g.Count() > 1).ToList();
                var dupValues = dupGroups.SelectMany(i => i).ToList();
                if (dupGroups.Any())
                {
                    foreach (var value in dupValues.Take(100))
                    {
                        Logging.Information("RowNum: {rowNum} Date: {date}", value.RowNum, value.DateValue);
                    }
                    //Logging.Information("Bad Rows: {badRows}", dupValues.Select(i => i.RowNum).ToArray());
                    Logging.Information("Duplicates: Groups: {groups} Values: {values}", dupGroups.Count, dupValues.Count);
                    foreach (var schval in dupValues)
                    {
                        schval.IsDuplicateInBatch = true;
                        schval.InvalidStatuses.Insert(0, Status.DuplicateInBatch);
                    }
                    Logging.Information("Found {count:n0} duplicates in batch", dupValues.Count());
                }
                stageStopwatch.Stop();
                Logging.Information("Checked for duplicates in batch in {elapsed}", stageStopwatch.Elapsed.TimeStr());
                stopwatch.Stop();
                Logging.Information("Processed Schema in {time}", stopwatch.Elapsed.TimeStr());
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
                throw;
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="dr"></param>
    private void ProcessRow(DataRow dr, int rowNum)
    {
        using (Logging.MethodCall(GetType(), new MethodCallParameters { { "Row", rowNum } }))
        {
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var stageStart = stopwatch.Elapsed;
#if VeryDetailedLogging
                Logging.Information("ProcessRow({RowNum})", rowNum);
#endif
                //Logging.Verbose(dr.Dump());
                DateTime dttme = DateTime.MinValue,
                dt = DateTime.MinValue,
                tm = DateTime.MinValue;
                bool ErrorInDate = false;
                bool ErrorInTime = false;

                string InvalidDateValue = String.Empty,
                       InvalidTimeValue = String.Empty,
                       RowComment = String.Empty;

                SchemaDefinition dtdef = schemaDefs.FirstOrDefault(t => t.IsDate);
                SchemaDefinition tmdef = schemaDefs.FirstOrDefault(t => t.IsTime) ?? schemaDefs.FirstOrDefault(t => t.IsFixedTime);

                Guid correlationID = Guid.NewGuid();

                if (dtdef != null)
                {
                    //CultureInfo ci = new CultureInfo(CultureInfo.CurrentCulture.LCID);
                    //ci.Calendar.TwoDigitYearMax = 2000;
                    string sDateValue = dr[dtdef.Index].ToString();
                    if (String.IsNullOrEmpty(sDateValue) || !DateTime.TryParseExact(sDateValue.ToUpper().Trim(), dtdef.Dateformat, null, DateTimeStyles.None, out dt))
                    {
                        ErrorInDate = true;
                        InvalidDateValue = sDateValue;
                        try
                        {
                            dt = DateTime.ParseExact(sDateValue.ToUpper().Trim(), dtdef.Dateformat, null, DateTimeStyles.None);
                        }
                        catch (Exception ex)
                        {
                            var exc = new FormatException($"{ex.Message} Row#: {rowNum} Date: {sDateValue} Format: {dtdef.Dateformat}", ex);
                            Logging.Exception(exc);
                            throw exc;
                        }
                    }
                }

                if (tmdef != null)
                {
                    if (tmdef.IsTime)
                    {
                        string sTimeValue = dr[tmdef.Index].ToString();
                        if (String.IsNullOrEmpty(sTimeValue) || !DateTime.TryParseExact(sTimeValue.ToUpper().Trim(), tmdef.Timeformat, null, DateTimeStyles.None, out tm))
                        {
                            ErrorInTime = true;
                            InvalidTimeValue = sTimeValue;
                            try
                            {
                                tm = DateTime.ParseExact(sTimeValue.ToUpper().Trim(), tmdef.Timeformat, null, DateTimeStyles.None);
                            }
                            catch (Exception ex)
                            {
                                var exc = new FormatException($"{ex.Message} Row#: {rowNum} Time: {sTimeValue} Format: {tmdef.Timeformat}", ex);
                                Logging.Exception(exc);
                                throw exc;
                            }
                        }
                    }
                    else if (tmdef.IsFixedTime)
                    {
                        tm = DateTime.Now.Date.AddMilliseconds(tmdef.FixedTimeValue.TotalMilliseconds);
                    }
                }

                if (concatedatetime &&
                   !ErrorInDate &&
                   !ErrorInTime)
                {
                    dttme = dt.Date.AddMilliseconds(tm.TimeOfDay.TotalMilliseconds);
                }

                //Add Row Comment
                foreach (var df in schemaDefs.Where(t => t.IsComment))
                {
                    var comment = dr[df.Index].ToString().Trim();
                    if (!string.IsNullOrWhiteSpace(comment))
                    {
                        if (string.IsNullOrWhiteSpace(RowComment))
                        {
                            RowComment = comment;
                        }
                        else
                        {
                            RowComment += "; " + comment;
                        }
                    }
                }


                for (int i = 0; i < schemaDefs.Count; i++)
                {
                    SchemaDefinition def = schemaDefs[i];

                    if (def.IsOffering)
                    {
#if VeryDetailedLogging
                        Logging.Information("Sensor: {sensor} {elapsed} {stage}", i + 1, stopwatch.Elapsed.TimeStr(), (stopwatch.Elapsed - stageStart).TimeStr());
                        stageStart = stopwatch.Elapsed;
#endif
                        var rec = new SchemaValue()
                        {
                            SensorNotFound = def.SensorNotFound,
                            RowNum = rowNum
                            //SensorID = def.SensorID
                        };
                        if (rec.SensorNotFound)
                        {
                            rec.SensorID = def.Sensors.FirstOrDefault()?.Id;
                            rec.InvalidStatuses.Add(Status.SensorNotFound);
                        }
                        string RawValue = dr[def.Index].ToString();
                        rec.InvalidOffering = def.InValidOffering;
                        rec.PhenomenonOfferingID = def.PhenomenonOfferingID;

                        if (rec.InvalidOffering)
                        {
                            rec.InvalidStatuses.Add(Status.OfferingInvalid);
                        }

                        rec.InvalidUOM = def.InValidUOM;
                        rec.PhenomenonUOMID = def.PhenomenonUOMID;
                        if (rec.InvalidUOM)
                        {
                            rec.InvalidStatuses.Add(Status.UOMInvalid);
                        }

                        if (Logging.Level == LogEventLevel.Verbose)
                        {
                            Logging.Verbose("Row#: {row} Index: {Index} Column: {Column} Phenomenon: {Phenomenon} Offering: {Offering} Phenomenon: {Phenomenon} UnitOfMeasure: {UnitOfMeasure}",
                                rowNum, def.Index, def.FieldName, def.PhenomenonOffering?.Phenomenon?.Name, def.PhenomenonOffering?.Offering?.Name, def.PhenomenonOUM?.Phenomenon?.Name, def.PhenomenonOUM?.UnitOfMeasure?.Unit);
                        }
                        if (ErrorInTime)
                        {
                            rec.TimeValueInvalid = true;
                            rec.InvalidTimeValue = InvalidTimeValue;
                            rec.InvalidStatuses.Add(Status.TimeInvalid);
                        }

                        if (ErrorInDate)
                        {
                            rec.DateValueInvalid = true;
                            rec.InvalidDateValue = InvalidDateValue;
                            rec.InvalidStatuses.Add(Status.DateInvalid);

                            //Make Time visible on input
                            rec.TimeValueInvalid = true;
                            rec.InvalidTimeValue = "";
                        }

                        if (concatedatetime && !ErrorInDate && !ErrorInTime)
                        {
                            rec.DateValue = dttme;
                        }
                        else if (!ErrorInDate)
                        {
                            rec.DateValue = dt;
                        }

                        if (!ErrorInTime)
                        {
                            rec.TimeValue = tm;
                        }

                        if (!ErrorInDate)
                        {
#if VeryDetailedLogging
                            Logging.Information("Error checks {elapsed} {stage}", stopwatch.Elapsed.TimeStr(), (stopwatch.Elapsed - stageStart).TimeStr());
                            stageStart = stopwatch.Elapsed;
#endif
                            // Find sensor based on DateValue
                            bool found = false;
                            bool foundTooEarly = false;
                            bool foundTooLate = false;
                            if (def.Sensors.Count > 0)
                            {
                                Logging.Verbose("Row#: {row} Sensors: {sensors}", rowNum, def.Sensors.Select(s => s.Name).ToList());
                            }
                            foreach (var sensor in def.Sensors)
                            {
                                // Sensor x Instrument_Sensor x Instrument x Station_Instrument x Station x Site
                                var index = def.Sensors.IndexOf(sensor);
                                var dates = def.SensorDates.ElementAt(index);
                                //var dates = new VSensorDateCollection().Where(VSensorDate.Columns.SensorID, sensor.Id).Load().FirstOrDefault();
                                if (dates == null)
                                {
                                    continue;
                                }

                                if (dates.StartDate.HasValue && (rec.DateValue < dates.StartDate.Value))
                                {
                                    Logging.Error("Row#: {row} Date too early, ignoring! Sensor: {sensor} StartDate: {startDate} Date: {recDate} Rec: {@rec}", rowNum, sensor.Name, dates.StartDate, rec.DateValue, rec);
                                    foundTooEarly = true;
                                    continue;
                                }
                                if (dates.EndDate.HasValue && (rec.DateValue > dates.EndDate.Value))
                                {
                                    Logging.Error("Row#: {row} Date too late, ignoring! Sensor: {sensor} EndDate: {endDate} Date: {recDate} Rec: {@rec}", rowNum, sensor.Name, dates.EndDate, rec.DateValue, rec);
                                    foundTooLate = true;
                                    continue;
                                }
                                rec.SensorID = sensor.Id;
                                found = true;
                                break;
                            }
                            if (!found)
                            {
                                if (foundTooEarly || foundTooLate)
                                {
                                    continue; // Ignore
                                }

                                if (LogBadValues)
                                {
                                    Logging.Error("Row#: {row} Index: {Index} FieldName: {FieldName} Sensor not found Sensors: {sensors}", rowNum, def.Index, def.FieldName, def.Sensors.Select(s => s.Name).ToList());
                                }
                                rec.SensorNotFound = true;
                                rec.SensorID = def.Sensors.FirstOrDefault()?.Id;
                                rec.InvalidStatuses.Add(Status.SensorNotFound);
                            }
                        }
#if VeryDetailedLogging
                        Logging.Information("Sensor lookup {elapsed} {stage}", stopwatch.Elapsed.TimeStr(), (stopwatch.Elapsed - stageStart).TimeStr());
                        stageStart = stopwatch.Elapsed;
#endif
                        //if (!ErrorInDate && LogHelper != null && LogHelper.CheckRecordGap(rec.DateValue))
                        //{
                        //    rec.RawValue = null;
                        //    rec.DataValue = null;
                        //}
                        //else if (String.IsNullOrEmpty(RawValue) || def.IsEmptyValue && RawValue.Trim() == def.EmptyValue)
                        if (String.IsNullOrEmpty(RawValue) || def.IsEmptyValue && RawValue.Trim() == def.EmptyValue)
                        {
                            rec.FieldRawValue = RawValue;
                            rec.TextValue = RawValue;
                            rec.RawValue = null; // dataSource.DefaultNullValue;
                            rec.DataValue = null; // dataSource.DefaultNullValue;
                            foreach (var transform in def.DataSourceTransformations)
                            {
                                TransformValue(transform, ref rec, rowNum, true);
                            }
                        }
                        else
                        {
                            rec.FieldRawValue = RawValue;
                            rec.RawValue = null;
                            double dvalue = -1;

                            // Possibly do lookups here
                            if (!double.TryParse(RawValue, out dvalue))
                            {
                                rec.TextValue = RawValue;
                                foreach (var transform in def.DataSourceTransformations.Where(t => t.TransformationType.Name == "Lookup"))
                                {
                                    TransformValue(transform, ref rec, rowNum);
                                    if (rec.RawValue.HasValue)
                                    {
                                        RawValue = rec.RawValue.Value.ToString();
                                    }
                                }
                            }
                            if (!double.TryParse(RawValue, out dvalue))
                            {
                                rec.RawValueInvalid = true;
                                rec.InvalidRawValue = RawValue;
                                rec.InvalidStatuses.Add(Status.ValueInvalid);
                                try
                                {
                                    double d = double.Parse(RawValue);
                                }
                                catch (Exception ex)
                                {
                                    Logging.Exception(ex, "Row#: {row} RawValue: {RawValue} DataRow: {Dump}", rowNum, RawValue, dr.Dump());
                                }
                            }
                            else
                            {
                                rec.RawValue = dvalue;
                                rec.DataValue = rec.RawValue;
                                foreach (var transform in def.DataSourceTransformations.Where(t => t.TransformationType.Name != "Lookup"))
                                {
                                    TransformValue(transform, ref rec, rowNum);
                                }
                            }
                        }
#if VeryDetailedLogging
                        Logging.Information("Transformations {elapsed} {stage}", stopwatch.Elapsed.TimeStr(), (stopwatch.Elapsed - stageStart).TimeStr());
                        stageStart = stopwatch.Elapsed;
#endif
                        // Location
                        var defLatitude = schemaDefs.FirstOrDefault(d => d.IsLatitude);
                        var defLongitude = schemaDefs.FirstOrDefault(d => d.IsLongitude);
                        var defElevation = schemaDefs.FirstOrDefault(d => d.IsElevation);
                        if ((defLatitude != null) && (defLongitude != null))
                        {
                            string rawLatitude = dr[defLatitude.Index].ToString();
                            string rawLongitude = dr[defLongitude.Index].ToString();
                            if (!string.IsNullOrWhiteSpace(rawLatitude) && !string.IsNullOrWhiteSpace(rawLongitude))
                            {
                                try
                                {
                                    rec.Latitude = double.Parse(rawLatitude);
                                }
                                catch (Exception ex)
                                {
                                    Logging.Exception(ex, "Row#: {row} RawLatitude: {RawLatitude} DataRow: {Dump}", rowNum, RawValue, dr.Dump());
                                }
                                try
                                {
                                    rec.Longitude = double.Parse(rawLongitude);
                                }
                                catch (Exception ex)
                                {
                                    Logging.Exception(ex, "Row#: {row} RawLongitude: {RawLongitude} DataRow: {Dump}", rowNum, RawValue, dr.Dump());
                                }
                            }
                        }
                        if (defElevation != null)
                        {
                            string rawElevation = dr[defElevation.Index].ToString();
                            if (!string.IsNullOrWhiteSpace(rawElevation))
                            {
                                try
                                {
                                    rec.Elevation = double.Parse(rawElevation);
                                }
                                catch (Exception ex)
                                {
                                    Logging.Exception(ex, "Row#: {row} RawElevation: {RawElevation} DataRow: {Dump}", rowNum, RawValue, dr.Dump());
                                }
                            }
                        }
#if VeryDetailedLogging
                        Logging.Verbose("Location {elapsed} {stage}", stopwatch.Elapsed.TimeStr(), (stopwatch.Elapsed - stageStart).TimeStr());
                        stageStart = stopwatch.Elapsed;
#endif
                        if (!string.IsNullOrWhiteSpace(RowComment))
                        {
                            rec.Comment = RowComment.TrimEnd();
                        }

                        rec.CorrelationID = correlationID;

                        CheckDuplicate(def, rec);
#if VeryDetailedLogging
                        Logging.Information("Duplicate {elapsed} {stage}", stopwatch.Elapsed.TimeStr(), (stopwatch.Elapsed - stageStart).TimeStr());
                        stageStart = stopwatch.Elapsed;
#endif

                        //                        CheckIsDuplicate(def, rec);
                        //#if VeryDetailedLogging
                        //                        Logging.Information("IsDuplicate {elapsed} {stage}", stopwatch.Elapsed.TimeStr(), (stopwatch.Elapsed - stageStart).TimeStr());
                        //                        stageStart = stopwatch.Elapsed;
                        //#endif
                        //                        CheckIsDuplicateOfNull(def, rec);
                        //#if VeryDetailedLogging
                        //                        Logging.Information("IsDuplicateOfNull {elapsed} {stage}", stopwatch.Elapsed.TimeStr(), (stopwatch.Elapsed - stageStart).TimeStr());
                        //                        stageStart = stopwatch.Elapsed;
                        //#endif
                        SchemaValues.Add(rec);
                    }
                }
#if DetailedLogging || VeryDetailedLogging
                Logging.Information("ProcessRow({RowNum}) in {elapsed}", rowNum, stopwatch.Elapsed.TimeStr());
#endif
            }
            catch (Exception ex)
            {
                Logging.Exception(ex, "Row#: {row} DataRow: {Dump}", rowNum, dr.Dump());
                throw;
            }
        }

        bool CheckDuplicate(SchemaDefinition def, SchemaValue schval)
        {
            SqlQuery q = new Select().From(Observation.Schema)
                .Where(Observation.Columns.SensorID).IsEqualTo(schval.SensorID)
                .And(Observation.Columns.ValueDate).IsEqualTo(schval.DateValue)
                .And(Observation.Columns.PhenomenonOfferingID).IsEqualTo(schval.PhenomenonOfferingID)
                .And(Observation.Columns.PhenomenonUOMID).IsEqualTo(schval.PhenomenonUOMID);
            ObservationCollection oCol = q.ExecuteAsCollection<ObservationCollection>();
            var result = oCol.Any();
            if (result)
            {
                var obs = oCol.FirstOrDefault();
                if (obs == null)
                {
                    schval.IsDuplicateOfNull = true;
                    schval.InvalidStatuses.Insert(0, Status.DuplicateOfNull);
                }
                else
                {
                    schval.IsDuplicate = true;
                    schval.InvalidStatuses.Insert(0, Status.Duplicate);
                }
            }
            return result;
        }

        //bool CheckIsDuplicate(SchemaDefinition def, SchemaValue schval)
        //{
        //    var result = false;
        //        SqlQuery q = new Select().From(Observation.Schema)
        //        .Where(Observation.Columns.SensorID).IsEqualTo(schval.SensorID)
        //        .And(Observation.Columns.ValueDate).IsEqualTo(schval.DateValue)
        //        .And(Observation.Columns.DataValue).IsEqualTo(schval.DataValue)
        //        .And(Observation.Columns.PhenomenonOfferingID).IsEqualTo(schval.PhenomenonOfferingID)
        //        .And(Observation.Columns.PhenomenonUOMID).IsEqualTo(schval.PhenomenonUOMID);

        //        ObservationCollection oCol = q.ExecuteAsCollection<ObservationCollection>();
        //        result = oCol.Any();
        //    if (result)
        //    {
        //        schval.IsDuplicate = true;
        //        schval.InvalidStatuses.Insert(0, Status.Duplicate);
        //    }
        //    return result;
        //}

        //bool CheckIsDuplicateOfNull(SchemaDefinition def, SchemaValue schval)
        //{
        //    var result = false;
        //        SqlQuery q = new Select().From(Observation.Schema)
        //    .Where(Observation.Columns.SensorID).IsEqualTo(schval.SensorID)
        //    .And(Observation.Columns.ValueDate).IsEqualTo(schval.DateValue)
        //    .And(Observation.Columns.DataValue).IsNull()
        //    .And(Observation.Columns.PhenomenonOfferingID).IsEqualTo(schval.PhenomenonOfferingID)
        //    .And(Observation.Columns.PhenomenonUOMID).IsEqualTo(schval.PhenomenonUOMID);

        //        ObservationCollection oCol = q.ExecuteAsCollection<ObservationCollection>();
        //        result = oCol.Any();
        //    if (result)
        //    {
        //        schval.IsDuplicateOfNull = true;
        //        schval.InvalidStatuses.Insert(0, Status.DuplicateOfNull);
        //    }
        //    return result;
        //}
    }

    /// <summary>
    ///
    /// </summary>
    private void TransformValue(DataSourceTransformation trns, ref SchemaValue rec, int rowNum, bool isEmpty = false)
    {
        using (Logging.MethodCall(GetType()))
        {
            try
            {
                bool valid = true;
                if (Logging.Level == LogEventLevel.Verbose)
                {
                    Logging.Verbose("Row#: {row} Phenomenon: {Phenomenon} Offering: {Offering} {TransOfferingID} {RecOfferingID} UnitOfMeasure: {UnitOfMeasure} {TransUnitOfMeasureID} {RecUnitOfMeasureID} StartDate: {StartDate} EndDate: {EndDate} Date: {Date}",
                        rowNum, trns?.Phenomenon?.Name, trns?.PhenomenonOffering?.Offering?.Name, trns?.PhenomenonOfferingID, rec?.PhenomenonOfferingID,
                        trns?.PhenomenonUOM?.UnitOfMeasure?.Unit, trns?.PhenomenonUOMID, rec?.PhenomenonUOMID, trns?.StartDate, trns?.EndDate, rec.DateValue);
                }
                bool process = trns.PhenomenonOfferingID.HasValue ? trns.PhenomenonOfferingID.Value == rec.PhenomenonOfferingID.Value : false ||
                                !trns.PhenomenonUOMID.HasValue || trns.PhenomenonUOMID.Value == rec.PhenomenonUOMID.Value;
                if (process)
                {
                    if (trns.StartDate.HasValue && (rec.DateValue < trns.StartDate.Value))
                    {
                        process = false;
                    }

                    if (trns.EndDate.HasValue && (rec.DateValue > trns.EndDate.Value))
                    {
                        process = false;
                    }
                }

                //if ((trns.TransformationTypeID.ToString() != TransformationType.Lookup) && !rec.RawValue.HasValue) process = false;

                if (!process)
                {
                    Logging.Verbose("Row#: {row} Ignoring transformation", rowNum);
                    //rec.DataValue = rec.RawValue;
                    return;
                }

                if (!isEmpty)
                {
                    if (trns.TransformationType.Code == TransformationType.CorrectionValues)
                    {
                        Dictionary<string, string> corrvals = trns.CorrectionValues;

                        if (corrvals.ContainsKey("eq"))
                        {
                            //string eq = corrvals["equation"].Replace("\"","").Replace("\"","");
                            Expression exp = new Expression(corrvals["eq"]);
                            exp.Parameters["value"] = rec.RawValue;
                            object val = exp.Evaluate();
                            rec.DataValue = double.Parse(val.ToString());
                            Logging.Verbose("Row#: {row} Correction Raw: {RawValue} Data: {DataValue}", rowNum, rec.RawValue, rec.DataValue);
                        }
                    }
                    else if (trns.TransformationType.Code == TransformationType.RatingTable)
                    {
                        rec.DataValue = trns.GetRatingValue(rec.RawValue.Value);
                        Logging.Verbose("Row#: {row} Rating Raw: {RawValue} Data: {DataValue}", rowNum, rec.RawValue, rec.DataValue);
                    }
                    else if (trns.TransformationType.Code == TransformationType.QualityControlValues)
                    {
                        if (!rec.DataValue.HasValue)
                        {
                            rec.DataValue = rec.RawValue;
                        }

                        Dictionary<string, double> qv = trns.QualityValues;
                        if (qv.ContainsKey("min") && rec.DataValue.Value < qv["min"])
                        {
                            valid = false;
                        }

                        if (qv.ContainsKey("max") && rec.DataValue.Value > qv["max"])
                        {
                            valid = false;
                        }

                        Logging.Verbose("Row#: {row} QualityControl Valid: {Valid}", rowNum, valid);
                        if (!valid)
                        {
                            rec.InvalidStatuses.Add(Status.TransformValueInvalid);
                            rec.DataSourceTransformationID = trns.Id;

                            rec.DataValueInvalid = true;
                            rec.InvalidDataValue = rec.DataValue.ToString();
                        }
                    }
                    else if (trns.TransformationType.Code == TransformationType.Lookup)
                    {
                        var qv = trns.LookupValues;
                        if (!qv.ContainsKey(rec.FieldRawValue))
                        {
                            valid = false;
                        }
                        else
                        {
                            rec.RawValue = trns.LookupValues[rec.FieldRawValue];
                            rec.DataValue = rec.RawValue;
                        }

                        Logging.Verbose("Row#: {row} Lookup Valid: {Valid} Raw: {RawValue} Data: {DataValue}", rowNum, valid, rec.RawValue, rec.DataValue);
                        if (!valid)
                        {
                            rec.InvalidStatuses.Add(Status.TransformValueInvalid);
                            rec.DataSourceTransformationID = trns.Id;

                            rec.DataValueInvalid = true;
                            rec.InvalidDataValue = rec.DataValue.ToString();
                        }
                    }
                    else if (trns.TransformationType.Code == TransformationType.Expression)
                    {
                        Expression expr = new Expression(trns.Definition, EvaluateOptions.IgnoreCase);
                        expr.Parameters["value"] = rec.RawValue;
                        if (trns.ParamA.HasValue)
                        {
                            expr.Parameters["a"] = trns.ParamA.Value;
                        }
                        if (trns.ParamB.HasValue)
                        {
                            expr.Parameters["b"] = trns.ParamB.Value;
                        }
                        if (trns.ParamC.HasValue)
                        {
                            expr.Parameters["c"] = trns.ParamC.Value;
                        }
                        if (trns.ParamD.HasValue)
                        {
                            expr.Parameters["d"] = trns.ParamD.Value;
                        }
                        if (trns.ParamE.HasValue)
                        {
                            expr.Parameters["e"] = trns.ParamE.Value;
                        }
                        if (trns.ParamF.HasValue)
                        {
                            expr.Parameters["f"] = trns.ParamF.Value;
                        }
                        if (trns.ParamG.HasValue)
                        {
                            expr.Parameters["g"] = trns.ParamG.Value;
                        }
                        if (trns.ParamH.HasValue)
                        {
                            expr.Parameters["h"] = trns.ParamH.Value;
                        }
                        if (trns.ParamI.HasValue)
                        {
                            expr.Parameters["i"] = trns.ParamI.Value;
                        }
                        if (trns.ParamJ.HasValue)
                        {
                            expr.Parameters["j"] = trns.ParamJ.Value;
                        }
                        if (trns.ParamK.HasValue)
                        {
                            expr.Parameters["k"] = trns.ParamK.Value;
                        }
                        if (trns.ParamL.HasValue)
                        {
                            expr.Parameters["l"] = trns.ParamL.Value;
                        }
                        if (trns.ParamM.HasValue)
                        {
                            expr.Parameters["m"] = trns.ParamM.Value;
                        }
                        if (trns.ParamN.HasValue)
                        {
                            expr.Parameters["n"] = trns.ParamN.Value;
                        }
                        if (trns.ParamO.HasValue)
                        {
                            expr.Parameters["o"] = trns.ParamO.Value;
                        }
                        if (trns.ParamP.HasValue)
                        {
                            expr.Parameters["p"] = trns.ParamP.Value;
                        }
                        if (trns.ParamQ.HasValue)
                        {
                            expr.Parameters["q"] = trns.ParamQ.Value;
                        }
                        if (trns.ParamR.HasValue)
                        {
                            expr.Parameters["r"] = trns.ParamR.Value;
                        }
                        if (trns.ParamSX.HasValue)
                        {
                            expr.Parameters["s"] = trns.ParamSX.Value;
                        }
                        if (trns.ParamT.HasValue)
                        {
                            expr.Parameters["t"] = trns.ParamT.Value;
                        }
                        if (trns.ParamU.HasValue)
                        {
                            expr.Parameters["u"] = trns.ParamU.Value;
                        }
                        if (trns.ParamV.HasValue)
                        {
                            expr.Parameters["v"] = trns.ParamV.Value;
                        }
                        if (trns.ParamW.HasValue)
                        {
                            expr.Parameters["w"] = trns.ParamW.Value;
                        }
                        if (trns.ParamX.HasValue)
                        {
                            expr.Parameters["x"] = trns.ParamX.Value;
                        }
                        if (trns.ParamY.HasValue)
                        {
                            expr.Parameters["y"] = trns.ParamY.Value;
                        }
                        try
                        {
                            if (expr.HasErrors())
                            {
                                throw new EvaluateException($"Error in expression - {expr.Error}");
                            }
                            var valueStr = expr.Evaluate();
                            //Logging.Verbose("ValueStr: {value}", valueStr);
                            var value = double.Parse(valueStr.ToString());
                            //Logging.Verbose("Value: {value}", value);
                            rec.DataValue = value;
                            Logging.Verbose("Row#: {row} Valid: {Valid} ValueStr: {ValueStr} Value: {Value} Raw: {RawValue} Data: {DataValue}", rowNum, true, valueStr, value, rec.RawValue, rec.DataValue);
                        }
                        catch (Exception ex)
                        {
                            rec.InvalidStatuses.Add(Status.TransformValueInvalid);
                            rec.DataSourceTransformationID = trns.Id;
                            rec.DataValueInvalid = true;
                            rec.InvalidDataValue = rec.RawValue?.ToString();
                            Logging.Verbose("Row#: {row} Value: {Valid} Raw: {RawValue} Expr: {expr} Ex: {Exception}", rowNum, false, rec.RawValue, trns.Definition, ex.Message);
                            throw;
                        }
                    }
                }
                //Set new offering/UOM
                if (trns.NewPhenomenonOfferingID.HasValue && (rec.PhenomenonOfferingID.Value != trns.NewPhenomenonOfferingID.Value))
                {
                    rec.RawPhenomenonOfferingID = trns.PhenomenonOfferingID;
                    rec.PhenomenonOfferingID = trns.NewPhenomenonOfferingID;
                }
                if (trns.NewPhenomenonUOMID.HasValue && (rec.PhenomenonUOMID.Value != trns.NewPhenomenonUOMID.Value))
                {
                    rec.RawPhenomenonUOMID = trns.PhenomenonUOMID;
                    rec.PhenomenonUOMID = trns.NewPhenomenonUOMID;
                }
            }
            catch (Exception ex)
            {
                Logging.Exception(ex, "Row#: {row} dtid: {dtid} rec: {@rec})", rowNum, trns.Id, rec);
                throw;
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    public List<object> Errors
    {
        get
        {
            List<object> _errors = new List<object>();

            foreach (ErrorInfo errinfo in engine.ErrorManager.Errors)
            {
                _errors.Add(new { ErrorMessage = errinfo.ExceptionInfo.Message, LineNo = errinfo.LineNumber, errinfo.RecordString });
            }

            return _errors;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
        {
            return;
        }

        if (disposing)
        {
        }
        disposed = true;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="ds"></param>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static string GetWorkingStream(DataSchema ds, StreamReader reader)
    {
        String Result;

        if (!String.IsNullOrEmpty(ds.SplitSelector))
        {
            StringBuilder sb = new StringBuilder();

            string line;

            int index = 0;

            while ((line = reader.ReadLine()) != null && index <= ds.SplitIndex)
            {
                if (line.StartsWith(ds.SplitSelector))
                {
                    index++;
                }

                if (index == ds.SplitIndex)
                {
                    sb.AppendLine(line);
                    break;
                }
            }

            while ((line = reader.ReadLine()) != null && index <= ds.SplitIndex)
            {
                if (line.StartsWith(ds.SplitSelector))
                {
                    index++;
                }

                if (index <= ds.SplitIndex)
                {
                    sb.AppendLine(line);
                }
            }

            Result = sb.ToString();
        }
        else
        {
            Result = reader.ReadToEnd();
        }

        return Result;
    }
}

