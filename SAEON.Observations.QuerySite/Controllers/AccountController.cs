using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using SAEON.Logs;
using System;
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
            return Redirect("/");
        }

        public ActionResult LogOut()
        {
            //Request.GetOwinContext().Authentication.SignOut("Cookies");
            Request.GetOwinContext().Authentication.SignOut();
            return Redirect("/");
        }

        public ActionResult Register()
        {
            Logging.Information("ReturnUrl: {returnUrl}", Properties.Settings.Default.IdentityServerUrl + $"/Account/Register?returnUrl={Properties.Settings.Default.QuerySiteUrl}");
            return Redirect(Properties.Settings.Default.IdentityServerUrl + $"/Account/Register?returnUrl={Properties.Settings.Default.QuerySiteUrl}");
        }

        [Authorize]
        public ActionResult Claims()
        {
            ViewBag.Message = "Claims";

            var cp = (ClaimsPrincipal)User;
            ViewData["access_token"] = cp?.FindFirst("access_token")?.Value;

            return View();
        }

        [Authorize]
#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
        public async Task<ActionResult> CallApi()
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
        {
            using (Logging.MethodCall(GetType()))
            {
                var client = await WebAPIClient.GetWebAPIClientAsync(Request, Session, (ClaimsPrincipal)User , Request.IsLocal);
                Logging.Verbose("Call: {url}", Properties.Settings.Default.WebAPIUrl + "/claims");
                var result = await client.GetStringAsync(Properties.Settings.Default.WebAPIUrl + "/claims");
                ViewBag.Json = JArray.Parse(result);
                return View("ShowApiResult");
            }
        }
    }
}