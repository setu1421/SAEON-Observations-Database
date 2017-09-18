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
    [RoutePrefix("TemporalCoverage")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ClientAuthorization("SAEON.Observations.QuerySite")]
    [RoleAuthorization("SAEON.Observations.Admin")]
    public class TemporalCoverageController : BaseController
    {
        public TemporalCoverageController() : base()
        {
            db.Database.CommandTimeout = 0;
        }

        [HttpPost]
        [Route]
        public async Task<TemporalCoverageOutput> Execute(TemporalCoverageInput input)
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
                    var dataList = await db.vApiTemporalCoverages
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
                    var output = new TemporalCoverageOutput();
                    // Series
                    // Date series
                    output.Series.Add(new DataSeries { Name = "Date", Caption = "Date" });
                    // Feature series
                    var basefeatures = dataList.Select(i => new DataFeature { Caption = i.FeatureCaption, Name = i.FeatureName }).Distinct().ToList();
                    var features = dataList.Select(i => new DataFeature { Caption = i.FeatureCaption, Name = i.FeatureName, Status = i.Status }).Distinct().ToList();
                    foreach (var feature in features)
                    {
                        output.Series.Add(new DataSeries { Name = feature.Name + "_" + feature.Status.Replace(" ", ""), Caption = $"{ basefeatures.IndexOf(basefeatures.Where(i => i.Name == feature.Name).FirstOrDefault()) + 1}-{feature.Caption}-{feature.Status}", IsFeature = true, Status = feature.Status });
                    }
                    // Rows
                    List<ExpandoObject> rows = new List<ExpandoObject>();
                    dynamic row = null;
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
                            foreach (var series in output.Series.Where(i=> i.IsFeature))
                            {
                                rowFeatures.Add(series.Name, new double?());
                            }
                            rows.Add(row);
                            isNewRow = false;
                        }
                        rowFeatures[data.FeatureName + "_" + data.Status.Replace(" ","")] = basefeatures.IndexOf(basefeatures.Where(i => i.Name == data.FeatureName).FirstOrDefault()) + 1;
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