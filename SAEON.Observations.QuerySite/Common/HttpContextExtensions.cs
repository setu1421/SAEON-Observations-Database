using System.Security.Claims;
using System.Web;

namespace SAEON.Observations.QuerySite
{
    public static class HttpContextExtensions
    {
        public static object UserInfo(this HttpContextBase context)
        {
            var result = new
            {
                TenantFromHeader = context.Request.Headers[Constants.HeaderKeyTenant],
                TenantFromSession = context.Session[Constants.SessionKeyTenant],
                SessionAccessToken = context.Session[Constants.SessionKeyAccessToken],
                SessionIdToken = context.Session[Constants.SessionKeyIdToken],
                //BearerToken = context.Request.GetBearerToken(), 
                context.User.Identity.IsAuthenticated,
                UserIsAdmin = context.UserIsAdmin(),
                UserId = context.UserId(),
                UserName = context.UserName(),
                UserEmail = context.UserEmail(),
                Claims = (context.User as ClaimsPrincipal)?.Claims.ToClaimsList()
            };
            return result;
        }

        public static string UserId(this HttpContextBase context)
        {
            return (context.User as ClaimsPrincipal).UserId();
        }

        public static bool UserIsAdmin(this HttpContextBase context)
        {
            return (context.User as ClaimsPrincipal).IsAdmin();
        }
        public static string UserName(this HttpContextBase context)
        {
            return (context.User as ClaimsPrincipal).Name();
        }

        public static string UserEmail(this HttpContextBase context)
        {
            return (context.User as ClaimsPrincipal).Email();
        }

    }
}