using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SAEON.Observations.QuerySite.Models
{

    public class DataWizardModel : BaseModel, IValidatableObject
    {
        public List<LocationTreeNode> LocationNodes { get; } = new List<LocationTreeNode>();
        public List<LocationTreeNode> LocationNodesSelected { get; } = new List<LocationTreeNode>();
        public List<LocationFilter> Locations { get; } = new List<LocationFilter>();
        public List<VariableTreeNode> VariableNodes { get; } = new List<VariableTreeNode>();
        public List<VariableTreeNode> VariableNodesSelected { get; } = new List<VariableTreeNode>();
        public List<VariableFilter> Variables { get; } = new List<VariableFilter>();
        public List<MapPoint> MapPoints { get; } = new List<MapPoint>();
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [IsBeforeDate(nameof(EndDate))]
        [DisplayName("Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Now.AddYears(-100).Date;
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [IsAfterDate(nameof(StartDate))]
        [DisplayName("End Date")]
        public DateTime EndDate { get; set; } = DateTime.Now.Date;
        [DisplayName("Minimum elevation")]
        public double ElevationMinimum { get; set; }
        [DisplayName("Maximum elevation")]
        public double ElevationMaximum { get; set; }
        public DataWizardApproximation Approximation { get; set; } = new DataWizardApproximation();
        public DataWizardDataOutput DataOutput { get; set; } = new DataWizardDataOutput();
        public bool IsAuthenticated { get; set; }
        public bool IsDataset { get; set; }
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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate > EndDate)
            {
                yield return new ValidationResult("Start Date can't be after End Date");
            }
            if (EndDate < StartDate)
            {
                yield return new ValidationResult("End Date can't be before Start Date");
            }
        }
    }

    public class StateModel
    {
        public bool IsAuthenticated { get; set; }
        public bool IsDataset { get; set; }
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