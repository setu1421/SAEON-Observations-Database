using Newtonsoft.Json.Linq;
using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Thinktecture.IdentityModel.WebApi;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    [RoutePrefix("DataQuery")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ResourceAuthorize("Observations.QuerySite", "DataQuery")]
    public class DataQueriesController : ApiController
    {
        protected ObservationsDbContext db = null;

        public DataQueriesController() : base()
        {
            using (Logging.MethodCall(GetType()))
            {
                db = new ObservationsDbContext();
                db.Configuration.AutoDetectChangesEnabled = false;
                db.Database.CommandTimeout = 0;
            }
        }

        protected override void Dispose(bool disposing)
        {
            using (Logging.MethodCall(GetType()))
            {
                if (disposing)
                {
                    if (db != null)
                    {
                        db.Dispose();
                    }
                }
                base.Dispose(disposing);
            }
        }

        public class Feature
        {
            public Guid PhenomenonId { get; set; }
            public Guid PhenomenonOfferingId { get; set; }
            public Guid OfferingId { get; set; }
            public Guid UnitOfMeasureId { get; set; }
            public string Caption { get; set; }
            public string Name { get; set; }

            public Feature(VDownload data)
            {
                PhenomenonId = data.PhenomenonId;
                PhenomenonOfferingId = data.PhenomenonOfferingId;
                OfferingId = data.OfferingId;
                UnitOfMeasureId = data.UnitOfMeasureId;
                Caption = $"{data.PhenomenonName}, {data.OfferingName}, {data.UnitOfMeasureSymbol}";
                Name = $"{data.PhenomenonCode}_{data.OfferingCode}_{data.UnitOfMeasureCode}";
            }
        }

        [HttpPost]
        [Route]
        public async Task<DataQueryOutput> DataQuery(DataQueryInput input)
        {
            using (Logging.MethodCall(GetType(), new ParameterList { { "Params", input } }))
            {
                try
                {
                    Logging.Verbose("Input: {@input}", input);
                    if (input == null) throw new ArgumentNullException("input");
                    if (input.Locations == null) throw new ArgumentNullException("input.Locations");
                    if (!input.Locations.Any()) throw new ArgumentOutOfRangeException("input.Locations");
                    if (input.Features == null) throw new ArgumentNullException("input.Features");
                    if (!input.Features.Any()) throw new ArgumentOutOfRangeException("input.Features");
                    var dataList = await db.VDownloads
                        .Where(i => input.Locations.Contains(i.StationId))
                        .Where(i => input.Features.Contains(i.PhenomenonOfferingId))
                        .Where(i => i.Date >= input.StartDate)
                        .Where(i => i.Date <= input.EndDate)
                        .OrderBy(i => i.SiteName)
                        .ThenBy(i => i.StationName)
                        .ThenBy(i => i.InstrumentName)
                        .ThenBy(i => i.Date)
                        .ToListAsync();
                    Logging.Verbose("DataList: {count}", dataList.Count);
                    //Logging.Verbose("DataList: {count} {@dataList}", dataList.Count, dataList);
                    string lastSite = null;
                    string lastStation = null;
                    string lastInstrument = null;
                    DateTime? lastDate = null;
                    var features = new List<Feature>();
                    var result = new DataQueryOutput();
                    result.Series.Add(new Series { ColumnName = "Date", Caption = "Date" });
                    List<ExpandoObject> rows = new List<ExpandoObject>();
                    dynamic row = null;
                    bool isNewRow = false;
                    foreach (var data in dataList)
                    {
                        if (lastSite != data.SiteName)
                        {
                            isNewRow = true;
                            lastSite = data.SiteName;
                            lastStation = data.StationName;
                            lastInstrument = data.InstrumentName;
                            lastDate = data.Date;
                        }
                        if (!isNewRow && (lastStation != data.StationName))
                        {
                            isNewRow = true;
                            lastStation = data.StationName;
                            lastInstrument = data.InstrumentName;
                            lastDate = data.Date;
                        }
                        if (!isNewRow && (lastInstrument != data.InstrumentName))
                        {
                            isNewRow = true;
                            lastInstrument = data.InstrumentName;
                            lastDate = data.Date;
                        }
                        if (!isNewRow && lastDate.HasValue && (lastDate.Value != data.Date))
                        {
                            isNewRow = true;
                            lastDate = data.Date;
                        }

                        if (isNewRow)
                        {
#pragma warning disable IDE0017 // Simplify object initialization
                            row = new ExpandoObject();
#pragma warning restore IDE0017 // Simplify object initialization
                            row.Date = data.Date;
                            rows.Add(row);
                            isNewRow = false;
                        }
                        var feature = new Feature(data);
                        if (!features.Any(i => i.Name == feature.Name))
                        {
                            features.Add(feature);
                            result.Series.Add(new Series { ColumnName = feature.Name, Caption = feature.Caption, IsFeature = true });
                            //Logging.Verbose("Adding {name} {caption}", col.ColumnName, col.Caption);
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
                    var jArray = new JArray();
                    foreach (var r in rows)
                    {
                        jArray.Add(JObject.FromObject(r));
                    }
                    result.ResultsChartData = jArray.ToString();
                    JObject jObject = JObject.FromObject(new
                    {
                        result = jArray,
                        count = jArray.Count
                    });
                    result.ResultsGridData = jObject.ToString();
                    //Logging.Verbose("GridResults: {GridResults}", result.ResultsGridData);
                    //Logging.Verbose("ChartResults: {ChartResults}", result.ResultsChartData);
                    Logging.Verbose("Result: Cols: {cols} Rows: {rows}", result.Series.Count, rows.Count);
                    return result;
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
