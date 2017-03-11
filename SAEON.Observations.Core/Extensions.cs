using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace SAEON.Observations.Core
{
    public static class UserExtensions
    {
        public static string GetUserId(this IPrincipal principal)
        {
            //var identity = principal as ClaimsIdentity;
            //return identity?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            return ClaimsPrincipal.Current.Claims.FirstOrDefault(c => c.Type == Constants.Subject)?.Value;
        }

        public static string GetEmail(this IPrincipal principal)
        {
            return ClaimsPrincipal.Current.Claims.FirstOrDefault(c => c.Type == Constants.Email)?.Value;
        }

        public static string GetUserName(this IPrincipal principal)
        {
            return ClaimsPrincipal.Current.Claims.FirstOrDefault(c => c.Type == Constants.UserName)?.Value;
        }


        public static List<string> GetClaims(this IPrincipal principal)
        {
            return ClaimsPrincipal.Current.Claims.Select(c => c.Type + ": "+c.Value).ToList();
        }
    }
}
