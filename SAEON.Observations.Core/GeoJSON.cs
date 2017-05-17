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
            var result = new List<double>();
            result.Add(Longitude);
            result.Add(Latitude);
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
        public string type { get; set; } = "Point";
        private GeoJSONCoordinate coordinate;
        [NotMapped]
        public GeoJSONCoordinate Coordinate
        {
            get { return coordinate; }
            set
            {
                coordinate = value;
                coordinates = coordinate.AsList();
            }
        }
        public List<Double> coordinates { get; set; }
    }

    public class GeoJSONFeatureGeometryPoint
    {
        public string type { get; set; } = "Feature";
        public GeoJSONGeometryPoint geometry { get; set; } = new GeoJSONGeometryPoint();

        public GeoJSONFeatureGeometryPoint() { }
        public GeoJSONFeatureGeometryPoint(GeoJSONCoordinate coordinate)
        {
            geometry.Coordinate = coordinate;
        }
    }

    public class GeoJSONPolygon
    {
        public string type { get; set; } = "Polygon";
        [NotMapped]
        private List<GeoJSONCoordinate> coordinatesList { get; set; } = new List<GeoJSONCoordinate>();
        public List<string> coordinates { get; set; }

        public void AddCoordinate(GeoJSONCoordinate coordinate)
        {
            coordinatesList.Add(coordinate);
            coordinates.Clear();
            coordinates.AddRange(coordinatesList.Select(i => i.ToString()));
        }
    }
}
