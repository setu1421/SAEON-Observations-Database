using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAEON.Logs;
using SAEON.Observations.Core;
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
        [Authorize(AuthenticationSchemes = OAuth2IntrospectionDefaults.AuthenticationScheme, Policy = TenantPolicyDefaults.AuthorizationPolicy)]
        public IActionResult ClaimsWebAPIToken()
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
    }
}
