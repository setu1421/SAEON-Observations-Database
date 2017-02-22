using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{

    public class HomeController : Controller
    {
        [Route]
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

        [Authorize, Route("Claims")]
        public ActionResult Claims()
        {
            ViewBag.Message = "Claims";

            var cp = (ClaimsPrincipal)User;
            ViewData["access_token"] = cp.FindFirst("access_token").Value;

            return View();
        }

        //[Authorize]
        //public async Task<ActionResult> CallApi()
        //{
        //    var token = (User as ClaimsPrincipal).FindFirst("access_token").Value;

        //    var client = new HttpClient();
        //    client.SetBearerToken(token);

        //    var result = await client.GetStringAsync(Constants.AspNetWebApiSampleApi + "identity");
        //    ViewBag.Json = JArray.Parse(result.ToString());

        //    return View();
        //}

    }
}