using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SAEON.AspNet.Auth;
using SAEON.Core;
using SAEON.Logs;
using SAEON.Observations.WebAPI.Hubs;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    [Authorize(Policy = ODPAuthenticationDefaults.AdminTokenPolicy)]
    public class AdminController : InternalApiController
    {
        private readonly IHubContext<AdminHub> AdminHub;

        public AdminController(IHubContext<AdminHub> webApiHub)
        {
            AdminHub = webApiHub;
            TrackChanges = true;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateDOIs()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return Content(await DOIHelper.CreateDOIsV2(DbContext, AdminHub, HttpContext, AnalyticsHelper.IsTest(Request)));
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
                    return Content(await MetadataHelper.CreateMetadataV2(DbContext, AdminHub, AnalyticsHelper.IsTest(Request)));
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
                    return Content(await ODPMetadataHelper.CreateMetadata(DbContext, AdminHub, Config));
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ImportSetup(IFormFile formFile)
        {
            using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "FileName", formFile?.FileName } }))
            {
                try
                {
                    if (formFile == null) throw new ArgumentNullException(nameof(formFile));
                    if (formFile.Length == 0) throw new ArgumentOutOfRangeException(nameof(formFile), "File length cannot be zero");
                    var ext = Path.GetExtension(formFile.FileName).ToLowerInvariant();
                    if (!(ext == ".xls" || ext == ".xlsx")) throw new ArgumentOutOfRangeException(nameof(formFile), "Invalid file extension");
                    ImportSetupHelper.UpdateData = Config["ImportSetupUpdateData"].IsTrue();
                    SAEONLogs.Information("ImportSetup: {FileName}", formFile.FileName);
                    return Content(await ImportSetupHelper.ImportFromSpreadsheet(DbContext, AdminHub, formFile));
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        //[HttpGet("[action]")]
        //public async Task<IActionResult> GetMetadataRecords()
        //{
        //    using (SAEONLogs.MethodCall(GetType()))
        //    {
        //        try
        //        {
        //            return Content(await ODPMetadataHelper.GetMetadataRecords(Config));
        //        }
        //        catch (Exception ex)
        //        {
        //            SAEONLogs.Exception(ex);
        //            throw;
        //        }
        //    }

        //}

        //[HttpGet("[action]")]
        //public async Task<IActionResult> UnpublishMetadataRecords()
        //{
        //    using (SAEONLogs.MethodCall(GetType()))
        //    {
        //        try
        //        {
        //            return Content(await ODPMetadataHelper.UnpublishMetadataRecords(Config));
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
