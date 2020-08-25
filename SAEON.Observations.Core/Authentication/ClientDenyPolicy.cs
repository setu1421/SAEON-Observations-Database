#if NETCOREAPP3_1
using Microsoft.AspNetCore.Authorization;
using SAEON.Logs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SAEON.Observations.Core.Authentication
{
    public static class ClientAllowPolicyDefaults
    {
        public const string AuthorizationPolicy = "ClientAllowPolicy";
    }

    public class ClientAllowAuthorizationRequirement : IAuthorizationRequirement
    {
        public string Clients { get; private set; }

        public ClientAllowAuthorizationRequirement(string clients) : base()
        {
            Clients = clients;
        }
    }

    public class ClientAllowAuthorizationHandler : AuthorizationHandler<ClientAllowAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ClientAllowAuthorizationRequirement requirement)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    if (context == null) throw new ArgumentNullException(nameof(context));
                    if (requirement == null) throw new ArgumentNullException(nameof(requirement));
                    var user = context.User;
                    if (!user.Identity.IsAuthenticated)
                    {
                        SAEONLogs.Error("ClientAllowAuthorization failed, not authenticated");
                        context.Fail();
                        return Task.CompletedTask;
                    }
                    if (!user.HasClaim(c => c.Type == "ClientId"))
                    {
                        SAEONLogs.Error("ClientAllowAuthorization failed, no client claim");
                        context.Fail();
                        return Task.CompletedTask;
                    }
                    var client = user.FindFirst("ClientId").Value;
                    var clients = requirement.Clients.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    if (!clients.Contains(client))
                    {
                        SAEONLogs.Error("ClientAllowAuthorization failed, unknown client");
                        context.Fail();
                        return Task.CompletedTask;
                    }
                    SAEONLogs.Debug("ClientAllowAuthorization succeeded");
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
    }

}
#endif