using IdentityModel.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using SAEON.Logs;
using SAEON.Observations.Core;
using Syncfusion.Licensing;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;

[assembly: OwinStartupAttribute(typeof(SAEON.Observations.QuerySite.Startup))]

namespace SAEON.Observations.QuerySite
{
    public class Startup
    {
        public Startup()
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            SyncfusionLicenseProvider.RegisterLicense("NTg2NTBAMzEzNjJlMzQyZTMwVjcvTjFsbHJOODZQaFR5eHlHeGZ6RTdCTHZOR3owa08xdXptTUNUVDBZYz0=;NTg2NTFAMzEzNjJlMzQyZTMwZCtCbjIwQjQ3YzdMQXpDMGJpTmJ2N0k4azFJU29lckdySndyUkFSbElhTT0=;NTg2NTJAMzEzNjJlMzQyZTMwV0RFa2VPbnFFUHhRdnRWeUo2OWRWVk5XOWhEL1pQZXUvcCtGTW9UZDNYND0=");
        }

        public void Configuration(IAppBuilder app)
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    Logging.Verbose("IdentityServer: {name}", Properties.Settings.Default.IdentityServerUrl);
                    AntiForgeryConfig.UniqueClaimTypeIdentifier = Constants.Subject;
                    app.UseCors(CorsOptions.AllowAll);
                    //app.UseResourceAuthorization(new AuthorizationManager());
                    app.UseCookieAuthentication(new CookieAuthenticationOptions
                    {
                        AuthenticationType = "Cookies",
                        ExpireTimeSpan = new TimeSpan(7, 0, 0, 0),
                        SlidingExpiration = true
                    });
                    app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
                    {
                        Authority = Properties.Settings.Default.IdentityServerUrl,
                        ClientId = "SAEON.Observations.QuerySite",
                        Scope = "openid profile email offline_access roles SAEON.Observations.WebAPI",
                        ResponseType = "id_token token code",
                        RedirectUri = Properties.Settings.Default.QuerySiteUrl + "/signin-oidc",
                        PostLogoutRedirectUri = Properties.Settings.Default.QuerySiteUrl,
                        TokenValidationParameters = new TokenValidationParameters
                        {
                            NameClaimType = "name",
                            RoleClaimType = "role"
                        },
                        SignInAsAuthenticationType = "Cookies",
                        UseTokenLifetime = false,
                        RequireHttpsMetadata = false,

                        Notifications = new OpenIdConnectAuthenticationNotifications
                        {
                            AuthorizationCodeReceived = async n =>
                            {
                                var identity = new ClaimsIdentity(n.AuthenticationTicket.Identity.AuthenticationType);

                                //var userInfoClient = new UserInfoClient(new Uri(n.Options.Authority + "/connect/userinfo").ToString());
                                //var userInfo = await userInfoClient.GetAsync(n.ProtocolMessage.AccessToken);
                                //identity.AddClaims(userInfo.Claims);
                                
                                var discoClient = new HttpClient();
                                var disco = await discoClient.GetDiscoveryDocumentAsync(Properties.Settings.Default.IdentityServerUrl);
                                if (disco.IsError)
                                {
                                    Logging.Error("Error: {error}", disco.Error);
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
                                    Logging.Error("Error: {error}", userInfoResponse.Error);
                                    throw new HttpException(userInfoResponse.Error);

                                }
                                identity.AddClaims(userInfoResponse.Claims);

                                // keep the id_token for logout
                                identity.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));

                                // add access token for sample API
                                identity.AddClaim(new Claim("access_token", n.ProtocolMessage.AccessToken));

                                // keep track of access token expiration
                                identity.AddClaim(new Claim("expires_at", DateTimeOffset.Now.AddSeconds(int.Parse(n.ProtocolMessage.ExpiresIn)).ToString()));

                                n.AuthenticationTicket = new AuthenticationTicket(identity, n.AuthenticationTicket.Properties);
                            },
                            SecurityTokenValidated = async n =>
                            {
                                var identity = new ClaimsIdentity(n.AuthenticationTicket.Identity.AuthenticationType, "given_name", "role");

                                //var userInfoClient = new UserInfoClient(new Uri(n.Options.Authority + "/connect/userinfo").ToString());
                                //var userInfo = await userInfoClient.GetAsync(n.ProtocolMessage.AccessToken);
                                //identity.AddClaims(userInfo.Claims);
                                
                                var discoClient = new HttpClient();
                                var disco = await discoClient.GetDiscoveryDocumentAsync(Properties.Settings.Default.IdentityServerUrl);
                                if (disco.IsError)
                                {
                                    Logging.Error("Error: {error}", disco.Error);
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
                                    Logging.Error("Error: {error}", userInfoResponse.Error);
                                    throw new HttpException(userInfoResponse.Error);

                                }
                                identity.AddClaims(userInfoResponse.Claims);

                                // keep the id_token for logout
                                identity.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));

                                // add access token for sample API
                                identity.AddClaim(new Claim("access_token", n.ProtocolMessage.AccessToken));

                                // keep track of access token expiration
                                identity.AddClaim(new Claim("expires_at", DateTimeOffset.Now.AddSeconds(int.Parse(n.ProtocolMessage.ExpiresIn)).ToString()));

                                n.AuthenticationTicket = new AuthenticationTicket(identity, n.AuthenticationTicket.Properties);
                            },
                            RedirectToIdentityProvider = n =>
                            {
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