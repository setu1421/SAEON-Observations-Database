using SAEON.Logs;
using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    public class HomeController : Controller
    {
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