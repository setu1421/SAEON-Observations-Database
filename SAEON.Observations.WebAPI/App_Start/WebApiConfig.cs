using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using SAEON.Observations.Core;
using System.Reflection;
using System.Linq.Expressions;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Http.Routing;
using System.Web.Http.Controllers;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using Newtonsoft.Json;

namespace SAEON.Observations.WebAPI
{
    public class InheritedDirectRouteProvider : DefaultDirectRouteProvider
    {
        protected override IReadOnlyList<IDirectRouteFactory>
        GetActionRouteFactories(HttpActionDescriptor actionDescriptor)
        {
            return actionDescriptor.GetCustomAttributes<IDirectRouteFactory>
            (inherit: true);
        }
    }

    public static class WebApiConfig
    {

        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes(new InheritedDirectRouteProvider());
            //config.MapHttpAttributeRoutes();

            // OData
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Instrument>("Instruments");
            builder.EntitySet<Offering>("Offerings");
            builder.EntitySet<Phenomenon>("Phenomena");
            builder.EntitySet<Sensor>("Sensors");
            builder.EntitySet<Site>("Sites");
            builder.EntitySet<Station>("Stations");
            builder.EntitySet<UnitOfMeasure>("UnitsOfMeasure");
            builder.EntitySet<UserDownload>("UserDownloads");
            builder.EntitySet<UserQuery>("UserQueries");
            builder.Ignore<ApplicationUser>();
            builder.Ignore<IdentityRole>();
            builder.Ignore<IdentityUserClaim>();
            builder.Ignore<IdentityUserLogin>();
            builder.Ignore<IdentityUserRole>();
            config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());

            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;

            //config.Routes.MapHttpRoute(
            //    name: "ApiById",
            //    routeTemplate: "{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
        }
    }
}
