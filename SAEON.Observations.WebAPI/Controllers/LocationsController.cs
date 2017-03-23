using SAEON.Observations.Core;
using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers
{
    [RoutePrefix("Locations")]
    public class LocationsController : ApiController
    {
        ObservationsDbContext db = new ObservationsDbContext();

        [HttpGet]
        [Route]
        public IQueryable<Location> GetAll()
        {
            using (Logging.MethodCall<Location>(this.GetType()))
            {
                try
                {
                    Logging.Verbose("Request.Uri: {uri}", Request.RequestUri);
                    Logging.Verbose("QueryString: {querystring}", string.Join(", ", Request.GetQueryNameValuePairs().Select(kv => $"{kv.Key}: {kv.Value}")));
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
