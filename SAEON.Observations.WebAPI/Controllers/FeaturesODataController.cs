using System;
using System.Linq;
using System.Web.OData;
using System.Web.OData.Routing;
using SAEON.Observations.Core;
using System.Net.Http;
using System.Web.Http.Description;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers
{
    [ODataRoutePrefix("Features")]
    [ApiExplorerSettings(IgnoreApi = true)]
    //[Authorize]
    public class FeaturesODataController : ODataController
    {
        ObservationsDbContext db = new ObservationsDbContext();

        [EnableQuery, ODataRoute]
        public IQueryable<Feature> GetAll()
        {
            using (Logging.MethodCall<Feature>(GetType()))
            {
                try
                {
                    Logging.Verbose("Request.Uri: {uri}", Request.RequestUri);
                    Logging.Verbose("QueryString: {querystring}", string.Join(", ", Request.GetQueryNameValuePairs().Select(kv => $"{kv.Key}: {kv.Value}")));
                    return FeaturesHelper.GetFeatures(db);
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
