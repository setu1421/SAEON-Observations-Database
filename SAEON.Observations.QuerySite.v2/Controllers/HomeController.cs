using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.QuerySite.Models;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SAEON.Observations.QuerySite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
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
            using (Logging.MethodCall(GetType(), new MethodCallParameters { { "Name", Name } }))
            {
                HttpContext.Session.SetString(Constants.TenantSession, Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public IActionResult Secure()
        {
            ViewData["Message"] = "Secure page.";

            return View();
        }

        public async Task<IActionResult> CallApi()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var content = await client.GetStringAsync(_config["IdentityServerUrl"]);

            ViewBag.Json = JArray.Parse(content).ToString();
            return View("json");
        }

    }
}
