using DocumentFormat.OpenXml.Packaging;
using Ext.Net;
using SAEON.Logs;
using SAEON.Observations.Core.Entities;
using SAEON.OpenXML;
using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;

public partial class Admin_ImportSetup : System.Web.UI.Page
{
    private readonly ObservationsDbContext dbContext = new ObservationsDbContext();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //TemplateFile.Text = @"G:\My Drive\Elwandle\Node Drive\Data Store\Observations\Observations Database Setup Template.xlsx";
        }
    }

    //private T GetValue<T>(object[,] array, int row, int col)
    //{
    //    try
    //    {
    //        var result = (T)array[row, col];
    //        if ((result is string s) && string.IsNullOrWhiteSpace(s))
    //            result = default;
    //        return result;
    //    }
    //    catch (Exception ex)
    //    {
    //        using (Logging.MethodCall(GetType()))
    //        {
    //            Logging.Exception(ex, "Row: {Row} Col: {Col} ExpectedType: {ExpectedType} ReturnedType: {ReturnedType} Value: {Value}", row, col, typeof(T).Name, array[row, col].GetType().Name, array[row, col]);
    //            throw;
    //        }
    //    }
    //}

    private DateTime? GetDate(object[,] array, int row, int col)
    {
        try
        {
            string result = array[row, col].ToString();
            return string.IsNullOrWhiteSpace(result) ? default(DateTime?) : DateTime.FromOADate(double.Parse(result));
        }
        catch (Exception ex)
        {
            using (Logging.MethodCall(GetType()))
            {
                Logging.Exception(ex, "Row: {Row} Col: {Col} Value: {Value}", row, col, array[row, col]);
                throw;
            }
        }
    }

    private Double? GetDouble(object[,] array, int row, int col)
    {
        try
        {
            string result = array[row, col].ToString();
            return string.IsNullOrWhiteSpace(result) ? default(double?) : double.Parse(result);
        }
        catch (Exception ex)
        {
            using (Logging.MethodCall(GetType()))
            {
                Logging.Exception(ex, "Row: {Row} Col: {Col} Value: {Value}", row, col, array[row, col]);
                throw;
            }
        }
    }

    private string GetString(object[,] array, int row, int col)
    {
        try
        {
            string result = array[row, col]?.ToString();
            return string.IsNullOrWhiteSpace(result) ? null : result;
        }
        catch (Exception ex)
        {
            using (Logging.MethodCall(GetType()))
            {
                Logging.Exception(ex, "Row: {Row} Col: {Col} Value: {Value}", row, col, array[row, col]);
                throw;
            }
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

        void SaveChanges()
        {
            try
            {
                dbContext.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                Logging.Exception(ex, "Errors: {Errors}", ex.EntityValidationErrors.SelectMany(i => i.ValidationErrors.Select(m => m.PropertyName + ": " + m.ErrorMessage)).ToList());
                throw;
            }
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
                    var saeonOrganisationId = dbContext.Organisations.First(i => i.Code == "SAEON").Id;
                    var ownerRoleId = dbContext.OrganisationRoles.First(i => i.Code == "Owner").Id;
                    var CSVTypeId = dbContext.DataSourceTypes.First(i => i.Code == "CSV").Id;
                    // Programmes
                    Logging.Information("Adding Programmes");
                    var programmes = ExcelHelper.GetRangeValues(doc, "Programmes!A3:F102");
                    var programmesList = ExcelHelper.GetRangeValues(doc, "Programmes!H3:J102");
                    for (int rProgramme = 0; rProgramme < programmesList.GetUpperBound(0) + 1; rProgramme++)
                    {
                        var programmeCode = GetString(programmesList, rProgramme, 0);
                        //Logging.Verbose("Row: {Row} Code: {Code}", rProgramme, programmeCode);
                        if (string.IsNullOrWhiteSpace(programmeCode)) continue;
                        var programme = dbContext.Programmes.FirstOrDefault(i => i.Code == programmeCode);
                        if (programme != null)
                        {
                            Logging.Verbose("Ignoring Programme {ProgrammeCode}", programmeCode);
                        }
                        else
                        {
                            programme = new Programme
                            {
                                Code = programmeCode,
                                Name = (string)programmesList[rProgramme, 1],
                                Description = GetString(programmesList, rProgramme, 2),
                                Url = GetString(programmes, rProgramme, 3),
                                StartDate = GetDate(programmes, rProgramme, 4),
                                EndDate = GetDate(programmes, rProgramme, 5),
                                UserId = AuthHelper.GetLoggedInUserId
                            };
                            //Logging.Verbose("Adding programme {@Programme}", programme);
                            dbContext.Programmes.Add(programme);
                            SaveChanges();
                            Logging.Verbose("Added Programme {ProgrammeCode}", programmeCode);
                        }
                    }
                    // Projects
                    Logging.Information("Adding Projects");
                    var projectProgrammes = ExcelHelper.GetRangeValues(doc, "Projects!A3:B102");
                    var projects = ExcelHelper.GetRangeValues(doc, "Projects!D3:I102");
                    var projectsList = ExcelHelper.GetRangeValues(doc, "Projects!K3:M102");
                    for (int rProject = 0; rProject < projectsList.GetUpperBound(0) + 1; rProject++)
                    {
                        var projectCode = GetString(projectsList, rProject, 0);
                        if (string.IsNullOrWhiteSpace(projectCode)) continue;
                        var project = dbContext.Projects.FirstOrDefault(i => i.Code == projectCode);
                        if (project != null)
                        {
                            Logging.Verbose("Ignoring Project {ProjectCode}", projectCode);
                        }
                        else
                        {
                            var programmeCode = GetString(projectProgrammes, rProject, 0);
                            project = new Project
                            {
                                ProgrammeId = dbContext.Programmes.First(i => i.Code == programmeCode).Id,
                                Code = projectCode,
                                Name = GetString(projectsList, rProject, 1),
                                Description = GetString(projectsList, rProject, 2),
                                Url = GetString(projects, rProject, 3),
                                StartDate = GetDate(projects, rProject, 4),
                                EndDate = GetDate(projects, rProject, 5),
                                UserId = AuthHelper.GetLoggedInUserId
                            };
                            //Logging.Verbose("Adding Project {@Project}", project);
                            dbContext.Projects.Add(project);
                            SaveChanges();
                            Logging.Verbose("Added Project {ProjectCode}", projectCode);
                        }
                    }
                    // Sites
                    Logging.Information("Adding Sites");
                    var sites = ExcelHelper.GetRangeValues(doc, "Sites!A3:F102");
                    var sitesList = ExcelHelper.GetRangeValues(doc, "Sites!H3:J102");
                    for (int rSite = 0; rSite < sitesList.GetUpperBound(0) + 1; rSite++)
                    {
                        var siteCode = GetString(sitesList, rSite, 0);
                        if (string.IsNullOrWhiteSpace(siteCode)) continue;
                        var site = dbContext.Sites.FirstOrDefault(i => i.Code == siteCode);
                        if (site != null)
                        {
                            Logging.Verbose("Ignoring Site {SiteCode}", siteCode);
                        }
                        else
                        {
                            site = new SAEON.Observations.Core.Entities.Site
                            {
                                Code = siteCode,
                                Name = GetString(sitesList, rSite, 1),
                                Description = GetString(sitesList, rSite, 2),
                                Url = GetString(sites, rSite, 3),
                                StartDate = GetDate(sites, rSite, 4),
                                EndDate = GetDate(sites, rSite, 5),
                                UserId = AuthHelper.GetLoggedInUserId
                            };
                            //Logging.Verbose("Adding Site {@Site}", site);
                            dbContext.Sites.Add(site);
                            SaveChanges();
                            var siteId = dbContext.Sites.First(i => i.Code == siteCode).Id;
                            var sql = 
                                "Insert Organisation_Site " +
                                "  (OrganisationID, SiteID, OrganisationRoleID, UserID) " +
                                "Values " +
                               $"  ('{saeonOrganisationId}','{siteId}','{ownerRoleId}','{AuthHelper.GetLoggedInUserId}')";
                            //Logging.Verbose("Sql: {Sql}", sql);
                            dbContext.Database.ExecuteSqlCommand(sql);
                            Logging.Verbose("Added Site {SiteCode}", siteCode);
                        }
                    }
                    // Stations
                    Logging.Information("Adding Stations");
                    var stationProjects = ExcelHelper.GetRangeValues(doc, "Stations!A3:B102");
                    var stationSites = ExcelHelper.GetRangeValues(doc, "Stations!D3:E102");
                    var stations = ExcelHelper.GetRangeValues(doc, "Stations!G3:O102");
                    var stationsList = ExcelHelper.GetRangeValues(doc, "Stations!Q3:S102");
                    for (int rStation = 0; rStation < stationsList.GetUpperBound(0) + 1; rStation++)
                    {
                        var stationCode = GetString(stationsList, rStation, 0);
                        if (string.IsNullOrWhiteSpace(stationCode)) continue;
                        var station = dbContext.Stations.FirstOrDefault(i => i.Code == stationCode);
                        if (station != null)
                        {
                            Logging.Verbose("Ignoring Station {StationCode}", stationCode);
                        }
                        else
                        {
                            var siteCode = GetString(stationSites, rStation, 0);
                            station = new Station
                            {
                                SiteId = dbContext.Sites.First(i => i.Code == siteCode).Id,
                                Code = stationCode,
                                Name = GetString(stationsList, rStation, 1),
                                Description = GetString(stationsList, rStation, 2),
                                Url = GetString(stations, rStation, 3),
                                Latitude = GetDouble(stations, rStation, 4),
                                Longitude = GetDouble(stations, rStation, 5),
                                Elevation = GetDouble(stations, rStation, 6),
                                StartDate = GetDate(stations, rStation, 7),
                                EndDate = GetDate(stations, rStation, 8),
                                UserId = AuthHelper.GetLoggedInUserId
                            };
                            //Logging.Verbose("Adding Station {@Station}", station);
                            dbContext.Stations.Add(station);
                            SaveChanges();
                            var stationId = dbContext.Stations.First(i => i.Code == stationCode).Id;
                            var projectCode = GetString(stationProjects, rStation, 0);
                            var projectId = dbContext.Projects.First(i => i.Code == projectCode).Id;
                            var sql =
                                "Insert Project_Station " +
                                "  (ProjectID, StationID, UserID) " +
                                "Values " +
                               $"  ('{projectId}','{stationId}','{AuthHelper.GetLoggedInUserId}')";
                            //Logging.Verbose("Sql: {Sql}", sql);
                            dbContext.Database.ExecuteSqlCommand(sql);
                            Logging.Verbose("Added Station {StationCode}", stationCode);
                        }
                    }
                    // Instruments
                    Logging.Information("Adding Instruments");
                    var instrumentStations = ExcelHelper.GetRangeValues(doc, "Instruments!A3:B102");
                    var instrumentInstrumentTypes = ExcelHelper.GetRangeValues(doc, "Instruments!D3:E102");
                    var instrumentManufacturers = ExcelHelper.GetRangeValues(doc, "Instruments!G3:H102");
                    var instruments = ExcelHelper.GetRangeValues(doc, "Instruments!J3:R102");
                    var instrumentsList = ExcelHelper.GetRangeValues(doc, "Instruments!T3:W102");
                    var instrumentsListShort = ExcelHelper.GetRangeValues(doc, "Instruments!Y3:AA102");
                    for (int rInstrument = 0; rInstrument < instrumentsList.GetUpperBound(0) + 1; rInstrument++)
                    {
                        var instrumentCode =GetString(instrumentsList,rInstrument, 0);
                        if (string.IsNullOrWhiteSpace(instrumentCode)) continue;
                        var instrument = dbContext.Instruments.FirstOrDefault(i => i.Code == instrumentCode);
                        if (instrument != null)
                        {
                            Logging.Verbose("Ignoring Instrument {InstrumentCode}", instrumentCode);
                        }
                        else
                        {
                            instrument = new Instrument
                            {
                                Code = instrumentCode,
                                Name = GetString(instrumentsList,rInstrument, 1),
                                Description = GetString(instrumentsList,rInstrument, 2),
                                Url = GetString(instruments,rInstrument, 3),
                                Latitude = GetDouble(instruments,rInstrument, 4),
                                Longitude = GetDouble(instruments,rInstrument, 5),
                                Elevation = GetDouble(instruments,rInstrument, 6),
                                StartDate = GetDate(instruments,rInstrument, 7),
                                EndDate = GetDate(instruments,rInstrument, 8),
                                UserId = AuthHelper.GetLoggedInUserId
                            };
                            //Logging.Verbose("Adding Instrument {@Instrument}", instrument);
                            dbContext.Instruments.Add(instrument);
                            SaveChanges();
                            var instrumentId = dbContext.Instruments.First(i => i.Code == instrumentCode).Id;
                            var stationCode = GetString(instrumentStations,rInstrument, 0);
                            var stationId = dbContext.Stations.First(i => i.Code == stationCode).Id;
                            var sql =
                                "Insert Station_Instrument " +
                                "  (StationID, InstrumentID, UserID) " +
                                "Values " +
                               $"  ('{stationId}','{instrumentId}','{AuthHelper.GetLoggedInUserId}')";
                            //Logging.Verbose("Sql: {Sql}", sql);
                            dbContext.Database.ExecuteSqlCommand(sql);
                            Logging.Verbose("Added Instrument {InstrumentCode}", instrumentCode);
                        }
                    }
                    // DataSchemas
                    Logging.Information("Adding DataSchemas");
                    for (int rInstrument = 0; rInstrument < instrumentsList.GetUpperBound(0) + 1; rInstrument++)
                    {
                        var dataSchemaCode = GetString(instrumentsListShort,rInstrument,0);
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
                                Name = GetString(instrumentsListShort, rInstrument, 1),
                                Description = GetString(instrumentsListShort, rInstrument, 2),
                                DataSourceTypeId = CSVTypeId,
                                UserId = AuthHelper.GetLoggedInUserId
                            };
                            //Logging.Verbose("Adding DataShcema {@DataShcema}", dataSchema);
                            dbContext.DataSchemas.Add(dataSchema);
                            SaveChanges();
                            Logging.Verbose("Added DataSchema {DataSchemaCode}", dataSchemaCode);
                        }
                    }
                    /*
                    // DataSources
                    Logging.Information("Adding DataSources");
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
                            try
                            {
                                dbContext.SaveChanges();
                            }
                            catch (DbEntityValidationException ex)
                            {
                                Logging.Exception(ex, "Errors: {Errors}", ex.EntityValidationErrors.SelectMany(i => i.ValidationErrors.Select(m => m.PropertyName + ": " + m.ErrorMessage)).ToList());
                                throw;
                            }
                            Logging.Verbose("Added DataSource {DataSourceCode}", dataSourceCode);
                        }
                    }
                    // Phenomena
                    Logging.Information("Adding Phenomena");
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
                        else
                        {
                            phenomenon = new Phenomenon
                            {
                                Code = phenomenonCode,
                                Name = (string)phenomena[rPhenomenon, 1],
                                Description = (string)phenomena[rPhenomenon, 2],
                                Url = (string)phenomena[rPhenomenon, 3]
                            };
                            dbContext.Phenomena.Add(phenomenon);
                            try
                            {
                                dbContext.SaveChanges();
                            }
                            catch (DbEntityValidationException ex)
                            {
                                Logging.Exception(ex, "Errors: {Errors}", ex.EntityValidationErrors.SelectMany(i => i.ValidationErrors.Select(m => m.PropertyName + ": " + m.ErrorMessage)).ToList());
                                throw;
                            }
                            Logging.Verbose("Added Phenomenon {PhenomenonCode}", phenomenonCode);
                        }
                    }
                    // Sensors
                    Logging.Information("Adding Sensors");
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
                        else
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
                            try
                            {
                                dbContext.SaveChanges();
                            }
                            catch (DbEntityValidationException ex)
                            {
                                Logging.Exception(ex, "Errors: {Errors}", ex.EntityValidationErrors.SelectMany(i => i.ValidationErrors.Select(m => m.PropertyName + ": " + m.ErrorMessage)).ToList());
                                throw;
                            }
                            Logging.Verbose("Added Sensor {SensorCode}", sensorCode);
                        }
                    }
                    */
                }
                ExtNet.Msg.Hide();
                MessageBoxes.Info("Information", "Done");
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