using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using SAEON.Logs;
using System;
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

            Request.GetOwinContext().Authentication.SignOut(OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
            return Redirect("/");
        }

        public ActionResult Register()
        {
            if (!Request.IsAuthenticated)
            {
                var properties = new AuthenticationProperties { RedirectUri = "/" };
                properties.Dictionary.Add("ODPRegister", "true");
                Request.GetOwinContext().Authentication.Challenge(properties, OpenIdConnectAuthenticationDefaults.AuthenticationType);
                return new HttpUnauthorizedResult();
            }
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
        */

        public ActionResult Claims()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var result = HttpContext.ApplicationInstance.Context.UserInfo();
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
        public ActionResult ClaimsAuthorized()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var result = HttpContext.ApplicationInstance.Context.UserInfo();
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

        /*
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