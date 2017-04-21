using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers
{
    [RoutePrefix("DataQuery")]
    //[ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    public class DataQueryController : ApiController
    {
        protected ObservationsDbContext db = null;

        public DataQueryController() : base()
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
                Name = $"Data_{data.PhenomenonCode}_{data.OfferingCode}_{data.UnitOfMeasureCode}";
            }
        }

        [HttpPost]
        //[Route("{stationIds}/{phenomenonOfferingIds}/{startDate:datetime?}/{enddate:datetime?}")]
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
                    result.Data.TableName = "Observations";
                    result.Data.Columns.Add("Site", typeof(string));
                    result.Data.Columns.Add("Station", typeof(string));
                    result.Data.Columns.Add("Instrument", typeof(string));
                    result.Data.Columns.Add("Date", typeof(DateTime));
                    result.Series.Add(new Series { Name = "Site", Caption = "Site" });
                    result.Series.Add(new Series { Name = "Station", Caption = "Station" });
                    result.Series.Add(new Series { Name = "Instrument", Caption = "Instrument" });
                    result.Series.Add(new Series { Name = "Date", Caption = "Date" });
                    DataRow row = null;
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
                            row = result.Data.Rows.Add();
                            row["Site"] = data.SiteName;
                            row["Station"] = data.StationName;
                            row["Instrument"] = data.InstrumentName;
                            row["Date"] = data.Date;
                            isNewRow = false;
                        }
                        var feature = new Feature(data);
                        if (!features.Any(i => i.Name == feature.Name))
                        {
                            features.Add(feature);
                            var col = result.Data.Columns.Add(feature.Name, typeof(double));
                            col.Caption = feature.Caption;
                            result.Series.Add(new Series { Name = col.ColumnName, Caption = col.Caption, IsFeature = true });
                            //Logging.Verbose("Adding {name} {caption}", col.ColumnName, col.Caption);
                        }
                        if (row.IsNull(feature.Name))
                            row[feature.Name] = data.Value;
                        else
                        {
                            double oldValue = row.Field<double>(feature.Name);
                            if (data.Value.HasValue)
                            {
                                row[feature.Name] = data.Value + oldValue;
                            }
                        }
                    }
                    Logging.Verbose("Result: Cols: {cols} Rows: {rows}", result.Data.Columns.Count, result.Data.Rows.Count);
                    foreach (var feature in features.Take(10))
                    {
                        var points = dataList
                            .Where(i => i.PhenomenonOfferingId == feature.PhenomenonOfferingId)
                            .Select(i => new SeriesPoint { Date = i.Date, Value = i.Value })
                            .ToList();
                        result.Series.Where(i => i.Name == feature.Name).First().Points.AddRange(points);
                        Logging.Verbose("Series: Name: {name} Points: {@points}", feature.Caption, points.Take(100));
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

    }
}
