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
        public static readonly string Pdf = "application/pdf";
        public static readonly string OM_Measurement = "http://www.opengis.net/def/observationType/OGC-OM/2.0/OM_Measurement";
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
        [Required, JsonProperty("name")]
        public string Name { get; set; }
        [Required, JsonProperty("symbol")]
        public string Symbol { get; set; }
        [Url, Required, JsonProperty("definition")]
        public string Definition { get; set; }
    }

    public class Property
    {
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class PropertyList : List<Property> { }

    #endregion

    public class BaseSensorThingEntity
    {
        [Key, JsonProperty("id")]
        public Guid Id { get; set; }
        //[Url, NotMapped]
        //public string SelfLink { get; set; }
        [Url, NotMapped]
        protected List<string> NavigationLinks { get; set; } = new List<string>();
        public Dictionary<string, object> SensorThingsProperties { get; set; } = new Dictionary<string, object>();

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
        [Required, JsonProperty("name")]
        public string Name { get; set; }
        [Required, JsonProperty("description")]
        public string Description { get; set; }
    }

    public class Thing : BaseNamedSensorThingEntity
    {
        [JsonProperty("properties")]
        public PropertyList Properties { get; set; } = new PropertyList();
        //public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        public Thing() : base()
        {
            NavigationLinks.Add("Locations");
            NavigationLinks.Add("HistoricalLocations");
            NavigationLinks.Add("Datastreams");
        }

        public void AddProperty(string key, object value)
        {
            if (value == null) return;
            Properties.Add(new Property { Key = key, Value = value.ToString() });
            //properties.Add(key, value.ToString());
        }

        // Navigation
        public List<Location> Locations { get; set; } = new List<Location>();
        public List<HistoricalLocation> HistoricalLocations { get; set; } = new List<HistoricalLocation>();
        public List<Datastream> Datastreams { get; private set; } = new List<Datastream>();
    }

    public class Location : BaseNamedSensorThingEntity
    {
        [Required, JsonProperty("encodingType")]
        public string EncodingType { get; private set; } = ValueCodes.GeoJson;
        [NotMapped]
        public double? Elevation { get; set; } = null;
        [NotMapped]
        public GeoJSONCoordinate Coordinate { get; set; } = null;
        [NotMapped]
        public BoundingBox BoundingBox { get; set; }
        [JsonProperty("location")]
        public GeoJSONFeatureGeometryPoint Location_ { get; set; }

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
                Location_ = new GeoJSONFeatureGeometryPoint(Coordinate);
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
                TimeStr = value.ToString("o");
            } }
        [Required, JsonProperty("time")]
        public string TimeStr { get; set; } = null;

        public HistoricalLocation() : base()
        {
            NavigationLinks.Add("Thing");
            NavigationLinks.Add("Location");
        }

        // Navigation
        public Thing Thing { get; set; }
        public List<Location> Locations { get; set; } = new List<Location>();
    }

    public class Datastream : BaseNamedSensorThingEntity
    {
        [Required, JsonProperty("unitOfMeasurement")]
        public UnitOfMeasurement UnitOfMeasurement { get; set; }
        [Required, JsonProperty("observationType")]
        public string ObservationType { get; set; }
        [NotMapped]
        public GeoJSONPolygon ObservedArea { get; set; }
        [NotMapped]
        public TimeInterval PhenomenonTime { get; set; }
        [NotMapped]
        public TimeInterval ResultTime { get; set; }

        public Datastream() : base()
        {
            NavigationLinks.Add("Thing");
            NavigationLinks.Add("Sensor");
            NavigationLinks.Add("ObservedProerties");
            NavigationLinks.Add("Observations");
        }

        public override void GenerateSensorThingsProperties()
        {
            base.GenerateSensorThingsProperties();
            if (PhenomenonTime != null)
            {
                SensorThingsProperties.Add("phenomenonTime", PhenomenonTime.ToString());
            }
            if (ResultTime != null)
            {
                SensorThingsProperties.Add("resultTime", ResultTime.ToString());
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
        [Required, JsonProperty("encodingType")]
        public string EncodingType { get; set; }
        [Required, JsonProperty("metadata")]
        public string Metadata { get; set; }

        public Sensor() : base()
        {
            NavigationLinks.Add("Datastreams");
        }

        // Navigation
        public List<Datastream> Datastreams { get; private set; } = new List<Datastream>();
    }

    public class ObservedProperty : BaseNamedSensorThingEntity
    {
        [Required, Url, JsonProperty("definition")]
        public string Definition { get; set; }

        public ObservedProperty() : base()
        {
            NavigationLinks.Add("Datastream");
        }
        // Navigation
        public Datastream Datastream { get; set; }
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
            NavigationLinks.Add("Datastream");
            NavigationLinks.Add("FeatureOfInterest"); 
        }
        // Navigation
        public Datastream Datastream { get; set; }
        public FeatureOfInterest FeatureOfInterest { get; set; }
    }

    public class FeatureOfInterest : BaseNamedSensorThingEntity
    {
        [Required, JsonProperty("encodingType")]
        public string EncodingType { get; private set; } = ValueCodes.GeoJson;
        [NotMapped]
        public GeoJSONCoordinate Coordinate { get; set; } = null;
        [JsonProperty("feature")]
        public GeoJSONFeatureGeometryPoint Feature { get; set; }

        public FeatureOfInterest() : base()
        {
            NavigationLinks.Add("Observations");
        }

        public override void GenerateSensorThingsProperties()
        {
            base.GenerateSensorThingsProperties();
            if (Coordinate != null)
            {
                Feature = new GeoJSONFeatureGeometryPoint(Coordinate);
            }
        }
        // Navigation
        public List<Observation> Observations { get; set; }
    }
}
