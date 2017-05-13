using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.Core.SensorThings
{
    public static class SensorThings
    {
        public static string BaseUrl { get; set; }
    }

    #region EmbeddedTypes
    public static class ValueCodes
    {
        public static readonly string GeoJson = "application/vnd.geo+json";
    }

    public class BoundingBox
    {
        [Required]
        public decimal Left { get; set; }
        [Required]
        public decimal Bottom { get; set; }
        [Required]
        public decimal Right { get; set; }
        [Required]
        public decimal Top { get; set; }
    }

    public class TimeInterval
    {
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
    }

    public class TimeOrInterval
    {
        [Required]
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
    }

    public class GeometryPoint
    {
        [Required]
        public decimal Latitude { get; set; }
        [Required]
        public decimal Longitude { get; set; }
    }

    public class UnitOfMeasurement
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Symbol { get; set; }
        [Url, Required]
        public string Definition { get; set; }
    }
    #endregion

    public class BaseSensorThingEntity
    {
        [Key]
        public Guid Id { get; set; }
        //[Url, NotMapped]
        //public string SelfLink { get; set; }
        //[Url, NotMapped]
        //public List<string> NavigationLinks { get; set; } = new List<string>();
        //public Dictionary<string, object> SensorThingsProperties { get; set; } = new Dictionary<string, object>();

        public BaseSensorThingEntity()
        {
            GenerateSensorThingsProperties();
        }

        protected virtual void GenerateSensorThingsProperties()
        {
            //SensorThingsProperties.Clear();
            //SensorThingsProperties.Add("@iot.id",Id);
            //SensorThingsProperties.Add("@iot.selfLink", $"{SensorThings.BaseUrl}/{GetType()}s({Id})");
            //foreach (var link in NavigationLinks)
            //{
            //    SensorThingsProperties.Add($"{link}@iot.navigationLink", $"{GetType()}s({Id})/{link}");
            //}
        }
    }

    public class BaseNamedSensorThingEntity : BaseSensorThingEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }

    public class Thing : BaseNamedSensorThingEntity
    {
        //[NotMapped]
        //public Dictionary<string, string> Properties { get; set; }
        public Dictionary<string, object> SensorThingsProperties { get; set; } = new Dictionary<string, object>();
        protected override void GenerateSensorThingsProperties()
        {
            base.GenerateSensorThingsProperties();
            SensorThingsProperties.Clear();
            SensorThingsProperties.Add("@iot.id", Id);
            SensorThingsProperties.Add("@iot.selfLink", $"{SensorThings.BaseUrl}/{GetType()}s({Id})");
        }
    }

    public class Thing2 
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        //public Dictionary<string, string> Properties { get; set; }
        public Dictionary<string, object> SensorThingsProperties { get; set; } = new Dictionary<string, object>();
    }

    public class Location : BaseNamedSensorThingEntity
    {
        [Required]
        public string EncodingType { get; set; }
        [Required, NotMapped]
        public GeometryPoint Point { get; set; }
    }

    public class HistoricalLocation : BaseSensorThingEntity
    {
        [Required, NotMapped]
        public DateTime Time { get; set; }

    }

    public class DataStream : BaseNamedSensorThingEntity
    {
        [Required, NotMapped]
        public UnitOfMeasurement UnitOfMeasure { get; set; }
        [Required]
        public string ObservationType { get; set; }
        [NotMapped]
        public BoundingBox ObservedArea { get; set; }
        [NotMapped]
        public TimeInterval PhenomenonTime { get; set; }
        [NotMapped]
        public TimeInterval ResultTime { get; set; }
    }

    public class Sensor : BaseNamedSensorThingEntity
    {
        [Required]
        public string EncodingType { get; set; }
        [Required]
        public string Metadata { get; set; }
    }

    public class ObservedProperty : BaseNamedSensorThingEntity
    {
        [Required, Url]
        public string Definition { get; set; }
    }

    public class Observation : BaseSensorThingEntity
    {
        [Required, NotMapped]
        public TimeOrInterval PhenomenonTime { get; set; }
        [Required, NotMapped]
        public double Result { get; set; }
        [Required, NotMapped]
        public DateTime ResultTime { get; set; }
        //public string ResultQuality { get; set; }
        [NotMapped]
        public TimeInterval ValidTime { get; set; }
        public List<string> Parameters { get; set; }
    }

    public class FeatureOfInterest : BaseNamedSensorThingEntity
    {
        [Required]
        public string EncodingType { get; set; }
        [Required]
        public string Feature { get; set; }
    }
}
