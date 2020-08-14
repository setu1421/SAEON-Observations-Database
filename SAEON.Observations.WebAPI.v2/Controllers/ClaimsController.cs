using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAEON.Logs;
using SAEON.Observations.Core;
using System;
using System.Linq;

namespace SAEON.Observations.WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize(Policy = Constants.ODPAuthorizationPolicy)]
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
        [HttpGet]
        [Authorize(Policy = TenantPolicyDefaults.AuthorizationPolicy)]
        //[Route("ClaimsApiToken")]
        public IActionResult ClaimsWebAPIToken()
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
    }
}
