using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.WebAPI.Filters;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    [RoutePrefix("DataDownload")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ClientId("SAEON.Observations.QuerySite")]
    public class DataDownloadController : BaseController
    {
        public DataDownloadController() : base()
        {
            db.Database.CommandTimeout = 0;
        }

        [HttpPost]
        [Route]
        public async Task<DataDownloadOutput> DataDownload(DataDownloadInput input)
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
                    var dataList = await db.vApiDataDownloads
                        .Where(i => input.Stations.Contains(i.StationId))
                        .Where(i => input.PhenomenaOfferings.Contains(i.PhenomenonOfferingId))
                        .Where(i => i.ValueDay >= input.StartDate)
                        .Where(i => i.ValueDay <= input.EndDate)
                        .OrderBy(i => i.SiteName)
                        .ThenBy(i => i.StationName)
                        //.ThenBy(i => i.InstrumentName)
                        //.ThenBy(i => i.SensorName)
                        .ThenBy(i => i.ValueDate)
                        .ToListAsync();
                    Logging.Verbose("DataList: {count}", dataList.Count);
                    var result = new DataDownloadOutput();
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
