using Newtonsoft.Json;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;

namespace SAEON.Observations.WebAPI
{
    public class InheritedDirectRouteProvider : DefaultDirectRouteProvider
    {
        protected override IReadOnlyList<IDirectRouteFactory>GetActionRouteFactories(HttpActionDescriptor actionDescriptor)
        {
            return actionDescriptor.GetCustomAttributes<IDirectRouteFactory>(inherit: true);
        }
    }

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            //config.AddODataQueryFilter();

            // Web API routes
            //config.MapHttpAttributeRoutes();
            config.MapHttpAttributeRoutes(new InheritedDirectRouteProvider());

            // OData
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            config.Count().Filter().OrderBy().Expand().Select().MaxTop(null); //new line
            builder.EntitySet<Feature>("Features");
            builder.EntitySet<Instrument>("Instruments");
            builder.EntitySet<Location>("Locations");
            builder.EntitySet<Offering>("Offerings");
            builder.EntitySet<Organisation>("Organisations");
            builder.EntitySet<Phenomenon>("Phenomena");
            builder.EntitySet<Sensor>("Sensors");
            builder.EntitySet<Site>("Sites");
            builder.EntitySet<Station>("Stations");
            builder.EntitySet<UnitOfMeasure>("UnitsOfMeasure");
            builder.EntitySet<UserDownload>("UserDownloads");
            builder.EntitySet<UserQuery>("UserQueries");
            config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());

            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
        }
    }
}
