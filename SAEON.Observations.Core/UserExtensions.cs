using System;

namespace SAEON.Observations.Core
{
    [Obsolete("UserExtensions is obsolete", true)]
    public static class UserExtensions
    {
        //public static string GetUserId(this IPrincipal principal)
        //{
        //    return (principal as ClaimsPrincipal).Claims.FirstOrDefault(c => c.Type.Equals(Constants.ClaimSubject,StringComparison.CurrentCultureIgnoreCase))?.Value;
        //}

        //public static string GetUserName(this IPrincipal principal)
        //{
        //    return (principal as ClaimsPrincipal).Claims.FirstOrDefault(c => c.Type.Equals(Constants.ClaimUserName,StringComparison.CurrentCultureIgnoreCase))?.Value;
        //}

        //public static List<string> GetClaims(this IPrincipal principal)
        //{
        //    return (principal as ClaimsPrincipal).Claims.Select(c => c.Type + ": " + c.Value).ToList();
        //}
    }
}
