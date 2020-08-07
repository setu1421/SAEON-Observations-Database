#if NETCOREAPP3_1
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SAEON.Logs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SAEON.Observations.Core
{
    public static class TenantPolicyDefaults
    {
        public const string AuthorizationPolicy = "TenantAuthorizationPolicy";
        public const string HeaderKeyTenant = "Tenant";
        public const string ConfigKeyTenants = "Tenants";
        public const string ConfigKeyTenant = "Tenant";
        public const string ConfigKeyDefaultTenant = "DefaultTenant";
    }

    public class TenantAuthorizationRequirement : IAuthorizationRequirement
    {
        public string Tenants { get; private set; } = null;
        public string DefaultTenant { get; private set; } = null;

        public TenantAuthorizationRequirement(string tenants, string defaultTenant)
        {
            Tenants = tenants;
            DefaultTenant = defaultTenant;
        }
    }

    public class TenantAuthorizationHandler : AuthorizationHandler<TenantAuthorizationRequirement>
    {
        private readonly IConfiguration _config;
        private readonly HttpContext _httpContext;

        public TenantAuthorizationHandler(IConfiguration config, HttpContext httpContext)
        {
            _config = config;
            _httpContext = httpContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TenantAuthorizationRequirement requirement)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    if (context == null) throw new ArgumentNullException(nameof(context));
                    if (requirement == null) throw new ArgumentNullException(nameof(requirement));
                    var tenants = requirement.Tenants?.Split(";", StringSplitOptions.RemoveEmptyEntries);
                    var defaultTenant = requirement.DefaultTenant;
                    string tenant = GetTenantFromHeaders(_httpContext.Request, _config);
                    SAEONLogs.Debug("Tenants: {Tenants} DefaultTenant: {DefaultTenant} Tenant: {Tenant}", tenants, defaultTenant, tenant);
                    if (string.IsNullOrWhiteSpace(tenant))
                    {
                        if (string.IsNullOrWhiteSpace(defaultTenant))
                        {
                            SAEONLogs.Error("Tenant Authorization Failed (No default tenant)");
                            context.Fail();
                            return Task.CompletedTask;
                        }
                        tenant = defaultTenant;
                    }
                    if (!tenants?.Any() ?? true)
                    {
                        SAEONLogs.Error("Tenant Authorization Failed (No tenants)");
                        context.Fail();
                        return Task.CompletedTask;
                    }
                    if (!tenants?.Contains(tenant) ?? true)
                    {
                        SAEONLogs.Error("Tenant Authorization Failed (Unknown tenant)");
                        context.Fail();
                        return Task.CompletedTask;
                    }
                    SAEONLogs.Debug("Tenant authorization succeeded");
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public static string GetTenantFromHeaders(HttpRequest request, IConfiguration config)
        {
            using (SAEONLogs.MethodCall(typeof(TenantAuthorizationHandler)))
            {
                var tenant = request?.Headers[TenantPolicyDefaults.HeaderKeyTenant].FirstOrDefault();
                SAEONLogs.Debug("Request: {Request} Config: {Config}", tenant, config?[TenantPolicyDefaults.ConfigKeyDefaultTenant]);
                if (string.IsNullOrWhiteSpace(tenant)) tenant = config[TenantPolicyDefaults.ConfigKeyDefaultTenant];
                return tenant;
            }
        }
    }
}
#endif