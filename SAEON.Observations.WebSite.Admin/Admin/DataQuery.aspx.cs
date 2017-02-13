using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using SAEON.Observations.Data;
using SubSonic;
using System.Xml;
using System.Xml.Xsl;
using Newtonsoft.Json;
using System.Collections;
using System.Dynamic;
using System.Text;
using System.Web.Script.Serialization;
using System.Data;
using Serilog;
using Serilog.Context;
using System.Transactions;

public partial class Admin_DataQuery : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!X.IsAjaxRequest)
        {
            FromFilter.SelectedDate = DateTime.Now.AddYears(-100);
            ToFilter.SelectedDate = DateTime.Now;
            ResourceManager ResourceManager1 = (ResourceManager)Master.FindControl("ResourceManager1");
            ResourceManager1.RegisterIcon(Icon.ResultsetNext);
            ResourceManager1.RegisterIcon((Icon)new ModuleX("A5C81FF7-69D6-4344-8548-E3EF7F08C4E7").Icon);
            ResourceManager1.RegisterIcon((Icon)new ModuleX("0585e63d-0f9f-4dda-98ec-7de9397dc614").Icon);
            ResourceManager1.RegisterIcon((Icon)new ModuleX("2610866B-8CBF-44E1-9A38-6511B31A8350").Icon);
            ResourceManager1.RegisterIcon((Icon)new ModuleX("9992ba10-cb0c-4a22-841c-1d695e8293d5").Icon);
            BuildTree();
        }
    }

    private PhenomenonOfferingCollection GetPhenomenonOfferings(Guid sensorId)
    {
        QueryCommand cmd = new QueryCommand("Select * from PhenomenonOffering where Exists(Select * from Observation where (Observation.SensorID = @SensorID) and (Observation.PhenomenonOfferingID = PhenomenonOffering.ID))", PhenomenonOffering.Schema.Provider.Name);
        cmd.AddParameter("@SensorID", sensorId, DbType.Guid);
        PhenomenonOfferingCollection result = new PhenomenonOfferingCollection();
        result.LoadAndCloseReader(DataService.GetReader(cmd));
        return result;
    }

    protected void NodeLoad(object sender, NodeLoadEventArgs e)
    {
        using (LogContext.PushProperty("Method", "NodeLoad"))
            try
            {
                if (e.NodeID.StartsWith("Organisations"))
                {
                    var col = new Select()
                        .From(Organisation.Schema)
                        .InnerJoin(OrganisationSite.Schema)
                        .Distinct()
                        .OrderAsc(Organisation.Columns.Name)
                        .ExecuteAsCollection<OrganisationCollection>();
                    foreach (var item in col)
                    {
                        Ext.Net.TreeNode node = new Ext.Net.TreeNode("Organisation_" + item.Id.ToString(), item.Name, Icon.ResultsetNext);
                        e.Nodes.Add(node);
                        var q = new Query(OrganisationSite.Schema).AddWhere(OrganisationSite.Columns.OrganisationID, item.Id).GetCount(OrganisationSite.Columns.SiteID);
                        if (q == 0)
                            node.Leaf = true;
                        else
                        {
                            AsyncTreeNode root = new AsyncTreeNode("Sites_" + item.Id.ToString() + "|" + node.NodeID, "Sites");
                            root.Icon = (Icon)new ModuleX("A5C81FF7-69D6-4344-8548-E3EF7F08C4E7").Icon;
                            node.Nodes.Add(root);
                        }
                    }
                }
                else if (e.NodeID.StartsWith("Sites_"))
                {
                    var organisation = new Organisation(e.NodeID.Split('|')[0].Split('_')[1]);
                    var col = new Select()
                        .From(SAEON.Observations.Data.Site.Schema)
                        .InnerJoin(OrganisationSite.Schema)
                        .Where(OrganisationSite.Columns.OrganisationID)
                        .IsEqualTo(organisation.Id)
                        .OrderAsc(SAEON.Observations.Data.Site.Columns.Name)
                        .Distinct()
                        .ExecuteAsCollection<SiteCollection>();
                    foreach (var item in col)
                    {
                        Ext.Net.TreeNode node = new Ext.Net.TreeNode("Site_" + item.Id.ToString() + "|" + e.NodeID, item.Name, Icon.ResultsetNext);
                        node.Checked = ThreeStateBool.False;
                        e.Nodes.Add(node);
                        var q = new Query(Station.Schema).AddWhere(Station.Columns.SiteID, item.Id).GetCount(Station.Columns.Id);
                        if (q == 0)
                            node.Leaf = true;
                        else
                        {
                            AsyncTreeNode root = new AsyncTreeNode("Stations_" + item.Id.ToString() + "|" + node.NodeID, "Stations");
                            root.Icon = (Icon)new ModuleX("0585e63d-0f9f-4dda-98ec-7de9397dc614").Icon;
                            node.Nodes.Add(root);
                        }
                    }
                }
                else if (e.NodeID.StartsWith("Stations_"))
                {
                    var site = new SAEON.Observations.Data.Site(e.NodeID.Split('|')[0].Split('_')[1]);
                    var col = new Select()
                        .From(Station.Schema)
                        .InnerJoin(SAEON.Observations.Data.Site.Schema)
                        .Where(Station.Columns.SiteID)
                        .IsEqualTo(site.Id)
                        .OrderAsc(Station.Columns.Name)
                        .Distinct()
                        .ExecuteAsCollection<StationCollection>();
                    foreach (var item in col)
                    {
                        Ext.Net.TreeNode node = new Ext.Net.TreeNode("Station_" + item.Id.ToString() + "|" + e.NodeID, item.Name, Icon.ResultsetNext);
                        node.Checked = ThreeStateBool.False;
                        e.Nodes.Add(node);
                        var q = new Query(StationInstrument.Schema).AddWhere(StationInstrument.Columns.StationID, item.Id).GetCount(StationInstrument.Columns.InstrumentID);
                        if (q == 0)
                            node.Leaf = true;
                        else
                        {
                            AsyncTreeNode root = new AsyncTreeNode("Instruments_" + item.Id.ToString() + "|" + node.NodeID, "Instruments");
                            root.Icon = (Icon)new ModuleX("2610866B-8CBF-44E1-9A38-6511B31A8350").Icon;
                            node.Nodes.Add(root);
                        }
                    }

                }
                else if (e.NodeID.StartsWith("Instruments_"))
                {
                    var station = new Station(e.NodeID.Split('|')[0].Split('_')[1]);
                    var col = new Select()
                        .From(Instrument.Schema)
                        .InnerJoin(StationInstrument.Schema)
                        .Where(StationInstrument.Columns.StationID)
                        .IsEqualTo(station.Id)
                        .OrderAsc(Instrument.Columns.Name)
                        .Distinct()
                        .ExecuteAsCollection<InstrumentCollection>();
                    foreach (var item in col)
                    {
                        Ext.Net.TreeNode node = new Ext.Net.TreeNode("Instrument_" + item.Id.ToString() + "|" + e.NodeID, item.Name, Icon.ResultsetNext);
                        node.Checked = ThreeStateBool.False;
                        e.Nodes.Add(node);
                        var q = new Query(InstrumentSensor.Schema).AddWhere(InstrumentSensor.Columns.InstrumentID, item.Id).GetCount(InstrumentSensor.Columns.SensorID);
                        if (q == 0)
                            node.Leaf = true;
                        else
                        {
                            AsyncTreeNode root = new AsyncTreeNode("Sensors_" + item.Id.ToString() + "|" + node.NodeID, "Sensors");
                            root.Icon = (Icon)new ModuleX("9992ba10-cb0c-4a22-841c-1d695e8293d5").Icon;
                            node.Nodes.Add(root);
                        }
                    }

                }
                else if (e.NodeID.StartsWith("Sensors_"))
                {
                    var instrument = new Instrument(e.NodeID.Split('|')[0].Split('_')[1]);
                    var col = new Select()
                        .From(Sensor.Schema)
                        .InnerJoin(InstrumentSensor.Schema)
                        .Where(InstrumentSensor.Columns.InstrumentID)
                        .IsEqualTo(instrument.Id)
                        .OrderAsc(Instrument.Columns.Name)
                        .Distinct()
                        .ExecuteAsCollection<SensorCollection>();
                    foreach (var item in col)
                    {
                        AsyncTreeNode node = new AsyncTreeNode("Sensor_" + item.Id.ToString() + "|" + e.NodeID, item.Name);
                        node.Icon = Icon.ResultsetNext;
                        node.Checked = ThreeStateBool.False;
                        e.Nodes.Add(node);
                        var colOfferings = GetPhenomenonOfferings(item.Id);
                        if (!colOfferings.Any())
                            node.Leaf = true;
                        else
                        {
                            //AsyncTreeNode root = new AsyncTreeNode("Phenomena_" + item.PhenomenonID.ToString(), "Phenomena");
                            //root.Icon = Icon.ResultsetNext;
                            //node.Nodes.Add(root);
                        }
                    }
                }
                else if (e.NodeID.StartsWith("Sensor_"))
                {
                    var sensor = new Sensor(e.NodeID.Split('|')[0].Split('_')[1]);
                    AsyncTreeNode root = new AsyncTreeNode("Phenomenon_" + sensor.PhenomenonID.ToString() + "|" + e.NodeID, sensor.Phenomenon.Name);
                    root.Icon = Icon.ResultsetNext;
                    e.Nodes.Add(root);
                }
                else if (e.NodeID.StartsWith("Phenomenon_"))
                {
                    var phenomenon = new Phenomenon(e.NodeID.Split('|')[0].Split('_')[1]);
                    var items = e.NodeID.Split('|').Select(i => new Tuple<string, string>(i.Split('_')[0], i.Split('_')[1])).ToList();
                    var col = GetPhenomenonOfferings(items.Where(i => i.Item1 == "Sensor").Select(i => Utilities.MakeGuid(i.Item2)).First());
                    foreach (var item in col)
                    {
                        Ext.Net.TreeNode node = new Ext.Net.TreeNode("Offering_" + item.Id.ToString() + "|" + e.NodeID, item.Offering.Name, Icon.ResultsetNext);
                        node.Checked = ThreeStateBool.False;
                        node.Leaf = true;
                        e.Nodes.Add(node);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unable to load node {nodeID}", e.NodeID);
                throw;
            }
    }

    /// <summary>
    /// 
    /// </summary>
    void BuildTree()
    {
        AsyncTreeNode root = new AsyncTreeNode("Organisations", "Organisations");
        root.Icon = (Icon)new ModuleX("e4c08bfa-a8f0-4112-b45c-dd1788ade5a0").Icon;
        FilterTree.Root.Add(root);
    }

    protected class QueryDataClass
    {
        public string NodeID { get; set; }
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

    private string GetItem(List<Tuple<string, string>> items, string itemType)
    {
        return items.Where(i => i.Item1 == itemType).Select(i => i.Item2).First();
    }

    protected void DQStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {

        DateTime fromDate = FromFilter.SelectedDate;
        DateTime ToDate = ToFilter.SelectedDate;

        SqlQuery q = new Select().From(VObservationRole.Schema);

        if (FilterTree.CheckedNodes == null)
        {
            this.ObservationsGrid.GetStore().DataSource = DataQueryRepository.GetPagedList(e, e.Parameters[this.GridFilters1.ParamPrefix], fromDate, ToDate);
        }
        else
        {
            List<SubmittedNode> nodes = FilterTree.CheckedNodes;
            List<QueryDataClass> QueryDataClassList = new List<QueryDataClass>();

            foreach (var item in nodes)
            {
                var items = item.NodeID.Split('|').Select(i => new Tuple<string, string>(i.Split('_')[0], i.Split('_')[1])).ToList();
                QueryDataClassList.Add(new QueryDataClass() { NodeID = item.NodeID, ID = new Guid(items[0].Item2), Type = items[0].Item1 });
            }

            //Log.Information("Items: {@QueryDataClassList}", QueryDataClassList);

            #region buildQ
            foreach (QueryDataClass item in QueryDataClassList)
            {

                int count = 0;
                //Offering offering = new Offering();
                //Phenomenon Phenomenon = new Phenomenon();
                //Sensor Sensor = new Sensor();
                //Station station = new Station();
                List<Tuple<string, string>> items = item.NodeID.Split('|').Select(i => new Tuple<string, string>(i.Split('_')[0], i.Split('_')[1])).ToList();
                //Log.Information("Items: {@items}", items);
                PhenomenonOffering offering = null;
                Phenomenon phenomenon = null;
                Sensor sensor = null;
                Instrument instrument = null;
                Station station = null;
                SAEON.Observations.Data.Site site = null;
                switch (item.Type)
                {
                    case "Offering":
                        count++;
                        offering = new PhenomenonOffering(item.ID);
                        phenomenon = new Phenomenon(new Guid(GetItem(items, "Phenomenon")));
                        sensor = new Sensor(new Guid(GetItem(items, "Sensor")));
                        instrument = new Instrument(new Guid(GetItem(items, "Instrument")));
                        station = new Station(new Guid(GetItem(items, "Station")));
                        site = new SAEON.Observations.Data.Site(new Guid(GetItem(items, "Site")));
                        q.OrExpression(VObservation.Columns.PhenomenonOfferingID).IsEqualTo(offering.Id)
                            .And(VObservation.Columns.PhenomenonID).IsEqualTo(phenomenon.Id)
                            .And(VObservation.Columns.SensorID).IsEqualTo(sensor.Id)
                            .And(VObservation.Columns.InstrumentID).IsEqualTo(instrument.Id)
                            .And(VObservation.Columns.StationID).IsEqualTo(station.Id)
                            .And(VObservation.Columns.SiteID).IsEqualTo(site.Id);
                        break;
                    case "Phenomenon":
                        count++;
                        phenomenon = new Phenomenon(item.ID);
                        sensor = new Sensor(new Guid(GetItem(items, "Sensor")));
                        instrument = new Instrument(new Guid(GetItem(items, "Instrument")));
                        station = new Station(new Guid(GetItem(items, "Station")));
                        site = new SAEON.Observations.Data.Site(new Guid(GetItem(items, "Site")));
                        q.OrExpression(VObservation.Columns.PhenomenonID).IsEqualTo(phenomenon.Id)
                            .And(VObservation.Columns.SensorID).IsEqualTo(sensor.Id)
                            .And(VObservation.Columns.InstrumentID).IsEqualTo(instrument.Id)
                            .And(VObservation.Columns.StationID).IsEqualTo(station.Id)
                            .And(VObservation.Columns.SiteID).IsEqualTo(site.Id);
                        break;
                    case "Sensor":
                        count++;
                        sensor = new Sensor(item.ID);
                        instrument = new Instrument(new Guid(GetItem(items, "Instrument")));
                        station = new Station(new Guid(GetItem(items, "Station")));
                        site = new SAEON.Observations.Data.Site(new Guid(GetItem(items, "Site")));
                        q.OrExpression(VObservation.Columns.SensorID).IsEqualTo(sensor.Id)
                            .And(VObservation.Columns.InstrumentID).IsEqualTo(instrument.Id)
                            .And(VObservation.Columns.StationID).IsEqualTo(station.Id)
                            .And(VObservation.Columns.SiteID).IsEqualTo(site.Id);
                        break;
                    case "Instrument":
                        count++;
                        instrument = new Instrument(item.ID);
                        station = new Station(new Guid(GetItem(items, "Station")));
                        site = new SAEON.Observations.Data.Site(new Guid(GetItem(items, "Site")));
                        q.OrExpression(VObservation.Columns.InstrumentID).IsEqualTo(instrument.Id)
                            .And(VObservation.Columns.StationID).IsEqualTo(station.Id)
                            .And(VObservation.Columns.SiteID).IsEqualTo(site.Id);
                        break;
                    case "Station":
                        count++;
                        station = new Station(item.ID);
                        site = new SAEON.Observations.Data.Site(new Guid(GetItem(items, "Site")));
                        q.OrExpression(VObservation.Columns.StationID).IsEqualTo(station.Id)
                            .And(VObservation.Columns.SiteID).IsEqualTo(site.Id);
                        break;
                    case "Site":
                        count++;
                        site = new SAEON.Observations.Data.Site(item.ID);
                        q.OrExpression(VObservation.Columns.SiteID).IsEqualTo(site.Id);
                        break;
                }

                if (count != 0)
                {
                    q.And(VObservationRole.Columns.RoleUserId).IsEqualTo(AuthHelper.GetLoggedInUserId);
                    if (fromDate.ToString() != "0001/01/01 00:00:00")
                    {
                        q.And(VObservation.Columns.ValueDate).IsGreaterThanOrEqualTo(fromDate.ToString());
                    }
                    if (ToDate.ToString() != "0001/01/01 00:00:00")
                    {
                        q.And(VObservation.Columns.ValueDate).IsLessThanOrEqualTo(ToDate.AddHours(23).AddMinutes(59).AddSeconds(59).ToString());
                    }
                    //DataQueryRepository.qFilterNSort(ref q, ref e);
                    q.CloseExpression();
                }
            }
            #endregion buildQ
            //DataQueryRepository.qPage(ref q, ref e);
            //Log.Information("SQL: {sql}", q.BuildSqlStatement());
            this.ObservationsGrid.GetStore().DataSource = DataQueryRepository.GetPagedFilteredList(e, e.Parameters[this.GridFilters1.ParamPrefix], ref q);
        }
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

        if (FilterTree.CheckedNodes != null)
        {
            List<SubmittedNode> nodes = FilterTree.CheckedNodes;
            List<QueryDataClass> QueryDataClassList = new List<QueryDataClass>();

            foreach (var item in nodes)
            {
                var items = item.NodeID.Split('|').Select(i => new Tuple<string, string>(i.Split('_')[0], i.Split('_')[1])).ToList();
                QueryDataClassList.Add(new QueryDataClass() { NodeID = item.NodeID, ID = new Guid(items[0].Item2), Type = items[0].Item1 });
            }
            //Log.Information("Items: {@QueryDataClassList}", QueryDataClassList);

            #region buildQ
            foreach (QueryDataClass item in QueryDataClassList)
            {

                int count = 0;
                List<Tuple<string, string>> items = item.NodeID.Split('|').Select(i => new Tuple<string, string>(i.Split('_')[0], i.Split('_')[1])).ToList();
                //Log.Information("Items: {@items}", items);
                PhenomenonOffering offering = null;
                Phenomenon phenomenon = null;
                Sensor sensor = null;
                Instrument instrument = null;
                Station station = null;
                SAEON.Observations.Data.Site site = null;
                switch (item.Type)
                {
                    case "Offering":
                        count++;
                        offering = new PhenomenonOffering(item.ID);
                        phenomenon = new Phenomenon(new Guid(GetItem(items, "Phenomenon")));
                        sensor = new Sensor(new Guid(GetItem(items, "Sensor")));
                        instrument = new Instrument(new Guid(GetItem(items, "Instrument")));
                        station = new Station(new Guid(GetItem(items, "Station")));
                        site = new SAEON.Observations.Data.Site(new Guid(GetItem(items, "Site")));
                        q.OrExpression(VObservation.Columns.PhenomenonOfferingID).IsEqualTo(offering.Id)
                            .And(VObservation.Columns.PhenomenonID).IsEqualTo(phenomenon.Id)
                            .And(VObservation.Columns.SensorID).IsEqualTo(sensor.Id)
                            .And(VObservation.Columns.InstrumentID).IsEqualTo(instrument.Id)
                            .And(VObservation.Columns.StationID).IsEqualTo(station.Id)
                            .And(VObservation.Columns.SiteID).IsEqualTo(site.Id);
                        break;
                    case "Phenomenon":
                        count++;
                        phenomenon = new Phenomenon(item.ID);
                        sensor = new Sensor(new Guid(GetItem(items, "Sensor")));
                        instrument = new Instrument(new Guid(GetItem(items, "Instrument")));
                        station = new Station(new Guid(GetItem(items, "Station")));
                        site = new SAEON.Observations.Data.Site(new Guid(GetItem(items, "Site")));
                        q.OrExpression(VObservation.Columns.PhenomenonID).IsEqualTo(phenomenon.Id)
                            .And(VObservation.Columns.SensorID).IsEqualTo(sensor.Id)
                            .And(VObservation.Columns.InstrumentID).IsEqualTo(instrument.Id)
                            .And(VObservation.Columns.StationID).IsEqualTo(station.Id)
                            .And(VObservation.Columns.SiteID).IsEqualTo(site.Id);
                        break;
                    case "Sensor":
                        count++;
                        sensor = new Sensor(item.ID);
                        instrument = new Instrument(new Guid(GetItem(items, "Instrument")));
                        station = new Station(new Guid(GetItem(items, "Station")));
                        site = new SAEON.Observations.Data.Site(new Guid(GetItem(items, "Site")));
                        q.OrExpression(VObservation.Columns.SensorID).IsEqualTo(sensor.Id)
                            .And(VObservation.Columns.InstrumentID).IsEqualTo(instrument.Id)
                            .And(VObservation.Columns.StationID).IsEqualTo(station.Id)
                            .And(VObservation.Columns.SiteID).IsEqualTo(site.Id);
                        break;
                    case "Instrument":
                        count++;
                        instrument = new Instrument(item.ID);
                        station = new Station(new Guid(GetItem(items, "Station")));
                        site = new SAEON.Observations.Data.Site(new Guid(GetItem(items, "Site")));
                        q.OrExpression(VObservation.Columns.InstrumentID).IsEqualTo(instrument.Id)
                            .And(VObservation.Columns.StationID).IsEqualTo(station.Id)
                            .And(VObservation.Columns.SiteID).IsEqualTo(site.Id);
                        break;
                    case "Station":
                        count++;
                        station = new Station(item.ID);
                        site = new SAEON.Observations.Data.Site(new Guid(GetItem(items, "Site")));
                        q.OrExpression(VObservation.Columns.StationID).IsEqualTo(station.Id)
                            .And(VObservation.Columns.SiteID).IsEqualTo(site.Id);
                        break;
                    case "Site":
                        count++;
                        site = new SAEON.Observations.Data.Site(item.ID);
                        q.OrExpression(VObservation.Columns.SiteID).IsEqualTo(site.Id);
                        break;
                }
                if (count != 0)
                {
                    q.And(VObservationRole.Columns.RoleUserId).IsEqualTo(AuthHelper.GetLoggedInUserId);
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
                    q.CloseExpression();
                }
            }
            #endregion buildQ
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

        Log.Information("SQL: {sql}", q.BuildSqlStatement());
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