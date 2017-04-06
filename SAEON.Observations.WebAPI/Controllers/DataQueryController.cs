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


        [HttpPost]
        //[Route("{stationIds}/{phenomenonOfferingIds}/{startDate:datetime?}/{enddate:datetime?}")]
        [Route]
        public DataQueryOutput DataQuery(DataQueryInput input)
        {
            using (Logging.MethodCall(this.GetType(), new ParameterList { { "Params", input } }))
            {
                try
                {
                    Logging.Verbose("Parameters: {@parameters}", input);
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
                    Logging.Verbose("DataList: {@dataList}", dataList);
                    string lastSite = null;
                    string lastStation = null;
                    var result = new DataQueryOutput
                    {
                        Columns = new List<string> { "Site", "Station" }
                    };
                    dynamic row = null;
                    bool isNewRow = false;
                    foreach (var data in dataList)
                    {
                        if (lastSite != data.SiteName)
                        {
                            isNewRow = true;
                            lastSite = data.SiteName;
                        }
                        if (!isNewRow && (lastStation != data.StationName))
                        {
                            isNewRow = true;
                            lastStation = data.StationName;
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
