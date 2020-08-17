using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SAEON.Logs;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SAEON.Observations.QuerySite.v2.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _config;

        public AccountController(ILogger<AccountController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [Authorize]
        public ActionResult LogIn()
        {
            return Redirect("/");
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }

        public ActionResult Register()
        {
            _logger.LogInformation("ReturnUrl: {returnUrl}", _config["IdentityServerUrl"] + $"/Account/Register?returnUrl={_config["QuerySiteUrl"]}");
            Logging.Information("ReturnUrl: {returnUrl}", _config["IdentityServerUrl"] + $"/Account/Register?returnUrl={_config["QuerySiteUrl"]}");
            return Redirect(_config["IdentityServerUrl"] + $"/Account/Register?returnUrl={_config["QuerySiteUrl"]}");
        }

        [Authorize]
        public ActionResult Claims()
        {
            ViewBag.Message = "Claims";
            return View();
        }

        public async Task<IActionResult> CallApi()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var content = await client.GetStringAsync(_config["WebAPIUrl"]+"/Claims");
            ViewBag.Json = JArray.Parse(content).ToString();
            return View("json");
        }


    }
}