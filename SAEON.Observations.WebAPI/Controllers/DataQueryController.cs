using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers
{
    [RoutePrefix("DataQuery")]
    //[ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    public class DataQueryController : ApiController
    {
        protected ObservationsDbContext db = new ObservationsDbContext();

        public DataQueryController() : base()
        {
            using (Logging.MethodCall(this.GetType()))
            {
            }
        }

        protected override void Dispose(bool disposing)
        {
            using (Logging.MethodCall(this.GetType()))
            {
                if (disposing)
                {
                    db.Dispose();
                }
                base.Dispose(disposing);
            }
        }

        public class Feature
        {
            public Guid PhenomenonId { get; set; }
            public Guid OfferingId { get; set; }
            public Guid UnitOfMeasureId { get; set; }
            public string Header { get; set; }
            public string Name { get; set; }

            public Feature(VDownload data)
            {
                PhenomenonId = data.PhenomenonId;
                OfferingId = data.OfferingId;
                UnitOfMeasureId = data.UnitOfMeasureId;
                Header = $"{data.PhenomenonName}, {data.OfferingName}, {data.UnitOfMeasureSymbol}";
                Name = $"{data.PhenomenonCode}_{data.OfferingCode}_{data.UnitOfMeasureCode}";
            }
        }

        [HttpPost]
        //[Route("{stationIds}/{phenomenonOfferingIds}/{startDate:datetime?}/{enddate:datetime?}")]
        [Route]
        public DataQueryOutput DataQuery(DataQueryInput input)
        {
            using (Logging.MethodCall(this.GetType(), new ParameterList { { "Params", input } }))
            {
                try
                {
                    Logging.Verbose("Input: {@input}", input);
                    if (input == null) throw new ArgumentNullException("input");
                    if (input.Locations == null) throw new ArgumentNullException("input.Locations");
                    if (!input.Locations.Any()) throw new ArgumentOutOfRangeException("input.Locations");
                    if (input.Features == null) throw new ArgumentNullException("input.Features");
                    if (!input.Features.Any()) throw new ArgumentOutOfRangeException("input.Features");
                    db.Configuration.AutoDetectChangesEnabled = false;
                    var dataList = db.VDownloads//.AsQueryable()
                        .Where(i => input.Locations.Contains(i.StationId))
                        .Where(i => input.Features.Contains(i.PhenomenonOfferingId))
                        .Where(i => i.Date >= input.StartDate)
                        .Where(i => i.Date <= input.EndDate)
                        .OrderBy(i => i.SiteName)
                        .ThenBy(i => i.StationName)
                        .ThenBy(i => i.Date)
                        .Take(100)
                        .ToList();
                    Logging.Verbose("DataList: {count} {@dataList}", dataList.Count, dataList);
                    string lastSite = null;
                    string lastStation = null;
                    string lastInstrument = null;
                    string lastSensor = null;
                    DateTime? lastDate = null;
                    var features = new List<Feature>();
                    var result = new DataQueryOutput
                    {
                        Headers = new List<string> { "Site", "Station", "Instrument", "Sensor", "Date" }
                    };
                    dynamic row = null;
                    bool isNewRow = false;
                    foreach (var data in dataList)
                    {
                        if (lastSite != data.SiteName)
                        {
                            isNewRow = true;
                            lastSite = data.SiteName;
                            lastStation = null;
                            lastInstrument = null;
                            lastSensor = null;
                            lastDate = null;
                        }
                        if (!isNewRow && (lastStation != data.StationName))
                        {
                            isNewRow = true;
                            lastStation = data.StationName;
                            lastInstrument = null;
                            lastSensor = null;
                            lastDate = null;
                        }
                        if (!isNewRow && (lastInstrument != data.InstrumentName))
                        {
                            isNewRow = true;
                            lastInstrument = data.InstrumentName;
                            lastSensor = null;
                            lastDate = null;
                        }
                        if (!isNewRow && (lastSensor != data.SensorName))
                        {
                            isNewRow = true;
                            lastSensor = data.SensorName;
                            lastDate = null;
                        }
                        if (!isNewRow && lastDate.HasValue && (lastDate.Value != data.Date))
                        {
                            isNewRow = true;
                            lastDate = data.Date;
                        }

                        if (isNewRow)
                        {
                            if (row != null)
                            {
                                result.Rows.Add(row);
                                row = null;
                            }
                            row = new ExpandoObject();
                            row.SiteName = data.SiteName;
                            row.StationName = data.StationName;
                            row.InstrumentName = data.InstrumentName;
                            row.SensorName = data.SensorName;
                            row.Date = data.Date;
                        }
                        var feature = new Feature(data);
                        if (!features.Any(i => i.Name == feature.Name))
                        {
                            features.Add(feature);
                            result.Headers.Add(feature.Header);
                        }
                        var r = row as IDictionary<string, object>;
                        if (!r.ContainsKey(feature.Name))
                        {
                            r.Add(feature.Name, data.Value);
                        }
                        else
                        {
                            double? oldValue = (double?)r[feature.Name];
                            if (!oldValue.HasValue || !data.Value.HasValue)
                            {
                                r[feature.Name] = data.Value;
                            }
                            else
                            {
                                r[feature.Name] = data.Value + oldValue.Value;
                            }
                        }
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        /*
        /// <summary>
        /// Get
        /// </summary>
        /// <returns>Json</returns>
        [HttpPost]
        //[Route("{stationIds}/{phenomenonOfferingIds}/{startDate:datetime?}/{enddate:datetime?}")]
        [Route]
        public List<object> DataQuery([FromBody] string locations, [FromBody] string features, [FromBody] DateTime? startDate, [FromBody] DateTime? endDate)
        {
            using (Logging.MethodCall(this.GetType(), new ParameterList {
                { "StationIds", locations },
                { "PhenomenonOfferingIds", features },
                { "StartDate", startDate },
                { "EndDate", endDate } }))
            {
                try
                {
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get");
                    throw;
                }
            }

        }
        */
    }
}
