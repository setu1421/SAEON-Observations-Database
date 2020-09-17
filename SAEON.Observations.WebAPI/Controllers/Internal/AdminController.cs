using Microsoft.AspNetCore.Mvc;
using SAEON.Logs;
using System;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    //[Authorize(Policy = ODPAuthenticationDefaults.AdminTokenPolicy)]
    public class AdminController : BaseController
    {
        //[HttpPost("CreateDynamicDOIs")]
        [HttpPost("[action]")]
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

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateODPMetadata()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return Content(await ODPMetadataHelper.CreateMetadata(DbContext, Config));
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        //[HttpPost("ImportSetup")]
        //public async Task<IActionResult> ImportSetup([FromForm] string fileName)
        //{
        //    using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "FileName", fileName } }))
        //    {
        //        try
        //        {
        //        }
        //        catch (Exception ex)
        //        {
        //            SAEONLogs.Exception(ex);
        //            throw;
        //        }
        //    }
        //}


    }
}
