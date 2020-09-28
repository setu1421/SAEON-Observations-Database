using Microsoft.AspNetCore.Mvc;
using SAEON.Logs;
using SAEON.Observations.QuerySite.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SAEON.Observations.QuerySite.Controllers
{
    [Route("[controller]/10.15493")]
    public class DOIController : BaseController
    {
        [Route("{id}")]
        public async Task<IActionResult> Index(string id)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var doi = "10.15493/" + id;
                    using (var client = GetWebAPIClient())
                    //using (var client = await GetWebAPIClientWithAccessTokenAsync())
                    {
                        using (var formContent = new FormUrlEncodedContent(new[] {
                                new KeyValuePair<string, string>("doi", doi) }))
                        {
                            var response = await client.PostAsync(Config["WebAPIUrl"] + "/Internal/DOI/AsHtml", formContent);
                            response.EnsureSuccessStatusCode();
                            var model = new DOIModel
                            {
                                Id = doi,
                                Html = await response.Content.ReadAsStringAsync()
                            };
                            return View(model);
                        }
                    }
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
