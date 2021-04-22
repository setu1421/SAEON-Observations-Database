using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAEON.Logs;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SAEON.Observations.QuerySite.Controllers
{
    //[Authorize(Roles = "admin,Admin")]
    public class AdminController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> APICreateDOIs()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    using (var client = GetWebAPIClient())
                    //using (var client = await GetWebAPIClientWithAccessTokenAsync())
                    {
                        var response = await client.PostAsync("/Internal/Admin/CreateDOIs", null);
                        response.EnsureSuccessStatusCode();
                    }
                    return NoContent();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public IActionResult CreateDOIs()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> APICreateMetadata()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    using (var client = GetWebAPIClient())
                    //using (var client = await GetWebAPIClientWithAccessTokenAsync())
                    {
                        var response = await client.PostAsync("/Internal/Admin/CreateMetadata", null);
                        response.EnsureSuccessStatusCode();
                    }
                    return NoContent();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public IActionResult CreateMetadata()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> APICreateODPMetadata()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    using (var client = GetWebAPIClient())
                    //using (var client = await GetWebAPIClientWithAccessTokenAsync())
                    {
                        var response = await client.PostAsync("/Internal/Admin/CreateODPMetadata", null);
                        response.EnsureSuccessStatusCode();
                    }
                    return NoContent();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public IActionResult CreateODPMetadata()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> APIImportSetup(IFormFile formFile)
        {
            using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "FileName", formFile?.FileName } }))
            {
                try
                {
                    if (formFile is null) throw new ArgumentNullException(nameof(formFile));
                    if (formFile.Length == 0) throw new ArgumentOutOfRangeException(nameof(formFile.Length), "File length cannot be zero");
                    var ext = Path.GetExtension(formFile.FileName).ToLowerInvariant();
                    if (!(ext == ".xls" || ext == ".xlsx")) throw new ArgumentOutOfRangeException(nameof(formFile.FileName), "Invalid file extension");
                    SAEONLogs.Information("Uploading {FileName}", formFile.FileName);
                    using (var client = GetWebAPIClient())
                    //using (var client = await GetWebAPIClientWithAccessTokenAsync())
                    {
                        using (var content = new MultipartFormDataContent())
                        {
                            content.Add(new StreamContent(formFile.OpenReadStream())
                            {
                                Headers =
                                    {
                                        ContentLength = formFile.Length,
                                        ContentType = new MediaTypeHeaderValue(formFile.ContentType)
                                    }
                            }, "FormFile", formFile.FileName);
                            var response = await client.PostAsync("/Internal/Admin/ImportSetup", content);
                            response.EnsureSuccessStatusCode();
                        }
                    }
                    return NoContent();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public IActionResult ImportSetup()
        {
            return View();
        }
    }
}
