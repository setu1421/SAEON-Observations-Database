using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SAEON.Core;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Hosting;
using System.Web.Http;
using System.Xml.Linq;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    [RoutePrefix("Internal/DataWizard")]
    public class DataWizardController : BaseController
    {
        private IQueryable<ImportBatchSummary> GetQuery(DataWizardDataInput input)
        {
            Logging.Verbose("Input: {@Input}", input);
            foreach (var orgId in input.Organisations)
            {
                input.Sites.AddRange(DbContext.Organisations.Where(i => i.Id == orgId).SelectMany(i => i.Sites).Select(i => i.Id));
                input.Stations.AddRange(DbContext.Organisations.Where(i => i.Id == orgId).SelectMany(i => i.Stations).Select(i => i.Id));
            }
            foreach (var phenomenonId in input.Phenomena)
            {
                input.Offerings.AddRange(DbContext.PhenomenonOfferings.Where(i => i.PhenomenonId == phenomenonId).Select(i => i.Id));
                input.Units.AddRange(DbContext.PhenomenonUnits.Where(i => i.PhenomenonId == phenomenonId).Select(i => i.Id));
            }
            input.StartDate = input.StartDate.Date;
            input.EndDate = input.EndDate.Date.AddDays(1);
            Logging.Verbose("Processed Input: {@Input}", input);
            var startDate = input.StartDate;
            var endDate = input.EndDate;
            return DbContext.ImportBatchSummary.Where(i =>
                (input.Sites.Contains(i.SiteId) || input.Stations.Contains(i.StationId)) &&
                ((!input.Offerings.Any() || input.Offerings.Contains(i.PhenomenonOfferingId)) && (!input.Units.Any() || input.Units.Contains(i.PhenomenonUnitId))) &&
                (i.StartDate >= startDate && i.EndDate < endDate));
        }

        private DataWizardApproximation CalculateApproximation(DataWizardDataInput input)
        {
            using (Logging.MethodCall<DataWizardApproximation>(GetType()))
            {
                try
                {
                    var q = GetQuery(input);
                    var dateRows = q.ToList().Sum(i => i.Count);
                    var result = new DataWizardApproximation
                    {
                        RowCount = dateRows
                    };
                    if (!(input.Organisations.Any() || input.Sites.Any() || input.Stations.Any()))
                    {
                        result.Errors.Add("Please select at least one Organisation, Site or Station");
                    }
                    if (!(input.Phenomena.Any() || input.Offerings.Any() || input.Units.Any()))
                    {
                        result.Errors.Add("Please select at least one Phenomenon, Offering or Unit");
                    }
                    Logging.Verbose("Result: {@Result}", result);
                    return result;
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("Approximation")]
        public DataWizardApproximation ApproximationGet([FromUri] string input)
        {
            using (Logging.MethodCall<DataWizardApproximation>(GetType()))
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
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        [HttpPost]
        [Route("Approximation")]
        public DataWizardApproximation ApproximationPost([FromBody] DataWizardDataInput input)
        {
            using (Logging.MethodCall<DataWizardApproximation>(GetType()))
            {
                try
                {
                    if (input == null)
                    {
                        throw new ArgumentNullException(nameof(input));
                    }
                    return CalculateApproximation(input);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        private class DataFeature
        {
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
                if (obj == null)
                {
                    return false;
                }

                if (!(obj is DataFeature feature))
                {
                    return false;
                }

                return Equals(feature);
            }

            public bool Equals(DataFeature feature)
            {
                if (feature == null)
                {
                    return false;
                }

                return
                    (Name == feature.Name);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    const int HashingBase = (int)2166136261;
                    const int HashingMultiplier = 16777619;
                    int hash = HashingBase;
                    hash = (hash * HashingMultiplier) ^ (Name?.GetHashCode() ?? 0);
                    return hash;
                }
            }
        }

        private DataWizardDataOutput GetData(DataWizardDataInput input, bool includeChart)
        {
            var result = new DataWizardDataOutput();
            result.DataMatrix.AddColumn("SiteName", "Site", MaxtixDataType.String);
            result.DataMatrix.AddColumn("StationName", "Station", MaxtixDataType.String);
            result.DataMatrix.AddColumn("InstrumentName", "Instrument", MaxtixDataType.String);
            result.DataMatrix.AddColumn("SensorName", "Sensor", MaxtixDataType.String);
            result.DataMatrix.AddColumn("Date", "Date", MaxtixDataType.Date);
            result.DataMatrix.AddColumn("Elevation", "Elevation", MaxtixDataType.Double);

            var q = GetQuery(input);
            var qFeatures = q.Select(i => new { i.PhenomenonOfferingId, i.PhenomenonUnitId, i.PhenomenonCode, i.PhenomenonName, i.OfferingCode, i.OfferingName, i.UnitCode, i.UnitName, i.UnitSymbol }).Distinct();
            var features = qFeatures.ToList().Select(i => new DataFeature
            {
                PhenomenonOfferingId = i.PhenomenonOfferingId,
                PhenomenonUnitId = i.PhenomenonUnitId,
                Phenomenon = i.PhenomenonName,
                Offering = i.OfferingName,
                Unit = i.UnitName,
                Symbol = i.UnitSymbol,
                Name = $"{i.PhenomenonCode.Replace(" ", "")}_{i.OfferingCode.Replace(" ", "")}_{i.UnitCode.Replace(" ", "")}",
                Caption = $"{i.PhenomenonName}, {i.OfferingName}, {i.UnitSymbol}"
            });
            foreach (var feature in features)
            {
                result.DataMatrix.AddColumn(feature.Name, feature.Caption, MaxtixDataType.Double);
                Logging.Verbose("Feature: {@Feature}", feature);
            }
            var phenomenonOfferingIds = features.Select(f => f.PhenomenonOfferingId);
            var phenomenonUnitIds = features.Select(f => f.PhenomenonUnitId);
            //var observations = q.Join(db.Observations.Where(i => (i.StatusName == "Verified")), l => l.ImportBatchId, r => r.ImportBatchId, (l, r) => r)
            var observations = q.Join(DbContext.Observations.Where(i => (i.StatusId == null) || (i.StatusName == "Verified")), l => l.ImportBatchId, r => r.ImportBatchId, (l, r) => r)
                    .Where(i => phenomenonOfferingIds.Contains(i.PhenomenonOfferingId))
                    .Where(i => phenomenonUnitIds.Contains(i.PhenomenonUnitId))
                    .OrderBy(i => i.SiteName)
                    .ThenBy(i => i.StationName)
                    .ThenBy(i => i.InstrumentName)
                    .ThenBy(i => i.SensorName)
                    .ThenBy(i => i.ValueDate)
                    .ThenBy(i => i.Elevation)
                    //.Take(1000)
                    .ToList();
            Logging.Verbose("Observations: {Observations}", observations.Count);
            var siteId = new Guid();
            var stationId = new Guid();
            var instrumentId = new Guid();
            var sensorId = new Guid();
            var date = DateTime.MinValue;
            double? elevation = null;
            DataMatrixRow row = null;
            var keywordsShort = new List<string>();
            var keywordsLong = new List<string>();
            var keywordsSiteShort = new List<string>();
            var keywordsSiteLong = new List<string>();
            var places = new List<string>();
            // Data Matrix
            int nRow = 0;
            foreach (var obs in observations)
            {
                if (++nRow % 10000 == 0)
                {
                    Logging.Verbose("DataMatrix.Row: {Row}", nRow);
                }
                if (obs.SiteId != siteId)
                {
                    if (string.IsNullOrEmpty(result.Title))
                    {
                        result.Title = "Observations collected from";
                        result.Description = "Observations collected from";
                    }
                    else
                    {
                        keywordsSiteShort = keywordsSiteShort.Distinct().ToList();
                        keywordsSiteShort.Sort();
                        keywordsSiteLong = keywordsSiteLong.Distinct().ToList();
                        keywordsSiteLong.Sort();
                        result.Title += " " + string.Join(", ", keywordsSiteShort) + ";";
                        result.Description += " " + string.Join(", ", keywordsSiteLong) + ";";
                    }
                    result.Title += " " + obs.SiteName + " -";
                    result.Description += " " + obs.SiteName + " -";
                    keywordsSiteShort.Clear();
                    keywordsSiteLong.Clear();
                }
                if (obs.StationId != stationId)
                {
                    var station = DbContext.Stations.First(i => i.Id == obs.StationId);
                    if (station.Latitude.HasValue && station.Longitude.HasValue)
                    {
                        places.Add($"{station.Name}:South Africa:{station.Latitude}:{station.Longitude}");
                    }
                }
                if ((obs.StationId != stationId) || (obs.InstrumentId != instrumentId) || (obs.SensorId != sensorId))
                {
                    //result.Title += $", {obs.PhenomenonName}";
                    //result.Description += $", {obs.PhenomenonName} {obs.OfferingName} {obs.UnitName}";
                    keywordsShort.Add(obs.PhenomenonName);
                    keywordsLong.Add($"{obs.PhenomenonName} {obs.OfferingName} {obs.UnitName}");
                    keywordsSiteShort.Add(obs.PhenomenonName);
                    keywordsSiteLong.Add($"{obs.PhenomenonName} {obs.OfferingName} {obs.UnitName}");
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
                var name = $"{obs.PhenomenonCode.Replace(" ", "")}_{obs.OfferingCode.Replace(" ", "")}_{obs.UnitCode.Replace(" ", "")}";
                //Logging.Verbose("Name: {Name}",name);
                row[name] = obs.DataValue;
                if (obs.ValueDate < (result.StartDate ?? DateTime.MaxValue))
                {
                    result.StartDate = obs.ValueDate;
                }

                if (obs.ValueDate > (result.EndDate ?? DateTime.MinValue))
                {
                    result.EndDate = obs.ValueDate;
                }

                if (obs.Latitude.HasValue && (obs.Latitude.Value > (result.LatitudeNorth ?? double.MinValue)))
                {
                    result.LatitudeNorth = obs.Latitude;
                }

                if (obs.Latitude.HasValue && (obs.Latitude.Value < (result.LatitudeSouth ?? double.MaxValue)))
                {
                    result.LatitudeSouth = obs.Latitude;
                }

                if (obs.Longitude.HasValue && (obs.Longitude.Value < (result.LongitudeWest ?? double.MaxValue)))
                {
                    result.LongitudeWest = obs.Longitude;
                }

                if (obs.Longitude.HasValue && (obs.Longitude.Value > (result.LongitudeEast ?? double.MinValue)))
                {
                    result.LongitudeEast = obs.Longitude;
                }

                if (obs.Elevation.HasValue && (obs.Elevation.Value < (result.ElevationMinimum ?? double.MaxValue)))
                {
                    result.ElevationMinimum = obs.Elevation;
                }

                if (obs.Elevation.HasValue && (obs.Elevation.Value > (result.ElevationMaximum ?? double.MinValue)))
                {
                    result.ElevationMaximum = obs.Elevation;
                }
            }
            Logging.Verbose("DataMatrix: Rows: {Rows} Cols: {Cols}", result.DataMatrix.Rows.Count, result.DataMatrix.Columns.Count);
            if (!string.IsNullOrEmpty(result.Title))
            {
                keywordsSiteShort = keywordsSiteShort.Distinct().ToList();
                keywordsSiteShort.Sort();
                keywordsSiteLong = keywordsSiteLong.Distinct().ToList();
                keywordsSiteLong.Sort();
                result.Title += " " + string.Join(", ", keywordsSiteShort) + ";";
                result.Description += " " + string.Join(", ", keywordsSiteLong) + ";";
            }
            result.Title = result.Title.TrimEnd(";");
            result.Description = result.Description.TrimEnd(";");
            if (result.LatitudeNorth.HasValue && result.LatitudeSouth.HasValue && result.LongitudeWest.HasValue && result.LongitudeEast.HasValue)
            {
                result.Description += $" in area {result.LatitudeNorth:f5},{result.LongitudeWest:f5} N,W {result.LatitudeSouth:f5},{result.LongitudeEast:f5} S,E";
            }
            if (result.ElevationMinimum.HasValue && result.ElevationMaximum.HasValue)
            {
                if (result.ElevationMinimum.Value == result.ElevationMaximum.Value)
                {
                    result.Description += $" at {result.ElevationMaximum:f2}m above mean sea level";
                }
                else
                {
                    result.Description += $" between {result.ElevationMinimum:f2}m and {result.ElevationMaximum:f2}m above mean sea level";
                }
            }
            if (result.StartDate.HasValue)
            {
                result.Title += $" from {result.StartDate.Value.ToString("yyyy-MM-dd")}";
                result.Description += $" from {result.StartDate.Value.ToString("yyyy-MM-dd HH:mm:ss")}";
            }
            if (result.EndDate.HasValue)
            {
                result.Title += $" to {result.EndDate.Value.ToString("yyyy-MM-dd")}";
                result.Description += $" to {result.EndDate.Value.ToString("yyyy-MM-dd HH:mm:ss")}";
            }
            result.Keywords.AddRange(keywordsShort.Distinct());
            result.Keywords.AddRange(keywordsLong.Distinct());
            result.Keywords.Sort();
            result.Places.AddRange(places.Distinct());
            result.Places.Sort();
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
                        Logging.Verbose("Chart.Row: {Row}", nRow);
                    }
                    if ((series == null) || (obs.SiteId != siteId) || (obs.StationId != stationId) || (obs.InstrumentId != instrumentId) || (obs.SensorId != sensorId))
                    {
                        series = new ChartSeries
                        {
                            Name = $"{obs.SensorCode.Replace(" ", "")}_{obs.PhenomenonCode.Replace(" ", "")}_{obs.OfferingCode.Replace(" ", "")}_{obs.UnitCode.Replace(" ", "")}",
                            Caption = $"{obs.SensorName}, {obs.PhenomenonName}, {obs.OfferingName}, {obs.UnitSymbol}"
                        };
                        result.ChartSeries.Add(series);
                        siteId = obs.SiteId;
                        stationId = obs.StationId;
                        instrumentId = obs.InstrumentId;
                        sensorId = obs.SensorId;
                    }
                    series.Add(obs.ValueDate, obs.DataValue);
                }
                Logging.Verbose("ChartSeries: Count: {count}", result.ChartSeries.Count);
            }
            return result;
        }

        [HttpGet]
        [Route("GetData")]
        [Authorize]
        public DataWizardDataOutput DataGet([FromUri] string input)
        {
            using (Logging.MethodCall<DataWizardDataOutput>(GetType()))
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
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        [HttpPost]
        [Route("GetData")]
        [Authorize]
        public DataWizardDataOutput DataPost([FromBody] DataWizardDataInput input)
        {
            using (Logging.MethodCall<DataWizardDataOutput>(GetType()))
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
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        private UserDownload GetDownload(DataWizardDownloadInput input)
        {
            string GetChecksum(string file)
            {
                using (FileStream stream = File.OpenRead(file))
                {
                    var sha = new SHA256Managed();
                    byte[] checksum = sha.ComputeHash(stream);
                    return BitConverter.ToString(checksum).Replace("-", String.Empty);
                }
            }

            List<string> GetValidationErrors(DbEntityValidationException ex)
            {
                return ex.EntityValidationErrors.SelectMany(e => e.ValidationErrors.Select(m => m.PropertyName + ": " + m.ErrorMessage)).ToList();
            }

            void SaveChanges()
            {
                try
                {
                    DbContext.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    Logging.Exception(ex, string.Join("; ", GetValidationErrors(ex)));
                    throw;
                }
            }

            string JohansAPIJson(UserDownload result)
            {
                var jSubjects =
                    new JArray(
                        new JObject(
                            new JProperty("subject", "Observations")
                        ),
                        new JObject(
                            new JProperty("subject", "South African Environmental Observation Network (SAEON)")
                        ),
                        new JObject(
                            new JProperty("subjectScheme", "SOFTWARE_APP"),
                            new JProperty("schemeURI", "http://www.saeon.ac.za/"),
                            new JProperty("subject", "Observations Database")
                        ),
                        new JObject(
                            new JProperty("subjectScheme", "SOFTWARE_URL"),
                            new JProperty("schemeURI", "http://www.saeon.ac.za/"),
                            new JProperty("subject", Properties.Settings.Default.QuerySiteUrl)
                        )
                    );
                var keywords = result.Keywords.Split(new string[] { "; " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var subject in keywords)
                {
                    jSubjects.Add(
                        new JObject(
                            new JProperty("subject", subject)
                        )
                    );
                }
                var places = result.Places.Split(new string[] { "; " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var place in places)
                {
                    jSubjects.Add(
                        new JObject(
                            new JProperty("subjectScheme", "name"),
                            new JProperty("schemeURI", "http://www.geonames.org/"),
                            new JProperty("subject", place)
                        )
                    );
                }

                var jGeoLocations = new JArray();
                foreach (var place in places)
                {
                    var splits = place.Split(new char[] { ':' });
                    jGeoLocations.Add(
                        new JObject(
                            new JProperty("geoLocationPlace", $"{splits[0]}, {splits[1]}")
                        )
                    );
                    jGeoLocations.Add(
                        new JObject(
                            new JProperty("geoLocationPoint", $"{splits[3]} {splits[2]}")
                        )
                    );
                }
                jGeoLocations.Add(
                    new JObject(
                        new JProperty("geoLocationBox", $"{result.LongitudeWest:f5} {result.LatitudeSouth:f5} {result.LongitudeEast:f5} {result.LatitudeNorth:f5}")
                    )
                );

                var jMetadata =
                    new JObject(
                        new JProperty("json",
                            new JObject(
                                new JProperty("dataSchema", "DataCite"),
                                new JProperty("language", "eng"),
                                new JProperty("identifier",
                                    new JObject(
                                        new JProperty("identifierType", "DOI"),
                                        new JProperty("identifier", result.DigitalObjectIdentifier.DOI)
                                    )
                                ),
                                new JProperty("resourceTypeGeneral", "Dataset"),
                                new JProperty("resourceType", "Tabular Data in Text File(s)"),
                                new JProperty("publisher", "South African Environmental Observation Network (SAEON)"),
                                new JProperty("publicationYear", $"{result.Date.Year}"),
                                new JProperty("dates",
                                    new JArray(
                                        new JObject(
                                            new JProperty("date", result.Date.ToString("yyyy-MM-dd")),
                                            new JProperty("dateType", "accepted")
                                        ),
                                        new JObject(
                                            new JProperty("date", result.Date.ToString("yyyy-MM-dd")),
                                            new JProperty("dateType", "issued")
                                        ),
                                        new JObject(
                                            new JProperty("date", result.StartDate.ToString("yyyy-MM-dd")),
                                            new JProperty("dateType", "collected")
                                        ),
                                        new JObject(
                                            new JProperty("date", result.EndDate.ToString("yyyy-MM-dd")),
                                            new JProperty("dateType", "collected")
                                        )
                                    )
                                ),
                                new JProperty("rights",
                                    new JArray(
                                        new JObject(
                                            new JProperty("rights", "Attribution-ShareAlike 4.0 International (CC BY-SA 4.0)"),
                                            new JProperty("rightsURI", "https://creativecommons.org/licenses/by-sa/4.0/")
                                        )
                                    )
                                ),
                                new JProperty("creators",
                                    new JArray(
                                        new JObject(
                                            new JProperty("creatorName", "South African Environmental Observation Network (SAEON)"),
                                            new JProperty("nameType", "Organizational")
                                        ),
                                        new JObject(
                                            new JProperty("creatorName", "Observations Database Administrator"),
                                            new JProperty("affiliation", "Organisation:South African Environmental Observation Network (SAEON);e-Mail Address:timpn@saeon.ac.za"),
                                            new JProperty("nameType", "Personal"),
                                            new JProperty("givenName", "Tim"),
                                            new JProperty("familyName", "Parker-Nance"),
                                            new JProperty("nameIdentifier", "0000-0001-7040-7736"),
                                            new JProperty("nameIdentifierScheme", "ORCID")
                                        )
                                    )
                                ),
                                new JProperty("titles",
                                    new JArray(
                                        new JObject(
                                            //new JProperty("titleType", ""),
                                            new JProperty("title", result.Title)
                                        )
                                    )
                                ),
                                new JProperty("description",
                                    new JArray(
                                        new JObject(
                                            new JProperty("descriptionType", "Abstract"),
                                            new JProperty("description", result.Description)
                                        )
                                    )
                                ),
                                new JProperty("contributors",
                                    new JArray(
                                        new JObject(
                                            new JProperty("contributorType", "DataManager"),
                                            new JProperty("contributorName", "SAEON uLwazi Node"),
                                            new JProperty("affiliation", "Organisation:South African Environmental Observation Network (SAEON);e-Mail Address:wim@saeon.ac.za")
                                        ),
                                        new JObject(
                                            new JProperty("contributorType", "DataCurator"),
                                            new JProperty("contributorName", "SAEON uLwazi Node"),
                                            new JProperty("affiliation", "Organisation:South African Environmental Observation Network (SAEON);e-Mail Address:wim@saeon.ac.za")
                                        )
                                    )
                                ),
                                new JProperty("alternateIdentifiers",
                                    new JArray(
                                        new JObject(
                                            new JProperty("alternateIdentifierType", "Internal"),
                                            new JProperty("alternateIdentifier", result.Id)
                                        )
                                    )
                                ),
                                new JProperty("subjects", jSubjects),
                                new JProperty("additionalFields",
                                    new JObject(
                                       new JProperty("onlineResources",
                                            new JArray(
                                                new JObject(
                                                    new JProperty("func", "download"),
                                                    new JProperty("desc", $"Observations Database Data Download {result.Id}"),
                                                    new JProperty("href", result.DownloadUrl)
                                                ),
                                                new JObject(
                                                    new JProperty("func", "download"),
                                                    new JProperty("desc", $"Observations Database Data Download {result.Id} as Zip"),
                                                    new JProperty("href", result.ZipUrl),
                                                    new JProperty("format", "zip")
                                                )
                                            )
                                        ),
                                        new JProperty("coverageBegin", result.StartDate.ToString("yyyy-MM-dd")),
                                        new JProperty("coverageEnd", result.EndDate.ToString("yyyy-MM-dd"))
                                    )
                                ),
                                new JProperty("geoLocations", jGeoLocations),
                                new JProperty("bounds",
                                    new JArray(
                                        new JValue($"{result.LatitudeSouth:f5}"), new JValue($"{result.LongitudeWest:f5}"),
                                        new JValue($"{result.LatitudeNorth:f5}"), new JValue($"{result.LongitudeEast:f5}")
                                    )
                                )
                            )
                        ),
                        new JProperty("schema", "datacite"),
                        new JProperty("mode", "manual"),
                        new JProperty("repository",
                            new JObject(
                                new JProperty("URL", "http://test.sasdi.net/"),
                                new JProperty("username", "testfour"),
                                new JProperty("password", "testfour"),
                                new JProperty("institution", "south-african-environmental-observation-network")
                            )
                        ),
                        new JProperty("target", "http://odp.org.za/test_sodp.aspx")
                    );
                return jMetadata.ToString();
            }

            string DataCiteXml(UserDownload result)
            {
                XNamespace ns = "http://datacite.org/schema/kernel-4";
                XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";

                var subjects =
                    new XElement(ns + "subjects",
                        new XElement(ns + "subject", "Observations"),
                        new XElement(ns + "subject", "South African Environmental Observation Network (SAEON)"),
                        new XElement(ns + "subject", new XAttribute("subjectScheme", "SOFTWARE_APP"), new XAttribute("schemeURI", "http://www.saeon.ac.za/"), "Observations Database"),
                        new XElement(ns + "subject", new XAttribute("subjectScheme", "SOFTWARE_URL"), new XAttribute("schemeURI", "http://www.saeon.ac.za/"), Properties.Settings.Default.QuerySiteUrl)
                    );
                var keywords = result.Keywords.Split(new string[] { "; " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var subject in keywords)
                {
                    subjects.Add(new XElement(ns + "subject", subject));
                }
                var places = result.Places.Split(new string[] { "; " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var place in places)
                {
                    subjects.Add(new XElement(ns + "subject", new XAttribute("subjectScheme", "name"), new XAttribute("schemeURI", "http://www.geonames.org/"), place));
                }

                var geoLocations = new XElement(ns + "geoLocations");
                foreach (var place in places)
                {
                    var splits = place.Split(new char[] { ':' });
                    geoLocations.Add(
                        new XElement(ns + "geoLocation",
                            new XElement(ns + "geoLocationPlace", $"{splits[0]}, {splits[1]}"),
                            new XElement(ns + "geoLocationPoint",
                                new XElement(ns + "pointLatitude", splits[2]),
                                new XElement(ns + "pointLongitude", splits[3])
                            )
                        ));
                }
                geoLocations.Add(
                    new XElement(ns + "geoLocation",
                        new XElement(ns + "geoLocationBox",
                            new XElement(ns + "westBoundLongitude", result.LongitudeWest),
                            new XElement(ns + "eastBoundLongitude", result.LongitudeEast),
                            new XElement(ns + "nothBoundLatitude", result.LatitudeNorth),
                            new XElement(ns + "southBoundLatitude", result.LatitudeSouth)
                        )
                    )
                );

                var root = new XElement(ns + "resource",
                    new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                    new XAttribute(xsi + "schemaLocation", "http://datacite.org/schema/kernel-4 http://schema.datacite.org/meta/kernel-4.2/metadata.xsd"),
                    new XElement(ns + "identifier", new XAttribute("identifierType", "DOI"), result.DigitalObjectIdentifier.DOI),
                    new XElement(ns + "language", "en-UK"),
                    new XElement(ns + "resourceType", new XAttribute("resourceTypeGeneral", "Dataset"), "Tabular Data in Text File(s)"),
                    new XElement(ns + "alternateIdentifiers",
                        new XElement(ns + "alternateIdentifier", new XAttribute("alternateIdentifierType", "Internal"), result.Id)
                    ),
                    new XElement(ns + "publisher", "South African Environmental Observation Network (SAEON)"),
                    new XElement(ns + "publicationYear", result.Date.Year),
                    new XElement(ns + "dates",
                        new XElement(ns + "date", new XAttribute("dateType", "accepted"), result.Date.Year),
                        new XElement(ns + "date", new XAttribute("dateType", "issued"), result.Date.Year),
                        new XElement(ns + "date", new XAttribute("dateType", "collected"), result.StartDate.Year),
                        new XElement(ns + "date", new XAttribute("dateType", "collected"), result.EndDate.Year)
                    ),
                    new XElement(ns + "rightsList",
                        new XElement(ns + "rights",
                            new XAttribute("schemeURI", "https://spdx.org/licenses/"),
                            new XAttribute("rightsIdentifierScheme", "SPDX"),
                            new XAttribute("rightsIdentifier", "CC-BY-SA-4.0"),
                            new XAttribute("rightsURI", "https://creativecommons.org/licenses/by-sa/4.0/"),
                            "Attribution-ShareAlike 4.0 International (CC BY-SA 4.0)")
                    ),
                    new XElement(ns + "creators",
                        new XElement(ns + "creator",
                            new XElement(ns + "creatorName", new XAttribute("nameType", "Organizational"), "South African Environmental Observation Network (SAEON)")
                        ),
                        new XElement(ns + "creator",
                            new XElement(ns + "creatorName", new XAttribute("nameType", "Personal"), "Observations Database Administrator"),
                            new XElement(ns + "givenName", "Tim"),
                            new XElement(ns + "familyName", "Parker-Nance"),
                            new XElement(ns + "nameIdentifier", new XAttribute("schemeURI", "http://orcid.org/"), new XAttribute("nameIdentifierScheme", "ORCID"), "0000-0001-7040-7736"),
                            new XElement(ns + "affiliation", "Organisation:South African Environmental Observation Network (SAEON);e-Mail Address:timpn@saeon.ac.za")
                        )
                    ),
                    new XElement(ns + "titles",
                        new XElement(ns + "title", result.Title)
                    ),
                    new XElement(ns + "descriptions",
                        new XElement(ns + "description", new XAttribute("descriptionType", "Abstract"), result.Description)
                    ),
                    new XElement(ns + "contributors",
                        new XElement(ns + "contributor", new XAttribute("contributorType", "ContactPerson"),
                            new XElement(ns + "contributorName", "Parker-Nance, Tim"),
                            new XElement(ns + "givenName", "Tim"),
                            new XElement(ns + "familyName", "Parker-Nance"),
                            new XElement(ns + "nameIdentifier", new XAttribute("schemeURI", "http://orcid.org/"), new XAttribute("nameIdentifierScheme", "ORCID"), "0000-0001-7040-7736"),
                            new XElement(ns + "affiliation", "Organisation:South African Environmental Observation Network (SAEON);e-Mail Address:timpn@saeon.ac.za")
                        ),
                        new XElement(ns + "contributor", new XAttribute("contributorType", "DataManager"),
                            new XElement(ns + "contributorName", "SAEON uLwazi Node"),
                            new XElement(ns + "affiliation", "Organisation:South African Environmental Observation Network (SAEON);e-Mail Address:wim@saeon.ac.za")
                        ),
                        new XElement(ns + "contributor", new XAttribute("contributorType", "DataCurator"),
                            new XElement(ns + "contributorName", "SAEON uLwazi Node"),
                            new XElement(ns + "affiliation", "Organisation:South African Environmental Observation Network (SAEON);e-Mail Address:wim@saeon.ac.za")
                        )
                    ),
                    new XElement(ns + "subjects", subjects),
                    new XElement(ns + "formats",
                        new XElement(ns + "format", "application/zip"),
                        new XElement(ns + "format", "text/csv")
                    ),
                    new XElement(ns + "geoLocations", geoLocations)
                );
                return root.ToString();
            }

            string DataCiteJson(UserDownload result)
            {
                var jSubjects =
                    new JArray(
                        new JObject(
                            new JProperty("subject", "Observations")
                        ),
                        new JObject(
                            new JProperty("subject", "South African Environmental Observation Network (SAEON)")
                        ),
                        new JObject(
                            new JProperty("subjectScheme", "SOFTWARE_APP"),
                            new JProperty("schemeURI", "http://www.saeon.ac.za/"),
                            new JProperty("subject", "Observations Database")
                        ),
                        new JObject(
                            new JProperty("subjectScheme", "SOFTWARE_URL"),
                            new JProperty("schemeURI", "http://www.saeon.ac.za/"),
                            new JProperty("subject", Properties.Settings.Default.QuerySiteUrl)
                        )
                    );
                var keywords = result.Keywords.Split(new string[] { "; " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var keyword in keywords)
                {
                    jSubjects.Add(
                        new JObject(
                            new JProperty("subject", keyword)
                        )
                    );
                }
                var places = result.Places.Split(new string[] { "; " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var place in places)
                {
                    jSubjects.Add(
                        new JObject(
                            new JProperty("subjectScheme", "name"),
                            new JProperty("schemeURI", "http://www.geonames.org/"),
                            new JProperty("subject", place)
                        )
                    );
                }

                var jGeoLocations = new JArray();
                foreach (var place in places)
                {
                    var splits = place.Split(new char[] { ':' });
                    jGeoLocations.Add(
                        new JObject(
                            new JProperty("geoLocationPlace", $"{splits[0]}, {splits[1]}"),
                            new JProperty("geoLocationPoint",
                                new JObject(
                                    new JProperty("pointLatitude", splits[2]),
                                    new JProperty("pointLongitude", splits[3])
                                )
                            )
                        )
                    );
                }
                jGeoLocations.Add(
                    new JObject(
                        new JProperty("geoLocationBox",
                            new JObject(
                                new JProperty("westBoundLongitude", result.LongitudeWest),
                                new JProperty("eastBoundLongitude", result.LongitudeEast),
                                new JProperty("northBoundLatitude", result.LatitudeNorth),
                                new JProperty("southBoundLatitude", result.LatitudeSouth)
                            )
                        )
                    )
                );

                var jDataCite =
                    new JObject(
                        new JProperty("id", result.DigitalObjectIdentifier.DOI),
                        new JProperty("type", "dois"),
                        new JProperty("attributes",
                            new JObject(
                                new JProperty("doi", result.DigitalObjectIdentifier.DOI),
                                new JProperty("identifiers",
                                    new JArray(
                                        new JObject(
                                            new JProperty("identifier", result.DigitalObjectIdentifier.DOIUrl),
                                            new JProperty("identifierType", "DOI")
                                        ),
                                        new JObject(
                                            new JProperty("identifier", result.Id),
                                            new JProperty("identifierType", "Internal")
                                        )
                                    )
                                ),
                            new JProperty("language", "en-uk"),
                            new JProperty("types",
                                new JObject(
                                    new JProperty("resourceTypeGeneral", "Dataset"),
                                    new JProperty("resourceType", "Tabular Data in Text File(s)")
                                )
                            ),
                            new JProperty("publisher", "South African Environmental Observation Network (SAEON)"),
                            new JProperty("publicationYear", $"{result.Date.Year}"),
                            new JProperty("dates",
                                new JArray(
                                    new JObject(
                                        new JProperty("date", result.Date.ToString("yyyy-MM-dd")),
                                        new JProperty("dateType", "accepted")
                                    ),
                                    new JObject(
                                        new JProperty("date", result.Date.ToString("yyyy-MM-dd")),
                                        new JProperty("dateType", "issued")
                                    ),
                                    new JObject(
                                        new JProperty("date", result.StartDate.ToString("yyyy-MM-dd")),
                                        new JProperty("dateType", "collected")
                                    ),
                                    new JObject(
                                        new JProperty("date", result.EndDate.ToString("yyyy-MM-dd")),
                                        new JProperty("dateType", "collected")
                                    )
                                )
                            ),
                            new JProperty("rights",
                                new JArray(
                                    new JObject(
                                        new JProperty("rights", "Attribution-ShareAlike 4.0 International (CC BY-SA 4.0)"),
                                        new JProperty("rightsURI", "https://creativecommons.org/licenses/by-sa/4.0/")
                                    )
                                )
                            ),
                            new JProperty("creators",
                                new JArray(
                                    new JObject(
                                        new JProperty("name", "South African Environmental Observation Network (SAEON)"),
                                        new JProperty("nameType", "Organizational")
                                    ),
                                    new JObject(
                                        new JProperty("name", "Observations Database Administrator"),
                                        new JProperty("nameType", "Personal"),
                                        new JProperty("givenName", "Tim"),
                                        new JProperty("familyName", "Parker-Nance"),
                                        new JProperty("nameIdentifier", "0000-0001-7040-7736"),
                                        new JProperty("nameIdentifierScheme", "ORCID"),
                                        new JProperty("affiliation", "Organisation:South African Environmental Observation Network (SAEON);e-Mail Address:timpn@saeon.ac.za")
                                    )
                                )
                            ),
                            new JProperty("titles",
                                new JArray(
                                    new JObject(
                                        new JProperty("title", result.Title)
                                    )
                                )
                            ),
                            new JProperty("descriptions",
                                new JArray(
                                    new JObject(
                                        new JProperty("descriptionType", "Abstract"),
                                        new JProperty("description", result.Description)
                                    )
                                )
                            ),
                            new JProperty("contributors",
                                new JArray(
                                    new JObject(
                                        new JProperty("contributorType", "ContactPerson"),
                                        new JProperty("contributorName", "Parker-Nance, Tim"),
                                        new JProperty("givenName", "Tim"),
                                        new JProperty("familyName", "Parker-Nance"),
                                        new JProperty("nameIdentifier", "0000-0001-7040-7736"),
                                        new JProperty("nameIdentifierScheme", "ORCID"),
                                        new JProperty("affiliation", "Organisation:South African Environmental Observation Network (SAEON);e-Mail Address:timpn@saeon.ac.za")
                                    ),
                                    new JObject(
                                        new JProperty("contributorType", "DataManager"),
                                        new JProperty("contributorName", "SAEON uLwazi Node"),
                                        new JProperty("affiliation", "Organisation:South African Environmental Observation Network (SAEON);e-Mail Address:wim@saeon.ac.za")
                                    ),
                                    new JObject(
                                        new JProperty("contributorType", "DataCurator"),
                                        new JProperty("contributorName", "SAEON uLwazi Node"),
                                        new JProperty("affiliation", "Organisation:South African Environmental Observation Network (SAEON);e-Mail Address:wim@saeon.ac.za")
                                    )
                                )
                            ),
                            new JProperty("subjects", jSubjects),
                            new JProperty("geoLocations", jGeoLocations)
                        )
                    )
                );
                return jDataCite.ToString();
            }

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    Logging.Verbose("UserId: {userId}", User.GetUserId());
                    Logging.Verbose("Claims: {claims}", User.GetClaims());
                    // Get Data
                    var output = GetData(input, false);
                    // Create Download
                    var accessed = output.Date.ToString("yyyyMMdd HH:mm:ss.fff");
                    var doiName = $"Data download on {accessed}";
                    // Get a DOI
                    Logging.Verbose("Minting DOI");
                    var doi = new DigitalObjectIdentifier { Name = doiName, AddedBy = User.GetUserId(), UpdatedBy = User.GetUserId() };
                    Logging.Verbose("DOI: {@DOI}", doi);
                    DbContext.DigitalObjectIdentifiers.Add(doi);
                    SaveChanges();
                    doi = DbContext.DigitalObjectIdentifiers.First(i => i.Name == doiName);
                    if (doi == null)
                    {
                        throw new InvalidOperationException($"Unable to find DOI {doiName}");
                    }
                    Logging.Verbose("DOI: {@DOI}", doi);
                    Logging.Verbose("Adding UserDownload");
                    var result = new UserDownload
                    {
                        UserId = User.GetUserId(),
                        Name = doiName,
                        Title = output.Title,
                        Description = output.Description,
                        Citation = "SAEON....",
                        Keywords = string.Join("; ", output.Keywords),
                        Date = output.Date,
                        DigitalObjectIdentifierId = doi.Id,
                        Input = JsonConvert.SerializeObject(input),
                        RequeryUrl = Properties.Settings.Default.QuerySiteUrl + $"/DataWizard/Requery",
                        MetadataJson = "{}",
                        MetadataUrl = "http://datacite.org/10.15493/obsdb.A0.B1.C2.D3",
                        DownloadUrl = Properties.Settings.Default.QuerySiteUrl + $"/DataWizard/ViewDownload/",
                        ZipFullName = HostingEnvironment.MapPath($"~/App_Data/Downloads/{output.Date.ToString("yyyyMM")}"),
                        ZipCheckSum = "ABCD",
                        ZipUrl = Properties.Settings.Default.QuerySiteUrl + $"/DataWizard/DownloadZip/",
                        Places = string.Join("; ", output.Places),
                        LatitudeNorth = output.LatitudeNorth,
                        LatitudeSouth = output.LatitudeSouth,
                        LongitudeWest = output.LongitudeWest,
                        LongitudeEast = output.LongitudeEast,
                        StartDate = output.StartDate.Value,
                        EndDate = output.EndDate.Value,
                        AddedBy = User.GetUserId(),
                        UpdatedBy = User.GetUserId()
                    };
                    Logging.Verbose("UserDownload: {@UserDownload}", result);
                    DbContext.UserDownloads.Add(result);
                    SaveChanges();
                    result = DbContext.UserDownloads.Include(i => i.DigitalObjectIdentifier).FirstOrDefault(i => i.Name == doiName);
                    if (result == null)
                    {
                        throw new InvalidOperationException($"Unable to find UserDownload {accessed}");
                    }
                    Logging.Verbose("UserDownload: {@UserDownload}", result);
                    result.Citation = $"Observations Database ({result.Date.Year}): {output.Title}. South African Environmental Observation Network (SAEON) (Dataset). {result.DigitalObjectIdentifier.DOIUrl}. Accessed {accessed}.";
                    result.Description += Environment.NewLine + "Please cite as follows:" + Environment.NewLine + result.Citation;
                    result.MetadataUrl = $"http://datacite.org/{result.DigitalObjectIdentifier.DOI}";
                    result.RequeryUrl = Properties.Settings.Default.QuerySiteUrl + $"/DataWizard/Requery/{result.Id}";
                    result.DownloadUrl = Properties.Settings.Default.QuerySiteUrl + $"/DataWizard/ViewDownload/{result.Id}";
                    var folder = HostingEnvironment.MapPath($"~/App_Data/Downloads/{output.Date.ToString("yyyyMM")}");
                    var dirInfo = Directory.CreateDirectory(Path.Combine(folder, result.Id.ToString()));
                    result.ZipFullName = Path.Combine(folder, $"{result.Id}.zip");
                    result.ZipUrl = Properties.Settings.Default.QuerySiteUrl + $"/DataWizard/DownloadZip/{result.Id}";
                    // Create files
                    Logging.Verbose("Creating files");
                    File.WriteAllText(Path.Combine(dirInfo.FullName, "Input.json"), JsonConvert.SerializeObject(input, Formatting.Indented));
                    //File.WriteAllText(Path.Combine(dirInfo.FullName, "Output.json"), JsonConvert.SerializeObject(output));
                    // Johan's API
                    result.MetadataJson = JohansAPIJson(result);
                    File.WriteAllText(Path.Combine(dirInfo.FullName, "Metadata.json"), result.MetadataJson);
                    File.WriteAllText(Path.Combine(dirInfo.FullName, "DataCite.xml"), DataCiteXml(result));
                    File.WriteAllText(Path.Combine(dirInfo.FullName, "DataCite.json"), DataCiteJson(result));
                    switch (input.DownloadFormat)
                    {
                        case DownloadFormats.CSV:
                            File.WriteAllText(Path.Combine(dirInfo.FullName, "Data.csv"), output.DataMatrix.AsCSV());
                            break;
                        case DownloadFormats.Excel:
                            break;
                        case DownloadFormats.NetCDF:
                            break;
                    }
                    // Create Zip
                    File.WriteAllText(Path.Combine(dirInfo.FullName, "Download.json"),
                        JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
                    ZipFile.CreateFromDirectory(dirInfo.FullName, Path.Combine(folder, $"{result.Id}.zip"));
                    dirInfo.Delete(true);
                    // Generate Checksum
                    result.ZipCheckSum = GetChecksum(result.ZipFullName);
                    Logging.Verbose("UserDownload: {@UserDownload}", result);
                    SaveChanges();
                    var jChecksum = new JObject(new JProperty("Checksum", result.ZipCheckSum));
                    File.WriteAllText(Path.Combine(folder, $"{result.Id} Checksum.json"), jChecksum.ToString());
                    transaction.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception rex)
                    {
                        Logging.Exception(rex);
                    }
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("GetDownload")]
        [Authorize]
        public UserDownload DownloadGet([FromUri] string input)
        {
            using (Logging.MethodCall<UserDownload>(GetType()))
            {
                try
                {
                    Logging.Verbose("Uri: {Uri}", Request.RequestUri);
                    Logging.Verbose("Input: {input}", input);
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        throw new ArgumentNullException(nameof(input));
                    }
                    return GetDownload(JsonConvert.DeserializeObject<DataWizardDownloadInput>(input));
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        [HttpPost]
        [Route("GetDownload")]
        [Authorize]
        public UserDownload DownloadPost([FromBody] DataWizardDownloadInput input)
        {
            using (Logging.MethodCall<UserDownload>(GetType()))
            {
                try
                {
                    Logging.Verbose("Uri: {Uri}", Request.RequestUri);
                    Logging.Verbose("Input: {@input}", input);
                    if (input == null)
                    {
                        throw new ArgumentNullException(nameof(input));
                    }
                    return GetDownload(input);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

    }

}

