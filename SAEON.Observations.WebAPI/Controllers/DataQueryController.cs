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

        /// <summary>
        /// Get
        /// </summary>
        /// <returns>Json</returns>
        [HttpPost]
        //[Route("{stationIds}/{phenomenonOfferingIds}/{startDate:datetime?}/{enddate:datetime?}")]
        [Route]
        public List<object> GetDataQuery([FromBody] string stationIds, [FromBody] string phenomenonOfferingIds, [FromBody] DateTime? startDate, [FromBody] DateTime? endDate)
        {
            using (Logging.MethodCall(this.GetType(), new ParameterList {
                { "StationIds", stationIds },
                { "PhenomenonOfferingIds", phenomenonOfferingIds },
                { "StartDate", startDate },
                { "EndDate", endDate } }))
            {
                try
                {
                    db.Configuration.AutoDetectChangesEnabled = false;
                    var query = db.VDownloads.AsQueryable();
                    if (!string.IsNullOrEmpty(stationIds))
                    {
                        List<string> stations = stationIds.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries).ToList();
                        query = query.Where(i => stations.Contains(i.StationId.ToString()));
                    }
                    if (!string.IsNullOrEmpty(phenomenonOfferingIds))
                    {
                        List<string> offerings = phenomenonOfferingIds.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries).ToList();
                        query = query.Where(i => offerings.Contains(i.PhenomenonOfferingId.ToString()));
                    }
                    if (startDate.HasValue)
                    {
                        query = query.Where(i => i.Date >= startDate.Value);
                    }
                    if (endDate.HasValue)
                    {
                        query = query.Where(i => i.Date <= endDate.Value);
                    }
                    query = query
                        .OrderBy(i => i.SiteName)
                        .ThenBy(i => i.StationName)
                        .ThenBy(i => i.Date);
                    var dataList = query.ToList();
                    string lastSite = null;
                    string lastStation = null;
                    var result = new List<object>();
                    dynamic row = null;
                    foreach (var data in dataList)
                    {
                        bool isNewRow = false;
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
                                result.Add(row);
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
                    Logging.Exception(ex, "Unable to get");
                    throw;
                }
            }

        }
    }
}
