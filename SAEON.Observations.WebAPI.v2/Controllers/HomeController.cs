using Microsoft.AspNetCore.Mvc;
using SAEON.Observations.WebAPI.Models;
using System.Diagnostics;

namespace SAEON.Observations.WebAPI.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
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

        [Route("GraphQL")]
        public IActionResult GraphQL()
        {
            return View();
        }

        [Route("HowToCite")]
        public IActionResult HowToCite()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
