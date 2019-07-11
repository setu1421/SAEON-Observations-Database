using System.Web;
using System.Web.Mvc;

namespace SAEON.Observations.WebAPI.ComingSoon
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ErrorHandler.AiHandleErrorAttribute());
        }
    }
}
