using Humanizer;
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
                    using (var client = await GetWebAPIClientAsync())
                    {
                        var response = await client.PostAsync("Internal/Admin/CreateDOIs", null);
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
                    using (var client = await GetWebAPIClientAsync())
                    {
                        var response = await client.PostAsync("Internal/Admin/CreateMetadata", null);
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
                    using (var client = await GetWebAPIClientAsync())
                    {
                        var response = await client.PostAsync("Internal/Admin/CreateODPMetadata", null);
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
        public async Task<ActionResult> APICreateDatasets()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    SAEONLogs.Information("Calling WebAPI");
                    using (var client = await GetWebAPIClientAsync())
                    {
                        var response = await client.PostAsync("Internal/Admin/CreateDatasets", null);
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

        public ActionResult CreateDatasets()
        {
            return View();
        }



        [HttpPost]
        public async Task<ActionResult> APIImportSetup(HttpPostedFileBase formFile)
        {
            using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "FileName", formFile?.FileName } }))
            {
                try
                {
                    if (formFile is null) throw new ArgumentNullException(nameof(formFile));
                    if (formFile.ContentLength == 0) throw new ArgumentOutOfRangeException(nameof(formFile.ContentLength), "File length cannot be zero");
                    var ext = Path.GetExtension(formFile.FileName).ToLowerInvariant();
                    if (!(ext == ".xls" || ext == ".xlsx")) throw new ArgumentOutOfRangeException(nameof(formFile.FileName), "Invalid file extension");
                    SAEONLogs.Information("Uploading {FileName} Size: {Size} Type: {Type}", formFile.FileName, formFile.ContentLength.Bytes().Humanize("MB"), formFile.ContentType);
                    using (var client = await GetWebAPIClientAsync())
                    {
                        using (var content = new MultipartFormDataContent())
                        {
                            content.Add(new StreamContent(formFile.InputStream)
                            {
                                Headers =
                                    {
                                        ContentLength = formFile.ContentLength,
                                        ContentType = new MediaTypeHeaderValue(formFile.ContentType)
                                    }
                            }, "formFile", formFile.FileName);
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
