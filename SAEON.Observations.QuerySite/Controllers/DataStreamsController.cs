using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    public class DataStreamsController : BaseController
    {
        // GET: DataStreams
        public ActionResult Index()
        {
            ViewBag.WebAPIUrl = Properties.Settings.Default.WebAPIUrl;
            return View();
        }
    }
}