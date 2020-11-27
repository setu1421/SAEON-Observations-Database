using SAEON.Logs;
using SAEON.Observations.QuerySite.Models;
using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    [RoutePrefix("Health")]
    public class HealthController : Controller
    {
        [Route, HttpGet]
        public async Task<JsonResult> IndexAsync()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                var model = new HealthModel();
                try
                {
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.ApplicationJson));
                    client.DefaultRequestHeaders.Add(Constants.HeaderKeyTenant, Session[Constants.SessionKeyTenant].ToString());
                    SAEONLogs.Verbose("Headers: {@Headers}", client.DefaultRequestHeaders);
                    var url = ConfigurationManager.AppSettings["AuthenticationServerHealthCheckUrl"];
                    SAEONLogs.Verbose("Calling: {url}", url);
                    var response = await client.GetAsync(url);
                    SAEONLogs.Verbose("Response: {response}", response);
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    model.AuthenticationServer = ex.Message;
                    model.Healthy = false;
                }
                try
                {
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.ApplicationJson));
                    client.DefaultRequestHeaders.Add(Constants.HeaderKeyTenant, Session[Constants.SessionKeyTenant].ToString());
                    SAEONLogs.Verbose("Headers: {@Headers}", client.DefaultRequestHeaders);
                    var url = ConfigurationManager.AppSettings["WebAPIHealthCheckUrl"];
                    SAEONLogs.Verbose("Calling: {url}", url);
                    var response = await client.GetAsync(url);
                    SAEONLogs.Verbose("Response: {response}", response);
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    model.WebApi = ex.Message;
                    model.Healthy = false;
                }
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
