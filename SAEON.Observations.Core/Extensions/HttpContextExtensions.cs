#if NETCOREAPP3_1
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace SAEON.Observations.Core
{
    public static class HttpContextExtensions
    {
        public static object GetUserInfo(this HttpContext context)
        {
            var session = (ISession)context.RequestServices.GetService(typeof(ISession));
            var result = new
            {
                TenantFromHeader = TenantAuthorizationHandler.GetTenantFromHeaders(context.Request, null),
                TenantFromSession = session?.GetString(TenantPolicyDefaults.HeaderKeyTenant),
                TokenFromSession = session?.GetString("AccessToken"),
                TokenFromRequest = context.Request?.GetBearerToken(),
                context.User.Identity.IsAuthenticated,
                UserId = context.GetLoggedInUserId(),
                UserName = context.GetLoggedInUserName(),
                UserEmail = context.GetLoggedInUserEmail(),
                Claims = context.User.Claims.Select(c => new { c.Type, c.Value }).ToArray()
            };
            return result;
        }

        public static string GetLoggedInUserId(this HttpContext context)
        {
            return context.User.GetUserId();
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
#endif
