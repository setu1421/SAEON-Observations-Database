using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using SAEON.ObservationsDB.Data;
using SubSonic;
using System.Xml;
using System.Xml.Xsl;
using Newtonsoft.Json;
using System.Collections;
using System.Dynamic;
using System.Text;
using System.Web.Script.Serialization;
using System.Data;

public partial class _DataQuery : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!X.IsAjaxRequest)
        {
            FromFilter.SelectedDate = DateTime.Now.AddYears(-100);
            ToFilter.SelectedDate = DateTime.Now;

            BuildTree();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void BuildTree()
    {
        ModuleX modx = new ModuleX("e4c08bfa-a8f0-4112-b45c-dd1788ade5a0");
        Ext.Net.TreeNode root = new Ext.Net.TreeNode("Organisation");
        root.Expanded = true;
        root.Icon = (Icon)modx.Icon;
        FilterTree.Root.Add(root);

        OrganisationCollection organisationCol = new OrganisationCollection().Where("ID", SubSonic.Comparison.IsNot, null).OrderByAsc(Organisation.Columns.Name).Load();

        foreach (Organisation organisation in organisationCol)
        {
            Ext.Net.TreeNode organisationNode = new Ext.Net.TreeNode(organisation.Name, Icon.ResultsetNext);
            root.Nodes.Add(organisationNode);

            ProjectSiteCollection projectSiteCol = new ProjectSiteCollection().Where("ID", SubSonic.Comparison.IsNot, null)
                .Where("OrganisationID", SubSonic.Comparison.Equals, organisation.Id)
                .OrderByAsc(ProjectSite.Columns.Name).Load();

            modx = new ModuleX("bd5f2a82-3dd3-46b1-8ce3-7ee99d5c77ad");
            Ext.Net.TreeNode projectSiteroot = new Ext.Net.TreeNode("Project Site", (Icon)modx.Icon);
            organisationNode.Nodes.Add(projectSiteroot);

            foreach (ProjectSite projectSite in projectSiteCol)
            {
                Ext.Net.TreeNode projectSiteNode = new Ext.Net.TreeNode(projectSite.Name, Icon.ResultsetNext);
                projectSiteroot.Nodes.Add(projectSiteNode);

                StationCollection StationCol = new StationCollection().Where("ID", SubSonic.Comparison.IsNot, null)
                    .Where("ProjectSiteID", SubSonic.Comparison.Equals, projectSite.Id)
                    .OrderByAsc(Station.Columns.Name).Load();

                modx = new ModuleX("0585e63d-0f9f-4dda-98ec-7de9397dc614");
                Ext.Net.TreeNode stationroot = new Ext.Net.TreeNode("Station", (Icon)modx.Icon);
                projectSiteNode.Nodes.Add(stationroot);

                foreach (Station station in StationCol)
                {
                    Ext.Net.TreeNode stationNode = new Ext.Net.TreeNode(station.Name, Icon.ResultsetNext);
                    stationNode.Checked = Ext.Net.ThreeStateBool.False;
                    stationNode.NodeID = station.Id.ToString() + "_Station";
                    stationroot.Nodes.Add(stationNode);

                    SensorCollection SensorCol = new SensorCollection().Where("ID", SubSonic.Comparison.IsNot, null)
                        .Where("StationID", SubSonic.Comparison.Equals, station.Id)
                        .OrderByAsc(Sensor.Columns.Name).Load();

                    modx = new ModuleX("9992ba10-cb0c-4a22-841c-1d695e8293d5");
                    Ext.Net.TreeNode Sensorroot = new Ext.Net.TreeNode("Sensor Procedure", (Icon)modx.Icon);
                    stationNode.Nodes.Add(Sensorroot);

                    foreach (Sensor sensor in SensorCol)
                    {
                        Ext.Net.TreeNode SensorNode = new Ext.Net.TreeNode(sensor.Name, Icon.ResultsetNext);
                        SensorNode.Checked = Ext.Net.ThreeStateBool.False;
                        SensorNode.NodeID = sensor.Id.ToString() + "_Sensor";
                        Sensorroot.Nodes.Add(SensorNode);
                        Phenomenon phenomenon = new Phenomenon(sensor.PhenomenonID);


                        Ext.Net.TreeNode phenomenonNode = new Ext.Net.TreeNode(phenomenon.Name, Icon.ResultsetNext);
                        phenomenonNode.Checked = Ext.Net.ThreeStateBool.False;

                        phenomenonNode.NodeID = sensor.Id.ToString() + "_Phenomenon";
                        SensorNode.Nodes.Add(phenomenonNode);

                        //PhenomenonOfferingCollection phenomenonOfferingCollection = new PhenomenonOfferingCollection().Where("ID", SubSonic.Comparison.IsNot, null)
                        //    .Where("PhenomenonID", SubSonic.Comparison.Equals, phenomenon.Id).Load();


                        // this.cbUnitofMeasure.GetStore().DataSource = new Select(PhenomenonUOM.IdColumn, UnitOfMeasure.UnitColumn)
                        //.From(UnitOfMeasure.Schema)
                        //.InnerJoin(PhenomenonUOM.UnitOfMeasureIDColumn, UnitOfMeasure.IdColumn)
                        //.Where(PhenomenonUOM.Columns.PhenomenonID).IsEqualTo(Id)
                        //.ExecuteDataSet().Tables[0];

                        PhenomenonOfferingCollection phenomenonOfferingCollection = new Select().From(PhenomenonOffering.Schema)
                                    .InnerJoin(Phenomenon.IdColumn, PhenomenonOffering.PhenomenonIDColumn)
                                    .Where(Phenomenon.IdColumn).IsEqualTo(phenomenon.Id)
                                    .OrderAsc(Phenomenon.Columns.Name)
                                    .ExecuteAsCollection<PhenomenonOfferingCollection>();

                        foreach (PhenomenonOffering phenomenonOffering in phenomenonOfferingCollection)
                        {
                            Ext.Net.TreeNode phenomenonOfferingNode = new Ext.Net.TreeNode(phenomenonOffering.Offering.Name, Icon.ResultsetNext);
                            phenomenonOfferingNode.Checked = Ext.Net.ThreeStateBool.False;
                            phenomenonOfferingNode.NodeID = phenomenonOffering.Offering.Id.ToString() + "_Offering#" + sensor.Id;
                            phenomenonNode.Nodes.Add(phenomenonOfferingNode);
                        }
                    }
                }
            }
        }
    }

    protected class QueryDataClass
    {
        public Guid ID { get; set; }
        public string Type { get; set; }
    }




    protected void DQStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string json = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);
        string visCols = VisCols.Value.ToString();



        string js = BuildQ(json, visCols, FromFilter.SelectedDate, ToFilter.SelectedDate, sortCol, sortDir);


        BaseRepository.doExport(type, js);

    }

    protected void DQStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {

        DateTime fromDate = FromFilter.SelectedDate;
        DateTime ToDate = ToFilter.SelectedDate;

        SqlQuery q = new Select().From(VObservationRole.Schema);
        
        if (FilterTree.CheckedNodes != null)
        {
            List<SubmittedNode> nodes = FilterTree.CheckedNodes;
            List<QueryDataClass> QueryDataClassList = new List<QueryDataClass>();

            foreach (var item in nodes)
            {
                QueryDataClassList.Add(new QueryDataClass() { ID = new Guid(item.NodeID.Substring(0, item.NodeID.IndexOf("_"))), Type = item.NodeID.Substring(item.NodeID.IndexOf("_") + 1) });
            }

            #region buildQ
            foreach (QueryDataClass item in QueryDataClassList)
            {

                int count = 0;
                Offering offering = new Offering();
                Phenomenon Phenomenon = new Phenomenon();
                Sensor Sensor = new Sensor();
                Station station = new Station();

                if (item.Type.Length > 20)
                {
                    if (item.Type.Substring(0, 8) == "Offering")
                    {
                        count++;
                        Offering off = new Offering(item.ID);
                        Sensor sp = new Sensor(item.Type.Substring(item.Type.IndexOf("#") + 1, item.Type.Substring(item.Type.IndexOf("#") + 1, 36).Length));


                        q.OrExpression(VObservation.Columns.OffID).IsEqualTo(off.Id)
                       .And(VObservation.Columns.PhenomenonID).IsEqualTo(sp.PhenomenonID)
                       .And(VObservation.Columns.SensorID).IsEqualTo(sp.Id)
                       .And(VObservation.Columns.StationID).IsEqualTo(sp.StationID);
                    }
                }

                if (item.Type == "Phenomenon")
                {
                    count++;
                    Sensor sp = new Sensor(item.ID);
                    Phenomenon phenomenon = new Phenomenon(sp.PhenomenonID);

                    q.OrExpression(VObservation.Columns.PhenomenonID).IsEqualTo(phenomenon.Id)
                    .And(VObservation.Columns.SensorID).IsEqualTo(sp.Id)
                    .And(VObservation.Columns.StationID).IsEqualTo(sp.StationID);

                }

                if (item.Type == "Sensor")
                {
                    count++;
                    Sensor = new Sensor(item.ID);

                    q.OrExpression(VObservation.Columns.SensorID).IsEqualTo(item.ID)
                    .And(VObservation.Columns.StationID).IsEqualTo(Sensor.StationID);

                }

                if (item.Type == "Station")
                {
                    count++;
                    station = new Station(item.ID);
                    q.OrExpression(VObservation.Columns.StationID).IsEqualTo(item.ID);

                }

                if (count != 0)
                {
                    if (fromDate.ToString() != "0001/01/01 00:00:00")
                    {
                        q.And(VObservation.Columns.ValueDate).IsGreaterThanOrEqualTo(fromDate.ToString());
                    }
                    if (ToDate.ToString() != "0001/01/01 00:00:00")
                    {
                        q.And(VObservation.Columns.ValueDate).IsLessThanOrEqualTo(ToDate.AddHours(23).AddMinutes(59).AddSeconds(59).ToString());
                    }
                    q.And(VObservationRole.Columns.Expr5).IsEqualTo(AuthHelper.GetLoggedInUserId);
                    DataQueryRepository.qFilterNSort(ref q, ref e);

                    q.CloseExpression();


                }
            }
            #endregion buildQ

            DataQueryRepository.qPage(ref q, ref e);
            Ext.Net.GridFilters gfs = FindControl("GridFilters1") as Ext.Net.GridFilters;
            this.ObservationsGrid.GetStore().DataSource = DataQueryRepository.GetPagedFilteredList(e, e.Parameters[this.GridFilters1.ParamPrefix], ref q);


        }
        else

            this.ObservationsGrid.GetStore().DataSource = DataQueryRepository.GetPagedList(e, e.Parameters[this.GridFilters1.ParamPrefix], fromDate, ToDate);


    }


    
    public string BuildQ(string json, string visCols, DateTime dateFrom, DateTime dateTo, string sortCol, string sortDir)
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


        SqlQuery q = new Select(colms).From(VObservationRole.Schema);

        //q.And(VObservationRole.Columns.Expr5).IsEqualTo(AuthHelper.GetLoggedInUserId);
        

        if (FilterTree.CheckedNodes != null)
        {
            List<SubmittedNode> nodes = FilterTree.CheckedNodes;
            List<QueryDataClass> QueryDataClassList = new List<QueryDataClass>();

            foreach (var item in nodes)
            {
                QueryDataClassList.Add(new QueryDataClass() { ID = new Guid(item.NodeID.Substring(0, item.NodeID.IndexOf("_"))), Type = item.NodeID.Substring(item.NodeID.IndexOf("_") + 1) });
            }

            #region buildQ
            foreach (QueryDataClass item in QueryDataClassList)
            {

                int count = 0;
                Offering offering = new Offering();
                Phenomenon Phenomenon = new Phenomenon();
                Sensor Sensor = new Sensor();
                Station station = new Station();

                if (item.Type.Length > 20)
                {
                    if (item.Type.Substring(0, 8) == "Offering")
                    {
                        count++;
                        Offering off = new Offering(item.ID);
                        Sensor sp = new Sensor(item.Type.Substring(item.Type.IndexOf("#") + 1, item.Type.Substring(item.Type.IndexOf("#") + 1, 36).Length));


                        q.OrExpression(VObservation.Columns.OffID).IsEqualTo(off.Id)
                       .And(VObservation.Columns.PhenomenonID).IsEqualTo(sp.PhenomenonID)
                       .And(VObservation.Columns.SensorID).IsEqualTo(sp.Id)
                       .And(VObservation.Columns.StationID).IsEqualTo(sp.StationID);
                    }
                }

                if (item.Type == "Phenomenon")
                {
                    count++;
                    Sensor sp = new Sensor(item.ID);
                    Phenomenon phenomenon = new Phenomenon(sp.PhenomenonID);

                    q.OrExpression(VObservation.Columns.PhenomenonID).IsEqualTo(phenomenon.Id)
                    .And(VObservation.Columns.SensorID).IsEqualTo(sp.Id)
                    .And(VObservation.Columns.StationID).IsEqualTo(sp.StationID);

                }

                if (item.Type == "Sensor")
                {
                    count++;
                    Sensor = new Sensor(item.ID);

                    q.OrExpression(VObservation.Columns.SensorID).IsEqualTo(item.ID)
                    .And(VObservation.Columns.StationID).IsEqualTo(Sensor.StationID);

                }

                if (item.Type == "Station")
                {
                    count++;
                    station = new Station(item.ID);
                    q.OrExpression(VObservation.Columns.StationID).IsEqualTo(item.ID);

                }

                if (count != 0)
                {

                    q.And(VObservation.Columns.ValueDate).IsGreaterThanOrEqualTo(dateFrom);

                    q.And(VObservation.Columns.ValueDate).IsLessThanOrEqualTo(dateTo.Date.AddHours(23).AddMinutes(59).AddSeconds(59));

                    if (json != null)
                    {
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
                    }

                    q.And(VObservationRole.Columns.Expr5).IsEqualTo(AuthHelper.GetLoggedInUserId);
                    q.CloseExpression();


                }
            }
            #endregion buildQ

            Ext.Net.GridFilters gfs = FindControl("GridFilters1") as Ext.Net.GridFilters;


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


    protected void ToExcel(object sender, DirectEventArgs e)
    {
        //Ext.Net.Hidden hiddenfield = FindControl("GridData") as Ext.Net.Hidden;
        //string json = hiddenfield.Value.ToString();
        ///////
        //Ext.Net.Hidden VisCols = FindControl("VisCols") as Ext.Net.Hidden;
        //string visCols = VisCols.Value.ToString();



        //string js = BuildQ(json, visCols, e.ExtraParams["dateFrom"], e.ExtraParams["dateTo"]);


        //System.Web.Script.Serialization.JavaScriptSerializer oSerializer =
        //new System.Web.Script.Serialization.JavaScriptSerializer();
        ////json = oSerializer.Serialize(LOutDynamic);

        //StoreSubmitDataEventArgs eSubmit = new StoreSubmitDataEventArgs(js, null);
        //XmlNode xml = eSubmit.Xml;

        //this.Response.Clear();
        //this.Response.ContentType = "application/vnd.ms-excel";
        //this.Response.AddHeader("Content-Disposition", "attachment; filename=submittedData.xls");
        //XslCompiledTransform xtExcel = new XslCompiledTransform();
        //xtExcel.Load(Server.MapPath("../Templates/Excel.xsl"));
        //xtExcel.Transform(xml, null, this.Response.OutputStream);
        //this.Response.End();
    }

    
}