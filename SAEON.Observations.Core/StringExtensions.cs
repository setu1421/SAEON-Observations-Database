using System.Security.Cryptography;
using System.Text;

namespace SAEON.Observations.Core
{
    public static class StringExtensions
    {
        public static byte[] Sha256(this string value)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
            }
        }

        public static string Replace(this string value, char[] oldChars, char newChar)
        {
            foreach (var oldChar in oldChars)
            {
                value = value.Replace(oldChar, newChar);
            }
            return value;
        }

        public static string Replace(this string value, string[] oldStrings, string newString)
        {
            foreach (var oldString in oldStrings)
            {
                value = value.Replace(oldString, newString);
            }
            return value;
        }

        //public static string HtmlB(this string value)
        //{
        //    return $"<b>{value}</b>";
        //}

#if NET5_0
        public static string ToCamelCase(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return char.ToLowerInvariant(value[0]) + value[1..];
        }
#endif
    }
}
