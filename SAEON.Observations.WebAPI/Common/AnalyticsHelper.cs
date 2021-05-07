using Microsoft.AspNetCore.Http;
using SAEON.Logs;

namespace SAEON.Observations.WebAPI
{
    public static class AnalyticsHelper
    {
        private static bool IsLocal(HttpRequest request)
        {
            return request.IsLocal() || request.Host.Host.ToLowerInvariant().Contains("-test");
        }

        public static string ApplicationInsightsKey(HttpRequest request)
        {
            return IsLocal(request) ? "f8e54b1e-96bf-4dcb-a5e2-4e9f3653a539" : "bbe0ad06-67b9-43d0-a30d-ebdcb656cd8b";
        }

        public static string GoogleGA4Key(HttpRequest request)
        {
            SAEONLogs.Verbose("API Host: {Host} IsLocal: {IsLocal}", request.Host.Host, request.IsLocal());
            return IsLocal(request) ? "G-NBVJ0F9CEY" : "G-ZSNHGJHKS7";
        }

        public static string GoogleUAKey(HttpRequest request)
        {
            return IsLocal(request) ? "UA-128063984-6" : "UA-128063984-8";
        }

        public static string ClarityKey(HttpRequest request)
        {
            return IsLocal(request) ? "6de2irdzig" : "6ddn2wemly";
        }
    }
}
