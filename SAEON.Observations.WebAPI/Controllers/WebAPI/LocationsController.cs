using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using SAEON.Observations.WebAPI.Filters;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    [RoutePrefix("Locations")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ClientId("SAEON.Observations.QuerySite")]
    public class LocationsController : ApiController
    {
        ObservationsDbContext db = new ObservationsDbContext();

        [HttpGet]
        [Route]
        public IQueryable<Location> GetAll()
        {
            using (Logging.MethodCall<Location>(GetType()))
            {
                try
                {
                    Logging.Verbose("Request.Uri: {uri}", Request.RequestUri);
                    //Logging.Verbose("QueryString: {querystring}", string.Join(", ", Request.GetQueryNameValuePairs().Select(kv => $"{kv.Key}: {kv.Value}")));
                    return LocationsHelper.GetLocations(db);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get all");
                    throw;
                }
            }
        }
    }
}
