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
        public static bool UpdateData = false;

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
                            //SAEONLogs.Verbose("Row: {Row} Code: {Code}", rProgramme, programmeCode);
                            if (string.IsNullOrWhiteSpace(programmeCode)) continue;
                            var programme = await dbContext.Programmes.FirstOrDefaultAsync(i => i.Code == programmeCode);
                            if (programme != null)
                                if (!UpdateData)
                                {
                                    SAEONLogs.Verbose("Ignoring Programme {ProgrammeCode}", programmeCode);
                                }
                                else
                                {
                                    await AddLineAsync($"Updating Programme {programmeCode}");
                                    programme.Name = (string)programmesList[rProgramme, 1];
                                    programme.Description = GetString(programmesList, rProgramme, 2);
                                    programme.Url = GetString(programmesData, rProgramme, 3);
                                    programme.StartDate = GetDate(programmesData, rProgramme, 4);
                                    programme.EndDate = GetDate(programmesData, rProgramme, 5);
                                    dbContext.SaveChanges();
                                }
                            else
                            {
                                await AddLineAsync($"Adding Programme {programmeCode}");
                                programme = new Programme
                                {
                                    Code = programmeCode,
                                    Name = (string)programmesList[rProgramme, 1],
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
                            var project = await dbContext.Projects.FirstOrDefaultAsync(i => i.Code == projectCode);
                            if (project != null)
                                if (!UpdateData)
                                {
                                    SAEONLogs.Verbose("Ignoring Project {ProjectCode}", projectCode);
                                }
                                else
                                {
                                    await AddLineAsync($"Updating Project {projectCode}");
                                    project.Name = GetString(projectsList, rProject, 1);
                                    project.Description = GetString(projectsList, rProject, 2);
                                    project.Url = GetString(projectsData, rProject, 3);
                                    project.StartDate = GetDate(projectsData, rProject, 4);
                                    project.EndDate = GetDate(projectsData, rProject, 5);
                                    await dbContext.SaveChangesAsync();
                                }
                            else
                            {
                                await AddLineAsync($"Adding Project {projectCode}");
                                var programmeCode = GetString(projectProgrammes, rProject, 0);
                                project = new Project
                                {
                                    ProgrammeId = (await dbContext.Programmes.FirstAsync(i => i.Code == programmeCode)).Id,
                                    Code = projectCode,
                                    Name = GetString(projectsList, rProject, 1),
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
                            var site = await dbContext.Sites.FirstOrDefaultAsync(i => i.Code == siteCode);
                            if (site != null)
                                if (!UpdateData)
                                {
                                    SAEONLogs.Verbose("Ignoring Site {SiteCode}", siteCode);
                                }
                                else
                                {
                                    await AddLineAsync($"Updating Site {siteCode}");
                                    site.Name = GetString(sitesList, rSite, 1);
                                    site.Description = GetString(sitesList, rSite, 2);
                                    site.Url = GetString(sitesData, rSite, 3);
                                    site.StartDate = GetDate(sitesData, rSite, 4);
                                    site.EndDate = GetDate(sitesData, rSite, 5);
                                    await dbContext.SaveChangesAsync();
                                }
                            else
                            {
                                await AddLineAsync($"Adding Site {siteCode}");
                                site = new Site
                                {
                                    Code = siteCode,
                                    Name = GetString(sitesList, rSite, 1),
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
                                    "Insert Organisation_Site " +
                                    "  (OrganisationID, SiteID, OrganisationRoleID, UserID) " +
                                    "Values " +
                                   $"  ('{saeonOrganisationId}','{siteId}','{ownerRoleId}','{userId}')";
                                //SAEONLogs.Verbose("Sql: {Sql}", sql);
                                await dbContext.Database.ExecuteSqlRawAsync(sql);
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
                            var station = await dbContext.Stations.FirstOrDefaultAsync(i => i.Code == stationCode);
                            if (station != null)
                                if (!UpdateData)
                                {
                                    SAEONLogs.Verbose("Ignoring Station {StationCode}", stationCode);
                                }
                                else
                                {
                                    await AddLineAsync($"Updating Station {stationCode}");
                                    station.Name = GetString(stationsList, rStation, 1);
                                    station.Description = GetString(stationsList, rStation, 2);
                                    station.Url = GetString(stationsData, rStation, 3);
                                    station.Latitude = GetDouble(stationsData, rStation, 4);
                                    station.Longitude = GetDouble(stationsData, rStation, 5);
                                    station.Elevation = GetDouble(stationsData, rStation, 6);
                                    station.StartDate = GetDate(stationsData, rStation, 7);
                                    station.EndDate = GetDate(stationsData, rStation, 8);
                                    await dbContext.SaveChangesAsync();
                                }
                            else
                            {
                                await AddLineAsync($"Adding Station {stationCode}");
                                var siteCode = GetString(stationSites, rStation, 0);
                                station = new Station
                                {
                                    SiteId = (await dbContext.Sites.FirstAsync(i => i.Code == siteCode)).Id,
                                    Code = stationCode,
                                    Name = GetString(stationsList, rStation, 1),
                                    Description = GetString(stationsList, rStation, 2),
                                    Url = GetString(stationsData, rStation, 3),
                                    Latitude = GetDouble(stationsData, rStation, 4),
                                    Longitude = GetDouble(stationsData, rStation, 5),
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
                                var sql =
                                    "Insert Project_Station " +
                                    "  (ProjectID, StationID, UserID) " +
                                    "Values " +
                                   $"  ('{projectId}','{stationId}','{userId}')";
                                //SAEONLogs.Verbose("Sql: {Sql}", sql);
                                await dbContext.Database.ExecuteSqlRawAsync(sql);
                            }
                        }
                        // Instruments
                        await AddLineAsync("Adding Instruments");
                        var instrumentInstrumentTypes = ExcelHelper.GetNameValues(doc, "InstrumentsTypes");
                        var instrumentManufacturers = ExcelHelper.GetNameValues(doc, "InstrumentsManufacturers");
                        var instrumentsData = ExcelHelper.GetNameValues(doc, "InstrumentsData");
                        var instrumentsList = ExcelHelper.GetTableValues(doc, "Table_Instruments");
                        for (int rInstrument = 0; rInstrument < instrumentsList.GetUpperBound(0) + 1; rInstrument++)
                        {
                            var instrumentCode = GetString(instrumentsList, rInstrument, 0);
                            if (string.IsNullOrWhiteSpace(instrumentCode)) continue;
                            var instrument = await dbContext.Instruments.FirstOrDefaultAsync(i => i.Code == instrumentCode);
                            if (instrument != null)
                                if (!UpdateData)
                                {
                                    SAEONLogs.Verbose("Ignoring Instrument {InstrumentCode}", instrumentCode);
                                }
                                else
                                {
                                    await AddLineAsync($"Updating Instrument {instrumentCode}");
                                    instrument.Name = GetString(instrumentsList, rInstrument, 1);
                                    instrument.Description = GetString(instrumentsList, rInstrument, 2);
                                    instrument.Url = GetString(instrumentsData, rInstrument, 3);
                                    instrument.Latitude = GetDouble(instrumentsData, rInstrument, 4);
                                    instrument.Longitude = GetDouble(instrumentsData, rInstrument, 5);
                                    instrument.Elevation = GetDouble(instrumentsData, rInstrument, 6);
                                    instrument.StartDate = GetDate(instrumentsData, rInstrument, 7);
                                    instrument.EndDate = GetDate(instrumentsData, rInstrument, 8);
                                }
                            else
                            {
                                await AddLineAsync($"Adding Instrument {instrumentCode}");
                                instrument = new Instrument
                                {
                                    Code = instrumentCode,
                                    Name = GetString(instrumentsList, rInstrument, 1),
                                    Description = GetString(instrumentsList, rInstrument, 2),
                                    Url = GetString(instrumentsData, rInstrument, 3),
                                    Latitude = GetDouble(instrumentsData, rInstrument, 4),
                                    Longitude = GetDouble(instrumentsData, rInstrument, 5),
                                    Elevation = GetDouble(instrumentsData, rInstrument, 6),
                                    StartDate = GetDate(instrumentsData, rInstrument, 7),
                                    EndDate = GetDate(instrumentsData, rInstrument, 8),
                                    UserId = userId
                                };
                                dbContext.Instruments.Add(instrument);
                                await dbContext.SaveChangesAsync();
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
                            var dataSchema = await dbContext.DataSchemas.FirstOrDefaultAsync(i => i.Code == dataSchemaCode);
                            if (dataSchema != null)
                                if (!UpdateData)
                                {
                                    SAEONLogs.Verbose("Ignoring DataSchema {DataSchemaCode}", dataSchemaCode);
                                }
                                else
                                {
                                    await AddLineAsync($"Updating DataSchema {dataSchemaCode}");
                                    dataSchema.Name = GetString(dataSchemasList, rInstrument, 1);
                                    dataSchema.Description = GetString(dataSchemasList, rInstrument, 2);
                                    dataSchema.DataSourceTypeId = CSVTypeId;
                                    await dbContext.SaveChangesAsync();
                                }
                            else
                            {
                                await AddLineAsync($"Adding DataSchema {dataSchemaCode}");
                                dataSchema = new DataSchema
                                {
                                    Code = dataSchemaCode,
                                    Name = GetString(dataSchemasList, rInstrument, 1),
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
                            var dataSource = await dbContext.DataSources.FirstOrDefaultAsync(i => i.Code == dataSourceCode);
                            if (dataSource != null)
                                if (!UpdateData)
                                {
                                    SAEONLogs.Verbose("Ignoring DataSource {DataSourceCode}", dataSourceCode);
                                }
                                else
                                {
                                    await AddLineAsync($"Updating DataSource {dataSourceCode}");
                                    var dataSchemaCode = GetString(dataSchemasList, rInstrument, 0);
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
                                await AddLineAsync($"Adding DataSource {dataSourceCode}");
                                var dataSchemaCode = GetString(dataSchemasList, rInstrument, 0);
                                dataSource = new DataSource
                                {
                                    Code = dataSourceCode,
                                    Name = GetString(dataSourcesList, rInstrument, 1),
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
                            var phenomenon = await dbContext.Phenomena.FirstOrDefaultAsync(i => i.Code == phenomenonCode);
                            if (phenomenon != null)
                                if (!UpdateData)
                                {
                                    SAEONLogs.Verbose("Ignoring Phenomenon {PhenomenonCode}", phenomenonCode);
                                }
                                else
                                {
                                    await AddLineAsync($"Updating Phenomenon {phenomenonCode}");
                                    phenomenon.Name = GetString(phenomenaList, rPhenomenon, 1);
                                    phenomenon.Description = GetString(phenomenaList, rPhenomenon, 2);
                                    phenomenon.Url = GetString(phenomenaList, rPhenomenon, 3);
                                    await dbContext.SaveChangesAsync();
                                }
                            else
                            {
                                await AddLineAsync($"Adding Phenomenon {phenomenonCode}");
                                phenomenon = new Phenomenon
                                {
                                    Code = phenomenonCode,
                                    Name = GetString(phenomenaList, rPhenomenon, 1),
                                    Description = GetString(phenomenaList, rPhenomenon, 2),
                                    Url = GetString(phenomenaList, rPhenomenon, 3),
                                    UserId = userId
                                };
                                dbContext.Phenomena.Add(phenomenon);
                                await dbContext.SaveChangesAsync();
                            }
                        }
                        // Sensors
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

                    void Dump(object[,] array, bool showTypes = false)
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine($"Rows: {array.GetUpperBound(0) + 1} Cols: {array.GetUpperBound(1) + 1}");
                        for (var r = 0; r <= array.GetUpperBound(0); r++)
                        {
                            sb.Append($"R: {r}");
                            for (var c = 0; c <= array.GetUpperBound(1); c++)
                            {
                                sb.Append($" {c}={array[r, c]}");
                                if (showTypes) sb.Append($" {array[r, c].GetType().Name}");
                            }
                            sb.AppendLine();
                        }
                        SAEONLogs.Information(sb.ToString());
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
        }
    }
}
