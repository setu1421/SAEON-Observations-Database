using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using SAEON.AspNet.Auth;

namespace SAEON.Observations.Auth
{
    public static class ObsDBAuthenticationDefaults
    {
        public const string AllowedClientsPolicy = "AllowedClientsPolicy";
        public const string ConfigKeyAuthenticationServerUrl = "AuthenticationServerUrl";
        public const string ConfigKeyAuthenticationServerIntrospectionUrl = "AuthenticationServerIntrospectionUrl";
        public const string CorsAllowAllPolicy = "CorsAllowAllPolicy";
        public const string CorsAllowQuerySitePolicy = "CorsAllowQuerySitePolicy";
        public const string CorsAllowSignalRPolicy = "CorsAllowSignalRPolicy";
        public const string DeniedClientsPolicy = "DeniedClientsPolicy";
        public const string QuerySiteClientId = "SAEON.Observations.QuerySite";
        public const string SessionKeyODPAccessToken = "ODPAccessToken";
        public const string WebAPIClientId = "SAEON.Observations.WebAPI";
        public const string WebAPIPostmanClientId = "SAEON.Observations.WebAPI.Postman";
        public const string WebAPISwaggerClientId = "SAEON.Observations.WebAPI.Swagger";
    }

    public static class ObsDBAuthenticationExtensions
    {
        public static void AddAllowedClientsPolicy(this AuthorizationOptions options)
        {
            options.AddPolicy(ObsDBAuthenticationDefaults.AllowedClientsPolicy, policy =>
            {
                policy.AddAuthenticationSchemes(ODPAuthenticationDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ODPAuthenticationDefaults.ClientIdClaim, ObsDBAuthenticationDefaults.QuerySiteClientId);
            });
        }

        public static void AddDeniedClientsPolicy(this AuthorizationOptions options)
        {
            options.AddPolicy(ObsDBAuthenticationDefaults.DeniedClientsPolicy, policy =>
            {
                policy.AddAuthenticationSchemes(ODPAuthenticationDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                    !context.User.HasClaim(ODPAuthenticationDefaults.ClientIdClaim, ObsDBAuthenticationDefaults.WebAPIPostmanClientId) &&
                    !context.User.HasClaim(ODPAuthenticationDefaults.ClientIdClaim, ObsDBAuthenticationDefaults.WebAPISwaggerClientId));
            });
        }

        public static void AddSAEONPolicies(this AuthorizationOptions options)
        {
            options.AddAllowedClientsPolicy();
            options.AddDeniedClientsPolicy();
        }

        public static void AddCorsAllowAllPolicy(this CorsOptions options)
        {
            options.AddPolicy(ObsDBAuthenticationDefaults.CorsAllowAllPolicy, b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
        }

        public static void AddCorsAllowQuerySitePolicy(this CorsOptions options)
        {
            options.AddPolicy(ObsDBAuthenticationDefaults.CorsAllowQuerySitePolicy, b => b
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithOrigins("https://localhost:44340", "https://observations.saeon.ac.za", "https://observations-test.saeon.ac.za"));
        }

        public static void AddCorsAllowSignalRPolicy(this CorsOptions options)
        {
            options.AddPolicy(ObsDBAuthenticationDefaults.CorsAllowSignalRPolicy, b => b
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .WithOrigins("https://localhost:44340", "https://observations.saeon.ac.za", "https://observations-test.saeon.ac.za"));
        }

        public static void AddSAEONCorsPolicies(this CorsOptions options)
        {
            options.AddCorsAllowAllPolicy();
            options.AddCorsAllowQuerySitePolicy();
            options.AddCorsAllowSignalRPolicy();
        }
    }

}
