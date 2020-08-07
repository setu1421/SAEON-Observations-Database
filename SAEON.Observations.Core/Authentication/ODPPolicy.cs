#if NETCOREAPP3_1
/*
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SAEON.Logs;
using System;
using System.Threading.Tasks;

namespace SAEON.Observations.Core
{
    public class ODPAuthorizationRequirement : IAuthorizationRequirement
    {
        public bool RequireLogin { get; private set; } = false;

        public ODPAuthorizationRequirement() : base() { }

        public ODPAuthorizationRequirement(bool requireLogin) : this()
        {
            RequireLogin = requireLogin;
        }
    }
    public class ODPAuthorizationHandler : AuthorizationHandler<ODPAuthorizationRequirement>
    {
        private readonly IConfiguration _config;
        private readonly HttpContext _httpContext;

        public ODPAuthorizationHandler(IConfiguration config, HttpContext httpContext)
        {
            _config = config;
            _httpContext = httpContext;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ODPAuthorizationRequirement requirement)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    if (context == null) throw new ArgumentNullException(nameof(context));
                    if (requirement == null) throw new ArgumentNullException(nameof(requirement));
                    var introspectionUrl = _config[Constants.AuthIntrospectionUrl];
                    SAEONLogs.Debug("IntrospectionUrl: {IntrospectionUrl}", introspectionUrl);
                    if (string.IsNullOrWhiteSpace(introspectionUrl)) throw new NullReferenceException(Constants.AuthIntrospectionUrl);
                    // Get token
                    var token = await _httpContext.GetTokenAsync("access_token");
                    SAEONLogs.Debug("Token: {Token}", token);
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        SAEONLogs.Error("ODP Authorization Failed, no token");
                        context.Fail();
                        return;
                    }
                    SAEONLogs.Debug("ODP authorization succeeded");
                    context.Succeed(requirement);
                    //return Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }
    }
}
*/
#endif