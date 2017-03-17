using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FileHelpers;
using FileHelpers.Dynamic;
using System.IO;
using System.Data;
using SubSonic;
using System.Globalization;
using System.Text;
using NCalc;
using SAEON.Observations.Data;
using Serilog;
using Serilog.Context;
using System.Web.Configuration;
using System.Web.Hosting;

/// <summary>
/// Summary description for ImportSchema
/// </summary>
public class ImportSchemaHelper : IDisposable
{
    bool disposed = false;
    FileHelperEngine engine;
    DataTable dtResults;
    DataSource dataSource;
    DataSchema dataSchema;
    List<DataSourceTransformation> transformations;
    Type recordType;
    List<SchemaDefinition> schemaDefs;
    public List<SchemaValue> SchemaValues;

    Sensor Sensor = null;

    /// <summary>
    /// Gap Record Helper
    /// </summary>
    ImportLogHelper LogHelper = null;

    //public SchemaValues

    Boolean concatedatetime = false;

    string docNamePrefix;

    // string SourceFile;
    // string Pass1File; // As loaded from source file
    // string Pass2File; // After 1st R Call
    // string Pass3File; // After processing
    // string Pass4File; // After 2nd R Call


    private List<string> LoadColumnNamesDelimited(DataSchema schema, string data)
    {
        List<string> result = new List<string>();
        string[] lines = data.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        int columnNamesLine = schema.IgnoreFirst;
        if (columnNamesLine >= lines.Length)
        {
            throw new IndexOutOfRangeException("Column Names line greater than lines in source file");
        }
        {
            List<string> columnNames = lines[columnNamesLine].Split(new string[] { schema.Delimiter.Replace("\\t", "\t") }, StringSplitOptions.None).ToList();
            List<string> badColumnNames = columnNames
                .GroupBy(x => x)
                .Where(g => g.Count() > 1)
                .Select(y => y.Key)
                .ToList();
            if (badColumnNames.Any())
            {
                throw new InvalidOperationException("Duplicate column names found " + string.Join(", ", columnNames));
            }
            else
            {
                result.AddRange(columnNames);
            }
        }
        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="schema"></param>
    /// <param name="InputStream"></param>
    public ImportSchemaHelper(DataSource ds, DataSchema schema, string data, ImportBatch batch, Sensor obj = null, ImportLogHelper loghelper = null)
    {
        dataSource = ds;
        dataSchema = schema;
        if (schema.DataSourceTypeID == new Guid(DataSourceType.CSV))
        {
            DelimitedClassBuilder cb = new DelimitedClassBuilder("ImportBatches", schema.Delimiter);
            cb.IgnoreEmptyLines = true;
            cb.IgnoreFirstLines = schema.IgnoreFirst;
            if (schema.HasColumnNames.HasValue && schema.HasColumnNames.Value)
            {
                cb.IgnoreFirstLines++;
            }
            cb.IgnoreLastLines = schema.IgnoreLast;
            if (!String.IsNullOrEmpty(schema.Condition))
                schema.Condition = schema.Condition;
            if (!String.IsNullOrEmpty(schema.SplitSelector))
            {
                cb.SplitSelector = schema.SplitSelector;
                cb.SplitIndex = schema.SplitIndex.Value;
            }
            if (!(schema.HasColumnNames.HasValue && schema.HasColumnNames.Value))
            {
                foreach (var col in schema.SchemaColumnRecords().OrderBy(sc => sc.Number))
                {
                    cb.AddField(col.Name, typeof(string));
                    switch (col.SchemaColumnType.Name)
                    {
                        case "Date":
                            cb.LastField.ValueDate = true;
                            cb.LastField.ValueDateformat = col.Format;
                            break;
                        case "Time":
                            cb.LastField.ValueTime = true;
                            cb.LastField.ValueTimeformat = col.Format;
                            break;
                        case "Ignore":
                            cb.LastField.FieldValueDiscarded = true;
                            break;
                        case "Comment":
                            cb.LastField.ValueComment = true;
                            break;
                        case "Offering":
                            if (!string.IsNullOrEmpty(col.EmptyValue))
                            {
                                cb.LastField.ValueEmpty = true;
                                cb.LastField.EmptyValue = col.EmptyValue;
                            }
                            cb.LastField.PhenomenonOfferingID = col.PhenomenonOfferingID;
                            cb.LastField.PhenomenonUOMID = col.PhenomenonUOMID;
                            break;
                        case "FixedTime":
                            if (!string.IsNullOrEmpty(col.EmptyValue))
                            {
                                cb.LastField.ValueEmpty = true;
                                cb.LastField.EmptyValue = col.EmptyValue;
                            }
                            cb.LastField.PhenomenonOfferingID = col.PhenomenonOfferingID;
                            cb.LastField.PhenomenonUOMID = col.PhenomenonUOMID;
                            if (!string.IsNullOrEmpty(col.FixedTime))
                            {
                                cb.LastField.FixedTime = col.FixedTime;
                            }
                            break;
                    }
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
                    cb.AddField(columnName, typeof(string));
                    if (col == null)
                    {
                        columnsNotInSchema.Add(columnName);
                        cb.LastField.FieldValueDiscarded = true;
                    }
                    else
                    {
                        switch (col.SchemaColumnType.Name)
                        {
                            case "Date":
                                cb.LastField.ValueDate = true;
                                cb.LastField.ValueDateformat = col.Format;
                                break;
                            case "Time":
                                cb.LastField.ValueTime = true;
                                cb.LastField.ValueTimeformat = col.Format;
                                break;
                            case "Ignore":
                                cb.LastField.FieldValueDiscarded = true;
                                break;
                            case "Comment":
                                cb.LastField.ValueComment = true;
                                break;
                            case "Offering":
                                if (!string.IsNullOrEmpty(col.EmptyValue))
                                {
                                    cb.LastField.ValueEmpty = true;
                                    cb.LastField.EmptyValue = col.EmptyValue;
                                }
                                cb.LastField.PhenomenonOfferingID = col.PhenomenonOfferingID;
                                cb.LastField.PhenomenonUOMID = col.PhenomenonUOMID;
                                break;
                            case "FixedTime":
                                if (!string.IsNullOrEmpty(col.EmptyValue))
                                {
                                    cb.LastField.ValueEmpty = true;
                                    cb.LastField.EmptyValue = col.EmptyValue;
                                }
                                cb.LastField.PhenomenonOfferingID = col.PhenomenonOfferingID;
                                cb.LastField.PhenomenonUOMID = col.PhenomenonUOMID;
                                if (!string.IsNullOrEmpty(col.FixedTime))
                                {
                                    cb.LastField.FixedTime = col.FixedTime;
                                }
                                break;
                        }
                    }
                }
                if (columnsNotInSchema.Any())
                {
                    batch.Issues += "Columns in data file but not in schema - " + string.Join(", ", columnsNotInSchema) + Environment.NewLine;

                }
                var columnsNotInDataFile = schema.SchemaColumnRecords().Select(c => c.Name.ToLower()).Except(cb.Fields.Select(f => f.FieldName.ToLower()));
                if (columnsNotInDataFile.Any())
                {
                    batch.Issues += "Columns in schema but not in data file - " + string.Join(", ", columnsNotInDataFile) + Environment.NewLine;
                }
            }
            recordType = cb.CreateRecordClass();
        }
        else
        {
            FixedLengthClassBuilder cb = new FixedLengthClassBuilder(schema.Name, FixedMode.AllowVariableLength);
            cb.IgnoreEmptyLines = true;
            cb.IgnoreFirstLines = schema.IgnoreFirst;
            //if (schema.HasColumnNames.HasValue && schema.HasColumnNames.Value)
            //{
            //    cb.IgnoreFirstLines++;
            //}
            cb.IgnoreLastLines = schema.IgnoreLast;
            if (!String.IsNullOrEmpty(schema.Condition))
                schema.Condition = schema.Condition;
            if (!String.IsNullOrEmpty(schema.SplitSelector))
            {
                cb.SplitSelector = schema.SplitSelector;
                cb.SplitIndex = schema.SplitIndex.Value;
            }
            if (true) // !(schema.HasColumnNames.HasValue && schema.HasColumnNames.Value))
            {
                foreach (var col in schema.SchemaColumnRecords().OrderBy(sc => sc.Number))
                {
                    cb.AddField(col.Name, col.Width.Value, typeof(string));
                    switch (col.SchemaColumnType.Name)
                    {
                        case "Date":
                            cb.LastField.ValueDate = true;
                            cb.LastField.ValueDateformat = col.Format;
                            break;
                        case "Time":
                            cb.LastField.ValueTime = true;
                            cb.LastField.ValueTimeformat = col.Format;
                            break;
                        case "Ignore":
                            cb.LastField.FieldValueDiscarded = true;
                            break;
                        case "Comment":
                            cb.LastField.ValueComment = true;
                            break;
                        case "Offering":
                            if (!string.IsNullOrEmpty(col.EmptyValue))
                            {
                                cb.LastField.ValueEmpty = true;
                                cb.LastField.EmptyValue = col.EmptyValue;
                            }
                            cb.LastField.PhenomenonOfferingID = col.PhenomenonOfferingID;
                            cb.LastField.PhenomenonUOMID = col.PhenomenonUOMID;
                            break;
                        case "FixedTime":
                            if (!string.IsNullOrEmpty(col.EmptyValue))
                            {
                                cb.LastField.ValueEmpty = true;
                                cb.LastField.EmptyValue = col.EmptyValue;
                            }
                            cb.LastField.PhenomenonOfferingID = col.PhenomenonOfferingID;
                            cb.LastField.PhenomenonUOMID = col.PhenomenonUOMID;
                            if (!string.IsNullOrEmpty(col.FixedTime))
                            {
                                cb.LastField.FixedTime = col.FixedTime;
                            }
                            break;
                    }
                }
            }
            else
            {
                // Still to come
            }
            recordType = cb.CreateRecordClass();
        }
        //recordType = ClassBuilder.LoadFromXmlString(schema.DataSchemaX).CreateRecordClass();
        engine = new FileHelperEngine(recordType);
        engine.ErrorMode = ErrorMode.SaveAndContinue;
        List<object> list = engine.ReadStringAsList(data);

        batch.SourceFile = Encoding.Unicode.GetBytes(data);
        docNamePrefix = $"{ds.Name}-{DateTime.Now.ToString("yyyyMMdd HHmmss")}-{Path.GetFileNameWithoutExtension(batch.FileName)}-";
        foreach (var c in Path.GetInvalidFileNameChars())
            docNamePrefix = docNamePrefix.Replace(c, '_');
        foreach (var c in Path.GetInvalidPathChars())
            docNamePrefix = docNamePrefix.Replace(c, '_');
        SaveDocument("Source.txt", data);
        dtResults = engine.ReadStringAsDT(data);
        //Log.Information(dtResults.Dump());
        dtResults.TableName = ds.Name + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
        using (StringWriter sw = new StringWriter())
        {
            dtResults.WriteXml(sw);
            batch.Pass1File = Encoding.Unicode.GetBytes(sw.ToString());
            SaveDocument("Pass1.xml", sw.ToString());
        }
        transformations = new List<DataSourceTransformation>();
        schemaDefs = new List<SchemaDefinition>();

        SchemaValues = new List<SchemaValue>();

        Sensor = obj;

        LogHelper = loghelper;
    }

    public void SaveDocument(string fileName, string text)
    {
        string docPath = HostingEnvironment.MapPath(Path.Combine(WebConfigurationManager.AppSettings["DocumentsPath"], "Uploads"));
        File.WriteAllText(Path.Combine(docPath, docNamePrefix + fileName), text);
    }

    /// <summary>
    /// 
    /// </summary>
    void BuildSchemaDefinition()
    {

        for (int i = 0; i < dtResults.Columns.Count; i++)
        {
            DataColumn dtcol = dtResults.Columns[i];

            SchemaDefinition def = new SchemaDefinition();

            def.Index = i;
            def.FieldName = dtcol.ColumnName;
            var schemaCol = dataSchema.SchemaColumnRecords().FirstOrDefault(sc => sc.Name.Equals(def.FieldName, StringComparison.CurrentCultureIgnoreCase));
            if (schemaCol == null)
                if ((dataSchema.DataSourceTypeID == new Guid(DataSourceType.CSV)) && dataSchema.HasColumnNames.HasValue && dataSchema.HasColumnNames.Value)
                    def.IsIgnored = true;
                else
                {
                    throw new ArgumentNullException($"Unable to find schema column with name {def.FieldName}");
                }
            else
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
                    case "Offering":
                    case "Fixed Time":
                        def.IsOffering = true;
                        def.PhenomenonOfferingID = schemaCol.PhenomenonOfferingID;
                        PhenomenonOffering off = new PhenomenonOffering(def.PhenomenonOfferingID);
                        if (off == null)
                            def.InValidOffering = true;
                        else
                            def.DataSourceTransformationIDs = LoadTransformations(def.PhenomenonOfferingID.Value);
                        def.PhenomenonUOMID = schemaCol.PhenomenonUOMID;
                        PhenomenonUOM uom = new PhenomenonUOM(def.PhenomenonUOMID);
                        if (uom == null)
                            def.InValidUOM = true;
                        if (Sensor != null)
                            def.SensorID = Sensor.Id;
                        else
                        {
                            SensorCollection colsens = new Select()
                                                                  .From(Sensor.Schema)
                                                                  .Where(Sensor.PhenomenonIDColumn).IsEqualTo(off.PhenomenonID)
                                                                  .And(Sensor.DataSourceIDColumn).IsEqualTo(this.dataSource.Id)
                                                                  .ExecuteAsCollection<SensorCollection>();
                            if (colsens.Count() == 0)
                                def.SensorNotFound = true;
                            else
                                def.SensorID = colsens[0].Id;
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

            schemaDefs.Add(def);
        }

        if (schemaDefs.FirstOrDefault(t => t.IsDate) != null && (schemaDefs.FirstOrDefault(t => t.IsTime) != null) || (schemaDefs.FirstOrDefault(t => t.IsFixedTime) != null))
            concatedatetime = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    List<Guid> LoadTransformations(Guid offId)
    {
        List<Guid> transforms = new List<Guid>();

        DataSourceTransformationCollection col = new Select()
                                                .From(DataSourceTransformation.Schema)
                                                .InnerJoin(Phenomenon.IdColumn, DataSourceTransformation.PhenomenonIDColumn)
                                                .InnerJoin(PhenomenonOffering.PhenomenonIDColumn, Phenomenon.IdColumn)
                                                .InnerJoin(TransformationType.IdColumn, DataSourceTransformation.TransformationTypeIDColumn)
                                                .Where(PhenomenonOffering.IdColumn).IsEqualTo(offId)
                                                .And(DataSourceTransformation.DataSourceIDColumn).IsEqualTo(this.dataSource.Id)
                                                .AndExpression(DataSourceTransformation.Columns.StartDate).IsNull()
                                                    .Or(DataSourceTransformation.StartDateColumn).IsLessThanOrEqualTo(DateTime.Now.Date)
                                                .CloseExpression()
                                                .AndExpression(DataSourceTransformation.Columns.EndDate).IsNull()
                                                    .Or(DataSourceTransformation.EndDateColumn).IsGreaterThanOrEqualTo(DateTime.Now)
                                                .CloseExpression()
                                                .OrderAsc(TransformationType.IorderColumn.QualifiedName)
                                                .ExecuteAsCollection<DataSourceTransformationCollection>();

        foreach (var item in col)
        {
            if (transformations.FirstOrDefault(t => t.Id == item.Id) == null)
            {
                transformations.Add(item);
            }

            transforms.Add(item.Id);
        }

        return transforms;
    }

    /// <summary>
    /// 
    /// </summary>
    public void ProcessSchema()
    {
        using (LogContext.PushProperty("Method", "ProcessSchema"))
        {
            Log.Information("Version 1.12");
            try
            {
                BuildSchemaDefinition();

                for (int i = 0; i < dtResults.Rows.Count; i++)
                {
                    DataRow dr = dtResults.Rows[i];
                    ProcessRow(dr);
                }

                //Add

                if (this.LogHelper != null)
                {
                    List<SchemaDefinition> defs = schemaDefs.Where(t => t.IsOffering).ToList();
                    List<DateTime> unprocesseddt = LogHelper.GetUnprocessedDates();


                    for (int b = 0; b < unprocesseddt.Count; b++)
                    {
                        for (int i = 0; i < defs.Count; i++)
                        {
                            SchemaDefinition def = defs[i];

                            SchemaValue rec = new SchemaValue();

                            rec.DateValue = unprocesseddt[b];
                            rec.SensorNotFound = def.SensorNotFound;
                            rec.SensorID = def.SensorID;

                            if (rec.SensorNotFound)
                                rec.InvalidStatuses.Add(Status.SensorNotFound);

                            rec.InValidOffering = def.InValidOffering;
                            rec.PhenomenonOfferingID = def.PhenomenonOfferingID;

                            if (rec.InValidOffering)
                                rec.InvalidStatuses.Add(Status.OfferingInvalid);

                            rec.InValidUOM = def.InValidUOM;
                            rec.PhenomenonUOMID = def.PhenomenonUOMID;
                            if (rec.InValidUOM)
                                rec.InvalidStatuses.Add(Status.UOMInvalid);

                            rec.RawValue = null;
                            rec.DataValue = null;


                            SchemaValues.Add(rec);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unprocessed");
                throw;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dr"></param>
    void ProcessRow(DataRow dr)
    {
        using (LogContext.PushProperty("Method", "ProcessRow"))
        {
            try
            {
                Log.Verbose(dr.Dump());
                DateTime dttme = DateTime.MinValue,
                dt = DateTime.MinValue,
                tm = DateTime.MinValue;
                Boolean ErrorInDate = false;
                Boolean ErrorInTime = false;

                string InvalidDateValue = String.Empty,
                       InvalidTimeValue = String.Empty,
                       RowComment = String.Empty;


                SchemaDefinition dtdef = schemaDefs.FirstOrDefault(t => t.IsDate);
                SchemaDefinition tmdef = schemaDefs.FirstOrDefault(t => t.IsTime);

                Guid correlationID = Guid.NewGuid();

                if (tmdef == null)
                    tmdef = schemaDefs.FirstOrDefault(t => t.IsFixedTime);

                if (dtdef != null)
                {
                    //CultureInfo ci = new CultureInfo(CultureInfo.CurrentCulture.LCID);
                    //ci.Calendar.TwoDigitYearMax = 2000;
                    string sDateValue = dr[dtdef.Index].ToString();
                    if (String.IsNullOrEmpty(sDateValue) || !DateTime.TryParseExact(sDateValue.ToUpper().Trim(), dtdef.Dateformat, null, DateTimeStyles.None, out dt))
                    {
                        ErrorInDate = true;
                        InvalidDateValue = sDateValue;
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
                        }
                    }
                    else if (tmdef.IsFixedTime)
                        tm = DateTime.Now.Date.AddMilliseconds(tmdef.FixedTimeValue.TotalMilliseconds);
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
                    RowComment += dr[df.Index].ToString() + " ";
                }

                for (int i = 0; i < schemaDefs.Count; i++)
                {
                    SchemaDefinition def = schemaDefs[i];

                    if (def.IsOffering)
                    {
                        SchemaValue rec = new SchemaValue();

                        rec.SensorNotFound = def.SensorNotFound;
                        rec.SensorID = def.SensorID;

                        if (rec.SensorNotFound)
                            rec.InvalidStatuses.Add(Status.SensorNotFound);

                        rec.InValidOffering = def.InValidOffering;
                        rec.PhenomenonOfferingID = def.PhenomenonOfferingID;

                        if (rec.InValidOffering)
                            rec.InvalidStatuses.Add(Status.OfferingInvalid);

                        rec.InValidUOM = def.InValidUOM;
                        rec.PhenomenonUOMID = def.PhenomenonUOMID;
                        if (rec.InValidUOM)
                            rec.InvalidStatuses.Add(Status.UOMInvalid);


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
                            rec.DateValue = dttme;
                        else if (!ErrorInDate)
                            rec.DateValue = dt;

                        if (!ErrorInTime)
                            rec.TimeValue = tm;

                        string RawValue = dr[def.Index].ToString();

                        if (!ErrorInDate && LogHelper != null && LogHelper.CheckRecordGap(rec.DateValue))
                        {
                            rec.RawValue = null;
                            rec.DataValue = null;
                        }
                        else if (String.IsNullOrEmpty(RawValue) || def.IsEmptyValue && RawValue.Trim() == def.EmptyValue)
                        {
                            rec.FieldRawValue = RawValue;
                            rec.RawValue = null; // dataSource.DefaultNullValue;
                            rec.DataValue = null; // dataSource.DefaultNullValue;
                            foreach (var transform in transformations.Where(t => def.DataSourceTransformationIDs.Contains(t.Id)))
                            {
                                TransformValue(transform.Id, ref rec, true);
                            }
                        }
                        else
                        {
                            rec.FieldRawValue = RawValue;
                            rec.RawValue = null;
                            Double dvalue = -1;

                            // Possibly do lookups here
                            if (!Double.TryParse(RawValue, out dvalue))
                            {
                                foreach (var transform in transformations.Where(t => t.TransformationType.Name == "Lookup" && def.DataSourceTransformationIDs.Contains(t.Id)))
                                {
                                    TransformValue(transform.Id, ref rec);
                                    if (rec.RawValue.HasValue)
                                        RawValue = rec.RawValue.Value.ToString();
                                }
                            }
                            if (!Double.TryParse(RawValue, out dvalue))
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
                                    Log.Error(ex, "RawValue: {RawValue} DataRow: {Dump}", RawValue, dr.Dump());
                                }
                            }
                            else
                            {
                                rec.RawValue = dvalue;
                                rec.DataValue = rec.RawValue;
                                foreach (var transform in transformations.Where(t => t.TransformationType.Name != "Lookup" && def.DataSourceTransformationIDs.Contains(t.Id)))
                                {
                                    TransformValue(transform.Id, ref rec);
                                }
                            }
                        }

                        if (RowComment.Trim().Length > 0)
                            rec.Comment = RowComment.TrimEnd();
                        rec.CorrelationID = correlationID;
                        //if (rec.DataValue.HasValue)
                        SchemaValues.Add(rec);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DataRow: {Dump}", dr.Dump());
                throw;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void TransformValue(Guid dtid, ref SchemaValue rec, bool isEmpty = false)
    {
        using (LogContext.PushProperty("Method", "TransformValue"))
        {
            try
            {
                bool valid = true;
                var trns = transformations.FirstOrDefault(t => t.Id == dtid);

                bool process = trns.PhenomenonOfferingID.HasValue ? trns.PhenomenonOfferingID.Value == rec.PhenomenonOfferingID : true &&
                        trns.PhenomenonUOMID.HasValue ? trns.PhenomenonUOMID.Value == rec.PhenomenonUOMID : true;

                if (process)
                {
                    if (trns.StartDate.HasValue && (rec.DateValue < trns.StartDate.Value))
                        process = false;
                    if (trns.EndDate.HasValue && (rec.DateValue > trns.EndDate.Value))
                        process = false;
                }

                //if ((trns.TransformationTypeID.ToString() != TransformationType.Lookup) && !rec.RawValue.HasValue) process = false;

                if (!process)
                {
                    rec.DataValue = rec.RawValue;
                    return;
                }

                if (!isEmpty)
                {
                    if (trns.TransformationTypeID.ToString() == TransformationType.CorrectionValues)
                    {
                        Dictionary<string, string> corrvals = trns.CorrectionValues;

                        if (corrvals.ContainsKey("eq"))
                        {
                            //string eq = corrvals["equation"].Replace("\"","").Replace("\"","");
                            Expression exp = new Expression(corrvals["eq"]);
                            exp.Parameters["value"] = rec.RawValue;
                            object val = exp.Evaluate();
                            rec.DataValue = Double.Parse(val.ToString());
                        }
                    }
                    else if (trns.TransformationTypeID.ToString() == TransformationType.RatingTable)
                    {

                        rec.DataValue = trns.GetRatingValue(rec.RawValue.Value);
                    }
                    else if (trns.TransformationTypeID.ToString() == TransformationType.QualityControlValues)
                    {
                        if (!rec.DataValue.HasValue)
                            rec.DataValue = rec.RawValue;

                        Dictionary<string, Double> qv = trns.QualityValues;
                        if (qv.ContainsKey("min") && rec.DataValue.Value < qv["min"])
                            valid = false;

                        if (qv.ContainsKey("max") && rec.DataValue.Value > qv["max"])
                            valid = false;

                        if (!valid)
                        {
                            rec.InvalidStatuses.Add(Status.TransformValueInvalid);
                            rec.DataSourceTransformationID = trns.Id;

                            rec.DataValueInvalid = true;
                            rec.InvalidDataValue = rec.DataValue.ToString();
                        }
                    }
                    else if (trns.TransformationTypeID.ToString() == TransformationType.Lookup)
                    {

                        var qv = trns.LookupValues;
                        if (!qv.ContainsKey(rec.FieldRawValue))
                            valid = false;
                        else
                        {
                            rec.RawValue = trns.LookupValues[rec.FieldRawValue];
                            rec.DataValue = rec.RawValue;
                        }

                        if (!valid)
                        {
                            rec.InvalidStatuses.Add(Status.TransformValueInvalid);
                            rec.DataSourceTransformationID = trns.Id;

                            rec.DataValueInvalid = true;
                            rec.InvalidDataValue = rec.DataValue.ToString();
                        }
                    }
                }
                //Set new offering/UOM
                if (trns.NewPhenomenonOfferingID.HasValue)
                {
                    rec.RawPhenomenonOfferingID = trns.PhenomenonOfferingID;
                    rec.PhenomenonOfferingID = trns.NewPhenomenonOfferingID;
                }
                if (trns.NewPhenomenonUOMID.HasValue)
                {
                    rec.RawPhenomenonUOMID = trns.PhenomenonUOMID;
                    rec.PhenomenonUOMID = trns.NewPhenomenonUOMID;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "dtid: {dtid} rec: {@rec})", dtid, rec);
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
                _errors.Add(new { ErrorMessage = errinfo.ExceptionInfo.Message, LineNo = errinfo.LineNumber, RecordString = errinfo.RecordString });
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
        if (disposed) return;
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

        String Result = String.Empty;

        if (!String.IsNullOrEmpty(ds.SplitSelector))
        {
            StringBuilder sb = new StringBuilder();

            string line;

            int index = 0;

            while ((line = reader.ReadLine()) != null && index <= ds.SplitIndex)
            {
                if (line.StartsWith(ds.SplitSelector))
                    index++;

                if (index == ds.SplitIndex)
                {
                    sb.AppendLine(line);
                    break;
                }
            }

            while ((line = reader.ReadLine()) != null && index <= ds.SplitIndex)
            {
                if (line.StartsWith(ds.SplitSelector))
                    index++;

                if (index <= ds.SplitIndex)
                {
                    sb.AppendLine(line);
                }
            }

            Result = sb.ToString();
        }
        else
            Result = reader.ReadToEnd();

        return Result;
    }

}

public class SchemaDefinition
{

    public SchemaDefinition()
    {
        this.DataSourceTransformationIDs = new List<Guid>();
    }

    public int Index { get; set; }
    public string FieldName { get; set; }
    public Boolean IsDate { get; set; }
    public string Dateformat { get; set; }
    public Boolean IsTime { get; set; }
    public string Timeformat { get; set; }
    public bool IsFixedTime { get; set; }
    public TimeSpan FixedTimeValue { get; set; }
    public Boolean IsIgnored { get; set; }
    public Guid? PhenomenonOfferingID { get; set; }
    public Boolean InValidOffering { get; set; }
    public Guid? PhenomenonUOMID { get; set; }
    public Boolean InValidUOM { get; set; }
    public List<Guid> DataSourceTransformationIDs { get; set; }
    public Boolean IsEmptyValue { get; set; }
    public string EmptyValue { get; set; }
    public Boolean IsOffering { get; set; }

    public bool IsComment { get; set; }
    public Guid? SensorID { get; set; }
    public bool SensorNotFound { get; set; }
}

/// <summary>
/// 
/// </summary>
public class SchemaValue
{

    public SchemaValue()
    {
        this.InvalidStatuses = new List<string>();
        this.Comment = String.Empty;
    }

    public DateTime DateValue { get; set; }
    public DateTime? TimeValue { get; set; }
    public string InvalidDateValue { get; set; }
    public string InvalidTimeValue { get; set; }
    public Boolean DateValueInvalid { get; set; }
    public Boolean TimeValueInvalid { get; set; }
    public Guid? PhenomenonOfferingID { get; set; }
    public Boolean InValidOffering { get; set; }
    public Guid? PhenomenonUOMID { get; set; }
    public Boolean InValidUOM { get; set; }
    public double? RawValue { get; set; }
    public double? DataValue { get; set; }
    public Boolean RawValueInvalid { get; set; }
    public string InvalidRawValue { get; set; }
    public Boolean DataValueInvalid { get; set; }
    public string InvalidDataValue { get; set; }
    public List<string> InvalidStatuses { get; set; }
    public Guid? DataSourceTransformationID { get; set; }
    public Guid? SensorID { get; set; }
    public bool SensorNotFound { get; set; }
    public string FieldRawValue { get; set; }
    public string Comment { get; set; }

    public Guid? RawPhenomenonOfferingID { get; set; }
    public Guid? RawPhenomenonUOMID { get; set; }

    public Guid CorrelationID { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool IsValid
    {
        get
        {
            return !DateValueInvalid &&
                   !TimeValueInvalid &&
                   !InValidOffering &&
                   !InValidUOM &&
                   !RawValueInvalid &&
                   !DataValueInvalid &&
                   !SensorNotFound;
        }
    }
}

