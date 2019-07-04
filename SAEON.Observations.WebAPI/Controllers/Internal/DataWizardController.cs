using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SAEON.AspNet.Common;
using SAEON.Core;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Threading.Tasks;
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
            string GetCode(string sensorCode, string phenomenonCode, string offeringCode, string unitCode)
            {
                return $"{sensorCode.Replace("_", string.Empty)}_{phenomenonCode.Replace("_", string.Empty)}_{offeringCode.Replace("_", string.Empty)}_{unitCode.Replace("_", string.Empty)}".Replace(" ", string.Empty);
            }

            string GetName(string sensorName, string phenomenonName, string offeringName, string unitName)
            {
                return $"{sensorName.Replace(", ", "_")}, {phenomenonName.Replace(", ", "_")}, {offeringName.Replace(", ", "_")}, {unitName.Replace(", ", "_")}";
            }


            var result = new DataWizardDataOutput();
            result.DataMatrix.AddColumn("SiteName", "Site", MaxtixDataType.String);
            result.DataMatrix.AddColumn("StationName", "Station", MaxtixDataType.String);
            result.DataMatrix.AddColumn("InstrumentName", "Instrument", MaxtixDataType.String);
            result.DataMatrix.AddColumn("SensorName", "Sensor", MaxtixDataType.String);
            result.DataMatrix.AddColumn("Date", "Date", MaxtixDataType.Date);
            result.DataMatrix.AddColumn("Elevation", "Elevation", MaxtixDataType.Double);

            var q = GetQuery(input);
            var qFeatures = q.Select(i => new { i.SensorId, i.SensorCode, i.SensorName, i.PhenomenonOfferingId, i.PhenomenonUnitId, i.PhenomenonCode, i.PhenomenonName, i.OfferingCode, i.OfferingName, i.UnitCode, i.UnitName, i.UnitSymbol }).Distinct();
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
                var name = GetCode(obs.SensorCode, obs.PhenomenonCode, obs.OfferingCode, obs.UnitCode);
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
            Logging.Verbose("DataMatrix: {DataMatrix}", JsonConvert.SerializeObject(result.DataMatrix));
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
                result.Description += $" in area {result.LatitudeNorth:f5},{result.LongitudeWest:f5} (N,W) {result.LatitudeSouth:f5},{result.LongitudeEast:f5} (S,E)";
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

