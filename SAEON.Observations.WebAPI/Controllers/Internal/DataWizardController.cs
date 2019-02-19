using Newtonsoft.Json;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using System;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web.Hosting;
using System.Web.Http;

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
                input.Sites.AddRange(db.Organisations.Where(i => i.Id == orgId).SelectMany(i => i.Sites).Select(i => i.Id));
                input.Stations.AddRange(db.Organisations.Where(i => i.Id == orgId).SelectMany(i => i.Stations).Select(i => i.Id));
            }
            foreach (var phenomenonId in input.Phenomena)
            {
                input.Offerings.AddRange(db.PhenomenonOfferings.Where(i => i.PhenomenonId == phenomenonId).Select(i => i.Id));
                input.Units.AddRange(db.PhenomenonUnits.Where(i => i.PhenomenonId == phenomenonId).Select(i => i.Id));
            }
            input.StartDate = input.StartDate.Date;
            input.EndDate = input.EndDate.Date.AddDays(1);
            Logging.Verbose("Processed Input: {@Input}", input);
            var startDate = input.StartDate;
            var endDate = input.EndDate;
            return db.ImportBatchSummary.Where(i =>
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
                    Logging.Verbose("Uri: {Uri}", Request.RequestUri);
                    Logging.Verbose("Input: {input}", input);
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
                    Logging.Verbose("Uri: {Uri}", Request.RequestUri);
                    Logging.Verbose("Input: {@input}", input);
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
            var observations = q.Join(db.Observations.Where(i => (i.StatusId == null) || (i.StatusName == "Verified")), l => l.ImportBatchId, r => r.ImportBatchId, (l, r) => r)
                    .Where(i => phenomenonOfferingIds.Contains(i.PhenomenonOfferingId))
                    .Where(i => phenomenonUnitIds.Contains(i.PhenomenonUnitId))
                    .OrderBy(i => i.SiteName)
                    .ThenBy(i => i.StationName)
                    .ThenBy(i => i.InstrumentName)
                    .ThenBy(i => i.SensorName)
                    .ThenBy(i => i.ValueDate)
                    //.Take(1000)
                    .ToList();
            Logging.Verbose("Observations: {Observations}", observations.Count);
            Guid siteId = new Guid();
            Guid stationId = new Guid();
            Guid instrumentId = new Guid();
            Guid sensorId = new Guid();
            var date = DateTime.MinValue;
            DataMatixRow row = null;
            // Data Matrix
            foreach (var obs in observations)
            {
                // DataMatrix
                if ((row == null) || (obs.SiteId != siteId) || (obs.StationId != stationId) || (obs.InstrumentId != instrumentId) || (obs.SensorId != sensorId) || (obs.ValueDate != date))
                {
                    row = result.DataMatrix.AddRow();
                    row["SiteName"] = obs.SiteName;
                    row["StationName"] = obs.StationName;
                    row["InstrumentName"] = obs.InstrumentName;
                    row["SensorName"] = obs.SensorName;
                    row["Date"] = obs.ValueDate;
                    siteId = obs.SiteId;
                    stationId = obs.StationId;
                    instrumentId = obs.InstrumentId;
                    sensorId = obs.SensorId;
                    date = obs.ValueDate;
                }
                var name = $"{obs.PhenomenonCode.Replace(" ", "")}_{obs.OfferingCode.Replace(" ", "")}_{obs.UnitCode.Replace(" ", "")}";
                //Logging.Verbose("Name: {Name}",name);
                row[name] = obs.DataValue;
            }
            Logging.Verbose("DataMatrix: Rows: {Rows} Cols: {Cols}", result.DataMatrix.Rows.Count, result.DataMatrix.Columns.Count);
            //Chart series
            if (includeChart)
            {
                ChartSeries series = null;
                siteId = new Guid();
                stationId = new Guid();
                instrumentId = new Guid();
                sensorId = new Guid();
                foreach (var obs in observations)
                {
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
                    Logging.Verbose("Uri: {Uri}", Request.RequestUri);
                    Logging.Verbose("Input: {input}", input);
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
                    Logging.Verbose("Uri: {Uri}", Request.RequestUri);
                    Logging.Verbose("Input: {@input}", input);
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
            Logging.Verbose("UserId: {userId}", User.GetUserId());
            Logging.Verbose("Claims: {claims}", User.GetClaims());
            // Get Data
            var dataOutput = GetData(input, false);
            // Create Download
            var date = DateTime.Now;
            var name = date.ToString("yyyyMMdd HH:mm:ss.fff");
            var result = new UserDownload
            {
                UserId = User.GetUserId(),
                Name = name,
                Description = $"Data download on {name}",
                QueryInput = JsonConvert.SerializeObject(input),
                QueryURL = Properties.Settings.Default.QuerySiteUrl + $"/DataWizard/GetData?json={JsonConvert.SerializeObject(input)}",
                DOI = "http://data.saeon.ac.za/10.11.12.13",
                MetadataURL = "http://data.saeon.ac.za",
                DownloadURL = Properties.Settings.Default.QuerySiteUrl + $"/DataWizard/ViewDownload",
                Citation = "SAEON....",
                AddedBy = User.GetUserId(),
                UpdatedBy = User.GetUserId()
            };
            Logging.Verbose("UserDownload: {@UserDownload}", result);
            db.UserDownloads.Add(result);
            db.SaveChanges();
            result = db.UserDownloads.FirstOrDefault(i => i.Name == name);
            if (result == null)
            {
                throw new InvalidOperationException($"Unable to find UserDownload {name}");
            }
            var folder = HostingEnvironment.MapPath($"~/App_Data/Downloads/{date.ToString("yyyyMM")}");
            var dirInfo = Directory.CreateDirectory(Path.Combine(folder, result.Id.ToString()));
            result.DownloadURL = Properties.Settings.Default.QuerySiteUrl + $"/DataWizard/Download/{result.Id}";
            db.SaveChanges();
            // Create files
            File.WriteAllText(Path.Combine(dirInfo.FullName, "Input.json"), JsonConvert.SerializeObject(input));
            File.WriteAllText(Path.Combine(dirInfo.FullName, "Metadata.json"), JsonConvert.SerializeObject(new { }));
            File.WriteAllText(Path.Combine(dirInfo.FullName, "Data.csv"), dataOutput.DataMatrix.AsCSV());
            // Excel
            // NetCDF
            ZipFile.CreateFromDirectory(dirInfo.FullName, Path.Combine(folder, $"{result.Id}.zip"));
            dirInfo.Delete(true);
            return result;
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

