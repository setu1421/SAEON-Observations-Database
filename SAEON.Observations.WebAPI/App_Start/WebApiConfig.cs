using Microsoft.Web.Http;
using SAEON.Observations.WebAPI.Controllers.OData;
using SAEON.Observations.WebAPI.Controllers.SensorThingsAPI;
using SAEON.Observations.WebAPI.Controllers.WebAPI;
using System.Web.Http;

namespace SAEON.Observations.WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.AddApiVersioning(o => {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });
            config.ConfigureWebAPI();
            config.ConfigureOData();
            config.ConfigureSensorThings();
            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
        }
    }
}
