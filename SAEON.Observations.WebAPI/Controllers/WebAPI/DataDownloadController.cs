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
    [RoutePrefix("DataDownload")]
    [ApiExplorerSettings(IgnoreApi = true)]
    //[ResourceAuthorize("Observations.QuerySite", "DataDownload")]
    //[ClaimsAuthorization("client_id","SAEON.Observations.QuerySite")]
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
                    if (input.Offerings == null) throw new ArgumentNullException("input.Offerings");
                    if (!input.Offerings.Any()) throw new ArgumentOutOfRangeException("input.Offerings");
                    var dataList = await db.VDownloads
                        .Where(i => input.Stations.Contains(i.StationId))
                        .Where(i => input.Offerings.Contains(i.PhenomenonOfferingId))
                        .Where(i => i.Date >= input.StartDate)
                        .Where(i => i.Date <= input.EndDate)
                        .OrderBy(i => i.SiteName)
                        .ThenBy(i => i.StationName)
                        .ThenBy(i => i.InstrumentName)
                        .ThenBy(i => i.Date)
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
