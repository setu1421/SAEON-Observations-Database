using SAEON.Observations.Core;
using System.Collections.Generic;

namespace SAEON.Observations.QuerySite.Models
{
    public class HomeModel : BaseModel
    {
        public int Organisations { get; set; }
        public int Programmes { get; set; }
        public int Projects { get; set; }
        public int Sites { get; set; }
        public int Stations { get; set; }
        public int Instruments { get; set; }
        public int Sensors { get; set; }
        public int Phenomena { get; set; }
        public int Offerings { get; set; }
        public int UnitsOfMeasure { get; set; }
        public int Variables { get; set; }
        public int Datasets { get; set; }
        public int Observations { get; set; }
        public List<MapPoint> MapPoints { get; } = new List<MapPoint>();
    }
}