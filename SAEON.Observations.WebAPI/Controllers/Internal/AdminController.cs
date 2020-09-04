using Microsoft.AspNetCore.Mvc;
using SAEON.Logs;
using System;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    //[Authorize(Policy = ODPAuthenticationDefaults.AdminTokenPolicy)]
    public class AdminController : BaseController
    {
        [HttpPost]
        [Route("CreateDynamicDOIs")]
        public async Task<IActionResult> CreateDynamicDOIs()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return Content(await DOIHelper.CreateDOIs(DbContext, HttpContext));
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
