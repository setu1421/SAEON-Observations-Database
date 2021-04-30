using Microsoft.AspNetCore.Http;

namespace SAEON.Observations.WebAPI
{
    public static class AnalyticsHelper
    {
        public static string Google4Tag(HttpRequest request)
        {
            if (request.Host.Host.ToLowerInvariant().Contains("-test"))
            {
                return "G-NBVJ0F9CEY";
            }
            else
            {
                return "G-ZSNHGJHKS7";
            }
        }

        public static string ClarityTag(HttpRequest request)
        {
            if (request.Host.Host.ToLowerInvariant().Contains("-test"))
            {
                return "6de2irdzig";
            }
            else
            {
                return "6ddn2wemly";
            }
        }
    }
}
