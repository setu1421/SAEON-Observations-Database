using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SAEON.AspNet.Auth;

namespace SAEON.Observations.Auth
{
    public static class HttpContextExtensions
    {
        public static object UserInfo(this HttpContext context)
        {
            var session = (ISession)context.RequestServices.GetService(typeof(ISession));
            var config = context.RequestServices.GetRequiredService<IConfiguration>();
            var result = new
            {
                TenantFromHeader = TenantAuthenticationHandler.GetTenantFromHeaders(context.Request, config),
                TenantFromSession = session?.GetString(TenantAuthenticationDefaults.HeaderKeyTenant),
                SessionODPAccessToken = session?.GetString(ODPAuthenticationDefaults.SessionKeyODPAccessToken),
                BearerToken = context.Request?.GetBearerToken(),
                context.User.Identity.IsAuthenticated,
                UserIsAdmin = context.UserIsAdmin(),
                UserId = context.UserId(),
                UserName = context.UserName(),
                UserEmail = context.UserEmail(),
                Claims = context.User.Claims.ToClaimsList()
            };
            return result;
        }
    }
}
