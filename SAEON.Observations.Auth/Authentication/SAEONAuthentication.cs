using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace SAEON.Observations.Auth
{
    public static class SAEONAuthenticationDefaults
    {
        public const string AllowedClientsPolicy = "AllowedClientsPolicy";
        public const string CorsAllowAllPolicy = "CorsAllowAllPolicy";
        public const string CorsAllowQuerySitePolicy = "CorsAllowQuerySitePolicy";
        public const string DeniedClientsPolicy = "DeniedClientsPolicy";
        public const string QuerySiteClientId = "SAEON.Observations.QuerySite";
        public const string WebAPIClientId = "SAEON.Observations.WebAPI";
        public const string WebAPIPostmanClientId = "SAEON.Observations.WebAPI.Postman";
        public const string WebAPISwaggerClientId = "SAEON.Observations.WebAPI.Swagger";
    }

    public static class SAEONAuthenticationExtensions
    {
        public static void AddAllowedClientsPolicy(this AuthorizationOptions options)
        {
            options.AddPolicy(SAEONAuthenticationDefaults.AllowedClientsPolicy, policy =>
            {
                policy.AddAuthenticationSchemes(ODPAuthenticationDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ODPAuthenticationDefaults.ClientIdClaim, ODPAuthenticationDefaults.QuerySiteClientId);
            });
        }

        public static void AddDeniedClientsPolicy(this AuthorizationOptions options)
        {
            options.AddPolicy(SAEONAuthenticationDefaults.DeniedClientsPolicy, policy =>
            {
                policy.AddAuthenticationSchemes(ODPAuthenticationDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                    !context.User.HasClaim(ODPAuthenticationDefaults.ClientIdClaim, ODPAuthenticationDefaults.WebAPIPostmanClientId) &&
                    !context.User.HasClaim(ODPAuthenticationDefaults.ClientIdClaim, ODPAuthenticationDefaults.WebAPISwaggerClientId));
            });
        }

        public static void AddSAEONPolicies(this AuthorizationOptions options)
        {
            options.AddAllowedClientsPolicy();
            options.AddDeniedClientsPolicy();
        }

        public static void AddCorsAllowAllPolicy(this CorsOptions options)
        {
            options.AddPolicy(SAEONAuthenticationDefaults.CorsAllowAllPolicy, b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
        }

        public static void AddCorsAllowQuerySitePolicy(this CorsOptions options)
        {
            options.AddPolicy(SAEONAuthenticationDefaults.CorsAllowQuerySitePolicy, b => b.AllowAnyHeader().AllowAnyMethod()
                .WithOrigins("https://localhost:44340", "https://observations.saeon.ac.za", "https://observations-test.saeon.ac.za"));
        }

        public static void AddSAEONCorsPolicies(this CorsOptions options)
        {
            options.AddCorsAllowAllPolicy();
            options.AddCorsAllowQuerySitePolicy();
        }
    }

}
