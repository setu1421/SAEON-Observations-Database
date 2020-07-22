using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using SAEON.AspNet.Common;
using SAEON.Logs;
using Syncfusion.Licensing;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Web;
using System.Web.Helpers;

[assembly: OwinStartupAttribute(typeof(SAEON.Observations.QuerySite.Startup))]

namespace SAEON.Observations.QuerySite
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    Logging.Verbose("IdentityServer: {name} RequireHttps: {RequireHTTPS}", Properties.Settings.Default.IdentityServerUrl, Properties.Settings.Default.RequireHTTPS);
                    AntiForgeryConfig.UniqueClaimTypeIdentifier = AspNetConstants.ClaimSubject;
                    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
                    SyncfusionLicenseProvider.RegisterLicense("MjkxOTc3QDMxMzgyZTMyMmUzMFdWUG1odVA0SURHM05OckJEaHEwUCt5U25KNytjVnpwNkViOFVuamxKd2c9;MjkxOTc4QDMxMzgyZTMyMmUzMFJJTXA2Mzg1Si8vZXZHK2JrK1VaYnhBVHFGRTU2emFjZmZ6V0E0YUhBNUE9;MjkxOTc5QDMxMzgyZTMyMmUzMEZiSzhqbEFWYUZQaEVxR1dQY3YvWGRTQjlBQVVYYjhsSnlUQTg5aDJFWEk9");

                    //app.UseResourceAuthorization(new AuthorizationManager());
                    app.UseCookieAuthentication(new CookieAuthenticationOptions
                    {
                        AuthenticationType = "Cookies",
                        ////CookieName = "SAEON.Observations.QuerySite",
                        ////ExpireTimeSpan = TimeSpan.FromDays(7),
                        ////SlidingExpiration = true
                    });
                    app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
                    {
                        Authority = Properties.Settings.Default.IdentityServerUrl,
                        ClientId = "SAEON.Observations.QuerySite",
                        Scope = "openid profile email roles SAEON.Observations.WebAPI offline_access",
                        ResponseType = "id_token code token",
                        RedirectUri = Properties.Settings.Default.QuerySiteUrl + "/signin-oidc",
                        PostLogoutRedirectUri = Properties.Settings.Default.QuerySiteUrl + "/signout-callback-oidc",
                        ////TokenValidationParameters = new TokenValidationParameters
                        ////{
                        ////    NameClaimType = JwtClaimTypes.Name,
                        ////    RoleClaimType = JwtClaimTypes.Role
                        ////},
                        SignInAsAuthenticationType = "Cookies",
                        RequireHttpsMetadata = Properties.Settings.Default.RequireHTTPS && !HttpContext.Current.Request.IsLocal,

                        UseTokenLifetime = false,

                        RedeemCode = true,
                        SaveTokens = true,
                        ClientSecret = "It6fWPU5J708"

                        /*
                        Notifications = new OpenIdConnectAuthenticationNotifications
                        {
                            AuthorizationCodeReceived = async n =>
                            {
                                Logging.Verbose("AuthorizationCodeReceived notification");
                                var identity = new ClaimsIdentity(n.AuthenticationTicket.Identity.AuthenticationType);
                                Logging.Verbose("IsAuthenticated: {IsAuthenticated} ", identity.IsAuthenticated);

                                var discoClient = new HttpClient();
                                var disco = await discoClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
                                {
                                    Address = Properties.Settings.Default.IdentityServerUrl,
                                    Policy = { RequireHttps = Properties.Settings.Default.RequireHTTPS && !HttpContext.Current.Request.IsLocal }
                                });
                                if (disco.IsError)
                                {
                                    Logging.Error("Disco Error: {error}", disco.Error);
                                    throw new HttpException(disco.Error);
                                }

                                var userInfoClient = new HttpClient();
                                var userInfoResponse = await userInfoClient.GetUserInfoAsync(new UserInfoRequest
                                {
                                    Address = disco.UserInfoEndpoint,
                                    Token = n.ProtocolMessage.AccessToken
                                });
                                if (userInfoResponse.IsError)
                                {
                                    Logging.Error("UserInfo Error: {error}", userInfoResponse.Error);
                                    throw new HttpException(userInfoResponse.Error);

                                }
                                identity.AddClaims(userInfoResponse.Claims);

                                Logging.Verbose("Code: {Code} RedirectURI: {RedirectURI}", n.Code, n.RedirectUri);
                                var tokenClient = new HttpClient();
                                var tokenResponse = await tokenClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
                                {
                                    Address = disco.TokenEndpoint,
                                    ClientId = "SAEON.Observations.QuerySite",
                                    ClientSecret = "It6fWPU5J708",
                                    Code = n.Code,
                                    RedirectUri = n.RedirectUri
                                });
                                if (tokenResponse.IsError)
                                {
                                    Logging.Error("Token Error: {error}", tokenResponse.Error);
                                    throw new HttpException(tokenResponse.Error);
                                }

                                identity.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));
                                identity.AddClaim(new Claim("access_token", n.ProtocolMessage.AccessToken));
                                identity.AddClaim(new Claim("refresh_token", tokenResponse.RefreshToken));
                                identity.AddClaim(new Claim("expires_at", DateTimeOffset.Now.AddSeconds(int.Parse(n.ProtocolMessage.ExpiresIn)).ToString("o")));

                                n.AuthenticationTicket = new AuthenticationTicket(identity, n.AuthenticationTicket.Properties);
                            },
                            RedirectToIdentityProvider = n =>
                                {
                                    Logging.Verbose("RedirectToIdentityProvider notification");
                                    if (n.ProtocolMessage.RequestType == Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectRequestType.Logout)
                                    {
                                        var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token");
                                        if (idTokenHint != null)
                                        {
                                            n.ProtocolMessage.IdTokenHint = idTokenHint.Value;
                                        }
                                    }
                                    return Task.FromResult(0);
                                }
                        },
                        */
                    });
                    // Make sure WebAPI is available
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to configure application");
                    throw;
                }
            }
        }
    }
}