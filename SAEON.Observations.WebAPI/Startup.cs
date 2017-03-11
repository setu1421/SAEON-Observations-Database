using IdentityServer3.AccessTokenValidation;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using SAEON.Observations.Core;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;

[assembly: OwinStartup(typeof(SAEON.Observations.WebAPI.Startup))]

namespace SAEON.Observations.WebAPI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            using (this.MethodCall())
            {
                try
                {
                    AntiForgeryConfig.UniqueClaimTypeIdentifier = Constants.Subject;
                    JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();
                    app.UseCors(CorsOptions.AllowAll);
                    app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
                    {
                        Authority = Properties.Settings.Default.IdentityServer,
                        RequiredScopes = new[] { "SAEON.Observations.WebAPI" }
                    });

                    // add app local claims per request
                    app.UseClaimsTransformation(incoming =>
                    {
                        // either add claims to incoming, or create new principal
                        var appPrincipal = new ClaimsPrincipal(incoming);
                        //incoming.Identities.First().AddClaim(new Claim("appSpecific", "some_value"));

                        return Task.FromResult(appPrincipal);
                    });

                    // web api configuration
                    var config = new HttpConfiguration();
                    WebApiConfig.Register(config);
                    app.UseWebApi(config);
                }
                catch (Exception ex)
                {
                    this.ErrorInCall(ex, "Unable to configure application");
                    throw;
                }
            }
        }
    }
}