using SAEON.Observations.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using SubSonic;
using ext = Ext.Net;
using System.Data;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for ItemRepository
/// </summary>
public class ViewRepository
{
    public ViewRepository()
    {
    }

    public static DataTable GetSchema(string View)
    {
        return SPs.ExecuteView(View,true,false, "", "", 1, 20, "").GetDataSet().Tables[0];
    }

    public static DataTable GetPagedSet(ref ext.StoreRefreshDataEventArgs e, string paramPrefix,string View)
    {

        string s = paramPrefix;
        String FilterColumn = String.Empty;
        List<string> Filters = new List<string>();

        if (!string.IsNullOrEmpty(s))
        {
            ext.FilterConditions fc = new ext.FilterConditions(s);

            foreach (ext.FilterCondition condition in fc.Conditions)
            {
                switch (condition.FilterType)
                {
                    case ext.FilterType.Date:
                    case ext.FilterType.Numeric:
                        switch (condition.Comparison)
                        {
                            case Ext.Net.Comparison.Eq:
                                Filters.Add(String.Concat("[", condition.Name, "] = ", condition.Value));
                                break;
                            case Ext.Net.Comparison.Gt:
                                Filters.Add(String.Concat("[", condition.Name, "] >= ", condition.Value));
                                break;
                            case Ext.Net.Comparison.Lt:
                                Filters.Add(String.Concat("[", condition.Name, "] <= ", condition.Value));
                                break;
                            default:
                                break;
                        }
                        break;

                    case ext.FilterType.String:
                        Filters.Add(String.Concat("[", condition.Name, "] like '%", condition.Value, "%'"));

                        break;
                    default:
                        break;
                }

            }
        }

        string FilterString = "";
        
        if(Filters.Count > 0)
            FilterString = String.Concat("AND ", String.Join(" AND ",Filters));


        StoredProcedure sp = SPs.ExecuteView(View,false,false,e.Sort, e.Dir.ToString(), e.Start + 1, e.Limit, FilterString);
        DataTable ResultTable = sp.GetDataSet().Tables[0];

       
        if (ResultTable.Rows.Count > 0)
            e.Total = int.Parse(ResultTable.Rows[0]["CNT"].ToString());
        else
            e.Total = 0;

        if (ResultTable.Columns.IndexOf("CNT") > 0) ResultTable.Columns.Remove("CNT");
        if (ResultTable.Columns.IndexOf("RowNo") > 0) ResultTable.Columns.Remove("RowNo");

        return ResultTable;

    }

    public static string Export(string json, string visCols, string sortCol, string sortDir,string View)
    {
        Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(visCols);
    
        String FilterColumn = String.Empty;
        List<string> Filters = new List<string>();

     
 

       if (!string.IsNullOrEmpty(json))
        {
            ext.FilterConditions fc = new ext.FilterConditions(json);

            foreach (ext.FilterCondition condition in fc.Conditions)
            {
                switch (condition.FilterType)
                {
                    case ext.FilterType.Date:
                    case ext.FilterType.Numeric:
                        switch (condition.Comparison)
                        {
                            case Ext.Net.Comparison.Eq:
                                Filters.Add(String.Concat("[", condition.Name, "] = ", condition.Value));
                                break;
                            case Ext.Net.Comparison.Gt:
                                Filters.Add(String.Concat("[", condition.Name, "] >= ", condition.Value));
                                break;
                            case Ext.Net.Comparison.Lt:
                                Filters.Add(String.Concat("[", condition.Name, "] <= ", condition.Value));
                                break;
                            default:
                                break;
                        }
                        break;

                    case ext.FilterType.String:
                        Filters.Add(String.Concat("[", condition.Name, "] like '%", condition.Value, "%'"));

                        break;
                    default:
                        break;
                }

            }
        }

        string FilterString = "";

        if (Filters.Count > 0)
            FilterString = String.Concat("AND ", String.Join(" AND ", Filters));


        StoredProcedure sp = SPs.ExecuteView(View, false,true, sortCol, sortDir,1,20, FilterString);

        
        DataTable ResultTable = sp.GetDataSet().Tables[0];

        JavaScriptSerializer ser = new JavaScriptSerializer();
        ser.MaxJsonLength = 2147483647;


        List<string> RemoveColumns = new List<string>();

        foreach (DataColumn col in ResultTable.Columns)
        {
           var entry = values.FirstOrDefault(e => e.Key == col.ColumnName);

           if (entry.Key == null)
               RemoveColumns.Add(col.ColumnName);           
        }

        foreach (var col in RemoveColumns)
        {
            ResultTable.Columns.Remove(col);
        }


        string js = JsonConvert.SerializeObject(ResultTable);

        return js;

    }
}