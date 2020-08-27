using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SAEON.Observations.Core
{
    public static class HttpContextExtensions
    {
        public static object GetUserInfo(this HttpContext context)
        {
            var session = (ISession)context.RequestServices.GetService(typeof(ISession));
            var config = context.RequestServices.GetRequiredService<IConfiguration>();
            var result = new
            {
                TenantFromHeader = TenantAuthenticationHandler.GetTenantFromHeaders(context.Request, config),
                TenantFromSession = session?.GetString(TenantAuthenticationDefaults.HeaderKeyTenant),
                TokenFromSession = session?.GetString(ODPAuthenticationDefaults.SessionToken),
                TokenFromRequest = context.Request?.GetBearerToken(),
                context.User.Identity.IsAuthenticated,
                IsAdmin = context.GetLoggedInUserIsAdmin(),
                UserId = context.GetLoggedInUserId(),
                UserName = context.GetLoggedInUserName(),
                UserEmail = context.GetLoggedInUserEmail(),
                Claims = context.User.Claims.ToClaimsList()
            };
            return result;
        }

        public static string GetLoggedInUserId(this HttpContext context)
        {
            return context.User.GetUserId();
        }

        public static bool GetLoggedInUserIsAdmin(this HttpContext context)
        {
            return context.User.GetUserIsAdmin();
        }
        public static string GetLoggedInUserName(this HttpContext context)
        {
            return context.User.GetUserName();
        }

        public static string GetLoggedInUserEmail(this HttpContext context)
        {
            return context.User.GetUserEmail();
        }
    }
}
