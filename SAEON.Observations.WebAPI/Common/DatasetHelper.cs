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
        public static readonly string DatasetsFolderConfigKey = "DatasetsFolder";
        private static readonly string UseDiskConfigKey = "DatasetsUseDisk";

        public static async Task<string> UpdateDatasets(ObservationsDbContext dbContext, IHubContext<AdminHub> adminHub, HttpContext httpContext)
        {
            using (SAEONLogs.MethodCall(typeof(DatasetHelper)))
            {
                try
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var sb = new StringBuilder();
                    await AddLineAsync("Updating datasets");
                    await GenerateDatasets();
                    await AddLineAsync($"Done in {stopwatch.Elapsed.TimeStr()}");
                    return sb.ToString();

                    async Task AddLineAsync(string line)
                    {
                        sb.AppendLine(line);
                        SAEONLogs.Information(line);
                        await adminHub.Clients.All.SendAsync(SignalRDefaults.UpdateDatasetsStatus, line);
                    }

                    async Task GenerateDatasets()
                    {
                        foreach (var inventoryDataset in await dbContext.VInventoryDatasets
                            .OrderBy(i => i.OrganisationName)
                            .ThenBy(i => i.ProgrammeName)
                            .ThenBy(i => i.ProgrammeName)
                            .ThenBy(i => i.SiteName)
                            .ThenBy(i => i.StationName)
                            .ToListAsync())
                        {
                            var dataset = await EnsureDataset(inventoryDataset);
                            dataset.Count = inventoryDataset.Count;
                            dataset.ValueCount = inventoryDataset.ValueCount;
                            dataset.NullCount = inventoryDataset.NullCount;
                            dataset.VerifiedCount = inventoryDataset.VerifiedCount;
                            dataset.UnverifiedCount = inventoryDataset.UnverifiedCount;
                            dataset.StartDate = inventoryDataset.StartDate;
                            dataset.EndDate = inventoryDataset.EndDate;
                            dataset.LatitudeNorth = inventoryDataset.LatitudeNorth;
                            dataset.LatitudeSouth = inventoryDataset.LatitudeSouth;
                            dataset.LongitudeWest = inventoryDataset.LongitudeWest;
                            dataset.LongitudeEast = inventoryDataset.LongitudeEast;
                            dataset.ElevationMinimum = inventoryDataset.ElevationMinimum;
                            dataset.ElevationMaximum = inventoryDataset.ElevationMaximum;
                            var oldHashCode = dataset.HashCode;
                            var newHashCode = dataset.CreateHashCode();
                            SAEONLogs.Verbose("OldHashCode: {OldHashCode} NewHashCode: {NewHashCode}", oldHashCode, newHashCode);
                            if (oldHashCode != newHashCode)
                            {
                                dataset.HashCode = newHashCode;
                                dataset.NeedsUpdate = true;
                            }
                        }
                        await dbContext.SaveChangesAsync();
                    }

                    async Task<Dataset> EnsureDataset(VInventoryDataset inventoryDataset)
                    {
                        var dataset = await dbContext.Datasets.FirstOrDefaultAsync(i =>
                            i.StationId == inventoryDataset.StationId &&
                            i.PhenomenonOfferingId == inventoryDataset.PhenomenonOfferingId &&
                            i.PhenomenonUnitId == inventoryDataset.PhenomenonUnitId);
                        if (dataset is null)
                        {
                            await AddLineAsync($"Adding dataset {inventoryDataset.Code}, {inventoryDataset.Name}");
                            dataset = new Dataset
                            {
                                Code = inventoryDataset.Code,
                                Name = inventoryDataset.Name,
                                StationId = inventoryDataset.StationId,
                                PhenomenonOfferingId = inventoryDataset.PhenomenonOfferingId,
                                PhenomenonUnitId = inventoryDataset.PhenomenonUnitId,
                                NeedsUpdate = true,
                                AddedBy = httpContext?.User?.UserId() ?? Guid.Empty.ToString(),
                                UpdatedBy = httpContext?.User?.UserId() ?? Guid.Empty.ToString(),
                                UserId = new Guid(httpContext?.User?.UserId() ?? Guid.Empty.ToString()),
                            };
                            dbContext.Datasets.Add(dataset);
                        }
                        return dataset;
                    }

                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public static async Task<string> CreateDatasetsFiles(ObservationsDbContext dbContext, IHubContext<AdminHub> adminHub, HttpContext httpContext, IConfiguration config)
        {
            using (SAEONLogs.MethodCall(typeof(DatasetHelper)))
            {
                try
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var sb = new StringBuilder();
                    await AddLineAsync("Creating dataset files");
                    await GenerateDatasets();
                    await AddLineAsync($"Done in {stopwatch.Elapsed.TimeStr()}");
                    return sb.ToString();

                    async Task AddLineAsync(string line)
                    {
                        sb.AppendLine(line);
                        SAEONLogs.Information(line);
                        await adminHub.Clients.All.SendAsync(SignalRDefaults.CreateDatasetFilesStatus, line);
                    }

                    async Task GenerateDatasets()
                    {
                        foreach (var vDataset in dbContext.VDatasetsExpansion.AsNoTracking()
                            .Where(i => !i.NeedsUpdate ?? false)
                            .OrderBy(i => i.OrganisationName)
                            .ThenBy(i => i.ProgrammeName)
                            .ThenBy(i => i.ProgrammeName)
                            .ThenBy(i => i.SiteName)
                            .ThenBy(i => i.StationName))
                        {
                            if ((string.IsNullOrEmpty(vDataset.CSVFileName) || !File.Exists(Path.Combine(config[DatasetsFolderConfigKey], vDataset.CSVFileName))) ||
                                ((vDataset.IsValid ?? false) && (string.IsNullOrEmpty(vDataset.ExcelFileName) || !File.Exists(Path.Combine(config[DatasetsFolderConfigKey], vDataset.ExcelFileName)))) /*||
                                ((vDataset.IsValid ?? false) && (string.IsNullOrEmpty(vDataset.NetCDFFileName) || !File.Exists(Path.Combine(config[DatasetsFolderConfigKey], vDataset.NetCDFFileName))))*/)
                            {
                                var dataset = dbContext.Datasets.First(i => i.Id == vDataset.Id);
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
                            await EnsureDatasetFiles(await dbContext.Datasets.FirstAsync(i => i.Id == datasetId));
                        }
                    }

                    async Task EnsureDatasetFiles(Dataset dataset)
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
                        var observations = await LoadFromDatabaseAsync(dbContext, dataset, false);
                        EnsureCSV();
                        var vDataset = dbContext.VDatasetsExpansion.First(i => i.Id == dataset.Id);
                        if (vDataset.IsValid ?? false)
                        {
                            EnsureExcel();
                            EnsureNetCDF();
                        }
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
                            using (var doc = ExcelSaxHelper.CreateSpreadsheet(Path.Combine(config[DatasetsFolderConfigKey], dataset.ExcelFileName), observations.Where(i => (i.Status == null) || (i.Status == "Verified")).ToList(), true))
                            {
                                doc.Save();
                            }
                        }

                        void EnsureNetCDF()
                        {
                            //SAEONLogs.Verbose("Creating {FileName}", dataset.NetCDFFileName);
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
            using (SAEONLogs.MethodCall(typeof(DatasetHelper), new MethodCallParameters { { "datasetId", dataset?.Id } }))
            {
                try
                {
                    Guard.IsNotNull(dataset, nameof(dataset));
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    using var reader = new StreamReader(Path.Combine(config[DatasetsFolderConfigKey], dataset.CSVFileName));
                    using var csv = CsvReaderHelper.GetCsvReader(reader);
                    var result = csv.GetRecords<ObservationDTO>().Where(i => (((i.Status == null) || (i.Status == "Verified")))).ToList();
                    SAEONLogs.Verbose("Loaded from disk in {Elapsed}", stopwatch.Elapsed.TimeStr());
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
            using (SAEONLogs.MethodCall(typeof(DatasetHelper), new MethodCallParameters { { "datasetId", dataset?.Id } }))
            {
                try
                {
                    Guard.IsNotNull(dataset, nameof(dataset));
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    using var reader = new StreamReader(Path.Combine(config[DatasetsFolderConfigKey], dataset.CSVFileName));
                    using var csv = CsvReaderHelper.GetCsvReader(reader);
                    var result = new List<ObservationDTO>();
                    await foreach (var record in csv.GetRecordsAsync<ObservationDTO>())

                    {
                        if ((record.Status == null) || (record.Status == "Verified"))
                            result.Add(record);
                    }
                    SAEONLogs.Verbose("Loaded from disk in {Elapsed}", stopwatch.Elapsed.TimeStr());
                    return result;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        private static IQueryable<ObservationDTO> GetQuery(ObservationsDbContext dbContext, Dataset dataset, bool onlyVerified)
        {
            using (SAEONLogs.MethodCall(typeof(DatasetHelper), new MethodCallParameters { { "datasetId", dataset?.Id } }))
            {
                try
                {
                    Guard.IsNotNull(dataset, nameof(dataset));
                    var result = dbContext.VObservationsExpansion.AsNoTracking()
                        .Where(i => (i.StationId == dataset.StationId) && (i.PhenomenonOfferingId == dataset.PhenomenonOfferingId) && (i.PhenomenonUnitId == dataset.PhenomenonUnitId));
                    if (onlyVerified)
                    {
                        result = result.Where(i => (i.StatusId == null) || (i.StatusName == "Verified"));
                    }
                    result = result.OrderBy(i => i.Elevation).ThenBy(i => i.ValueDate);
                    return result.Select(i => new ObservationDTO
                    {
                        Id = i.Id,
                        Site = i.SiteName,
                        Station = i.StationName,
                        Phenomenon = i.PhenomenonName,
                        Offering = i.OfferingName,
                        Unit = i.UnitName,
                        UnitSymbol = i.UnitSymbol,
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

        private static List<ObservationDTO> LoadFromDatabase(ObservationsDbContext dbContext, Dataset dataset, bool onlyVerified)
        {
            using (SAEONLogs.MethodCall(typeof(DatasetHelper), new MethodCallParameters { { "datasetId", dataset?.Id } }))
            {
                try
                {
                    Guard.IsNotNull(dataset, nameof(dataset));
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var result = GetQuery(dbContext, dataset, onlyVerified).ToList();
                    SAEONLogs.Verbose("Loaded from database in {Elapsed}", stopwatch.Elapsed.TimeStr());
                    return result;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        private static async Task<List<ObservationDTO>> LoadFromDatabaseAsync(ObservationsDbContext dbContext, Dataset dataset, bool onlyVerified)
        {
            using (SAEONLogs.MethodCall(typeof(DatasetHelper), new MethodCallParameters { { "datasetId", dataset?.Id } }))
            {
                try
                {
                    Guard.IsNotNull(dataset, nameof(dataset));
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var result = await GetQuery(dbContext, dataset, onlyVerified).ToListAsync();
                    SAEONLogs.Verbose("Loaded from database in {Elapsed}", stopwatch.Elapsed.TimeStr());
                    return result;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        private static bool IsOnDisk(IConfiguration config, Dataset dataset)
        {
            using (SAEONLogs.MethodCall(typeof(DatasetHelper), new MethodCallParameters { { "datasetId", dataset?.Id } }))
            {
                try
                {
                    Guard.IsNotNull(dataset, nameof(dataset));
                    string fileName = dataset.CSVFileName;
                    var useDisk = config[UseDiskConfigKey]?.IsTrue() ?? false;
                    var fileExists = (!string.IsNullOrEmpty(fileName) && File.Exists(Path.Combine(config[DatasetsFolderConfigKey], dataset.CSVFileName)));
                    var result = useDisk && fileExists;
                    SAEONLogs.Verbose("UseDisk: {UseDisk} FileName: {FileName} FileExists: {FileExists} IsOnDisk: {IsOnDisk}", useDisk, fileName, fileExists, result);
                    return result;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public static async Task<List<ObservationDTO>> LoadAsync(ObservationsDbContext dbContext, IConfiguration config, Guid datasetId)
        {
            using (SAEONLogs.MethodCall(typeof(DatasetHelper), new MethodCallParameters { { "datasetId", datasetId } }))
            {
                try
                {
                    var dataset = await dbContext.Datasets.FirstOrDefaultAsync(i => i.Id == datasetId);
                    Guard.IsNotNull(dataset, nameof(dataset));
                    if (IsOnDisk(config, dataset))
                    {
                        return await LoadFromDiskAsync(dataset, config);
                    }
                    else
                    {
                        return await LoadFromDatabaseAsync(dbContext, dataset, true);
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public static List<ObservationDTO> Load(ObservationsDbContext dbContext, IConfiguration config, Guid datasetId)
        {
            using (SAEONLogs.MethodCall(typeof(DatasetHelper), new MethodCallParameters { { "datasetId", datasetId } }))
            {
                try
                {
                    var dataset = dbContext.Datasets.FirstOrDefault(i => i.Id == datasetId);
                    Guard.IsNotNull(dataset, nameof(dataset));
                    if (IsOnDisk(config, dataset))
                    {
                        return LoadFromDisk(dataset, config);
                    }
                    else
                    {
                        return LoadFromDatabase(dbContext, dataset, true);
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
