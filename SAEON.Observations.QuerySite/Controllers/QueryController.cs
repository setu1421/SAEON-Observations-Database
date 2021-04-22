using System.Configuration;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    public class QueryController : BaseController
    {
        public async Task<ActionResult> Datasets()
        {
            ViewBag.Authorization = await GetAuthorizationAsync();
            ViewBag.Tenant = Tenant;
            ViewBag.WebAPIUrl = ConfigurationManager.AppSettings["WebAPIUrl"];
            //SAEONLogs.Information("Authorization: {Authorization} Tenant: {Tenant}", ViewBag.Authorization, ViewBag.Tenant);
            return View();
        }

        public async Task<ActionResult> Sensors()
        {
            ViewBag.Authorization = await GetAuthorizationAsync();
            ViewBag.Tenant = Tenant;
            ViewBag.WebAPIUrl = ConfigurationManager.AppSettings["WebAPIUrl"];
            //SAEONLogs.Information("Authorization: {Authorization} Tenant: {Tenant}", ViewBag.Authorization, ViewBag.Tenant);
            return View();
        }
    }
}