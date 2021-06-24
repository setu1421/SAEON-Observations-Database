using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAEON.Logs;
using SAEON.Observations.Auth;
using System;
using auth = SAEON.AspNet.Auth;

namespace SAEON.Observations.WebAPI.Controllers
{
    //[Route("[controller]")]
    [Route("[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ClaimsController : ControllerBase
    {

        [HttpGet]
        public IActionResult ClaimsWebAPI()
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

        [HttpGet]
        [Authorize(Policy = TenantAuthenticationDefaults.TenantPolicy)]
        [Authorize(Policy = ODPAuthenticationDefaults.AccessTokenPolicy)]
        public IActionResult ClaimsWebAPIAccessToken()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    //SAEONLogs.Information("Token: {Token}", HttpContext.Request.GetBearerToken());
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

        [HttpGet]
        [Authorize(Policy = TenantAuthenticationDefaults.TenantPolicy)]
        [Authorize(Policy = ODPAuthenticationDefaults.IdTokenPolicy)]
        public IActionResult ClaimsWebAPIIdToken()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    //SAEONLogs.Information("Token: {Token}", HttpContext.Request.GetBearerToken());
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

        public IActionResult GetBearerToken()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {

                    var token = auth.HttpRequestExtensions.GetBearerToken(HttpContext.Request);
                    SAEONLogs.Information("Token: {Token}", token);
                    return new JsonResult(token);
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
