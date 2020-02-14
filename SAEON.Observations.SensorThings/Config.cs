using Microsoft.AspNet.OData.Builder;
using Microsoft.OData.Edm;

namespace SAEON.Observations.SensorThings
{
    public static class Config
    {
        public static string BaseUrl { get; set; }
        public const int PageSize = 25;
        public const int MaxTop = 500;
        public const int MaxAll = 1000;
        //public static int PageSize =>  int.Parse(ConfigurationManager.AppSettings["ODataPageSize"] ?? "25");
        //public static int MaxTop => int.Parse(ConfigurationManager.AppSettings["ODataMaxTop"] ?? "500");
        //public static int MaxAll => int.Parse(ConfigurationManager.AppSettings["ODataMaxTop"] ?? "1000");

        public static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder { ContainerName = "SensorThingsAPI" };
            builder.EnableLowerCamelCase();
            builder.AddComplexType(typeof(ODataNamedValueDictionary<string>));
            builder.EntitySet<Datastream>("Datastreams");
            builder.EntitySet<HistoricalLocation>("HistoricalLocations");
            builder.EntitySet<FeatureOfInterest>("FeaturesOfInterest");
            builder.EntitySet<Location>("Locations");
            builder.EntitySet<Observation>("Observations");
            builder.EntitySet<ObservedProperty>("ObservedProperties");
            builder.EntitySet<Sensor>("Sensors");
            builder.EntitySet<Thing>("Things");
            return builder.GetEdmModel();
        }
    }

}
