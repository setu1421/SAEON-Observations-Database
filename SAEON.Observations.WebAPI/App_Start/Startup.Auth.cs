using Owin;
using System.IdentityModel.Tokens;
using IdentityServer3.AccessTokenValidation;

namespace SAEON.Observations.WebAPI
{
    public partial class Startup
    {
        private const string ClientUri = @"http://localhost:58091/";
        private const string CallbackEndpoint = ClientUri + @"/account/signInCallback";
        private const string IdServBaseUri = @"https://localhost:44311/oauth2";
        private const string AuthorizeUri = IdServBaseUri + @"/connect/authorize";
        private const string LogoutUri = IdServBaseUri + @"/connect/endsession";
        private const string UserInfoEndpoint = IdServBaseUri + @"/connect/userinfo";
        private const string TokenEndpoint = IdServBaseUri + @"/connect/token";


        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            JwtSecurityTokenHandler.InboundClaimTypeMap.Clear();
            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                Authority = AuthorizeUri,
                RequiredScopes = new[] { "write" },

                // client credentials for the introspection endpoint
                ClientId = "SAEON.Observations.WebAPI",
                ClientSecret = "81g5wyGSC89a"
            });
        }
    }
}
