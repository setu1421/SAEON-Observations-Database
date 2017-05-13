using Microsoft.Web.OData.Builder;
using Newtonsoft.Json;
using SAEON.Observations.Core.SensorThings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;

namespace SAEON.Observations.WebAPI.Controllers.SensorThingsAPI
{
    public static class Configure
    {
        public static void ConfigureSensorThings(this HttpConfiguration config)
        {
            config.Count().Filter().OrderBy().Expand().Select().MaxTop(null);
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            var modelBuilder = new VersionedODataModelBuilder(config)
            {
                //ModelBuilderFactory = () => new ODataConventionModelBuilder().EnableLowerCamelCase(),
                DefaultModelConfiguration = (builder, apiVersion) =>
                    {
                        builder.EntitySet<Thing2>("Things").EntityType.HasDynamicProperties(i => i.SensorThingsProperties);
                    }
            };
            config.MapVersionedODataRoutes("sensorthings", "sensorthings", modelBuilder.GetEdmModels());
        }
    }
}