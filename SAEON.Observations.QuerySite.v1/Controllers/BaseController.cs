using Newtonsoft.Json;
using SAEON.Logs;
using SAEON.Observations.QuerySite.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Thinktecture.IdentityModel.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    [HandleError, HandleForbidden]
    public class BaseController : Controller
    {
        private static readonly string apiBaseUrl = Properties.Settings.Default.WebAPIUrl;

        private async Task<HttpClient> GetWebAPIClientAsync()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return await WebAPIClient.GetWebAPIClientAsync(Session, (ClaimsPrincipal)User, Request.IsLocal);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public List<string> ModelStateErrors
        {
            get
            {
                return ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception).ToList();
            }
        }

        public List<string> GetValidationErrors(DbEntityValidationException ex)
        {
            return ex?.EntityValidationErrors.SelectMany(e => e.ValidationErrors.Select(m => m.PropertyName + ": " + m.ErrorMessage)).ToList();
        }

        protected async Task<Stream> GetStreamAsync(string resource)
        {
            using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "Resource", resource } }))
            {
                try
                {
                    using (var client = await GetWebAPIClientAsync())
                    {
                        var url = $"{apiBaseUrl}/{resource}";
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

        protected async Task<List<TEntity>> GetListAsync<TEntity>(string resource)// where TEntity : BaseEntity
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType(), new MethodCallParameters { { "Resource", resource } }))
            {
                try
                {
                    using (var client = await GetWebAPIClientAsync())
                    {
                        var url = $"{apiBaseUrl}/{resource}";
                        SAEONLogs.Verbose("Calling: {url}", url);
                        var response = await client.GetAsync(url);
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

        protected async Task<TOutput> GetEntityAsync<TOutput>(string resource)
        {
            using (SAEONLogs.MethodCall<TOutput>(GetType(), new MethodCallParameters { { "Resource", resource } }))
            {
                try
                {
                    using (var client = await GetWebAPIClientAsync())
                    {
                        var url = $"{apiBaseUrl}/{resource}";
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

        protected async Task<TOutput> GetEntityAsync<TInput, TOutput>(string resource, TInput input)
        {
            string GenerateQueryString()
            {
                if (input == null)
                    return "";
                else
                    return $"?json={JsonConvert.SerializeObject(input)}";
            }

            using (SAEONLogs.MethodCall<TOutput>(GetType(), new MethodCallParameters { { "Resource", resource } }))
            {
                try
                {
                    using (var client = await GetWebAPIClientAsync())
                    {
                        var url = $"{apiBaseUrl}/{resource}" + GenerateQueryString();
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

        protected async Task<TOutput> PostEntityAsync<TInput, TOutput>(string resource, TInput input)
        {
            using (SAEONLogs.MethodCall<TOutput>(GetType(), new MethodCallParameters { { "Resource", resource } }))
            {
                try
                {
                    using (var client = await GetWebAPIClientAsync())
                    {
                        var url = $"{apiBaseUrl}/{resource}";
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
    }

    public class BaseController<TModel> : BaseController where TModel : BaseModel, new()
    {
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

        [HttpGet]
        protected JsonResult GetListAsJson<TEntity>(List<TEntity> list)
        {
            var result = Json(list, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }

    }
}
