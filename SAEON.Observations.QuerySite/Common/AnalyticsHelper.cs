using SAEON.Logs;
using System.Web;

namespace SAEON.Observations.QuerySite
{
    public class AnalyticsHelper
    {
        private static bool IsLocal(HttpRequest request)
        {
            return request.IsLocal || request.Url.DnsSafeHost.ToLowerInvariant().Contains("-test");
        }

        public static string ApplicationInsightsKey(HttpRequest request)
        {
            return IsLocal(request) ? "ab8abeba-cc7e-4238-a3d1-00781fb48938" : "16bb3687-4016-4288-a1f6-432351d2d7c7";
        }

        public static string GoogleGA4Key(HttpRequest request)
        {
            SAEONLogs.Verbose("Query Host: {Host} IsLocal: {IsLocal}", request.Url.DnsSafeHost, request.IsLocal);
            return IsLocal(request) ? "G-WE08ZKT2RG" : "G-CLMPSTENQ6";
        }

        public static string GoogleUAKey(HttpRequest request)
        {
            return IsLocal(request) ? "UA-128063984-4" : "UA-128063984-5";
        }

        public static string ClarityKey(HttpRequest request)
        {
            return IsLocal(request) ? "6de1scpzw4" : "6dbvienruh";
        }
    }
}