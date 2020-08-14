using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.QuerySite.Controllers;
using System.Diagnostics;

namespace SAEON.Observations.QuerySite.Models
{
    public class HomeController : BaseController
    {

        public HomeController(IHttpContextAccessor httpContextAccessor, IConfiguration config) : base(httpContextAccessor, config) { }

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

        [Route("SetTenant/{Name}")]
        public IActionResult SetTenant(string Name)
        {
            using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "Name", Name } }))
            {
                SAEONLogs.Information("Tenant: {Tenant}", Name);
                HttpContext.Session.SetString(TenantPolicyDefaults.HeaderKeyTenant, Name);
                SAEONLogs.Information("Session: {Tenant}", HttpContext.Session.GetString(TenantPolicyDefaults.HeaderKeyTenant));
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
