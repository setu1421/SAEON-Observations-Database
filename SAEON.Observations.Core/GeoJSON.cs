using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SAEON.Observations.Core.GeoJSON
{
    public class GeoJSONCoordinate
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double? Elevation { get; set; }

        public List<double> AsList()
        {
            var result = new List<double>
            {
                Longitude,
                Latitude
            };
            if (Elevation.HasValue)
            {
                result.Add(Elevation.Value);
            }
            return result;
        }

        public override string ToString()
        {
            var result = $"[{Longitude},{Latitude}";
            if (Elevation.HasValue)
                result += $",{Elevation.Value}";
            result += "]";
            return result;
        }
    }

    public class GeoJSONGeometryPoint
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "Point";
        private GeoJSONCoordinate coordinate;
        [NotMapped]
        public GeoJSONCoordinate Coordinate
        {
            get { return coordinate; }
            set
            {
                coordinate = value;
                Coordinates = coordinate.AsList();
            }
        }
        [JsonProperty("coordinates")]
        public List<Double> Coordinates { get; set; }
    }

    public class GeoJSONFeatureGeometryPoint
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "Feature";
        [JsonProperty("geometry")]
        public GeoJSONGeometryPoint Geometry { get; set; } = new GeoJSONGeometryPoint();

        public GeoJSONFeatureGeometryPoint() { }
        public GeoJSONFeatureGeometryPoint(GeoJSONCoordinate coordinate)
        {
            Geometry.Coordinate = coordinate;
        }
    }

    public class GeoJSONPolygon
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "Polygon";
        [NotMapped]
        private List<GeoJSONCoordinate> CoordinatesList { get; set; } = new List<GeoJSONCoordinate>();
        [JsonProperty("coordinates")]
        public List<string> Coordinates { get; set; }

        public void AddCoordinate(GeoJSONCoordinate coordinate)
        {
            CoordinatesList.Add(coordinate);
            Coordinates.Clear();
            Coordinates.AddRange(CoordinatesList.Select(i => i.ToString()));
        }
    }
}
