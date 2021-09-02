﻿using Ext.Net;
using Newtonsoft.Json;
using SAEON.Core;
using SAEON.Logs;
using SAEON.Observations.Data;
using SubSonic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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

    //private PhenomenonOfferingCollection GetPhenomenonOfferings(Guid sensorId)
    //{
    //    QueryCommand cmd = new QueryCommand("Select Distinct * from PhenomenonOffering inner join ImportBatchSummary on (ImportBatchSummary.PhenomenonOfferingID = PhenomenonOffering.ID) where (SensorID = @SensorID)", PhenomenonOffering.Schema.Provider.Name);
    //    cmd.AddParameter("@SensorID", sensorId, DbType.Guid);
    //    PhenomenonOfferingCollection result = new PhenomenonOfferingCollection();
    //    result.LoadAndCloseReader(DataService.GetReader(cmd));
    //    return result;
    //}

    protected void NodeLoad(object sender, NodeLoadEventArgs e)
    {
        if (!ConfigurationManager.AppSettings["AsyncTree"].IsTrue()) return;

        using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "NodeID", e.NodeID } }))
            try
            {
                if (e.NodeID.StartsWith("Organisations"))
                {
                    var col = new Select()
                                .From(Organisation.Schema)
                                .InnerJoin(VInventoryDataset.Schema.TableName, VInventoryDataset.Columns.OrganisationID, Organisation.Schema.TableName, Organisation.Columns.Id)
                                .Distinct()
                                .OrderAsc(Organisation.Columns.Name)
                                .ExecuteAsCollection<OrganisationCollection>();
                    SAEONLogs.Verbose("Organisations: {count}", col.Count());
                    foreach (var item in col)
                    {
                        SAEONLogs.Verbose("Organisation: {name}", item.Name);
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
                                .InnerJoin(VInventoryDataset.Schema.TableName, VInventoryDataset.Columns.SiteID, SAEON.Observations.Data.Site.Schema.TableName, SAEON.Observations.Data.Site.Columns.Id)
                                .Where(VInventoryDataset.Columns.OrganisationID)
                                .IsEqualTo(organisation.Id)
                                .Distinct()
                                .OrderAsc(SAEON.Observations.Data.Site.Columns.Name)
                                .ExecuteAsCollection<SiteCollection>();
                    SAEONLogs.Verbose("Sites: {count}", col.Count());
                    foreach (var item in col)
                    {
                        SAEONLogs.Verbose("Site: {name}", item.Name);
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
                                .InnerJoin(VInventoryDataset.Schema.TableName, VInventoryDataset.Columns.StationID, Station.Schema.TableName, Station.Columns.Id)
                                .Where(VInventoryDataset.Columns.SiteID)
                                .IsEqualTo(site.Id)
                                .Distinct()
                                .OrderAsc(Station.Columns.Name)
                                .ExecuteAsCollection<StationCollection>();
                    SAEONLogs.Verbose("Stations: {count}", col.Count());
                    foreach (var item in col)
                    {
                        SAEONLogs.Verbose("Station: {name}", item.Name);
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
                    SAEONLogs.Verbose("Instruments: {count}", col.Count());
                    foreach (var item in col)
                    {
                        SAEONLogs.Verbose("Instrument: {name}", item.Name);
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
                    SAEONLogs.Verbose("Sensors: {count}", col.Count());
                    foreach (var item in col)
                    {
                        SAEONLogs.Verbose("Sensor: {name}", item.Name);
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
                SAEONLogs.Exception(ex, "Unable to load node {nodeID}", e.NodeID);
                throw;
            }
    }

    void BuildAsyncTree()
    {
        AsyncTreeNode root = new AsyncTreeNode("Organisations", "Organisations")
        {
            Icon = (Icon)new ModuleX("e4c08bfa-a8f0-4112-b45c-dd1788ade5a0").Icon
        };
        FilterTree.Root.Add(root);
    }

    void BuildFullTree()
    {
        Ext.Net.TreeNode rootOrganisations = new Ext.Net.TreeNode("Organisations", "Organisations", (Icon)new ModuleX("e4c08bfa-a8f0-4112-b45c-dd1788ade5a0").Icon);
        FilterTree.Root.Add(rootOrganisations);
        var colOrganisations = new Select()
            .From(Organisation.Schema)
            .InnerJoin(VInventoryDataset.Schema.TableName, VInventoryDataset.Columns.OrganisationID, Organisation.Schema.TableName, Organisation.Columns.Id)
            .Distinct()
            .OrderAsc(Organisation.Columns.Name)
            .ExecuteAsCollection<OrganisationCollection>();
        SAEONLogs.Verbose("Organisations: {count}", colOrganisations.Count());
        foreach (var organisation in colOrganisations)
        {
            SAEONLogs.Verbose("Organisation: {name}", organisation.Name);
            Ext.Net.TreeNode nodeOrganisation = new Ext.Net.TreeNode("Organisation_" + organisation.Id.ToString(), organisation.Name, Icon.ResultsetNext);
            rootOrganisations.Nodes.Add(nodeOrganisation);
            Ext.Net.TreeNode rootSites = new Ext.Net.TreeNode("Sites_" + organisation.Id.ToString() + "|" + nodeOrganisation.NodeID, "Sites",
                (Icon)new ModuleX("A5C81FF7-69D6-4344-8548-E3EF7F08C4E7").Icon);
            nodeOrganisation.Nodes.Add(rootSites);
            var colSites = new Select()
                .From(SAEON.Observations.Data.Site.Schema)
                .InnerJoin(VInventoryDataset.Schema.TableName, VInventoryDataset.Columns.SiteID, SAEON.Observations.Data.Site.Schema.TableName, SAEON.Observations.Data.Site.Columns.Id)
                .Where(VInventoryDataset.Columns.OrganisationID)
                .IsEqualTo(organisation.Id)
                .Distinct()
                .OrderAsc(SAEON.Observations.Data.Site.Columns.Name)
                .ExecuteAsCollection<SiteCollection>();
            SAEONLogs.Verbose("Sites: {count}", colSites.Count());
            foreach (var site in colSites)
            {
                SAEONLogs.Verbose("Site: {name}", site.Name);
                Ext.Net.TreeNode nodeSite = new Ext.Net.TreeNode("Site_" + site.Id.ToString() + "|" + nodeOrganisation.NodeID, site.Name, Icon.ResultsetNext)
                {
                    Checked = ThreeStateBool.False
                };
                rootSites.Nodes.Add(nodeSite);
                Ext.Net.TreeNode rootStations = new Ext.Net.TreeNode("Stations_" + site.Id.ToString() + "|" + nodeSite.NodeID, "Stations",
                    (Icon)new ModuleX("0585e63d-0f9f-4dda-98ec-7de9397dc614").Icon);
                nodeSite.Nodes.Add(rootStations);
                var colStations = new Select()
                    .From(Station.Schema)
                    .InnerJoin(VInventoryDataset.Schema.TableName, VInventoryDataset.Columns.StationID, Station.Schema.TableName, Station.Columns.Id)
                    .Where(VInventoryDataset.Columns.SiteID)
                    .IsEqualTo(site.Id)
                    .Distinct()
                    .OrderAsc(Station.Columns.Name)
                    .ExecuteAsCollection<StationCollection>();
                SAEONLogs.Verbose("Stations: {count}", colStations.Count());
                foreach (var station in colStations)
                {
                    SAEONLogs.Verbose("Station: {name}", station.Name);
                    Ext.Net.TreeNode nodeStation = new Ext.Net.TreeNode("Station_" + station.Id.ToString() + "|" + nodeSite.NodeID, station.Name,
                        Icon.ResultsetNext)
                    {
                        Checked = ThreeStateBool.False
                    };
                    rootStations.Nodes.Add(nodeStation);
                    var qStation = new Select()
                        .From(ImportBatchSummary.Schema)
                        .Where(ImportBatchSummary.StationIDColumn)
                        .IsEqualTo(station.Id)
                        .GetRecordCount();
                    if (qStation == 0)
                        nodeStation.Leaf = true;
                    else
                    {
                        Ext.Net.TreeNode rootInstruments = new Ext.Net.TreeNode("Instruments_" + station.Id.ToString() + "|" + nodeStation.NodeID, "Instruments",
                            (Icon)new ModuleX("2610866B-8CBF-44E1-9A38-6511B31A8350").Icon);
                        nodeStation.Nodes.Add(rootInstruments);
                        var colInstruments = new Select()
                            .From(Instrument.Schema)
                            .InnerJoin(ImportBatchSummary.InstrumentIDColumn, Instrument.IdColumn)
                            .Where(ImportBatchSummary.StationIDColumn)
                            .IsEqualTo(station.Id)
                            .Distinct()
                            .OrderAsc(Instrument.Columns.Name)
                            .ExecuteAsCollection<InstrumentCollection>();
                        SAEONLogs.Verbose("Instruments: {count}", colInstruments.Count());
                        foreach (var instrument in colInstruments)
                        {
                            SAEONLogs.Verbose("Instrument: {name}", instrument.Name);
                            Ext.Net.TreeNode nodeInstrument = new Ext.Net.TreeNode("Instrument_" + instrument.Id.ToString() + "|" + nodeStation.NodeID, instrument.Name,
                                Icon.ResultsetNext)
                            {
                                Checked = ThreeStateBool.False
                            };
                            rootInstruments.Nodes.Add(nodeInstrument);
                            var qInstrument = new Select()
                                .From(ImportBatchSummary.Schema)
                                .Where(ImportBatchSummary.InstrumentIDColumn)
                                .IsEqualTo(instrument.Id)
                                .GetRecordCount();
                            if (qInstrument == 0)
                                nodeInstrument.Leaf = true;
                            else
                            {
                                Ext.Net.TreeNode rootSensors = new Ext.Net.TreeNode("Sensors_" + instrument.Id.ToString() + "|" + nodeInstrument.NodeID, "Sensors",
                                    (Icon)new ModuleX("9992ba10-cb0c-4a22-841c-1d695e8293d5").Icon);
                                nodeInstrument.Nodes.Add(rootSensors);
                                var colSensors = new Select()
                                    .From(Sensor.Schema)
                                    .InnerJoin(ImportBatchSummary.SensorIDColumn, Sensor.IdColumn)
                                    .Where(ImportBatchSummary.InstrumentIDColumn)
                                    .IsEqualTo(instrument.Id)
                                    .Distinct()
                                    .OrderAsc(Sensor.Columns.Name)
                                    .ExecuteAsCollection<SensorCollection>();
                                SAEONLogs.Verbose("Sensors: {count}", colSensors.Count());
                                foreach (var sensor in colSensors)
                                {
                                    SAEONLogs.Verbose("Sensor: {name}", sensor.Name);
                                    Ext.Net.TreeNode nodeSensor = new Ext.Net.TreeNode("Sensor_" + sensor.Id.ToString() + "|" + nodeInstrument.NodeID, sensor.Name, Icon.ResultsetNext)
                                    {
                                        Checked = ThreeStateBool.False
                                    };
                                    nodeInstrument.Nodes.Add(nodeSensor);
                                    var colPhenomenaOfferings = new Select(VImportBatchSummary.Columns.PhenomenonOfferingID, VImportBatchSummary.Columns.PhenomenonName, VImportBatchSummary.Columns.OfferingName)
                                        .From(VImportBatchSummary.Schema)
                                        .Where(VImportBatchSummary.Columns.SensorID)
                                        .IsEqualTo(sensor.Id)
                                        .OrderAsc(VImportBatchSummary.Columns.PhenomenonName)
                                        .OrderAsc(VImportBatchSummary.Columns.OfferingName)
                                        .Distinct()
                                        .ExecuteAsCollection<VImportBatchSummaryCollection>();
                                    if (colPhenomenaOfferings.Count == 0)
                                        nodeSensor.Leaf = true;
                                    else
                                    {
                                        Ext.Net.TreeNode phenomenonNode = null;
                                        foreach (var phenomenonOffering in colPhenomenaOfferings)
                                        {
                                            if (phenomenonNode?.Text != phenomenonOffering.PhenomenonName)
                                            {
                                                phenomenonNode = new Ext.Net.TreeNode("Phenomenon_" + phenomenonOffering.PhenomenonOfferingID.ToString() + "|" + nodeSensor.NodeID, phenomenonOffering.PhenomenonName, Icon.ResultsetNext);
                                                nodeSensor.Nodes.Add(phenomenonNode);
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
                    }
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void BuildTree()
    {
        using (SAEONLogs.MethodCall(GetType()))
            try
            {
                if (ConfigurationManager.AppSettings["AsyncTree"].IsTrue())
                {
                    BuildAsyncTree();
                }
                else
                {
                    BuildFullTree();
                }
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex, "Unable to load tree");
                throw;
            }
    }

    protected class QueryDataClass
    {
        public string NodeID { get; set; }
        public Guid ID { get; set; }
        public string Type { get; set; }
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

    private string GetItem(List<Tuple<string, string>> items, string itemType)
    {
        return items.Where(i => i.Item1 == itemType).Select(i => i.Item2).FirstOrDefault();
    }

    private SqlQuery BuildQuery(out string log, string[] columns = null)
    {
        using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "Columns", columns } }))
        {
            try
            {
                log = "Query -> ";
                DateTime fromDate = FromFilter.SelectedDate;
                DateTime toDate = ToFilter.SelectedDate;

                SqlQuery q = null;
                if ((columns == null) || (columns.Length == 0))
                    q = new Select().From(VObservationExpansion.Schema);
                else
                    q = new Select(columns).From(VObservationExpansion.Schema);

                if (FilterTree.CheckedNodes != null)
                {
                    List<SubmittedNode> nodes = FilterTree.CheckedNodes;
                    List<QueryDataClass> QueryDataClassList = new List<QueryDataClass>();

                    foreach (var item in nodes)
                    {
                        var items = item.NodeID.Split('|').Select(i => new Tuple<string, string>(i.Split('_')[0], i.Split('_')[1])).ToList();
                        QueryDataClassList.Add(new QueryDataClass() { NodeID = item.NodeID, ID = new Guid(items[0].Item2), Type = items[0].Item1 });
                    }

                    SAEONLogs.Verbose("Items: {@QueryDataClassList}", QueryDataClassList);

                    #region buildQ
                    foreach (QueryDataClass item in QueryDataClassList)
                    {

                        int count = 0;
                        List<Tuple<string, string>> items = item.NodeID.Split('|').Select(i => new Tuple<string, string>(i.Split('_')[0], i.Split('_')[1])).ToList();
                        SAEONLogs.Verbose("Items: {@items}", items);
                        PhenomenonOffering phenomenonOffering = null;
                        Phenomenon phenomenon = null;
                        Sensor sensor = null;
                        Instrument instrument = null;
                        Station station = null;
                        SAEON.Observations.Data.Site site = null;
                        switch (item.Type)
                        {
                            case "Offering":
                                count++;
                                phenomenonOffering = new PhenomenonOffering(item.ID);
                                sensor = new Sensor(new Guid(GetItem(items, "Sensor")));
                                instrument = new Instrument(new Guid(GetItem(items, "Instrument")));
                                station = new Station(new Guid(GetItem(items, "Station")));
                                site = new SAEON.Observations.Data.Site(new Guid(GetItem(items, "Site")));
                                q.OrExpression(VObservationExpansion.Columns.PhenomenonOfferingID).IsEqualTo(phenomenonOffering.Id)
                                    .And(VObservationExpansion.Columns.SensorID).IsEqualTo(sensor.Id)
                                    .And(VObservationExpansion.Columns.InstrumentID).IsEqualTo(instrument.Id)
                                    .And(VObservationExpansion.Columns.StationID).IsEqualTo(station.Id)
                                    .And(VObservationExpansion.Columns.SiteID).IsEqualTo(site.Id);
                                log += $"Site: {site.Name} Station: {station.Name} Instrument: {instrument.Name} Sensor: {sensor.Name} Phenomenon: {phenomenonOffering.Phenomenon.Name} Offering: {phenomenonOffering.Offering.Name}";
                                break;
                            case "Phenomenon":
                                count++;
                                phenomenon = new PhenomenonOffering(item.ID).Phenomenon;
                                sensor = new Sensor(new Guid(GetItem(items, "Sensor")));
                                instrument = new Instrument(new Guid(GetItem(items, "Instrument")));
                                station = new Station(new Guid(GetItem(items, "Station")));
                                site = new SAEON.Observations.Data.Site(new Guid(GetItem(items, "Site")));
                                q.OrExpression(VObservationExpansion.Columns.PhenomenonID).IsEqualTo(phenomenon.Id)
                                    .And(VObservationExpansion.Columns.SensorID).IsEqualTo(sensor.Id)
                                    .And(VObservationExpansion.Columns.InstrumentID).IsEqualTo(instrument.Id)
                                    .And(VObservationExpansion.Columns.StationID).IsEqualTo(station.Id)
                                    .And(VObservationExpansion.Columns.SiteID).IsEqualTo(site.Id);
                                log += $"Site: {site.Name} Station: {station.Name} Instrument: {instrument.Name} Sensor: {sensor.Name} Phenomenon: {phenomenonOffering.Phenomenon.Name}";
                                break;
                            case "Sensor":
                                count++;
                                sensor = new Sensor(item.ID);
                                instrument = new Instrument(new Guid(GetItem(items, "Instrument")));
                                station = new Station(new Guid(GetItem(items, "Station")));
                                site = new SAEON.Observations.Data.Site(new Guid(GetItem(items, "Site")));
                                q.OrExpression(VObservationExpansion.Columns.SensorID).IsEqualTo(sensor.Id)
                                    .And(VObservationExpansion.Columns.InstrumentID).IsEqualTo(instrument.Id)
                                    .And(VObservationExpansion.Columns.StationID).IsEqualTo(station.Id)
                                    .And(VObservationExpansion.Columns.SiteID).IsEqualTo(site.Id);
                                log += $"Site: {site.Name} Station: {station.Name} Instrument: {instrument.Name} Sensor: {sensor.Name}";
                                break;
                            case "Instrument":
                                count++;
                                instrument = new Instrument(item.ID);
                                station = new Station(new Guid(GetItem(items, "Station")));
                                site = new SAEON.Observations.Data.Site(new Guid(GetItem(items, "Site")));
                                q.OrExpression(VObservationExpansion.Columns.InstrumentID).IsEqualTo(instrument.Id)
                                    .And(VObservationExpansion.Columns.StationID).IsEqualTo(station.Id)
                                    .And(VObservationExpansion.Columns.SiteID).IsEqualTo(site.Id);
                                log += $"Site: {site.Name} Station: {station.Name} Instrument: {instrument.Name}";
                                break;
                            case "Station":
                                count++;
                                station = new Station(item.ID);
                                site = new SAEON.Observations.Data.Site(new Guid(GetItem(items, "Site")));
                                q.OrExpression(VObservationExpansion.Columns.StationID).IsEqualTo(station.Id)
                                    .And(VObservationExpansion.Columns.SiteID).IsEqualTo(site.Id);
                                log += $"Site: {site.Name} Station: {station.Name}";
                                break;
                            case "Site":
                                count++;
                                site = new SAEON.Observations.Data.Site(item.ID);
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
                return q;
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
                    var log = string.Empty;
                    var q = BuildQuery(out log);
                    ObservationsGrid.GetStore().DataSource = DataQueryRepository.GetPagedFilteredList(e, e.Parameters[GridFilters1.ParamPrefix], ref q);
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