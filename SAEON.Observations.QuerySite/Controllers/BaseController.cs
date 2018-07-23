using IdentityModel.Client;
using SAEON.Logs;
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

namespace SAEON.Observations.QuerySite.Controllers
{
    public class BaseController : Controller
    {
        protected int TimeOut { get; set; } = 30; // In minutes
        private readonly string apiBaseUrl = Properties.Settings.Default.WebAPIUrl;

        private async Task<HttpClient> GetWebAPIClientAsync()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Logging.Verbose("Claims: {claims}", string.Join("; ", User.GetClaims()));
            var token = (User as ClaimsPrincipal)?.FindFirst("access_token")?.Value;
            if (token == null)
            {
                var tokenClient = new TokenClient(Properties.Settings.Default.IdentityServerUrl + "/connect/token", "SAEON.Observations.QuerySite", "It6fWPU5J708");
                var tokenResponse = await tokenClient.RequestClientCredentialsAsync("SAEON.Observations.WebAPI");
                if (tokenResponse.IsError)
                {
                    Logging.Error("Error: {error}", tokenResponse.Error);
                    throw new HttpException(tokenResponse.Error);
                }
                token = tokenResponse.AccessToken;
            }
            Logging.Verbose("Token: {token}", token);
            client.SetBearerToken(token);
            return client;
        }

        protected async Task<IEnumerable<TEntity>> GetList<TEntity>(string resource)// where TEntity : BaseEntity
        {
            using (Logging.MethodCall<TEntity>(GetType(), new ParameterList { { "Resource", resource } }))
            {
                try
                {
                    using (var client = await GetWebAPIClientAsync())
                    {
                        client.Timeout = TimeSpan.FromMinutes(30);
                        var url = $"{apiBaseUrl}/{resource}";
                        Logging.Verbose("Calling: {url}", url);
                        var response = await client.GetAsync(url);
                        Logging.Verbose("Response: {response}", response);
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new HttpException((int)response.StatusCode, response.ReasonPhrase);
                        }
                        var data = await response.Content.ReadAsAsync<List<TEntity>>();
                        //Logging.Verbose("Data: {@data}", data);
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