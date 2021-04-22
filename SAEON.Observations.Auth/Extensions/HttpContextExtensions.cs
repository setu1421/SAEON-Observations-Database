using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        public static string UserId(this HttpContext context)
        {
            return context.User.UserId();
        }

        public static bool UserIsAdmin(this HttpContext context)
        {
            return context.User.IsAdmin();
        }
        public static string UserName(this HttpContext context)
        {
            return context.User.Name();
        }

        public static string UserEmail(this HttpContext context)
        {
            return context.User.Email();
        }
    }
}
