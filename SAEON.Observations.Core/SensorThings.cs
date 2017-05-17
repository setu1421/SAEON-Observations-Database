using Newtonsoft.Json;
using SAEON.Observations.Core.GeoJSON;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        //public override string ToString()
        //{
        //}
    }

    public class TimeInterval
    {
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }

        public override string ToString()
        {
            return $"{Start.ToString("o")}/{End.ToString("o")}";
        }
    }

    public class TimeOrInterval
    {
        [Required]
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }

        public override string ToString()
        {
            var result = $"{Start.ToString("o")}";
            if (End.HasValue)
            {
                result += $"/{End.Value.ToString("o")}";
            }
            return result;
        }
    }

    public class UnitOfMeasurement
    {
        [Required]
        public string name { get; set; }
        [Required]
        public string symbol { get; set; }
        [Url, Required]
        public string definition { get; set; }
    }

    public class Property
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class PropertyList : List<Property> { }

    #endregion

    public class BaseSensorThingEntity
    {
        [Key]
        public Guid id { get; set; }
        //[Url, NotMapped]
        //public string SelfLink { get; set; }
        [Url, NotMapped]
        protected List<string> NavigationLinks { get; set; } = new List<string>();
        public Dictionary<string, object> SensorThingsProperties { get; set; } = new Dictionary<string, object>();

        public virtual void GenerateSensorThingsProperties()
        {
            SensorThingsProperties.Clear();
            SensorThingsProperties.Add("iot_id", id);
            SensorThingsProperties.Add("iot_selfLink", $"{SensorThings.BaseUrl}/{GetType().Name}s({id})");
            foreach (var link in NavigationLinks)
            {
                SensorThingsProperties.Add($"{link}_iot_navigationLink", $"{GetType().Name}s({id})/{link}");
            }
        }
    }

    public class BaseNamedSensorThingEntity : BaseSensorThingEntity
    {
        [Required]
        public string name { get; set; }
        [Required]
        public string description { get; set; }
    }

    public class Thing : BaseNamedSensorThingEntity
    {
        public PropertyList properties { get; set; } = new PropertyList();
        //public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        public Thing() : base()
        {
            NavigationLinks.Add("Locations");
            NavigationLinks.Add("HistoricalLocations");
            NavigationLinks.Add("DataStreams");
        }

        public void AddProperty(string key, object value)
        {
            if (value == null) return;
            properties.Add(new Property { key = key, value = value.ToString() });
            //properties.Add(key, value.ToString());
        }

        // Navigation
        public List<Location> Locations { get; set; } = new List<Location>();
        public List<HistoricalLocation> HistoricalLocations { get; set; } = new List<HistoricalLocation>();
        public List<DataStream> DataStreams { get; set; } = new List<DataStream>();
    }

    public class Location : BaseNamedSensorThingEntity
    {
        [Required]
        public string encodingType { get; set; } = ValueCodes.GeoJson;
        [NotMapped]
        public double? Elevation { get; set; } = null;
        [NotMapped]
        public GeoJSONCoordinate Coordinate { get; set; } = null;
        [NotMapped]
        public BoundingBox BoundingBox { get; set; }
        public GeoJSONFeatureGeometryPoint location { get; set; }

        public Location() : base()
        {
            NavigationLinks.Add("Things");
            NavigationLinks.Add("HistoricalLocations");
        }

        public override void GenerateSensorThingsProperties()
        {
            base.GenerateSensorThingsProperties();
            if (Coordinate != null)
            {
                location = new GeoJSONFeatureGeometryPoint(Coordinate);
            }
        }

        // Navigation
        public List<Thing> Things { get; set; } = new List<Thing>();
        public List<HistoricalLocation> HistoricalLocations { get; set; } = new List<HistoricalLocation>();
    }

    public class HistoricalLocation : BaseSensorThingEntity
    {
        private DateTime _time;
        [Required, NotMapped]
        public DateTime Time { get { return _time;
            } set {
                _time = value;
                time = value.ToString("o");
            } }
        [Required]
        public string time { get; set; } = null;

        public HistoricalLocation() : base()
        {
            NavigationLinks.Add("Thing");
            NavigationLinks.Add("Location");
        }

        // Navigation
        public Thing Thing { get; set; }
        public List<Location> Locations { get; set; } = new List<Location>();
    }

    public class DataStream : BaseNamedSensorThingEntity
    {
        [Required]
        public UnitOfMeasurement unitOfMeasurement { get; set; }
        [Required]
        public string observationType { get; set; }
        [NotMapped]
        public GeoJSONPolygon observedArea { get; set; }
        [NotMapped]
        public TimeInterval phenomenonTime { get; set; }
        [NotMapped]
        public TimeInterval resultTime { get; set; }

        public DataStream() : base()
        {
            NavigationLinks.Add("Thing");
            NavigationLinks.Add("Sensor");
            NavigationLinks.Add("ObservedProerties");
            NavigationLinks.Add("Observations");
        }

        public override void GenerateSensorThingsProperties()
        {
            base.GenerateSensorThingsProperties();
            if (phenomenonTime != null)
            {
                SensorThingsProperties.Add("phenomenonTime", phenomenonTime.ToString());
            }
            if (resultTime != null)
            {
                SensorThingsProperties.Add("resultTime", resultTime.ToString());
            }
        }

        // Navigation
        public Thing Thing { get; set; }
        public Sensor Sensor { get; set; }
        public ObservedProperty ObservedProperty { get; set; }
        public List<Observation> Observations { get; set; }
    }

    public class Sensor : BaseNamedSensorThingEntity
    {
        [Required]
        public string EncodingType { get; set; }
        [Required]
        public string Metadata { get; set; }

        public Sensor() : base()
        {
            NavigationLinks.Add("DataStreams");
        }

        // Navigation
        public List<DataStream> DataStreams { get; set; }
    }

    public class ObservedProperty : BaseNamedSensorThingEntity
    {
        [Required, Url]
        public string Definition { get; set; }

        public ObservedProperty() : base()
        {
            NavigationLinks.Add("DataStream");
        }
        // Navigation
        public DataStream DataStream { get; set; }
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

        public Observation() : base()
        {
            NavigationLinks.Add("DataStream");
            NavigationLinks.Add("FeatureOfInterest");
        }
        // Navigation
        public DataStream DataStream { get; set; }
        public FeatureOfInterest FeatureOfInterest { get; set; }
    }

    public class FeatureOfInterest : BaseNamedSensorThingEntity
    {
        [Required]
        public string EncodingType { get; set; }
        [Required]
        public string Feature { get; set; }

        public FeatureOfInterest() : base()
        {
            NavigationLinks.Add("Observations");
        }
        // Navigation
        public List<Observation> Observations { get; set; }
    }
}
