using Microsoft.Web.OData.Builder;
using Newtonsoft.Json;
using SAEON.Observations.Core.Entities;
using System.Linq;
using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    public static class Configure
    {
        public static void ConfigureOData(this HttpConfiguration config)
        {
            //ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            ////builder.EntitySet<Feature>("Features");
            //builder.EntitySet<Instrument>("Instruments");
            ////builder.EntitySet<Location>("Locations");
            //builder.EntitySet<Offering>("Offerings");
            //builder.EntitySet<Organisation>("Organisations");
            //builder.EntitySet<Phenomenon>("Phenomena");
            //builder.EntitySet<Sensor>("Sensors");
            //builder.EntitySet<Site>("Sites");
            //builder.EntitySet<Station>("Stations");
            //builder.EntitySet<UnitOfMeasure>("UnitsOfMeasure");
            //builder.EntitySet<UserDownload>("UserDownloads");
            //builder.EntitySet<UserQuery>("UserQueries");
            //config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());

            config.Count().Filter().OrderBy().Expand().Select().MaxTop(null);
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            var modelBuilder = new VersionedODataModelBuilder(config)
            {
                DefaultModelConfiguration = (builder, apiVersion) =>
                {
                    //builder.EntitySet<Feature>("Features");
                    builder.EntitySet<Instrument>("Instruments");
                    //builder.EntitySet<Location>("Locations");
                    builder.EntitySet<Offering>("Offerings");
                    builder.EntitySet<Organisation>("Organisations");
                    builder.EntitySet<Phenomenon>("Phenomena");
                    builder.EntitySet<Sensor>("Sensors");
                    builder.EntitySet<Site>("Sites");
                    builder.EntitySet<Station>("Stations");
                    builder.EntitySet<UnitOfMeasure>("UnitsOfMeasure");
                    builder.EntitySet<UserDownload>("UserDownloads");
                    builder.EntitySet<UserQuery>("UserQueries");
                }
            };
            config.MapVersionedODataRoutes("odata", "odata", modelBuilder.GetEdmModels());
        }
    }
}