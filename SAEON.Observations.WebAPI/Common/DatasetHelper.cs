using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SAEON.AspNet.Auth;
using SAEON.Core;
using SAEON.Logs;
using SAEON.Observations.Auth;
using SAEON.Observations.Core;
using SAEON.Observations.WebAPI.Hubs;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI
{
    public class DatasetCSV
    {
        public string Station { get; set; }
        public string Variable => $"{Phenomenon.Replace(", ", "_")}, {Offering.Replace(", ", "_")}, {Unit.Replace(", ", "_")}";
        public double? Elevation { get; set; }
        public DateTime Date { get; set; }
        public double? Value { get; set; }
        public string Comment { get; set; }
        public string Site { get; set; }
        public string Phenomenon { get; set; }
        public string Offering { get; set; }
        public string Unit { get; set; }
        public string Instrument { get; set; }
        public string Sensor { get; set; }
    }

    public static class DatasetHelper
    {
        public static async Task<string> CreateDatasets(ObservationsDbContext dbContext, IHubContext<AdminHub> adminHub, HttpContext httpContext, IWebHostEnvironment environment)
        {
            using (SAEONLogs.MethodCall(typeof(DOIHelper)))
            {
                try
                {
                    var sb = new StringBuilder();
                    await GenerateDatasets();
                    await AddLineAsync("Done");
                    return sb.ToString();

                    async Task AddLineAsync(string line)
                    {
                        sb.AppendLine(line);
                        SAEONLogs.Information(line);
                        await adminHub.Clients.All.SendAsync(SignalRDefaults.CreateDOIsStatusUpdate, line);
                    }

                    async Task GenerateDatasets()
                    {
                        await AddLineAsync("Generating datasets");
                        var orgCodes = new string[] { "SAEON", "SMCRI", "EFTEON" };
                        foreach (var datasetExpansion in await dbContext.DatasetExpansions.AsNoTracking()
                            .Where(i =>
                                orgCodes.Contains(i.OrganisationCode) &&
                                i.LatitudeNorth.HasValue && i.LongitudeEast.HasValue &&
                                i.VerifiedCount > 0 &&
                                (i.NeedsUpdate ?? false))
                            .OrderBy(i => i.OrganisationName)
                            .ThenBy(i => i.ProgrammeName)
                            .ThenBy(i => i.ProgrammeName)
                            .ThenBy(i => i.SiteName)
                            .ThenBy(i => i.StationName)
                            .ToListAsync())
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
                        var csvFolder = Path.Combine(environment.ContentRootPath, "Datasets");
                        Directory.CreateDirectory(csvFolder);
                        var observations = await dbContext.VObservationExpansions.AsNoTracking()
                                    .Where(i =>
                                        (i.StationId == dataset.StationId) && (i.PhenomenonOfferingId == dataset.PhenomenonOfferingId) && (i.PhenomenonUnitId == dataset.PhenomenonUnitId) &&
                                        ((i.StatusId == null) || (i.StatusName == "Verified")))
                                    .OrderBy(i => i.Elevation).ThenBy(i => i.ValueDate)
                                    //.Select(i => new DatasetCSV
                                    //{
                                    //    Site = i.SiteName,
                                    //    Station = i.StationName,
                                    //    Phenomenon = i.PhenomenonName,
                                    //    Offering = i.OfferingName,
                                    //    Unit = i.UnitName,
                                    //    Elevation = i.Elevation,
                                    //    Date = i.ValueDate,
                                    //    Value = i.DataValue,
                                    //    Instrument = i.InstrumentName,
                                    //    Sensor = i.SensorName,
                                    //    Comment = i.Comment
                                    //})
                                    .ToListAsync();
                        var config = new CsvConfiguration(CultureInfo.CreateSpecificCulture("en-za"))
                        {
                            NewLine = Environment.NewLine,
                            IgnoreReferences = true
                        };
                        using (var writer = new StreamWriter(Path.Combine(csvFolder, dataset.Code + ".csv")))
                        using (var csv = new CsvWriter(writer, config))
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
    }
}
