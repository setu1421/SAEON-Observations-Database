using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using SAEON.Logs;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SAEON.Observations.Core
{
    public static class TenantAuthenticationDefaults
    {
        public const string AuthenticationScheme = "Tenant";
        public const string ConfigKeyTenants = "Tenants";
        public const string ConfigKeyDefaultTenant = "DefaultTenant";
        public const string HeaderKeyTenant = "Tenant";
        public const string TenantClaim = "Tenant";
        public const string TenantPolicy = "TenantPolicy";
    }

    public class TenantAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string Tenants { get; set; }
        public string DefaultTenant { get; set; }
    }

    public class TenantAuthenticationPostConfigureOptions : IPostConfigureOptions<TenantAuthenticationOptions>
    {
        public void PostConfigure(string name, TenantAuthenticationOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrEmpty(options.Tenants))
            {
                throw new InvalidOperationException("Tenants must be provided in options");
            }
            if (string.IsNullOrEmpty(options.DefaultTenant))
            {
                throw new InvalidOperationException("DefaultTenant must be provided in options");
            }
        }
    }

    public class TenantAuthenticationHandler : AuthenticationHandler<TenantAuthenticationOptions>
    {

        public TenantAuthenticationHandler(
            IOptionsMonitor<TenantAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var tenants = (Options.Tenants ?? string.Empty).Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    var defaultTenant = (Options.DefaultTenant ?? string.Empty);
                    string tenant = defaultTenant;
                    if (Request.Headers.TryGetValue(TenantAuthenticationDefaults.HeaderKeyTenant, out StringValues values))
                    {
                        tenant = values.FirstOrDefault();
                    }
                    SAEONLogs.Debug("Tenants: {Tenants} DefaultTenant: {DefaultTenant} Tenant: {Tenant}", tenants.ToArray(), defaultTenant, tenant);
                    if (string.IsNullOrWhiteSpace(tenant))
                    {
                        SAEONLogs.Error("Tenant Authorization Failed (No tenant)");
                        return Task.FromResult(AuthenticateResult.Fail("No tenant"));
                    }
                    if (!tenants.Any())
                    {
                        SAEONLogs.Error("Tenant Authorization Failed (No tenants)");
                        return Task.FromResult(AuthenticateResult.Fail("No tenants"));
                    }
                    if (!tenants.Contains(tenant))
                    {
                        SAEONLogs.Error("Tenant Authorization Failed (Unknown tenant)");
                        return Task.FromResult(AuthenticateResult.Fail("Unknown tenant"));
                    }
                    var claims = new[] { new Claim(TenantAuthenticationDefaults.TenantClaim, tenant) };
                    var identity = new ClaimsIdentity(claims, TenantAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, TenantAuthenticationDefaults.AuthenticationScheme);
                    SAEONLogs.Debug("Tenant authentication succeeded Tenant: {Tenant} Claims: {@Claims}", tenant, claims.ToClaimsList());
                    return Task.FromResult(AuthenticateResult.Success(ticket));
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public static string GetTenantFromHeaders(HttpRequest request, Microsoft.Extensions.Configuration.IConfiguration config)
        {
            using (SAEONLogs.MethodCall(typeof(TenantAuthenticationHandler)))
            {
                if (request == null) throw new ArgumentNullException(nameof(request));
                if (config == null) throw new ArgumentNullException(nameof(config));
                var tenants = (config[TenantAuthenticationDefaults.ConfigKeyTenants] ?? string.Empty).Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var defaultTenant = (config[TenantAuthenticationDefaults.ConfigKeyDefaultTenant] ?? string.Empty);
                string tenant = defaultTenant;
                if (request.Headers.TryGetValue(TenantAuthenticationDefaults.HeaderKeyTenant, out StringValues values))
                {
                    tenant = values.FirstOrDefault();
                }
                SAEONLogs.Debug("Tenants: {Tenants} DefaultTenant: {DefaultTenant} Tenant: {Tenant}", tenants.ToArray(), defaultTenant, tenant);
                return tenant;
            }
        }
    }

    public static class TenantAuthenticationExtensions
    {
        public static AuthenticationBuilder AddTenant(this AuthenticationBuilder builder)
        {
            return AddTenant(builder, TenantAuthenticationDefaults.AuthenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddTenant(this AuthenticationBuilder builder, string authenticationScheme)
        {
            return AddTenant(builder, authenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddTenant(this AuthenticationBuilder builder, Action<TenantAuthenticationOptions> configureOptions)
        {
            return AddTenant(builder, TenantAuthenticationDefaults.AuthenticationScheme, configureOptions);
        }

        public static AuthenticationBuilder AddTenant(this AuthenticationBuilder builder, string authenticationScheme, Action<TenantAuthenticationOptions> configureOptions)
        {
            builder.Services.AddSingleton<IPostConfigureOptions<TenantAuthenticationOptions>, TenantAuthenticationPostConfigureOptions>();
            //builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<TenantAuthenticationOptions>, TenantAuthenticationPostConfigureOptions>());
            return builder.AddScheme<TenantAuthenticationOptions, TenantAuthenticationHandler>(authenticationScheme, configureOptions);
        }

        public static void AddTenantPolicy(this AuthorizationOptions options)
        {
            options.AddPolicy(TenantAuthenticationDefaults.TenantPolicy, policy =>
            {
                policy.AddAuthenticationSchemes(TenantAuthenticationDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(TenantAuthenticationDefaults.TenantClaim);
            });
        }
    }
}
