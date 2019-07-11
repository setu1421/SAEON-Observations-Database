using System.Web;
using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.ComingSoon
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ErrorHandler.AiHandleErrorAttribute());
        }
    }
}
