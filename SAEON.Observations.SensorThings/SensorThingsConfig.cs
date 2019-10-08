using Microsoft.AspNet.OData.Builder;
using Microsoft.OData.Edm;
using System.Configuration;

namespace SAEON.Observations.SensorThings
{
    public static class SensorThingsConfig
    {
        public static string BaseUrl { get; set; }
        public const int PageSize = 10; //int.Parse(ConfigurationManager.AppSettings["ODataPageSize"] ?? "10");

        public static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder { ContainerName = "SensorThingsAPI" };
            builder.EnableLowerCamelCase();
            builder.AddComplexType(typeof(ODataNamedValueDictionary<string>));
            builder.EntitySet<Thing>("Things");
            builder.EntitySet<Location>("Locations");
            return builder.GetEdmModel();
        }
    }

}
