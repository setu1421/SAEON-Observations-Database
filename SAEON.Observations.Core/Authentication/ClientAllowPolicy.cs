#if NETCOREAPP3_1
using Microsoft.AspNetCore.Authorization;
using SAEON.Logs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SAEON.Observations.Core.Authentication
{
    public static class ClientDenyPolicyDefaults
    {
        public const string AuthorizationPolicy = "ClientDenyPolicy";
    }

    public class ClientDenyAuthorizationRequirement : IAuthorizationRequirement
    {
        public string Clients { get; private set; }

        public ClientDenyAuthorizationRequirement(string clients) : base()
        {
            Clients = clients;
        }
    }

    public class ClientDenyAuthorizationHandler : AuthorizationHandler<ClientDenyAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ClientDenyAuthorizationRequirement requirement)
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
                        SAEONLogs.Error("ClientDenyAuthorization failed, not authenticated");
                        context.Fail();
                        return Task.CompletedTask;
                    }
                    if (!user.HasClaim(c => c.Type == "ClientId"))
                    {
                        SAEONLogs.Error("ClientDenyAuthorization failed, no client claim");
                        context.Fail();
                        return Task.CompletedTask;
                    }
                    var client = user.FindFirst("ClientId").Value;
                    var clients = requirement.Clients.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    if (clients.Contains(client))
                    {
                        SAEONLogs.Error("ClientDenyAuthorization failed, client denied");
                        context.Fail();
                        return Task.CompletedTask;
                    }
                    SAEONLogs.Debug("ClientDenyAuthorization succeeded");
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