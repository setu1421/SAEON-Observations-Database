using Microsoft.OData;
using Microsoft.Spatial;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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
        [Required]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }

    public class Thing : NamedSensorThingsEntity
    {
        public ODataNamedValueDictionary<string> Properties { get; } = new ODataNamedValueDictionary<string>();
        public Location Location { get; set; } = null;
        public List<HistoricalLocation> HistoricalLocations { get; set; } = new List<HistoricalLocation>();
        //public List<Datastream> Datastreams { get; } = new List<Datastream>();

        public Thing() : base()
        {
            EntitySetName = "Things";
            NavigationLinks.Add("Location");
            NavigationLinks.Add("HistoricalLocations");
            NavigationLinks.Add("Datastreams");
        }
    }

    public class Location : NamedSensorThingsEntity
    {
        public string EncodingType { get; private set; } = ValueCodes.GeoJson;
        public GeographyPoint location { get; set; } = null;
        public List<Thing> Things { get; set; } = new List<Thing>();
        public List<HistoricalLocation> HistoricalLocations { get; set; } = new List<HistoricalLocation>();

        public Location() : base()
        {
            EntitySetName = "Locations";
            NavigationLinks.Add("Things");
            NavigationLinks.Add("HistoricalLocations");
        }
    }

    public class HistoricalLocation : SensorThingsEntity
    {
        public DateTime Time { get; set; }
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
            Time = time ?? DateTime.Now;
        }
    }

    public class Datastream : NamedSensorThingsEntity
    {
        public UnitOfMeasurement UnitOfMeasurement { get; set; } = null;
        public string ObservationType { get; set; } = ValueCodes.OM_Measurement;
        public GeographyPolygon ObservedArea { get; set; } = null;
        public TimeInterval PhenomenonTime { get; set; } = null;
        public TimeInterval ResultTime { get; set; } = null;
        public Thing Thing { get; set; } = null;
        //public Sensor Sensor { get; set; } = null;
        //public ObservedProperty ObservedProperty { get; set; } = null;
        //public List<Observation> Observations { get; } = new List<Observation>();

        public Datastream() : base()
        {
            EntitySetName = "Datastreams";
            NavigationLinks.Add("Thing");
            NavigationLinks.Add("Sensor");
            NavigationLinks.Add("ObservedProperty");
            NavigationLinks.Add("Observations");
        }
    }

}
