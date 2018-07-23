using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.Restier.Core;
using Microsoft.Restier.Core.Model;
using Microsoft.Restier.Providers.EntityFramework;
using Microsoft.Restier.Publishers.OData;
using Microsoft.Restier.Publishers.OData.Batch;
using Newtonsoft.Json;
using SAEON.Logs;
using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using System.Web.OData;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using System.Web.OData.Query;
using System.Web.OData.Routing;
using System.Web.OData.Routing.Conventions;

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

    public class ObservationsRESTierApi : EntityFrameworkApi<ObservationsDbContext>
    {
        public ObservationsRESTierApi(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected static new IServiceCollection ConfigureApi(Type apiType, IServiceCollection services)
        {
            // Add customized OData validation settings 
            ODataValidationSettings validationSettingFactory(IServiceProvider sp) => new ODataValidationSettings
            {
                MaxAnyAllExpressionDepth = 3,
                MaxExpansionDepth = 3
            };

            return EntityFrameworkApi<ObservationsDbContext>.ConfigureApi(apiType, services)
                .AddSingleton<ODataValidationSettings>(validationSettingFactory)
                .AddService<IModelBuilder, ObservationsModelBuilder>();
        }

        private class ObservationsModelBuilder : IModelBuilder
        {
            public async Task<IEdmModel> GetModelAsync(ModelContext context, CancellationToken cancellationToken)
            {
                return await Task.Run<IEdmModel>(() =>
                {
                    return WebApiConfig.GetObservationsEdmModel();
                });
            }
        }
    }

    public class InternalRESTierApi : EntityFrameworkApi<ObservationsDbContext>
    {
        public InternalRESTierApi(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected static new IServiceCollection ConfigureApi(Type apiType, IServiceCollection services)
        {
            // Add customized OData validation settings 
            ODataValidationSettings validationSettingFactory(IServiceProvider sp) => new ODataValidationSettings
            {
                MaxAnyAllExpressionDepth = 3,
                MaxExpansionDepth = 3
            };

            return EntityFrameworkApi<ObservationsDbContext>.ConfigureApi(apiType, services)
                .AddSingleton<ODataValidationSettings>(validationSettingFactory)
                .AddService<IModelBuilder, InternalModelBuilder>();
        }

        private class InternalModelBuilder : IModelBuilder
        {
            public async Task<IEdmModel> GetModelAsync(ModelContext context, CancellationToken cancellationToken)
            {
                return await Task.Run<IEdmModel>(() =>
                {
                    return WebApiConfig.GetInternalEdmModel();
                });
            }
        }
    }

    public static class WebApiConfig
    {
        public static IEdmModel GetObservationsEdmModel()
        {
            ODataConventionModelBuilder odataModelBuilder = new ODataConventionModelBuilder { ContainerName = "Observations" };
            odataModelBuilder.EntitySet<Instrument>("Instruments").IgnoreEntityItemLists();
            odataModelBuilder.EntitySet<Offering>("Offerings").IgnoreEntityItemLists();
            odataModelBuilder.EntitySet<Organisation>("Organisations").IgnoreEntityItemLists();
            odataModelBuilder.EntitySet<Phenomenon>("Phenomena").IgnoreEntityItemLists();
            odataModelBuilder.EntitySet<Programme>("Programmes").IgnoreEntityItemLists();
            odataModelBuilder.EntitySet<Project>("Projects").IgnoreEntityItemLists();
            odataModelBuilder.EntitySet<Sensor>("Sensors").IgnoreEntityItemLists();
            odataModelBuilder.EntitySet<Site>("Sites").IgnoreEntityItemLists();
            odataModelBuilder.EntitySet<Station>("Stations").IgnoreEntityItemLists();
            odataModelBuilder.EntitySet<Unit>("Units").IgnoreEntityItemLists();
            return odataModelBuilder.GetEdmModel();
        }

        public static IEdmModel GetInternalEdmModel()
        {
            ODataConventionModelBuilder odataModelBuilder = new ODataConventionModelBuilder { ContainerName = "Internal" };
            odataModelBuilder.EntitySet<Inventory>("Inventory");
            return odataModelBuilder.GetEdmModel();
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

                // Web API routes
                //config.MapHttpAttributeRoutes();
                config.MapHttpAttributeRoutes(new InheritedDirectRouteProvider());

                // OData
                config.Filter().Expand().Select().OrderBy().MaxTop(null).Count();
                //config.MapODataServiceRoute("OData", "ODataOld", GetEdmModel());
                //config.MapControllerBoundODataServiceRoute("ODataOld", "ODataOld", GetObservationsEdmModel());
                //config.MapControllerBoundODataServiceRoute("Internal", "Internal", GetInternalEdmModel());

                // RESTier
                config.MapRestierRoute<ObservationsRESTierApi>("ObservationsRESTier", "OData",
                    new RestierBatchHandler(GlobalConfiguration.DefaultServer)).GetAwaiter().GetResult();
                config.MapRestierRoute<InternalRESTierApi>("InternalRESTier", "Internal",
                    new RestierBatchHandler(GlobalConfiguration.DefaultServer)).GetAwaiter().GetResult();

                //config.Routes.MapHttpRoute(
                //    name: "DefaultApi",
                //    routeTemplate: "api/{controller}/{id}",
                //    defaults: new { id = RouteParameter.Optional }
                //);
            }
        }
    }
}
