using Newtonsoft.Json.Linq;
using SAEON.AspNet.Auth;
using SAEON.Core;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.QuerySite.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
#if ResponseCaching
    [OutputCache(Duration = Defaults.CacheDuration)]
#endif
    public abstract class BaseController : Controller
    {
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

        protected List<string> ModelStateErrors
        {
            get
            {
                return ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception).ToList();
            }
        }

        private async Task<string> GetAccessTokenAsync()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    string token = null;
                    var idToken = (User as ClaimsPrincipal)?.FindFirst(Constants.IdTokenClaim)?.Value;
                    if (!string.IsNullOrEmpty(idToken))
                    {
                        token = (User as ClaimsPrincipal)?.FindFirst(Constants.AccessTokenClaim)?.Value;
                        if (string.IsNullOrWhiteSpace(token))
                        {
                            SAEONLogs.Error("AccessTokenClaim is invalid");
                            throw new InvalidOperationException("AccessTokenClaim is invalid");
                        }
                        Session[Constants.SessionKeyODPAccessToken] = token;
                    }
                    else
                    {
                        token = Session[Constants.SessionKeyODPAccessToken]?.ToString();
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
                                    SAEONLogs.Verbose("Requesting ODP token");
                                    var response = await client.PostAsync("oauth2/token", formContent);
                                    if (!response.IsSuccessStatusCode)
                                    {
                                        SAEONLogs.Error("HttpError: {StatusCode} {Reason}", response.StatusCode, response.ReasonPhrase);
                                        SAEONLogs.Error("Response: {Response}", await response.Content.ReadAsStringAsync());
                                    }
                                    response.EnsureSuccessStatusCode();
                                    var odpToken = await response.Content.ReadAsStringAsync();
                                    SAEONLogs.Verbose("ODPToken: {ODPToken}", odpToken);
                                    if (string.IsNullOrWhiteSpace(odpToken))
                                    {
                                        SAEONLogs.Error("ODPToken is invalid");
                                        throw new InvalidOperationException("ODPToken is invalid");
                                    }
                                    var jObj = JObject.Parse(odpToken);
                                    token = jObj.Value<string>("access_token");
                                    Session[Constants.SessionKeyODPAccessToken] = token;
                                }
                            }
                        }
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

        protected async Task<string> GetAuthorizationAsync()
        {
            var token = await GetAccessTokenAsync();
            return $"Bearer {token}";
        }

        private HttpClient GetWebAPIClientBase()
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

        protected async Task<HttpClient> GetWebAPIClientAsync()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var client = GetWebAPIClientBase();
                    var token = await GetAccessTokenAsync();
                    client.SetBearerToken(token);
                    return client;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        //        protected HttpClient GetWebAPIClientWithIdToken(bool useSession = true)
        //        {
        //            using (SAEONLogs.MethodCall(GetType()))
        //            {
        //                try
        //                {
        //                    var client = GetWebAPIClientBase();
        //                    client.SetBearerToken(GetIdToken(useSession));
        //                    return client;
        //                }
        //                catch (Exception ex)
        //                {
        //                    SAEONLogs.Exception(ex);
        //                    throw;
        //                }
        //            }
        //        }

        //protected async Task<HttpClient> GetWebAPIClientAsync(bool useSession = true)
        //{
        //    var client = GetWebAPIClientBase();
        //    var token = Session[Constants.SessionKeyIdToken]?.ToString();
        //    if (!string.IsNullOrWhiteSpace(token))
        //    {
        //        client.SetBearerToken(token);
        //    }
        //    else
        //    {
        //        token = await GetODPAccessTokenAsync(useSession);
        //        client.SetBearerToken(GetBearerTokenFromAccessToken(token));
        //    }
        //    return client;
        //}
    }

    public abstract class BaseController<TModel> : BaseController where TModel : BaseModel, new()
    {
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
        //        if (input is null)
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
            if (Session[sessionModelKey] is null)
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

    [Authorize]
    public abstract class BaseRestController<TEntity> : BaseController where TEntity : NamedEntity, new()
    {
        protected string Resource { get; set; }


        [HttpGet]
        public async Task<ActionResult> Index()
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType()))
            {
                try
                {
                    using (var client = await GetWebAPIClientAsync())
                    {
                        var response = await client.GetAsync($"{client.BaseAddress}{Resource}");
                        SAEONLogs.Verbose("Response: {response}", response);
                        response.EnsureSuccessStatusCode();
                        var data = await response.Content.ReadAsAsync<IEnumerable<TEntity>>();
                        //SAEONLogs.Verbose("Data: {data}", data);
                        return View(data);
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
        [Route("{id:guid}")]
        public virtual async Task<ActionResult> Details(Guid? id)
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType(), new MethodCallParameters { { "Id", id } }))
            {
                try
                {
                    if (!id.HasValue) return RedirectToAction("Index");
                    using (var client = await GetWebAPIClientAsync())
                    {
                        var response = await client.GetAsync($"{client.BaseAddress}{Resource}/{id.Value}");
                        SAEONLogs.Verbose("Response: {response}", response);
                        response.EnsureSuccessStatusCode();
                        var data = await response.Content.ReadAsAsync<TEntity>();
                        SAEONLogs.Verbose("Data: {data}", data);
                        if (data is null) return RedirectToAction("Index");
                        return View(data);
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("{id:guid}")]
        public virtual async Task<ActionResult> Edit(Guid? id)
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType(), new MethodCallParameters { { "Id", id } }))
            {
                try
                {
                    SAEONLogs.Information("Claims: {Claims}", ((ClaimsPrincipal)User).Claims.ToClaimsList());
                    if (!id.HasValue) return RedirectToAction("Index");
                    using (var client = await GetWebAPIClientAsync())
                    {
                        var response = await client.GetAsync($"{client.BaseAddress}{Resource}/{id?.ToString()}");
                        SAEONLogs.Verbose("Response: {response}", response);
                        response.EnsureSuccessStatusCode();
                        var data = await response.Content.ReadAsAsync<TEntity>();
                        SAEONLogs.Verbose("Data: {data}", data);
                        if (data is null) return RedirectToAction("Index");
                        return View(data);
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Edit(TEntity delta)
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType(), new MethodCallParameters { { "Id", delta?.Id }, { "Delta", delta } }))
            {
                try
                {
                    SAEONLogs.Verbose("Delta: {@Delta}", delta);
                    if (!ModelState.IsValid)
                    {
                        SAEONLogs.Error("ModelState.Invalid {ModelStateErrors}", ModelStateErrors);
                        return View(delta);
                    }
                    else
                    {
                        using (var client = await GetWebAPIClientAsync())
                        {
                            var response = await client.PutAsJsonAsync<TEntity>($"{client.BaseAddress}{Resource}/{delta?.Id}", delta);
                            SAEONLogs.Verbose("Response: {response}", response);
                            response.EnsureSuccessStatusCode();
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to edit {id}", delta?.Id);
                    throw;
                }
            }

        }

        [HttpGet]
        [Route("{id:guid}")]
        public virtual async Task<ActionResult> Delete(Guid? id)
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType(), new MethodCallParameters { { "Id", id } }))
            {
                try
                {
                    if (!id.HasValue) return RedirectToAction("Index");
                    using (var client = await GetWebAPIClientAsync())
                    {
                        var response = await client.GetAsync($"{client.BaseAddress}{Resource}/{id?.ToString()}");
                        SAEONLogs.Verbose("Response: {response}", response);
                        response.EnsureSuccessStatusCode();
                        var data = await response.Content.ReadAsAsync<TEntity>();
                        SAEONLogs.Verbose("Data: {data}", data);
                        if (data is null) return RedirectToAction("Index");
                        return View(data);
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to delete {id}", id);
                    throw;
                }
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType(), new MethodCallParameters { { "Id", id } }))
            {
                try
                {
                    using (var client = await GetWebAPIClientAsync())
                    {
                        var response = await client.DeleteAsync($"{client.BaseAddress}{Resource}/{id}");
                        SAEONLogs.Verbose("Response: {response}", response);
                        response.EnsureSuccessStatusCode();
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to delete {id}", id);
                    throw;
                }
            }
        }
    }
}