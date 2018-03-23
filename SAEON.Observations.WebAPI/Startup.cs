using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using SAEON.Logs;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Web.Helpers;
using System.Web.Http;

[assembly: OwinStartup(typeof(SAEON.Observations.WebAPI.Startup))]

namespace SAEON.Observations.WebAPI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    Logging.Verbose("IdentityServer: {name}", Properties.Settings.Default.IdentityServerUrl);
                    AntiForgeryConfig.UniqueClaimTypeIdentifier = Constants.Subject;
                    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap = new Dictionary<string, string>();
                    app.UseCors(CorsOptions.AllowAll);
                    //app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
                    //{
                    //    Authority = Properties.Settings.Default.IdentityServerUrl,
                    //    RequiredScopes = new[] { "SAEON.Observations.WebAPI" },
                    //});

                    /*
                    // add app local claims per request
                    app.UseClaimsTransformation(incoming =>
                    {
                        // either add claims to incoming, or create new principal
                        var appPrincipal = new ClaimsPrincipal(incoming);
                        //incoming.Identities.First().AddClaim(new Claim("appSpecific", "some_value"));

                        return Task.FromResult(appPrincipal);
                    });
                    */

                    // web api configuration
                    var config = new HttpConfiguration();
                    WebApiConfig.Register(config);
                    app.UseWebApi(config);
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