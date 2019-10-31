using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SAEON.AspNet.Common;
using SAEON.Logs;
using SAEON.Observations.QuerySite.Models;
using System.Diagnostics;

namespace SAEON.Observations.QuerySite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("SetTenant/{Name}")]
        public ActionResult SetTenant(string Name)
        {
            using (Logging.MethodCall(GetType(), new ParameterList { { "Name", Name } }))
            {
                HttpContext.Session.SetString(Constants.TenantSession, Name);
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
