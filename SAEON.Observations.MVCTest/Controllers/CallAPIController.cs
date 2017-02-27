using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SAEON.Observations.MVCTest.Controllers
{
    public class CallAPIController : Controller
    {

        private async Task<TokenResponse> GetTokenAsync()
        {
            Log.Information("CallAPIController.GetTokenAsync");
            var client = new TokenClient(
                "https://localhost:44311/oauth2/connect/token",
                "mvc_service",
                "secret");
            try
            {
                return await client.RequestClientCredentialsAsync("sampleApi");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unable to get token");
                throw;
            }
        }

        private async Task<string> CallApi(string token)
        {
            Log.Information("CallAPIController.CallApi");
            var client = new HttpClient();
            client.SetBearerToken(token);
            var json = await client.GetStringAsync("http://localhost:58197/identity");
            Log.Information("JSON: " + json);
            return JArray.Parse(json).ToString();
        }

        // GET: CallApi/ClientCredentials
        public async Task<ActionResult> ClientCredentials()
        {
            Log.Information("CallAPIController.ClientCredentials");
            var response = await GetTokenAsync();
            Log.Information("Token: "+response.AccessToken);
            var result = await CallApi(response.AccessToken);
            ViewBag.Json = result;
            return View("ShowApiResult");
        }
    }
}