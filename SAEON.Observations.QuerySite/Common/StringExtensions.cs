namespace SAEON.Observations.QuerySite
{
    public static class StringExtensions
    {
        public static string RemoveHttp(this string source)
        {
            return source.Replace("https://", string.Empty).Replace("http://", string.Empty);
        }
    }
}