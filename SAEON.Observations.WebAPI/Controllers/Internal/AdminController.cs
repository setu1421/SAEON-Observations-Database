using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAEON.Logs;
using System;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    //[Authorize(Policy = ODPAuthenticationDefaults.AdminTokenPolicy)]
    public class AdminController : InternalApiController
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateDOIs()
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
        public async Task<IActionResult> CreateMetadata()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return Content(await MetadataHelper.CreateMetadata(DbContext));
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

        [HttpPost("ImportSetup")]
        public async Task<IActionResult> ImportSetup(IFormFile fileData)
        {
            using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "FileName", fileData?.FileName } }))
            {
                try
                {
                    if (fileData == null) throw new ArgumentNullException(nameof(fileData));
                    SAEONLogs.Information("ImportSetup: {FileName}", fileData.FileName);
                    return Content(await ImportSetupHelper.ImportFromSpreadsheet(DbContext, fileData));
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
