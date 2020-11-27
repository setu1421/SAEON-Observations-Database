using SAEON.Logs;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    [Authorize(Roles = "admin,Admin")]
    public class AdminController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> APICreateDOIs()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    SAEONLogs.Information("Calling WebAPI");
                    using (var client = GetWebAPIClientWithIdToken())
                    {
                        var response = await client.PostAsync("/Internal/Admin/CreateDOIs", null);
                        response.EnsureSuccessStatusCode();
                    }
                    return new HttpStatusCodeResult(HttpStatusCode.NoContent);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public ActionResult CreateDOIs()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> APICreateMetadata()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    using (var client = GetWebAPIClientWithIdToken())
                    {
                        var response = await client.PostAsync("/Internal/Admin/CreateMetadata", null);
                        response.EnsureSuccessStatusCode();
                    }
                    return new HttpStatusCodeResult(HttpStatusCode.NoContent);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public ActionResult CreateMetadata()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> APICreateODPMetadata()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    using (var client = GetWebAPIClientWithIdToken())
                    {
                        var response = await client.PostAsync("/Internal/Admin/CreateODPMetadata", null);
                        response.EnsureSuccessStatusCode();
                    }
                    return new HttpStatusCodeResult(HttpStatusCode.NoContent);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public ActionResult CreateODPMetadata()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> APIImportSetup(HttpPostedFileBase file)
        {
            using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "FileName", file?.FileName } }))
            {
                try
                {
                    if (file is null) throw new ArgumentNullException(nameof(file));
                    if (file.ContentLength == 0) throw new ArgumentOutOfRangeException(nameof(file.ContentLength), "File length cannot be zero");
                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (!(ext == ".xls" || ext == ".xlsx")) throw new ArgumentOutOfRangeException(nameof(file.FileName), "Invalid file extension");
                    SAEONLogs.Information("Uploading {FileName}", file.FileName);
                    using (var client = await GetWebAPIClientWithAccessTokenAsync())
                    {
                        using (var content = new MultipartFormDataContent())
                        {
                            content.Add(new StreamContent(file.InputStream)
                            {
                                Headers =
                                    {
                                        ContentLength = file.ContentLength,
                                        ContentType = new MediaTypeHeaderValue(file.ContentType)
                                    }
                            }, "formFile", file.FileName);
                            var response = await client.PostAsync("/Internal/Admin/ImportSetup", content);
                            response.EnsureSuccessStatusCode();
                        }
                    }
                    return new HttpStatusCodeResult(HttpStatusCode.NoContent);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public ActionResult ImportSetup()
        {
            return View();
        }
    }
}
