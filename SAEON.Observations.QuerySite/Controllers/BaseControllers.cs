//#define ODPAuth
#if ODPAuth
using Newtonsoft.Json.Linq;
#endif
using SAEON.Core;
using SAEON.Logs;
using SAEON.Observations.Core;
#if ODPAuth
using SAEON.Observations.Auth;
#endif
using SAEON.Observations.QuerySite.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
#if ODPAuth
using System.Security.Claims;
#endif
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    [OutputCache(Duration = Defaults.CacheDuration)]
    public abstract class BaseController : Controller
    {
        public BaseController() : base()
        {
#if ODPAuth
            ViewBag.ODPAuth = true;
#else
            ViewBag.ODPAuth = false;
#endif
        }

        protected string Tenant
        {
            get
            {
                var tenant = Session[Constants.SessionKeyTenant]?.ToString();
                if (string.IsNullOrWhiteSpace(tenant))
                {
                    tenant = ConfigurationManager.AppSettings[Constants.ConfigKeyDefaultTenant] ?? "Fynbos";
                    Session[Constants.SessionKeyTenant] = tenant;
                }
                return tenant;
            }
            set
            {
                Session[Constants.SessionKeyTenant] = value;
            }
        }

#if ODPAuth
        private async Task<string> GetAccessTokenAsync(bool useSession = true)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var token = useSession ? Session[Constants.SessionKeyAccessToken]?.ToString() : null;
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        using (var client = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["AuthenticationServerUrl"].AddTrailingForwardSlash()) })
                        {
                            using (var formContent = new FormUrlEncodedContent(new[] {
                                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                                new KeyValuePair<string, string>("scope", ConfigurationManager.AppSettings["WebAPIClientId"]),
                                new KeyValuePair<string, string>("client_id", ConfigurationManager.AppSettings["QuerySiteClientId"]),
                                new KeyValuePair<string, string>("client_secret", ConfigurationManager.AppSettings["QuerySiteClientSecret"]),
                                }))
                            {
                                //SAEONLogs.Information("scope: {scope} client_id: {client_id} client_secret: {client_secret}", ODPAuthenticationDefaults.WebAPIClientId, ODPAuthenticationDefaults.QuerySiteClientId), config["QuerySiteClientSecret"]);
                                SAEONLogs.Verbose("Requesting access token");
                                var response = await client.PostAsync("oauth2/token", formContent);
                                if (!response.IsSuccessStatusCode)
                                {
                                    SAEONLogs.Error("HttpError: {StatusCode} {Reason}", response.StatusCode, response.ReasonPhrase);
                                    SAEONLogs.Error("Response: {Response}", await response.Content.ReadAsStringAsync());
                                }
                                response.EnsureSuccessStatusCode();
                                token = await response.Content.ReadAsStringAsync();
                                if (!string.IsNullOrWhiteSpace(token))
                                {
                                    Session[Constants.SessionKeyAccessToken] = token;
                                }
                            }
                        }
                    }
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        SAEONLogs.Error("AccessToken is invalid");
                        if (string.IsNullOrWhiteSpace(token)) throw new InvalidOperationException("AccessToken is invalid");
                    }
                    SAEONLogs.Verbose("AccessToken: {AccessToken}", token);
                    return token;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        private string GetBearerTokenFromAccessToken(string accessToken)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var jObj = JObject.Parse(accessToken);
                    var token = jObj.Value<string>("access_token");
                    SAEONLogs.Verbose("BearerToken: {BearerToken}", token);
                    return token;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        private string GetIdToken(bool useSession = true)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var token = useSession ? Session[Constants.SessionKeyIdToken]?.ToString() : null;
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        token = (User as ClaimsPrincipal)?.FindFirst(Constants.IdTokenClaim)?.Value;
                    }
                    if (string.IsNullOrEmpty(token))
                    {
                        SAEONLogs.Error("The access token cannot be found in the authentication ticket. Make sure that SaveTokens is set to true in the OIDC options.");
                        throw new InvalidOperationException("The access token cannot be found in the authentication ticket. Make sure that SaveTokens is set to true in the OIDC options.");
                    }
                    Session[Constants.SessionKeyIdToken] = token;
                    SAEONLogs.Verbose("IdToken: {IdToken}", token);
                    return token;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }
