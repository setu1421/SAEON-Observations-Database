using SAEON.Logs;
using SAEON.Observations.QuerySite.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{

    [RoutePrefix("DOI/10.15493")]
    public class DOIController : BaseController
    {
        [Route("{id}")]
        public async Task<ActionResult> Index(string id)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var doi = "10.15493/" + id;
                    using (var client = await GetWebAPIClientAsync())
                    {
                        using (var formContent = new FormUrlEncodedContent(new[] {
                                new KeyValuePair<string, string>("doi", doi) }))
                        {
                            var response = await client.PostAsync("/Internal/DOI/AsHtml", formContent);
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
