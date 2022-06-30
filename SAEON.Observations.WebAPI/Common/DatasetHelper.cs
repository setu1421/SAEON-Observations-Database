using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Toolkit.Diagnostics;
using SAEON.AspNet.Auth;
using SAEON.Core;
using SAEON.CSV;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.WebAPI.Hubs;
using SAEON.OpenXML;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI
{
    public static class DatasetHelper
    {
        private static readonly string BatchSizeConfigKey = "DatasetsBatchSize";
        private static readonly string DatasetsFolderConfigKey = "DatasetsFolder";
        private static readonly string UseDiskConfigKey = "DatasetsUseDisk";

        public static async Task<string> CreateDatasets(ObservationsDbContext dbContext, IHubContext<AdminHub> adminHub, HttpContext httpContext, IConfiguration config)
        {
            using (SAEONLogs.MethodCall(typeof(DOIHelper)))
            {
                try
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var sb = new StringBuilder();
                    await AddLineAsync("Generating datasets");
                    await GenerateDatasets();
                    await AddLineAsync($"Done in {stopwatch.Elapsed.TimeStr()}");
                    return sb.ToString();

                    async Task AddLineAsync(string line)
                    {
                        sb.AppendLine(line);
                        SAEONLogs.Information(line);
                        await adminHub.Clients.All.SendAsync(SignalRDefaults.CreateDatasetsStatusUpdate, line);
                    }

                    async Task GenerateDatasets()
                    {
                        foreach (var dataset in dbContext.Datasets.Where(i => !i.NeedsUpdate ?? false))
                        {
                            if ((string.IsNullOrEmpty(dataset.CSVFileName) || !File.Exists(Path.Combine(config[DatasetsFolderConfigKey], dataset.CSVFileName))) ||
                                (string.IsNullOrEmpty(dataset.ExcelFileName) || !File.Exists(Path.Combine(config[DatasetsFolderConfigKey], dataset.ExcelFileName))) /*||
                                (string.IsNullOrEmpty(dataset.NetCDFFileName) || !File.Exists(Path.Combine(config[DatasetsFolderConfigKey], dataset.NetCDFFileName)))*/)
                            {
                                dataset.NeedsUpdate = true;
                            }
                        }
                        await dbContext.SaveChangesAsync();
                        var datasetIds = await dbContext.VDatasetsExpansion.AsNoTracking()
                            .Where(i => i.NeedsUpdate ?? false)
                            .OrderBy(i => i.OrganisationName)
                            .ThenBy(i => i.ProgrammeName)
                            .ThenBy(i => i.ProgrammeName)
                            .ThenBy(i => i.SiteName)
                            .ThenBy(i => i.StationName)
                            .Select(i => i.Id)
                            .ToListAsync();
                        if (int.TryParse(config[BatchSizeConfigKey], out var take))
                        {
                            if (take > 0)
                            {
                                datasetIds = datasetIds.Take(take).ToList();
                            }
                        }
                        foreach (var datasetId in datasetIds)
                        {
                            await EnsureDataset(await dbContext.Datasets.FirstAsync(i => i.Id == datasetId));
                        }
                    }

                    async Task EnsureDataset(Dataset dataset)
                    {
                        var stopwatch = new Stopwatch();
                        stopwatch.Start();
                        await AddLineAsync($"{dataset.Code} {dataset.Name}");
                        var folder = $"{DateTime.Now:yyyy-MM}";
                        Directory.CreateDirectory(Path.Combine(config[DatasetsFolderConfigKey], folder));
                        var fileName = Path.Combine(folder, dataset.FileName);
                        dataset.CSVFileName = $"{fileName}.csv";
                        dataset.ExcelFileName = $"{fileName}.xlsx";
                        dataset.NetCDFFileName = $"{fileName}.nc";
                        var observations = await LoadFromDatabaseAsync(dbContext, dataset);
                        EnsureCSV();
                        EnsureExcel();
                        EnsureNetCDF();
                        dataset.NeedsUpdate = false;
                        if (dbContext.Entry(dataset).State != EntityState.Unchanged)
                        {
                            dataset.UpdatedBy = httpContext?.User?.UserId() ?? Guid.Empty.ToString();
                            dataset.UserId = new Guid(httpContext?.User?.UserId() ?? Guid.Empty.ToString());
                        }
                        await dbContext.SaveChangesAsync();
                        await AddLineAsync($"{dataset.Code} done in {stopwatch.Elapsed.TimeStr()}");

                        void EnsureCSV()
                        {
                            SAEONLogs.Verbose("Creating {FileName}", dataset.CSVFileName);
                            using var writer = new StreamWriter(Path.Combine(config[DatasetsFolderConfigKey], dataset.CSVFileName));
                            using var csv = CsvWriterHelper.GetCsvWriter(writer);
                            csv.WriteRecords(observations);
                            writer.Flush();
                        }

                        void EnsureExcel()
                        {
                            SAEONLogs.Verbose("Creating {FileName}", dataset.ExcelFileName);
                            using (var doc = ExcelSaxHelper.CreateSpreadsheet(Path.Combine(config[DatasetsFolderConfigKey], dataset.ExcelFileName), observations, true))
                            {
                                doc.Save();
                            }
                        }

                        void EnsureNetCDF()
                        {
                            SAEONLogs.Verbose("Creating {FileName}", dataset.NetCDFFileName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        private static List<ObservationDTO> LoadFromDisk(Dataset dataset, IConfiguration config)
        {
            using (SAEONLogs.MethodCall(typeof(DOIHelper), new MethodCallParameters { { "datasetId", dataset?.Id } }))
            {
                try
                {
                    Guard.IsNotNull(dataset, nameof(dataset));
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var fileName = dataset.CSVFileName; // @@@ Remove once new style are populated
                    if (string.IsNullOrEmpty(fileName))
                    {
                        fileName = $"{dataset.OldFileName}.csv";
                    }
                    using var reader = new StreamReader(Path.Combine(config[DatasetsFolderConfigKey], fileName));
                    //using var reader = new StreamReader(Path.Combine(config[DatasetsFolderConfigKey], dataset.CSVFileName));
                    using var csv = CsvReaderHelper.GetCsvReader(reader);
                    var result = csv.GetRecords<ObservationDTO>().ToList();
                    SAEONLogs.Verbose("Loaded in {Elapsed}", stopwatch.Elapsed.TimeStr());
                    return result;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        private static async Task<List<ObservationDTO>> LoadFromDiskAsync(Dataset dataset, IConfiguration config)
        {
            using (SAEONLogs.MethodCall(typeof(DOIHelper), new MethodCallParameters { { "datasetId", dataset?.Id } }))
            {
                try
                {
                    Guard.IsNotNull(dataset, nameof(dataset));
                    var fileName = dataset.CSVFileName; // @@@ Remove once new style are populated
                    if (string.IsNullOrEmpty(fileName))
                    {
                        fileName = $"{dataset.OldFileName}.csv";
                    }
                    using var reader = new StreamReader(Path.Combine(config[DatasetsFolderConfigKey], fileName));
                    //using var reader = new StreamReader(Path.Combine(config[DatasetsFolderConfigKey], dataset.CSVFileName));
                    using var csv = CsvReaderHelper.GetCsvReader(reader);
                    var result = new List<ObservationDTO>();
                    await foreach (var record in csv.GetRecordsAsync<ObservationDTO>())
                    {
                        result.Add(record);
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        private static IQueryable<ObservationDTO> GetQuery(ObservationsDbContext dbContext, Dataset dataset)
        {
            using (SAEONLogs.MethodCall(typeof(DOIHelper), new MethodCallParameters { { "datasetId", dataset?.Id } }))
            {
                try
                {
                    Guard.IsNotNull(dataset, nameof(dataset));
                    return dbContext.VObservationsExpansion.AsNoTracking()
                        .Where(i =>
                            (i.StationId == dataset.StationId) && (i.PhenomenonOfferingId == dataset.PhenomenonOfferingId) && (i.PhenomenonUnitId == dataset.PhenomenonUnitId) &&
                            ((i.StatusId == null) || (i.StatusName == "Verified")))
                        .OrderBy(i => i.Elevation).ThenBy(i => i.ValueDate)
                        .Select(i => new ObservationDTO
                        {
                            Site = i.SiteName,
                            Station = i.StationName,
                            Phenomenon = i.PhenomenonName,
                            Offering = i.OfferingName,
                            Unit = i.UnitName,
                            Date = i.ValueDate,
                            Value = i.DataValue,
                            Instrument = i.InstrumentName,
                            Sensor = i.SensorName,
                            Comment = i.Comment,
                            Latitude = i.Latitude,
                            Longitude = i.Longitude,
                            Elevation = i.Elevation,
                            Status = i.StatusName,
                            Reason = i.StatusReasonName,
                        });
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        private static List<ObservationDTO> LoadFromDatabase(ObservationsDbContext dbContext, Dataset dataset)
        {
            using (SAEONLogs.MethodCall(typeof(DOIHelper), new MethodCallParameters { { "datasetId", dataset?.Id } }))
            {
                try
                {
                    Guard.IsNotNull(dataset, nameof(dataset));
                    return GetQuery(dbContext, dataset).ToList();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        private static async Task<List<ObservationDTO>> LoadFromDatabaseAsync(ObservationsDbContext dbContext, Dataset dataset)
        {
            using (SAEONLogs.MethodCall(typeof(DOIHelper), new MethodCallParameters { { "datasetId", dataset?.Id } }))
            {
                try
                {
                    Guard.IsNotNull(dataset, nameof(dataset));
                    return await GetQuery(dbContext, dataset).ToListAsync();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        private static bool IsOnDisk(IConfiguration config, Dataset dataset, DatasetFileTypes fileType)
        {
            using (SAEONLogs.MethodCall(typeof(DOIHelper), new MethodCallParameters { { "datasetId", dataset?.Id }, { "fileType", fileType } }))
            {
                try
                {
                    Guard.IsNotNull(dataset, nameof(dataset));
                    string fileName = null;
                    string oldFileName = null;
                    switch (fileType)
                    {
                        case DatasetFileTypes.CSV:
                            fileName = dataset.CSVFileName;
                            oldFileName = $"{dataset.OldFileName}.csv";
                            break;
                        case DatasetFileTypes.Excel:
                            fileName = dataset.ExcelFileName;
                            break;
                        case DatasetFileTypes.NetCDF:
                            fileName = dataset.NetCDFFileName;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(fileType));
                    }
                    var useDisk = config[UseDiskConfigKey]?.IsTrue() ?? false;
                    var fileExists = (!string.IsNullOrEmpty(fileName) && File.Exists(Path.Combine(config[DatasetsFolderConfigKey], fileName))) ||
                                     (!string.IsNullOrEmpty(oldFileName) && File.Exists(Path.Combine(config[DatasetsFolderConfigKey], oldFileName)));
                    var result = useDisk && fileExists;
                    SAEONLogs.Verbose("UseDisk: {UseDisk} FileName: {FileName} OldFileName: {OldFileName} FileExists: {FileExists} IsOnDisk: {IsOnDisk}", useDisk, fileName, oldFileName, fileExists, result);
                    return result;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }
        public static async Task<List<ObservationDTO>> LoadAsync(ObservationsDbContext dbContext, IConfiguration config, Guid datasetId, DatasetFileTypes fileType)
        {
            using (SAEONLogs.MethodCall(typeof(DOIHelper), new MethodCallParameters { { "datasetId", datasetId }, { "fileType", fileType } }))
            {
                try
                {
                    var dataset = await dbContext.Datasets.FirstOrDefaultAsync(i => i.Id == datasetId);
                    Guard.IsNotNull(dataset, nameof(dataset));
                    if (IsOnDisk(config, dataset, fileType))
                    {
                        return await LoadFromDiskAsync(dataset, config);
                    }
                    else
                    {
                        return await LoadFromDatabaseAsync(dbContext, dataset);
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public static List<ObservationDTO> Load(ObservationsDbContext dbContext, IConfiguration config, Guid datasetId, DatasetFileTypes fileType)
        {
            using (SAEONLogs.MethodCall(typeof(DOIHelper), new MethodCallParameters { { "datasetId", datasetId }, { "fileType", fileType } }))
            {
                try
                {
                    var dataset = dbContext.Datasets.FirstOrDefault(i => i.Id == datasetId);
                    Guard.IsNotNull(dataset, nameof(dataset));
                    if (IsOnDisk(config, dataset, fileType))
                    {
                        return LoadFromDisk(dataset, config);
                    }
                    else
                    {
                        return LoadFromDatabase(dbContext, dataset);
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
}
