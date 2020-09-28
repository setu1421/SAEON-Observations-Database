using SAEON.Logs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Transactions;

/// <summary>
/// Summary description for Utilities
/// </summary>
public static class Utilities
{
    public static Guid MakeGuid(object value)
    {
        return (value == null) || ((value is string @string) && string.IsNullOrEmpty(@string)) ? Guid.Empty : new Guid(value.ToString());
    }

    public static string NullIfEmpty(string value)
    {
        value = value?.Trim();
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }
        else
        {
            return value;
        }
    }

    public static string Dump(this DataRow dataRow, bool oneLine = true)
    {
        List<string> result = new List<string>();
        foreach (DataColumn col in dataRow.Table.Columns)
        {
            result.Add($"{col.ColumnName}: {dataRow[col.ColumnName]}");
        }

        return string.Join(oneLine ? "; " : "<br />", result);
    }

    public static string Dump(this DataTable dataTable, string name = "")
    {
        List<string> result = new List<string>();
        if (!string.IsNullOrEmpty(name))
        {
            result.Add(name);
        }

        foreach (DataRow row in dataTable.Rows)
        {
            result.Add(row.Dump());
        }

        return string.Join("<br />", result);
    }

    public static TransactionScope NewTransactionScope()
    {
        var timeout = TimeSpan.Parse(ConfigurationManager.AppSettings["TransactionTimeout"]);
        SAEONLogs.Information("Transaction: Config {config} Default {Default}", timeout, TransactionManager.DefaultTimeout);
        return new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted, Timeout = timeout });
    }

    public static NumberFormatInfo NumberFormat { get; private set; } = new NumberFormatInfo { NumberDecimalSeparator = "." };

    public static double ParseDouble(string value)
    {
        return double.Parse(value, NumberFormat);
    }

    public static bool TryParseDouble(string value, out double result)
    {
        return double.TryParse(value, NumberStyles.Float, NumberFormat, out result);
    }

}