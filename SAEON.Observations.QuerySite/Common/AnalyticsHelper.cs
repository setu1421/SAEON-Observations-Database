using System.Web;

namespace SAEON.Observations.QuerySite
{
    public class AnalyticsHelper
    {
        public static string Google4Tag(HttpRequest request)
        {
            if (request.Url.DnsSafeHost.ToLowerInvariant().Contains("-test"))
            {
                return "G-WE08ZKT2RG";
            }
            else
            {
                return "G-CLMPSTENQ6";
            }
        }

        public static string ClarityTag(HttpRequest request)
        {
            if (request.Url.DnsSafeHost.ToLowerInvariant().Contains("-test"))
            {
                return "G-WE08ZKT2RG";
            }
            else
            {
                return "6de1scpzw4";
            }
        }

    }
}