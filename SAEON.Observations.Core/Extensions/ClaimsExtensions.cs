using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace SAEON.Observations.Core
{
    public static class ClaimsExtensions
    {
        public static Dictionary<string, string> ToClaimsList(this IEnumerable<Claim> claims)
        {
            return claims.ToDictionary(c => c.Type, c => c.Value);
        }
        public static Dictionary<string, string> ToClaimsList(this List<Claim> claims)
        {
            return claims.ToDictionary(c => c.Type, c => c.Value);
        }
    }
}
