using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Newtonsoft.Json;
using SAEON.Core;
using SAEON.Logs;
using db = SAEON.Observations.Core.Entities;
using st = SAEON.Observations.SensorThings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Cors;
using System.Web.Http.Routing;

namespace SAEON.Observations.WebAPI
{
    public class InheritedDirectRouteProvider : DefaultDirectRouteProvider
    {
        protected override IReadOnlyList<IDirectRouteFactory> GetActionRouteFactories(HttpActionDescriptor actionDescriptor)
        {
            return actionDescriptor.GetCustomAttributes<IDirectRouteFactory>(inherit: true);
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ODataRouteNameAttribute : Attribute
    {
        public string Name { get; private set; }

        public ODataRouteNameAttribute(string name)
        {
            Name = name;
        }
    }

    public class ControllerBoundAttributeRoutingConvention : AttributeRoutingConvention
    {
        private readonly List<Type> controllers = new List<Type> { typeof(MetadataController) };

        public ControllerBoundAttributeRoutingConvention(string routeName, HttpConfiguration config, IEnumerable<Type> controllers) : base(routeName, config)
        {
            //using (Logging.MethodCall(GetType(), new ParameterList { { "routeName", routeName } }))
            {
                this.controllers.AddRange(controllers);
                //Logging.Verbose("RouteName: {routeName} Controllers: {controllers}", routeName, controllers.OrderBy(i => i.FullName).Select(i => i.FullName));
            }
        }

        public override bool ShouldMapController(HttpControllerDescriptor controller)
        {
            //using (Logging.MethodCall(GetType()))
            {
                var result = controllers.Contains(controller.ControllerType);
                //Logging.Verbose("Name: {controllerName} Mapped: {mapped}", controller.ControllerType.FullName, result);
                return result;
            }
        }
    }

    public static class HttpConfigurationExtensions
    {
        private static IEnumerable<Type> GetTypesWith<TAttribute>(bool inherit) where TAttribute : System.Attribute
        {
            return from a in AppDomain.CurrentDomain.GetAssemblies()
                   from t in a.GetTypes()
                   where t.IsDefined(typeof(TAttribute), inherit)
                   select t;
        }

        public static ODataRoute MapControllerBoundODataServiceRoute(this HttpConfiguration configuration, string routeName, string routePrefix, IEdmModel model)
        {
            var controllers = GetTypesWith<ODataRouteNameAttribute>(true).Where(c =>
            {
                var attr = (ODataRouteNameAttribute)c.GetCustomAttributes(typeof(ODataRouteNameAttribute), true).FirstOrDefault();
                return attr?.Name.Equals(routeName, StringComparison.CurrentCultureIgnoreCase) ?? false;
            });
            var conventions = ODataRoutingConventions.CreateDefault();
            conventions.Insert(0, new ControllerBoundAttributeRoutingConvention(routeName, configuration, controllers));
            return configuration.MapODataServiceRoute(routeName, routePrefix, model, new DefaultODataPathHandler(), conventions);
        }

        public static ODataRoute MapControllerBoundODataServiceRoute2(this HttpConfiguration configuration, string routeName, string routePrefix, IEdmModel model)
        {
            var controllers = GetTypesWith<ODataRouteNameAttribute>(true).Where(c =>
            {
                var attr = (ODataRouteNameAttribute)c.GetCustomAttributes(typeof(ODataRouteNameAttribute), true).FirstOrDefault();
                return attr?.Name.Equals(routeName, StringComparison.CurrentCultureIgnoreCase) ?? false;
            });
            var conventions = ODataRoutingConventions.CreateDefault();
            conventions.Insert(0, new ControllerBoundAttributeRoutingConvention(routeName, configuration, controllers));
            return configuration.MapODataServiceRoute(routeName, routePrefix, builder =>
                builder
                    .AddService<IEdmModel>(ServiceLifetime.Singleton, sp => model)
                    .AddService<ODataSerializerProvider, st.SensorThingsODataSerializerProvider>(ServiceLifetime.Singleton)
                    .AddService<IEnumerable<IODataRoutingConvention>>(ServiceLifetime.Singleton, sp => conventions));
        }
    }

    public static class ODataExtensions
    {
        public static void IgnoreEntityItemLists<TEntity>(this EntitySetConfiguration<TEntity> source) where TEntity : db.NamedEntity
        {
            //using (Logging.MethodCall(typeof(ODataExtensions)))
            {
                var type = typeof(TEntity);
                foreach (var prop in type.GetProperties().Where(i => i.Name.EndsWith("List")))
                {
                    var parameter = Expression.Parameter(type, "entity");
                    var property = Expression.Property(parameter, prop);
                    var funcType = typeof(Func<,>).MakeGenericType(type, prop.PropertyType);
                    var lambda = Expression.Lambda(funcType, property, parameter);
                    source.EntityType.GetType()
                       .GetMethod("Ignore")
                       .MakeGenericMethod(prop.PropertyType)
                       .Invoke(source.EntityType, new[] { lambda });
                }
            }
        }
    }

    public static class WebApiConfig
    {
        private static IEdmModel ObservationsEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder { ContainerName = "Observations" };
            builder.EntitySet<db.Instrument>("Instruments").IgnoreEntityItemLists();
            builder.EntitySet<db.Inventory>("Inventory");
            builder.EntitySet<db.Offering>("Offerings").IgnoreEntityItemLists();
            builder.EntitySet<db.Organisation>("Organisations").IgnoreEntityItemLists();
            builder.EntitySet<db.Phenomenon>("Phenomena").IgnoreEntityItemLists();
            builder.EntitySet<db.Programme>("Programmes").IgnoreEntityItemLists();
            builder.EntitySet<db.Project>("Projects").IgnoreEntityItemLists();
            builder.EntitySet<db.Sensor>("Sensors").IgnoreEntityItemLists();
            builder.EntitySet<db.Site>("Sites").IgnoreEntityItemLists();
            builder.EntitySet<db.Station>("Stations").IgnoreEntityItemLists();
            builder.EntitySet<db.Unit>("Units").IgnoreEntityItemLists();
            return builder.GetEdmModel();
        }

        private static IEdmModel InternalEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder { ContainerName = "Internal" };
            builder.EntitySet<db.Location>("Locations");
            builder.EntitySet<db.Feature>("Features");
            return builder.GetEdmModel();
        }

