using Microsoft.OData.Edm;
using Newtonsoft.Json;
using SAEON.Logs;
using SAEON.Observations.Core.Entities;
//using SAEON.Observations.Core;
//using SAEON.Observations.WebAPI.Controllers.SensorThingsAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using System.Web.OData;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using System.Web.OData.Routing;
using System.Web.OData.Routing.Conventions;
using ef = SAEON.Observations.Core.Entities;
//using sos = SAEON.Observations.Core.SensorThings;

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

        public static ODataRoute MapControllerBoundODataServiceRoute(this HttpConfiguration configuration, string routeName, string routePrefix,
            IEdmModel model)
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
    }

    public static class ODataExtensions
    {
        public static void IgnoreEntityItemLists<TEntity>(this EntitySetConfiguration<TEntity> source) where TEntity : NamedEntity
        {
            using (Logging.MethodCall(typeof(ODataExtensions)))
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
        public static void Register(HttpConfiguration config)
        {
            using (Logging.MethodCall(typeof(WebApiConfig)))
            {
                config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                config.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
                config.Formatters.Remove(config.Formatters.XmlFormatter);

                // Web API routes
                //config.MapHttpAttributeRoutes();
                config.MapHttpAttributeRoutes(new InheritedDirectRouteProvider());

                // OData
                config.Filter().Expand().Select().OrderBy().MaxTop(null).Count();

                // Entities

                ODataConventionModelBuilder odataModelBuilder = new ODataConventionModelBuilder { ContainerName = "Observations" };
                odataModelBuilder.EntitySet<ef.Instrument>("Instruments").IgnoreEntityItemLists();
                odataModelBuilder.EntitySet<ef.Offering>("Offerings").IgnoreEntityItemLists();
                odataModelBuilder.EntitySet<ef.Organisation>("Organisations").IgnoreEntityItemLists();
                odataModelBuilder.EntitySet<ef.Phenomenon>("Phenomena").IgnoreEntityItemLists();
                odataModelBuilder.EntitySet<ef.Programme>("Programmes").IgnoreEntityItemLists();
                odataModelBuilder.EntitySet<ef.Project>("Projects").IgnoreEntityItemLists();
                odataModelBuilder.EntitySet<ef.Sensor>("Sensors").IgnoreEntityItemLists();
                odataModelBuilder.EntitySet<ef.Site>("Sites").IgnoreEntityItemLists();
                odataModelBuilder.EntitySet<ef.Station>("Stations").IgnoreEntityItemLists();
                odataModelBuilder.EntitySet<ef.Unit>("Units").IgnoreEntityItemLists();
                //@odataModelBuilder.EntitySet<ef.UserDownload>("UserDownloads");
                //@odataModelBuilder.EntitySet<ef.UserQuery>("UserQueries");
                config.MapControllerBoundODataServiceRoute("OData", "OData", odataModelBuilder.GetEdmModel());

                /*
                // SensorThings
                ODataConventionModelBuilder sensorThingsModelBuilder = new ODataConventionModelBuilder { ContainerName = "SensorThings" };
                sensorThingsModelBuilder.EntitySet<sos.Thing>("Things");
                sensorThingsModelBuilder.EntitySet<sos.Location>("Locations");
                sensorThingsModelBuilder.EntitySet<sos.HistoricalLocation>("HistoricalLocations");
                sensorThingsModelBuilder.EntitySet<sos.Datastream>("Datastreams");
                sensorThingsModelBuilder.EntitySet<sos.Sensor>("Sensors");
                sensorThingsModelBuilder.EntitySet<sos.ObservedProperty>("ObservedProperties");
                sensorThingsModelBuilder.EntitySet<sos.Observation>("Observations");
                sensorThingsModelBuilder.EntitySet<sos.FeatureOfInterest>("FeaturesOfInterest");
                //config.MapControllerBoundODataServiceRoute("SensorThings", "SensorThings", sensorThingsModelBuilder.GetEdmModel());

                SensorThingsHelper.Load();
                */

                //config.Routes.MapHttpRoute(
                //    name: "DefaultApi",
                //    routeTemplate: "api/{controller}/{id}",
                //    defaults: new { id = RouteParameter.Optional }
                //);
            }
        }
    }
}
