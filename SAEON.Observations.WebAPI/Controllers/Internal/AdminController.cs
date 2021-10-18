using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SAEON.AspNet.Auth;
using SAEON.Core;
using SAEON.Logs;
using SAEON.Observations.WebAPI.Hubs;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    //[Authorize(Policy = ODPAuthenticationDefaults.AdminTokenPolicy)]
    public class AdminController : InternalApiController
    {
        private readonly IHubContext<AdminHub> AdminHub;

        public AdminController(IHubContext<AdminHub> webApiHub)
        {
            AdminHub = webApiHub;
            TrackChanges = true;
        }

        [HttpPost("[action]")]
        [Authorize(Policy = ODPAuthenticationDefaults.AdminTokenPolicy)]
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
        [Authorize(Policy = ODPAuthenticationDefaults.AdminTokenPolicy)]
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
        [Authorize(Policy = ODPAuthenticationDefaults.AdminTokenPolicy)]
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
        [Authorize(Policy = ODPAuthenticationDefaults.AccessTokenPolicy)]
        public async Task<IActionResult> UpdateODP()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var sb = new StringBuilder();
                    SAEONLogs.Information("CreateDOIs");
                    sb.AppendLine("CreateDOIs");
                    sb.AppendLine(await DOIHelper.CreateDOIsV2(DbContext, AdminHub, HttpContext, AnalyticsHelper.IsTest(Request)));
                    SAEONLogs.Information("CreateMetadata");
                    sb.AppendLine("CreateMetadata");
                    sb.AppendLine(await MetadataHelper.CreateMetadataV2(DbContext, AdminHub, AnalyticsHelper.IsTest(Request)));
                    SAEONLogs.Information("CreateODPMetadata");
                    sb.AppendLine("CreateODPMetadata");
                    sb.AppendLine(await ODPMetadataHelper.CreateMetadata(DbContext, AdminHub, Config));
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
        public async Task<IActionResult> CreateSnapshots()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var sb = new StringBuilder();
                    SAEONLogs.Information("CreateInventorySnapshot");
                    sb.AppendLine("CreateInventorySnapshot");
                    var inventorySnapshot = (await DbContext.InventorySnapshots.FromSqlRaw("spCreateInventorySnapshot").ToListAsync()).FirstOrDefault();
                    SAEONLogs.Information("InventorySnapshot: {InventorySnapshot}", inventorySnapshot);
                    SAEONLogs.Information("Done");
                    sb.AppendLine("Done");
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
        public async Task<IActionResult> CreateImportBatchSummaries()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var sb = new StringBuilder();
                    SAEONLogs.Information("CreateImportBatchSummaries");
                    sb.AppendLine("CreateImportBatchSummaries");
                    _ = (await DbContext.ImportBatchSummaries.FromSqlRaw("spCreateImportBatchSummaries").ToListAsync());
                    SAEONLogs.Information("Done");
                    sb.AppendLine("Done");
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
