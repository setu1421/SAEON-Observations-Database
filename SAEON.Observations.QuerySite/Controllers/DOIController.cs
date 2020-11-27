namespace SAEON.Observations.QuerySite.Controllers
{
    /*
    [Route("[controller]/10.15493")]
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
                    using (var client = GetWebAPIClient())
                    //using (var client = await GetWebAPIClientWithAccessTokenAsync())
                    {
                        using (var formContent = new FormUrlEncodedContent(new[] {
                                new KeyValuePair<string, string>("doi", doi) }))
                        {
                            var response = await client.PostAsync(ConfigurationManager.AppSettings["WebAPIUrl"] + "/Internal/DOI/AsHtml", formContent);
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
    */
}
