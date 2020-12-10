using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SAEON.Logs;
using SAEON.Observations.Auth;
using SAEON.Observations.WebAPI.Hubs;
using SAEON.OpenXML;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI
{
    public static class ImportSetupHelper
    {
        public static bool UpdateData { get; set; } = false;

        public static async Task<string> ImportFromSpreadsheet(ObservationsDbContext dbContext, IHubContext<AdminHub> adminHub, IFormFile fileData)
        {
            using (SAEONLogs.MethodCall(typeof(ImportSetupHelper), new MethodCallParameters { { "FileName", fileData?.FileName } }))
                try
                {
                    if (dbContext is null) throw new System.ArgumentNullException(nameof(dbContext));
                    if (fileData is null) throw new System.ArgumentNullException(nameof(fileData));

                    var sb = new StringBuilder();
                    await AddLineAsync($"Importing setup from {fileData.FileName}");
                    using (var stream = fileData.OpenReadStream())
                    using (var doc = SpreadsheetDocument.Open(stream, false))
                    {
                        var userId = new Guid("662E267D-4219-4229-BE96-F589100708AC");
                        // Programmes
                        await AddLineAsync("Adding Programmes");
                        var programmesData = ExcelHelper.GetNameValues(doc, "ProgrammesData");
                        var programmesList = ExcelHelper.GetTableValues(doc, "Table_Programmes");
                        for (int rProgramme = 0; rProgramme < programmesList.GetUpperBound(0) + 1; rProgramme++)
                        {
                            var programmeCode = GetString(programmesList, rProgramme, 0);
                            if (string.IsNullOrWhiteSpace(programmeCode)) continue;
                            var programmeName = GetString(programmesList, rProgramme, 1);
                            var programme = await dbContext.Programmes.FirstOrDefaultAsync(i => i.Code == programmeCode);
                            if (programme != null)
                                if (!UpdateData)
                                {
                                    SAEONLogs.Verbose("Ignoring Programme {ProgrammeCode}, {ProgrammeName}", programmeCode, programmeName);
                                }
                                else
                                {
                                    await AddLineAsync($"Updating Programme {programmeCode}, {programmeName}");
                                    programme.Name = programmeName;
                                    programme.Description = GetString(programmesList, rProgramme, 2);
                                    programme.Url = GetString(programmesData, rProgramme, 3);
                                    programme.StartDate = GetDate(programmesData, rProgramme, 4);
                                    programme.EndDate = GetDate(programmesData, rProgramme, 5);
                                    dbContext.SaveChanges();
                                }
                            else
                            {
                                await AddLineAsync($"Adding Programme {programmeCode}, {programmeName}");
                                programme = new Programme
                                {
                                    Code = programmeCode,
                                    Name = programmeName,
                                    Description = GetString(programmesList, rProgramme, 2),
                                    Url = GetString(programmesData, rProgramme, 3),
                                    StartDate = GetDate(programmesData, rProgramme, 4),
                                    EndDate = GetDate(programmesData, rProgramme, 5),
                                    UserId = userId
                                };
                                dbContext.Programmes.Add(programme);
                                await dbContext.SaveChangesAsync();
                            }
                        }
                        // Projects
                        await AddLineAsync("Adding Projects");
                        var projectProgrammes = ExcelHelper.GetNameValues(doc, "ProjectsProgrammes");
                        var projectsData = ExcelHelper.GetNameValues(doc, "ProjectsData");
                        var projectsList = ExcelHelper.GetTableValues(doc, "Table_Projects");
                        for (int rProject = 0; rProject < projectsList.GetUpperBound(0) + 1; rProject++)
                        {
                            var projectCode = GetString(projectsList, rProject, 0);
                            if (string.IsNullOrWhiteSpace(projectCode)) continue;
                            var projectName = GetString(projectsList, rProject, 1);
                            var project = await dbContext.Projects.FirstOrDefaultAsync(i => i.Code == projectCode);
                            if (project != null)
                                if (!UpdateData)
                                {
                                    SAEONLogs.Verbose("Ignoring Project {ProjectCode}, {ProjectName}", projectCode, projectName);
                                }
                                else
                                {
                                    await AddLineAsync($"Updating Project {projectCode}, {projectName}");
                                    project.Name = projectName;
                                    project.Description = GetString(projectsList, rProject, 2);
                                    project.Url = GetString(projectsData, rProject, 3);
                                    project.StartDate = GetDate(projectsData, rProject, 4);
                                    project.EndDate = GetDate(projectsData, rProject, 5);
                                    await dbContext.SaveChangesAsync();
                                }
                            else
                            {
                                await AddLineAsync($"Adding Project {projectCode}, {projectName}");
                                var programmeCode = GetString(projectProgrammes, rProject, 0);
                                project = new Project
                                {
                                    ProgrammeId = (await dbContext.Programmes.FirstAsync(i => i.Code == programmeCode)).Id,
                                    Code = projectCode,
                                    Name = projectName,
                                    Description = GetString(projectsList, rProject, 2),
                                    Url = GetString(projectsData, rProject, 3),
                                    StartDate = GetDate(projectsData, rProject, 4),
                                    EndDate = GetDate(projectsData, rProject, 5),
                                    UserId = userId
                                };
                                dbContext.Projects.Add(project);
                                await dbContext.SaveChangesAsync();
                            }
                        }
                        // Sites
                        await AddLineAsync("Adding Sites");
                        var sitesData = ExcelHelper.GetNameValues(doc, "SitesData");
                        var sitesList = ExcelHelper.GetTableValues(doc, "Table_Sites");
                        for (int rSite = 0; rSite < sitesList.GetUpperBound(0) + 1; rSite++)
                        {
                            var siteCode = GetString(sitesList, rSite, 0);
                            if (string.IsNullOrWhiteSpace(siteCode)) continue;
                            var siteName = GetString(sitesList, rSite, 1);
                            var site = await dbContext.Sites.FirstOrDefaultAsync(i => i.Code == siteCode);
                            if (site != null)
                                if (!UpdateData)
                                {
                                    SAEONLogs.Verbose("Ignoring Site {SiteCode} {SiteName}", siteCode, siteName);
                                }
                                else
                                {
                                    await AddLineAsync($"Updating Site {siteCode}, {siteName}");
                                    site.Name = siteName;
                                    site.Description = GetString(sitesList, rSite, 2);
                                    site.Url = GetString(sitesData, rSite, 3);
                                    site.StartDate = GetDate(sitesData, rSite, 4);
                                    site.EndDate = GetDate(sitesData, rSite, 5);
                                    await dbContext.SaveChangesAsync();
                                }
                            else
                            {
                                await AddLineAsync($"Adding Site {siteCode}, {siteName}");
                                site = new Site
                                {
                                    Code = siteCode,
                                    Name = siteName,
                                    Description = GetString(sitesList, rSite, 2),
                                    Url = GetString(sitesData, rSite, 3),
                                    StartDate = GetDate(sitesData, rSite, 4),
                                    EndDate = GetDate(sitesData, rSite, 5),
                                    UserId = userId
                                };
                                dbContext.Sites.Add(site);
                                await dbContext.SaveChangesAsync();
                                var saeonOrganisationId = (await dbContext.Organisations.FirstAsync(i => i.Code == "SAEON")).Id;
                                var ownerRoleId = (await dbContext.OrganisationRoles.FirstAsync(i => i.Code == "Owner")).Id;
                                var siteId = (await dbContext.Sites.FirstAsync(i => i.Code == siteCode)).Id;
                                var sql =
                                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                                    $@"
                                    Insert Organisation_Site 
                                      (OrganisationID, SiteID, OrganisationRoleID, UserID) 
                                    Values 
                                      ({saeonOrganisationId},{siteId},{ownerRoleId},{userId})");
                            }
                        }
                        // Stations
                        await AddLineAsync("Adding Stations");
                        var stationProjects = ExcelHelper.GetNameValues(doc, "StationsProjects");
                        var stationSites = ExcelHelper.GetNameValues(doc, "StationsSites");
                        var stationsData = ExcelHelper.GetNameValues(doc, "StationsData");
                        var stationsList = ExcelHelper.GetTableValues(doc, "Table_Stations");
                        for (int rStation = 0; rStation < stationsList.GetUpperBound(0) + 1; rStation++)
                        {
                            var stationCode = GetString(stationsList, rStation, 0);
                            if (string.IsNullOrWhiteSpace(stationCode)) continue;
                            var stationName = GetString(stationsList, rStation, 1);
                            var station = await dbContext.Stations.FirstOrDefaultAsync(i => i.Code == stationCode);
                            if (station != null)
                                if (!UpdateData)
                                {
                                    SAEONLogs.Verbose("Ignoring Station {StationCode}, {StationName}", stationCode, stationName);
                                }
                                else
                                {
                                    await AddLineAsync($"Updating Station {stationCode}, {stationName}");
                                    station.Name = stationName;
                                    station.Description = GetString(stationsList, rStation, 2);
                                    station.Url = GetString(stationsData, rStation, 3);
                                    station.Latitude = GetLatitude(stationsData, rStation, 4);
                                    station.Longitude = GetLongitude(stationsData, rStation, 5);
                                    station.Elevation = GetDouble(stationsData, rStation, 6);
                                    station.StartDate = GetDate(stationsData, rStation, 7);
                                    station.EndDate = GetDate(stationsData, rStation, 8);
                                    await dbContext.SaveChangesAsync();
                                }
                            else
                            {
                                await AddLineAsync($"Adding Station {stationCode}, {stationName}");
                                var siteCode = GetString(stationSites, rStation, 0);
                                station = new Station
                                {
                                    SiteId = (await dbContext.Sites.FirstAsync(i => i.Code == siteCode)).Id,
                                    Code = stationCode,
                                    Name = stationName,
                                    Description = GetString(stationsList, rStation, 2),
                                    Url = GetString(stationsData, rStation, 3),
                                    Latitude = GetLatitude(stationsData, rStation, 4),
                                    Longitude = GetLongitude(stationsData, rStation, 5),
                                    Elevation = GetDouble(stationsData, rStation, 6),
                                    StartDate = GetDate(stationsData, rStation, 7),
                                    EndDate = GetDate(stationsData, rStation, 8),
                                    UserId = userId
                                };
                                dbContext.Stations.Add(station);
                                await dbContext.SaveChangesAsync();
                                var stationId = (await dbContext.Stations.FirstAsync(i => i.Code == stationCode)).Id;
                                var projectCode = GetString(stationProjects, rStation, 0);
                                var projectId = (await dbContext.Projects.FirstAsync(i => i.Code == projectCode)).Id;
                                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                                    $@"
                                    Insert Project_Station 
                                      (ProjectID, StationID, UserID)
                                    Values 
                                      ({projectId},{stationId},{userId})");
                            }
                        }
                        // Instruments
                        await AddLineAsync("Adding Instruments");
                        var instrumentStations = ExcelHelper.GetNameValues(doc, "InstrumentsStations");
                        var instrumentInstrumentTypes = ExcelHelper.GetNameValues(doc, "InstrumentsTypes");
                        var instrumentManufacturers = ExcelHelper.GetNameValues(doc, "InstrumentsManufacturers");
                        var instrumentsData = ExcelHelper.GetNameValues(doc, "InstrumentsData");
                        var instrumentsList = ExcelHelper.GetTableValues(doc, "Table_Instruments");
                        for (int rInstrument = 0; rInstrument < instrumentsList.GetUpperBound(0) + 1; rInstrument++)
                        {
                            var instrumentCode = GetString(instrumentsList, rInstrument, 0);
                            if (string.IsNullOrWhiteSpace(instrumentCode)) continue;
                            var instrumentName = GetString(instrumentsList, rInstrument, 1);
                            var instrument = await dbContext.Instruments.FirstOrDefaultAsync(i => i.Code == instrumentCode);
                            var stationCode = GetString(instrumentStations, rInstrument, 0);
                            if (instrument != null)
                                if (!UpdateData)
                                {
                                    SAEONLogs.Verbose("Ignoring Instrument {InstrumentCode}, {InstrumentName}", instrumentCode, instrumentName);
                                }
                                else
                                {
                                    await AddLineAsync($"Updating Instrument {instrumentCode}, {instrumentName}");
                                    instrument.Name = instrumentName;
                                    instrument.Description = GetString(instrumentsList, rInstrument, 2);
                                    instrument.Url = GetString(instrumentsData, rInstrument, 3);
                                    instrument.Latitude = GetLatitude(instrumentsData, rInstrument, 4);
                                    instrument.Longitude = GetLongitude(instrumentsData, rInstrument, 5);
                                    instrument.Elevation = GetDouble(instrumentsData, rInstrument, 6);
                                    instrument.StartDate = GetDate(instrumentsData, rInstrument, 7);
                                    instrument.EndDate = GetDate(instrumentsData, rInstrument, 8);
                                }
                            else
                            {
                                await AddLineAsync($"Adding Instrument {instrumentCode}, {instrumentName}");
                                instrument = new Instrument
                                {
                                    Code = instrumentCode,
                                    Name = instrumentName,
                                    Description = GetString(instrumentsList, rInstrument, 2),
                                    Url = GetString(instrumentsData, rInstrument, 3),
                                    Latitude = GetLatitude(instrumentsData, rInstrument, 4),
                                    Longitude = GetLongitude(instrumentsData, rInstrument, 5),
                                    Elevation = GetDouble(instrumentsData, rInstrument, 6),
                                    StartDate = GetDate(instrumentsData, rInstrument, 7),
                                    EndDate = GetDate(instrumentsData, rInstrument, 8),
                                    UserId = userId
                                };
                                dbContext.Instruments.Add(instrument);
                                await dbContext.SaveChangesAsync();
                                if (!string.IsNullOrWhiteSpace(stationCode))
                                {
                                    var instrumentId = (await dbContext.Instruments.FirstAsync(i => i.Code == instrumentCode)).Id;
                                    var stationId = (await dbContext.Stations.FirstAsync(i => i.Code == stationCode)).Id;
                                    await dbContext.Database.ExecuteSqlInterpolatedAsync(
                                        $@"
                                        Insert Station_Instrument
                                          (StationID, InstrumentID, UserID) 
                                        Values 
                                          ({stationId},{instrumentId},{userId})");
                                }
                            }
                        }
                        // DataSchemas
                        await AddLineAsync("Adding DataSchemas");
                        var CSVTypeId = (await dbContext.DataSourceTypes.FirstAsync(i => i.Code == "CSV")).Id;
                        var dataSchemasList = ExcelHelper.GetNameValues(doc, "DataSchemasList");
                        for (int rInstrument = 0; rInstrument < instrumentsList.GetUpperBound(0) + 1; rInstrument++)
                        {
                            var dataSchemaCode = GetString(dataSchemasList, rInstrument, 0);
                            if (string.IsNullOrWhiteSpace(dataSchemaCode)) continue;
                            var dataSchemaName = GetString(dataSchemasList, rInstrument, 1);
                            var dataSchema = await dbContext.DataSchemas.FirstOrDefaultAsync(i => i.Code == dataSchemaCode);
                            if (dataSchema != null)
                                if (!UpdateData)
                                {
                                    SAEONLogs.Verbose("Ignoring DataSchema {DataSchemaCode}, {DataSchemaName}", dataSchemaCode, dataSchemaName);
                                }
                                else
                                {
                                    await AddLineAsync($"Updating DataSchema {dataSchemaCode}, {dataSchemaName}");
                                    dataSchema.Name = dataSchemaName;
                                    dataSchema.Description = GetString(dataSchemasList, rInstrument, 2);
                                    dataSchema.DataSourceTypeId = CSVTypeId;
                                    await dbContext.SaveChangesAsync();
                                }
                            else
                            {
                                await AddLineAsync($"Adding DataSchema {dataSchemaCode}, {dataSchemaName}");
                                dataSchema = new DataSchema
                                {
                                    Code = dataSchemaCode,
                                    Name = dataSchemaName,
                                    Description = GetString(dataSchemasList, rInstrument, 2),
                                    DataSourceTypeId = CSVTypeId,
                                    UserId = userId
                                };
                                dbContext.DataSchemas.Add(dataSchema);
                                await dbContext.SaveChangesAsync();
                            }
                        }
                        // DataSources
                        SAEONLogs.Information("Adding DataSources");
                        var dataSourcesList = ExcelHelper.GetNameValues(doc, "DataSourcesList");
                        for (int rInstrument = 0; rInstrument < instrumentsList.GetUpperBound(0) + 1; rInstrument++)
                        {
                            var dataSourceCode = GetString(dataSourcesList, rInstrument, 0);
                            if (string.IsNullOrWhiteSpace(dataSourceCode)) continue;
                            var dataSourceName = GetString(dataSourcesList, rInstrument, 1);
                            var dataSource = await dbContext.DataSources.FirstOrDefaultAsync(i => i.Code == dataSourceCode);
                            if (dataSource != null)
                                if (!UpdateData)
                                {
                                    SAEONLogs.Verbose("Ignoring DataSource {DataSourceCode}, {DataSourceName}", dataSourceCode, dataSourceName);
                                }
                                else
                                {
                                    await AddLineAsync($"Updating DataSource {dataSourceCode}, {dataSourceName}");
                                    var dataSchemaCode = dataSourceName;
                                    dataSource.Name = GetString(dataSourcesList, rInstrument, 1);
                                    dataSource.Description = GetString(dataSourcesList, rInstrument, 2);
                                    dataSource.DataSchemaId = (await dbContext.DataSchemas.FirstAsync(i => i.Code == dataSchemaCode)).Id;
                                    dataSource.Url = "http://observations.saeon.ac.za";
                                    dataSource.UpdateFreq = 0;
                                    dataSource.LastUpdate = new DateTime(1900, 1, 1);
                                    await dbContext.SaveChangesAsync();
                                }
                            else
                            {
                                await AddLineAsync($"Adding DataSource {dataSourceCode}, {dataSourceName}");
                                var dataSchemaCode = GetString(dataSchemasList, rInstrument, 0);
                                dataSource = new DataSource
                                {
                                    Code = dataSourceCode,
                                    Name = dataSourceName,
                                    Description = GetString(dataSourcesList, rInstrument, 2),
                                    DataSchemaId = (await dbContext.DataSchemas.FirstAsync(i => i.Code == dataSchemaCode)).Id,
                                    Url = "http://observations.saeon.ac.za",
                                    UpdateFreq = 0,
                                    LastUpdate = new DateTime(1900, 1, 1),
                                    UserId = userId
                                };
                                dbContext.DataSources.Add(dataSource);
                                await dbContext.SaveChangesAsync();
                            }
                        }
                        // Phenomena
                        await AddLineAsync("Adding Phenomena");
                        var phenomenaList = ExcelHelper.GetTableValues(doc, "Table_Phenomena");
                        for (int rPhenomenon = 0; rPhenomenon < phenomenaList.GetUpperBound(0) + 1; rPhenomenon++)
                        {
                            var phenomenonCode = GetString(phenomenaList, rPhenomenon, 0);
                            if (string.IsNullOrWhiteSpace(phenomenonCode)) continue;
                            var phenomenonName = GetString(phenomenaList, rPhenomenon, 1);
                            var phenomenon = await dbContext.Phenomena.FirstOrDefaultAsync(i => i.Code == phenomenonCode);
                            if (phenomenon != null)
                                if (!UpdateData)
                                {
                                    SAEONLogs.Verbose("Ignoring Phenomenon {PhenomenonCode}, {PhenomenonName}", phenomenonCode, phenomenonName);
                                }
                                else
                                {
                                    await AddLineAsync($"Updating Phenomenon {phenomenonCode}, {phenomenonName}");
                                    phenomenon.Name = phenomenonName;
                                    phenomenon.Description = GetString(phenomenaList, rPhenomenon, 2);
                                    phenomenon.Url = GetString(phenomenaList, rPhenomenon, 3);
                                    await dbContext.SaveChangesAsync();
                                }
                            else
                            {
                                await AddLineAsync($"Adding Phenomenon {phenomenonCode}, {phenomenonName}");
                                phenomenon = new Phenomenon
                                {
                                    Code = phenomenonCode,
                                    Name = phenomenonName,
                                    Description = GetString(phenomenaList, rPhenomenon, 2),
                                    Url = GetString(phenomenaList, rPhenomenon, 3),
                                    UserId = userId
                                };
                                dbContext.Phenomena.Add(phenomenon);
                                await dbContext.SaveChangesAsync();
                            }
                        }
                        // Sensors
                        var sensorInstruments = ExcelHelper.GetNameValues(doc, "SensorsInstruments");
                        var sensorPhenomena = ExcelHelper.GetNameValues(doc, "SensorsPhenomena");
                        var sensorsData = ExcelHelper.GetNameValues(doc, "SensorsData");
                        var sensorsList = ExcelHelper.GetTableValues(doc, "Table_Sensors");
                        for (int rSensor = 0; rSensor < sensorsList.GetUpperBound(0) + 1; rSensor++)
                        {
                            var sensorCode = GetString(sensorsList, rSensor, 0);
                            if (string.IsNullOrWhiteSpace(sensorCode)) continue;
                            var sensorName = GetString(sensorsList, rSensor, 1);
                            var sensor = await dbContext.Sensors.FirstOrDefaultAsync(i => i.Code == sensorCode);
                            var instrumentCode = GetString(sensorInstruments, rSensor, 0);
                            var rInstrument = FindRowIndex(instrumentsList, 0, instrumentCode);
                            var dataSourceCode = GetString(dataSourcesList, rInstrument, 0);
                            var phenomenaCode = GetString(sensorPhenomena, rSensor, 0);
                            if (sensor != null)
                                if (!UpdateData)
                                {
                                    SAEONLogs.Verbose("Ignoring Sensor {SensorCode}, {SensorName}", sensorCode, sensorName);
                                }
                                else
                                {
                                    await AddLineAsync($"Updating Sensor {sensorCode}, {sensorName}");
                                    sensor.Name = GetString(sensorsList, rSensor, 1);
                                    sensor.Description = GetString(sensorsList, rSensor, 2);
                                    sensor.Url = GetString(sensorsData, rSensor, 2);
                                    sensor.Latitude = GetLatitude(sensorsData, rSensor, 3);
                                    sensor.Longitude = GetLongitude(sensorsData, rSensor, 4);
                                    sensor.Elevation = GetDouble(sensorsData, rSensor, 5);
                                    sensor.DataSourceId = (await dbContext.DataSources.FirstAsync(i => i.Code == dataSourceCode)).Id;
                                    sensor.PhenomenonId = (await dbContext.Phenomena.FirstAsync(i => i.Code == phenomenaCode)).Id;
                                    await dbContext.SaveChangesAsync();
                                }
                            else
                            {
                                await AddLineAsync($"Adding Sensor {sensorCode}, {sensorName}");
                                sensor = new Sensor
                                {
                                    Code = sensorCode,
                                    Name = GetString(sensorsList, rSensor, 1),
                                    Description = GetString(sensorsList, rSensor, 2),
                                    Url = GetString(sensorsData, rSensor, 2),
                                    Latitude = GetLatitude(sensorsData, rSensor, 3),
                                    Longitude = GetLongitude(sensorsData, rSensor, 4),
                                    Elevation = GetDouble(sensorsData, rSensor, 5),
                                    DataSourceId = (await dbContext.DataSources.FirstAsync(i => i.Code == dataSourceCode)).Id,
                                    PhenomenonId = (await dbContext.Phenomena.FirstAsync(i => i.Code == phenomenaCode)).Id,
                                    UserId = userId
                                };
                                dbContext.Sensors.Add(sensor);
                                await dbContext.SaveChangesAsync();
                                var instrumentId = (await dbContext.Instruments.FirstAsync(i => i.Code == instrumentCode)).Id;
                                var sensorId = (await dbContext.Sensors.FirstAsync(i => i.Code == sensorCode)).Id;
                                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                                    $@"
                                    Insert Instrument_Sensor 
                                      (InstrumentID, SensorID, UserID) 
                                    Values 
                                      ({instrumentId},{sensorId},{userId})");
                            }
                        }
                    }
                    await AddLineAsync("Done");
                    return sb.ToString();

                    async Task AddLineAsync(string line)
                    {
                        sb.AppendLine(line);
                        SAEONLogs.Information(line);
                        await adminHub.Clients.All.SendAsync(SignalRDefaults.ImportSetupStatusUpdate, line);
                    }

                    DateTime? GetDate(object[,] array, int row, int col)
                    {
                        try
                        {
                            string result = array[row, col].ToString();
                            return string.IsNullOrWhiteSpace(result) ? default(DateTime?) : DateTime.FromOADate(double.Parse(result));
                        }
                        catch (Exception ex)
                        {
                            SAEONLogs.Exception(ex, "GetDate: Row: {Row} Col: {Col} Value: {Value}", row, col, array?[row, col]);
                            throw;
                        }
                    }

                    Double? GetDouble(object[,] array, int row, int col)
                    {
                        try
                        {
                            string result = array[row, col].ToString();
                            return string.IsNullOrWhiteSpace(result) ? default(double?) : double.Parse(result);
                        }
                        catch (Exception ex)
                        {
                            using (SAEONLogs.MethodCall(typeof(ImportSetupHelper)))
                            {
                                SAEONLogs.Exception(ex, "GetDouble: Row: {Row} Col: {Col} Value: {Value}", row, col, array?[row, col]);
                                throw;
                            }
                        }
                    }

                    Double? GetLatitude(object[,] array, int row, int col)
                    {
                        var latitude = GetDouble(array, row, col);
                        if (latitude.HasValue)
                        {
                            latitude = -Math.Abs(latitude.Value);
                        }
                        return latitude;
                    }

                    Double? GetLongitude(object[,] array, int row, int col)
                    {
                        var longitude = GetDouble(array, row, col);
                        if (longitude.HasValue)
                        {
                            longitude = Math.Abs(longitude.Value);
                        }
                        return longitude;
                    }

                    string GetString(object[,] array, int row, int col)
                    {
                        try
                        {
                            string result = array[row, col]?.ToString();
                            return string.IsNullOrWhiteSpace(result) ? null : result;
                        }
                        catch (Exception ex)
                        {
                            SAEONLogs.Exception(ex, "GetString: Row: {Row} Col: {Col} Value: {Value}", row, col, array?[row, col]);
                            throw;
                        }
                    }

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

                    //void Dump(object[,] array, bool showTypes = false)
                    //{
                    //    var sb = new StringBuilder();
                    //    sb.AppendLine($"Rows: {array.GetUpperBound(0) + 1} Cols: {array.GetUpperBound(1) + 1}");
                    //    for (var r = 0; r <= array.GetUpperBound(0); r++)
                    //    {
                    //        sb.Append($"R: {r}");
                    //        for (var c = 0; c <= array.GetUpperBound(1); c++)
                    //        {
                    //            sb.Append($" {c}={array[r, c]}");
                    //            if (showTypes) sb.Append($" {array[r, c].GetType().Name}");
                    //        }
                    //        sb.AppendLine();
                    //    }
                    //    SAEONLogs.Information(sb.ToString());
                    //}
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
        }
    }

    //public class ImportSetupService : IHostedService
    //{
    //    private readonly IServiceScopeFactory scopeFactory;

    //    public ImportSetupService(IServiceScopeFactory scopeFactory)
    //    {
    //        this.scopeFactory = scopeFactory;
    //    }

    //    public Task StartAsync(CancellationToken cancellationToken)
    //    {
    //        using (SAEONLogs.MethodCall(GetType()))
    //        {
    //            try
    //            {
    //                DoWork()

    //            }
    //            catch (Exception ex)
    //            {
    //                SAEONLogs.Exception(ex);
    //                throw;
    //            }

    //        }
    //    }

    //    public Task StopAsync(CancellationToken cancellationToken)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private void DoWork()
    //    {
    //        using (var scope = scopeFactory.CreateScope())
    //        {
    //            var dbContext = scope.ServiceProvider.GetRequiredService<ObservationsDbContext>();
    //            var adminGub = scope.ServiceProvider.GetRequiredService<IHubContext<AdminHub>>();
    //        }
    //    }
    //}
}
