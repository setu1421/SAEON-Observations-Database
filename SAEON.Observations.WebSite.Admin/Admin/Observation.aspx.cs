using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using System.Xml;
using System.Xml.Xsl;
using SubSonic;
using Newtonsoft.Json;
using SAEON.Observations.Data;
using System.Web.Script.Serialization;

public partial class _Observation : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void ObservationStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        this.ObservationGrid.GetStore().DataSource = ObservationRepository.GetPagedList(e, e.Parameters[this.GridFilters1.ParamPrefix]);
    }


	protected void ObservationStore_Submit(object sender, StoreSubmitDataEventArgs e)
	{
		string type = FormatType.Text;
		string visCols = VisCols.Value.ToString();

        string json = GridData.Value.ToString();
        StoreSubmitDataEventArgs eSubmit = new StoreSubmitDataEventArgs(json, null); 
        XmlNode xml = eSubmit.Xml;

		string gridData = GridData.Text;
		string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
		string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

		string js = BaseRepository.BuildExportQ("VObservationRoles", gridData, visCols, sortCol, sortDir);

		BaseRepository.doExport(type, js);
	
		//#region doExport
		//System.Web.Script.Serialization.JavaScriptSerializer oSerializer =
		//new System.Web.Script.Serialization.JavaScriptSerializer();
		//StoreSubmitDataEventArgs eSubmit = new StoreSubmitDataEventArgs(js, null);
		//XmlNode xml = eSubmit.Xml;
		//HttpContext.Current.Response.Clear();

		//switch (type) 
		//{
		//    //case "xml":
		//    //    string strXml = xml.OuterXml;
		//    //    this.Response.AddHeader("Content-Disposition", "attachment; filename=submittedData.xml");
		//    //    this.Response.AddHeader("Content-Length", strXml.Length.ToString());
		//    //    this.Response.ContentType = "application/xml";
		//    //    this.Response.Write(strXml);
		//    //    break;

		//    case "exc":
		//        HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
		//        HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=submittedData.xls");
		//        XslCompiledTransform xtExcel = new XslCompiledTransform();
		//        xtExcel.Load(Server.MapPath("../Templates/Excel.xsl"));
		//        xtExcel.Transform(xml, null, Response.OutputStream);
		//        break;

		//    case "csv":
		//        HttpContext.Current.Response.ContentType = "application/octet-stream";
		//        HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=submittedData.csv");
		//        XslCompiledTransform xtCsv = new XslCompiledTransform();
		//        xtCsv.Load(Server.MapPath("../Templates/Csv.xsl"));
		//        xtCsv.Transform(xml, null, Response.OutputStream);
		//        break;
		//}
		//this.Response.End();
		//#endregion
	}

	
}