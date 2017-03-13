using Newtonsoft.Json.Linq;
using SAEON.Observations.Core;
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
    [RoutePrefix("Account")]
    [Route("{action=index}")]
    public class AccountController : Controller
    {
        [Authorize]
        public ActionResult LogIn()
        {
            return this.Redirect("/");
        }

        public ActionResult LogOut()
        {
            this.Request.GetOwinContext().Authentication.SignOut();
            return this.Redirect("/");
        }

        [Authorize, Route("Claims")]
        public ActionResult Claims()
        {
            ViewBag.Message = "Claims";

            var cp = (ClaimsPrincipal)User;
            ViewData["access_token"] = cp.FindFirst("access_token").Value;

            return View();
        }

        [Authorize]
        public async Task<ActionResult> CallApi()
        {
            using (Logging.MethodCall(this.GetType()))
            {
                var token = (User as ClaimsPrincipal).FindFirst("access_token").Value;
                var client = new HttpClient();
                client.SetBearerToken(token);
                var result = await client.GetStringAsync("http://localhost:63378/sites");
                ViewBag.Json = JArray.Parse(result.ToString());
                return View("ShowApiResult");
            }
        }
    }
}