using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using SAEON.AspNet.Auth;
using SAEON.Logs;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace SAEON.Observations.WebAPI.Common
{
    public class AccessTokenOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var policies = context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Union(context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>());
            SAEONLogs.Information("{Filter} {Type} Policies: {Policies}", GetType().Name, context.MethodInfo.DeclaringType.Name, string.Join(";", policies.Select(i => i.Policy)));
            var hasAccessToken = policies.Any(i => i.Policy == ODPAuthenticationDefaults.AccessTokenPolicy);
            var hasIdToken = policies.Any(i => i.Policy == ODPAuthenticationDefaults.IdTokenPolicy);
            var hasAuthorize = hasAccessToken && !hasIdToken;

            if (hasAuthorize)
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

                operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [
                        new OpenApiSecurityScheme {Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "AccessToken"}
                        }
                    ] = new[] { "SAEON.Observations.WebAP" }
                }
            };

            }
        }
    }

    public class IdTokenOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var policies = context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Union(context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>());
            SAEONLogs.Information("{Filter} {Type} Policies: {Policies}", GetType().Name, context.MethodInfo.DeclaringType.Name, string.Join(";", policies.Select(i => i.Policy)));
            var hasAccessToken = policies.Any(i => i.Policy == ODPAuthenticationDefaults.AccessTokenPolicy);
            var hasIdToken = policies.Any(i => i.Policy == ODPAuthenticationDefaults.IdTokenPolicy);
            var hasAuthorize = hasIdToken;

            if (hasAuthorize)
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

                operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [
                        new OpenApiSecurityScheme {Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "IdToken"}
                        }
                    ] = new[] { "SAEON.Observations.WebAP" }
                }
            };

            }
        }
    }
}
