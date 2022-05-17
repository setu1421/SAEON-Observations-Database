﻿#if NET472
using Newtonsoft.Json;
#endif
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
#if NET5_0
using System.Text.Json.Serialization;
#endif

namespace SAEON.Observations.Core
{
    public static class EntityConfig
    {
        public static string BaseUrl { get; set; }
    }

    /// <summary>
    /// Absolute base class
    /// </summary>
    public abstract class BaseEntity
    {
        [NotMapped, JsonIgnore, SwaggerIgnore]
        public string EntitySetName { get; protected set; } = null;
        [NotMapped, JsonIgnore, SwaggerIgnore]
        public List<string> Links { get; } = new List<string>();
    }

    public abstract class GuidIdEntity : BaseEntity
    {
        /// <summary>
        /// Id of the Entity
        /// </summary>
        //[Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        //[HiddenInput]
        public Guid Id { get; set; }

        /// <summary>
        /// Navigation links of this Entity
        /// </summary>
        [NotMapped, JsonIgnore, SwaggerIgnore]
        public Dictionary<string, string> NavigationLinks
        {
            get
            {
                if (string.IsNullOrWhiteSpace(EntitySetName))
                    return null;
                else
                {
                    var result = new Dictionary<string, string>
                    {
                        { "Self", $"{EntityConfig.BaseUrl}/{EntitySetName}/{Id}" }
                    };
                    foreach (var link in Links.OrderBy(i => i))
                    {
                        result.Add(link, $"{EntityConfig.BaseUrl}/{EntitySetName}/{Id}/{link}");
                    }
                    return result;
                };
            }
        }
    }

    public abstract class IntIdEntity : BaseEntity
    {
        /// <summary>
        /// Id of the Entity
        /// </summary>
        //[Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        //[HiddenInput]
        public int Id { get; set; }

        /// <summary>
        /// Navigation links of this Entity
        /// </summary>
        [NotMapped, JsonIgnore, SwaggerIgnore]
        public Dictionary<string, string> NavigationLinks
        {
            get
            {
                if (string.IsNullOrWhiteSpace(EntitySetName))
                    return null;
                else
                {
                    var result = new Dictionary<string, string>
                    {
                        { "Self", $"{EntityConfig.BaseUrl}/{EntitySetName}/{Id}" }
                    };
                    foreach (var link in Links.OrderBy(i => i))
                    {
                        result.Add(link, $"{EntityConfig.BaseUrl}/{EntitySetName}/{Id}/{link}");
                    }
                    return result;
                };
            }
        }
    }

    public abstract class LongIdEntity : BaseEntity
    {
        /// <summary>
        /// Id of the Entity
        /// </summary>
        //[Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        //[HiddenInput]
        public long Id { get; set; }

        /// <summary>
        /// Navigation links of this Entity
        /// </summary>
        [NotMapped, JsonIgnore, SwaggerIgnore]
        public Dictionary<string, string> NavigationLinks
        {
            get
            {
                if (string.IsNullOrWhiteSpace(EntitySetName))
                    return null;
                else
                {
                    var result = new Dictionary<string, string>
                    {
                        { "Self", $"{EntityConfig.BaseUrl}/{EntitySetName}/{Id}" }
                    };
                    foreach (var link in Links.OrderBy(i => i))
                    {
                        result.Add(link, $"{EntityConfig.BaseUrl}/{EntitySetName}/{Id}/{link}");
                    }
                    return result;
                };
            }
        }
    }

    /// <summary>
    /// Base for entities
    /// </summary>
    public abstract class IdedEntity : GuidIdEntity
    {
        [JsonIgnore, SwaggerIgnore, Timestamp, Column(Order = 10000), ConcurrencyCheck, ScaffoldColumn(false)]
        //[HiddenInput]
        [IgnoreDataMember]
        public byte[] RowVersion { get; set; }
        [JsonIgnore, SwaggerIgnore, Required]
        [IgnoreDataMember]
        public Guid UserId { get; set; }
    }

    public abstract class CodedEntity : IdedEntity
    {
        /// <summary>
        /// Code of the Entity
        /// </summary>
        [Required, StringLength(50)]
        public virtual string Code { get; set; }
    }

    public abstract class NamedEntity : IdedEntity
    {
        /// <summary>
        /// Name of the Entity
        /// </summary>
        [Required, StringLength(150)]
        public virtual string Name { get; set; }
    }

    public abstract class CodedNamedEntity : IdedEntity
    {
        /// <summary>
        /// Code of the Entity
        /// </summary>
        [Required, StringLength(50)]
        public virtual string Code { get; set; }
        /// <summary>
        /// Name of the Entity
        /// </summary>
        [Required, StringLength(150)]
        public virtual string Name { get; set; }
    }

    /// <summary>
    /// Data Schema entity
    /// </summary>
    [Table("DataSchema")]
    public class DataSchema : CodedNamedEntity
    {
        /// <summary>
        /// DataSourceTypeId of the DataSchema
        /// </summary>
        [Required]
        public Guid DataSourceTypeId { get; set; }
        /// <summary>
        /// Description of the DataSchema
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }
        /// <summary>
        /// Delimiter of the DataSchema
        /// </summary>
        [StringLength(3)]
        public string Delimiter { get; set; }

