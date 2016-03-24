using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FileHelpers;
using FileHelpers.Dynamic;


/// <summary>
/// 
/// </summary>
public class ImportLogHelper
{

    public List<DateTime> RecordGaps { get; set; }
    public List<DateTime> DataGaps { get; set; }


    public List<DateTime> FoundRecordGaps { get; set; }
    public List<DateTime> FoundDataGaps { get; set; }

    public ImportLogHelper()
    {
        RecordGaps = new List<DateTime>();
        DataGaps = new List<DateTime>();

        FoundRecordGaps = new List<DateTime>();
        FoundDataGaps = new List<DateTime>();

    }

    /// <summary>
    /// 
    /// </summary>
    public void ReadLog(string LogData)
    {
        MultiRecordEngine engine;

        engine = new MultiRecordEngine(typeof(InterRecordGap),
                                        typeof(DataRecordGap),
                                        typeof(RecordIgnoredGap));
        engine.RecordSelector = new RecordTypeSelector(LogTypeSelector);

        engine.ErrorMode = ErrorMode.IgnoreAndContinue;

        object[] recs = engine.ReadString(LogData);

        foreach (object rec in recs.Where(t => t.GetType() == typeof(InterRecordGap) || t.GetType() == typeof(DataRecordGap)))
        {
            if (rec.GetType() == typeof(InterRecordGap))
                RecordGaps.AddRange(((InterRecordGap)rec).GetList());
            else
                DataGaps.AddRange(((DataRecordGap)rec).GetList());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="record"></param>
    /// <returns></returns>
    Type LogTypeSelector(MultiRecordEngine engine, string record)
    {
        if (record.Length == 0)
            return null;

        if (record.ToUpper().StartsWith("INTER-RECORD GAP"))
            return typeof(InterRecordGap);
        else if (record.ToUpper().StartsWith("DATA GAP"))
            return typeof(DataRecordGap);
        else
            return typeof(RecordIgnoredGap);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool CheckRecordGap(DateTime dt)
    {
        bool Result = false;
        DateTime dts = this.RecordGaps.FirstOrDefault(t => t.Year == dt.Year && t.Month == dt.Month && t.Day == dt.Day && t.Hour == dt.Hour && t.Minute == dt.Minute);
        DateTime dts2 = this.DataGaps.FirstOrDefault(t => t.Year == dt.Year && t.Month == dt.Month && t.Day == dt.Day && t.Hour == dt.Hour && t.Minute == dt.Minute);
        if (dts != DateTime.MinValue || dts2 != DateTime.MinValue)
        {
            if (dts != DateTime.MinValue)
                FoundRecordGaps.Add(dts);

            if(dts2 != DateTime.MinValue)
                FoundDataGaps.Add(dts);

            Result = true;
        }     
        else
            Result = false;

        return Result;
    }

    public List<DateTime> GetUnprocessedDates()
    {
        List<DateTime> uDates = new List<DateTime>();

        this.RecordGaps.Except(FoundRecordGaps).ToList().ForEach(t => uDates.Add(t));
        this.DataGaps.Except(FoundDataGaps).ToList().ForEach(t => uDates.Add(t));

        return uDates.Distinct().ToList();
    }
}

/// <summary>
/// 
/// </summary>
[FixedLengthRecord(FixedMode.AllowVariableLength)]
public class InterRecordGap
{
    [FieldFixedLength(21)]
    [FieldValueDiscarded]
    public string RecordType;

    [FieldFixedLength(11)]
    [FieldConverter(ConverterKind.Date, "ddMMyy HHmm")]
    public DateTime StartDate;

    [FieldFixedLength(4)]
    [FieldValueDiscarded()]
    public string IgnoreTo;

    [FieldFixedLength(11)]
    [FieldConverter(ConverterKind.Date, "ddMMyy HHmm")]
    public DateTime EndDate;


    public List<DateTime> GetList()
    {
        List<DateTime> dataList = new List<DateTime>();
        dataList.Add(this.StartDate);

        DateTime dt = this.StartDate;

        while (dt.AddHours(1) < this.EndDate)
        {
            dt = dt.AddHours(1);
            DateTime fixdt = dt.Date.AddHours(dt.TimeOfDay.Hours);
            dataList.Add(fixdt);
        }

        return dataList;
    }
}

/// <summary>
/// 
/// </summary>
[FixedLengthRecord(FixedMode.AllowVariableLength)]
public class DataRecordGap
{
    [FieldFixedLength(13)]
    [FieldValueDiscarded]
    public string RecordType;

    [FieldFixedLength(11)]
    [FieldConverter(ConverterKind.Date, "ddMMyy HHmm")]
    public DateTime StartDate;

    [FieldFixedLength(4)]
    [FieldValueDiscarded()]
    public string IgnoreTo;

    [FieldFixedLength(11)]
    [FieldConverter(ConverterKind.Date, "ddMMyy HHmm")]
    public DateTime EndDate;

    public List<DateTime> GetList()
    {
        List<DateTime> dataList = new List<DateTime>();
        dataList.Add(this.StartDate);

        DateTime dt = this.StartDate;

        while (dt.AddHours(1) < this.EndDate)
        {
            dt = dt.AddHours(1);
            DateTime fixdt = dt.Date.AddHours(dt.TimeOfDay.Hours);
            dataList.Add(fixdt);
        }

        return dataList;
    }
}

[FixedLengthRecord(FixedMode.AllowVariableLength)]
public class RecordIgnoredGap
{
    [FieldFixedLength(75)]
    [FieldValueDiscarded]
    public string IgnoredType;

}
