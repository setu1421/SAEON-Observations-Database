using Microsoft.AspNetCore.Mvc;
using SAEON.Logs;
using System;
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

        public async Task<IActionResult> CreateDynamicDOIs()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    using (var client = GetWebAPIClient())
                    //using (var client = await GetWebAPIClientWithAccessTokenAsync())
                    {
                        var response = await client.PostAsync(Config["WebAPIUrl"] + "/Internal/Admin/CreateDynamicDOIs", null);
                        response.EnsureSuccessStatusCode();
                        ViewData["Results"] = (await response.Content.ReadAsStringAsync()).Replace(Environment.NewLine, "<br/>");
                    }
                    return View();
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
