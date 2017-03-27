using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAEON.Observations.QuerySite.Models
{
    public class QueryModel
    {
        public List<Feature> Features { get; set; } = null;
        public List<Feature> SelectedFeatures { get; set; } = new List<Feature>();
        public List<Location> Locations { get; set; } = null;
        public List<Location> SelectedLocations { get; set; } = new List<Location>();
    }
}