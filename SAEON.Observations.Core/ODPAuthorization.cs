#if NETCOREAPP3_1
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ODPAuthorizationHandler> _logger;
        private readonly IConfiguration _config;
        private readonly HttpContext _httpContext;

        public ODPAuthorizationHandler(ILogger<ODPAuthorizationHandler> logger, IConfiguration config, HttpContext httpContext)
        {
            _logger = logger;
            _config = config;
            _httpContext = httpContext;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ODPAuthorizationRequirement requirement)
        {
            using (_logger.MethodCall(GetType()))
            {
                try
                {
                    if (context == null) throw new ArgumentNullException(nameof(context));
                    if (requirement == null) throw new ArgumentNullException(nameof(requirement));
                    var introspectionUrl = _config[Constants.AuthIntrospectionUrl];
                    if (string.IsNullOrWhiteSpace(introspectionUrl)) throw new NullReferenceException(Constants.AuthIntrospectionUrl);
                    // Get token
                    var token = await _httpContext.GetTokenAsync("access_token");
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        Logging.Error("ODP Authorization Failed, no token");
                        context.Fail();
                        return;
                    }
                    _logger.LogDebug("Token: {Token}", token);

                    _logger.LogDebug("ODP authorization succeeded");
                    context.Succeed(requirement);
                    //return Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    _logger.LogException(ex);
                    throw;
                }
            }
        }
    }
}
#endif