using IdentityModel.Client;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;

namespace SAEON.Observations.MVCTest
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Seq("http://localhost:5341/")
                .CreateLogger();

            AntiForgeryConfig.UniqueClaimTypeIdentifier = "sub";
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                Authority = "https://localhost:44311/oauth2",
                ClientId = "mvc",
                RedirectUri = "http://localhost:62751/",
                ResponseType = "code id_token token",
                Scope = "openid profile roles sampleApi",
                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                },
                SignInAsAuthenticationType = "Cookies",
                UseTokenLifetime = false,
                //Notifications = new OpenIdConnectAuthenticationNotifications
                //{
                //    SecurityTokenValidated = n =>
                //    {
                //        var id = n.AuthenticationTicket.Identity;

                //        // we want to keep first name, last name, subject and roles
                //        var givenName = id.FindFirst("given_name");
                //        var familyName = id.FindFirst("family_name");
                //        var sub = id.FindFirst("sub");
                //        var roles = id.FindAll("role");

                //        // create new identity and set name and role claim type
                //        var nid = new ClaimsIdentity(
                //            id.AuthenticationType,
                //            "given_name",
                //            "role");

                //        nid.AddClaim(givenName);
                //        nid.AddClaim(familyName);
                //        nid.AddClaim(sub);
                //        nid.AddClaims(roles);

                //        // add some other app specific claim
                //        nid.AddClaim(new Claim("app_specific", "some data"));

                //        n.AuthenticationTicket = new AuthenticationTicket(
                //            nid,
                //            n.AuthenticationTicket.Properties);

                //        return Task.FromResult(0);
                //    }
                //}
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthorizationCodeReceived = async n =>
                    {
                        var userInfoClient = new UserInfoClient(new Uri("https://localhost:44311/oauth2/connect/userinfo"), n.ProtocolMessage.AccessToken);
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
            app.UseResourceAuthorization(new AuthorizationManager());
        }


    }
}