using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SAEON.Observations.QuerySite.Models
{
    public class QueryModel
    {
        public List<Feature> Features { get; set; }
        public List<Feature> SelectedFeatures { get; private set; } = new List<Feature>();
        public List<Location> Locations { get; set; }
        public List<Location> SelectedLocations { get; private set; } = new List<Location>();
        public List<QueryMapPoint> MapPoints { get; private set; } = new List<QueryMapPoint>();
        [DisplayName("Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Now.AddYears(-100).Date;
        [DisplayName("End Date")]
        public DateTime EndDate { get; set; } = DateTime.Now.Date;
        public DataQueryOutput Results { get; set; } = new DataQueryOutput();
        public List<UserQuery> UserQueries { get; set; }
    }

    public class LoadQueryModel
    {
        [Required, StringLength(150)]
        public string Name { get; set; }
    }

    public class SaveQueryModel
    {
        [Required, StringLength(150)]
        public string Name { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
    }
}