        public static void Register(HttpConfiguration config)
        {
            using (Logging.MethodCall(typeof(WebApiConfig)))
            {
                config.Formatters.Clear();
                config.Formatters.Add(new JsonMediaTypeFormatter());
                config.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented;
                config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                config.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;

                var querySiteUrl = Properties.Settings.Default.QuerySiteUrl.TrimEnd("//").Replace("https:","http:");
                var corsUrls = querySiteUrl + "," + querySiteUrl.Replace("http:", "https:");
                Logging.Information("CORS: {corsURLS}", corsUrls);
                var corsAttr = new EnableCorsAttribute(corsUrls, "*", "*");
                //{
                //    SupportsCredentials = true
                //};
                config.EnableCors(corsAttr);

                // Web API routes
                //config.MapHttpAttributeRoutes();
                config.MapHttpAttributeRoutes(new InheritedDirectRouteProvider());

                // OData
                config.Filter().Expand().Select().OrderBy().MaxTop(null).Count();
                config.MapControllerBoundODataServiceRoute("OData", "OData", ObservationsEdmModel());
                config.MapControllerBoundODataServiceRoute("Internal", "Internal", InternalEdmModel());
                config.MapControllerBoundODataServiceRoute2("SensorThings", "SensorThings", st.SensorThingsConfig.GetEdmModel());
                //config.MapControllerBoundODataServiceRoute("SensorThings", "SensorThings", builder =>
                //    builder
                //        .AddService<IEdmModel>(ServiceLifetime.Singleton, sp => SensorThingsEdmModel())
                //        .AddService<ODataSerializerProvider, SensorThingsODataSerializerProvider>(ServiceLifetime.Singleton)
                //        .AddService<IEnumerable<IODataRoutingConvention>>(ServiceLifetime.Singleton, sp =>
                //               ODataRoutingConventions.CreateDefaultWithAttributeRouting("SensorThings", config)));

                //config.Routes.MapHttpRoute(
                //    name: "DefaultApi",
                //    routeTemplate: "api/{controller}/{id}",
                //    defaults: new { id = RouteParameter.Optional }
                //);
            }
        }
    }
}
