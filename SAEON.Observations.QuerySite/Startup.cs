using IdentityModel.Client;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;

[assembly: OwinStartupAttribute(typeof(SAEON.Observations.QuerySite.Startup))]

namespace SAEON.Observations.QuerySite
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            using (Logging.MethodCall(this.GetType()))
            {
                try
                {
                    Logging.Verbose("IdentityServer: {name}", Properties.Settings.Default.IdentityServer);
                    AntiForgeryConfig.UniqueClaimTypeIdentifier = Constants.Subject;
                    JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();
                    app.UseCors(CorsOptions.AllowAll);
                    app.UseCookieAuthentication(new CookieAuthenticationOptions
                    {
                        AuthenticationType = "Cookies"
                    });
                    app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
                    {
                        Authority = Properties.Settings.Default.IdentityServer,
                        ClientId = "SAEON.Observations.QuerySite",
                        //Scope = "openid profile email roles offline_access SAEON.Observations.WebAPI",
                        Scope = "openid profile roles SAEON.Observations.WebAPI",
                        //ResponseType = "code id_token token",
                        ResponseType = "id_token token code",
                        RedirectUri = "http://localhost:50160/",
                        //TokenValidationParameters = new TokenValidationParameters
                        //{
                        //    NameClaimType = "name",
                        //    RoleClaimType = "role"
                        //},
                        SignInAsAuthenticationType = "Cookies",
                        UseTokenLifetime = false,
                        Notifications = new OpenIdConnectAuthenticationNotifications
                        {
                            AuthorizationCodeReceived = async n =>
                            {
                                var identity = new ClaimsIdentity(n.AuthenticationTicket.Identity.AuthenticationType, "givven_name", "role");

                                var userInfoClient = new UserInfoClient(new Uri(n.Options.Authority + "/connect/userinfo"), n.ProtocolMessage.AccessToken);
                                var userInfo = await userInfoClient.GetAsync();

                                identity.AddClaims(userInfo.GetClaimsIdentity().Claims);

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

                                // get userinfo data
                                var userInfoClient = new UserInfoClient(new Uri(n.Options.Authority + "/connect/userinfo"), n.ProtocolMessage.AccessToken);

                                var userInfo = await userInfoClient.GetAsync();
                                identity.AddClaims(userInfo.GetClaimsIdentity().Claims);

                                // keep the id_token for logout
                                identity.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));

                                // add access token for sample API
                                identity.AddClaim(new Claim("access_token", n.ProtocolMessage.AccessToken));

                                // keep track of access token expiration
                                identity.AddClaim(new Claim("expires_at", DateTimeOffset.Now.AddSeconds(int.Parse(n.ProtocolMessage.ExpiresIn)).ToString()));

                                // add some other app specific claim
                                identity.AddClaim(new Claim("app_specific", "some data"));

                                n.AuthenticationTicket = new AuthenticationTicket(identity, n.AuthenticationTicket.Properties);
                            },
                            RedirectToIdentityProvider = n =>
                            {
                                if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
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