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
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    //[Authorize(Policy = ODPAuthenticationDefaults.AdminTokenPolicy)]
    public class AdminController : InternalApiController
    {
        private readonly IHubContext<AdminHub> AdminHub;

        public AdminController(IHubContext<AdminHub> webApiHub) : base()
        {
            AdminHub = webApiHub;
            TrackChanges = true;
            CommandTimeoutKey = "AdminCommandTimeoutInMins";
        }

        [HttpPost("[action]")]
        [Authorize(Policy = ODPAuthenticationDefaults.AdminTokenPolicy)]
        public async Task<IActionResult> CreateDOIs()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return Content(await DOIHelper.CreateDOIs(DbContext, AdminHub, HttpContext/*, AnalyticsHelper.IsTest(Request)*/));
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpPost("[action]")]
        [Authorize(Policy = ODPAuthenticationDefaults.AdminTokenPolicy)]
        public async Task<IActionResult> CreateMetadata()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return Content(await MetadataHelper.CreateMetadata(DbContext, AdminHub, HttpContext/*, AnalyticsHelper.IsTest(Request)*/));
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpPost("[action]")]
        [Authorize(Policy = ODPAuthenticationDefaults.AdminTokenPolicy)]
        public async Task<IActionResult> CreateODPMetadata()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return Content(await ODPMetadataHelper.CreateODPMetadata(DbContext, AdminHub, HttpContext, Config));
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpPost("[action]")]
        [Authorize(Policy = ODPAuthenticationDefaults.AdminTokenPolicy)]
        public async Task<IActionResult> CreateDatasets()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return Content(await DatasetHelper.CreateDatasets(DbContext, AdminHub, HttpContext, Config));
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpPost("[action]")]
        [Authorize(Policy = ODPAuthenticationDefaults.AdminTokenPolicy)]
        public async Task<IActionResult> CreateImportBatchSummaries()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return Content(await ImportBatchSummaryHelper.CreateImportBatchSummaries(DbContext, AdminHub));
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpPost("[action]")]
        [Authorize(Policy = ODPAuthenticationDefaults.AdminTokenPolicy)]
        public async Task<IActionResult> CreateSnapshots()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return Content(await SnapshotHelper.CreateSnapshots(DbContext, AdminHub));
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpPost("[action]")]
        [Authorize(Policy = ODPAuthenticationDefaults.AccessTokenPolicy)]
        public async Task<IActionResult> DailyUpdate()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var sb = new StringBuilder();
                    sb.AppendLine(await ImportBatchSummaryHelper.CreateImportBatchSummaries(DbContext, AdminHub));
                    sb.AppendLine(await DOIHelper.CreateDOIs(DbContext, AdminHub, HttpContext));
                    sb.AppendLine(await MetadataHelper.CreateMetadata(DbContext, AdminHub, HttpContext));
                    sb.AppendLine(await DatasetHelper.CreateDatasets(DbContext, AdminHub, HttpContext, Config));
                    sb.AppendLine(await ODPMetadataHelper.CreateODPMetadata(DbContext, AdminHub, HttpContext, Config));
                    sb.AppendLine(await SnapshotHelper.CreateSnapshots(DbContext, AdminHub));
                    return Content(sb.ToString());
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpPost("[action]")]
        [Authorize(Policy = ODPAuthenticationDefaults.AccessTokenPolicy)]
        public async Task<IActionResult> HourlyUpdate()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var sb = new StringBuilder();
                    sb.AppendLine(await DOIHelper.CreateDOIs(DbContext, AdminHub, HttpContext));
                    sb.AppendLine(await MetadataHelper.CreateMetadata(DbContext, AdminHub, HttpContext));
                    sb.AppendLine(await DatasetHelper.CreateDatasets(DbContext, AdminHub, HttpContext, Config));
                    return Content(sb.ToString());
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpPost("[action]")]
        [Authorize(Policy = ODPAuthenticationDefaults.AdminTokenPolicy)]
        public async Task<IActionResult> ImportSetup(IFormFile formFile)
        {
            using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "FileName", formFile?.FileName } }))
            {
                try
                {
                    if (formFile is null) throw new ArgumentNullException(nameof(formFile));
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
