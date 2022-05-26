using CsvHelper;
using CsvHelper.Configuration;
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
        private static readonly string DatasetsFolderConfigKey = "DatasetsFolder";

        public static async Task<string> CreateDatasets(ObservationsDbContext dbContext, IHubContext<AdminHub> adminHub, HttpContext httpContext, IConfiguration config)
        {
            using (SAEONLogs.MethodCall(typeof(DOIHelper)))
            {
                try
                {
                    var sb = new StringBuilder();
                    await AddLineAsync("Generating datasets");
                    await GenerateDatasets();
                    await AddLineAsync("Done");
                    return sb.ToString();

                    async Task AddLineAsync(string line)
                    {
                        sb.AppendLine(line);
                        SAEONLogs.Information(line);
                        await adminHub.Clients.All.SendAsync(SignalRDefaults.CreateDatasetsStatusUpdate, line);
                    }

                    async Task GenerateDatasets()
                    {
                        var query = dbContext.VDatasetsExpansion.AsNoTracking()
                            .Where(i => (i.NeedsUpdate ?? false))
                            .OrderBy(i => i.OrganisationName)
                            .ThenBy(i => i.ProgrammeName)
                            .ThenBy(i => i.ProgrammeName)
                            .ThenBy(i => i.SiteName)
                            .ThenBy(i => i.StationName);
                        if (int.TryParse(config["DatasetBatchSize"], out var take))
                        {
                            if (take > 0)
                            {
                                query = (IOrderedQueryable<VDatasetExpansion>)query.Take(take);
                            }
                        }
                        foreach (var datasetExpansion in await query.ToListAsync())
                        {
                            var dataset = await dbContext.Datasets.FirstAsync(i => i.Id == datasetExpansion.Id);
                            await EnsureDataset(dataset);
                        }
                    }

                    async Task EnsureDataset(Dataset dataset)
                    {
                        var stopwatch = new Stopwatch();
                        stopwatch.Start();
                        await AddLineAsync($"{dataset.Code} {dataset.Name}");
                        var datasetsFolder = config[DatasetsFolderConfigKey];
                        var observations = await LoadFromDatabase(dbContext, dataset.Id);
                        var csvConfig = new CsvConfiguration(CultureInfo.CreateSpecificCulture("en-za"))
                        {
                            NewLine = Environment.NewLine,
                            IgnoreReferences = true
                        };
                        using (var writer = new StreamWriter(Path.Combine(datasetsFolder, dataset.FileName)))
                        using (var csv = new CsvWriter(writer, csvConfig))
                        {
                            csv.WriteRecords(observations);
                        }
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

        public static async Task<bool> IsOnDisk(ObservationsDbContext dbContext, IConfiguration config, Guid datasetId)
        {
            using (SAEONLogs.MethodCall(typeof(DOIHelper), new MethodCallParameters { { "datasetId", datasetId } }))
            {
                try
                {
                    var dataset = await dbContext.Datasets.FirstOrDefaultAsync(i => i.Id == datasetId);
                    if (!dataset?.NeedsUpdate ?? false)
                    {
                        var result = File.Exists(Path.Combine(config[DatasetsFolderConfigKey], dataset.FileName));
                        SAEONLogs.Verbose("IsOnDisk: {FileName}, {Result}", dataset.FileName, result);
                        return result;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public static async Task<List<ObservationDTO>> LoadFromDatabase(ObservationsDbContext dbContext, Guid datasetId)
        {
            using (SAEONLogs.MethodCall(typeof(DOIHelper), new MethodCallParameters { { "datasetId", datasetId } }))
            {
                try
                {
                    var dataset = await dbContext.Datasets.FirstOrDefaultAsync(i => i.Id == datasetId);
                    Guard.IsNotNull(dataset, nameof(dataset));
                    return await dbContext.VObservationsExpansion.AsNoTracking()
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
                            Comment = i.Comment
                        })
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public static async Task<List<ObservationDTO>> LoadFromDisk(ObservationsDbContext dbContext, IConfiguration config, Guid datasetId)
        {
            using (SAEONLogs.MethodCall(typeof(DOIHelper), new MethodCallParameters { { "datasetId", datasetId } }))
            {
                try
                {
                    if (!await IsOnDisk(dbContext, config, datasetId)) throw new InvalidOperationException($"Dataset {datasetId} is not IsOnDisk disk");
                    var dataset = await dbContext.Datasets.FirstAsync(i => i.Id == datasetId);
                    var csvConfig = new CsvConfiguration(CultureInfo.CreateSpecificCulture("en-za"))
                    {
                        NewLine = Environment.NewLine,
                        IgnoreReferences = true
                    };
                    using (var reader = new StreamReader(Path.Combine(config[DatasetsFolderConfigKey], dataset.FileName)))
                    using (var csv = new CsvReader(reader, csvConfig))
                    {
                        var result = new List<ObservationDTO>();
                        await foreach (var record in csv.GetRecordsAsync<ObservationDTO>())
                        {
                            result.Add(record);
                        }

                        return result;
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
