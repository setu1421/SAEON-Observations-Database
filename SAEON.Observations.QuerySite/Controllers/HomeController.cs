using SAEON.AspNet.Common;
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
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Route("Contact")]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Route("HowToCite")]
        public ActionResult HowToCite()
        {
            return View();
        }

        [Route("SetTenant/{Name}")]
        public ActionResult SetTenant(string Name)
        {
            using (Logging.MethodCall(GetType(), new MethodCallParameters { { "Name", Name } }))
            {
                Session[AspNetConstants.TenantSession] = Name;
                return RedirectToAction("Index", "Home");
            }
        }
    }
}