using SAEON.AspNet.Auth;
using System.Security.Claims;
using System.Web;

namespace SAEON.Observations.QuerySite
{
    public static class HttpContextExtensions
    {
        public static object UserInfo(this HttpContext context)
        {
            var result = new
            {
                TenantFromHeader = context.Request.Headers[Constants.HeaderKeyTenant],
                TenantFromSession = context.Session[Constants.SessionKeyTenant],
                SessionODPAccessToken = context.Session[Constants.SessionKeyODPAccessToken],
                context.User.Identity.IsAuthenticated,
                UserIsAdmin = context.UserIsAdmin(),
                UserId = context.UserId(),
                UserName = context.UserName(),
                UserEmail = context.UserEmail(),
                Claims = (context.User as ClaimsPrincipal)?.Claims.ToClaimsList()
            };
            return result;
        }
    }
}