using System.Web.Mvc;

namespace SAEON.Observations.WebAPI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new ErrorHandler.AiHandleErrorAttribute());
        }
    }
}
