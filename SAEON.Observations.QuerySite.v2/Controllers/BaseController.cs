using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.Core.Authentication;
using System;
using System.Collections.Generic;
using System.Net.Http;
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
                    var tenant = httpContext.Session.GetString(TenantAuthenticationDefaults.HeaderKeyTenant);
                    if (string.IsNullOrWhiteSpace(tenant))
                    {
                        httpContext.Session.SetString(TenantAuthenticationDefaults.HeaderKeyTenant, config[TenantAuthenticationDefaults.ConfigKeyDefaultTenant]);
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        private async Task<string> GetAccessToken(bool useSession = true)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var token = useSession ? HttpContext.Session.GetString("AccessToken") : null;
                    //if (!string.IsNullOrEmpty(token))
                    //{
                    //    var jObj = JObject.Parse(token);
                    //    var expires = DateTimeOffset.FromUnixTimeSeconds(jObj.Value<long>("exp"));
                    //    SAEONLogs.Information("Expires: {Expires}", expires);
                    //}
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        using (var client = new HttpClient())
                        {
                            using (var formContent = new FormUrlEncodedContent(new[] {
                                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                                new KeyValuePair<string, string>("scope", config["WebAPIClientID"]),
                                new KeyValuePair<string, string>("client_id", config["QuerySiteClientID"]),
                                new KeyValuePair<string, string>("client_secret", config["QuerySiteClientSecret"]),
                                }))
                            {
                                SAEONLogs.Information("scope: {scope} client_id: {client_id} client_secret: {client_secret}", config["WebAPIClientID"], config["QuerySiteClientID"], config["QuerySiteClientSecret"]);
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

        private async Task<string> GetIdToken()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var token = await HttpContext.GetTokenAsync("access_token");
                    if (string.IsNullOrEmpty(token))
                    {
                        SAEONLogs.Error("The access token cannot be found in the authentication ticket. " +
                                                            "Make sure that SaveTokens is set to true in the OIDC options.");
                        throw new InvalidOperationException("The access token cannot be found in the authentication ticket. " +
                                                            "Make sure that SaveTokens is set to true in the OIDC options.");
                    }
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

        protected HttpClient GetWebAPIClient()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Add(TenantAuthenticationDefaults.HeaderKeyTenant, HttpContext.Session.GetString(TenantAuthenticationDefaults.HeaderKeyTenant));
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

        protected async Task<HttpClient> GetWebAPIClientWithAccessToken(bool useSession = true)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var client = GetWebAPIClient();
                    client.SetBearerToken(GetBearerTokenFromAccessToken(await GetAccessToken(useSession)));
                    return client;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        protected async Task<HttpClient> GetWebAPIClientWithIdToken()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var client = GetWebAPIClient();
                    client.SetBearerToken(await GetIdToken());
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