#endif

        protected async Task<string> GetAuthorizationAsync()
        {
            var token = Session[Constants.SessionKeyIdToken]?.ToString();
#if ODPAuth
            if (string.IsNullOrWhiteSpace(token))
            {
                token = GetBearerTokenFromAccessToken(await GetAccessTokenAsync());
            }
#endif
            return $"Bearer {token}";
        }

        protected HttpClient GetWebAPIClientBase()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Add(Constants.HeaderKeyTenant, Tenant);
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIUrl"].AddTrailingForwardSlash());
                    client.Timeout = TimeSpan.FromMinutes(Convert.ToInt32(ConfigurationManager.AppSettings["WebAPITimeoutMins"] ?? "15"));
                    return client;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        protected async Task<HttpClient> GetWebAPIClientWithAccessTokenAsync(bool useSession = true)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var client = GetWebAPIClientBase();
#if ODPAuth
                    client.SetBearerToken(GetBearerTokenFromAccessToken(await GetAccessTokenAsync(useSession)));
#endif
                    return client;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        protected HttpClient GetWebAPIClientWithIdToken(bool useSession = true)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var client = GetWebAPIClientBase();
#if ODPAuth
                    client.SetBearerToken(GetIdToken(useSession));
#endif
                    return client;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        protected async Task<HttpClient> GetWebAPIClientAsync(bool useSession = true)
        {
            var client = GetWebAPIClientBase();
#if ODPAuth
            var token = Session[Constants.SessionKeyIdToken]?.ToString();
            if (!string.IsNullOrWhiteSpace(token))
            {
                client.SetBearerToken(token);
            }
            else
            {
                token = await GetAccessTokenAsync(useSession);
                client.SetBearerToken(GetBearerTokenFromAccessToken(token));
            }
#endif
            return client;
        }
    }

    public abstract class BaseController<TModel> : BaseController where TModel : BaseModel, new()
    {
        //public List<string> ModelStateErrors
        //{
        //    get
        //    {
        //        return ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception).ToList();
        //    }
        //}

        //public List<string> GetValidationErrors(DbEntityValidationException ex)
        //{
        //    return ex?.EntityValidationErrors.SelectMany(e => e.ValidationErrors.Select(m => m.PropertyName + ": " + m.ErrorMessage)).ToList();
        //}

        protected async Task<TOutput> GetEntityAsync<TOutput>(string resource)
        {
            using (SAEONLogs.MethodCall<TOutput>(GetType(), new MethodCallParameters { { "Resource", resource } }))
            {
                try
                {
                    using (var client = await GetWebAPIClientAsync())
                    {
                        var url = $"{client.BaseAddress}{resource}";
                        SAEONLogs.Verbose("Calling: {url}", url);
                        var response = await client.GetAsync(url);
                        SAEONLogs.Verbose("Response: {response}", response);
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new HttpException((int)response.StatusCode, response.ReasonPhrase);
                        }
                        var data = await response.Content.ReadAsAsync<TOutput>();
                        //SAEONLogs.Verbose("Data: {@data}", data);
                        return data;
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        //protected async Task<TOutput> GetEntityAsync<TInput, TOutput>(string resource, TInput input)
        //{
        //    string GenerateQueryString()
        //    {
        //        if (input == null)
        //            return "";
        //        else
        //            return $"?json={JsonConvert.SerializeObject(input)}";
        //    }

        //    using (SAEONLogs.MethodCall<TOutput>(GetType(), new MethodCallParameters { { "Resource", resource } }))
        //    {
        //        try
        //        {
        //            using (var client = await GetWebAPIClientAsync())
        //            {
        //                var url = $"{client.BaseAddress}{resource}";
        //                SAEONLogs.Verbose("Calling: {url}", url);
        //                var response = await client.GetAsync(url);
        //                SAEONLogs.Verbose("Response: {response}", response);
        //                if (!response.IsSuccessStatusCode)
        //                {
        //                    throw new HttpException((int)response.StatusCode, response.ReasonPhrase);
        //                }
        //                var data = await response.Content.ReadAsAsync<TOutput>();
        //                //SAEONLogs.Verbose("Data: {@data}", data);
        //                return data;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            SAEONLogs.Exception(ex);
        //            throw;
        //        }
        //    }
        //}

        protected async Task<List<TEntity>> GetListAsync<TEntity>(string resource)// where TEntity : BaseEntity
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType(), new MethodCallParameters { { "Resource", resource } }))
            {
                try
                {
                    using (var client = await GetWebAPIClientAsync())
                    {
                        var url = $"{client.BaseAddress}{resource}";
                        SAEONLogs.Verbose("Calling: {url}", url);
                        var response = await client.GetAsync(resource);
                        SAEONLogs.Verbose("Response: {response}", response);
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new HttpException((int)response.StatusCode, response.ReasonPhrase);
                        }
                        var data = await response.Content.ReadAsAsync<List<TEntity>>();
                        //SAEONLogs.Verbose("Data: {@data}", data);
                        return data;
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        protected JsonResult GetListAsJson<TEntity>(List<TEntity> list)
        {
            var result = Json(list, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }

        protected async Task<Stream> GetStreamAsync(string resource)
        {
            using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "Resource", resource } }))
            {
                try
                {
                    using (var client = await GetWebAPIClientAsync())
                    {
                        var url = $"{client.BaseAddress}{resource}";
                        SAEONLogs.Verbose("Calling: {url}", url);
                        var response = await client.GetAsync(url);
                        SAEONLogs.Verbose("Response: {response}", response);
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new HttpException((int)response.StatusCode, response.ReasonPhrase);
                        }
                        var data = await response.Content.ReadAsStreamAsync();
                        //SAEONLogs.Verbose("Data: {@data}", data);
                        return data;
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        protected async Task<TOutput> PostEntityAsync<TInput, TOutput>(string resource, TInput input)
        {
            using (SAEONLogs.MethodCall<TOutput>(GetType(), new MethodCallParameters { { "Resource", resource } }))
            {
                try
                {
                    using (var client = await GetWebAPIClientAsync())
                    {
                        var url = $"{client.BaseAddress}{resource}";
                        SAEONLogs.Verbose("Calling: {url}", url);
                        SAEONLogs.Verbose("Input: {@Input}", input);
                        var response = await client.PostAsJsonAsync<TInput>(url, input);
                        SAEONLogs.Verbose("Response: {response}", response);
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new HttpException((int)response.StatusCode, response.ReasonPhrase);
                        }
                        var data = await response.Content.ReadAsAsync<TOutput>();
                        //SAEONLogs.Verbose("Data: {@data}", data);
                        return data;
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        private readonly string sessionModelKey = typeof(TModel).Name + "Model";

        protected TModel SessionModel { get { return GetSessionModel(); } set { SetSessionModel(value); } }

        private TModel GetSessionModel()
        {
            if (Session[sessionModelKey] == null)
            {
                Session[sessionModelKey] = new TModel();
            }
            return (TModel)Session[sessionModelKey];

        }

        private void SetSessionModel(TModel value)
        {
            Session[sessionModelKey] = value;
        }

        protected void RemoveSessionModel()
        {
            Session.Remove(sessionModelKey);
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

        protected virtual Task<TModel> LoadModelAsync(TModel model)
        {
            throw new NotImplementedException();
        }

        protected async Task<TModel> CreateModelAsync()
        {
            var model = await LoadModelAsync(new TModel());
            SessionModel = model;
            return model;
        }

    }

}