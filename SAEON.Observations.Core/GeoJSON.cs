using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAEON.Observations.Core.GeoJSON
{
    public class Coordinate
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
    }

    public class GeometryPoint
    {
        public string type { get; set; } = "Point";
        private Coordinate coordinate;
        [NotMapped]
        public Coordinate Coordinate
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

    public class FeatureGeometryPoint
    {
        public string type { get; set; } = "Feature";
        public GeometryPoint geometry { get; set; } = new GeometryPoint();

        public FeatureGeometryPoint() { }
        public FeatureGeometryPoint(Coordinate coordinate)
        {
            geometry.Coordinate = coordinate;
        }
    }
}
