using Owin;
using System.IdentityModel.Tokens;
using IdentityServer3.AccessTokenValidation;

namespace SAEON.Observations.WebAPI
{
    public partial class Startup
    {
        private const string IdServBaseUri = @"https://localhost:44311/oauth2";
        private const string IdentityUri = IdServBaseUri + @"/identity";


        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            //JwtSecurityTokenHandler.InboundClaimTypeMap.Clear();
            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                Authority = IdServBaseUri,
                RequiredScopes = new[] { "SAEON.Observations.WebAPI" },
                
            });
            app.UseWebApi(WebApiConfig.Register());
        }
    }
}
