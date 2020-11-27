using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAEON.Logs;
using SAEON.Observations.Auth;
using System;
using System.Threading.Tasks;

namespace SAEON.Observations.QuerySite.Controllers
{
    public class AccountController : BaseController
    {
        [Authorize]
        [HttpGet]
        public IActionResult LogIn()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> LogOutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ClearAccessToken()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    HttpContext.Session.Remove(ODPAuthenticationDefaults.SessionAccessToken);
                    return NoContent();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }

        }

        public IActionResult ClearIdToken()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    HttpContext.Session.Remove(ODPAuthenticationDefaults.SessionIdToken);
                    return NoContent();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public IActionResult ClaimsQuerySite()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var result = HttpContext.UserInfo();
                    SAEONLogs.Information("UserInfo: {@UserInfo}", result);
                    return new JsonResult(result);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [Authorize]
        public IActionResult ClaimsQuerySiteUser()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var result = HttpContext.UserInfo();
                    SAEONLogs.Information("UserInfo: {@UserInfo}", result);
                    return new JsonResult(result);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public async Task<IActionResult> ClaimsWebAPIAsync()
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

        public async Task<IActionResult> ClaimsWebAPIAccessTokenAsync()
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
        public async Task<IActionResult> ClaimsWebAPIIdTokenAsync()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    using (var client = await GetWebAPIClientWithIdTokenAsync())
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

        public async Task<IActionResult> GetBearerToken()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    using (var client = await GetWebAPIClientWithAccessTokenAsync(false))
                    {
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
    }
}
