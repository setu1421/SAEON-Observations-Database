using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SAEON.Observations.QuerySite.Models
{
    public class DataWizardModel : BaseModel
    {
        public List<LocationNode> LocationNodes { get; } = new List<LocationNode>();
        public List<LocationNode> LocationNodesSelected { get; } = new List<LocationNode>();
        public List<Location> Locations { get; } = new List<Location>();
        public List<VariableNode> VariableNodes { get; } = new List<VariableNode>();
        public List<VariableNode> VariableNodesSelected { get; } = new List<VariableNode>();
        public List<Variable> Variables { get; } = new List<Variable>();
        public List<MapPoint> MapPoints { get; } = new List<MapPoint>();
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
            LocationNodes.Clear();
            LocationNodesSelected.Clear();
            Locations.Clear();
            VariableNodes.Clear();
            VariableNodesSelected.Clear();
            Variables.Clear();
            MapPoints.Clear();
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