        // Navigation
        [JsonIgnore, SwaggerIgnore, IgnoreDataMember]
        public DataSourceType DataSourceType { get; set; }
    }

    /// <summary>
    /// Data Source entity
    /// </summary>
    [Table("DataSource")]
    public class DataSource : CodedNamedEntity
    {
        public Guid DataSchemaId { get; set; }
        /// <summary>
        /// Description of the DataSource
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }
        /// <summary>
        /// Url of the DataSource
        /// </summary>
        [Url, StringLength(250)]
        public string Url { get; set; }
        /// <summary>
        /// Update Frequency of the DataSource
        /// Enum of {AdHoc, Daily, Weekly, Monthly, Quarterly, Annually, Hourly}
        /// </summary>
        public int UpdateFreq { get; set; }
        /// <summary>
        /// Last update of the DataSource
        /// </summary>
        public DateTime LastUpdate { get; set; }
        /// <summary>
        /// StartDate of the DataSource, null means always
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// EndDate of the DataSource, null means always
        /// </summary>
        public DateTime? EndDate { get; set; }

        // Navigation
        [JsonIgnore, SwaggerIgnore, IgnoreDataMember]
        public DataSchema DataSchema { get; set; }
        [JsonIgnore, SwaggerIgnore]
        public List<Sensor> Sensors { get; set; }
    }

    [Table("DataSourceType")]
    public class DataSourceType : CodedEntity
    {
        //public EntityListItem AsEntityListItem => new EntityListItem { Id = Id, Code = Code };

        /// <summary>
        /// Description of the Instrument
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }
        public new Guid? UserId { get; set; }


        // Navigation
        [JsonIgnore, SwaggerIgnore]
        public List<DataSchema> DataSchemas { get; set; }
    }

    public abstract class DatasetBase : CodedNamedEntity
    {
        [StringLength(200)]
        public override string Code { get => base.Code; set => base.Code = value; }
        [StringLength(500)]
        public override string Name { get => base.Name; set => base.Name = value; }
        /// <summary>
        /// Title of the Dataset
        /// </summary>
        [StringLength(5000)]
        public string Title { get; set; }
        /// <summary>
        /// Description of the Dataset
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }
        /// <summary>
        /// StationId of the dataset
        /// </summary>
        public Guid? StationId { get; set; }
        /// <summary>
        /// PhenomenonOfferingId of the dataset
        /// </summary>
        public Guid? PhenomenonOfferingId { get; set; }
        /// <summary>
        /// PhenomenonUOMId of the dataset
        /// </summary>
        [Column("PhenomenonUOMId")]
        public Guid? PhenomenonUnitId { get; set; }
        /// <summary>
        /// DigitalObjectIdentifierId of the dataset
        /// </summary>
        public int? DigitalObjectIdentifierId { get; set; }
        /// <summary>
        /// Count of observations in the dataset
        /// </summary>
        public int? Count { get; set; }
        /// <summary>
        /// Count of observations with values in the dataset
        /// </summary>
        public int? ValueCount { get; set; }
        /// <summary>
        /// Count of observations of null in the dataset
        /// </summary>
        public int? NullCount { get; set; }
        /// <summary>
        /// Count of verified observations in the dataset
        /// </summary>
        public int? VerifiedCount { get; set; }
        /// <summary>
        /// Count of unverified observations in the dataset
        /// </summary>
        public int? UnverifiedCount { get; set; }
        /// <summary>
        /// StartDate of the dataset
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// EndDate of the dataset
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// LatitudeNorth of the dataset
        /// </summary>
        public double? LatitudeNorth { get; set; }
        /// <summary>
        /// LatitudeSouth of the dataset
        /// </summary>
        public double? LatitudeSouth { get; set; }
        /// <summary>
        /// LongitudeWest of the dataset
        /// </summary>
        public double? LongitudeWest { get; set; }
        /// <summary>
        /// LongitudeEast of the dataset
        /// </summary>
        public double? LongitudeEast { get; set; }
        /// <summary>
        /// ElevationMinimum of the dataset
        /// </summary>
        public double? ElevationMinimum { get; set; }
        /// <summary>
        /// ElevationMaximum of the dataset
        /// </summary>
        public double? ElevationMaximum { get; set; }
        /// <summary>
        /// If CSV for dataset needs to be rebuilt
        /// </summary>
        public bool? NeedsUpdate { get; set; } // CSV needs to be rebuild
        /// <summary> 
        /// UserId of user who added the DigitalObjectIdentifier   
        /// </summary>
        [StringLength(36), ScaffoldColumn(false)]
        public string AddedBy { get; set; }
        /// <summary>
        /// UserId of user who last updated the DigitalObjectIdentifier 
        /// </summary>
        [StringLength(36), ScaffoldColumn(false)]
        public string UpdatedBy { get; set; }
        [JsonIgnore, SwaggerIgnore]
        /// <summary>
        /// HashCode of the dataset
        /// </summary>
        public int HashCode { get; set; }

        public int CreateHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(StationId);
            hashCode.Add(PhenomenonOfferingId);
            hashCode.Add(PhenomenonUnitId);
            hashCode.Add(Count);
            hashCode.Add(StartDate);
            hashCode.Add(EndDate);
            hashCode.Add(LatitudeNorth);
            hashCode.Add(LatitudeSouth);
            hashCode.Add(LongitudeWest);
            hashCode.Add(LongitudeEast);
            hashCode.Add(ElevationMinimum);
            hashCode.Add(ElevationMaximum);
            return hashCode.ToHashCode();
        }

        // Navigation
        [JsonIgnore, SwaggerIgnore]
        public Station Station { get; set; }
        [JsonIgnore, SwaggerIgnore]
        public DigitalObjectIdentifier DigitalObjectIdentifier { get; set; }
    }

    /// <summary>
    /// Dataset entity
    /// </summary>
    public class Dataset : DatasetBase { }

    public class VDatasetExpansion : DatasetBase
    {
        public Guid OrganisationId { get; set; }
        public string OrganisationCode { get; set; }
        public string OrganisationName { get; set; }
        public string OrganisationDescription { get; set; }
        public string OrganisationUrl { get; set; }
        public Guid ProgrammeId { get; set; }
        public string ProgrammeCode { get; set; }
        public string ProgrammeName { get; set; }
        public string ProgrammeDescription { get; set; }
        public string ProgrammeUrl { get; set; }
        public Guid ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public string ProjectUrl { get; set; }
        public Guid SiteId { get; set; }
        public string SiteCode { get; set; }
        public string SiteName { get; set; }
        public string SiteDescription { get; set; }
        public string SiteUrl { get; set; }
        public string StationCode { get; set; }
        public string StationName { get; set; }
        public string StationDescription { get; set; }
        public string StationUrl { get; set; }
        public Guid PhenomenonId { get; set; }
        public string PhenomenonCode { get; set; }
        public string PhenomenonName { get; set; }
        public string PhenomenonDescription { get; set; }
        //public Guid PhenomenonOfferingID { get; set; }
        public Guid OfferingId { get; set; }
        public string OfferingCode { get; set; }
        public string OfferingName { get; set; }
        public string OfferingDescription { get; set; }
        //[Column("PhenomenonUOMID")]
        //public Guid PhenomenonUnitID { get; set; }
        [Column("UnitOfMeasureID")]
        public Guid UnitId { get; set; }
        [Column("UnitOfMeasureCode")]
        public string UnitCode { get; set; }
        [Column("UnitOfMeasureUnit")]
        public string UnitName { get; set; }
        [Column("UnitOfMeasureSymbol")]
        public string UnitSymbol { get; set; }

        public string Variable => $"{PhenomenonName.Replace(", ", "_")}, {OfferingName.Replace(", ", "_")}, {UnitName.Replace(", ", "_")}";
    }

    public enum DOIType { /*ObservationsDb, Collection, Organisation, Programme, Project, Site, Station,*/ Dataset = 7/*, Periodic, AdHoc*/ }

    /// <summary>
    /// DigitalObjectIdentifiers entity
    /// </summary>
    public class DigitalObjectIdentifier : IntIdEntity
    {
        //[HiddenInput]
        //public int? ParentId { get; set; }
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public Guid? AlternateId { get; set; }
        [Required]
        [DisplayName("DOI Type")]
        public DOIType DOIType { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string DOI { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Display(Name = "DOI Url"), Url]
        public string DOIUrl { get; set; }
        public Guid? DatasetId { get; set; }
        /// <summary>
        /// Code of the DigitalObjectIdentifier
        /// </summary>
        [Required, StringLength(200)]
        public string Code { get; set; }
        /// <summary>
        /// Name of the DigitalObjectIdentifier
        /// </summary>
        [Required, StringLength(500)]
        public string Name { get; set; }
        /// <summary>
        /// Title of the DigitalObjectIdentifier
        /// </summary>
        [Required, StringLength(5000)]
        public string Title { get; set; }
        /// <summary>
        /// Description of the DigitalObjectIdentifier
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }
        /// <summary>
        /// Description of the DigitalObjectIdentifier as Html 
        /// </summary>
        [DisplayName("Description")]
        public string DescriptionHtml { get; set; }
        /// <summary>
        /// Citation of the DigitalObjectIdentifier
        /// </summary>
        public string Citation { get; set; }
        /// <summary>
        /// Citation of the DigitalObjectIdentifier as Html 
        /// </summary>
        [DisplayName("Citation")]
        public string CitationHtml { get; set; }
        /// <summary>
        /// MetadataJson of the DigitalObjectIdentifier
        /// </summary>
        [Required]
        public string MetadataJson { get; set; }
        /// <summary>
        /// Sha256 of the MetadataJson of the DigitalObjectIdentifier
        /// </summary>
        [Required, MinLength(32), MaxLength(32)]
        public byte[] MetadataJsonSha256 { get; set; }
        /// <summary>
        /// MetadataHtml of the DigitalObjectIdentifier
        /// </summary>
        [Required]
        public string MetadataHtml { get; set; }
        /// <summary>
        /// Url of the ODP metadata record of the DigitalObjectIdentifier
        /// </summary>
        [Required, StringLength(250), Url]
        public string MetadataUrl { get; set; }
        /// <summary>
        /// Object Store Url of the DigitalObjectIdentifier
        /// </summary>
        [StringLength(250), Url]
        public string ObjectStoreUrl { get; set; }
        /// <summary>
        /// Query Url of the DigitalObjectIdentifier
        /// </summary>
        [StringLength(250), Url]
        public string QueryUrl { get; set; }
        /// <summary>
        /// ODP Id for the DigitalObjectIdentifier
        /// </summary>
        public Guid? ODPMetadataId { get; set; }
        /// <summary>
        /// ODP metadata needs update for DigitalObjectIdentifier
        /// </summary>
        public bool? ODPMetadataNeedsUpdate { get; set; }
        /// <summary>
        /// ODP metadata is valid for DigitalObjectIdentifier
        /// </summary>
        public bool? ODPMetadataIsValid { get; set; }
        public string ODPMetadataErrors { get; set; }
        public bool? ODPMetadataIsPublished { get; set; }
        public string ODPMetadataPublishErrors { get; set; }
        /// <summary> 
        /// UserId of user who added the DigitalObjectIdentifier   
        /// </summary>
        [StringLength(36), ScaffoldColumn(false)]
        public string AddedBy { get; set; }
        /// <summary>
        /// UserId of user who last updated the DigitalObjectIdentifier 
        /// </summary>
        [StringLength(36), ScaffoldColumn(false)]
        public string UpdatedBy { get; set; }
        [Timestamp, ConcurrencyCheck, ScaffoldColumn(false)]
        //[HiddenInput]
        public byte[] RowVersion { get; set; }

        // Navigation
        [JsonIgnore, SwaggerIgnore]
        public Dataset Dataset { get; set; }
        //[JsonIgnore, SwaggerIgnore, IgnoreDataMember]
        //public DigitalObjectIdentifier Parent { get; set; }
        //[JsonIgnore, SwaggerIgnore]
        //public List<DigitalObjectIdentifier> Children { get; set; }
        //[JsonIgnore, SwaggerIgnore]
        //public List<Organisation> Organisations { get; set; }
        //[JsonIgnore, SwaggerIgnore]
        //public List<Programme> Programmes { get; set; }
        //[JsonIgnore, SwaggerIgnore]
        //public List<Project> Projects { get; set; }
        //[JsonIgnore, SwaggerIgnore]
        //public List<Site> Sites { get; set; }
        //[JsonIgnore, SwaggerIgnore]
        //public List<Station> Stations { get; set; }
        //[JsonIgnore, SwaggerIgnore]
        //public List<ImportBatchSummary> ImportBatchSummaries { get; set; }
        //[JsonIgnore, SwaggerIgnore]
        //public List<UserDownload> UserDownloads { get; set; }

        public void SetUrls()
        {
            MetadataUrl = $"https://catalogue.saeon.ac.za/records/{DOI}";
            QueryUrl = $"https://observations.saeon.ac.za/Dataset/{DOI}";
        }
    }

    [Table("ImportBatch")]
    public class ImportBatch : GuidIdEntity
    {
        public int Code { get; set; }
        public Guid DataSourceId { get; set; }
        public DateTime ImportDate { get; set; }
        public int Status { get; set; }
        [StringLength(250)]
        public string FileName { get; set; }
        [StringLength(8000)]
        public string Comment { get; set; }
        public Guid? StatusId { get; set; }
        public Guid? StatusReasonId { get; set; }
        [StringLength(1000)]
        public string Issues { get; set; }
        public int? DurationInSecs { get; set; }

        // Navigation
        [JsonIgnore, SwaggerIgnore]
        public DataSource DataSource { get; set; }
        [JsonIgnore, SwaggerIgnore]
        public List<ImportBatchSummary> ImportBatchSummaries { get; set; }
    }

    [Table("ImportBatchSummary")]
    public class ImportBatchSummary : GuidIdEntity
    {
        public Guid ImportBatchId { get; set; }
        public Guid SensorId { get; set; }
        //public Guid InstrumentId { get; set; }
        //public Guid StationId { get; set; }
        //public Guid SiteId { get; set; }
        public Guid PhenomenonOfferingId { get; set; }
        [Column("PhenomenonUOMID")]
        public Guid PhenomenonUnitId { get; set; }
        //[JsonIgnore, SwaggerIgnore, IgnoreDataMember]
        //public int? DigitalObjectIdentifierId { get; set; }
        public int Count { get; set; }
        public double? Minimum { get; set; }
        public double? Maximum { get; set; }
        public double? Average { get; set; }
        public double? StandardDeviation { get; set; }
        public double? Variance { get; set; }
        public double? LatitudeNorth { get; set; } // +N to -S
        public double? LatitudeSouth { get; set; } // +N to -S 
        public double? LongitudeWest { get; set; } // -W to +E
        public double? LongitudeEast { get; set; } // -W to +E 
        public double? ElevationMinimum { get; set; }
        public double? ElevationMaximum { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? VerifiedCount { get; set; }

        // Navigation
        [JsonIgnore, SwaggerIgnore]
        public ImportBatch ImportBatch { get; set; }
        [JsonIgnore, SwaggerIgnore]
        public Sensor Sensor { get; set; }
        //public Instrument Instrument { get; set; }
        //public Station Station { get; set; }
        //public Site Site { get; set; }
        //[JsonIgnore, SwaggerIgnore]
        //public PhenomenonOffering PhenomenonOffering { get; set; }
        //[JsonIgnore, SwaggerIgnore]
        //public PhenomenonUnit PhenomenonUnit { get; set; }
        //[JsonIgnore, SwaggerIgnore, IgnoreDataMember]
        //public DigitalObjectIdentifier DigitalObjectIdentifier { get; set; }
    }

    /// <summary>
    /// Instrument entity
    /// </summary>
    [Table("Instrument")]
    public class Instrument : CodedNamedEntity
    {
        /// <summary>
        /// Description of the Instrument
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }
        /// <summary>
        /// Url of the Instrument
        /// </summary>
        [Url, StringLength(250)]
        public string Url { get; set; }
        /// <summary>
        /// StartDate of the Instrument, null means always
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// EndDate of the Instrument, null means always
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// Latitude of the Instrument
        /// </summary>
        public double? Latitude { get; set; }
        /// <summary>
        /// Longitude of the Instrument
        /// </summary>
        public double? Longitude { get; set; }
        /// <summary>
        /// Elevation of the Instrument, positive above sea level, negative below sea level
        /// </summary>
        public double? Elevation { get; set; }

        // Navigation
        [JsonIgnore, SwaggerIgnore]
        public List<Organisation> Organisations { get; set; }

        [JsonIgnore, SwaggerIgnore]
        public List<Station> Stations { get; set; }

        [JsonIgnore, SwaggerIgnore]
        public List<Sensor> Sensors { get; set; }

        public Instrument() : base()
        {
            EntitySetName = "Instruments";
            Links.Add("Organisations");
            Links.Add("Stations");
            Links.Add("Sensors");
        }
    }

    /// <summary>
    /// Offering entity
    /// </summary>
    [Table("Offering")]
    public class Offering : CodedNamedEntity
    {
        /// <summary>
        /// Description of the Offering
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }

        // Navigation
        [JsonIgnore, SwaggerIgnore]
        public List<Phenomenon> Phenomena { get; set; }

        public Offering() : base()
        {
            EntitySetName = "Offerings";
            Links.Add("Phenomena");
        }
    }

    /// <summary>
    /// Organisation entity
    /// </summary>
    [Table("Organisation")]
    public class Organisation : CodedNamedEntity
    {
        /// <summary>
        /// Description of the Organisation
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }
        /// <summary>
        /// Url of the Organisation
        /// </summary>
        [Url, StringLength(250)]
        public string Url { get; set; }

        /// <summary>
        /// DigitalObjectIdentifierID of the Organisation
        /// </summary>
        //[JsonIgnore, IgnoreDataMember, SwaggerIgnore]
        //public int? DigitalObjectIdentifierID { get; set; }

        // Navigation
        //[JsonIgnore, IgnoreDataMember, SwaggerIgnore]
        //public DigitalObjectIdentifier DigitalObjectIdentifier { get; set; }
        [JsonIgnore, SwaggerIgnore]
        public List<Instrument> Instruments { get; set; }
        [JsonIgnore, SwaggerIgnore]
        public List<Site> Sites { get; set; }
        [JsonIgnore, SwaggerIgnore]
        public List<Station> Stations { get; set; }

        public Organisation() : base()
        {
            EntitySetName = "Organisations";
            Links.Add("Sites");
            Links.Add("Stations");
            Links.Add("Instruments");
        }
    }

    /// <summary>
    /// Phenomenon entity
    /// </summary>
    [Table("Phenomenon")]
    public class Phenomenon : CodedNamedEntity
    {
        /// <summary>
        /// Description of the Phenomenon
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }
        /// <summary>
        /// Url of the Phenomenon
        /// </summary>
        [Url, StringLength(250)]
        public string Url { get; set; }

        // Navigation
        [JsonIgnore, SwaggerIgnore]
        public List<Offering> Offerings { get; set; }
        [JsonIgnore, SwaggerIgnore]
        public List<Unit> Units { get; set; }
        [JsonIgnore, SwaggerIgnore]
        public List<Sensor> Sensors { get; set; }

        public Phenomenon() : base()
        {
            EntitySetName = "Phenomena";
            Links.Add("Offerings");
            Links.Add("Sensors");
            Links.Add("Units");
        }
    }

    /// <summary>
    /// Programme entity
    /// </summary>
    [Table("Programme")]
    public class Programme : CodedNamedEntity
    {
        /// <summary>
        /// Description of the Programme
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }
        /// <summary>
        /// Url of the Programme
        /// </summary>
        [Url, StringLength(250)]
        public string Url { get; set; }
        /// <summary>
        /// StartDate of the Programme, null means always
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// EndDate of the Programme, null means always
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// DigitalObjectIdentifierID of the Programme
        /// </summary>
        //[JsonIgnore, IgnoreDataMember, SwaggerIgnore]
        //public int? DigitalObjectIdentifierID { get; set; }

        // Navigation
        //[JsonIgnore, IgnoreDataMember, SwaggerIgnore]
        //public DigitalObjectIdentifier DigitalObjectIdentifier { get; set; }
        [JsonIgnore, SwaggerIgnore]
        public List<Project> Projects { get; set; }

        public Programme() : base()
        {
            EntitySetName = "Programmes";
            Links.Add("Projects");
        }
    }

    /// <summary>
    /// Project entity
    /// </summary>
    [Table("Project")]
    public class Project : CodedNamedEntity
    {
        /// <summary>
        /// The Programme of the Project
        /// </summary>
        [Required, JsonIgnore]
        public Guid ProgrammeId { get; set; }
        /// <summary>
        /// Description of the Project
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }
        /// <summary>
        /// Url of the Project
        /// </summary>
        [Url, StringLength(250)]
        public string Url { get; set; }
        /// <summary>
        /// StartDate of the Project, null means always
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// EndDate of the Project, null means always
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// DigitalObjectIdentifierID of the Project
        /// </summary>
        //[JsonIgnore, IgnoreDataMember, SwaggerIgnore]
        //public int? DigitalObjectIdentifierID { get; set; }

        // Navigation
        //[JsonIgnore, IgnoreDataMember, SwaggerIgnore]
        //public DigitalObjectIdentifier DigitalObjectIdentifier { get; set; }
        [JsonIgnore, SwaggerIgnore]
        public Programme Programme { get; set; }
        [JsonIgnore, SwaggerIgnore]
        public List<Station> Stations { get; set; }

        public Project() : base()
        {
            EntitySetName = "Projects";
            Links.Add("Programmes");
            Links.Add("Stations");
        }
    }

    /// <summary>
    /// Sensor entity
    /// </summary>
    [Table("Sensor")]
    public class Sensor : CodedNamedEntity
    {
        [StringLength(75)]
        public override string Code { get => base.Code; set => base.Code = value; }
        /// <summary>
        /// Description of the Sensor
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }
        /// <summary>
        /// Url of the Sensor
        /// </summary>
        [Url, StringLength(250)]
        public string Url { get; set; }
        [Required, IgnoreDataMember]
        public Guid DataSourceId { get; set; }
        /// <summary>
        /// PhenomenonId of the sensor
        /// </summary>
        public Guid PhenomenonId { get; set; }
        /// <summary>
        /// Latitude of the Sensor
        /// </summary>
        public double? Latitude { get; set; }
        /// <summary>
        /// Longitude of the Sensor
        /// </summary>
        public double? Longitude { get; set; }
        /// <summary>
        /// Elevation of the Sensor, positive above sea level, negative below sea level
        /// </summary>
        public double? Elevation { get; set; }

        // Navigation
        [JsonIgnore, SwaggerIgnore, IgnoreDataMember]
        public DataSource DataSource { get; set; }
        [JsonIgnore, SwaggerIgnore]
        public Phenomenon Phenomenon { get; set; }
        [JsonIgnore, SwaggerIgnore]
        public List<Instrument> Instruments { get; set; }

        public Sensor() : base()
        {
            EntitySetName = "Sensors";
            Links.Add("Phenomena");
            Links.Add("Instruments");
            //Links.Add("Observations"); @@
        }
    }

    /// <summary>
    /// Site entity
    /// </summary>
    [Table("Site")]
    public class Site : CodedNamedEntity
    {
        /// <summary>
        /// Description of the Site
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }
        /// <summary>
        /// Url of the Site
        /// </summary>
        [Url, StringLength(250)]
        public string Url { get; set; }
        /// <summary>
        /// StartDate of the Site, null means always
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// EndDate of the Site, null means always
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// DigitalObjectIdentifierID of the Site
        /// </summary>
        //[JsonIgnore, IgnoreDataMember, SwaggerIgnore]
        //public int? DigitalObjectIdentifierID { get; set; }

        // Navigation
        //[JsonIgnore, IgnoreDataMember, SwaggerIgnore]
        //public DigitalObjectIdentifier DigitalObjectIdentifier { get; set; }
        [JsonIgnore, SwaggerIgnore]
        public List<Organisation> Organisations { get; set; }
        [JsonIgnore, SwaggerIgnore]
        public List<Station> Stations { get; set; }

        public Site() : base()
        {
            EntitySetName = "Sites";
            Links.Add("Organisations");
            Links.Add("Stations");
        }
    }

    /// <summary>
    /// Station entity
    /// </summary>
    [Table("Station")]
    public class Station : CodedNamedEntity
    {
        /// <summary>
        /// Description of the Station
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }
        /// <summary>
        /// The SiteId of the Station
        /// </summary>
        [Required, JsonIgnore]
        public Guid SiteId { get; set; }
        /// <summary>
        /// Url of the Station
        /// </summary>
        [Url, StringLength(250)]
        public string Url { get; set; }
        /// <summary>
        /// StartDate of the site, null means always
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// EndDate of the Station, null means always
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// Latitude of the Station
        /// </summary>
        public double? Latitude { get; set; }
        /// <summary>
        /// Longitude of the Station
        /// </summary>
        public double? Longitude { get; set; }
        /// <summary>
        /// Elevation of the Station, positive above sea level, negative below sea level
        /// </summary>
        public double? Elevation { get; set; }
        //[JsonIgnore, IgnoreDataMember, SwaggerIgnore]
        //public int? DigitalObjectIdentifierID { get; set; }

        // Navigation
        //[JsonIgnore, IgnoreDataMember, SwaggerIgnore]
        //public DigitalObjectIdentifier DigitalObjectIdentifier { get; set; }
        [JsonIgnore, SwaggerIgnore]
        public Site Site { get; set; }
        [JsonIgnore, SwaggerIgnore]
        public List<Organisation> Organisations { get; set; }
        [JsonIgnore, SwaggerIgnore]
        public List<Project> Projects { get; set; }
        [JsonIgnore, SwaggerIgnore]
        public List<Instrument> Instruments { get; set; }
        [JsonIgnore, SwaggerIgnore]
        public List<Dataset> Datasets { get; set; }
        [JsonIgnore, SwaggerIgnore]
        public List<Observation> Observations { get; set; }

        public Station() : base()
        {
            EntitySetName = "Stations";
            Links.Add("Site");
            Links.Add("Organisations");
            Links.Add("Projects");
            Links.Add("Instruments");
            Links.Add("Datasets");
            Links.Add("Observations");
        }
    }

    [Table("vStationObservations")]
    public class Observation : IntIdEntity
    {
        //public Guid ImportBatchId { get; set; } 
        public Guid StationId { get; set; }
        public Guid InstrumentId { get; set; }
        public string InstrumentCode { get; set; }
        public string InstrumentName { get; set; }
        public string InstrumentDescription { get; set; }
        public Guid SensorId { get; set; }
        public string SensorCode { get; set; }
        public string SensorName { get; set; }
        public string SensorDescription { get; set; }
        public DateTime ValueDate { get; set; }
        //public double? RawValue { get; set; }
        public double? DataValue { get; set; }
        public string TextValue { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Elevation { get; set; }
        public Guid PhenomenonId { get; set; }
        public string PhenomenonCode { get; set; }
        public string PhenomenonName { get; set; }
        public string PhenomenonDescription { get; set; }
        //public Guid PhenomenonOfferingId { get; set; }
        public Guid OfferingId { get; set; }
        public string OfferingCode { get; set; }
        public string OfferingName { get; set; }
        public string OfferingDescription { get; set; }
        //[Column("PhenomenonUOMID")]
        //public Guid PhenomenonUnitId { get; set; }
        [Column("UnitOfMeasureID")]
        public Guid UnitId { get; set; }
        [Column("UnitOfMeasureCode")]
        public string UnitCode { get; set; }
        [Column("UnitOfMeasureUnit")]
        public string UnitName { get; set; }
        [Column("UnitOfMeasureSymbol")]
        public string UnitSymbol { get; set; }
        public string Comment { get; set; }
        public Guid? CorrelationId { get; set; }
        //public Guid? StatusId { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        public string StatusDescription { get; set; }
        //public Guid? StatusReasonId { get; set; }
        public string StatusReasonCode { get; set; }
        public string StatusReasonName { get; set; }
        public string StatusReasonDescription { get; set; }

        // Navigation
        [JsonIgnore, SwaggerIgnore]
        public Station Station { get; set; }
    }

    /// <summary>
    /// Unit Entity
    /// </summary>
    [Table("UnitOfMeasure")]
    public class Unit : CodedEntity
    {
        /// <summary>
        /// Unit of the Unit 
        /// </summary>
        [Required, StringLength(100), Column("Unit")]
        public string Name { get; set; }

        /// <summary>
        /// Symbol of the Unit
        /// </summary>
        [Required, StringLength(20), Column("UnitSymbol")]
        public string Symbol { get; set; }

        // Navigation
        [JsonIgnore, SwaggerIgnore]
        public List<Phenomenon> Phenomena { get; set; }

        public Unit() : base()
        {
            EntitySetName = "Units";
            Links.Add("Phenomena");
        }
    }

    /// <summary>
    /// UserDownload entity
    /// </summary>
    public class UserDownload : NamedEntity
    {
        [SwaggerIgnore]
        public bool IsDeleted { get; set; }
        /// <summary>
        /// Title of the UserDownload
        /// </summary>
        [Required, StringLength(5000)]
        public string Title { get; set; }
        /// <summary>
        /// Description of the UserDownload
        /// </summary>
        [Required, StringLength(5000)]
        public string Description { get; set; }
        /// <summary>
        /// Description of the UserDownload as Html
        /// </summary>
        [Required, StringLength(5000)]
        public string DescriptionHtml { get; set; }
        /// <summary>
        /// Citation of the UserDownload
        /// </summary>
        [Required, StringLength(5000)]
        public string Citation { get; set; }
        /// <summary>
        /// When the query for the download was executed
        /// </summary>
        /// <summary>
        /// Citation of the UserDownload as Html
        /// </summary>
        [Required, StringLength(5000)]
        public string CitationHtml { get; set; }
        /// <summary>
        /// When the query for the download was executed
        /// </summary>
        [Required]
        public DateTime Date { get; set; }
        /// <summary>
        /// The input of the query that generated the download
        /// </summary>
        [Required, StringLength(5000), Display(Name = "Input")]
        public string Input { get; set; }
        /// <summary>
        /// Requery Url for download
        /// </summary>
        [Required, StringLength(2000), Display(Name = "Requery Url"), Url]
        public string RequeryUrl { get; set; }
        /// <summary>
        /// DigitalObjectIdentifierID of the download
        /// </summary>
        //[Required, Display(Name = "Digital Object Identifier (DOI)")]
        ////[JsonIgnore, SwaggerIgnore]
        //public int DigitalObjectIdentifierId { get; set; }
        /// <summary>
        /// Url to view the download
        /// </summary>
        [Required, StringLength(2000), Display(Name = "Download Url"), Url]
        public string DownloadUrl { get; set; }
        /// <summary>
        /// Full file name of Zip on server
        /// </summary>
        [Required, StringLength(2000), Display(Name = "Zip Full Name")]
        public string ZipFullName { get; set; }
        /// <summary>
        /// SHA256 checksum of Zip
        /// </summary>
        [Required, StringLength(64), Display(Name = "Zip SHA256 Checksum")]
        public string ZipCheckSum { get; set; }
        /// <summary>
        /// Url to Zip of the download
        /// </summary>
        [Required, StringLength(2000), Display(Name = "Zip Url"), Url]
        public string ZipUrl { get; set; }
        [JsonIgnore, SwaggerIgnore]
        public string IPAddress { get; set; }
        ///// <summary>
        ///// UserId of the UserDownload
        ///// </summary>
        //[ScaffoldColumn(false)]
        //[HiddenInput]
        public new string UserId { get; set; }
        /// <summary>
        /// UserId of user who added the UserDownload
        /// </summary>
        [Required, StringLength(128), ScaffoldColumn(false)]
        public string AddedBy { get; set; }
        /// <summary>
        /// Time the UserDownload was added
        /// </summary>
        [ScaffoldColumn(false)]
        public DateTime? AddedAt { get; set; }
        ///// <summary>
        ///// UserId of user who last updated the UserDownload
        ///// </summary>
        [Required, StringLength(128), ScaffoldColumn(false)]
        public string UpdatedBy { get; set; }
        /// <summary>
        /// Time the UserDownload was updated
        /// </summary>
        [ScaffoldColumn(false)]
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        ////[JsonIgnore, SwaggerIgnore]
        //public DigitalObjectIdentifier DigitalObjectIdentifier { get; set; }
    }

    public class UserDownloadPatch : NamedEntity
    {
        public new string UserId { get; set; }
    }

    /// <summary>
    /// UserQuery entity
    /// </summary>
    public class UserQuery : NamedEntity
    {
        /// <summary>
        /// Description of the UserQuery
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }
        /// <summary>
        /// URI of the user query
        /// </summary>
        [Required, StringLength(5000), Display(Name = "Input")]
        public string QueryInput { get; set; }
        /// <summary>
        /// UserId of UserQuery
        /// </summary>
        [ScaffoldColumn(false)]
        //[HiddenInput]
        public new string UserId { get; set; }
        /// <summary>
        /// UserId of user who added the UserQuery
        /// </summary>
        [StringLength(128), ScaffoldColumn(false)]
        public string AddedBy { get; set; }
        /// <summary>
        /// Time the UserQuery was added
        /// </summary>
        [ScaffoldColumn(false)]
        public DateTime? AddedAt { get; set; }
        /// <summary>
        /// UserId of user who last updated the UserQuery
        /// </summary>
        [StringLength(128), ScaffoldColumn(false)]
        public string UpdatedBy { get; set; }
        /// <summary>
        /// Time the UserQuery was updated
        /// </summary>
        [ScaffoldColumn(false)]
        public DateTime? UpdatedAt { get; set; }
    }

    public class UserQueryPatch : NamedEntity
    {
        public new string UserId { get; set; }
        [StringLength(5000)]
        public string Description { get; set; }
    }

    public class InventorySnapshot : GuidIdEntity
    {
        public DateTime When { get; set; }
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
        public int Downloads { get; set; }
    }

}
