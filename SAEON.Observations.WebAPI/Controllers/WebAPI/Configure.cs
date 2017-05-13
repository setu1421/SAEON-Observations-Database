using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    public class InheritedDirectRouteProvider : DefaultDirectRouteProvider
    {
        protected override IReadOnlyList<IDirectRouteFactory> GetActionRouteFactories(HttpActionDescriptor actionDescriptor)
        {
            return actionDescriptor.GetCustomAttributes<IDirectRouteFactory>(inherit: true);
        }
    }

    public static class Configure
    {
        public static void ConfigureWebAPI(this HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes(new InheritedDirectRouteProvider());
            //config.MapHttpAttributeRoutes();
        }
    }
}