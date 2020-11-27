using Newtonsoft.Json.Linq;
using SAEON.Core;
using SAEON.Logs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    public class BaseController : Controller
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

        protected async Task<string> GetAuthorizationAsync()
        {
            var token = Session[Constants.SessionKeyIdToken]?.ToString();
            if (string.IsNullOrWhiteSpace(token))
            {
                token = await GetAccessTokenAsync();
            }
            token = GetBearerTokenFromAccessToken(token);
            return $"Bearer {token}";
        }

        protected HttpClient GetWebAPIClient()
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

        protected HttpClient GetWebAPIClientWithIdToken(bool useSession = true)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var client = GetWebAPIClient();
                    client.SetBearerToken(GetIdToken(useSession));
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