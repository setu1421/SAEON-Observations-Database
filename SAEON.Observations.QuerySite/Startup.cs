using IdentityModel.Client;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Helpers;

[assembly: OwinStartupAttribute(typeof(SAEON.Observations.QuerySite.Startup))]
namespace SAEON.Observations.QuerySite
{
    public partial class Startup
    {
        private const string ClientUri = @"http://localhost:58091/";
        //private const string CallbackEndpoint = ClientUri + @"/account/signInCallback";
        private const string IdServBaseUri = @"https://localhost:44311/oauth2";
        private const string AuthorizeUri = IdServBaseUri + @"/connect/authorize";
        private const string LogoutUri = IdServBaseUri + @"/connect/endsession";
        private const string UserInfoEndpoint = IdServBaseUri + @"/connect/userinfo";
        private const string TokenEndpoint = IdServBaseUri + @"/connect/token";

        public void Configuration(IAppBuilder app)
        {
            AntiForgeryConfig.UniqueClaimTypeIdentifier = "sub";
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();
            //{
            //    { "role", ClaimTypes.Role}
            //};

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = "SAEON.Observations.QuerySite",
                    Authority = IdServBaseUri,
                    RedirectUri = ClientUri,
                    PostLogoutRedirectUri = ClientUri,
                    ResponseType = "code id_token token",
                    Scope = "openid profile email roles offline_access",
                    TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    },
                    SignInAsAuthenticationType = "Cookies",
                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        AuthorizationCodeReceived = async n =>
                        {
                            var userInfoClient = new UserInfoClient(new Uri(UserInfoEndpoint), n.ProtocolMessage.AccessToken);
                            var userInfoResponse = await userInfoClient.GetAsync();

                            var identity = new ClaimsIdentity(n.AuthenticationTicket.Identity.AuthenticationType);
                            identity.AddClaims(userInfoResponse.GetClaimsIdentity().Claims);

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
    }
}
