using DocumentFormat.OpenXml.Packaging;
using Ext.Net;
using SAEON.Logs;
using SAEON.Observations.Core.Entities;
using SAEON.OpenXML;
using System;
using System.IO;
using System.Linq;

public partial class Admin_ImportSetup : System.Web.UI.Page
{
    private ObservationsDbContext dbContext = new ObservationsDbContext();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //TemplateFile.Text = @"G:\My Drive\Elwandle\Node Drive\Data Store\Observations\Observations Database Setup Template.xlsx";
        }
    }

    protected void CreateClick(object sender, DirectEventArgs e)
    {
        int FindRowIndex(object[,] array, int col, string value)
        {
            for (int row = array.GetLowerBound(col); row < array.GetUpperBound(col) + 1; row++)
            {
                if ((string)array[row, col] == value)
                {
                    return row;
                }
            }
            return -1;
        }

        using (Logging.MethodCall(GetType()))
        {
            if (!TemplateFile.HasFile)
            {
                ExtNet.Msg.Hide();
                MessageBoxes.Error("Error", "No Template Spreadsheet selected");
                return;
            }
            try
            {
                Logging.Information("FileName: {FileName}", TemplateFile.PostedFile.FileName);
                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(TemplateFile.PostedFile.InputStream, false))
                {
                    // Programmes
                    var programmes = ExcelHelper.GetRangeValues(doc, "Programmes!A3:G102");
                    var programmesList = ExcelHelper.GetRangeValues(doc, "Programmes!H3:I102");
                    for (int rProgramme = 0; rProgramme < programmesList.GetUpperBound(0) + 1; rProgramme++)
                    {
                        var programmeCode = (string)programmesList[rProgramme, 0];
                        if (string.IsNullOrWhiteSpace(programmeCode)) continue;
                        var programme = dbContext.Programmes.FirstOrDefault(i => i.Code == programmeCode);
                        if (programme != null)
                        {
                            Logging.Verbose("Ignoring Programme {ProgrammeCode}", programmeCode);
                        }
                        {
                            programme = new Programme
                            {
                                Code = programmeCode,
                                Name = (string)programmesList[rProgramme, 1],
                                Description = (string)programmes[rProgramme, 2],
                                Url = (string)programmes[rProgramme, 3],
                                StartDate = (DateTime?)programmes[rProgramme, 4],
                                EndDate = (DateTime?)programmes[rProgramme, 5]
                            };
                            dbContext.Programmes.Add(programme);
                            dbContext.SaveChanges();
                            Logging.Verbose("Added Programme {ProgrammeCode}", programmeCode);
                        }
                    }
                    // Projects
                    var projectProgrammes = ExcelHelper.GetRangeValues(doc, "Projects!A3:B102");
                    var projects = ExcelHelper.GetRangeValues(doc, "Projects!D3:I102");
                    var projectsList = ExcelHelper.GetRangeValues(doc, "Projects!K3:L102");
                    for (int rProject = 0; rProject < projectsList.GetUpperBound(0) + 1; rProject++)
                    {
                        var projectCode = (string)projectsList[rProject, 0];
                        if (string.IsNullOrWhiteSpace(projectCode)) continue;
                        var project = dbContext.Projects.FirstOrDefault(i => i.Code == projectCode);
                        if (project != null)
                        {
                            Logging.Verbose("Ignoring Project {ProjectCode}", projectCode);
                        }
                        {
                            project = new Project
                            {
                                Code = projectCode,
                                Name = (string)projectsList[rProject, 1],
                                ProgrammeId = dbContext.Programmes.First(i => i.Code == (string)projectProgrammes[rProject, 0]).Id,
                                Description = (string)projects[rProject, 5],
                                Url = (string)projects[rProject, 6],
                                StartDate = (DateTime?)projects[rProject, 7],
                                EndDate = (DateTime?)projects[rProject, 8]
                            };
                            dbContext.Projects.Add(project);
                            dbContext.SaveChanges();
                            Logging.Verbose("Added Project {ProjectCode}", projectCode);
                        }
                    }
                    // Sites
                    var sites = ExcelHelper.GetRangeValues(doc, "Sites!A3:F102");
                    var sitesList = ExcelHelper.GetRangeValues(doc, "Sites!H3:I102");
                    for (int rSite = 0; rSite < sitesList.GetUpperBound(0) + 1; rSite++)
                    {
                        var siteCode = (string)sitesList[rSite, 0];
                        if (string.IsNullOrWhiteSpace(siteCode)) continue;
                        var site = dbContext.Sites.FirstOrDefault(i => i.Code == siteCode);
                        if (site != null)
                        {
                            Logging.Verbose("Ignoring Site {SiteCode}", siteCode);
                        }
                        {
                            site = new SAEON.Observations.Core.Entities.Site
                            {
                                Code = siteCode,
                                Name = (string)sitesList[rSite, 1],
                                Description = (string)sites[rSite, 2],
                                Url = (string)sites[rSite, 3],
                                StartDate = (DateTime?)sites[rSite, 4],
                                EndDate = (DateTime?)sites[rSite, 5]
                            };
                            dbContext.Sites.Add(site);
                            dbContext.SaveChanges();
                            Logging.Verbose("Added Site {SiteCode}", siteCode);
                        }
                    }
                    // Stations
                    var stationProjects = ExcelHelper.GetRangeValues(doc, "Stations!A3:B102");
                    var stationSites = ExcelHelper.GetRangeValues(doc, "Stations!D3:E102");
                    var stations = ExcelHelper.GetRangeValues(doc, "Stations!G3:O102");
                    var stationsList = ExcelHelper.GetRangeValues(doc, "Stations!Q3:R102");
                    for (int rStation = 0; rStation < stationsList.GetUpperBound(0) + 1; rStation++)
                    {
                        var stationCode = (string)stationsList[rStation, 0];
                        if (string.IsNullOrWhiteSpace(stationCode)) continue;
                        var station = dbContext.Stations.FirstOrDefault(i => i.Code == stationCode);
                        if (station != null)
                        {
                            Logging.Verbose("Ignoring Station {StationCode}", stationCode);
                        }
                        {
                            station = new Station
                            {
                                Code = stationCode,
                                Name = (string)stationsList[rStation, 1],
                                SiteId = dbContext.Sites.First(i => i.Code == (string)stationSites[rStation, 0]).Id,
                                Description = (string)stations[rStation, 2],
                                Url = (string)stations[rStation, 3],
                                Latitude = (double?)stations[rStation, 4],
                                Longitude = (double?)stations[rStation, 5],
                                Elevation = (double?)stations[rStation, 6],
                                StartDate = (DateTime?)stations[rStation, 7],
                                EndDate = (DateTime?)stations[rStation, 8]
                            };
                            station.Projects.Add(dbContext.Projects.First(i => i.Code == (string)stationProjects[rStation, 0]));
                            dbContext.Stations.Add(station);
                            dbContext.SaveChanges();
                            Logging.Verbose("Added Station {StationCode}", stationCode);
                        }
                    }
                    // Instruments
                    var instrumentStations = ExcelHelper.GetRangeValues(doc, "Instruments!A3:B102");
                    var instruments = ExcelHelper.GetRangeValues(doc, "Instruments!J3:R102");
                    var instrumentsList = ExcelHelper.GetRangeValues(doc, "Instruments!T3:U102");
                    for (int rInstrument = 0; rInstrument < instrumentsList.GetUpperBound(0) + 1; rInstrument++)
                    {
                        var instrumentCode = (string)instrumentsList[rInstrument, 0];
                        if (string.IsNullOrWhiteSpace(instrumentCode)) continue;
                        var instrument = dbContext.Instruments.FirstOrDefault(i => i.Code == instrumentCode);
                        if (instrument != null)
                        {
                            Logging.Verbose("Ignoring Instrument {InstrumentCode}", instrumentCode);
                        }
                        {
                            instrument = new Instrument
                            {
                                Code = instrumentCode,
                                Name = (string)instrumentsList[rInstrument, 1],
                                Description = (string)instruments[rInstrument, 2],
                                Url = (string)instruments[rInstrument, 3],
                                Latitude = (double?)instruments[rInstrument, 4],
                                Longitude = (double?)instruments[rInstrument, 5],
                                Elevation = (double?)instruments[rInstrument, 6],
                                StartDate = (DateTime?)instruments[rInstrument, 7],
                                EndDate = (DateTime?)instruments[rInstrument, 8]
                            };
                            instrument.Stations.Add(dbContext.Stations.First(i => i.Code == (string)instrumentStations[rInstrument, 0]));
                            dbContext.Instruments.Add(instrument);
                            dbContext.SaveChanges();
                            Logging.Verbose("Added Instrument {InstrumentCode}", instrumentCode);
                        }
                    }
                    // DataSchemas
                    var instrumentInstrumentTypes = ExcelHelper.GetRangeValues(doc, "Instruments!D3:E102");
                    var instrumentManufacturers = ExcelHelper.GetRangeValues(doc, "Instruments!G3:H102");
                    for (int rInstrument = 0; rInstrument < instrumentsList.GetUpperBound(0) + 1; rInstrument++)
                    {
                        var dataSchemaCode = $"{instrumentInstrumentTypes[rInstrument, 0]}-{instrumentManufacturers[rInstrument, 0]}-{instruments[rInstrument, 0]}";
                        if (string.IsNullOrWhiteSpace(dataSchemaCode)) continue;
                        var dataSchema = dbContext.DataSchemas.FirstOrDefault(i => i.Code == dataSchemaCode);
                        if (dataSchema != null)
                        {
                            Logging.Verbose("Ignoring DataSchema {DataSchemaCode}", dataSchemaCode);
                        }
                        else
                        {
                            dataSchema = new DataSchema
                            {
                                Code = dataSchemaCode,
                                Name = $"{instrumentInstrumentTypes[rInstrument, 1]}, {instrumentManufacturers[rInstrument, 1]}, {instruments[rInstrument, 1]}",
                                Description = $"{instrumentInstrumentTypes[rInstrument, 1]}, {instrumentManufacturers[rInstrument, 1]}, {instruments[rInstrument, 1]}",
                                DataSourceTypeId = dbContext.DataSourceTypes.First(i => i.Code == "CSV").Id
                            };
                            dbContext.DataSchemas.Add(dataSchema);
                            Logging.Verbose("Added DataSchema {DataSchemaCode}", dataSchemaCode);
                        }
                    }
                    // DataSources
                    for (int rInstrument = 0; rInstrument < instrumentsList.GetUpperBound(0) + 1; rInstrument++)
                    {
                        var dataSchemaCode = $"{instrumentInstrumentTypes[rInstrument, 0]}-{instrumentManufacturers[rInstrument, 0]}-{instruments[rInstrument, 0]}";
                        var dataSourceCode = $"{instrumentInstrumentTypes[rInstrument, 0]}-{instrumentManufacturers[rInstrument, 0]}-{instruments[rInstrument, 0]}-{instruments[rInstrument, 1]}".TrimEnd('-');
                        if (string.IsNullOrWhiteSpace(dataSourceCode)) continue;
                        var dataSource = dbContext.DataSources.FirstOrDefault(i => i.Code == dataSourceCode);
                        if (dataSource != null)
                        {
                            Logging.Verbose("Ignoring DataSource {DataSourceCode}", dataSourceCode);
                        }
                        else
                        {
                            dataSource = new DataSource
                            {
                                Code = dataSourceCode,
                                Name = $"{instrumentInstrumentTypes[rInstrument, 1]}, {instrumentManufacturers[rInstrument, 1]}, {instruments[rInstrument, 1]}",
                                Description = $"{instrumentInstrumentTypes[rInstrument, 1]}, {instrumentManufacturers[rInstrument, 1]}, {instruments[rInstrument, 1]}",
                                DataSchemaId = dbContext.DataSchemas.First(i => i.Code == dataSchemaCode).Id
                            };
                            dbContext.DataSources.Add(dataSource);
                            Logging.Verbose("Added DataSource {DataSourceCode}", dataSourceCode);
                        }
                    }
                    // Phenomena
                    var phenomena = ExcelHelper.GetRangeValues(doc, "Phenomena!A3:D102");
                    for (int rPhenomenon = 0; rPhenomenon < phenomena.GetUpperBound(0) + 1; rPhenomenon++)
                    {
                        var phenomenonCode = (string)phenomena[rPhenomenon, 0];
                        if (string.IsNullOrWhiteSpace(phenomenonCode)) continue;
                        var phenomenon = dbContext.Phenomena.FirstOrDefault(i => i.Code == phenomenonCode);
                        if (phenomenon != null)
                        {
                            Logging.Verbose("Ignoring Phenomenon {PhenomenonCode}", phenomenonCode);
                        }
                        {
                            phenomenon = new Phenomenon
                            {
                                Code = phenomenonCode,
                                Name = (string)phenomena[rPhenomenon, 1],
                                Description = (string)phenomena[rPhenomenon, 2],
                                Url = (string)phenomena[rPhenomenon, 3]
                            };
                            dbContext.Phenomena.Add(phenomenon);
                            dbContext.SaveChanges();
                            Logging.Verbose("Added Phenomenon {PhenomenonCode}", phenomenonCode);
                        }
                    }
                    // Sensors
                    var sensorInstruments = ExcelHelper.GetRangeValues(doc, "Sensors!A3:B102");
                    var sensorPhenomena = ExcelHelper.GetRangeValues(doc, "Sensors!D3:E102");
                    var sensors = ExcelHelper.GetRangeValues(doc, "Sensors!G3:N102");
                    var sensorsList = ExcelHelper.GetRangeValues(doc, "Sensors!P3:Q102");
                    for (int rSensor = 0; rSensor < sensorsList.GetUpperBound(0) + 1; rSensor++)
                    {
                        var sensorCode = (string)sensorsList[rSensor, 0];
                        if (string.IsNullOrWhiteSpace(sensorCode)) continue;
                        var sensor = dbContext.Sensors.FirstOrDefault(i => i.Code == sensorCode);
                        if (sensor != null)
                        {
                            Logging.Verbose("Ignoring Sensor {SensorCode}", sensorCode);
                        }
                        {
                            var instrumentCode = (string)sensorInstruments[rSensor, 0];
                            var rInstrument = FindRowIndex(instrumentsList, 0, instrumentCode);
                            var dataSourceCode = $"{instrumentInstrumentTypes[rInstrument, 0]}-{instrumentManufacturers[rInstrument, 0]}-{instruments[rInstrument, 0]}-{instruments[rInstrument, 1]}".TrimEnd('-');
                            var phenomenaCode = (string)sensorPhenomena[rSensor, 0];
                            sensor = new Sensor
                            {
                                Code = sensorCode,
                                Name = (string)sensorsList[rSensor, 1],
                                Description = (string)sensors[rSensor, 1],
                                Url = (string)sensors[rSensor, 2],
                                Latitude = (double?)sensors[rSensor, 3],
                                Longitude = (double?)sensors[rSensor, 4],
                                Elevation = (double?)sensors[rSensor, 5],
                                DataSourceId = dbContext.DataSources.First(i => i.Code == dataSourceCode).Id,
                                PhenomenonId = dbContext.Phenomena.First(i => i.Code == phenomenaCode).Id
                            };
                            dbContext.Sensors.Add(sensor);
                            dbContext.SaveChanges();
                            Logging.Verbose("Added Sensor {SensorCode}", sensorCode);
                        }
                    }
                }
                ExtNet.Msg.Hide();
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
                ExtNet.Msg.Hide();
                MessageBoxes.Error("Error", ex.Message);
            }
        }
    }

    protected void ValidateTemplateFile(object sender, RemoteValidationEventArgs e)
    {
        if (File.Exists(TemplateFile.Text))
        {
            e.Success = true;
        }
        else
        {
            e.Success = false;
            e.ErrorMessage = "Template spreadsheet does not exist";
        }
    }
}