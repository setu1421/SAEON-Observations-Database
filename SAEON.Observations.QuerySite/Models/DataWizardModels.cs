using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SAEON.Observations.QuerySite.Models
{
    public class DataWizardModel : BaseModel
    {
        public List<LocationNode> Locations { get; private set; } = new List<LocationNode>();
        public List<LocationNode> LocationsSelected { get; private set; } = new List<LocationNode>();
        public List<Guid> Organisations { get; private set; } = new List<Guid>();
        public List<Guid> Sites { get; private set; } = new List<Guid>();
        public List<Guid> Stations { get; private set; } = new List<Guid>();
        public List<MapPoint> MapPoints { get; private set; } = new List<MapPoint>();
        public List<FeatureNode> Features { get; private set; } = new List<FeatureNode>();
        public List<FeatureNode> FeaturesSelected { get; private set; } = new List<FeatureNode>();
        public List<Guid> Phenomena { get; private set; } = new List<Guid>();
        public List<Guid> Offerings { get; private set; } = new List<Guid>();
        public List<Guid> Units { get; private set; } = new List<Guid>();
        [DisplayName("Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Now.AddYears(-100).Date;
        [DisplayName("End Date")]
        public DateTime EndDate { get; set; } = DateTime.Now.Date;
        public DataWizardApproximation Approximation { get; set; } = new DataWizardApproximation();
        public List<UserQuery> UserQueries { get; private set; } = new List<UserQuery>();
    }

    //public class LoadQueryModel
    //{
    //    [Required, StringLength(150)]
    //    public string Name { get; set; }
    //}

    //public class SaveQueryModel
    //{
    //    [Required, StringLength(150)]
    //    public string Name { get; set; }
    //    [StringLength(500)]
    //    public string Description { get; set; }
    //}
}