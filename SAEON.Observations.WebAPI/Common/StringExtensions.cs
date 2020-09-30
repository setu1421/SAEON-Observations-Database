using System.Security.Cryptography;
using System.Text;

namespace SAEON.Observations.WebAPI
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return char.ToLowerInvariant(value[0]) + value.Substring(1);
        }

        public static byte[] Sha256(this string value)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
            }
        }

        public static string HtmlB(this string value)
        {
            return $"<b>{value}</b>";
        }
        public static string AddTrailingSlash(this string aString)
        {
            if (aString.EndsWith("/"))
                return aString;
            else
                return aString + "/";
        }
    }
}
