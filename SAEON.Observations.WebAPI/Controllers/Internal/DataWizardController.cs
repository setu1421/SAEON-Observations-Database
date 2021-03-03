using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SAEON.Core;
using SAEON.Logs;
using SAEON.Observations.Auth;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    public class DataWizardController : InternalApiController
    {
        private IWebHostEnvironment hostEnvironment;
        protected IWebHostEnvironment HostEnvironment => hostEnvironment ??= HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();

        private void CleanInput(ref DataWizardDataInput input)
        {
            //SAEONLogs.Verbose("Input: {@Input}", input);
            input.Locations = input.Locations.Distinct().ToList();
            input.Variables = input.Variables.Distinct().ToList();
            input.StartDate = input.StartDate.Date;
            input.EndDate = input.EndDate.Date.AddDays(1);
            //SAEONLogs.Verbose("Processed Input: {@Input}", input);
        }

        private IQueryable<VImportBatchSummary> GetSummaryQuery(ref DataWizardDataInput input)
        {
            CleanInput(ref input);
            SAEONLogs.Verbose("Input: {@Input}", input);
            var startDate = input.StartDate;
            var endDate = input.EndDate;
            var elevationMinimum = input.ElevationMinimum;
            var elevationMaximum = input.ElevationMaximum;
            var result = DbContext.VImportBatchSummary
                .AsNoTracking()
                //.AsNoTrackingWithIdentityResolution()
                .Where(ibs =>
                (ibs.Count > 0) &&
                ((ibs.LatitudeNorth != null) && (ibs.LatitudeSouth != null) && (ibs.LongitudeEast != null) && (ibs.LongitudeWest != null)) &&
                ((ibs.StartDate >= startDate) && (ibs.EndDate < endDate)) &&
                (!ibs.ElevationMinimum.HasValue || (ibs.ElevationMinimum >= elevationMinimum)) &&
                (!ibs.ElevationMaximum.HasValue || (ibs.ElevationMaximum <= elevationMaximum)));
            return result.AsQueryable();
        }

        private List<VImportBatchSummary> GetSummary(ref DataWizardDataInput input)
        {
            CleanInput(ref input);
            SAEONLogs.Verbose("Input: {@Input}", input);
            var locations = input.Locations;
            var variables = input.Variables;
            var result = GetSummaryQuery(ref input)
                .AsEnumerable() // Force fetch from database
                .Where(ibs =>
                    (!locations.Any() || locations.Contains(new Location { StationId = ibs.StationId })) &&
                    (!variables.Any() || variables.Contains(new Variable { PhenomenonId = ibs.PhenomenonId, OfferingId = ibs.OfferingId, UnitId = ibs.UnitId })))
                .OrderBy(i => i.SiteName)
                .ThenBy(i => i.StationName)
                .ThenBy(i => i.InstrumentName)
                .ThenBy(i => i.SensorName)
                .ToList();
            SAEONLogs.Verbose("Summaries: {Count}", result.Count);
            return result;
        }

        private DataWizardApproximation CalculateApproximation(DataWizardDataInput input)
        {
            using (SAEONLogs.MethodCall<DataWizardApproximation>(GetType()))
            {
                try
                {
                    SAEONLogs.Verbose("Input: {@Input}", input);
                    var q = GetSummary(ref input);
                    var rows = q.Sum(i => i.Count);
                    //var rows = q.Select(i => i.Count).ToList().Sum();
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
                    if (input == null)
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

        private class DataFeature
        {
            public Guid SensorID { get; set; }
            public Guid PhenomenonOfferingId { get; set; }
            public Guid PhenomenonUnitId { get; set; }
            public string Phenomenon { get; set; }
            public string Offering { get; set; }
            public string Unit { get; set; }
            public string Symbol { get; set; }
            public string Name { get; set; }
            public string Caption { get; set; }

            public override bool Equals(object obj)
            {
                return obj is DataFeature feature &&
                       Name == feature.Name;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Name);
            }
        }

        private DataWizardDataOutput GetData(DataWizardDataInput input, bool includeChart)
        {
            string GetCode(string sensorCode, string phenomenonCode, string offeringCode, string unitCode)
            {
                return $"{sensorCode.Replace("_", string.Empty)}_{phenomenonCode.Replace("_", string.Empty)}_{offeringCode.Replace("_", string.Empty)}_{unitCode.Replace("_", string.Empty)}".Replace(" ", string.Empty);
            }

            string GetName(string sensorName, string phenomenonName, string offeringName, string unitName)
            {
                return $"{sensorName.Replace(", ", "_")}, {phenomenonName.Replace(", ", "_")}, {offeringName.Replace(", ", "_")}, {unitName.Replace(", ", "_")}";
            }

            var stopwatch = new Stopwatch();
            var stageStopwatch = new Stopwatch();
            stopwatch.Start();
            stageStopwatch.Start();
            var result = new DataWizardDataOutput();
            result.DataMatrix.AddColumn("SiteName", "Site", MaxtixDataType.mdtString);
            result.DataMatrix.AddColumn("StationName", "Station", MaxtixDataType.mdtString);
            result.DataMatrix.AddColumn("InstrumentName", "Instrument", MaxtixDataType.mdtString);
            result.DataMatrix.AddColumn("SensorName", "Sensor", MaxtixDataType.mdtString);
            result.DataMatrix.AddColumn("Date", "Date", MaxtixDataType.mdtDate);
            result.DataMatrix.AddColumn("Elevation", "Elevation", MaxtixDataType.mdtDouble);

            var summaries = GetSummary(ref input);
            SAEONLogs.Verbose("Summaries: Stage {Stage} Total {Total}", stageStopwatch.Elapsed.TimeStr(), stopwatch.Elapsed.TimeStr());
            stageStopwatch.Restart();
            var qFeatures = summaries.Select(i => new { i.SensorId, i.SensorCode, i.SensorName, i.PhenomenonOfferingId, i.PhenomenonUnitId, i.PhenomenonCode, i.PhenomenonName, i.OfferingCode, i.OfferingName, i.UnitCode, i.UnitName, i.UnitSymbol }).Distinct();
            var features = qFeatures.ToList().Select(i => new DataFeature
            {
                SensorID = i.SensorId,
                PhenomenonOfferingId = i.PhenomenonOfferingId,
                PhenomenonUnitId = i.PhenomenonUnitId,
                Phenomenon = i.PhenomenonName,
                Offering = i.OfferingName,
                Unit = i.UnitName,
                Symbol = i.UnitSymbol,
                Name = GetCode(i.SensorCode, i.PhenomenonCode, i.OfferingCode, i.UnitCode),
                Caption = GetName(i.SensorName, i.PhenomenonName, i.OfferingName, i.UnitName)
            });
            foreach (var feature in features)
            {
                result.DataMatrix.AddColumn(feature.Name, feature.Caption, MaxtixDataType.mdtDouble);
                SAEONLogs.Verbose("Feature: {@Feature}", feature);
            }
            var phenomenonOfferingIds = features.Select(f => f.PhenomenonOfferingId);
            var phenomenonUnitIds = features.Select(f => f.PhenomenonUnitId);
            SAEONLogs.Verbose("Features: Stage {Stage} Total {Total}", stageStopwatch.Elapsed.TimeStr(), stopwatch.Elapsed.TimeStr());
            stageStopwatch.Restart();
            var importBatchIDs = summaries.Select(i => i.ImportBatchId).Distinct();
            var observations = DbContext.VObservationExpansions.AsNoTracking()
                .Where(i =>
                    importBatchIDs.Contains(i.ImportBatchId) &&
                    ((i.StatusId == null) || (i.StatusName == "Verified")) &&
                    ((i.ValueDate >= input.StartDate) && (i.ValueDate < input.EndDate)) &&
                    (!i.Elevation.HasValue || (i.Elevation >= input.ElevationMinimum)) &&
                    (!i.Elevation.HasValue || (i.Elevation <= input.ElevationMaximum)))
                .OrderBy(obs => obs.SiteName)
                .ThenBy(obs => obs.StationName)
                .ThenBy(obs => obs.InstrumentName)
                .ThenBy(obs => obs.SensorName)
                .ThenBy(obs => obs.ValueDate)
                .ThenBy(obs => obs.Elevation)
                .AsEnumerable() // Force fetch from database
                .Where(ibs =>
                    (!input.Locations.Any() || input.Locations.Contains(new Location { StationId = ibs.StationId })) &&
                    (!input.Variables.Any() || input.Variables.Contains(new Variable { PhenomenonId = ibs.PhenomenonId, OfferingId = ibs.OfferingId, UnitId = ibs.UnitId })))
                .ToList();
            SAEONLogs.Verbose("Observations: {Observations} Stage {Stage} Total {Total}", observations.Count, stageStopwatch.Elapsed.TimeStr(), stopwatch.Elapsed.TimeStr());
            stageStopwatch.Restart();
            var siteId = new Guid();
            var stationId = new Guid();
            var instrumentId = new Guid();
            var sensorId = new Guid();
            var date = DateTime.MinValue;
            double? elevation = null;
            DataMatrixRow row = null;
            // Data Matrix
            int nRow = 0;
            foreach (var obs in observations)
            {
                if (++nRow % 10000 == 0)
                {
                    SAEONLogs.Verbose("DataMatrix.Row: {Row}", nRow);
                }
                if ((row == null) || (obs.SiteId != siteId) || (obs.StationId != stationId) || (obs.InstrumentId != instrumentId) || (obs.SensorId != sensorId) || (obs.ValueDate != date) || (obs.Elevation != elevation))
                {
                    row = result.DataMatrix.AddRow();
                    row["SiteName"] = obs.SiteName;
                    row["StationName"] = obs.StationName;
                    row["InstrumentName"] = obs.InstrumentName;
                    row["SensorName"] = obs.SensorName;
                    row["Date"] = obs.ValueDate;
                    row["Elevation"] = obs.Elevation;
                    siteId = obs.SiteId;
                    stationId = obs.StationId;
                    instrumentId = obs.InstrumentId;
                    sensorId = obs.SensorId;
                    date = obs.ValueDate;
                    elevation = obs.Elevation;
                }
                var name = GetCode(obs.SensorCode, obs.PhenomenonCode, obs.OfferingCode, obs.UnitCode);
                //SAEONLogs.Verbose("Name: {Name}",name);
                row[name] = obs.DataValue;
            }
            SAEONLogs.Verbose("DataMatrix: Rows: {Rows} Cols: {Cols} Stage {Stage} Total {Total}", result.DataMatrix.Rows.Count, result.DataMatrix.Columns.Count, stageStopwatch.Elapsed.TimeStr(), stopwatch.Elapsed.TimeStr());
            stageStopwatch.Restart();
            // Metadata
            result.Metadata.PublicationDate = result.Date;
            result.Metadata.Title = "Observations in the SAEON Observations Database for " +
                string.Join("; ", observations.Select(i => $"{i.SiteName} - {i.StationName} of {i.PhenomenonName}").Distinct().OrderBy(i => i));
            result.Metadata.Description = "Observations in the SAEON Observations Database for " +
                string.Join("; ", observations.Select(i => $"{i.SiteName} - {i.StationName} of {i.PhenomenonName}, {i.OfferingName}, {i.UnitName}").Distinct().OrderBy(i => i));
            // Keywords
            result.Metadata.Subjects.AddRange(observations.Select(i => i.SiteName).Distinct().Select(i => new MetadataSubject { Name = i }));
            result.Metadata.Subjects.AddRange(observations.Select(i => i.StationName).Distinct().Select(i => new MetadataSubject { Name = i }));
            result.Metadata.Subjects.AddRange(observations.Select(i => i.PhenomenonName).Distinct().Select(i => new MetadataSubject { Name = i }));
            result.Metadata.Subjects.AddRange(observations.Select(i => $"{i.PhenomenonName}, {i.OfferingName}, {i.UnitName}").Distinct().Select(i => new MetadataSubject { Name = i }));
            // Dates
            result.Metadata.StartDate = observations.Min(i => i.ValueDate);
            result.Metadata.EndDate = observations.Min(i => i.ValueDate);
            // Bounding box
            result.Metadata.LatitudeNorth = observations.Max(i => i.Latitude);
            result.Metadata.LatitudeSouth = observations.Min(i => i.Latitude);
            result.Metadata.LongitudeEast = observations.Max(i => i.Longitude);
            result.Metadata.LongitudeWest = observations.Min(i => i.Longitude);
            result.Metadata.ElevationMaximum = observations.Max(i => i.Elevation);
            result.Metadata.ElevationMinimum = observations.Min(i => i.Elevation);
            // Places - Might come later
            SAEONLogs.Verbose("Metadata: {@Metadata}", result.Metadata);
            SAEONLogs.Verbose("Metadata: Stage {Stage} Total {Total}", stageStopwatch.Elapsed.TimeStr(), stopwatch.Elapsed.TimeStr());
            //Chart series
            if (includeChart)
            {
                ChartSeries series = null;
                siteId = new Guid();
                stationId = new Guid();
                instrumentId = new Guid();
                sensorId = new Guid();
                nRow = 0;
                foreach (var obs in observations)
                {
                    if (++nRow % 10000 == 0)
                    {
                        SAEONLogs.Verbose("Chart.Row: {Row}", nRow);
                    }
                    if ((series == null) || (obs.SiteId != siteId) || (obs.StationId != stationId) || (obs.InstrumentId != instrumentId) || (obs.SensorId != sensorId))
                    {
                        series = new ChartSeries
                        {
                            Name = GetCode(obs.SensorCode, obs.PhenomenonCode, obs.OfferingCode, obs.UnitCode),
                            Caption = GetName(obs.SensorName, obs.PhenomenonName, obs.OfferingName, obs.UnitName)
                        };
                        result.ChartSeries.Add(series);
                        siteId = obs.SiteId;
                        stationId = obs.StationId;
                        instrumentId = obs.InstrumentId;
                        sensorId = obs.SensorId;
                    }
                    series.Add(obs.ValueDate, obs.DataValue);
                }
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
                    if (input == null)
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

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    SAEONLogs.Verbose("UserId: {userId} Claims: {claims}", User.UserId(), User.Claims.ToClaimsList());
                    // Get Data
                    var output = GetData(input, false);
                    // Create Download
                    var doiCode = $"Data download on {output.Date:yyyy-MM-dd HH:mm:ss.fff}";
                    // Get a DOI
                    SAEONLogs.Verbose("Minting DOI");
                    var doi = await DOIHelper.CreateAdHocDOI(DbContext, HttpContext, doiCode, output.Metadata.Title);
                    SAEONLogs.Verbose("DOI: {@DOI}", doi);
                    var metadata = await MetadataHelper.CreateAddHocMetadata(DbContext, doi, output.Metadata);
                    metadata.Accessed = output.Date;
                    metadata.Generate(metadata.Title, metadata.Description);
                    SAEONLogs.Verbose("Metadata: {@Metadata}", metadata);
                    SAEONLogs.Verbose("Adding UserDownload");
                    var baseUrl = $"{Config["QuerySiteUrl"].AddTrailingForwardSlash()}DataWizard";
                    var result = new UserDownload
                    {
                        UserId = User.UserId(),
                        Name = doiCode,
                        Description = metadata.Title,
                        Date = output.Date,
                        DigitalObjectIdentifierId = doi.Id,
                        Input = JsonConvert.SerializeObject(input),
                        RequeryUrl = $"{baseUrl}/Requery",
                        DownloadUrl = $"{baseUrl}/ViewDownload",
                        ZipFullName = $"{HostEnvironment.ContentRootPath.AddTrailingForwardSlash()}Downloads/{output.Date:yyyyMM}",
                        ZipCheckSum = "ABCD",
                        ZipUrl = $"{baseUrl}/DownloadZip",
                        AddedBy = User.UserId(),
                        UpdatedBy = User.UserId()
                    };
                    SAEONLogs.Verbose("UserDownload: {@UserDownload}", result);
                    DbContext.UserDownloads.Add(result);
                    await SaveChangesAsync();
                    result = await DbContext.UserDownloads.Include(i => i.DigitalObjectIdentifier).FirstOrDefaultAsync(i => i.Name == doiCode);
                    if (result == null)
                    {
                        throw new InvalidOperationException($"Unable to find UserDownload {doiCode}");
                    }
                    SAEONLogs.Verbose("UserDownload: {@UserDownload}", result);
                    result.ZipCheckSum = null;
                    result.Description += Environment.NewLine + "Please cite as follows:" + Environment.NewLine + metadata.CitationText;
                    result.RequeryUrl = $"{baseUrl}/Requery/{result.Id}";
                    result.DownloadUrl = $"{baseUrl}/ViewDownload/{result.Id}";
                    var folder = $"{HostEnvironment.ContentRootPath.AddTrailingForwardSlash()}Downloads/{output.Date:yyyyMM}";
                    var dirInfo = Directory.CreateDirectory(Path.Combine(folder, result.Id.ToString()));
                    result.ZipFullName = Path.Combine(folder, $"{result.Id}.zip");
                    result.ZipUrl = $"{baseUrl}/DownloadZip/{result.Id}";
                    // Create files
                    SAEONLogs.Verbose("Creating files");
                    System.IO.File.WriteAllText(Path.Combine(dirInfo.FullName, "Input.json"), JsonConvert.SerializeObject(input, Formatting.Indented));
                    System.IO.File.WriteAllText(Path.Combine(dirInfo.FullName, "Metadata.json"), metadata.ToJson());
                    switch (input.DownloadFormat)
                    {
                        case DownloadFormat.CSV:
                            System.IO.File.WriteAllText(Path.Combine(dirInfo.FullName, "Data.csv"), output.DataMatrix.AsCSV());
                            break;
                        case DownloadFormat.Excel:
                            break;
                        case DownloadFormat.NetCDF:
                            break;
                    }
                    // Create Zip
                    System.IO.File.WriteAllText(Path.Combine(dirInfo.FullName, "Download.json"),
                        JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
                    ZipFile.CreateFromDirectory(dirInfo.FullName, Path.Combine(folder, $"{result.Id}.zip"));
                    dirInfo.Delete(true);
                    // Generate Checksum
                    result.ZipCheckSum = GetChecksum(result.ZipFullName);
                    var jChecksum = new JObject(new JProperty("Checksum", result.ZipCheckSum));
                    System.IO.File.WriteAllText(Path.Combine(folder, $"{result.Id} Checksum.json"), jChecksum.ToString());
                    SAEONLogs.Verbose("UserDownload: {@UserDownload}", result);
                    await SaveChangesAsync();
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
                    if (input == null)
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
                    if (userDownload == null)
                    {
                        throw new ArgumentException($"UserDownload with Id: {id} not found!");
                        //return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"UserDownload with Id: {id} not found!");
                    }
                    return File(userDownload.ZipFullName, MediaTypeNames.Application.Zip, Path.GetFileName(userDownload.ZipFullName));
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