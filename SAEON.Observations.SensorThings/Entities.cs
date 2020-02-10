﻿using Microsoft.Spatial;
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
        [Required]
        public DateTime Time { get; set; }

        public TimeString() { }

        public TimeString(DateTime time)
        {
            Time = time;
        }

        public TimeString(DateTime? time)
        {
            Time = time ?? DateTime.Now;
        }

        public override string ToString()
        {
            return Time.ToString("o");
        }
    }

    public class TimeInterval
    {
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }

        public TimeInterval()
        {
        }

        public TimeInterval(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public override string ToString()
        {
            return $"{Start.ToString("o")}/{End.ToString("o")}";
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
        [Key, Column("id")]
        public Guid Id { get; set; }
        [NotMapped]
        public string SelfLink { get { return $"{Config.BaseUrl}/{EntitySetName}({Id})"; } set {; } }
        [NotMapped]
        public List<string> NavigationLinks { get; } = new List<string>();
    }

    public abstract class NamedSensorThingsEntity : SensorThingsEntity
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
        public TimeInterval PhenomenonTime { get; set; } = null;
        public TimeInterval ResultTime { get; set; } = null;

        public Thing Thing { get; set; } = null;
        public Sensor Sensor { get; set; } = null;
        public ObservedProperty ObservedProperty { get; set; } = null;
        //public List<Observation> Observations { get; } = new List<Observation>();

        public Datastream() : base()
        {
            EntitySetName = "Datastreams";
            NavigationLinks.Add("Thing");
            NavigationLinks.Add("Sensor");
            NavigationLinks.Add("ObservedProperty");
            //NavigationLinks.Add("Observations");
        }
    }

    public class FeatureOfInterest : SensorThingsEntity
    {
        public string EncodingType { get; set; } = ValueCodes.GeoJson;
        public GeoJSONPoint Feature { get; set; } = null;

        public List<Observation> Observations { get; set; } = new List<Observation>();

        public FeatureOfInterest() : base()
        {
            EntitySetName = "FeaturesOfInterest";
            NavigationLinks.Add("Observations");
        }
    }

    public class HistoricalLocation : SensorThingsEntity
    {
        public TimeString Time { get; set; } = new TimeString();

        public List<Location> Locations { get; set; } = new List<Location>();
        public Thing Thing { get; set; } = null;

        public HistoricalLocation() : base()
        {
            EntitySetName = "HistoricalLocations";
            NavigationLinks.Add("Locations");
            NavigationLinks.Add("Thing");
        }

        public HistoricalLocation(DateTime? time) : this()
        {
            Time.Time = time ?? DateTime.Now;
        }
    }

    public class Location : NamedSensorThingsEntity
    {
        public string EncodingType { get; set; } = ValueCodes.GeoJson;
        public GeoJSONPoint location { get; set; } = null;

        public List<Thing> Things { get; set; } = new List<Thing>();
        public List<HistoricalLocation> HistoricalLocations { get; set; } = new List<HistoricalLocation>();

        public Location() : base()
        {
            EntitySetName = "Locations";
            NavigationLinks.Add("Things");
            NavigationLinks.Add("HistoricalLocations");
        }
    }

    public class Observation : NamedSensorThingsEntity
    {
        public TimeString PhenomenonTime { get; set; } = null;
        public Double? Result { get; set; } = null;
        public TimeString ResultTime { get; set; } = null;
        public string ResultQuality { get; set; }
        public TimeInterval ValidTime { get; set; } = null;
        public ODataNamedValueDictionary<string> Parameters { get; } = new ODataNamedValueDictionary<string>();

        public Datastream Datastream { get; set; } = null;
        public FeatureOfInterest FeatureOfInterest { get; set; } = null;

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

        public List<Datastream> Datastreams { get; set; } = new List<Datastream>();

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

        public List<Datastream> Datastreams { get; set; } = new List<Datastream>();

        public Sensor() : base()
        {
            EntitySetName = "Sensors";
            NavigationLinks.Add("Datastreams");
        }
    }

    public class Thing : NamedSensorThingsEntity
    {
        public ODataNamedValueDictionary<string> Properties { get; } = new ODataNamedValueDictionary<string>();

        public List<Location> Locations { get; set; } = new List<Location>();
        public List<HistoricalLocation> HistoricalLocations { get; set; } = new List<HistoricalLocation>();
        public List<Datastream> Datastreams { get; } = new List<Datastream>();

        public Thing() : base()
        {
            EntitySetName = "Things";
            NavigationLinks.Add("Locations");
            NavigationLinks.Add("HistoricalLocations");
            NavigationLinks.Add("Datastreams");
        }
    }

}
