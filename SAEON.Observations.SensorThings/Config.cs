using Microsoft.AspNet.OData.Builder;
using Microsoft.OData.Edm;

namespace SAEON.Observations.SensorThings
{
    public static class Config
    {
        public static string BaseUrl { get; set; }
        public const int PageSize = 25; //int.Parse(ConfigurationManager.AppSettings["ODataPageSize"] ?? "25");

        public static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder { ContainerName = "SensorThingsAPI" };
            builder.EnableLowerCamelCase();
            builder.AddComplexType(typeof(ODataNamedValueDictionary<string>));
            builder.EntitySet<Thing>("Things");
            builder.EntitySet<Location>("Locations");
            builder.EntitySet<HistoricalLocation>("HistoricalLocations");
            builder.EntitySet<Datastream>("Datastreams");
            return builder.GetEdmModel();
        }
    }

}
