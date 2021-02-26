using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Newtonsoft.Json.Linq;
using Owin;
using SAEON.Logs;
using SAEON.Observations.Auth;
using Syncfusion.Licensing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

[assembly: OwinStartupAttribute(typeof(SAEON.Observations.QuerySite.Startup))]

namespace SAEON.Observations.QuerySite
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    SAEONLogs.Verbose("AuthenticationServer: {AuthenticationServer}", ConfigurationManager.AppSettings["AuthenticationServerUrl"]);
                    SyncfusionLicenseProvider.RegisterLicense("Mzk0Nzc2QDMxMzgyZTM0MmUzMGlPREwxWmJGV3Rtck5GVmNNNzdlbXhFUk1IemVBWGZSbnV4aEt6MDcyOG89;Mzk0Nzc3QDMxMzgyZTM0MmUzMGtGdFdiM2FSamVMK295eWsrVGNjRzdZL2VkZk43R3c0NnhRT3hGbTU5RmM9");

                    app.UseCookieAuthentication(new CookieAuthenticationOptions
                    {
                        AuthenticationType = "Cookies",
                    });
                    app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
                    {
                        Authority = ConfigurationManager.AppSettings["AuthenticationServerUrl"],
                        ClientId = ConfigurationManager.AppSettings["QuerySiteClientId"],
                        ClientSecret = ConfigurationManager.AppSettings["QuerySiteClientSecret"],
                        Scope = $"openid {ConfigurationManager.AppSettings["WebAPIClientId"]}",
                        RedirectUri = ConfigurationManager.AppSettings["QuerySiteUrl"] + "/signin-oidc",
                        PostLogoutRedirectUri = ConfigurationManager.AppSettings["QuerySiteUrl"] + "/signout-callback-oidc",
                        ResponseType = "code",
                        SignInAsAuthenticationType = "Cookies",
                        SaveTokens = true,
                        RedeemCode = true,
                        Notifications = new OpenIdConnectAuthenticationNotifications
                        {
                            SecurityTokenValidated = async (context) =>
                            {
                                //SAEONLogs.Information("*** SecurityTokenValidated {@ProtocolMessage}", context.ProtocolMessage);
                                var token = context.ProtocolMessage.AccessToken;
                                if (string.IsNullOrWhiteSpace(token))
                                {
                                    SAEONLogs.Error("ODPAuthorization Failed, no token");
                                    throw new SecurityTokenValidationException("ODPAuthorization Failed, no token");
                                }
                                SAEONLogs.Debug("Token: {Token}", token);
                                // Validate token
                                using (var client = new HttpClient())
                                {
                                    client.SetBearerToken(token);
                                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.ApplicationJson));
                                    using (var formContent = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("token", token) }))
                                    {
                                        var response = await client.PostAsync(new Uri(ConfigurationManager.AppSettings["AuthenticationServerIntrospectionUrl"]), formContent).ConfigureAwait(false);
                                        if (!response.IsSuccessStatusCode)
                                        {
                                            SAEONLogs.Error("HttpError: {StatusCode} {Reason}", response.StatusCode, response.ReasonPhrase);
                                            SAEONLogs.Error("Response: {Response}", await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                                            throw new SecurityTokenValidationException();
                                        }
                                        response.EnsureSuccessStatusCode();
                                        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                                        SAEONLogs.Information("Response: {Response}", json);
                                        var jObj = JObject.Parse(json);
                                        var isActive = jObj.Value<bool>("active");
                                        if (!isActive)
                                        {
                                            SAEONLogs.Error("ODPAuthorization, invalid token {Token}", token);
                                            throw new SecurityTokenValidationException("Invalid token");
                                        }
                                        if (jObj["ext"] == null)
                                        { // Access token
                                            var clientId = jObj.Value<string>("client_id");
                                            var claims = new List<Claim> {
                                                new Claim(Constants.ClientIdClaim, clientId),
                                                new Claim(Constants.AccessTokenClaim, token)
                                            };
                                            SAEONLogs.Debug("ODPAuthentication id token succeeded Claims: {@Claims}", claims.ToClaimsList());
                                            context.AuthenticationTicket.Identity.AddClaims(claims);
                                        }
                                        else // Id token
                                        {
                                            var clientId = jObj.Value<string>("client_id");
                                            var userId = jObj.Value<string>("sub");
                                            //var userId = jObj["ext"].Value<string>("user_id");
                                            var userEmail = jObj["ext"].Value<string>("email");
                                            var userRoles = from r in jObj["ext"]["access_rights"] select (string)r["role_name"];
                                            SAEONLogs.Debug("User Id: {Id} Email: {Email}, Roles: {Role}", userId, userEmail, userRoles);
                                            var claims = new List<Claim> {
                                                new Claim(Constants.ClientIdClaim,clientId),
                                                new Claim(Constants.IdTokenClaim, token),
                                                new Claim(ClaimTypes.NameIdentifier,userId),
                                                new Claim(ClaimTypes.Email,userEmail)
                                            };
                                            foreach (var userRole in userRoles)
                                            {
                                                claims.Add(new Claim(ClaimTypes.Role, userRole));
                                            }
                                            if (userRoles.Contains("admin"))
                                            {
                                                claims.Add(new Claim(Constants.AdminTokenClaim, true.ToString()));
                                            }
                                            context.AuthenticationTicket.Identity.AddClaims(claims);
                                            SAEONLogs.Debug("ODPAuthentication id token succeeded Claims: {@Claims}", claims.ToClaimsList());
                                        }
                                    }
                                }
                            },
                            RedirectToIdentityProvider = (context) =>
                            {
                                SAEONLogs.Information("*** RedirectToIdentityProvider {@ProtocolMessage}", context.ProtocolMessage);
                                if (context.ProtocolMessage.RequestType == OpenIdConnectRequestType.Authentication)
                                {
                                    if (context.OwinContext.Authentication.AuthenticationResponseChallenge?.Properties.Dictionary.ContainsKey("ODPRegister") ?? false)
                                    {
                                        SAEONLogs.Information("Enabling registration");
                                        context.ProtocolMessage.SetParameter("mode", "signup");
                                    }
                                }
                                else if (context.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
                                {
                                    SAEONLogs.Information("Claims: {claims}", context.OwinContext.Authentication.User.Claims.ToClaimsList());
                                    var idTokenHint = context.OwinContext.Authentication.User.FindFirst(Constants.IdTokenClaim)?.Value;
                                    context.ProtocolMessage.IdTokenHint = idTokenHint;
                                }
                                SAEONLogs.Information("*** RedirectToIdentityProvider {@ProtocolMessage}", context.ProtocolMessage);
                                return Task.FromResult(0);
                            },
                            //MessageReceived = (context) =>
                            //{
                            //    SAEONLogs.Information("*** MessageReceived {@ProtocolMessage}", context.ProtocolMessage);
                            //    return Task.FromResult(0);
                            //},
                            //SecurityTokenReceived = (context) =>
                            //{
                            //    SAEONLogs.Information("*** SecurityTokenReceived {@ProtocolMessage}", context.ProtocolMessage);
                            //    return Task.FromResult(0);
                            //},
                            //SecurityTokenValidated = (context) =>
                            //{
                            //    SAEONLogs.Information("*** SecurityTokenValidated {@ProtocolMessage}", context.ProtocolMessage);
                            //    return Task.FromResult(0);
                            //},
                            //AuthorizationCodeReceived = (context) =>
                            //{
                            //    SAEONLogs.Information("*** AuthorizationCodeReceived {@ProtocolMessage}", context.ProtocolMessage);
                            //    return Task.FromResult(0);
                            //},
                            //AuthenticationFailed = (context) =>
                            //{
                            //    SAEONLogs.Information("*** AuthenticationFailed {@ProtocolMessage}", context.ProtocolMessage);
                            //    return Task.FromResult(0);
                            //},
                        }
                    });
                    // Make sure ODP is available
                    // Make sure WebAPI is available
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to configure application");
                    throw;
                }
            }
        }
    }
}