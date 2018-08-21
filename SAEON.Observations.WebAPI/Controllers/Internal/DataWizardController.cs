using Newtonsoft.Json;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using System;
using System.Data;
using System.Linq;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    [RoutePrefix("Internal/DataWizard")]
    public class DataWizardController : BaseController
    {
        private IQueryable<ImportBatchSummary> GetQuery(DataWizardInput input)
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
                (input.Offerings.Contains(i.PhenomenonOfferingId) || input.Units.Contains(i.PhenomenonUnitId)) &&
                (i.StartDate >= startDate && i.EndDate < endDate)
                );
        }

        private DataWizardApproximation CalculateApproximation(DataWizardInput input)
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
        public DataWizardApproximation ApproximationGet([FromUri] string json)
        {
            using (Logging.MethodCall<DataWizardApproximation>(GetType()))
            {
                try
                {
                    Logging.Verbose("Uri: {Uri}", Request.RequestUri);
                    Logging.Verbose("Json: {Json}", json);
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        throw new ArgumentNullException(nameof(json));
                    }
                    return CalculateApproximation(JsonConvert.DeserializeObject<DataWizardInput>(json));
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
        public DataWizardApproximation ApproximationPost([FromBody] DataWizardInput input)
        {
            using (Logging.MethodCall<DataWizardApproximation>(GetType()))
            {
                try
                {
                    Logging.Verbose("Uri: {Uri}", Request.RequestUri);
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
                if (obj == null) return false;
                if (!(obj is DataFeature feature)) return false;
                return Equals(feature);
            }

            public bool Equals(DataFeature feature)
            {
                if (feature == null) return false;
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

        private DataWizardOutput Execute(DataWizardInput input)
        {
            var result = new DataWizardOutput();
            result.DataMatrix.AddColumn("SiteName", "Site", MaxtixDataType.String);
            result.DataMatrix.AddColumn("StationName", "Station", MaxtixDataType.String);
            result.DataMatrix.AddColumn("InstrumentName", "Instrument", MaxtixDataType.String);
            result.DataMatrix.AddColumn("SensorName", "Sensor", MaxtixDataType.String);
            result.DataMatrix.AddColumn("Date", "Date", MaxtixDataType.Date);

            var q = GetQuery(input);
            var qFeatures = q.Select(i => new {i.PhenomenonOfferingId, i.PhenomenonUnitId, i.PhenomenonCode, i.PhenomenonName, i.OfferingCode, i.OfferingName, i.UnitCode, i.UnitName, i.UnitSymbol }).Distinct();
            var features = qFeatures.ToList().Select(i => new DataFeature
                {
                    PhenomenonOfferingId = i.PhenomenonOfferingId,
                    PhenomenonUnitId = i.PhenomenonUnitId,
                    Phenomenon = i.PhenomenonName,
                    Offering = i.OfferingName,
                    Unit = i.UnitName,
                    Symbol = i.UnitSymbol,
                    Name = $"{i.PhenomenonCode.Replace(" ", "")}_{i.OfferingCode.Replace(" ", "")}_{i.UnitCode.Replace(" ","")}",
                    Caption = $"{i.PhenomenonName}, {i.OfferingName}, {i.UnitSymbol}"
                });
            foreach (var feature in features)
            {
                result.DataMatrix.AddColumn(feature.Name, feature.Caption, MaxtixDataType.Double);
            }
            var observations = q.Join(db.Observations, l => l.ImportBatchId, r => r.ImportBatchId, (l, r) => r)
                    .OrderBy(i => i.SiteName)
                    .ThenBy(i => i.StationName)
                    .ThenBy(i => i.InstrumentName)
                    .ThenBy(i => i.SensorName)
                    .ThenBy(i => i.ValueDate)
                    //.Take(1000)
                    .ToList();
            Guid siteId = new Guid();
            Guid stationId = new Guid();
            Guid instrumentId = new Guid();
            Guid sensorId = new Guid();
            var date = DateTime.MinValue;
            DataMatixRow row = null;
            ChartSeries series = null;
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
                row[name] = obs.DataValue;
            }
            Logging.Verbose("DataMatrix: Rows: {Rows} Cols: {Cols}", result.DataMatrix.Rows.Count, result.DataMatrix.Columns.Count);
            //Chart series
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
            return result;
        }

        [HttpGet]
        [Route("Execute")]
        public DataWizardOutput ExecuteGet([FromUri] string json)
        {
            using (Logging.MethodCall<DataWizardOutput>(GetType()))
            {
                try
                {
                    Logging.Verbose("Uri: {Uri}", Request.RequestUri);
                    Logging.Verbose("Json: {Json}", json);
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        throw new ArgumentNullException(nameof(json));
                    }
                    return Execute(JsonConvert.DeserializeObject<DataWizardInput>(json));
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        [HttpPost]
        [Route("Execute")]
        public DataWizardOutput ExecutePost([FromBody] DataWizardInput input)
        {
            using (Logging.MethodCall<DataWizardOutput>(GetType()))
            {
                try
                {
                    Logging.Verbose("Uri: {Uri}", Request.RequestUri);
                    if (input == null)
                    {
                        throw new ArgumentNullException(nameof(input));
                    }
                    return Execute(input);
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
