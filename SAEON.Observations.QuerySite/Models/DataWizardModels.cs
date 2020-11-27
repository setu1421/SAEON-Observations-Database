using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SAEON.Observations.QuerySite.Models
{
    public class DataWizardModel : BaseModel
    {
        public List<LocationNode> Locations { get; } = new List<LocationNode>();
        public List<LocationNode> LocationsSelected { get; } = new List<LocationNode>();
        public List<Guid> Organisations { get; } = new List<Guid>();
        public List<Guid> Sites { get; } = new List<Guid>();
        public List<Guid> Stations { get; } = new List<Guid>();
        public List<MapPoint> MapPoints { get; } = new List<MapPoint>();
        public List<FeatureNode> Features { get; } = new List<FeatureNode>();
        public List<FeatureNode> FeaturesSelected { get; } = new List<FeatureNode>();
        public List<Guid> Phenomena { get; } = new List<Guid>();
        public List<Guid> Offerings { get; } = new List<Guid>();
        public List<Guid> Units { get; } = new List<Guid>();
        [DisplayName("Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Now.AddYears(-100).Date;
        [DisplayName("End Date")]
        public DateTime EndDate { get; set; } = DateTime.Now.Date;
        [DisplayName("Minimum elevation")]
        public float ElevationMinimum { get; set; }
        [DisplayName("Maximum elevation")]
        public float ElevationMaximum { get; set; }
        public DataWizardApproximation Approximation { get; set; } = new DataWizardApproximation();
        public DataWizardDataOutput DataOutput { get; set; } = new DataWizardDataOutput();
        public bool IsAuthenticated { get; set; }
        public bool HaveSearched { get; set; }
        public List<UserDownload> UserDownloads { get; } = new List<UserDownload>();
        public List<UserQuery> UserQueries { get; } = new List<UserQuery>();

        public override void Clear()
        {
            Locations.Clear();
            LocationsSelected.Clear();
            Organisations.Clear();
            Sites.Clear();
            Stations.Clear();
            MapPoints.Clear();
            Features.Clear();
            FeaturesSelected.Clear();
            Phenomena.Clear();
            Offerings.Clear();
            Units.Clear();
            StartDate = DateTime.Now.AddYears(-100).Date;
            EndDate = DateTime.Now.Date;
            ElevationMinimum = -100;
            ElevationMaximum = 3000;
            Approximation = new DataWizardApproximation();
            DataOutput = new DataWizardDataOutput();
            IsAuthenticated = false;
            HaveSearched = false;
            UserDownloads.Clear();
            UserQueries.Clear();
        }
    }

    public class StateModel
    {
        public bool IsAuthenticated { get; set; }
        public bool LoadEnabled { get; set; }
        public bool SaveEnabled { get; set; }
        public bool SearchEnabled { get; set; }
        public bool DownloadEnabled { get; set; }
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