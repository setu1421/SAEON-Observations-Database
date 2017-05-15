using geoF = GeoJSON.Net.Feature;
using geoG = GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections;
using Newtonsoft.Json.Linq;

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

    public class LatLong
    {
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }

        public LatLong() { }
        public LatLong(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
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

    public class Property
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class PropertyList : List<Property> { }

    #endregion

    public class BaseSensorThingEntity
    {
        [Key]
        public Guid Id { get; set; }
        //[Url, NotMapped]
        //public string SelfLink { get; set; }
        [Url, NotMapped]
        protected List<string> NavigationLinks { get; private set; } = new List<string>();
        public Dictionary<string, object> SensorThingsProperties { get; private set; } = new Dictionary<string, object>();

        public virtual void GenerateSensorThingsProperties()
        {
            SensorThingsProperties.Clear();
            SensorThingsProperties.Add("iot_id", Id);
            SensorThingsProperties.Add("iot_selfLink", $"{SensorThings.BaseUrl}/{GetType().Name}s({Id})");
            foreach (var link in NavigationLinks)
            {
                SensorThingsProperties.Add($"{link}_iot_navigationLink", $"{GetType().Name}s({Id})/{link}");
            }
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
        public PropertyList Properties { get; private set; } = new PropertyList();
        //public Dictionary<string,string> Properties { get; private set; } = new Dictionary<string, string>();

        public Thing() : base()
        {
            NavigationLinks.Add("Locations");
            NavigationLinks.Add("HistoricalLocations");
            NavigationLinks.Add("DataStreams");
        }

        public void AddProperty(string key, object value)
        {
            if (value == null) return;
            Properties.Add(new Property { Key = key, Value = value.ToString() });
            //Properties.Add(key, value.ToString());
        }

        public List<Location> Locations { get; private set; } = new List<Location>();
        public List<Location> HistoricalLocations { get; private set; } = new List<Location>();
    }

    public class Location : BaseNamedSensorThingEntity
    {
        [Required]
        public string EncodingType { get; set; }
        [NotMapped]
        public double? Elevation { get; set; } = null;
        [NotMapped]
        public LatLong Point { get; set; }
        [NotMapped]
        public BoundingBox BoundingBox { get; set; }
        public geoF.Feature location { get; set; }

        public Location() : base()
        {
            NavigationLinks.Add("Things");
            NavigationLinks.Add("HistoricalLocations");
        }

        public override void GenerateSensorThingsProperties()
        {
            base.GenerateSensorThingsProperties();
            if (Point != null)
            {
                EncodingType = ValueCodes.GeoJson;
                var point = new geoG.Point(new geoG.GeographicPosition(Point.Latitude, Point.Longitude, Elevation));
                location = new geoF.Feature(point, null);
            }
        }

        public List<Thing> Things { get; private set; } = new List<Thing>();
        public List<Location> HistoricalLocations { get; private set; } = new List<Location>();
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
