using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    public class CoverageController : BaseController
    {
        // GET: Coverage
        public ActionResult Index()
        {
            ViewBag.WebAPIUrl = Properties.Settings.Default.WebAPIUrl;
            return View();
        }
    }
}