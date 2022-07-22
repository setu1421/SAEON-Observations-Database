using Ext.Net;
using Newtonsoft.Json;
using SAEON.Core;
using SAEON.Logs;
using SAEON.Observations.Data;
using SubSonic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
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

    /// <summary>
    /// 
    /// </summary>
    void BuildTree()
    {
        using (SAEONLogs.MethodCall(GetType()))
        {
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                Ext.Net.TreeNode rootOrganisations = new Ext.Net.TreeNode("Organisations", "Organisations", (Icon)new ModuleX("e4c08bfa-a8f0-4112-b45c-dd1788ade5a0").Icon);
                FilterTree.Root.Add(rootOrganisations);
                string organisationName = null;
                string siteName = null;
                string stationName = null;
                string instrumentName = null;
                string sensorName = null;
                string phenomenonName = null;
                string offeringName = null;
                var vInventorySensors =
                    new Select()
                    .From(VInventorySensor.Schema)
                    .Where("Count").IsGreaterThan(0)
                    .OrderAsc(VInventorySensor.Columns.OrganisationName)
                    .OrderAsc(VInventorySensor.Columns.SiteName)
                    .OrderAsc(VInventorySensor.Columns.StationName)
                    .OrderAsc(VInventorySensor.Columns.InstrumentName)
                    .OrderAsc(VInventorySensor.Columns.SensorName)
                    .OrderAsc(VInventorySensor.Columns.PhenomenonName)
                    .OrderAsc(VInventorySensor.Columns.OfferingName)
                    .Distinct()
                    .ExecuteAsCollection<VInventorySensorCollection>();
                Ext.Net.TreeNode nodeOrganisation = null;
                Ext.Net.TreeNode rootSites = null;
                Ext.Net.TreeNode nodeSite = null;
                Ext.Net.TreeNode rootStations = null;
                Ext.Net.TreeNode nodeStation = null;
                Ext.Net.TreeNode rootInstruments = null;
                Ext.Net.TreeNode nodeInstrument = null;
                Ext.Net.TreeNode rootSensors = null;
                Ext.Net.TreeNode nodeSensor = null;
                Ext.Net.TreeNode phenomenonNode = null;
                Ext.Net.TreeNode offeringNode = null;
                foreach (var dataset in vInventorySensors)
                {
                    if (organisationName != dataset.OrganisationName)
                    {
                        organisationName = dataset.OrganisationName;
                        siteName = null;
                        stationName = null;
                        instrumentName = null;
                        sensorName = null;
                        phenomenonName = null;
                        offeringName = null;
                        //SAEONLogs.Verbose("Organisation: {name}", organisationName);
                        nodeOrganisation = new Ext.Net.TreeNode("Organisation_" + dataset.OrganisationID.ToString(), dataset.OrganisationName, Icon.ResultsetNext);
                        rootOrganisations.Nodes.Add(nodeOrganisation);
                        rootSites = new Ext.Net.TreeNode("Sites_" + dataset.OrganisationID.ToString() + "|" + nodeOrganisation.NodeID, "Sites",
                            (Icon)new ModuleX("A5C81FF7-69D6-4344-8548-E3EF7F08C4E7").Icon);
                        nodeOrganisation.Nodes.Add(rootSites);
                    }
                    if (siteName != dataset.SiteName)
                    {
                        siteName = dataset.SiteName;
                        stationName = null;
                        instrumentName = null;
                        sensorName = null;
                        phenomenonName = null;
                        offeringName = null;
                        //SAEONLogs.Verbose("Site: {name}", siteName);
                        nodeSite = new Ext.Net.TreeNode("Site_" + dataset.SiteID.ToString() + "|" + nodeOrganisation.NodeID, dataset.SiteName, Icon.ResultsetNext)
                        {
                            Checked = ThreeStateBool.False
                        };
                        rootSites.Nodes.Add(nodeSite);
                        rootStations = new Ext.Net.TreeNode("Stations_" + dataset.SiteID.ToString() + "|" + nodeSite.NodeID, "Stations",
                            (Icon)new ModuleX("0585e63d-0f9f-4dda-98ec-7de9397dc614").Icon);
                        nodeSite.Nodes.Add(rootStations);
                    }
                    if (stationName != dataset.StationName)
                    {
                        stationName = dataset.StationName;
                        instrumentName = null;
                        sensorName = null;
                        phenomenonName = null;
                        offeringName = null;
                        //SAEONLogs.Verbose("Station: {name}", stationName);
                        nodeStation = new Ext.Net.TreeNode("Station_" + dataset.StationID.ToString() + "|" + nodeSite.NodeID, dataset.StationName, Icon.ResultsetNext)
                        {
                            Checked = ThreeStateBool.False
                        };
                        rootStations.Nodes.Add(nodeStation);
                        rootInstruments = new Ext.Net.TreeNode("Instruments_" + dataset.StationID.ToString() + "|" + nodeStation.NodeID, "Instruments",
                            (Icon)new ModuleX("2610866B-8CBF-44E1-9A38-6511B31A8350").Icon);
                        nodeStation.Nodes.Add(rootInstruments);
                    }
                    if (instrumentName != dataset.InstrumentName)
                    {
                        instrumentName = dataset.InstrumentName;
                        sensorName = null;
                        phenomenonName = null;
                        offeringName = null;
                        //SAEONLogs.Verbose("Instrument: {name}", instrumentName);
                        nodeInstrument = new Ext.Net.TreeNode("Instrument_" + dataset.InstrumentID.ToString() + "|" + nodeStation.NodeID, dataset.InstrumentName, Icon.ResultsetNext)
                        {
                            Checked = ThreeStateBool.False
                        };
                        rootInstruments.Nodes.Add(nodeInstrument);
                        rootSensors = new Ext.Net.TreeNode("Sensors_" + dataset.InstrumentID.ToString() + "|" + nodeInstrument.NodeID, "Sensors",
                            (Icon)new ModuleX("9992ba10-cb0c-4a22-841c-1d695e8293d5").Icon);
                        nodeInstrument.Nodes.Add(rootSensors);
                    }
                    if (sensorName != dataset.SensorName)
                    {
                        sensorName = dataset.SensorName;
                        phenomenonName = null;
                        offeringName = null;
                        //SAEONLogs.Verbose("Sensor: {name}", sensorName);
                        nodeSensor = new Ext.Net.TreeNode("Sensor_" + dataset.SensorID.ToString() + "|" + nodeInstrument.NodeID, dataset.SensorName, Icon.ResultsetNext)
                        {
                            Checked = ThreeStateBool.False
                        };
                        nodeInstrument.Nodes.Add(nodeSensor);
                    }
                    if (phenomenonName != dataset.PhenomenonName)
                    {
                        phenomenonName = dataset.PhenomenonName;
                        offeringName = null;
                        //SAEONLogs.Verbose("Phenomenon: {name}", phenomenonName);
                        phenomenonNode = new Ext.Net.TreeNode("Phenomenon_" + dataset.PhenomenonOfferingID.ToString() + "|" + nodeSensor.NodeID, dataset.PhenomenonName, Icon.ResultsetNext);
                        nodeSensor.Nodes.Add(phenomenonNode);
                    }
                    if (offeringName != dataset.OfferingName)
                    {
                        offeringName = dataset.OfferingName;
                        //SAEONLogs.Verbose("Offering: {name}", offeringName);
                        offeringNode = new Ext.Net.TreeNode("Offering_" + dataset.PhenomenonOfferingID.ToString() + "|" + phenomenonNode.NodeID, dataset.OfferingName, Icon.ResultsetNext)
                        {
                            Leaf = true,
                            Checked = ThreeStateBool.False
                        };
                        phenomenonNode.Nodes.Add(offeringNode);
                    }
                }
                SAEONLogs.Information("BuildTree done in {Elapsed}", stopwatch.Elapsed.TimeStr());
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex, "Unable to load tree");
                throw;
            }
        }
    }

    protected class QueryDataClass
    {
        public string NodeID { get; set; }
        public Guid ID { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public List<(string Type, string Id)> Items { get; set; } = new List<(string Type, string Id)>();
    }

    protected void ObservationsGridStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string json = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);
        string visCols = VisCols.Value.ToString();
        SqlQuery query = BuildQ(json, visCols, sortCol, sortDir, out var log);
        BaseRepository.Export(query, visCols, type, "Data Query", Response, i =>
        {
            var count = Convert.ToInt64(i.Compute("Count(ValueDate)", ""));
            var start = Convert.ToDateTime(i.Compute("Min(ValueDate)", ""));
            var end = Convert.ToDateTime(i.Compute("Max(ValueDate)", ""));
            SAEONLogs.Verbose("Count: {count} Start: {start} End: {end}", count, start, end);
            log += $" Result -> Rows: {count:N0} Start: {start:dd MMM yyyy} End: {end:dd MMM yyyy}";
            Auditing.Log(GetType(), new MethodCallParameters { { "Log", log } });
        });
    }

    private string GetItem(List<(string Type, string Id)> items, string itemType)
    {
        return items.Where(i => i.Type == itemType).Select(i => i.Id).FirstOrDefault();
    }

    private SqlQuery BuildQuery(out string log, string[] columns = null)
    {
        using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "Columns", columns } }))
        {
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                log = "Query -> ";
                DateTime fromDate = FromFilter.SelectedDate;
                DateTime toDate = ToFilter.SelectedDate;

                SqlQuery q = null;
                // @@ Remove Top
                if ((columns == null) || (columns.Length == 0))
                    q = new Select()/*.Top("100")*/.From(VObservationExpansion.Schema);
                else
                    q = new Select(columns)/*.Top("100")*/.From(VObservationExpansion.Schema);

                if (FilterTree.CheckedNodes != null)
                {
                    var nodes = new List<QueryDataClass>();

                    foreach (var node in FilterTree.CheckedNodes)
                    {
                        var items = node.NodeID.Split('|').Select(i => { (string Type, string Id) d = (i.Split('_')[0], i.Split('_')[1]); return d; }).ToList();
                        nodes.Add(new QueryDataClass() { NodeID = node.NodeID, ID = new Guid(items[0].Id), Type = items[0].Type, Name = node.Text, Items = items });
                    }

                    //SAEONLogs.Verbose("Items: {@QueryDataClassList}", QueryDataClassList);

                    #region buildQ
                    foreach (var node in nodes)
                    {

                        int count = 0;
                        PhenomenonOffering phenomenonOffering = null;
                        Phenomenon phenomenon = null;
                        Sensor sensor = null;
                        Instrument instrument = null;
                        Station station = null;
                        SAEON.Observations.Data.Site site = null;
                        switch (node.Type)
                        {
                            case "Offering":
                                count++;
                                phenomenonOffering = new PhenomenonOffering(node.ID);
                                sensor = new Sensor(new Guid(GetItem(node.Items, "Sensor")));
                                instrument = new Instrument(new Guid(GetItem(node.Items, "Instrument")));
                                station = new Station(new Guid(GetItem(node.Items, "Station")));
                                site = new SAEON.Observations.Data.Site(new Guid(GetItem(node.Items, "Site")));
                                q.OrExpression(VObservationExpansion.Columns.PhenomenonOfferingID).IsEqualTo(phenomenonOffering.Id)
                                    .And(VObservationExpansion.Columns.SensorID).IsEqualTo(sensor.Id)
                                    .And(VObservationExpansion.Columns.InstrumentID).IsEqualTo(instrument.Id)
                                    .And(VObservationExpansion.Columns.StationID).IsEqualTo(station.Id)
                                    .And(VObservationExpansion.Columns.SiteID).IsEqualTo(site.Id);
                                log += $"Site: {site.Name} Station: {station.Name} Instrument: {instrument.Name} Sensor: {sensor.Name} Phenomenon: {phenomenonOffering.Phenomenon.Name} Offering: {phenomenonOffering.Offering.Name}";
                                break;
                            case "Phenomenon":
                                count++;
                                phenomenon = new PhenomenonOffering(node.ID).Phenomenon;
                                sensor = new Sensor(new Guid(GetItem(node.Items, "Sensor")));
                                instrument = new Instrument(new Guid(GetItem(node.Items, "Instrument")));
                                station = new Station(new Guid(GetItem(node.Items, "Station")));
                                site = new SAEON.Observations.Data.Site(new Guid(GetItem(node.Items, "Site")));
                                q.OrExpression(VObservationExpansion.Columns.PhenomenonID).IsEqualTo(phenomenon.Id)
                                    .And(VObservationExpansion.Columns.SensorID).IsEqualTo(sensor.Id)
                                    .And(VObservationExpansion.Columns.InstrumentID).IsEqualTo(instrument.Id)
                                    .And(VObservationExpansion.Columns.StationID).IsEqualTo(station.Id)
                                    .And(VObservationExpansion.Columns.SiteID).IsEqualTo(site.Id);
                                log += $"Site: {site.Name} Station: {station.Name} Instrument: {instrument.Name} Sensor: {sensor.Name} Phenomenon: {phenomenonOffering.Phenomenon.Name}";
                                break;
                            case "Sensor":
                                count++;
                                sensor = new Sensor(node.ID);
                                instrument = new Instrument(new Guid(GetItem(node.Items, "Instrument")));
                                station = new Station(new Guid(GetItem(node.Items, "Station")));
                                site = new SAEON.Observations.Data.Site(new Guid(GetItem(node.Items, "Site")));
                                q.OrExpression(VObservationExpansion.Columns.SensorID).IsEqualTo(sensor.Id)
                                    .And(VObservationExpansion.Columns.InstrumentID).IsEqualTo(instrument.Id)
                                    .And(VObservationExpansion.Columns.StationID).IsEqualTo(station.Id)
                                    .And(VObservationExpansion.Columns.SiteID).IsEqualTo(site.Id);
                                log += $"Site: {site.Name} Station: {station.Name} Instrument: {instrument.Name} Sensor: {sensor.Name}";
                                break;
                            case "Instrument":
                                count++;
                                instrument = new Instrument(node.ID);
                                station = new Station(new Guid(GetItem(node.Items, "Station")));
                                site = new SAEON.Observations.Data.Site(new Guid(GetItem(node.Items, "Site")));
                                q.OrExpression(VObservationExpansion.Columns.InstrumentID).IsEqualTo(instrument.Id)
                                    .And(VObservationExpansion.Columns.StationID).IsEqualTo(station.Id)
                                    .And(VObservationExpansion.Columns.SiteID).IsEqualTo(site.Id);
                                log += $"Site: {site.Name} Station: {station.Name} Instrument: {instrument.Name}";
                                break;
                            case "Station":
                                count++;
                                station = new Station(node.ID);
                                site = new SAEON.Observations.Data.Site(new Guid(GetItem(node.Items, "Site")));
                                q.OrExpression(VObservationExpansion.Columns.StationID).IsEqualTo(station.Id)
                                    .And(VObservationExpansion.Columns.SiteID).IsEqualTo(site.Id);
                                log += $"Site: {site.Name} Station: {station.Name}";
                                break;
                            case "Site":
                                count++;
                                site = new SAEON.Observations.Data.Site(node.ID);
                                q.OrExpression(VObservationExpansion.Columns.SiteID).IsEqualTo(site.Id);
                                log += $"Site: {site.Name}";
                                break;
                        }

                        if (count != 0)
                        {
                            if (fromDate.ToString() != "0001/01/01 00:00:00")
                            {
                                q.And(VObservationExpansion.Columns.ValueDate).IsGreaterThanOrEqualTo(fromDate.ToString());
                                log += $" Start: {fromDate:dd MMM yyyy}";

                            }
                            if (toDate.ToString() != "0001/01/01 00:00:00")
                            {
                                q.And(VObservationExpansion.Columns.ValueDate).IsLessThanOrEqualTo(toDate.AddHours(23).AddMinutes(59).AddSeconds(59).ToString());
                                log += $" End: {toDate:dd MMM yyyy}";
                            }
                            //DataQueryRepository.qFilterNSort(ref q, ref e);
                            q.CloseExpression();
                        }
                    }
                    #endregion buildQ
                }
                SAEONLogs.Verbose("SQL: {sql}", q.BuildSqlStatement());
                SAEONLogs.Information("BuildQuery done in {Elapsed}", stopwatch.Elapsed.TimeStr());
                return q;
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                throw;
            }
        }
    }


    private List<ObservationDTO> LoadData()
    {
        using (SAEONLogs.MethodCall(GetType()))
        {
            try
            {
                //DatasetHelper.UpdateDatasetsFromDisk();
                SAEONLogs.Information("DatasetsFolder: {DatasetsFolder}", ConfigurationManager.AppSettings["DatasetsFolder"]);
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var result = new List<ObservationDTO>();
                if (FilterTree.CheckedNodes != null)
                {
                    var nodes = new List<QueryDataClass>();
                    foreach (var node in FilterTree.CheckedNodes)
                    {
                        var items = node.NodeID.Split('|').Select(i => { (string Type, string Id) d = (i.Split('_')[0], i.Split('_')[1]); return d; }).ToList();
                        nodes.Add(new QueryDataClass() { NodeID = node.NodeID, ID = new Guid(items[0].Id), Type = items[0].Type, Name = node.Text, Items = items });
                    }
                    SAEONLogs.Verbose("Items: {@Items}", nodes);
                    var allDatasets = new VDatasetsExpansionCollection().Load();
                    var selectedDatasets = new List<VDatasetsExpansion>();
                    foreach (var node in nodes)
                    {
                        PhenomenonOffering phenomenonOffering = null;
                        Phenomenon phenomenon = null;
                        Station station = null;
                        SAEON.Observations.Data.Site site = null;
                        switch (node.Type)
                        {
                            case "Offering":
                                phenomenonOffering = new PhenomenonOffering(node.ID);
                                station = new Station(new Guid(GetItem(node.Items, "Station")));
                                site = new SAEON.Observations.Data.Site(new Guid(GetItem(node.Items, "Site")));
                                selectedDatasets.AddRange(allDatasets.Where(i => i.PhenomenonOfferingID == phenomenonOffering.Id && i.StationID == station.Id && i.SiteID == site.Id));
                                break;
                            case "Phenomenon":
                                phenomenon = new PhenomenonOffering(node.ID).Phenomenon;
                                station = new Station(new Guid(GetItem(node.Items, "Station")));
                                site = new SAEON.Observations.Data.Site(new Guid(GetItem(node.Items, "Site")));
                                selectedDatasets.AddRange(allDatasets.Where(i => i.PhenomenonID == phenomenon.Id && i.StationID == station.Id && i.SiteID == site.Id));
                                break;
                            case "Sensor":
                                station = new Station(new Guid(GetItem(node.Items, "Station")));
                                site = new SAEON.Observations.Data.Site(new Guid(GetItem(node.Items, "Site")));
                                selectedDatasets.AddRange(allDatasets.Where(i => i.StationID == station.Id && i.SiteID == site.Id));
                                break;
                            case "Instrument":
                                station = new Station(new Guid(GetItem(node.Items, "Station")));
                                site = new SAEON.Observations.Data.Site(new Guid(GetItem(node.Items, "Site")));
                                selectedDatasets.AddRange(allDatasets.Where(i => i.StationID == station.Id && i.SiteID == site.Id));
                                break;
                            case "Station":
                                station = new Station(node.ID);
                                site = new SAEON.Observations.Data.Site(new Guid(GetItem(node.Items, "Site")));
                                selectedDatasets.AddRange(allDatasets.Where(i => i.StationID == station.Id && i.SiteID == site.Id));
                                break;
                            case "Site":
                                site = new SAEON.Observations.Data.Site(node.ID);
                                selectedDatasets.AddRange(allDatasets.Where(i => i.StationID == station.Id && i.SiteID == site.Id));
                                break;
                        }
                    }
                    selectedDatasets = selectedDatasets.Distinct().ToList();
                    SAEONLogs.Verbose("All: {All} Selected: {Selected}", allDatasets.Count, selectedDatasets.Count);
                    SAEONLogs.Verbose("Datasets: {@Datasets}", selectedDatasets);
                    foreach (var dataset in selectedDatasets)
                    {
                        result.AddRange(DatasetHelper.Load(dataset.Id));
                    }
                }
                SAEONLogs.Information("Loaded: {Elapsed}", stopwatch.Elapsed.TimeStr());
                return result;
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                throw;
            }
        }
    }

    protected void ObservationsGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        using (SAEONLogs.MethodCall(GetType()))
        {
            try
            {
                if (FilterTree.CheckedNodes == null)
                {
                    ObservationsGrid.GetStore().DataSource = null;
                }
                else
                {
                    ObservationsGrid.GetStore().DataSource = LoadData().Take(10);
                    ObservationsGrid.GetStore().DataBind();
                    return;
                    var log = string.Empty;
                    var q = BuildQuery(out log);
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    ObservationsGrid.GetStore().DataSource = DataQueryRepository.GetPagedFilteredList(e, e.Parameters[GridFilters1.ParamPrefix], ref q);
                    SAEONLogs.Information("Loaded: {Elapsed}", stopwatch.Elapsed.TimeStr());
                }
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                throw;
            }
        }
    }



    public SqlQuery BuildQ(string json, string visCols, string sortCol, string sortDir, out string log)
    {
        using (SAEONLogs.MethodCall(GetType()))
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
                var q = BuildQuery(out log, colms);
                if (!string.IsNullOrEmpty(json))
                {
                    FilterConditions fc = new FilterConditions(json);
                    var filters = string.Empty;
                    foreach (FilterCondition condition in fc.Conditions)
                    {
                        switch (condition.FilterType)
                        {
                            case FilterType.Date:
                                switch (condition.Comparison.ToString())
                                {
                                    case "Eq":
                                        q.And(condition.Name).IsEqualTo(condition.Value);
                                        filters += $"{condition.Name} = {condition.Value}";
                                        break;
                                    case "Gt":
                                        q.And(condition.Name).IsGreaterThanOrEqualTo(condition.Value);
                                        filters += $"{condition.Name} >= {condition.Value}";
                                        break;
                                    case "Lt":
                                        q.And(condition.Name).IsLessThanOrEqualTo(condition.Value);
                                        filters += $"{condition.Name} <= {condition.Value}";

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
                                        filters += $"{condition.Name} = {condition.Value}";
                                        break;
                                    case "Gt":
                                        q.And(condition.Name).IsGreaterThanOrEqualTo(condition.Value);
                                        filters += $"{condition.Name} >= {condition.Value}";
                                        break;
                                    case "Lt":
                                        q.And(condition.Name).IsLessThanOrEqualTo(condition.Value);
                                        filters += $"{condition.Name} <= {condition.Value}";
                                        break;
                                    default:
                                        break;
                                }

                                break;

                            case FilterType.String:
                                q.And(condition.Name).Like("%" + condition.Value + "%");
                                filters += $"{condition.Name} like {condition.Value}";


                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                    }
                    if (!string.IsNullOrEmpty(filters))
                    {
                        log += $" Filters -> {filters}";
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
                SAEONLogs.Exception(ex);
                throw;
            }
        }
    }


}