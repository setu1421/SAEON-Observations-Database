using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAEON.Logs;
using SAEON.Observations.QuerySite.Controllers;
using System.Diagnostics;

namespace SAEON.Observations.QuerySite.Models
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("About")]
        public IActionResult About()
        {
            return View();
        }

        [Route("Contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [Route("DataUsage")]
        public IActionResult DataUsage()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("HowToCite")]
        public IActionResult HowToCite()
        {
            return View();
        }

        [Route("SetTenant/{Tenant}")]
        public IActionResult SetTenant(string tenant)
        {
            using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { nameof(tenant), tenant } }))
            {
                SAEONLogs.Information("Tenant: {Tenant}", tenant);
                Tenant = tenant;
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
