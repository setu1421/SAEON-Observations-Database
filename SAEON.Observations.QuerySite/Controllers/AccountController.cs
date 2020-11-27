using System.Web;
using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    [RoutePrefix("Account")]
    [Route("{action=index}")]
    public class AccountController : BaseController
    {
        [Authorize]
        public ActionResult LogIn()
        {
            return Redirect("/");
        }

        public ActionResult LogOut()
        {
            Request.GetOwinContext().Authentication.SignOut("Cookies");
            //Request.GetOwinContext().Authentication.SignOut();
            return Redirect("/");
        }

        public ActionResult Register()
        {
            //SAEONLogs.Information("ReturnUrl: {returnUrl}", Properties.Settings.Default.IdentityServerUrl + $"/Account/Register?returnUrl={Properties.Settings.Default.QuerySiteUrl}");
            //return Redirect(Properties.Settings.Default.IdentityServerUrl + $"/Account/Register?returnUrl={Properties.Settings.Default.QuerySiteUrl}");
            return Redirect("/");
        }

        /*
        //[Authorize]
        public ActionResult Claims()
        {
            ViewBag.Message = "Claims";
            if (User is ClaimsPrincipal cp)
            {
                ViewData["access_token"] = cp?.FindFirst("access_token")?.Value;
            }
            return View();
        }

        [Authorize]
        public async Task<ActionResult> CallApi()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                using (var client = await GetWebAPIClientWithAccessTokenAsync())
                {
                    //SAEONLogs.Verbose("Call: {url}", Properties.Settings.Default.WebAPIUrl + "/claims");
                    var result = await client.GetStringAsync(ConfigurationManager.AppSettings["WebAPIUrl"].AddTrailingForwardSlash() + "ClaimsWebAPI");
                    ViewBag.Json = JArray.Parse(result);
                    return View("ShowApiResult");
                }
            }
        }

        public ActionResult ClearAccessToken()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    Session.Remove(Constants.SessionKeyAccessToken);
                    return new EmptyResult();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }

        }

        public ActionResult ClearIdToken()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    Session.Remove(Constants.SessionKeyIdToken);
                    return new EmptyResult();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }

        }

        public ActionResult ClaimsQuerySite()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var result = HttpContext.UserInfo();
                    SAEONLogs.Information("UserInfo: {@UserInfo}", result);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [Authorize]
        public ActionResult ClaimsQuerySiteUser()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var result = HttpContext.UserInfo();
                    SAEONLogs.Information("UserInfo: {@UserInfo}", result);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public async Task<ActionResult> ClaimsWebAPI()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    using (var client = GetWebAPIClient())
                    {
                        var response = await client.GetAsync("Claims/ClaimsWebAPI");
                        if (!response.IsSuccessStatusCode)
                        {
                            SAEONLogs.Error("HttpError: {StatusCode} {Reason}", response.StatusCode, response.ReasonPhrase);
                            SAEONLogs.Error("Response: {Response}", await response.Content.ReadAsStringAsync());
                        }
                        response.EnsureSuccessStatusCode();
                        var claims = await response.Content.ReadAsStringAsync();
                        SAEONLogs.Information("UserInfo: {@UserInfo}", claims);
                        return Content(claims);
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }

        }

        public async Task<ActionResult> ClaimsWebAPIAccessToken()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    using (var client = await GetWebAPIClientWithAccessTokenAsync())
                    {
                        SAEONLogs.Verbose("Calling WebAPI");
                        var response = await client.GetAsync("Claims/ClaimsWebAPIAccessToken");
                        if (!response.IsSuccessStatusCode)
                        {
                            SAEONLogs.Error("HttpError: {StatusCode} {Reason}", response.StatusCode, response.ReasonPhrase);
                            SAEONLogs.Error("Response: {Response}", await response.Content.ReadAsStringAsync());
                        }
                        response.EnsureSuccessStatusCode();
                        var claims = await response.Content.ReadAsStringAsync();
                        SAEONLogs.Information("UserInfo: {@UserInfo}", claims);
                        return Content(claims);
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [Authorize]
        public async Task<ActionResult> ClaimsWebAPIIdToken()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    using (var client = GetWebAPIClientWithIdToken())
                    {
                        var response = await client.GetAsync("Claims/ClaimsWebAPIIdToken");
                        if (!response.IsSuccessStatusCode)
                        {
                            SAEONLogs.Error("HttpError: {StatusCode} {Reason}", response.StatusCode, response.ReasonPhrase);
                            SAEONLogs.Error("Response: {Response}", await response.Content.ReadAsStringAsync());
                        }
                        response.EnsureSuccessStatusCode();
                        var claims = await response.Content.ReadAsStringAsync();
                        SAEONLogs.Information("UserInfo: {@UserInfo}", claims);
                        return Content(claims);
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public async Task<ActionResult> GetBearerToken()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    using (var client = await GetWebAPIClientWithAccessTokenAsync(false))
                    {
                        //client.SetBearerToken("ag3JOPT14XGbSM9C4QUwkNTolO0PgHijZLiEfKbPPW0.Y9VI5WMTdh_oKV2Bj0PW4UYiyUqgCPBF4fEkaeQJRpw");
                        var response = await client.GetAsync("Claims/GetBearerToken");
                        if (!response.IsSuccessStatusCode)
                        {
                            SAEONLogs.Error("HttpError: {StatusCode} {Reason}", response.StatusCode, response.ReasonPhrase);
                            SAEONLogs.Error("Response: {Response}", await response.Content.ReadAsStringAsync());
                        }
                        response.EnsureSuccessStatusCode();
                        var token = await response.Content.ReadAsStringAsync();
                        SAEONLogs.Information("BearerToken: {BearerToken}", token);
                        return Content(token);
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }

        }
        */
    }
}