using IdentityServer3.AccessTokenValidation;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using SAEON.AspNet.Common;
using SAEON.Logs;
using System;
using System.IdentityModel.Tokens;
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
                    AntiForgeryConfig.UniqueClaimTypeIdentifier = Constants.ClaimSubject;
                    JwtSecurityTokenHandler.InboundClaimTypeMap.Clear();

                    //var corsPolicy = new CorsPolicy
                    //{
                    //    AllowAnyMethod = true,
                    //    AllowAnyHeader = true,
                    //    SupportsCredentials = true
                    //};
                    //Logging.Verbose("CORS Origin: {cors}", Properties.Settings.Default.QuerySiteUrl);
                    //corsPolicy.Origins.Add(Properties.Settings.Default.QuerySiteUrl);
                    //var corsOptions = new CorsOptions
                    //{
                    //    PolicyProvider = new CorsPolicyProvider
                    //    {
                    //        PolicyResolver = context => Task.FromResult(corsPolicy)
                    //    }
                    //};
                    //app.UseCors(corsOptions);
                    //app.UseCors(CorsOptions.AllowAll);

                    app.UseCookieAuthentication(new CookieAuthenticationOptions
                    {
                        AuthenticationType = "Cookies",
                        ExpireTimeSpan = new TimeSpan(7, 0, 0, 0),
                        SlidingExpiration = true
                    });
                    app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
                    {
                        Authority = Properties.Settings.Default.IdentityServerUrl,
                        RequiredScopes = new[] { "SAEON.Observations.WebAPI" },
                    });

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
                    config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

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