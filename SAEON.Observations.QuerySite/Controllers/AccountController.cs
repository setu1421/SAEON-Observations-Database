using IdentityModel.Client;
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
        public async Task<ActionResult> CallApi()
        {
            using (Logging.MethodCall(GetType()))
            {
                var token = (User as ClaimsPrincipal)?.FindFirst("access_token")?.Value;
                if (token == null)
                {
                    //var tokenClient = new TokenClient(Properties.Settings.Default.IdentityServerUrl + "/connect/token", "SAEON.Observations.QuerySite", "It6fWPU5J708");
                    //var tokenResponse = await tokenClient.RequestClientCredentialsAsync("SAEON.Observations.WebAPI");
                    var discoClient = new HttpClient();
                    var disco = await discoClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
                    {
                        Address = Properties.Settings.Default.IdentityServerUrl,
                        Policy = { RequireHttps = Properties.Settings.Default.RequireHTTPS && !Request.IsLocal }
                    });

                    if (disco.IsError)
                    {
                        Logging.Error("Error: {error}", disco.Error);
                        throw new HttpException(disco.Error);
                    }
                    var tokenClient = new HttpClient();
                    var tokenResponse = await tokenClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                    {
                        Address = disco.TokenEndpoint,
                        ClientId = "SAEON.Observations.QuerySite",
                        ClientSecret = "It6fWPU5J708",
                        Scope = "SAEON.Observations.WebAPI"
                    });
                    if (tokenResponse.IsError)
                    {
                        Logging.Error("Error: {error}", tokenResponse.Error);
                        throw new HttpException(tokenResponse.Error);
                    }
                    token = tokenResponse.AccessToken;
                }
                Logging.Verbose("Token: {token}", token);
                var client = new HttpClient();
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.SetBearerToken(token);
                var result = await client.GetStringAsync(Properties.Settings.Default.WebAPIUrl + "/claims");
                ViewBag.Json = JArray.Parse(result.ToString());
                return View("ShowApiResult");
            }
        }
    }
}