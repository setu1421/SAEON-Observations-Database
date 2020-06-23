#if NETCOREAPP3_0 || NETCOREAPP3_1
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SAEON.AspNet.Common;
using SAEON.Logs;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace SAEON.Observations.Core
{
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
        private readonly ILogger<TenantAuthorizationHandler> _logger;
        private readonly IConfiguration _config;
        private readonly HttpContext _httpContext;

        public TenantAuthorizationHandler(ILogger<TenantAuthorizationHandler> logger, IConfiguration config, HttpContext httpContext)
        {
            _logger = logger;
            _config = config;
            _httpContext = httpContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TenantAuthorizationRequirement requirement)
        {
            using (_logger.MethodCall(GetType()))
            {
                //var httpContext = _httpContext.HttpContext;
                var tenants = requirement.Tenants?.Split(";", StringSplitOptions.RemoveEmptyEntries);
                var defaultTenant = requirement.DefaultTenant;
                string tenant = GetTenantFromHeaders(_httpContext.Request, _config);
                _logger.LogDebug("Tenants: {Tenants} DefaultTenant: {DefaultTenant} Tenant: {Tenant}", tenants, defaultTenant, tenant);
                if (string.IsNullOrWhiteSpace(tenant))
                {
                    tenant = defaultTenant;
                }
                if (!tenants?.Any() ?? true)
                {
                    _logger.LogError("Tenant Authorization Failed (No tenants)");
                    context.Fail();
                    return Task.CompletedTask;
                }
                if (!tenants?.Contains(tenant) ?? true)
                {
                    _logger.LogError("Tenant Authorization Failed (Unknown tenant)");
                    context.Fail();
                    return Task.CompletedTask;
                }
                _logger.LogDebug("Tenant authorization succeeded");
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }

        public static string GetTenantFromHeaders(HttpRequest request, IConfiguration config)
        {
            using (Logging.MethodCall(typeof(TenantAuthorizationHandler)))
            {
                var tenant = request?.Headers[AspNetConstants.TenantHeader].FirstOrDefault();
                Logging.Debug("Request: {Request} Config: {Config}", tenant, config[AspNetConstants.TenantDefault]);
                if (string.IsNullOrWhiteSpace(tenant)) tenant = config[AspNetConstants.TenantDefault];
                return tenant;
            }
        }
    }
}
#endif