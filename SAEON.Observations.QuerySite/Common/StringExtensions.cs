namespace SAEON.Observations.QuerySite.Common
{
    public static class StringExtensions
    {
        public static string AddTrailingSlash(this string aString)
        {
            if (aString.EndsWith("/"))
                return aString;
            else
                return aString + "/";
        }
    }
}
