using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Ext.Net;
using Newtonsoft.Json;
using SAEON.Core;
using SAEON.Logs;
using SAEON.OpenXML;
using SubSonic;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Xsl;


public static class DataTableExtensions
{
    public static byte[] ToCsv(this DataTable dataTable)
    {
        var sb = new StringBuilder();
        IEnumerable<String> headerValues = dataTable
            .Columns
            .OfType<DataColumn>()
            .Select(column => column.ColumnName);
        sb.AppendLine(String.Join(",", headerValues));
        foreach (DataRow row in dataTable.Rows)
        {
            var values = new List<string>();
            foreach (DataColumn col in dataTable.Columns)
            {
                if (row.IsNull(col))
                {
                    values.Add(string.Empty);
                }
                else
                {
                    var v = row[col];
                    if ((v is string) || (v is Guid))
                    {
                        values.Add(v.ToString().DoubleQuoted());
                    }
                    //else if (v is DateTime date)
                    else if (v is DateTime)
                    {
                        //values.Add(date.ToString("o"));
                        //values.Add(((DateTime)v).ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ssK"));
                        //values.Add(((DateTime)v).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                        values.Add(((DateTime)v).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ssK"));
                    }
                    else
                    {
                        values.Add(v.ToString());
                    }
                }
            }
            sb.AppendLine(string.Join(",", values));
        }
        return Encoding.Unicode.GetBytes(sb.ToString());
    }

