using System.Security.Claims;

namespace SAEON.Observations.Core
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public static bool GetUserIsAdmin(this ClaimsPrincipal principal)
        {
            return
                principal.HasClaim(c => c.Type == ODPAuthenticationDefaults.IdTokenClaim) &&
                principal.HasClaim(c => c.Type == ODPAuthenticationDefaults.AdminTokenClaim);
        }

        public static string GetUserName(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.Name);
        }

        public static string GetUserEmail(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.Email);
        }
    }
}
