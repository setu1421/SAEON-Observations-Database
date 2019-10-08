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

    public class GeometryPoint
    {
        public string Type { get; set; } = "Point";
        public double[] Coordinates { get; private set; }
        public GeometryPoint(double latitude, double longitude, double? elevation)
        {
            if (elevation.HasValue)
            {
                Coordinates = new double[3] { longitude, latitude, elevation.Value };
            }
            else
            {
                Coordinates = new double[2] { longitude, latitude };
            }
        }
    }

    public class GeometryLocation
    {
        public string Type { get; set; } = "Feature";
        public GeometryPoint Geometry { get; set; }
        public GeometryLocation(double latitude, double longitude, double? elevation)
        {
            Geometry = new GeometryPoint(latitude, longitude, elevation);
        }
    }

    public abstract class SensorThingsEntity
    {
        [NotMapped]
        public string EntitySetName { get; protected set; }
        [Key, Column("id")]
        public Guid Id { get; set; }
        [NotMapped]
        public string SelfLink { get { return $"{SensorThingsConfig.BaseUrl}/{EntitySetName}({Id})"; } set {; } }
        [NotMapped]
        public List<string> NavigationLinks { get; } = new List<string>();
    }

    public abstract class NamedSensorThingsEntity : SensorThingsEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }

    public class Thing : NamedSensorThingsEntity
    {
        public ODataNamedValueDictionary<string> Properties { get; } = new ODataNamedValueDictionary<string>();
        public Location Location { get; set; } = null;
        //public List<HistoricalLocation> HistoricalLocations { get; } = new List<HistoricalLocation>();
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
        public GeometryLocation location { get; set; } = null;
        //public List<HistoricalLocation> HistoricalLocations { get; } = new List<HistoricalLocation>();

        public Location() : base()
        {
            EntitySetName = "Locations";
            NavigationLinks.Add("Things");
            NavigationLinks.Add("HistoricalLocations");
        }

        // Navigation
        [NotMapped]
        public List<Thing> Things { get; set; }
    }

}
