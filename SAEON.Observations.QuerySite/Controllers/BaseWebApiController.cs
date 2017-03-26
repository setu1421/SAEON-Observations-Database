using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace SAEON.Observations.QuerySite.Controllers
{
    [Authorize]
    public class BaseWebApiController : Controller
    {
        private static string apiBaseUrl = Properties.Settings.Default.WebAPIUrl;
        //private static string identityUrl = Properties.Settings.Default.IdentityServerUrl;

        //protected HttpSessionState CurrentSession { get { return System.Web.HttpContext.Current.Session; } }

        protected async Task<IEnumerable<TEntity>> GetList<TEntity>(string resource)// where TEntity : BaseEntity
        {
            using (Logging.MethodCall<TEntity>(this.GetType(), new ParameterList { { "Resource", resource } }))
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        //client.BaseAddress = new Uri(apiBaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var user = User as ClaimsPrincipal;
                        var token = user.FindFirst("access_token").Value;
                        Logging.Verbose("Token: {token}", token);
                        client.SetBearerToken(token);
                        var url = $"{apiBaseUrl}/{resource}";
                        Logging.Verbose("Calling: {url}", url);
                        var response = await client.GetAsync(url);
                        Logging.Verbose("Response: {response}", response);
                        response.EnsureSuccessStatusCode();
                        var data = await response.Content.ReadAsAsync<IEnumerable<TEntity>>();
                        Logging.Verbose("Data: {@data}", data);
                        return data;
                    }
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }
    }
}