    public static byte[] ToExcel(this DataTable dataTable)
    {
        using (var ms = new MemoryStream())
        {
            using (var doc = ExcelHelper.CreateSpreadsheet(ms))
            {
                WorksheetPart wsp = ExcelHelper.GetWorksheetPart(doc, 1);
                int r = 1;
                int c = 1;
                foreach (DataColumn col in dataTable.Columns)
                {
                    ExcelHelper.SetCellValue(doc, wsp, c++, r, col.ColumnName);
                }
                foreach (DataRow row in dataTable.Rows)
                {
                    r++;
                    c = 1;
                    foreach (DataColumn col in dataTable.Columns)
                    {
                        ExcelHelper.SetCellValue(doc, wsp, c++, r, row[col]);
                    }
                }
            }
            return ms.ToArray();
        }
    }
}


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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="q"></param>
    /// <param name="e"></param>
    /// <param name="paramPrefix"></param>
    /// <returns></returns>
    public static void GetPagedQuery(ref SqlQuery q, StoreRefreshDataEventArgs e, string filters)
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

        int total = q.GetRecordCount();

        e.Total = total;


        int currenPage = e.Start / e.Limit + 1;

        if (e.Limit > total)
            q.Paged(currenPage, total);
        else
            q.Paged(currenPage, e.Limit);

    }

    //public enum ExportTypes { Csv, Excel };

    public static void Export(SqlQuery query, string visCols, string exportType, string fileName)
    {
        using (Logging.MethodCall(typeof(BaseRepository), new ParameterList { { "Columns", visCols }, { "ExportType", exportType }, { "FileName", fileName } }))
        {
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
                DataTable dt = query.ExecuteDataSet().Tables[0];
                for (int k = 0; k < dt.Columns.Count; k++)
                {
                    for (int j = 0; j < colNames.Count; j++)
                    {
                        if (colNames[j].ToLower() == dt.Columns[k].ColumnName.ToLower())
                        {
                            dt.Columns[k].Caption = colCaptions[j];
                        }
                    }
                }
                HttpContext.Current.Response.Clear();
                byte[] bytes;
                switch (exportType)
                {
                    case "csv": //ExportTypes.Csv:
                        HttpContext.Current.Response.ContentType = "text/csv";
                        HttpContext.Current.Response.Charset = "UNICODE";
                        HttpContext.Current.Response.AddHeader("Content-Disposition", $"attachment; filename={fileName}.csv");
                        bytes = dt.ToCsv();
                        HttpContext.Current.Response.AddHeader("Content-Length", bytes.Length.ToString());
                        HttpContext.Current.Response.BinaryWrite(bytes);
                        break;
                    case "exc": //ExportTypes.Excel
                        HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        HttpContext.Current.Response.AddHeader("Content-Disposition", $"attachment; filename={fileName}.xlsx");
                        bytes = dt.ToExcel();
                        HttpContext.Current.Response.AddHeader("Content-Length", bytes.Length.ToString());
                        HttpContext.Current.Response.BinaryWrite(bytes);
                        break;
                }
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
                throw;
            }
        }
    }

    public static void Export(string tableName, string filters, string visCols, string sortCol, string sortDir, string exportType, string fileName)
    {
        using (Logging.MethodCall(typeof(BaseRepository), new ParameterList { { "TableName", tableName }, { "Filters", filters }, { "Columns", visCols }, { "SortBy", sortCol },
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
                Export(q, visCols, exportType, fileName);
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
                throw;
            }
        }
    }

    //public static string BuildExportQ(string TableName, string json, string visCols, string sortCol, string sortDir)
    //{
    //    Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(visCols);

    //    List<string> colmsL = new List<string>();
    //    List<string> colmsDisplayNamesL = new List<string>();

    //    foreach (var item in values)
    //    {
    //        if (!string.IsNullOrWhiteSpace(item.Value) && !string.IsNullOrWhiteSpace(item.Key))
    //        {
    //            colmsL.Add(item.Value);
    //            colmsDisplayNamesL.Add(item.Key.Replace(" ", "").Replace("/", ""));
    //        }
    //    }

    //    string[] colms = colmsL.ToArray();
    //    string[] colmsDisplayNames = colmsDisplayNamesL.ToArray();

    //    SqlQuery q = new Select(colms).From(TableName);

    //    #region filters
    //    if (!string.IsNullOrEmpty(json))
    //    {
    //        FilterConditions fc = new FilterConditions(json);
    //        foreach (FilterCondition condition in fc.Conditions)
    //        {
    //            switch (condition.FilterType)
    //            {
    //                case FilterType.Date:
    //                    switch (condition.Comparison.ToString())
    //                    {
    //                        case "Eq":
    //                            q.And(condition.Name).IsEqualTo(condition.Value);

    //                            break;
    //                        case "Gt":
    //                            q.And(condition.Name).IsGreaterThanOrEqualTo(condition.Value);

    //                            break;
    //                        case "Lt":
    //                            q.And(condition.Name).IsLessThanOrEqualTo(condition.Value);

    //                            break;
    //                        default:
    //                            break;
    //                    }
    //                    break;

    //                case FilterType.Numeric:
    //                    switch (condition.Comparison.ToString())
    //                    {
    //                        case "Eq":
    //                            q.And(condition.Name).IsEqualTo(condition.Value);

    //                            break;
    //                        case "Gt":
    //                            q.And(condition.Name).IsGreaterThanOrEqualTo(condition.Value);

    //                            break;
    //                        case "Lt":
    //                            q.And(condition.Name).IsLessThanOrEqualTo(condition.Value);

    //                            break;
    //                        default:
    //                            break;
    //                    }
    //                    break;

    //                case FilterType.String:
    //                    q.And(condition.Name).Like("%" + condition.Value + "%");

    //                    break;
    //                default:
    //                    throw new ArgumentOutOfRangeException();
    //            }
    //        }
    //    }

    //    if (!(string.IsNullOrEmpty(sortCol) && string.IsNullOrEmpty(sortDir)))
    //    {
    //        if (sortDir.ToLower() == Ext.Net.SortDirection.DESC.ToString().ToLower())
    //        {
    //            q.OrderDesc(sortCol);
    //        }
    //        else
    //        {
    //            q.OrderAsc(sortCol);
    //        }
    //    }
    //    #endregion filters

    //    try
    //    {
    //        DataTable dt = q.ExecuteDataSet().Tables[0];
    //        for (int k = 0; k < dt.Columns.Count; k++)
    //        {
    //            for (int j = 0; j < colms.Length; j++)
    //            {
    //                if (colms[j].ToLower() == dt.Columns[k].ToString().ToLower())
    //                {
    //                    dt.Columns[k].ColumnName = colmsDisplayNames[j];
    //                }
    //            }
    //        }

    //        JavaScriptSerializer ser = new JavaScriptSerializer();
    //        ser.MaxJsonLength = 2147483647;

    //        string js = JsonConvert.SerializeObject(dt);

    //        return js;
    //    }
    //    catch (Exception ex)
    //    {
    //        Logging.Exception(ex);
    //        throw;
    //    }
    //}

    //public static void doExport(string type, string js)
    //{
    //    #region doExport
    //    System.Web.Script.Serialization.JavaScriptSerializer oSerializer =
    //    new System.Web.Script.Serialization.JavaScriptSerializer();
    //    StoreSubmitDataEventArgs eSubmit = new StoreSubmitDataEventArgs(js, null);
    //    XmlNode xml = eSubmit.Xml;
    //    HttpContext.Current.Response.Clear();

    //    switch (type)
    //    {
    //        //case "xml":
    //        //    string strXml = xml.OuterXml;
    //        //    this.Response.AddHeader("Content-Disposition", "attachment; filename=submittedData.xml");
    //        //    this.Response.AddHeader("Content-Length", strXml.Length.ToString());
    //        //    this.Response.ContentType = "application/xml";
    //        //    this.Response.Write(strXml);
    //        //    break;

    //        case "exc":
    //            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
    //            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=Export.xls");
    //            XslCompiledTransform xtExcel = new XslCompiledTransform();
    //            xtExcel.Load(HttpContext.Current.Server.MapPath("../Templates/Excel.xsl"));
    //            xtExcel.Transform(xml, null, HttpContext.Current.Response.OutputStream);
    //            break;

    //        case "csv":
    //            //HttpContext.Current.Response.ContentType = "application/octet-stream";
    //            HttpContext.Current.Response.ContentType = "text/csv";
    //            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=Export.csv");
    //            XslCompiledTransform xtCsv = new XslCompiledTransform();
    //            xtCsv.Load(HttpContext.Current.Server.MapPath("../Templates/Csv.xsl"));
    //            xtCsv.Transform(xml, null, HttpContext.Current.Response.OutputStream);
    //            break;
    //    }
    //    HttpContext.Current.Response.End();
    //    #endregion
    //}
}