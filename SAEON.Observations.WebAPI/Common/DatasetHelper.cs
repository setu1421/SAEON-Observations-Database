using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Toolkit.Diagnostics;
using SAEON.AspNet.Auth;
using SAEON.Core;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.WebAPI.Hubs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
                            if (!File.Exists(Path.Combine(config[DatasetsFolderConfigKey], dataset.FileName)))
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
                        var datasetsFolder = config[DatasetsFolderConfigKey];
                        var observations = await LoadFromDatabaseAsync(dbContext, dataset);
                        using var writer = new StreamWriter(Path.Combine(datasetsFolder, dataset.FileName));
                        using var csv = GetCsvWriter(writer);
                        csv.WriteRecords(observations);
                        dataset.NeedsUpdate = false;
                        if (dbContext.Entry(dataset).State != EntityState.Unchanged)
                        {
                            dataset.UpdatedBy = httpContext?.User?.UserId() ?? Guid.Empty.ToString();
                            dataset.UserId = new Guid(httpContext?.User?.UserId() ?? Guid.Empty.ToString());
                        }
                        await dbContext.SaveChangesAsync();
                        await AddLineAsync($"{dataset.Code} done in {stopwatch.Elapsed.TimeStr()}");
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        private static CsvWriter GetCsvWriter(TextWriter writer)
        {
            var result = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { IgnoreReferences = true });
            var options = new TypeConverterOptions { Formats = new[] { "o" } };
            result.Context.TypeConverterOptionsCache.AddOptions<DateTime>(options);
            result.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(options);
            return result;
        }

        private static CsvReader GetCsvReader(TextReader reader)
        {
            var result = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { IgnoreReferences = true });

            return result;
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
                    using var reader = new StreamReader(Path.Combine(config[DatasetsFolderConfigKey], dataset.FileName));
                    using var csv = GetCsvReader(reader);
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
                    using var reader = new StreamReader(Path.Combine(config[DatasetsFolderConfigKey], dataset.FileName));
                    using var csv = GetCsvReader(reader);
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
                            Elevation = i.Elevation,
                            Date = i.ValueDate,
                            Value = i.DataValue,
                            Instrument = i.InstrumentName,
                            Sensor = i.SensorName,
                            Comment = i.Comment,
                            Latitude = i.Latitude,
                            Longitude = i.Longitude,
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

        private static bool IsOnDisk(IConfiguration config, Dataset dataset)
        {
            using (SAEONLogs.MethodCall(typeof(DOIHelper), new MethodCallParameters { { "datasetId", dataset?.Id } }))
            {
                try
                {
                    Guard.IsNotNull(dataset, nameof(dataset));
                    SAEONLogs.Verbose("UseDisk: {UseDisk} NeedsUpdate: {NeedsUpdate} FileExists: {FileExists}", config[UseDiskConfigKey]?.IsTrue() ?? false, dataset.NeedsUpdate ?? false, File.Exists(Path.Combine(config[DatasetsFolderConfigKey], dataset.FileName)));
                    var result = (config[UseDiskConfigKey]?.IsTrue() ?? false) && (!dataset.NeedsUpdate ?? false) && File.Exists(Path.Combine(config[DatasetsFolderConfigKey], dataset.FileName));
                    SAEONLogs.Verbose("IsOnDisk: {FileName}, {Result}", dataset.FileName, result);
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
            using (SAEONLogs.MethodCall(typeof(DOIHelper), new MethodCallParameters { { "datasetId", datasetId } }))
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

        public static List<ObservationDTO> Load(ObservationsDbContext dbContext, IConfiguration config, Guid datasetId)
        {
            using (SAEONLogs.MethodCall(typeof(DOIHelper), new MethodCallParameters { { "datasetId", datasetId } }))
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