#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
        private async Task<UserDownload> GetDownload(DataWizardDownloadInput input)
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
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

            async Task SaveChangesAsync()
            {
                try
                {
                    await DbContext.SaveChangesAsync();
                }
                catch (DbEntityValidationException ex)
                {
                    Logging.Exception(ex, string.Join("; ", GetValidationErrors(ex)));
                    throw;
                }
            }

            JObject ODPApiJson(UserDownload result)
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
                                new JProperty("westBoundLongitude", result.LongitudeWest.ToString()),
                                new JProperty("eastBoundLongitude", result.LongitudeEast.ToString()),
                                new JProperty("northBoundLatitude", result.LatitudeNorth.ToString()),
                                new JProperty("southBoundLatitude", result.LatitudeSouth.ToString())
                            )
                        )
                    )
                );

                var jODBApi =
                    new JObject(
                        new JProperty("identifier",
                            new JObject(
                                new JProperty("identifier", result.DigitalObjectIdentifier.DOI),
                                new JProperty("identifierType", "DOI")
                            )
                        ),
                        new JProperty("alternateIdentifiers",
                            new JArray(
                                new JObject(
                                    new JProperty("alternateIdentifier", result.Id),
                                    new JProperty("alternateIdentifierType", "Internal")
                                )
                            )
                        ),
                        new JProperty("language", "en-uk"),
                        new JProperty("resourceType",
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
                                    new JProperty("dateType", "Accepted")
                                ),
                                new JObject(
                                    new JProperty("date", result.Date.ToString("yyyy-MM-dd")),
                                    new JProperty("dateType", "Issued")
                                ),
                                new JObject(
                                    new JProperty("date", result.StartDate.ToString("yyyy-MM-dd")),
                                    new JProperty("dateType", "Collected")
                                ),
                                new JObject(
                                    new JProperty("date", result.EndDate.ToString("yyyy-MM-dd")),
                                    new JProperty("dateType", "Collected")
                                )
                            )
                        ),
                        new JProperty("rightsList",
                            new JArray(
                                new JObject(
                                    new JProperty("rights", "Attribution-ShareAlike 4.0 International (CC BY-SA 4.0)"),
                                    new JProperty("rightsURI", "https://creativecommons.org/licenses/by-sa/4.0/"),
                                    new JProperty("rightsIdentifier", "CC-BY-SA-4.0"),
                                    new JProperty("rightsIdentifierScheme", "SPDX"),
                                    new JProperty("schemeURI", "https://spdx.org/licenses/")
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
                                    new JProperty("nameIdentifiers",
                                        new JArray(
                                            new JObject(
                                                new JProperty("nameIdentifier", "0000-0001-7040-7736"),
                                                new JProperty("nameIdentifierScheme", "ORCID"),
                                                new JProperty("schemeURI", "http://orcid.org/")
                                            )
                                        )
                                    ),
                                    new JProperty("affiliations",
                                        new JArray(
                                            new JObject(
                                                new JProperty("affiliation", "Organisation:South African Environmental Observation Network (SAEON);e-Mail Address:timpn@saeon.ac.za")
                                            )
                                        )
                                    )
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
                                    new JProperty("name", "Parker-Nance, Tim"),
                                    new JProperty("givenName", "Tim"),
                                    new JProperty("familyName", "Parker-Nance"),
                                    new JProperty("nameIdentifiers",
                                        new JArray(
                                            new JObject(
                                                new JProperty("nameIdentifier", "0000-0001-7040-7736"),
                                                new JProperty("nameIdentifierScheme", "ORCID"),
                                                new JProperty("schemeURI", "http://orcid.org/")
                                            )
                                        )
                                    ),
                                    new JProperty("affiliations",
                                        new JArray(
                                            new JObject(
                                                new JProperty("affiliation", "Organisation:South African Environmental Observation Network (SAEON);e-Mail Address:timpn@saeon.ac.za")
                                            )
                                        )
                                    )
                                ),
                                new JObject(
                                    new JProperty("contributorType", "DataManager"),
                                    new JProperty("name", "SAEON uLwazi Node"),
                                    new JProperty("affiliations",
                                        new JArray(
                                            new JObject(
                                                new JProperty("affiliation", "Organisation:South African Environmental Observation Network (SAEON);e-Mail Address:wim@saeon.ac.za")
                                            )
                                        )
                                    )
                                ),
                                new JObject(
                                    new JProperty("contributorType", "DataCurator"),
                                    new JProperty("name", "SAEON uLwazi Node"),
                                    new JProperty("affiliations",
                                        new JArray(
                                            new JObject(
                                                new JProperty("affiliation", "Organisation:South African Environmental Observation Network (SAEON);e-Mail Address:wim@saeon.ac.za")
                                            )
                                        )
                                    )
                                )
                            )
                        ),
                        new JProperty("subjects", jSubjects),
                        new JProperty("geoLocations", jGeoLocations),
                        //new JProperty("url", result.DownloadUrl),
                        //new JProperty("contentUrl", result.ZipUrl),
                        new JProperty("immutableResource",
                            new JObject(
                                //new JProperty("resourceURL", result.ZipUrl),
                                new JProperty("resourceURL", result.DownloadUrl),
                                new JProperty("resourceChecksum", result.ZipCheckSum),
                                new JProperty("checksumAlgorithm", "sha256"),
                                new JProperty("resourceName", result.Title),
                                new JProperty("resourceDescription", result.Description)
                            )
                        )
                    );
                return jODBApi;
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
                    var accessed = output.Date.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    var doiName = $"Data download on {accessed}";
                    // Get a DOI
                    Logging.Verbose("Minting DOI");
                    var doi = new DigitalObjectIdentifier { Name = doiName, AddedBy = User.GetUserId(), UpdatedBy = User.GetUserId() };
                    Logging.Verbose("DOI: {@DOI}", doi);
                    DbContext.DigitalObjectIdentifiers.Add(doi);
                    await SaveChangesAsync();
                    doi = await DbContext.DigitalObjectIdentifiers.FirstAsync(i => i.Name == doiName);
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
                    await SaveChangesAsync();
                    result = await DbContext.UserDownloads.Include(i => i.DigitalObjectIdentifier).FirstOrDefaultAsync(i => i.Name == doiName);
                    if (result == null)
                    {
                        throw new InvalidOperationException($"Unable to find UserDownload {accessed}");
                    }
                    Logging.Verbose("UserDownload: {@UserDownload}", result);
                    result.ZipCheckSum = null;
                    result.Citation = $"Observations Database ({result.Date.Year}): {output.Title}. South African Environmental Observation Network (SAEON) (Dataset). " +
                        $"{result.DigitalObjectIdentifier.DOIUrl}. Accessed {result.Date.ToString("yyyy-MM-dd HH:mm")}.";
                    result.Description += Environment.NewLine + "Please cite as follows:" + Environment.NewLine + result.Citation;
                    result.MetadataUrl = $"https://api.datacite.org/dois/{result.DigitalObjectIdentifier.DOI}";
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
                    var jODP = new JObject(
                        new JProperty("institution", "south-african-environmental-observation-network"),
                        new JProperty("metadata_standard", "saeon-odp-4-2"),
                        new JProperty("infrastructures", new JArray()),
                        new JProperty("collection", "saeon-observations-database"),
                        new JProperty("metadata", ODPApiJson(result))
                    );
                    result.MetadataJson = jODP.ToString();
                    File.WriteAllText(Path.Combine(dirInfo.FullName, "Metadata.json"), result.MetadataJson);
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
                    jODP["metadata"] = ODPApiJson(result);
                    result.MetadataJson = jODP.ToString();
                    Logging.Verbose("UserDownload: {@UserDownload}", result);
                    var jChecksum = new JObject(new JProperty("Checksum", result.ZipCheckSum));
                    File.WriteAllText(Path.Combine(folder, $"{result.Id} Checksum.json"), jChecksum.ToString());
                    var client = new HttpClient
                    {
                        BaseAddress = new Uri(Properties.Settings.Default.ODPUrl)
                    };
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.ApplicationJson));
                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization",ConfigurationManager.AppSettings["ODPUrl"]);
                    HttpResponseMessage response = await client.PostAsync("/metadata/", new StringContent(jODP.ToString(Formatting.None)));
                    if (!response.IsSuccessStatusCode)
                    {
                        Logging.Error("HttpError: {StatusCode} {Reason}", response.StatusCode, response.ReasonPhrase);
                        Logging.Error("Body: {Body}", jODP.ToString());
                        Logging.Error("Response: {Response}", await response.Content.ReadAsStringAsync());
                    }
                    response.EnsureSuccessStatusCode();
                    var jObj = JObject.Parse(await response.Content.ReadAsStringAsync());
                    if (!bool.Parse(jObj.Value<string>("validated")))
                    {
                        Logging.Error("Unable to create metadata on Open Data Platform. Errors: {Errors}", jObj.Value<string>("errors"));
                        throw new InvalidOperationException("Unable to create metadata on Open Data Platform");
                    }
                    result.OpenDataPlatformId = Guid.Parse(jObj.Value<string>("id"));
                    await SaveChangesAsync();
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
        public async Task<UserDownload> DownloadGetAsync([FromUri] string input)
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
                    return await GetDownload(JsonConvert.DeserializeObject<DataWizardDownloadInput>(input));
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
        public async Task<UserDownload> DownloadPostAsync([FromBody] DataWizardDownloadInput input)
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
                    return await GetDownload(input);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("DownloadZip/{id:guid}")]
        public HttpResponseMessage DownloadZip(Guid id)
        {
            using (Logging.MethodCall<UserDownload>(GetType(), new ParameterList { { "Id", id } }))
            {
                try
                {
                    var userDownload = DbContext.UserDownloads.FirstOrDefault(i => i.Id == id);
                    if (userDownload == null)
                    {
                        throw new ArgumentException($"UserDownload with Id: {id} not found!");
                        //return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"UserDownload with Id: {id} not found!");
                    }
                    var bytes = File.ReadAllBytes(userDownload.ZipFullName);
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new ByteArrayContent(bytes);
                    response.Content.Headers.ContentLength = bytes.LongLength;
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = Path.GetFileName(userDownload.ZipFullName)
                    };
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Zip);
                    return response;
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

