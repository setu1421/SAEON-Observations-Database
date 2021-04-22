using System;

namespace SAEON.Observations.Core
{
    public static class DateExtensions
    {
        public static string ToJsonDate(this DateTime aDate)
        {
            return aDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssK");
        }

        public static string ToJsonDate(this DateTime? aDate)
        {
            return aDate.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssK");
        }

        public static string ToJsonDate(this DateTimeOffset aDate)
        {
            return aDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'sszzz");
        }

        public static string ToJsonDate(this DateTimeOffset? aDate)
        {
            return aDate.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'sszzz");
        }
    }
}
