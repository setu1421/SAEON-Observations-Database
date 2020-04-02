using Ext.Net;
using System;
using System.Collections.Generic;
using System.Data;

public static class TextFieldExtensions
{
    public static bool HasValue(this TextField field)
    {
        field.Text = field.Text.Trim();
        return !field.IsEmpty;
    }
}

public static class TextAreaExtensions
{
    public static bool HasValue(this TextArea field)
    {
        field.Text = field.Text.Trim();
        return !field.IsEmpty;
    }
}

public static class DateFieldExtensions
{
    public static bool HasValue(this DateField field)
    {
        return !field.IsEmpty && (field.SelectedDate.Year >= 1900);
    }
}

public static class HiddenExtensions
{
    public static bool HasValue(this Hidden field)
    {
        field.Text = field.Text.Trim();
        return !field.IsEmpty;
    }
}

public static class StringExtensions
{
    public static bool IsQuoted(this string value)
    {
        return (value.StartsWith("\"") && value.EndsWith("\""));
    }

    public static string RemoveQuotes(this string value)
    {
        if (!value.IsQuoted())
        {
            return value;
        }
        else
        {
            value = value.Remove(0, 1);
            value = value.Remove(value.Length - 1, 1);
            return value;
        }
    }

    public static bool IsTrue(this string value)
    {
        if (bool.TryParse(value, out bool result))
            return result;
        else
            return false;
    }
}

public static class DataRowExtensions
{
    public static string AsString(this DataRow dataRow, bool oneLine = true)
    {
        List<string> result = new List<string>();
        foreach (DataColumn col in dataRow.Table.Columns)
            result.Add($"{col.ColumnName}: {dataRow[col.ColumnName]}");
        return string.Join(oneLine ? "; " : Environment.NewLine, result);
    }

    public static T GetValue<T>(this DataRow dataRow, string columnName)
    {
        if (dataRow.IsNull(columnName))
            return default;
        else
            return (T)dataRow[columnName];
    }

    public static T SetValue<T>(this DataRow dataRow, string columnName, T value)
    {
        if (value == null)
            dataRow[columnName] = DBNull.Value;
        else
            dataRow[columnName] = value;
        return value;
    }
}

public static class TimeSpanExtensions
{
    public static string TimeStr(this TimeSpan timeSpan)
    {
        if (timeSpan.Days > 0)
            return $"{timeSpan.Days}d {timeSpan.Hours}h {timeSpan.Minutes}m {timeSpan.Seconds}.{timeSpan.Milliseconds:D3}s";
        else if (timeSpan.Hours > 0)
            return $"{timeSpan.Hours}h {timeSpan.Minutes}m {timeSpan.Seconds}.{timeSpan.Milliseconds:D3}s";
        else if (timeSpan.Minutes > 0)
            return $"{timeSpan.Minutes}m {timeSpan.Seconds}.{timeSpan.Milliseconds:D3}s";
        else
            return $"{timeSpan.Seconds}.{timeSpan.Milliseconds:D3}s";
    }
}

