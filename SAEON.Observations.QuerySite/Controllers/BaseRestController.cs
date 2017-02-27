using IdentityModel.Client;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;
using SAEON.Observations.Core;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    //[Authorize]
    public class BaseRestController<TEntity> : Controller where TEntity : BaseEntity, new()
    {
        private static string apiBaseUrl = "http://localhost:50840/";
        private static string identityUrl = "https://localhost:44311/oauth2";
        private static string tokenUrl = identityUrl + "/connect/token";
        private string resource = null;
        protected string Resource { get { return resource; } set { resource = value; } }

        protected List<TEntity> GetAll()
        {
            using (LogContext.PushProperty("Method", "Index"))
            {
                try
                {
                    RestClient client = new RestClient(apiBaseUrl);
                    RestRequest request = new RestRequest(Resource, Method.GET);
                    //request.AddParameter("Authorization", $"Bearer {access_token}"),ParameterType.HttpHeader);
                    request.AddHeader("Accept", "application/json");
                    request.RequestFormat = DataFormat.Json;
                    var response = client.Execute<List<TEntity>>(request);
                    return response.Data;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to get all");
                    throw;
                }
            }
        }

        private async Task<TokenResponse> GetTokenAsync()
        {
            var client = new TokenClient(tokenUrl, "SAEON.Observations.WebAPI", "81g5wyGSC89a");
            return await client.RequestClientCredentialsAsync("SAEON.Observations.WebAPI");
        }

        public virtual async Task<ActionResult> Index()
        {
            using (LogContext.PushProperty("Method", "Index"))
            {
                try
                {
                    var client = new HttpClient();
                    var user = User as ClaimsPrincipal;
                    var token = user.FindFirst("access_token").Value;
                    client.SetBearerToken(token);
                    var response = await client.GetAsync(apiBaseUrl + "/" + Resource);
                    var data = JsonConvert.DeserializeObject<List<TEntity>>(response.Content.ToString());
                    return View(data);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to get all");
                    throw;
                }
            }
        }

        //public virtual ActionResult Index()
        //{
        //    using (LogContext.PushProperty("Method", "Index"))
        //    {
        //        try
        //        {
        //            RestClient client = new RestClient(apiBaseUrl);
        //            client.AddHandler("*", new JsonDeserializer());
        //            RestRequest request = new RestRequest(Resource, Method.GET);
        //            //request.AddParameter("Authorization", $"Bearer {access_token}"),ParameterType.HttpHeader);
        //            request.AddHeader("Accept", "application/json");
        //            request.RequestFormat = DataFormat.Json;
        //            var response = client.Execute<List<TEntity>>(request);
        //            var data = JsonConvert.DeserializeObject<List<TEntity>>(response.Content);
        //            return View(data);
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.Error(ex, "Unable to get all");
        //            throw;
        //        }
        //    }
        //}

    }
}