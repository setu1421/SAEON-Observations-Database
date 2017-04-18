using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SAEON.Observations.QuerySite.Models
{
    public class QueryModel
    {
        public List<Feature> Features { get; set; }
        public List<Feature> SelectedFeatures { get; set; } = new List<Feature>();
        public List<Location> Locations { get; set; }
        public List<Location> SelectedLocations { get; set; } = new List<Location>();
        [DisplayName("Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Now.AddYears(-100).Date;
        [DisplayName("End Date")]
        public DateTime EndDate { get; set; } = DateTime.Now.Date;
        public DataQueryOutput QueryResults { get; set; } = new DataQueryOutput();
    }
}