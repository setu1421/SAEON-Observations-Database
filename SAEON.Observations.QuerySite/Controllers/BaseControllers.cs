using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using SAEON.Logs;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SAEON.Observations.QuerySite.Controllers
{
    public class BaseController : Controller
    {
        private IConfiguration config;
        protected IConfiguration Config
        {
            get
            {
                return config ??= HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            }
        }

        protected string Tenant
        {
            get
            {
                var tenant = HttpContext.Session.GetString(TenantAuthenticationDefaults.HeaderKeyTenant);
                if (string.IsNullOrWhiteSpace(tenant))
                {
                    tenant = Config[TenantAuthenticationDefaults.ConfigKeyDefaultTenant];
                    HttpContext.Session.SetString(TenantAuthenticationDefaults.HeaderKeyTenant, tenant);
                }
                return tenant;
            }
            set
            {
                HttpContext.Session.SetString(TenantAuthenticationDefaults.HeaderKeyTenant, value);
            }
        }

        private async Task<string> GetAccessTokenAsync(bool useSession = true)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var token = useSession ? HttpContext.Session.GetString(ODPAuthenticationDefaults.SessionAccessToken) : null;
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        using (var client = new HttpClient())
                        {
                            using (var formContent = new FormUrlEncodedContent(new[] {
                                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                                new KeyValuePair<string, string>("scope", ODPAuthenticationDefaults.WebAPIClientId),
                                new KeyValuePair<string, string>("client_id", ODPAuthenticationDefaults.QuerySiteClientId),
                                new KeyValuePair<string, string>("client_secret", Config["QuerySiteClientSecret"]),
                                }))
                            {
                                //SAEONLogs.Information("scope: {scope} client_id: {client_id} client_secret: {client_secret}", ODPAuthenticationDefaults.WebAPIClientId, ODPAuthenticationDefaults.QuerySiteClientId), config["QuerySiteClientSecret"]);
                                SAEONLogs.Verbose("Requesting access token");
                                var response = await client.PostAsync(Config["AuthenticationServerUrl"] + "/oauth2/token", formContent);
                                if (!response.IsSuccessStatusCode)
                                {
                                    SAEONLogs.Error("HttpError: {StatusCode} {Reason}", response.StatusCode, response.ReasonPhrase);
                                    SAEONLogs.Error("Response: {Response}", await response.Content.ReadAsStringAsync());
                                }
                                response.EnsureSuccessStatusCode();
                                token = await response.Content.ReadAsStringAsync();
                                if (!string.IsNullOrWhiteSpace(token))
                                {
                                    HttpContext.Session.SetString(ODPAuthenticationDefaults.SessionAccessToken, token);
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

        private async Task<string> GetIdTokenAsync(bool useSession = true)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var token = useSession ? HttpContext.Session.GetString(ODPAuthenticationDefaults.SessionIdToken) : null;
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        token = await HttpContext.GetTokenAsync("access_token");
                        if (!string.IsNullOrWhiteSpace(token))
                        {
                            HttpContext.Session.SetString(ODPAuthenticationDefaults.SessionIdToken, token);
                        }
                    }
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
                    client.DefaultRequestHeaders.Add(TenantAuthenticationDefaults.HeaderKeyTenant, Tenant);
                    client.BaseAddress = new Uri(Config["WebAPIUrl"]);
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
                    var client = GetWebAPIClient();
                    client.SetBearerToken(GetBearerTokenFromAccessToken(await GetAccessTokenAsync(useSession)));
                    return client;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        protected async Task<HttpClient> GetWebAPIClientWithIdTokenAsync(bool useSession = true)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var client = GetWebAPIClient();
                    client.SetBearerToken(await GetIdTokenAsync(useSession));
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
