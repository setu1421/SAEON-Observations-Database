#if NET472
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
#endif
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
#if NET5_0_OR_GREATER
using System.Text.Json.Serialization;
#endif

namespace SAEON.Observations.Core
{
    public class LinkAttribute
    {
        public string Title { get; set; }

        public LinkAttribute() { }
        public LinkAttribute(string title)
        {
            Title = title;
        }
    }

    public abstract class TreeNode
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public string Key { get; set; }
        public string ParentKey { get; set; }
        public string Text { get; set; }
        public string Name { get; set; }
        public bool HasChildren { get; set; }
        public bool IsExpanded { get; set; }
        public bool IsSelected { get; set; }
        public bool IsChecked { get; set; }
        public LinkAttribute ToolTip { get; set; }
    }

    public class LocationTreeNode : TreeNode
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Elevation { get; set; }
        public string Url { get; set; }
    }

    public class VariableTreeNode : TreeNode { }

    #region Maps
    public class MapPoint
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Elevation { get; set; }
        public bool IsSelected { get; set; }
    }
    #endregion

    #region DataWizard

    public class LocationFilter
    {
        public Guid StationId { get; set; }

        public override bool Equals(object obj)
        {
            return obj is LocationFilter filter &&
                   StationId.Equals(filter.StationId);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(StationId);
        }
    }

    public class VariableFilter
    {
        public Guid PhenomenonId { get; set; }
        public Guid OfferingId { get; set; }
        public Guid UnitId { get; set; }

        public override bool Equals(object obj)
        {
            return obj is VariableFilter variable &&
                   PhenomenonId.Equals(variable.PhenomenonId) &&
                   OfferingId.Equals(variable.OfferingId) &&
                   UnitId.Equals(variable.UnitId);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PhenomenonId, OfferingId, UnitId);
        }
    }

    public class DataWizardDataInput : IValidatableObject
    {
        public List<LocationFilter> Locations { get; set; } = new List<LocationFilter>();
        public List<VariableFilter> Variables { get; set; } = new List<VariableFilter>();
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
        public double ElevationMinimum { get; set; } = -100; // m
        public double ElevationMaximum { get; set; } = 3000; // m
        //public string DownloadFormat { get; set; } = "CSV";

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

    public class ChartData
    {
        public DateTime Date { get; set; }
        public double? Value { get; set; }
    }

    public class ChartSeries
    {
        public string Station { get; set; }
        public string Phenomenon { get; set; }
        public string Offering { get; set; }
        public string Unit { get; set; }
        public string Name { get; set; }
        public string Caption { get; set; }
        public List<ChartData> Data { get; } = new List<ChartData>();
    }


    [SwaggerName("Observation")]
    public class ObservationDTO
    {
        [SwaggerIgnore, JsonIgnore]
        public int Id { get; set; }
        public string Station { get; set; }
        public string Variable => $"{Phenomenon.Replace(", ", "_")}, {Offering.Replace(", ", "_")}, {Unit.Replace(", ", "_")}";
        public DateTime Date { get; set; }
        public double? Value { get; set; }
        public string Comment { get; set; }
        public string Site { get; set; }
        public string Phenomenon { get; set; }
        public string Offering { get; set; }
        public string Unit { get; set; }
        public string UnitSymbol { get; set; }
        public string Instrument { get; set; }
        public string Sensor { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Elevation { get; set; }
        [SwaggerIgnore, JsonIgnore]
        public string Status { get; set; }
        [SwaggerIgnore, JsonIgnore]
        public string Reason { get; set; }

        public override bool Equals(object obj)
        {
            return obj is ObservationDTO dTO &&
                   Station == dTO.Station &&
                   Variable == dTO.Variable &&
                   Date == dTO.Date &&
                   Value == dTO.Value &&
                   Comment == dTO.Comment &&
                   Site == dTO.Site &&
                   Phenomenon == dTO.Phenomenon &&
                   Offering == dTO.Offering &&
                   Unit == dTO.Unit &&
                   Instrument == dTO.Instrument &&
                   Sensor == dTO.Sensor &&
                   Latitude == dTO.Latitude &&
                   Longitude == dTO.Longitude &&
                   Elevation == dTO.Elevation;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Station);
            hash.Add(Variable);
            hash.Add(Date);
            hash.Add(Value);
            hash.Add(Comment);
            hash.Add(Site);
            hash.Add(Phenomenon);
            hash.Add(Offering);
            hash.Add(Unit);
            hash.Add(Instrument);
            hash.Add(Sensor);
            hash.Add(Latitude);
            hash.Add(Longitude);
            hash.Add(Elevation);
            return hash.ToHashCode();
        }
    }

    public class DataWizardDataOutput
    {
        public Guid Id { get; } = Guid.NewGuid();
        public List<ObservationDTO> Data { get; set; } = new List<ObservationDTO>();
        public DateTime Date { get; } = DateTime.Now;
        public MetadataCore Metadata { get; private set; } = new MetadataCore();
        public List<ChartSeries> ChartSeries { get; } = new List<ChartSeries>();
    }

    public class DataWizardApproximation
    {
        public long RowCount { get; set; }
        public List<string> Errors { get; } = new List<string>();
    }

    public enum DownloadFormat { CSV, Excel, NetCDF }

    public class DataWizardDownloadInput : DataWizardDataInput
    {
#if NET472
        [JsonConverter(typeof(StringEnumConverter))]
#elif NET5_0_OR_GREATER
        [JsonConverter(typeof(JsonStringEnumConverter))]
#endif
        public DownloadFormat DownloadFormat { get; set; } = DownloadFormat.CSV;
    }
    #endregion

}
