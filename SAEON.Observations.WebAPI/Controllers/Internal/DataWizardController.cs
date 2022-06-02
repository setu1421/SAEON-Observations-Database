using Humanizer;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SAEON.AspNet.Auth;
using SAEON.Core;
using SAEON.Logs;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    public class DataWizardController : InternalApiController
    {
        public DataWizardController() : base()
        {
            TrackChanges = true;
        }

        private IWebHostEnvironment hostEnvironment;
        protected IWebHostEnvironment HostEnvironment => hostEnvironment ??= HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();

        private void CleanInput(ref DataWizardDataInput input)
        {
            //SAEONLogs.Verbose("Input: {@Input}", input);
            input.Locations = input.Locations.Distinct().ToList();
            input.Variables = input.Variables.Distinct().ToList();
            if (input.StartDate > input.EndDate) throw new ArgumentException("StartDate after EndDate", nameof(input));
            if (input.EndDate < input.StartDate) throw new ArgumentException("EndDate before StartDate", nameof(input));
            input.StartDate = input.StartDate.Date;
            input.EndDate = input.EndDate.Date.AddDays(1).AddMilliseconds(-1);
            //SAEONLogs.Verbose("Processed Input: {@Input}", input);
        }

        private List<VDatasetExpansion> GetDatasets(ref DataWizardDataInput input)
        {
            CleanInput(ref input);
            SAEONLogs.Verbose("Input: {@Input}", input);
            var startDate = input.StartDate;
            var endDate = input.EndDate;
            var elevationMinimum = input.ElevationMinimum;
            var elevationMaximum = input.ElevationMaximum;
            var locations = input.Locations;
            var variables = input.Variables;
            var result = DbContext.VDatasetsExpansion
                .Include(i => i.DigitalObjectIdentifier)
                .AsNoTracking()
                //.AsNoTrackingWithIdentityResolution()
                .AsEnumerable()
                .Where(i =>
                    (!locations.Any() || locations.Contains(new LocationFilter { /*SiteId = i.SiteId,*/ StationId = i.StationId })) &&
                    (!variables.Any() || variables.Contains(new VariableFilter { PhenomenonId = i.PhenomenonId, OfferingId = i.OfferingId, UnitId = i.UnitId })) &&
                    DateRangesOverlap(startDate, endDate, i.StartDate, i.EndDate) &&
                    DoubleRangesOverlap(elevationMinimum, elevationMaximum, i.ElevationMinimum, i.ElevationMaximum))
                .Distinct()
                .OrderBy(i => i.SiteName)
                .ThenBy(i => i.StationName)
                //.ThenBy(i => i.InstrumentName)
                //.ThenBy(i => i.SensorName)
                .ToList();
            SAEONLogs.Verbose("Datasets: {Count}", result.Count);
            return result;

            bool DateRangesOverlap(DateTime startA, DateTime endA, DateTime? startB, DateTime? endB)
            {
                //SAEONLogs.Verbose("startA: {startA} endA: {endA} startB: {startB} endB: {endB}", startA, endA, startB, endB);
                return (!endB.HasValue || (startA < endB)) && (!startB.HasValue || (endA > startB));
            }

            bool DoubleRangesOverlap(double startA, double endA, double? startB, double? endB)
            {
                return (!endB.HasValue || (startA < endB)) && (!startB.HasValue || (endA > startB));
            }
        }

        private DataWizardApproximation CalculateApproximation(DataWizardDataInput input)
        {
            using (SAEONLogs.MethodCall<DataWizardApproximation>(GetType()))
            {
                try
                {
                    SAEONLogs.Verbose("Input: {@Input}", input);
                    var q = GetDatasets(ref input);
                    var rows = q.Sum(i => i.VerifiedCount ?? 0);
                    var result = new DataWizardApproximation
                    {
                        RowCount = rows
                    };
                    if (!(input.Locations.Any()))
                    {
                        result.Errors.Add("Please select a location in the Locations tab");
                    }
                    if (!(input.Variables.Any()))
                    {
                        result.Errors.Add("Please select a variable in the Variables tab");
                    }
                    //SAEONLogs.Information("RowCount: {RowCount} Info: {@input}", rows, input);
                    SAEONLogs.Verbose("Result: {@Result}", result);
                    return result;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet("Approximation")]
        public DataWizardApproximation ApproximationGet([FromQuery] string input)
        {
            using (SAEONLogs.MethodCall<DataWizardApproximation>(GetType()))
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        throw new ArgumentNullException(nameof(input));
                    }
                    return CalculateApproximation(JsonConvert.DeserializeObject<DataWizardDataInput>(input));
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpPost("Approximation")]
        public DataWizardApproximation ApproximationPost([FromBody] DataWizardDataInput input)
        {
            using (SAEONLogs.MethodCall<DataWizardApproximation>(GetType()))
            {
                try
                {
                    SAEONLogs.Information("Input: {@Input}", input);
                    if (input is null)
                    {
                        throw new ArgumentNullException(nameof(input));
                    }
                    return CalculateApproximation(input);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        //private class DataFeature
        //{
        //    public Guid StationId { get; set; }
        //    public Guid PhenomenonOfferingId { get; set; }
        //    public Guid PhenomenonUnitId { get; set; }
        //    public string Name { get; set; }
        //    public string Caption { get; set; }

        //    public override bool Equals(object obj)
        //    {
        //        return obj is DataFeature feature &&
        //               StationId.Equals(feature.StationId) &&
        //               PhenomenonOfferingId.Equals(feature.PhenomenonOfferingId) &&
        //               PhenomenonUnitId.Equals(feature.PhenomenonUnitId);
        //    }

        //    public override int GetHashCode()
        //    {
        //        return HashCode.Combine(StationId, PhenomenonOfferingId, PhenomenonUnitId);
        //    }
        //}

        private DataWizardDataOutput GetData(DataWizardDataInput input, bool includeChart)
        {
            string GetCode(string stationCode, string phenomenonCode, string offeringCode, string unitCode)
            {
                return $"{MetadataHelper.CleanPrefixes(stationCode).Replace("_", string.Empty)}_{phenomenonCode.Replace("_", string.Empty)}_{offeringCode.Replace("_", string.Empty)}_{unitCode.Replace("_", string.Empty)}".Replace(" ", string.Empty);
            }

            string GetName(string stationName, string phenomenonName, string offeringName, string unitName)
            {
                return $"{MetadataHelper.CleanPrefixes(stationName.Replace(", ", "_"))}, {phenomenonName.Replace(", ", "_")}, {offeringName.Replace(", ", "_")}, {unitName.Replace(", ", "_")}";
            }

            var stopwatch = new Stopwatch();
            var stageStopwatch = new Stopwatch();
            stopwatch.Start();
            stageStopwatch.Start();
            var result = new DataWizardDataOutput();
            var datasets = GetDatasets(ref input);
            SAEONLogs.Verbose("Datasets: Stage {Stage} Total {Total}", stageStopwatch.Elapsed.TimeStr(), stopwatch.Elapsed.TimeStr());
            stageStopwatch.Restart();
            //var features = datasets.Select(i => new DataFeature
            //{
            //    StationId = i.StationId,
            //    PhenomenonOfferingId = i.PhenomenonOfferingId,
            //    PhenomenonUnitId = i.PhenomenonUnitId,
            //    Name = GetCode(i.StationCode, i.PhenomenonCode, i.OfferingCode, i.UnitCode),
            //    Caption = GetName(i.StationName, i.PhenomenonName, i.OfferingName, i.UnitName)
            //}).Distinct().ToList();
            //SAEONLogs.Verbose("Features: Stage {Stage} Total {Total}", stageStopwatch.Elapsed.TimeStr(), stopwatch.Elapsed.TimeStr());
            stageStopwatch.Restart();
            var observations = new List<ObservationDTO>();
            foreach (var dataset in datasets)
            {
                var datasetObservations = DatasetHelper.Load(DbContext, Config, dataset.Id)
                    .Where(i =>
                        ((i.Date >= input.StartDate) && (i.Date <= input.EndDate)) &&
                        (!i.Elevation.HasValue || (i.Elevation >= input.ElevationMinimum)) &&
                        (!i.Elevation.HasValue || (i.Elevation <= input.ElevationMaximum)))
                    .Distinct();
                observations.AddRange(datasetObservations);
            }
            observations = observations
                .Distinct()
                .OrderBy(obs => obs.Site)
                .ThenBy(obs => obs.Station)
                .ThenBy(obs => obs.Elevation)
                .ThenBy(obs => obs.Phenomenon)
                .ThenBy(obs => obs.Offering)
                .ThenBy(obs => obs.Unit)
                .ThenBy(obs => obs.Date)
                .ToList();
            result.Data.AddRange(observations);
            SAEONLogs.Verbose("Observations: {Observations} Stage {Stage} Total {Total}", observations.Count, stageStopwatch.Elapsed.TimeStr(), stopwatch.Elapsed.TimeStr());
            stageStopwatch.Restart();
            // Metadata
            result.Metadata.PublicationDate = result.Date;
            var titles = observations.Where(i => i.Station.StartsWith("ELW, ")).Select(i =>
            {
                var variable = $"{MetadataHelper.CleanPrefixes(i.Phenomenon)}, {i.Offering}, {i.Unit}";
                var siteName = MetadataHelper.CleanPrefixes(i.Site);
                var stationName = MetadataHelper.CleanPrefixes(i.Station);
                if (stationName.EndsWith(siteName))
                {
                    stationName = stationName.Substring(0, stationName.Length - siteName.Length - 2);
                }
                return $"{variable} for {stationName}";
            }).Union(observations.Where(i => !i.Station.StartsWith("ELW, ")).Select(i =>
            {
                var variable = $"{MetadataHelper.CleanPrefixes(i.Phenomenon)}, {i.Offering}, {i.Unit}";
                var siteName = MetadataHelper.CleanPrefixes(i.Site);
                var stationName = MetadataHelper.CleanPrefixes(i.Station);
                return $"{variable} for {siteName}, {stationName}";
            })).Distinct();
            result.Metadata.Title = string.Join("; ", titles.OrderBy(i => i));
            var descriptions = observations.Where(i => i.Station.StartsWith("ELW, ")).Select(i =>
            {
                var variable = $"{MetadataHelper.CleanPrefixes(i.Phenomenon)}, {i.Offering}, {i.Unit}";
                var siteName = MetadataHelper.CleanPrefixes(i.Site);
                var stationName = MetadataHelper.CleanPrefixes(i.Station);
                if (stationName.EndsWith(siteName))
                {
                    stationName = stationName.Substring(0, stationName.Length - siteName.Length - 2);
                }
                return $"{variable} for {stationName}";
            }).Union(observations.Where(i => !i.Station.StartsWith("ELW, ")).Select(i =>
            {
                var variable = $"{MetadataHelper.CleanPrefixes(i.Phenomenon)}, {i.Offering}, {i.Unit}";
                var siteName = MetadataHelper.CleanPrefixes(i.Site);
                var stationName = MetadataHelper.CleanPrefixes(i.Station);
                return $"{variable} for {siteName}, {stationName}";
            })).Distinct();
            result.Metadata.Description = string.Join("; ", descriptions.OrderBy(i => i));
            //var datasetCodes = observations.Select(i => $"{i.Station}~{i.Phenomenon}~{i.Offering}~{i.Unit}").Distinct();
            //SAEONLogs.Verbose("DatasetCodes: {DatasetCodes}", datasetCodes.ToList());
            var datasetDOIs = datasets.Select(i => i.DigitalObjectIdentifier).OrderBy(i => i.Code).ToList();
            if (!datasetDOIs.Any())
            {
                SAEONLogs.Error("No dataset DOIs found! Dataset Codes: {Codes}", datasets.Select(i => i.Code).ToList());
            }
            else
            {
                result.Metadata.Citation = $"Please cite the use of {"this".ToQuantity(datasetDOIs.Count, ShowQuantityAs.None)} {"dataset".ToQuantity(datasetDOIs.Count, ShowQuantityAs.None)} as follows: " + string.Join("; ", datasetDOIs.Select(i => $"{i.Citation} accessed {result.Date:yyyy-MM-dd HH:mm}"));
                var sbHtml = new StringBuilder();
                sbHtml.AppendLine("<p>");
                sbHtml.AppendLine($"Please cite the use of {"this".ToQuantity(datasetDOIs.Count, ShowQuantityAs.None)} {"dataset".ToQuantity(datasetDOIs.Count, ShowQuantityAs.None)} as follows:");
                sbHtml.AppendHtmlUL(datasetDOIs.Select(i => $"{i.CitationHtml} accessed {result.Date:yyyy-MM-dd HH:mm}"));
                sbHtml.AppendLine("</p>");
                result.Metadata.CitationHtml = sbHtml.ToString();
            }
            // Keywords
            //result.Metadata.Subjects.AddRange(observations.Select(i => i.OrganisationName).Distinct().Select(i => new MetadataSubject { Name = MetadataHelper.CleanPrefixes(i) }));
            //result.Metadata.Subjects.AddRange(observations.Select(i => i.ProgrammeName).Distinct().Select(i => new MetadataSubject { Name = MetadataHelper.CleanPrefixes(i) }));
            //result.Metadata.Subjects.AddRange(observations.Select(i => i.ProjectName).Distinct().Select(i => new MetadataSubject { Name = MetadataHelper.CleanPrefixes(i) }));
            result.Metadata.Subjects.AddRange(observations.Select(i => i.Site).Distinct().Select(i => new MetadataSubject { Name = MetadataHelper.CleanPrefixes(i) }));
            result.Metadata.Subjects.AddRange(observations.Select(i => i.Station).Distinct().Select(i => new MetadataSubject { Name = MetadataHelper.CleanPrefixes(i) }));
            result.Metadata.Subjects.AddRange(observations.Select(i => i.Phenomenon).Distinct().Select(i => new MetadataSubject { Name = i }));
            result.Metadata.Subjects.AddRange(observations.Select(i => $"{i.Phenomenon}, {i.Offering}, {i.Unit}").Distinct().Select(i => new MetadataSubject { Name = i }));
            // Dates
            result.Metadata.StartDate = observations.Min(i => i.Date);
            result.Metadata.EndDate = observations.Max(i => i.Date);
            // Bounding box
            result.Metadata.LatitudeNorth = observations.Max(i => i.Latitude);
            result.Metadata.LatitudeSouth = observations.Min(i => i.Latitude);
            result.Metadata.LongitudeEast = observations.Max(i => i.Longitude);
            result.Metadata.LongitudeWest = observations.Min(i => i.Longitude);
            result.Metadata.ElevationMaximum = observations.Max(i => i.Elevation);
            result.Metadata.ElevationMinimum = observations.Min(i => i.Elevation);
            // Places - Might come later
            //SAEONLogs.Verbose("Metadata: {@Metadata}", result.Metadata);
            SAEONLogs.Verbose("Metadata: Stage {Stage} Total {Total}", stageStopwatch.Elapsed.TimeStr(), stopwatch.Elapsed.TimeStr());
            stageStopwatch.Restart();
            //Chart series
            if (includeChart)
            {
                foreach (var dataset in datasets)
                {
                    var series = new ChartSeries
                    {
                        Name = GetCode(dataset.StationCode, dataset.PhenomenonCode, dataset.OfferingCode, dataset.UnitCode),
                        Caption = GetName(dataset.StationName, dataset.PhenomenonName, dataset.OfferingName, dataset.UnitName)
                    };
                    series.Data.AddRange(observations.Where(o => o.Station == dataset.StationName && o.Phenomenon == dataset.PhenomenonName && o.Offering == dataset.OfferingName && o.Unit == dataset.UnitName)
                        .OrderBy(i => i.Date)
                        .Select(i => new ChartData { Date = i.Date, Value = i.Value }));
                    result.ChartSeries.Add(series);
                }
                //SAEONLogs.Verbose("ChartSeries: Count: {count} Stage {Stage} Total {Total} Series: {@Series}", result.ChartSeries.Count, stageStopwatch.Elapsed.TimeStr(), stopwatch.Elapsed.TimeStr(), result.ChartSeries);
                SAEONLogs.Verbose("ChartSeries: Count: {count} Stage {Stage} Total {Total}", result.ChartSeries.Count, stageStopwatch.Elapsed.TimeStr(), stopwatch.Elapsed.TimeStr());
                stageStopwatch.Restart();
            }
            SAEONLogs.Verbose("GetData: {Total}", stopwatch.Elapsed.TimeStr());
            return result;
        }

        [HttpGet("GetData")]
        //[Authorize]
        public DataWizardDataOutput DataGet([FromQuery] string input)
        {
            using (SAEONLogs.MethodCall<DataWizardDataOutput>(GetType()))
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        throw new ArgumentNullException(nameof(input));
                    }
                    return GetData(JsonConvert.DeserializeObject<DataWizardDataInput>(input), true);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpPost("GetData")]
        //[Authorize]
        public DataWizardDataOutput DataPost([FromBody] DataWizardDataInput input)
        {
            using (SAEONLogs.MethodCall<DataWizardDataOutput>(GetType()))
            {
                try
                {
                    if (input is null)
                    {
                        throw new ArgumentNullException(nameof(input));
                    }
                    return GetData(input, true);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        private async Task<UserDownload> GetDownload(DataWizardDownloadInput input)
        {
            string GetChecksum(string file)
            {
                using (FileStream stream = System.IO.File.OpenRead(file))
                {
                    using (var sha = SHA256.Create())
                    {
                        byte[] checksum = sha.ComputeHash(stream);
                        return BitConverter.ToString(checksum).Replace("-", String.Empty);
                    }
                }
            }

            //List<string> GetValidationErrors(DbEntityValidationException ex)
            //{
            //    return ex.EntityValidationErrors.SelectMany(e => e.ValidationErrors.Select(m => m.PropertyName + ": " + m.ErrorMessage)).ToList();
            //}

            async Task SaveChangesAsync()
            {
                try
                {
                    await DbContext.SaveChangesAsync();
                }
                //catch (DbEntityValidationException ex)
                //{
                //    SAEONLogs.Exception(ex, string.Join("; ", GetValidationErrors(ex)));
                //    throw;
                //}
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }

            var executionStrategy = DbContext.Database.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = DbContext.Database.BeginTransaction())
                {
                    try
                    {
                        SAEONLogs.Verbose("UserId: {userId} Claims: {claims}", User.UserId(), User.Claims.ToClaimsList());
                        // Get Data
                        var output = GetData(input, false);
                        // Create Download
                        var doiCode = $"Data download on {output.Date:yyyy-MM-dd HH:mm:ss.fff} {User.UserId()}";
                        //// Get a DOI
                        //SAEONLogs.Verbose("Minting DOI");
                        //var doi = await DOIHelper.CreateAdHocDOI(DbContext, HttpContext, doiCode, doiCode);
                        //SAEONLogs.Verbose("DOI: {@DOI}", doi);
                        var metadata = new Metadata(output.Metadata)
                        {
                            Accessed = output.Date
                        };
                        metadata.Generate();
                        SAEONLogs.Verbose("Metadata: {@Metadata}", metadata);
                        SAEONLogs.Verbose("Adding UserDownload");
                        var baseUrl = $"{Config["QuerySiteUrl"].AddEndForwardSlash()}Query/Data";
                        var result = new UserDownload
                        {
                            UserId = User.UserId(),
                            Date = output.Date,
                            Name = doiCode,
                            Title = metadata.Title,
                            Description = metadata.Description,
                            DescriptionHtml = metadata.DescriptionHtml,
                            Citation = metadata.Citation,
                            CitationHtml = metadata.CitationHtml,
                            //DigitalObjectIdentifierId = doi.Id,
                            Input = JsonConvert.SerializeObject(input),
                            RequeryUrl = $"{baseUrl}/Requery",
                            DownloadUrl = $"{baseUrl}/ViewDownload",
                            ZipFullName = $"{HostEnvironment.ContentRootPath.AddEndForwardSlash()}Downloads/{output.Date:yyyyMM}",
                            ZipCheckSum = "ABCD",
                            ZipUrl = $"{baseUrl}/DownloadZip",
                            AddedBy = User.UserId(),
                            UpdatedBy = User.UserId(),
                            IPAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString()
                        };
                        SAEONLogs.Verbose("UserDownload: {@UserDownload}", result);
                        DbContext.UserDownloads.Add(result);
                        await SaveChangesAsync();
                        SAEONLogs.Verbose("UserDownload: {@UserDownload}", result);
                        result.ZipCheckSum = null;
                        //result.Description += Environment.NewLine + "Please cite as follows:" + Environment.NewLine + metadata.CitationText;
                        result.RequeryUrl = $"{baseUrl}/Requery/{result.Id}";
                        result.DownloadUrl = $"{baseUrl}/ViewDownload/{result.Id}";
                        var folder = $"{HostEnvironment.ContentRootPath.AddEndBackSlash()}Downloads\\{output.Date:yyyyMM}";
                        var dirInfo = Directory.CreateDirectory(Path.Combine(folder, result.Id.ToString()));
                        result.ZipFullName = Path.Combine(folder, $"{result.Id}.zip");
                        result.ZipUrl = $"{baseUrl}/DownloadZip/{result.Id}";
                        // Create files
                        SAEONLogs.Verbose("Creating files");
                        var fileName = $"SAEON Observations Database Download {output.Date:yyyyMMdd HHmmss}";
                        switch (input.DownloadFormat)
                        {
                            case DownloadFormat.CSV:
                                fileName += ".csv";
                                System.IO.File.WriteAllText(Path.Combine(dirInfo.FullName, fileName), output.Data.ToCSV());
                                break;
                            case DownloadFormat.Excel:
                                fileName += ".xlsx";
                                System.IO.File.WriteAllBytes(Path.Combine(dirInfo.FullName, fileName), output.Data.ToExcel());
                                break;
                            case DownloadFormat.NetCDF:
                                throw new NotSupportedException();
                                //break;
                        }
                        // Create Zip
                        ZipFile.CreateFromDirectory(dirInfo.FullName, Path.Combine(folder, $"{result.Id}.zip"));
                        result.FileSize = new FileInfo(Path.Combine(dirInfo.FullName, fileName)).Length;
                        result.ZipSize = new FileInfo(Path.Combine(folder, $"{result.Id}.zip")).Length;
                        // Download info extras
                        System.IO.File.WriteAllText(Path.Combine(dirInfo.FullName, "Input.json"), JsonConvert.SerializeObject(input, Formatting.Indented));
                        //System.IO.File.WriteAllText(Path.Combine(dirInfo.FullName, "Metadata.json"), metadata.ToJson());
                        System.IO.File.WriteAllText(Path.Combine(dirInfo.FullName, "Download.json"),
                            JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
                        // Generate Checksum
                        result.ZipCheckSum = GetChecksum(result.ZipFullName);
                        var jChecksum = new JObject(new JProperty("Checksum", result.ZipCheckSum));
                        //System.IO.File.WriteAllText(Path.Combine(folder, $"{result.Id} Checksum.json"), jChecksum.ToString());
                        System.IO.File.WriteAllText(Path.Combine(dirInfo.FullName, $"{result.Id} Checksum.json"), jChecksum.ToString());
                        //dirInfo.Delete(true);
                        SAEONLogs.Verbose("UserDownload: {@UserDownload}", result);
                        await SaveChangesAsync();
                        await RequestLogger.LogAsync(DbContext, Request, result.Id.ToString());
                        transaction.Commit();
                        return result;
                    }
                    catch (Exception ex)
                    {
                        SAEONLogs.Exception(ex);
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception rex)
                        {
                            SAEONLogs.Exception(rex);
                        }
                        throw;
                    }
                }
            });
        }

        [HttpGet("GetDownload/{input}")]
        [Authorize]
        public async Task<UserDownload> DownloadGetAsync(string input)
        {
            using (SAEONLogs.MethodCall<UserDownload>(GetType()))
            {
                try
                {
                    SAEONLogs.Verbose("Uri: {Uri}", Request.GetUri());
                    SAEONLogs.Verbose("Input: {input}", input);
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        throw new ArgumentNullException(nameof(input));
                    }
                    return await GetDownload(JsonConvert.DeserializeObject<DataWizardDownloadInput>(input));
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpPost("GetDownload")]
        [Authorize]
        public async Task<UserDownload> DownloadPostAsync([FromBody] DataWizardDownloadInput input)
        {
            using (SAEONLogs.MethodCall<UserDownload>(GetType()))
            {
                try
                {
                    SAEONLogs.Verbose("Uri: {Uri}", Request.GetUri());
                    SAEONLogs.Verbose("Input: {@input}", input);
                    if (input is null)
                    {
                        throw new ArgumentNullException(nameof(input));
                    }
                    return await GetDownload(input);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet("DownloadZip/{id:guid}")]
        public ActionResult DownloadZip(Guid id)
        {
            using (SAEONLogs.MethodCall<UserDownload>(GetType(), new MethodCallParameters { { "Id", id } }))
            {
                try
                {
                    var userDownload = DbContext.UserDownloads.FirstOrDefault(i => i.Id == id);
                    if (userDownload is null)
                    {
                        throw new ArgumentException($"UserDownload with Id: {id} not found!");
                        //return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"UserDownload with Id: {id} not found!");
                    }
                    var fs = System.IO.File.OpenRead(userDownload.ZipFullName);
                    return File(fs, MediaTypeNames.Application.Zip, Path.GetFileName(userDownload.ZipFullName));
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