using SAEON.Observations.QuerySite.ErrorHandler;
using System.Web;
using System.Web.Mvc;

namespace SAEON.Observations.QuerySite
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new AiHandleErrorAttribute());
        }
    }
}
