using SAEON.Observations.Core;
using SAEON.Observations.WebAPI.ErrorHandler;
using System.Web;
using System.Web.Mvc;

namespace SAEON.Observations.WebAPI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new AiHandleErrorAttribute());
            filters.Add(new SecurityHeadersAttribute());
        }
    }
}
