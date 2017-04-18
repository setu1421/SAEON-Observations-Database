﻿using SAEON.Observations.Core;
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
        private string sessionModelKey;
        //private static string identityUrl = Properties.Settings.Default.IdentityServerUrl;

        public BaseWebApiController() : base()
        {
            sessionModelKey = this.GetType().Name + "Model";
        }

        private HttpSessionState CurrentSession { get { return System.Web.HttpContext.Current.Session; } }

        protected TEntity GetSessionModel<TEntity>() where TEntity : class, new()
        {
            if (CurrentSession[sessionModelKey] == null)
            {
                CurrentSession[sessionModelKey] = new TEntity();
            }
            return (TEntity)CurrentSession[sessionModelKey];

        }

        protected void SetSessionModel<TEntity>(TEntity value)
        {
            CurrentSession[sessionModelKey] = value;
        }

        protected void RemoveSessionModel()
        {
            CurrentSession.Remove(sessionModelKey);
        }

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

        protected async Task<TEntity> Post<TEntity, TInput>(string resource, TInput input)// where TEntity : BaseEntity
        {
            using (Logging.MethodCall<TEntity>(this.GetType(), new ParameterList { { "Resource", resource }, { "Input", input } }))
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
                        var response = await client.PostAsJsonAsync(url, input);
                        Logging.Verbose("Response: {response}", response);
                        response.EnsureSuccessStatusCode();
                        var data = await response.Content.ReadAsAsync<TEntity>();
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

        protected async Task<IEnumerable<TEntity>> PostList<TEntity,TParams>(string resource, TParams parameters)// where TEntity : BaseEntity
        {
            using (Logging.MethodCall<TEntity>(this.GetType(), new ParameterList { { "Resource", resource }, { "Parameters", parameters} }))
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
                        var response = await client.PostAsJsonAsync(url,parameters);
                        Logging.Verbose("Response: {response}", response);
                        response.EnsureSuccessStatusCode();
                        var data = await response.Content.ReadAsAsync<IEnumerable<TEntity>>();
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