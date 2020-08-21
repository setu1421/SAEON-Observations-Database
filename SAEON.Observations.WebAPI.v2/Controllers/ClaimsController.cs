#define UseODP
#if !UseODP
using IdentityModel.AspNetCore.OAuth2Introspection;
#endif
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.Core.Authentication;
using System;

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
                    var result = HttpContext.GetUserInfo();
                    SAEONLogs.Information("UserInfo: {UserInfo}", result);
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
#if UseODP
        [Authorize(AuthenticationSchemes = ODPAccessTokenAuthenticationDefaults.AuthenticationScheme + "," + TenantAuthenticationDefaults.AuthenticationScheme)]
#else
        [Authorize(AuthenticationSchemes = OAuth2IntrospectionDefaults.AuthenticationScheme + "," + TenantAuthenticationDefaults.AuthenticationScheme)]
#endif
        public IActionResult ClaimsWebAPIAccessToken()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    //SAEONLogs.Information("Token: {Token}", HttpContext.Request.GetBearerToken());
                    var result = HttpContext.GetUserInfo();
                    SAEONLogs.Information("UserInfo: {UserInfo}", result);
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
        [Authorize(AuthenticationSchemes = TenantAuthenticationDefaults.AuthenticationScheme)]
#if UseODP
        [Authorize(AuthenticationSchemes = ODPIdTokenAuthenticationDefaults.AuthenticationScheme)]
#else
        [Authorize(AuthenticationSchemes = OAuth2IntrospectionDefaults.AuthenticationScheme)]
#endif
        public IActionResult ClaimsWebAPIIdToken()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    //SAEONLogs.Information("Token: {Token}", HttpContext.Request.GetBearerToken());
                    var result = HttpContext.GetUserInfo();
                    SAEONLogs.Information("UserInfo: {UserInfo}", result);
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
                    var token = HttpContext.Request.GetBearerToken();
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
