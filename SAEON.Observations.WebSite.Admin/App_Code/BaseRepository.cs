using Ext.Net;
using Newtonsoft.Json;
using SAEON.Core;
using SAEON.Logs;
using SubSonic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading;
using System.Web;

/// <summary>
/// Summary description for BaseRepository
/// </summary>
public class BaseRepository
{
    public BaseRepository()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    private static void GetQuery(ref SqlQuery q, StoreRefreshDataEventArgs e, string filters)
    {
        if (!string.IsNullOrEmpty(filters))
        {
            FilterConditions fc = new FilterConditions(filters);

            foreach (FilterCondition condition in fc.Conditions)
            {
                switch (condition.FilterType)
                {
                    case FilterType.Date:
                        switch (condition.Comparison.ToString())
                        {
                            case "Eq":
                                q.And(condition.Name).IsEqualTo(condition.Value);

                                break;
                            case "Gt":
                                q.And(condition.Name).IsGreaterThanOrEqualTo(condition.Value);

                                break;
                            case "Lt":
                                q.And(condition.Name).IsLessThanOrEqualTo(condition.Value);

                                break;
                            default:
                                break;
                        }

                        break;

                    case FilterType.Numeric:
                        switch (condition.Comparison.ToString())
                        {
                            case "Eq":
                                q.And(condition.Name).IsEqualTo(condition.Value);

                                break;
                            case "Gt":
                                q.And(condition.Name).IsGreaterThanOrEqualTo(condition.Value);

                                break;
                            case "Lt":
                                q.And(condition.Name).IsLessThanOrEqualTo(condition.Value);

                                break;
                            default:
                                break;
                        }

                        break;

                    case FilterType.String:
                        q.And(condition.Name).Like("%" + condition.Value + "%");


                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }

        }

        if (!string.IsNullOrEmpty(e.Sort))
        {
            if (e.Dir == Ext.Net.SortDirection.DESC)
            {
                q.OrderDesc(e.Sort);
            }
            else
            {
                q.OrderAsc(e.Sort);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="q"></param>
    /// <param name="e"></param>
    /// <param name="paramPrefix"></param>
    /// <returns></returns>
    public static void GetPagedQuery(ref SqlQuery q, StoreRefreshDataEventArgs e, string filters)
    {
        using (SAEONLogs.MethodCall(typeof(BaseRepository)))
        {
            GetQuery(ref q, e, filters);
            int total = q.GetRecordCount();
            int currentPage = (e.Start / e.Limit) + 1;
            SAEONLogs.Verbose("e.Limit: {Limit} e.Start: {Start} e.Total: {Total} CurrentPage: {CurrentPage} Total: {Total}", e.Limit, e.Start, e.Total, currentPage, total);
            e.Total = total;
            if (e.Limit > e.Total)
                q.Paged(currentPage, e.Total);
            else
                q.Paged(currentPage, e.Limit);
            SAEONLogs.Verbose("Sql: {sql}", q.BuildSqlStatement());
        }
    }

    public static void GetUnpagedQuery(ref SqlQuery q, StoreRefreshDataEventArgs e, string filters)
    {
        using (SAEONLogs.MethodCall(typeof(BaseRepository)))
        {
            GetQuery(ref q, e, filters);
            SAEONLogs.Verbose("Sql: {sql}", q.BuildSqlStatement());
        }
    }

    //public enum ExportTypes { Csv, Excel };

    public static void Export(SqlQuery query, string visCols, string exportType, string fileName, HttpResponse response, Action<DataTable> doSAEONLogs = null)
    {
        using (SAEONLogs.MethodCall(typeof(BaseRepository), new MethodCallParameters { { "Columns", visCols }, { "ExportType", exportType }, { "FileName", fileName } }))
        {
            var result = -1;
            try
            {
                var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(visCols);
                var colNames = new List<string>();
                var colCaptions = new List<string>();
                foreach (var item in values)
                {
                    if (!string.IsNullOrWhiteSpace(item.Value) && !string.IsNullOrWhiteSpace(item.Key))
                    {
                        colNames.Add(item.Value);
                        colCaptions.Add(item.Key.Replace(" ", "").Replace("/", ""));
                    }
                }
                SAEONLogs.Verbose("Sql: {sql}", query.BuildSqlStatement());
                DataTable dt = query.ExecuteDataSet().Tables[0];
                result = dt.Rows.Count;
                for (int k = 0; k < dt.Columns.Count; k++)
                {
                    for (int j = 0; j < colNames.Count; j++)
                    {
                        if (colNames[j].Equals(dt.Columns[k].ColumnName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            dt.Columns[k].Caption = colCaptions[j];
                        }
                    }
                }
                doSAEONLogs?.Invoke(dt);
                response.Clear();
                byte[] bytes = null;
                switch (exportType)
                {
                    case "csv": //ExportTypes.Csv:
                        response.ContentType = "text/csv";
                        var UTF16 = ConfigurationManager.AppSettings["UTF16CSVs"].IsTrue();
                        response.Charset = UTF16 ? "UTF-16" : "UTF-8";
                        response.AddHeader("Content-Disposition", $"attachment; filename={fileName}.csv");
                        //bytes = Encoding.UTF8.GetBytes(dt.ToList().ToCSV());
                        bytes = dt.ToCSV(UTF16);
                        break;
                    case "exc": //ExportTypes.Excel
                        response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        response.AddHeader("Content-Disposition", $"attachment; filename={fileName}.xlsx");
                        //bytes = dt.ToList().ToExcel();
                        bytes = dt.ToExcel();
                        break;
                }
                response.AddHeader("Content-Length", bytes.Length.ToString());
                response.BinaryWrite(bytes);
                response.Flush();
                response.End();
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                throw;
            }
        }
    }

    public static void Export(string tableName, string filters, string visCols, string sortCol, string sortDir, string exportType, string fileName, HttpResponse response)
    {
        using (SAEONLogs.MethodCall(typeof(BaseRepository), new MethodCallParameters { { "TableName", tableName }, { "Filters", filters }, { "Columns", visCols }, { "SortBy", sortCol },
            { "SortDir", sortDir }, { "ExportType", exportType }, { "FileName", fileName } }))
        {
            try
            {
                var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(visCols);
                var colNames = new List<string>();
                foreach (var item in values)
                {
                    if (!string.IsNullOrWhiteSpace(item.Value) && !string.IsNullOrWhiteSpace(item.Key))
                    {
                        colNames.Add(item.Value);
                    }
                }
                SqlQuery q = new Select(colNames.ToArray()).From(tableName);
                if (!string.IsNullOrEmpty(filters))
                {
                    FilterConditions fc = new FilterConditions(filters);
                    foreach (FilterCondition condition in fc.Conditions)
                    {
                        switch (condition.FilterType)
                        {
                            case FilterType.Date:
                                switch (condition.Comparison.ToString())
                                {
                                    case "Eq":
                                        q.And(condition.Name).IsEqualTo(condition.Value);

                                        break;
                                    case "Gt":
                                        q.And(condition.Name).IsGreaterThanOrEqualTo(condition.Value);

                                        break;
                                    case "Lt":
                                        q.And(condition.Name).IsLessThanOrEqualTo(condition.Value);

                                        break;
                                    default:
                                        break;
                                }
                                break;

                            case FilterType.Numeric:
                                switch (condition.Comparison.ToString())
                                {
                                    case "Eq":
                                        q.And(condition.Name).IsEqualTo(condition.Value);

                                        break;
                                    case "Gt":
                                        q.And(condition.Name).IsGreaterThanOrEqualTo(condition.Value);

                                        break;
                                    case "Lt":
                                        q.And(condition.Name).IsLessThanOrEqualTo(condition.Value);

                                        break;
                                    default:
                                        break;
                                }
                                break;

                            case FilterType.String:
                                q.And(condition.Name).Like("%" + condition.Value + "%");

                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
                if (!(string.IsNullOrEmpty(sortCol) && string.IsNullOrEmpty(sortDir)))
                {
                    if (sortDir.ToLower() == Ext.Net.SortDirection.DESC.ToString().ToLower())
                    {
                        q.OrderDesc(sortCol);
                    }
                    else
                    {
                        q.OrderAsc(sortCol);
                    }
                }
                Export(q, visCols, exportType, fileName, response);
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                throw;
            }
        }
    }

}