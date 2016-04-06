using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ext.Net;
using SubSonic;
using SAEON.ObservationsDB.Data;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Xsl;
using System.Data;

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

	public static string BuildExportQ(string TableName, string json, string visCols, string sortCol, string sortDir)
	{
		Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(visCols);

		List<string> colmsL = new List<string>();
		List<string> colmsDisplayNamesL = new List<string>();	
		
		foreach (var item in values)
		{
			if (string.IsNullOrWhiteSpace(item.Value) || string.IsNullOrWhiteSpace(item.Key))
			{
				
			}
			else 
			{ 
				//colms[i] = item.Value;
				//colmsDisplayNames[i] = item.Key;
				//i++;
				colmsL.Add(item.Value);
				colmsDisplayNamesL.Add(item.Key.Replace(" ", "").Replace("/", ""));
				
			}
		}

		string[] colms = colmsL.ToArray();
		string[] colmsDisplayNames = colmsDisplayNamesL.ToArray();

		SqlQuery q = new Select(colms).From(TableName);

		if (TableName == "VObservationRoles")
		{
			q.And(VObservationRole.Columns.Expr5).IsEqualTo(AuthHelper.GetLoggedInUserId);
		}

		#region filters
		if (!string.IsNullOrEmpty(json))
		{
			FilterConditions fc = new FilterConditions(json);
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
		#endregion filters

		//string js = q.ExecuteJSON("set", "a");
		//JavaScriptSerializer ser = new JavaScriptSerializer();
		//ser.MaxJsonLength = 2147483647;
		//object o = ser.DeserializeObject(js);
		//object b = ((Dictionary<string, object>)(((Dictionary<string, object>)o)["set"]))["a"];
	
		//js = ser.Serialize(b);

		DataTable dt = q.ExecuteDataSet().Tables[0];

		for (int k = 0; k < dt.Columns.Count; k++)
		{
			for (int j = 0; j < colms.Length; j++)
			{
				if (colms[j].ToLower() == dt.Columns[k].ToString().ToLower())
				{
					dt.Columns[k].ColumnName = colmsDisplayNames[j];
				}
			}
		}

		JavaScriptSerializer ser = new JavaScriptSerializer();
		ser.MaxJsonLength = 2147483647;

		string js = JsonConvert.SerializeObject(dt);


		return js;
	}

	public static void doExport(string type, string js)
	{
		#region doExport
		System.Web.Script.Serialization.JavaScriptSerializer oSerializer =
		new System.Web.Script.Serialization.JavaScriptSerializer();
		StoreSubmitDataEventArgs eSubmit = new StoreSubmitDataEventArgs(js, null);
		XmlNode xml = eSubmit.Xml;
		HttpContext.Current.Response.Clear();

		switch (type)
		{
			//case "xml":
			//    string strXml = xml.OuterXml;
			//    this.Response.AddHeader("Content-Disposition", "attachment; filename=submittedData.xml");
			//    this.Response.AddHeader("Content-Length", strXml.Length.ToString());
			//    this.Response.ContentType = "application/xml";
			//    this.Response.Write(strXml);
			//    break;

			case "exc":
				HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
				HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=Export.xls");
				XslCompiledTransform xtExcel = new XslCompiledTransform();
				xtExcel.Load(HttpContext.Current.Server.MapPath("../Templates/Excel.xsl"));
				xtExcel.Transform(xml, null, HttpContext.Current.Response.OutputStream);
				break;

			case "csv":
				HttpContext.Current.Response.ContentType = "application/octet-stream";
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=Export.csv");
				XslCompiledTransform xtCsv = new XslCompiledTransform();
				xtCsv.Load(HttpContext.Current.Server.MapPath("../Templates/Csv.xsl"));
				xtCsv.Transform(xml, null, HttpContext.Current.Response.OutputStream);
				break;
		}
		HttpContext.Current.Response.End();
		#endregion
	}
}