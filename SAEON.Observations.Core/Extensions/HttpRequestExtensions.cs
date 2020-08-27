using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http.Headers;

namespace SAEON.Observations.Core
{
    public static class HttpRequestExtensions
    {
        public static string GetBearerToken(this HttpRequest request)
        {
            if (!request.Headers.ContainsKey("Authorization"))
            {
                return null;
            }
            if (!AuthenticationHeaderValue.TryParse(request.Headers["Authorization"], out AuthenticationHeaderValue headerValue))
            {
                return null;
            }
            if (!"Bearer".Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            return headerValue.Parameter;
        }
    }
}
