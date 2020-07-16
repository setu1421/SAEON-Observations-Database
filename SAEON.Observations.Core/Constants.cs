namespace SAEON.Observations.Core
{
    public static class Constants
    {
        public const string AuthServerUrl = "AuthServerUrl";
        public const string AuthServerHealthCheckUrl = "AuthServerHealthCheckUrl";
        public const string AuthIntrospectionUrl = "AuthIntrospectionUrl";
        public const string AuthIntrospectionHealthCheckUrl = "AuthIntrospectionHealthCheckUrl";
        public const string ClientIdQuerySite = "SAEON.Observations.QuerySite";
        public const string ClientIdNodes = "SAEON.Observations.WebAPI.Nodes";
        public const string ClientIdPostman = "SAEON.Observations.WebAPI.Postman";
        public const string ClientIdSwagger = "SAEON.Observations.WebAPI.Swagger";
        public const string ODPAuthorizationPolicy = "ODPAuthorizationPolicy";
        public const string QuerySiteUrl = "QuerySiteUrl";
        public const string QuerySiteHealthCheckUrl = "QuerySiteHealthCheckUrl";
        public static readonly string TenantDefault = "DefaultTenant"; // Config
        public static readonly string TenantHeader = "Tenant"; // Header
        public static readonly string TenantSession = "Tenant"; // Session
        public static readonly string TenantTenants = "Tenants"; // Config
        public const string TenantAuthorizationPolicy = "TenantAuthorizationPolicy";
        public const string WebAPIUrl = "WebAPIUrl";
        public const string WebAPIHealthCheckUrl = "WebAPIHealthCheckUrl";
    }
}
