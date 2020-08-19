using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SAEON.Logs;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;

namespace SAEON.Observations.QuerySite.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IConfiguration config;

        public BaseController(IHttpContextAccessor httpContextAccessor, IConfiguration config) : base()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    this.config = config;
                    var httpContext = httpContextAccessor.HttpContext;
                    var tenant = httpContext.Session.GetString(TenantPolicyDefaults.HeaderKeyTenant);
                    if (string.IsNullOrWhiteSpace(tenant))
                    {
                        httpContext.Session.SetString(TenantPolicyDefaults.HeaderKeyTenant, config[TenantPolicyDefaults.ConfigKeyDefaultTenant]);
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        protected async Task<string> GetAccessToken()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var token = HttpContext.Session.GetString("AccessToken");
                    if (string.IsNullOrWhiteSpace(token))
                        using (var client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
                            using (var formContent = new FormUrlEncodedContent(new[] {
                                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                                new KeyValuePair<string, string>("scope", config["WebAPIClientID"]),
                                new KeyValuePair<string, string>("client_id", config["QuerySiteClientID"]),
                                new KeyValuePair<string, string>("client_secret", config["QuerySiteClientSecret"]),
                                }))
                            {
                                SAEONLogs.Verbose("Requesting access token");
                                var response = await client.PostAsync(config["AuthenticationServerUrl"] + "/oauth2/token", formContent);
                                if (!response.IsSuccessStatusCode)
                                {
                                    SAEONLogs.Error("HttpError: {StatusCode} {Reason}", response.StatusCode, response.ReasonPhrase);
                                    SAEONLogs.Error("Response: {Response}", await response.Content.ReadAsStringAsync());
                                }
                                response.EnsureSuccessStatusCode();
                                token = await response.Content.ReadAsStringAsync();
                                if (string.IsNullOrWhiteSpace(token)) throw new InvalidOperationException("Token is invalid");
                                HttpContext.Session.SetString("AccessToken", token);
                            }
                        }
                    SAEONLogs.Verbose("Token: {Token}", token);
                    return token;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        protected async Task<HttpClient> GetWebAPIClient(bool useAccessToken = true)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Zip));
                    client.DefaultRequestHeaders.Add(TenantPolicyDefaults.HeaderKeyTenant, HttpContext.Session.GetString(TenantPolicyDefaults.HeaderKeyTenant));
                    if (useAccessToken)
                    {
                        client.SetBearerToken(await GetAccessToken());
                    }
                    client.BaseAddress = new Uri(config["WebAPIUrl"]);
                    return client;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

    }
}
