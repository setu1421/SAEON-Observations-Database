using Newtonsoft.Json.Linq;
using SAEON.Logs;
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
            Request.GetOwinContext().Authentication.SignOut();
            return Redirect("/");
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
            using (Logging.MethodCall(GetType()))
            {
                var token = (User as ClaimsPrincipal).FindFirst("access_token").Value;
                var client = new HttpClient();
                client.SetBearerToken(token);
                var result = await client.GetStringAsync("http://localhost:63378/claims");
                ViewBag.Json = JArray.Parse(result.ToString());
                return View("ShowApiResult");
            }
        }
    }
}