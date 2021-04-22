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
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Data;
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
        public DataWizardController()
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
            public Guid StationId { get; set; }
            public Guid PhenomenonOfferingId { get; set; }
            public Guid PhenomenonUnitId { get; set; }
            public string Name { get; set; }
            public string Caption { get; set; }

            public override bool Equals(object obj)
            {
                return obj is DataFeature feature &&
                       StationId.Equals(feature.StationId) &&
                       PhenomenonOfferingId.Equals(feature.PhenomenonOfferingId) &&
                       PhenomenonUnitId.Equals(feature.PhenomenonUnitId);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(StationId, PhenomenonOfferingId, PhenomenonUnitId);
            }
        }

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

            string GetVariableName(string phenomenonName, string offeringName, string unitName)
            {
                return $"{phenomenonName.Replace(", ", "_")}, {offeringName.Replace(", ", "_")}, {unitName.Replace(", ", "_")}";
            }

            var stopwatch = new Stopwatch();
            var stageStopwatch = new Stopwatch();
            stopwatch.Start();
            stageStopwatch.Start();
            var result = new DataWizardDataOutput();
            var summaries = GetSummary(ref input);
            SAEONLogs.Verbose("Summaries: Stage {Stage} Total {Total}", stageStopwatch.Elapsed.TimeStr(), stopwatch.Elapsed.TimeStr());
            stageStopwatch.Restart();
            var features = summaries.Select(i => new DataFeature
            {
                StationId = i.StationId,
                PhenomenonOfferingId = i.PhenomenonOfferingId,
                PhenomenonUnitId = i.PhenomenonUnitId,
                Name = GetCode(i.StationCode, i.PhenomenonCode, i.OfferingCode, i.UnitCode),
                Caption = GetName(i.StationName, i.PhenomenonName, i.OfferingName, i.UnitName)
            }).Distinct().ToList();
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
                .ThenBy(obs => obs.Elevation)
                .ThenBy(obs => obs.PhenomenonName)
                .ThenBy(obs => obs.OfferingName)
                .ThenBy(obs => obs.UnitName)
                .ThenBy(obs => obs.ValueDate)
                .AsEnumerable() // Force fetch from database
                .Where(ibs =>
                    (!input.Locations.Any() || input.Locations.Contains(new Location { StationId = ibs.StationId })) &&
                    (!input.Variables.Any() || input.Variables.Contains(new Variable { PhenomenonId = ibs.PhenomenonId, OfferingId = ibs.OfferingId, UnitId = ibs.UnitId })))
                .ToList();
            SAEONLogs.Verbose("Observations: {Observations} Stage {Stage} Total {Total}", observations.Count, stageStopwatch.Elapsed.TimeStr(), stopwatch.Elapsed.TimeStr());
            stageStopwatch.Restart();
            var siteId = new Guid();
            var stationId = new Guid();
            double? elevation = null;
            var phenomenonId = new Guid();
            var offeringId = new Guid();
            var unitId = new Guid();
            var date = DateTime.MinValue;
            DataMatrixRow row = null;
            // Data Matrix
            result.DataMatrix.AddColumn("Site", MaxtixDataType.mdtString);
            result.DataMatrix.AddColumn("Station", MaxtixDataType.mdtString);
            result.DataMatrix.AddColumn("Elevation", MaxtixDataType.mdtDouble);
            result.DataMatrix.AddColumn("Phenomenon", MaxtixDataType.mdtString);
            result.DataMatrix.AddColumn("Offering", MaxtixDataType.mdtString);
            result.DataMatrix.AddColumn("Unit", MaxtixDataType.mdtString);
            result.DataMatrix.AddColumn("Variable", MaxtixDataType.mdtString);
            result.DataMatrix.AddColumn("Date", MaxtixDataType.mdtDate);
            result.DataMatrix.AddColumn("Value", MaxtixDataType.mdtDouble);
            result.DataMatrix.AddColumn("Instrument", MaxtixDataType.mdtString);
            result.DataMatrix.AddColumn("Sensor", MaxtixDataType.mdtString);
            int nRow = 0;
            foreach (var obs in observations)
            {
                if (++nRow % 10000 == 0)
                {
                    SAEONLogs.Verbose("DataMatrix.Row: {Row}", nRow);
                }
                if ((row == null) || (obs.SiteId != siteId) || (obs.StationId != stationId) || (obs.Elevation != elevation) || (obs.PhenomenonId != phenomenonId) || (obs.OfferingId != offeringId) || (obs.UnitId != unitId) || (obs.ValueDate != date))
                {
                    row = result.DataMatrix.AddRow();
                    row["Site"] = obs.SiteName;
                    row["Station"] = obs.StationName;
                    //var siteName = MetadataHelper.CleanPrefixes(obs.SiteName);
                    //var stationName = MetadataHelper.CleanPrefixes(obs.StationName);
                    //if (obs.StationName.StartsWith("ELW, "))
                    //{
                    //    if (stationName.EndsWith(siteName))
                    //    {
                    //        stationName = stationName.Substring(0, stationName.Length - siteName.Length - 2);
                    //    }
                    //}
                    //row["Site"] = siteName;
                    //row["Station"] = stationName;
                    row["Phenomenon"] = obs.PhenomenonName;
                    row["Offering"] = obs.OfferingName;
                    row["Unit"] = obs.UnitName;
                    row["Variable"] = GetVariableName(obs.PhenomenonName, obs.OfferingName, obs.UnitName);
                    row["Date"] = obs.ValueDate;
                    row["Elevation"] = obs.Elevation;
                    row["Instrument"] = obs.InstrumentName;
                    row["Sensor"] = obs.SensorName;
                    siteId = obs.SiteId;
                    stationId = obs.StationId;
                    phenomenonId = obs.PhenomenonId;
                    offeringId = obs.OfferingId;
                    unitId = obs.UnitId;
                    date = obs.ValueDate;
                    elevation = obs.Elevation;
                }
                row["Value"] = obs.DataValue;
            }
            if (SAEONLogs.Level == LogEventLevel.Verbose && Config["SaveSearches"].IsTrue())
            {
                var folder = $"{HostEnvironment.ContentRootPath.AddTrailingForwardSlash()}Searches/{result.Date:yyyyMM}";
                Directory.CreateDirectory(folder);
                System.IO.File.WriteAllText(Path.Combine(folder, $"{result.Date:yyyyMMdd HHmmss}.csv"), result.DataMatrix.ToCSV());
            }
            SAEONLogs.Verbose("DataMatrix: Rows: {Rows} Cols: {Cols} Stage {Stage} Total {Total}", result.DataMatrix.Rows.Count, result.DataMatrix.Columns.Count, stageStopwatch.Elapsed.TimeStr(), stopwatch.Elapsed.TimeStr());
            stageStopwatch.Restart();
            // Metadata
            result.Metadata.PublicationDate = result.Date;
            var titles = observations.Where(i => i.StationName.StartsWith("ELW, ")).Select(i =>
            {
                var siteName = MetadataHelper.CleanPrefixes(i.SiteName);
                var stationName = MetadataHelper.CleanPrefixes(i.StationName);
                if (stationName.EndsWith(siteName))
                {
                    stationName = stationName.Substring(0, stationName.Length - siteName.Length - 2);
                }
                return $"{stationName} of {i.PhenomenonName}";
            }).Union(observations.Where(i => !i.StationName.StartsWith("ELW, ")).Select(i =>
            {
                var siteName = MetadataHelper.CleanPrefixes(i.SiteName);
                var stationName = MetadataHelper.CleanPrefixes(i.StationName);
                return $"{siteName}, {stationName} of {i.PhenomenonName}";
            })).Distinct();
            result.Metadata.Title = "Observations in the SAEON Observations Database for " +
                string.Join("; ", titles.OrderBy(i => i));
            var descriptions = observations.Where(i => i.StationName.StartsWith("ELW, ")).Select(i =>
            {
                var siteName = MetadataHelper.CleanPrefixes(i.SiteName);
                var stationName = MetadataHelper.CleanPrefixes(i.StationName);
                if (stationName.EndsWith(siteName))
                {
                    stationName = stationName.Substring(0, stationName.Length - siteName.Length - 2);
                }
                return $"{stationName} of {i.PhenomenonName}, {i.OfferingName}, {i.UnitName}";
            }).Union(observations.Where(i => !i.StationName.StartsWith("ELW, ")).Select(i =>
            {
                var siteName = MetadataHelper.CleanPrefixes(i.SiteName);
                var stationName = MetadataHelper.CleanPrefixes(i.StationName);
                return $"{siteName}, {stationName} of {i.PhenomenonName}, {i.OfferingName}, {i.UnitName}";
            })).Distinct();
            result.Metadata.Description = "Observations in the SAEON Observations Database for " +
                string.Join("; ", descriptions.OrderBy(i => i));
            // Keywords
            //result.Metadata.Subjects.AddRange(observations.Select(i => i.OrganisationName).Distinct().Select(i => new MetadataSubject { Name = MetadataHelper.CleanPrefixes(i) }));
            //result.Metadata.Subjects.AddRange(observations.Select(i => i.ProgrammeName).Distinct().Select(i => new MetadataSubject { Name = MetadataHelper.CleanPrefixes(i) }));
            //result.Metadata.Subjects.AddRange(observations.Select(i => i.ProjectName).Distinct().Select(i => new MetadataSubject { Name = MetadataHelper.CleanPrefixes(i) }));
            result.Metadata.Subjects.AddRange(observations.Select(i => i.SiteName).Distinct().Select(i => new MetadataSubject { Name = MetadataHelper.CleanPrefixes(i) }));
            result.Metadata.Subjects.AddRange(observations.Select(i => i.StationName).Distinct().Select(i => new MetadataSubject { Name = MetadataHelper.CleanPrefixes(i) }));
            result.Metadata.Subjects.AddRange(observations.Select(i => i.PhenomenonName).Distinct().Select(i => new MetadataSubject { Name = i }));
            result.Metadata.Subjects.AddRange(observations.Select(i => $"{i.PhenomenonName}, {i.OfferingName}, {i.UnitName}").Distinct().Select(i => new MetadataSubject { Name = i }));
            // Dates
            result.Metadata.StartDate = observations.Min(i => i.ValueDate);
            result.Metadata.EndDate = observations.Max(i => i.ValueDate);
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
                nRow = 0;
                foreach (var obs in observations.OrderBy(i => i.SiteName).ThenBy(i => i.StationName).ThenBy(i => i.ValueDate))
                {
                    if (++nRow % 10000 == 0)
                    {
                        SAEONLogs.Verbose("Chart.Row: {Row}", nRow);
                    }
                    if ((series == null) || (obs.SiteId != siteId) || (obs.StationId != stationId))
                    {
                        series = new ChartSeries
                        {
                            Name = GetCode(obs.StationCode, obs.PhenomenonCode, obs.OfferingCode, obs.UnitCode),
                            Caption = GetName(obs.StationName, obs.PhenomenonName, obs.OfferingName, obs.UnitName)
                        };
                        result.ChartSeries.Add(series);
                        siteId = obs.SiteId;
                        stationId = obs.StationId;
                    }
                    series.Add(obs.ValueDate, obs.DataValue);
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
                        var doiCode = $"Data download for {User.UserId()} on {output.Date:yyyy-MM-dd HH:mm:ss.fff}";
                        // Get a DOI
                        SAEONLogs.Verbose("Minting DOI");
                        var doi = await DOIHelper.CreateAdHocDOI(DbContext, HttpContext, doiCode, doiCode);
                        SAEONLogs.Verbose("DOI: {@DOI}", doi);
                        var metadata = await MetadataHelper.CreateAddHocMetadata(DbContext, doi, output.Metadata);
                        metadata.Accessed = output.Date;
                        metadata.Generate(metadata.Title, metadata.Description);
                        doi.MetadataJson = metadata.ToJson();
                        var oldSha256 = doi.MetadataJsonSha256;
                        doi.MetadataJsonSha256 = doi.MetadataJson.Sha256();
                        doi.ODPMetadataNeedsUpdate = oldSha256 != doi.MetadataJsonSha256 || (!doi.ODPMetadataIsValid ?? true); ;
                        doi.Title = metadata.Title;
                        doi.MetadataHtml = metadata.ToHtml();
                        doi.CitationHtml = metadata.CitationHtml;
                        doi.CitationText = metadata.CitationText;
                        SAEONLogs.Verbose("Metadata: {@Metadata}", metadata);
                        SAEONLogs.Verbose("Adding UserDownload");
                        var baseUrl = $"{Config["QuerySiteUrl"].AddTrailingForwardSlash()}Query/Data";
                        var result = new UserDownload
                        {
                            UserId = User.UserId(),
                            Name = doiCode,
                            Description = metadata.Description,
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
                        //result.Description += Environment.NewLine + "Please cite as follows:" + Environment.NewLine + metadata.CitationText;
                        result.RequeryUrl = $"{baseUrl}/Requery/{result.Id}";
                        result.DownloadUrl = $"{baseUrl}/ViewDownload/{result.Id}";
                        var folder = $"{HostEnvironment.ContentRootPath.AddTrailingBackSlash()}Downloads\\{output.Date:yyyyMM}";
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
                                System.IO.File.WriteAllText(Path.Combine(dirInfo.FullName, "Data.csv"), output.DataMatrix.ToCSV());
                                break;
                            case DownloadFormat.Excel:
                                System.IO.File.WriteAllBytes(Path.Combine(dirInfo.FullName, "Data.xlsx"), output.DataMatrix.ToExcel());
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