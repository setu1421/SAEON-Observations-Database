using IdentityServer3.AccessTokenValidation;
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
                    AntiForgeryConfig.UniqueClaimTypeIdentifier = Constants.ClaimSubject;
                    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

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
                    });

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