using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SAEON.Logs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SAEON.Observations.QuerySite.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(IHttpContextAccessor httpContextAccessor, IConfiguration config) : base(httpContextAccessor, config) { }

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

        //[Authorize]
        public IActionResult ClaimsQuerySite()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var result = new
                    {
                        User,
                        User.Identity.IsAuthenticated,
                        Claims = User.Claims.Select(c => new { c.Type, c.Value })
                    };
                    SAEONLogs.Information("Claims: {claims}", result);
                    return new JsonResult(result);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }
        public async Task<IActionResult> ClaimsWeBAPIAsync()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    using (var client = await GetWebAPIClient())
                    {
                        var response = await client.GetAsync("ClaimsWebAPI");
                        if (!response.IsSuccessStatusCode)
                        {
                            SAEONLogs.Error("HttpError: {StatusCode} {Reason}", response.StatusCode, response.ReasonPhrase);
                            SAEONLogs.Error("Response: {Response}", await response.Content.ReadAsStringAsync());
                        }
                        response.EnsureSuccessStatusCode();
                        var claims = await response.Content.ReadAsStringAsync();
                        SAEONLogs.Information("Claims: {Claims}", claims);
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

        public async Task<IActionResult> ClaimsWeBAPITokenAsync()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    using (var client = await GetWebAPIClient())
                    {
                        var response = await client.GetAsync("ClaimsWebAPIToken");
                        if (!response.IsSuccessStatusCode)
                        {
                            SAEONLogs.Error("HttpError: {StatusCode} {Reason}", response.StatusCode, response.ReasonPhrase);
                            SAEONLogs.Error("Response: {Response}", await response.Content.ReadAsStringAsync());
                        }
                        response.EnsureSuccessStatusCode();
                        var claims = await response.Content.ReadAsStringAsync();
                        SAEONLogs.Information("Claims: {Claims}", claims);
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
    }
}
