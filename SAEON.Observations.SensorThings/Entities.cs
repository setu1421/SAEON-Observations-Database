using Microsoft.Spatial;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAEON.Observations.SensorThings
{
    public static class ValueCodes
    {
        public static readonly string GeoJson = "application/vnd.geo+json";
        public static readonly string Pdf = "application/pdf";
        public static readonly string SensorML = "http://www.opengis.net/doc/IS/SensorML/2.0";
        public static readonly string OM_Measurement = "http://www.opengis.net/def/observationType/OGC-OM/2.0/OM_Measurement";
    }

    #region HelperClasses

    public class UnitOfMeasurement
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string Definition { get; set; }
    }

    public class TimeString
    {
        [Required, NotMapped]
        private DateTime time { get; set; }

        public TimeString(DateTime time)
        {
            this.time = time;
        }

        public TimeString(DateTime? time) : this(time ?? DateTime.Now) { }

        public override string ToString()
        {
            return time.ToString("o");
        }
    }

    public class TimeInterval
    {
        public DateTime _start { get; set; }
        public DateTime _end { get; set; }

        public TimeInterval(DateTime start, DateTime end)
        {
            _start = start;
            _end = end;
        }

        public override string ToString()
        {
            return $"{_start.ToString("o")}/{_end.ToString("o")}";
        }
    }

    public class GeoJSONPoint
    {
        public string Type { get; set; } = "Feature";
        public GeographyPoint Geometry { get; set; } = null;
        public GeoJSONPoint(GeographyPoint point)
        {
            Geometry = point;
        }
    }
    #endregion

    public abstract class SensorThingsEntity
    {
        [NotMapped]
        public string EntitySetName { get; protected set; }
        [NotMapped]
        public List<string> NavigationLinks { get; } = new List<string>();
    }

    public abstract class SensorThingsGuidIdEntity : SensorThingsEntity
    {
        [Key, Column("id")]
        public Guid Id { get; set; }
        [NotMapped]
        public string SelfLink { get { return $"{Config.BaseUrl}/{EntitySetName}({Id})"; } set {; } }
    }

    public abstract class SensorThingsIntIdEntity : SensorThingsEntity
    {
        [Key, Column("id")]
        public int Id { get; set; }
        [NotMapped]
        public string SelfLink { get { return $"{Config.BaseUrl}/{EntitySetName}({Id})"; } set {; } }
    }

    public abstract class NamedSensorThingsEntity : SensorThingsGuidIdEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class Datastream : NamedSensorThingsEntity
    {
        public UnitOfMeasurement UnitOfMeasurement { get; set; } = null;
        public string ObservationType { get; set; } = ValueCodes.OM_Measurement;
        public GeographyPolygon ObservedArea { get; set; } = null;
        [NotMapped]
        public TimeInterval PhenomenonTimeInterval { get; set; } = null;
        public string PhenomenonTime { get { return PhenomenonTimeInterval?.ToString(); } set {; } }
        [NotMapped]
        public TimeInterval ResultTimeInterval { get; set; } = null;
        public string ResultTime { get { return ResultTimeInterval?.ToString(); } set {; } }

        // Navigation
        public Thing Thing { get; set; }
        public Sensor Sensor { get; set; }
        public ObservedProperty ObservedProperty { get; set; }
        public List<Observation> Observations { get; }

        public Datastream() : base()
        {
            EntitySetName = "Datastreams";
            NavigationLinks.Add("Thing");
            NavigationLinks.Add("Sensor");
            NavigationLinks.Add("ObservedProperty");
            NavigationLinks.Add("Observations");
        }
    }

    public class FeatureOfInterest : SensorThingsGuidIdEntity
    {
        public string EncodingType { get; set; } = ValueCodes.GeoJson;
        public GeoJSONPoint Feature { get; set; } = null;

        // Navigation
        public List<Observation> Observations { get; set; }

        public FeatureOfInterest() : base()
        {
            EntitySetName = "FeaturesOfInterest";
            NavigationLinks.Add("Observations");
        }
    }

    public class HistoricalLocation : SensorThingsGuidIdEntity
    {
        [NotMapped]
        public TimeString TimeString { get; set; } = null;
        public string Time { get { return TimeString?.ToString(); } set {; } }

        // Navigation
        public List<Location> Locations { get; set; }
        public Thing Thing { get; set; }

        public HistoricalLocation() : base()
        {
            EntitySetName = "HistoricalLocations";
            NavigationLinks.Add("Locations");
            NavigationLinks.Add("Thing");
        }

        public HistoricalLocation(DateTime? time) : this()
        {
            TimeString = new TimeString(time);
        }
    }

    public class Location : NamedSensorThingsEntity
    {
        public string EncodingType { get; set; } = ValueCodes.GeoJson;
#pragma warning disable IDE1006 // Naming Styles
        public GeoJSONPoint location { get; set; } = null;
#pragma warning restore IDE1006 // Naming Styles

        // Navigation
        public List<Thing> Things { get; set; }
        public List<HistoricalLocation> HistoricalLocations { get; set; }

        public Location() : base()
        {
            EntitySetName = "Locations";
            NavigationLinks.Add("Things");
            NavigationLinks.Add("HistoricalLocations");
        }
    }

    public class Observation : SensorThingsIntIdEntity
    {
        [NotMapped]
        public TimeString PhenomenonTimeString { get; set; } = null;
        public string PhenomenonTime { get { return PhenomenonTimeString?.ToString(); } set {; } }
        public Double? Result { get; set; } = null;
        [NotMapped]
        public TimeString ResultTimeString { get; set; } = null;
        public string ResultTime { get { return ResultTimeString?.ToString(); } set {; } }
        //public string ResultQuality { get; set; }
        [NotMapped]
        public TimeInterval ValidTimeInterval { get; set; } = null;
        public string ValueTime { get { return ValidTimeInterval?.ToString(); } set {; } }
        public ODataNamedValueDictionary<string> Parameters { get; } = new ODataNamedValueDictionary<string>();

        // Navigation
        public Datastream Datastream { get; set; }
        public FeatureOfInterest FeatureOfInterest { get; set; }

        public Observation() : base()
        {
            EntitySetName = "Observation";
            NavigationLinks.Add("Datastream");
            NavigationLinks.Add("FeatureOfInterest");
        }
    }

    public class ObservedProperty : NamedSensorThingsEntity
    {
        public string Definition { get; set; }

        // Navigation
        public List<Datastream> Datastreams { get; set; }

        public ObservedProperty() : base()
        {
            EntitySetName = "ObservedProperties";
            NavigationLinks.Add("Datastreams");
        }
    }

    public class Sensor : NamedSensorThingsEntity
    {
        public string EncodingType { get; set; } = ValueCodes.Pdf;
        public string Metdadata { get; set; }

        // Navigation
        public List<Datastream> Datastreams { get; set; }

        public Sensor() : base()
        {
            EntitySetName = "Sensors";
            NavigationLinks.Add("Datastreams");
        }
    }

    public class Thing : NamedSensorThingsEntity
    {
        public ODataNamedValueDictionary<string> Properties { get; } = new ODataNamedValueDictionary<string>();

        // Navigation
        public List<Location> Locations { get; set; }
        public List<HistoricalLocation> HistoricalLocations { get; set; }
        public List<Datastream> Datastreams { get; }

        public Thing() : base()
        {
            EntitySetName = "Things";
            NavigationLinks.Add("Locations");
            NavigationLinks.Add("HistoricalLocations");
            NavigationLinks.Add("Datastreams");
        }
    }

}
