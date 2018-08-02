using Ext.Net;
using Newtonsoft.Json;
using SAEON.Logs;
using SAEON.Observations.Data;
using SubSonic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;

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
        QueryCommand cmd = new QueryCommand("Select Distinct * from PhenomenonOffering inner join ImportBatchSummary on (ImportBatchSummary.PhenomenonOfferingID = PhenomenonOffering.ID) where (SensorID = @SensorID)", PhenomenonOffering.Schema.Provider.Name);
        cmd.AddParameter("@SensorID", sensorId, DbType.Guid);
        PhenomenonOfferingCollection result = new PhenomenonOfferingCollection();
        result.LoadAndCloseReader(DataService.GetReader(cmd));
        return result;
    }

    protected void NodeLoad(object sender, NodeLoadEventArgs e)
    {
        using (Logging.MethodCall(GetType(), new ParameterList { { "NodeID", e.NodeID } }))
            try
            {
                if (e.NodeID.StartsWith("Organisations"))
                {
                    var col = new Select()
                                .From(Organisation.Schema)
                                .InnerJoin(VLocation.Schema.TableName, VLocation.Columns.OrganisationID, Organisation.Schema.TableName, Organisation.Columns.Id)
                                .Distinct()
                                .OrderAsc(Organisation.Columns.Name)
                                .ExecuteAsCollection<OrganisationCollection>();
                    Logging.Verbose("Organisations: {count}", col.Count());
                    foreach (var item in col)
                    {
                        Logging.Verbose("Organisation: {name}", item.Name);
                        Ext.Net.TreeNode node = new Ext.Net.TreeNode("Organisation_" + item.Id.ToString(), item.Name, Icon.ResultsetNext);
                        e.Nodes.Add(node);
                        AsyncTreeNode root = new AsyncTreeNode("Sites_" + item.Id.ToString() + "|" + node.NodeID, "Sites")
                        {
                            Icon = (Icon)new ModuleX("A5C81FF7-69D6-4344-8548-E3EF7F08C4E7").Icon
                        };
                        node.Nodes.Add(root);
                    }
                }
                else if (e.NodeID.StartsWith("Sites_"))
                {
                    var organisation = new Organisation(e.NodeID.Split('|')[0].Split('_')[1]);
                    var col = new Select()
                                .From(SAEON.Observations.Data.Site.Schema)
                                .InnerJoin(VLocation.Schema.TableName, VLocation.Columns.SiteID, SAEON.Observations.Data.Site.Schema.TableName, SAEON.Observations.Data.Site.Columns.Id)
                                .Where(VLocation.Columns.OrganisationID)
                                .IsEqualTo(organisation.Id)
                                .Distinct()
                                .OrderAsc(SAEON.Observations.Data.Site.Columns.Name)
                                .ExecuteAsCollection<SiteCollection>();
                    Logging.Verbose("Sites: {count}", col.Count());
                    foreach (var item in col)
                    {
                        Logging.Verbose("Site: {name}", item.Name);
                        Ext.Net.TreeNode node = new Ext.Net.TreeNode("Site_" + item.Id.ToString() + "|" + e.NodeID, item.Name, Icon.ResultsetNext)
                        {
                            Checked = ThreeStateBool.False
                        };
                        e.Nodes.Add(node);
                        AsyncTreeNode root = new AsyncTreeNode("Stations_" + item.Id.ToString() + "|" + node.NodeID, "Stations")
                        {
                            Icon = (Icon)new ModuleX("0585e63d-0f9f-4dda-98ec-7de9397dc614").Icon
                        };
                        node.Nodes.Add(root);
                    }
                }
                else if (e.NodeID.StartsWith("Stations_"))
                {
                    var site = new SAEON.Observations.Data.Site(e.NodeID.Split('|')[0].Split('_')[1]);
                    var col = new Select()
                                .From(Station.Schema)
                                .InnerJoin(VLocation.Schema.TableName, VLocation.Columns.StationID, Station.Schema.TableName, Station.Columns.Id)
                                .Where(VLocation.Columns.SiteID)
                                .IsEqualTo(site.Id)
                                .Distinct()
                                .OrderAsc(Station.Columns.Name)
                                .ExecuteAsCollection<StationCollection>();
                    Logging.Verbose("Stations: {count}", col.Count());
                    foreach (var item in col)
                    {
                        Logging.Verbose("Station: {name}", item.Name);
                        Ext.Net.TreeNode node = new Ext.Net.TreeNode("Station_" + item.Id.ToString() + "|" + e.NodeID, item.Name, Icon.ResultsetNext)
                        {
                            Checked = ThreeStateBool.False
                        };
                        e.Nodes.Add(node);
                        var q = new Select()
                            .From(ImportBatchSummary.Schema)
                            .Where(ImportBatchSummary.StationIDColumn)
                            .IsEqualTo(item.Id)
                            .GetRecordCount();
                        if (q == 0)
                            node.Leaf = true;
                        else
                        {
                            AsyncTreeNode root = new AsyncTreeNode("Instruments_" + item.Id.ToString() + "|" + node.NodeID, "Instruments")
                            {
                                Icon = (Icon)new ModuleX("2610866B-8CBF-44E1-9A38-6511B31A8350").Icon
                            };
                            node.Nodes.Add(root);
                        }
                    }

                }
                else if (e.NodeID.StartsWith("Instruments_"))
                {
                    var station = new Station(e.NodeID.Split('|')[0].Split('_')[1]);
                    var col = new Select()
                        .From(Instrument.Schema)
                        .InnerJoin(ImportBatchSummary.InstrumentIDColumn, Instrument.IdColumn)
                        .Where(ImportBatchSummary.StationIDColumn)
                        .IsEqualTo(station.Id)
                        .Distinct()
                        .OrderAsc(Instrument.Columns.Name)
                        .ExecuteAsCollection<InstrumentCollection>();
                    Logging.Verbose("Instruments: {count}", col.Count());
                    foreach (var item in col)
                    {
                        Logging.Verbose("Instrument: {name}", item.Name);
                        Ext.Net.TreeNode node = new Ext.Net.TreeNode("Instrument_" + item.Id.ToString() + "|" + e.NodeID, item.Name, Icon.ResultsetNext)
                        {
                            Checked = ThreeStateBool.False
                        };
                        e.Nodes.Add(node);
                        var q = new Select()
                            .From(ImportBatchSummary.Schema)
                            .Where(ImportBatchSummary.InstrumentIDColumn)
                            .IsEqualTo(item.Id)
                            .GetRecordCount();
                        if (q == 0)
                            node.Leaf = true;
                        else
                        {
                            AsyncTreeNode root = new AsyncTreeNode("Sensors_" + item.Id.ToString() + "|" + node.NodeID, "Sensors")
                            {
                                Icon = (Icon)new ModuleX("9992ba10-cb0c-4a22-841c-1d695e8293d5").Icon
                            };
                            node.Nodes.Add(root);
                        }
                    }

                }
                else if (e.NodeID.StartsWith("Sensors_"))
                {
                    var instrument = new Instrument(e.NodeID.Split('|')[0].Split('_')[1]);
                    var col = new Select()
                        .From(Sensor.Schema)
                        .InnerJoin(ImportBatchSummary.SensorIDColumn, Sensor.IdColumn)
                        .Where(ImportBatchSummary.InstrumentIDColumn)
                        .IsEqualTo(instrument.Id)
                        .Distinct()
                        .OrderAsc(Sensor.Columns.Name)
                        .ExecuteAsCollection<SensorCollection>();
                    Logging.Verbose("Sensors: {count}", col.Count());
                    foreach (var item in col)
                    {
                        Logging.Verbose("Sensor: {name}", item.Name);
                        Ext.Net.TreeNode node = new Ext.Net.TreeNode("Sensor_" + item.Id.ToString() + "|" + e.NodeID, item.Name, Icon.ResultsetNext)
                        {
                            Icon = Icon.ResultsetNext,
                            Checked = ThreeStateBool.False
                        };
                        e.Nodes.Add(node);
                        var colPhenomenaOfferings = new Select(VImportBatchSummary.Columns.PhenomenonOfferingID, VImportBatchSummary.Columns.PhenomenonName, VImportBatchSummary.Columns.OfferingName)
                            .From(VImportBatchSummary.Schema)
                            .Where(VImportBatchSummary.Columns.SensorID)
                            .IsEqualTo(item.Id)
                            .OrderAsc(VImportBatchSummary.Columns.PhenomenonName)
                            .OrderAsc(VImportBatchSummary.Columns.OfferingName)
                            .Distinct()
                            .ExecuteAsCollection<VImportBatchSummaryCollection>();
                        if (colPhenomenaOfferings.Count == 0)
                            node.Leaf = true;
                        else
                        {
                            Ext.Net.TreeNode phenomenonNode = null;
                            foreach (var phenomenonOffering in colPhenomenaOfferings)
                            {
                                if (phenomenonNode?.Text != phenomenonOffering.PhenomenonName)
                                {
                                    phenomenonNode = new Ext.Net.TreeNode("Phenomenon_" + phenomenonOffering.PhenomenonOfferingID.ToString() + "|" + node.NodeID, phenomenonOffering.PhenomenonName, Icon.ResultsetNext);
                                    node.Nodes.Add(phenomenonNode);
                                }
                                var offeringNode = new Ext.Net.TreeNode("Offering_" + phenomenonOffering.PhenomenonOfferingID.ToString() + "|" + phenomenonNode.NodeID, phenomenonOffering.OfferingName, Icon.ResultsetNext)
                                {
                                    Leaf = true,
                                    Checked = ThreeStateBool.False
                                };
                                phenomenonNode.Nodes.Add(offeringNode);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Exception(ex, "Unable to load node {nodeID}", e.NodeID);
                throw;
            }
    }

    /// <summary>
    /// 
    /// </summary>
    void BuildTree()
    {
        AsyncTreeNode root = new AsyncTreeNode("Organisations", "Organisations")
        {
            Icon = (Icon)new ModuleX("e4c08bfa-a8f0-4112-b45c-dd1788ade5a0").Icon
        };
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
        SqlQuery query = BuildQ(json, visCols, sortCol, sortDir);
        BaseRepository.Export(query, visCols, type, "Data Query", Response);
    }

    private string GetItem(List<Tuple<string, string>> items, string itemType)
    {
        return items.Where(i => i.Item1 == itemType).Select(i => i.Item2).FirstOrDefault();
    }

    private SqlQuery BuildQuery(string[] columns = null)
    {
        using (Logging.MethodCall(GetType(), new ParameterList { { "Columns", columns } })) ;
        {
            try
            {

                DateTime fromDate = FromFilter.SelectedDate;
                DateTime ToDate = ToFilter.SelectedDate;

                SqlQuery q = null;
                if ((columns == null) || (columns.Length == 0))
                    q = new Select().From(VObservation.Schema);
                else
                    q = new Select(columns).From(VObservation.Schema);

                if (FilterTree.CheckedNodes != null)
                {
                    List<SubmittedNode> nodes = FilterTree.CheckedNodes;
                    List<QueryDataClass> QueryDataClassList = new List<QueryDataClass>();

                    foreach (var item in nodes)
                    {
                        var items = item.NodeID.Split('|').Select(i => new Tuple<string, string>(i.Split('_')[0], i.Split('_')[1])).ToList();
                        QueryDataClassList.Add(new QueryDataClass() { NodeID = item.NodeID, ID = new Guid(items[0].Item2), Type = items[0].Item1 });
                    }

                    Logging.Verbose("Items: {@QueryDataClassList}", QueryDataClassList);

                    #region buildQ
                    foreach (QueryDataClass item in QueryDataClassList)
                    {

                        int count = 0;
                        List<Tuple<string, string>> items = item.NodeID.Split('|').Select(i => new Tuple<string, string>(i.Split('_')[0], i.Split('_')[1])).ToList();
                        Logging.Verbose("Items: {@items}", items);
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
                                phenomenon = offering.Phenomenon;
                                sensor = new Sensor(new Guid(GetItem(items, "Sensor")));
                                instrument = new Instrument(new Guid(GetItem(items, "Instrument")));
                                station = new Station(new Guid(GetItem(items, "Station")));
                                site = new SAEON.Observations.Data.Site(new Guid(GetItem(items, "Site")));
                                q.OrExpression(VObservation.Columns.PhenomenonOfferingID).IsEqualTo(offering.Id)
                                    .And(VObservation.Columns.SensorID).IsEqualTo(sensor.Id)
                                    .And(VObservation.Columns.InstrumentID).IsEqualTo(instrument.Id)
                                    .And(VObservation.Columns.StationID).IsEqualTo(station.Id)
                                    .And(VObservation.Columns.SiteID).IsEqualTo(site.Id);
                                break;
                            case "Phenomenon":
                                count++;
                                phenomenon = new PhenomenonOffering(item.ID).Phenomenon;
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
                }
                Logging.Verbose("SQL: {sql}", q.BuildSqlStatement());
                return q;
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
                throw;
            }
        }
    }

    protected void DQStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        using (Logging.MethodCall(GetType()))
        {
            try
            {
                if (FilterTree.CheckedNodes == null)
                {
                    ObservationsGrid.GetStore().DataSource = null;
                }
                else
                {
                    var q = BuildQuery();
                    ObservationsGrid.GetStore().DataSource = DataQueryRepository.GetPagedFilteredList(e, e.Parameters[GridFilters1.ParamPrefix], ref q);
                }
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
                throw;
            }
        }
    }



    public SqlQuery BuildQ(string json, string visCols, string sortCol, string sortDir)
    {
        using (Logging.MethodCall(GetType()))
        {
            try
            {
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(visCols);

                List<string> colmsL = new List<string>();
                List<string> colmsDisplayNamesL = new List<string>();

                foreach (var item in values)
                {
                    if (!(string.IsNullOrWhiteSpace(item.Value) || string.IsNullOrWhiteSpace(item.Key)))
                    {
                        colmsL.Add(item.Value);
                        colmsDisplayNamesL.Add(item.Key.Replace(" ", "").Replace("/", ""));
                    }
                }

                string[] colms = colmsL.ToArray();
                string[] colmsDisplayNames = colmsDisplayNamesL.ToArray();
                var q = BuildQuery(colms);
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

                if (!string.IsNullOrEmpty(sortCol) && !string.IsNullOrEmpty(sortDir))
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
                return q;
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
                throw;
            }
        }
    }


}