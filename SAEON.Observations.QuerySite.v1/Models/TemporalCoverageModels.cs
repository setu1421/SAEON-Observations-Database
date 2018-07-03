using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SAEON.Observations.QuerySite.Models
{
    public class TemporalCoverageModel
    {
        public List<Feature> Features { get; set; }
        public List<Feature> SelectedFeatures { get; private set; } = new List<Feature>();
        public List<Location> Locations { get; set; }
        public List<Location> SelectedLocations { get; private set; } = new List<Location>();
        [DisplayName("Start Date")]
        public DateTime StartDate { get; set; } = new DateTime(DateTime.Now.Year,DateTime.Now.Month,1);
        [DisplayName("End Date")]
        public DateTime EndDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);
        public TemporalCoverageOutput Results { get; set; } = new TemporalCoverageOutput();
    }

}