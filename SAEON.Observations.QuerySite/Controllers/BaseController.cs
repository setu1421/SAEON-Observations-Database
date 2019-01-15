using IdentityModel.Client;
using Newtonsoft.Json;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.QuerySite.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using Thinktecture.IdentityModel.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    [HandleError, HandleForbidden]
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
                var tokenClient = new HttpClient();
                var tokenResponse = await tokenClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = Properties.Settings.Default.IdentityServerUrl + "/connect/token",
                    ClientId = "It6fWPU5J708",
                    ClientSecret = "secret",
                    Scope = "SAEON.Observations.WebAPI"
                });
                if (tokenResponse.IsError)
                {
                    Logging.Error("Error: {error}", tokenResponse.Error);
                    throw new HttpException(tokenResponse.Error);
                }
                token = tokenResponse.AccessToken;
            }
            Logging.Verbose("Token: {token}", token);
            client.SetBearerToken(token);
            client.Timeout = TimeSpan.FromMinutes(TimeOut);
            return client;
        }

        protected async Task<List<TEntity>> GetList<TEntity>(string resource)// where TEntity : BaseEntity
        {
            using (Logging.MethodCall<TEntity>(GetType(), new ParameterList { { "Resource", resource } }))
            {
                try
                {
                    using (var client = await GetWebAPIClientAsync())
                    {
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

        protected async Task<TOutput> GetEntity<TOutput>(string resource)
        {
            using (Logging.MethodCall<TOutput>(GetType(), new ParameterList { { "Resource", resource } }))
            {
                try
                {
                    using (var client = await GetWebAPIClientAsync())
                    {
                        var url = $"{apiBaseUrl}/{resource}";
                        Logging.Verbose("Calling: {url}", url);
                        var response = await client.GetAsync(url);
                        Logging.Verbose("Response: {response}", response);
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new HttpException((int)response.StatusCode, response.ReasonPhrase);
                        }
                        var data = await response.Content.ReadAsAsync<TOutput>();
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

        protected async Task<TOutput> GetEntity<TInput, TOutput>(string resource, TInput input)
        {
            string GenerateQueryString()
            {
                if (input == null)
                    return "";
                else
                    return $"?json={JsonConvert.SerializeObject(input)}";
            }

            using (Logging.MethodCall<TOutput>(GetType(), new ParameterList { { "Resource", resource } }))
            {
                try
                {
                    using (var client = await GetWebAPIClientAsync())
                    {
                        var url = $"{apiBaseUrl}/{resource}" + GenerateQueryString();
                        Logging.Verbose("Calling: {url}", url);
                        var response = await client.GetAsync(url);
                        Logging.Verbose("Response: {response}", response);
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new HttpException((int)response.StatusCode, response.ReasonPhrase);
                        }
                        var data = await response.Content.ReadAsAsync<TOutput>();
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

        protected async Task<TOutput> PostEntity<TInput, TOutput>(string resource, TInput input)
        {
            using (Logging.MethodCall<TOutput>(GetType(), new ParameterList { { "Resource", resource } }))
            {
                try
                {
                    using (var client = await GetWebAPIClientAsync())
                    {
                        var url = $"{apiBaseUrl}/{resource}";
                        Logging.Verbose("Calling: {url}", url);
                        Logging.Verbose("Input: {@Input}", input);
                        var response = await client.PostAsJsonAsync<TInput>(url, input);
                        Logging.Verbose("Response: {response}", response);
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new HttpException((int)response.StatusCode, response.ReasonPhrase);
                        }
                        var data = await response.Content.ReadAsAsync<TOutput>();
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

    public class BaseController<TModel> : BaseController where TModel : BaseModel, new()
    {
        private readonly string sessionModelKey = typeof(TModel).Name + "Model";

        private HttpSessionState CurrentSession { get { return System.Web.HttpContext.Current.Session; } }

        protected TModel SessionModel { get { return GetSessionModel(); } set { SetSessionModel(value); } }

        private TModel GetSessionModel()
        {
            if (CurrentSession[sessionModelKey] == null)
            {
                CurrentSession[sessionModelKey] = new TModel();
            }
            return (TModel)CurrentSession[sessionModelKey];

        }

        private void SetSessionModel(TModel value)
        {
            CurrentSession[sessionModelKey] = value;
        }

        protected void RemoveSessionModel()
        {
            CurrentSession.Remove(sessionModelKey);
        }

        protected virtual TModel LoadModel(TModel model)
        {
            throw new NotImplementedException();
        }

        protected TModel CreateModel()
        {
            var model = LoadModel(new TModel());
            SessionModel = model;
            return model;
        }

        protected virtual async Task<TModel> LoadModelAsync(TModel model)
        {
            throw new NotImplementedException();
        }

        protected async Task<TModel> CreateModelAsync()
        {
            var model = await LoadModelAsync(new TModel());
            SessionModel = model;
            return model;
        }

        [HttpGet]
        protected JsonResult GetListAsJson<TEntity>(List<TEntity> list)
        {
            var result = Json(list, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }

    }
}
