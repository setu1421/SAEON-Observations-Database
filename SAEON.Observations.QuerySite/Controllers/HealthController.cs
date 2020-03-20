using SAEON.AspNet.Common;
using SAEON.Logs;
using SAEON.Observations.QuerySite.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    [RoutePrefix("Health")]
    public class HealthController : Controller
    {
        [Route]
        public async Task<JsonResult> IndexAsync()
        {
            using (Logging.MethodCall(GetType()))
            {
                var model = new HealthModel();
                try
                {
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(AspNetConstants.ApplicationJson));
                    client.DefaultRequestHeaders.Add(AspNetConstants.TenantHeader, Session[AspNetConstants.TenantSession].ToString());
                    Logging.Verbose("Headers: {@Headers}", client.DefaultRequestHeaders);
                    var url = Properties.Settings.Default.IdentityServerUrl + "/Health";
                    Logging.Verbose("Calling: {url}", url);
                    var response = await client.GetAsync(url);
                    Logging.Verbose("Response: {response}", response);
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    model.IdentityService = ex.Message;
                    model.Healthy = false;
                }
                try
                {
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add(AspNetConstants.TenantHeader, Session[AspNetConstants.TenantSession].ToString());
                    Logging.Verbose("Headers: {@Headers}", client.DefaultRequestHeaders);
                    var url = Properties.Settings.Default.WebAPIUrl + "/Health";
                    Logging.Verbose("Calling: {url}", url);
                    var response = await client.GetAsync(url);
                    Logging.Verbose("Response: {response}", response);
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    model.WebApi = ex.Message;
                    model.Healthy = false;
                }
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
