using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Newtonsoft.Json.Linq;
using Owin;
using SAEON.AspNet.Auth;
using SAEON.Core;
using SAEON.Logs;
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
using System.Web.Helpers;

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
                    SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBPh8sVXJ0S0V+XE9AcVRDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS3xSdkVmWXdfd3FXRGFdVQ==;Mgo+DSMBMAY9C3t2VVhjQlFaclhJXGFWfVJpTGpQdk5xdV9DaVZUTWY/P1ZhSXxRd0VjXH9WcXNQRGJeVEA=");
                    AntiForgeryConfig.UniqueClaimTypeIdentifier = Constants.AntiForgeryClaim;
                    //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

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
                        RedirectUri = ConfigurationManager.AppSettings["QuerySiteUrl"].AddEndForwardSlash() + "signin-oidc",
                        PostLogoutRedirectUri = ConfigurationManager.AppSettings["QuerySiteUrl"].AddEndForwardSlash(),
                        ResponseType = "code",
                        SignInAsAuthenticationType = "Cookies",
                        SaveTokens = true,
                        RedeemCode = true,
                        Notifications = new OpenIdConnectAuthenticationNotifications
                        {
                            SecurityTokenReceived = (context) =>
                            {
                                SAEONLogs.Verbose("*** SecurityTokenReceived {@ProtocolMessage}", context.ProtocolMessage);
                                return Task.FromResult(0);
                            },
                            SecurityTokenValidated = async (context) =>
                            {
                                SAEONLogs.Verbose("*** SecurityTokenValidated {@ProtocolMessage}", context.ProtocolMessage);
                                var loginReferer = context.Response.Headers["LoginReferer"];
                                //SAEONLogs.Information("LoginReferer: {LoginReferer}", loginReferer);
                                var accessToken = context.ProtocolMessage.AccessToken;
                                var idToken = context.ProtocolMessage.IdToken;
                                if (string.IsNullOrWhiteSpace(accessToken))
                                {
                                    SAEONLogs.Error("ODPAuthorization Failed, no access token");
                                    throw new SecurityTokenValidationException("ODPAuthorization Failed, no acess token");
                                }
                                //if (string.IsNullOrWhiteSpace(idToken))
                                //{
                                //    SAEONLogs.Error("ODPAuthorization Failed, no id token");
                                //    throw new SecurityTokenValidationException("ODPAuthorization Failed, no id token");
                                //}
                                SAEONLogs.Verbose("AccessToken: {AccessToken} IdToken: {IdToken}", accessToken, idToken);
                                // Validate token
                                using (var client = new HttpClient())
                                {
                                    client.SetBearerToken(accessToken);
                                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.ApplicationJson));
                                    using (var formContent = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("token", accessToken) }))
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
                                            SAEONLogs.Error("ODPAuthorization, invalid token {Token}", accessToken);
                                            throw new SecurityTokenValidationException("Invalid token");
                                        }
                                        if (jObj["ext"] is null)
                                        { // Access token
                                            var clientId = jObj.Value<string>("client_id");
                                            var claims = new List<Claim> {
                                                new Claim(Constants.ClientIdClaim, clientId),
                                                new Claim(Constants.AccessTokenClaim, accessToken)
                                            };
                                            SAEONLogs.Debug("ODPAuthentication id token succeeded Claims: {@Claims}", claims.ToClaimsList());
                                            foreach (var claim in claims)
                                            {
                                                if (!context.AuthenticationTicket.Identity.HasClaim(i => (i.Type == claim.Type) && (i.Value == claim.Value)))
                                                {
                                                    context.AuthenticationTicket.Identity.AddClaim(claim);
                                                }
                                            }
                                            //context.AuthenticationTicket.Identity.AddClaims(claims);
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
                                                new Claim(Constants.AccessTokenClaim, accessToken),
                                                new Claim(Constants.IdTokenClaim, idToken),
                                                new Claim(ClaimTypes.NameIdentifier,userId),
                                                new Claim(ClaimTypes.Email,userEmail),
                                                new Claim("LoginReferer",loginReferer),
                                            };
                                            foreach (var userRole in userRoles)
                                            {
                                                claims.Add(new Claim(ClaimTypes.Role, userRole));
                                            }
                                            if (userRoles.Contains("admin"))
                                            {
                                                claims.Add(new Claim(Constants.AdminTokenClaim, true.ToString()));
                                            }
                                            foreach (var claim in claims)
                                            {
                                                if (!context.AuthenticationTicket.Identity.HasClaim(i => (i.Type == claim.Type) && (i.Value == claim.Value)))
                                                {
                                                    context.AuthenticationTicket.Identity.AddClaim(claim);
                                                }
                                            }
                                            //context.AuthenticationTicket.Identity.AddClaims(claims);
                                            SAEONLogs.Verbose("ODPAuthentication id token succeeded Claims: {@Claims}", claims.ToClaimsList());
                                        }
                                    }
                                }
                            },
                            RedirectToIdentityProvider = (context) =>
                            {
                                SAEONLogs.Verbose("*** RedirectToIdentityProvider {@ProtocolMessage}", context.ProtocolMessage);
                                if (context.ProtocolMessage.RequestType == OpenIdConnectRequestType.Authentication)
                                {
                                    var loginReferer = context.Request.Headers["Referer"];
                                    context.Response.Headers["LoginReferer"] = loginReferer;
                                    //SAEONLogs.Information("LoginReferer: {LoginReferer}", loginReferer);
                                    SAEONLogs.Verbose("State: {State}", context.ProtocolMessage.State);
                                    var stateQueryString = context.ProtocolMessage.State.Split('=');
                                    var protectedState = stateQueryString[1];
                                    var state = context.Options.StateDataFormat.Unprotect(protectedState);
                                    state.Dictionary.Add("LoginReferer", loginReferer);
                                    context.ProtocolMessage.State = stateQueryString[0] + "=" + context.Options.StateDataFormat.Protect(state);
                                    if (context.OwinContext.Authentication.AuthenticationResponseChallenge?.Properties.Dictionary.ContainsKey("ODPRegister") ?? false)
                                    {
                                        SAEONLogs.Verbose("Enabling registration");
                                        context.ProtocolMessage.SetParameter("mode", "signup");
                                    }
                                }
                                else if (context.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
                                {
                                    SAEONLogs.Verbose("Claims: {claims}", context.OwinContext.Authentication.User.Claims.ToClaimsList());
                                    var idTokenHint = context.OwinContext.Authentication.User.FindFirst(Constants.IdTokenClaim)?.Value;
                                    context.ProtocolMessage.IdTokenHint = idTokenHint;
                                }
                                SAEONLogs.Verbose("*** RedirectToIdentityProvider {@ProtocolMessage}", context.ProtocolMessage);
                                return Task.FromResult(0);
                            },
                            MessageReceived = (context) =>
                            {
                                SAEONLogs.Verbose("*** MessageReceived {@ProtocolMessage}", context.ProtocolMessage);
                                return Task.FromResult(0);
                            },
                            AuthorizationCodeReceived = (context) =>
                            {
                                SAEONLogs.Verbose("*** AuthorizationCodeReceived {@ProtocolMessage}", context.ProtocolMessage);
                                return Task.FromResult(0);
                            },
                            AuthenticationFailed = (context) =>
                            {
                                SAEONLogs.Verbose("*** AuthenticationFailed {@ProtocolMessage}", context.ProtocolMessage);
                                return Task.FromResult(0);
                            },
                            TokenResponseReceived = (context) =>
                            {
                                SAEONLogs.Verbose("*** TokenResponseReceived {@ProtocolMessage}", context.ProtocolMessage);
                                var protectedState = context.ProtocolMessage.State.Split('=')[1];
                                var state = context.Options.StateDataFormat.Unprotect(protectedState);
                                state.Dictionary.TryGetValue("LoginReferer", out string loginReferer);
                                //SAEONLogs.Information("LoginReferer: {LoginReferer}", loginReferer);
                                context.Response.Headers["LoginReferer"] = loginReferer;
                                return Task.FromResult(0);
                            }
                        }
                    });
                    // Make sure ODP is available
                    // Make sure WebAPI is available
                    //var client = new HttpClient();
                    //client.DefaultRequestHeaders.Accept.Clear();
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.ApplicationJson));
                    //var url = ConfigurationManager.AppSettings["QuerySiteHealthCheckUrl"];
                    //SAEONLogs.Verbose("Calling: {url}", url);
                    //var response = await client.GetAsync(url);
                    //SAEONLogs.Verbose("Response: {response}", response);
                    //response.EnsureSuccessStatusCode();
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