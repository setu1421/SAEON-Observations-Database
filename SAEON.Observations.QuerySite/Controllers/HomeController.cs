using SAEON.Logs;
using System.Web.Mvc;
using System.Web.UI;

namespace SAEON.Observations.QuerySite.Controllers
{
#if ResponseCaching
    [OutputCache(Duration = Defaults.CacheDuration)]
#endif
    public class HomeController : BaseController
    {
        [OutputCache(Duration = 0, Location = OutputCacheLocation.None, NoStore = true)]
        public ActionResult Index()
        {
            return View();
        }

        [Route("About")]
        public ActionResult About()
        {
            return View();
        }

        [Route("Contact")]
        public ActionResult Contact()
        {
            return View();
        }

        [Route("HowToCite")]
        public ActionResult HowToCite()
        {
            return View();
        }

        [Route("SetTenant/{Name}")]
        [OutputCache(Duration = 0, Location = OutputCacheLocation.None, NoStore = true)]
        public ActionResult SetTenant(string name)
        {
            using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "Name", name } }))
            {
                SAEONLogs.Verbose("Tenant: {Tenant}", name);
                Session[Constants.SessionKeyTenant] = name;
                return RedirectToAction("Index", "Home");
            }
        }
    }
}