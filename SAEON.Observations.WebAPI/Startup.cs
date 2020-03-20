using IdentityServer3.AccessTokenValidation;
using Microsoft.IdentityModel.Logging;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using SAEON.AspNet.Common;
using SAEON.Logs;
using System;
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
                    AntiForgeryConfig.UniqueClaimTypeIdentifier = AspNetConstants.ClaimSubject;
                    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
                    var identityServerUri = new Uri(Properties.Settings.Default.IdentityServerUrl);
                    IdentityModelEventSource.ShowPII = identityServerUri.Scheme.ToLowerInvariant() == "https";

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
                        CookieName = "SAEON.Observtions.WebAPI",
                        ExpireTimeSpan = TimeSpan.FromDays(7),
                        SlidingExpiration = true
                    });
                    app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
                    {
                        Authority = Properties.Settings.Default.IdentityServerUrl,
                        RequiredScopes = new[] { "SAEON.Observations.WebAPI" },
                        RequireHttps = identityServerUri.Scheme.ToLowerInvariant() == "https"
                    });

                    // web api configuration
                    var config = new HttpConfiguration();
                    config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
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