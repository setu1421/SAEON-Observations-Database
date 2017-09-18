using SAEON.AspNet.WebApi;
using SAEON.Logs;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    [RoutePrefix("DataQuery")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ClientAuthorization("SAEON.Observations.QuerySite")]
    public class DataQueryController : BaseController
    {
        public DataQueryController() : base()
        {
            db.Database.CommandTimeout = 0;
        }

        [HttpPost]
        [Route]
        public async Task<DataQueryOutput> Execute(DataQueryInput input)
        {
            using (Logging.MethodCall(GetType(), new ParameterList { { "Params", input } }))
            {
                try
                {
                    Logging.Verbose("Input: {@input}", input);
                    if (input == null) throw new ArgumentNullException("input");
                    if (input.Stations == null) throw new ArgumentNullException("input.Stations");
                    if (!input.Stations.Any()) throw new ArgumentOutOfRangeException("input.Stations");
                    if (input.PhenomenaOfferings == null) throw new ArgumentNullException("input.PhenomenaOfferings");
                    if (!input.PhenomenaOfferings.Any()) throw new ArgumentOutOfRangeException("input.PhenomenaOfferings");
                    var output = new DataQueryOutput();
                    foreach (var station in await db.Stations.Where(i => input.Stations
                        .Contains(i.Id))
                        .Include(i => i.Site)
                        .Include(i => i.Instruments.Select(j => j.Sensors.Select(s => s.Phenomenon)))
                        .OrderBy(i => i.Name)
                        .ToListAsync())
                    {
                        var card = new Card { Site = station.Site.Name, Station = station.Name };
                        card.Instruments.AddRange(station.Instruments.OrderBy(i => i.Name).Select(i => new CardInstrument { Name = i.Name }));
                        card.Phenomena.AddRange(station.Instruments.SelectMany(i => i.Sensors).Select(s => s.Phenomenon).Select(i => new CardPhenomenon { Name = i.Name }));
                        output.Cards.Add(card);
                    }
                    var dataList = await db.vApiDataQueries
                        .Where(i => input.Stations.Contains(i.StationId))
                        .Where(i => input.PhenomenaOfferings.Contains(i.PhenomenonOfferingId))
                        .Where(i => i.ValueDay >= input.StartDate)
                        .Where(i => i.ValueDay <= input.EndDate)
                        .OrderBy(i => i.SiteName)
                        .ThenBy(i => i.StationName)
                        .ThenBy(i => i.ValueDate)
                        .ToListAsync();
                    Logging.Verbose("DataList: {count}", dataList.Count);
                    //Logging.Verbose("DataList: {count} {@dataList}", dataList.Count, dataList);
                    string lastSite = null;
                    string lastStation = null;
                    DateTime? lastDate = null;
                    // Series
                    // Date series
                    output.DataTable.Columns.Add("Date", typeof(DateTime));
                    output.Series.Add(new DataSeries { Name = "Date", Caption = "Date" });
                    // Feature series
                    var features = dataList.Select(i => new DataFeature { Caption = i.FeatureCaption, Name = i.FeatureName }).Distinct().ToList();
                    foreach (var feature in features)
                    {
                        output.DataTable.Columns.Add(feature.Name, typeof(double)).Caption = feature.Caption;
                        output.Series.Add(new DataSeries { Name = feature.Name, Caption = feature.Caption, IsFeature = true });
                    }
                    // Rows
                    List<ExpandoObject> rows = new List<ExpandoObject>();
                    dynamic row = null;
                    DataRow dataRow = null;
                    IDictionary<string, object> rowFeatures = null;
                    bool isNewRow = false;
                    foreach (var data in dataList)
                    {
                        if (lastSite != data.SiteName)
                        {
                            isNewRow = true;
                            lastSite = data.SiteName;
                            lastStation = data.StationName;
                            lastDate = data.ValueDate;
                        }
                        if (!isNewRow && (lastStation != data.StationName))
                        {
                            isNewRow = true;
                            lastStation = data.StationName;
                            lastDate = data.ValueDate;
                        }
                        if (!isNewRow && lastDate.HasValue && (lastDate.Value != data.ValueDate))
                        {
                            isNewRow = true;
                            lastDate = data.ValueDate;
                        }

                        if (isNewRow)
                        {
#pragma warning disable IDE0017 // Simplify object initialization
                            row = new ExpandoObject();
#pragma warning restore IDE0017 // Simplify object initialization
                            row.Date = data.ValueDate;
                            rowFeatures = row as IDictionary<string, object>;
                            foreach (var feature in features)
                            {
                                rowFeatures.Add(feature.Name, new double?());
                            }
                            rows.Add(row);
                            dataRow = output.DataTable.Rows.Add();
                            dataRow["Date"] = data.ValueDate;
                            isNewRow = false;
                        }
                        double? oldValue = (double?)rowFeatures[data.FeatureName];
                        if (oldValue.HasValue && data.Value.HasValue)
                        {
                            rowFeatures[data.FeatureName] = data.Value + oldValue.Value;
                        }
                        else if (!oldValue.HasValue && data.Value.HasValue)
                        {
                            rowFeatures[data.FeatureName] = data.Value;
                        }
                        if (dataRow.IsNull(data.FeatureName))
                        {
                            if (data.Value.HasValue)
                                dataRow[data.FeatureName] = data.Value.Value;
                        }
                        else
                        {
                            dataRow[data.FeatureName] = (double)dataRow[data.FeatureName] + data.Value.Value;
                        }
                            
                    }
                    output.Data.AddRange(rows);
                    //Logging.Verbose("Data: {Data}", result.Data);
                    Logging.Verbose("Result: Cols: {cols} Rows: {rows}", output.Series.Count, rows.Count);
                    return output;
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
