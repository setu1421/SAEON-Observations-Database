using IdentityServer3.AccessTokenValidation;
using Owin;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SAEON.Observations.WebAPITest
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Seq("http://localhost:5341/")
                .CreateLogger();
            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                Authority = @"https://localhost:44311/oauth2",
                RequiredScopes = new[] { "sampleAPI" }
            });

            // web api configuration
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            config.EnableCors();
            app.UseWebApi(config);
        }
    }
}