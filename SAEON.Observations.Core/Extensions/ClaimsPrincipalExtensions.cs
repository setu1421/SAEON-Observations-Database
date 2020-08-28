using System.Security.Claims;

namespace SAEON.Observations.Core
{
    public static class ClaimsPrincipalExtensions
    {
        public static string UserId(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public static bool IsAdmin(this ClaimsPrincipal principal)
        {
            return
                principal.Identity.IsAuthenticated &&
                principal.HasClaim(c => c.Type == ClaimTypes.NameIdentifier) &&
                principal.IsInRole("admin");
        }

        public static string Name(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.Name);
        }

        public static string Email(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.Email);
        }
    }
}
