using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SAEON.Logs;
using SAEON.Observations.Auth;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SAEON.Observations.WebAPI
{
    /// <summary>
    /// Data Schema entity
    /// </summary>
    [Table("DataSchema")]
    public class DataSchema : NamedEntity
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

        // Navigation
        public DataSourceType DataSourceType { get; set; }
    }

    /// <summary>
    /// Data Source entity
    /// </summary>
    [Table("DataSource")]
    public class DataSource : NamedEntity
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
        public DataSchema DataSchema { get; set; }
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
        public List<DataSchema> DataSchemas { get; set; }
    }

    public enum DOIType { ObservationsDb, Organisation, Programme, Project, Site, Station, Dataset, Periodic, AdHoc }

    /// <summary>
    /// DigitalObjectIdentifiers entity
    /// </summary>
    public class DigitalObjectIdentifier : IntIdEntity
    {
        [HiddenInput]
        public int? ParentId { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? AlternateId { get; set; }
        [Required]
        [DisplayName("DOI Type")]
        public DOIType DOIType { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string DOI { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Display(Name = "DOI Url")]
        public string DOIUrl { get; set; }
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
        [Required, StringLength(250)]
        public string MetadataUrl { get; set; }
        /// <summary>
        /// Object Store Url of the DigitalObjectIdentifier
        /// </summary>
        [StringLength(250)]
        public string ObjectStoreUrl { get; set; }
        /// <summary>
        /// Query Url of the DigitalObjectIdentifier
        /// </summary>
        [StringLength(250)]
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
        /// <summary> 
        /// UserId of user who added the DigitalObjectIdentifier   
        /// </summary>
        [StringLength(128), ScaffoldColumn(false)]
        public string AddedBy { get; set; }
        /// <summary>
        /// UserId of user who last updated the DigitalObjectIdentifier 
        /// </summary>
        [StringLength(128), ScaffoldColumn(false)]
        public string UpdatedBy { get; set; }
        [Timestamp, ConcurrencyCheck, ScaffoldColumn(false)]
        [HiddenInput]
        public byte[] RowVersion { get; set; }

        // Navigation

        public DigitalObjectIdentifier Parent { get; set; }
        public List<DigitalObjectIdentifier> Children { get; set; }
        public List<Organisation> Organisations { get; set; }
        public List<Programme> Programmes { get; set; }
        public List<Project> Projects { get; set; }
        public List<Site> Sites { get; set; }
        public List<Station> Stations { get; set; }
        public List<ImportBatchSummary> ImportBatchSummaries { get; set; }
        //public List<UserDownload> UserDownloads { get; set; }
    }

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

        public List<ImportBatchSummary> ImportBatchSummaries { get; set; }
    }

    public class ImportBatchSummary : GuidIdEntity
    {
        public Guid ImportBatchId { get; set; }
        public Guid SensorId { get; set; }
        public Guid InstrumentId { get; set; }
        public Guid StationId { get; set; }
        public Guid SiteId { get; set; }
        public Guid PhenomenonOfferingId { get; set; }
        [Column("PhenomenonUOMID")]
        public Guid PhenomenonUnitId { get; set; }
        public int? DigitalObjectIdentifierId { get; set; }
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

        // Navigation
        public ImportBatch ImportBatch { get; set; }
        //public Sensor Sensor { get; set; }
        //public Instrument Instrument { get; set; }
        //public Station Station { get; set; }
        //public Site Site { get; set; }
        //public PhenomenonOffering PhenomenonOffering { get; set; }
        //public PhenomenonUnit PhenomenonUnit { get; set; }
        public DigitalObjectIdentifier DigitalObjectIdentifier { get; set; }
    }

    /// <summary>
    /// Instrument entity
    /// </summary>
    [Table("Instrument")]
    public class Instrument : NamedEntity
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
    public class Offering : NamedEntity
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
    public class Organisation : NamedEntity
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
        [JsonIgnore, SwaggerIgnore]
        public int? DigitalObjectIdentifierID { get; set; }

        // Navigation

        [JsonIgnore, SwaggerIgnore]
        public DigitalObjectIdentifier DigitalObjectIdentifier { get; set; }
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
    /// OrganisationRole entity
    /// </summary>
    [Table("OrganisationRole")]
    public class OrganisationRole : NamedEntity
    {
        // Navigation
        //public List<Organisation> Organisations { get; set; }
    }

    /// <summary>
    /// Phenomenon entity
    /// </summary>
    [Table("Phenomenon")]
    public class Phenomenon : NamedEntity
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
    public class Programme : NamedEntity
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
        [JsonIgnore, SwaggerIgnore]
        public int? DigitalObjectIdentifierID { get; set; }

        // Navigation

        [JsonIgnore, SwaggerIgnore]
        public DigitalObjectIdentifier DigitalObjectIdentifier { get; set; }
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
    public class Project : NamedEntity
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
        [JsonIgnore, SwaggerIgnore]
        public int? DigitalObjectIdentifierID { get; set; }

        // Navigation

        [JsonIgnore, SwaggerIgnore]
        public DigitalObjectIdentifier DigitalObjectIdentifier { get; set; }
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
    public class Sensor : NamedEntity
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
        [Required]
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

        [JsonIgnore, SwaggerIgnore]
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
    public class Site : NamedEntity
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
        [JsonIgnore, SwaggerIgnore]
        public int? DigitalObjectIdentifierID { get; set; }

        // Navigation

        [JsonIgnore, SwaggerIgnore]
        public DigitalObjectIdentifier DigitalObjectIdentifier { get; set; }
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
    public class Station : NamedEntity
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
        [JsonIgnore, SwaggerIgnore]
        public int? DigitalObjectIdentifierID { get; set; }

        // Navigation

        [JsonIgnore, SwaggerIgnore]
        public DigitalObjectIdentifier DigitalObjectIdentifier { get; set; }
        [JsonIgnore, SwaggerIgnore]
        public Site Site { get; set; }
        [JsonIgnore, SwaggerIgnore]
        public List<Organisation> Organisations { get; set; }
        [JsonIgnore, SwaggerIgnore]
        public List<Project> Projects { get; set; }
        [JsonIgnore, SwaggerIgnore]
        public List<Instrument> Instruments { get; set; }

        public Station() : base()
        {
            EntitySetName = "Stations";
            Links.Add("Site");
            Links.Add("Organisations");
            Links.Add("Projects");
            Links.Add("Instruments");
            //Links.Add("Datasets"); @@
            //Links.Add("Observations"); @@
        }
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
        /// <summary>
        /// Description of the UserDownload
        /// DataCite Abstracts, should include citation format
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }
        /// <summary>
        /// Title of the DownLoad
        /// DataCite Titles
        /// </summary>
        [Required, StringLength(5000)]
        public string Title { get; set; }
        /// <summary>
        /// How this DownLoad should be cited
        /// </summary>
        [Required, StringLength(5000)]
        public string Citation { get; set; }
        /// <summary>
        /// Keywords of the Download, semi-colon separated
        /// </summary>
        [Required, StringLength(1000)]
        public string Keywords { get; set; }
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
        [Required, StringLength(2000), Display(Name = "Requery Url")]
        public string RequeryUrl { get; set; }
        ///// <summary>
        ///// DigitalObjectIdentifierID of the download
        ///// </summary>
        //[Required, Display(Name = "Digital Object Identifier (DOI)")]
        //public int DigitalObjectIdentifierId { get; set; }
        /// <summary>
        /// Json sent to metadata service
        /// </summary>
        [Required, Display(Name = "Metadata Json")]
        public string MetadataJson { get; set; }
        /// <summary>
        /// Metadata Url for download
        /// </summary>
        [Required, StringLength(2000), Display(Name = "Metadata Url")]
        public string MetadataUrl { get; set; }
        /// <summary>
        /// ODP Id for the download
        /// </summary>
        public Guid OpenDataPlatformId { get; set; }
        /// <summary>
        /// Url to view the download
        /// </summary>
        [Required, StringLength(2000), Display(Name = "Download Url")]
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
        [Required, StringLength(2000), Display(Name = "Zip Url")]
        public string ZipUrl { get; set; }
        /// <summary>
        /// Places of the DownLoad
        /// Lookup on GeoNames in format Name:Country:Lat:Lon, semi-colon separated
        /// </summary>
        [Required, StringLength(5000)]
        public string Places { get; set; }
        /// <summary>
        /// North-most Latitude of the download
        /// </summary>
        public double? LatitudeNorth { get; set; } // +N to -S
        /// <summary>
        /// South-most Latitude of the download
        /// </summary>
        public double? LatitudeSouth { get; set; } // +N to -S
        /// <summary>
        /// West-morthmost Longitude of the download
        /// </summary>
        public double? LongitudeWest { get; set; } // -W to +E
        /// <summary>
        /// East-morthmost Longitude of the download
        /// </summary>
        public double? LongitudeEast { get; set; } // -W to +E
        /// <summary>
        /// Minimum elevation of the download
        /// </summary>
        public double? ElevationMinimum { get; set; }
        /// <summary>
        /// Maximum elevation of the download
        /// </summary>
        public double? ElevationMaximum { get; set; }
        /// <summary>
        /// Start date of the download
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }
        /// <summary>
        /// End date of the download
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }
        /// <summary>
        /// UserId of the UserDownload
        /// </summary>
        [ScaffoldColumn(false)]
        [HiddenInput]
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
        //public DigitalObjectIdentifier DigitalObjectIdentifier { get; set; }
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
        [HiddenInput]
        public new string UserId { get; set; }
        /// <summary>
        /// UserId of user who added the UserQuery
        /// </summary>
        [StringLength(128), ScaffoldColumn(false)]
        public string AddedBy { get; set; }
        /// <summary>
        /// Time the UserDownload was added
        /// </summary>
        [ScaffoldColumn(false)]
        public DateTime? AddedAt { get; set; }
        /// <summary>
        /// UserId of user who last updated the UserQuery
        /// </summary>
        [StringLength(128), ScaffoldColumn(false)]
        public string UpdatedBy { get; set; }
        /// <summary>
        /// Time the UserDownload was updated
        /// </summary>
        [ScaffoldColumn(false)]
        public DateTime? UpdatedAt { get; set; }
    }

    //[Table("vImportBatchSummary")]
    public class VImportBatchSummary : GuidIdEntity
    {
        public Guid ImportBatchId { get; set; }

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
        public Guid StationId { get; set; }
        public string StationCode { get; set; }
        public string StationName { get; set; }
        public string StationDescription { get; set; }
        public string StationUrl { get; set; }
        public Guid InstrumentId { get; set; }
        public string InstrumentCode { get; set; }
        public string InstrumentName { get; set; }
        public string InstrumentDescription { get; set; }
        public string InstrumentUrl { get; set; }
        public Guid SensorId { get; set; }
        public string SensorCode { get; set; }
        public string SensorName { get; set; }
        public string SensorDescription { get; set; }
        public string SensorUrl { get; set; }
        public Guid PhenomenonId { get; set; }
        public string PhenomenonCode { get; set; }
        public string PhenomenonName { get; set; }
        public string PhenomenonDescription { get; set; }
        public string PhenomenonUrl { get; set; }
        public Guid PhenomenonOfferingId { get; set; }
        public Guid OfferingId { get; set; }
        public string OfferingCode { get; set; }
        public string OfferingName { get; set; }
        public string OfferingDescription { get; set; }
        [Column("PhenomenonUOMID")]
        public Guid PhenomenonUnitId { get; set; }
        [Column("UnitOfMeasureId")]
        public Guid UnitId { get; set; }
        [Column("UnitOfMeasureCode")]
        public string UnitCode { get; set; }
        [Column("UnitOfMeasureUnit")]
        public string UnitName { get; set; }
        [Column("UnitOfMeasureSymbol")]
        public string UnitSymbol { get; set; }
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

        // Navigation
        //public ImportBatch ImportBatch { get; set; }
    }

    //[Table("vInventoryDatasets")]
    //[Keyless]
    public class InventoryDataset : BaseEntity
    {
        // Remove once OData allows Keyless views
        [Key]
        public long Id { get; set; }
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
        public Guid StationId { get; set; }
        public string StationCode { get; set; }
        public string StationName { get; set; }
        public string PhenomenonCode { get; set; }
        public string PhenomenonName { get; set; }
        public Guid PhenomenonOfferingId { get; set; }
        public string OfferingCode { get; set; }
        public string OfferingName { get; set; }
        [Column("PhenomenonUOMID")]
        public Guid PhenomenonUnitId { get; set; }
        [Column("UnitOfMeasureCode")]
        public string UnitCode { get; set; }
        [Column("UnitOfMeasureUnit")]
        public string UnitName { get; set; }
        public int Count { get; set; }
        public double? LatitudeNorth { get; set; } // +N to -S
        public double? LatitudeSouth { get; set; } // +N to -S
        public double? LongitudeWest { get; set; } // -W to +E
        public double? LongitudeEast { get; set; } // -W to +E
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    //[Table("vInventorySensors")]
    //[Keyless]
    public class InventorySensor : BaseEntity
    {
        // Remove once OData allows Keyless views
        [Key]
        public long Id { get; set; }
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
        public Guid StationId { get; set; }
        public string StationCode { get; set; }
        public string StationName { get; set; }
        public Guid InstrumentId { get; set; }
        public string InstrumentCode { get; set; }
        public string InstrumentName { get; set; }
        public Guid SensorId { get; set; }
        public string SensorCode { get; set; }
        public string SensorName { get; set; }
        public string PhenomenonCode { get; set; }
        public string PhenomenonName { get; set; }
        public Guid PhenomenonOfferingId { get; set; }
        public string OfferingCode { get; set; }
        public string OfferingName { get; set; }
        [Column("PhenomenonUOMID")]
        public Guid PhenomenonUnitId { get; set; }
        [Column("UnitOfMeasureCode")]
        public string UnitCode { get; set; }
        [Column("UnitOfMeasureUnit")]
        public string UnitName { get; set; }
        public int Count { get; set; }
        public double? LatitudeNorth { get; set; } // +N to -S
        public double? LatitudeSouth { get; set; } // +N to -S
        public double? LongitudeWest { get; set; } // -W to +E
        public double? LongitudeEast { get; set; } // -W to +E
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    //[Keyless]
    public class VLocation : BaseEntity
    {
        public Guid OrganisationID { get; set; }
        public string OrganisationName { get; set; }
        public string OrganisationUrl { get; set; }
        public Guid ProgrammeID { get; set; }
        public string ProgrammeName { get; set; }
        public string ProgrammeUrl { get; set; }
        public Guid ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string ProjectUrl { get; set; }
        public Guid SiteID { get; set; }
        public string SiteName { get; set; }
        public string SiteUrl { get; set; }
        public Guid StationID { get; set; }
        public string StationName { get; set; }
        public string StationUrl { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Elevation { get; set; }
    }

    //[Keyless]
    public class VFeature : BaseEntity
    {
        public Guid PhenomenonID { get; set; }
        public string PhenomenonName { get; set; }
        public string PhenomenonUrl { get; set; }
        public Guid PhenomenonOfferingID { get; set; }
        public Guid OfferingID { get; set; }
        public string OfferingName { get; set; }
        [Column("PhenomenonUOMID")]
        public Guid PhenomenonUnitID { get; set; }
        [Column("UnitOfMeasureID")]
        public Guid UnitID { get; set; }
        [Column("UnitOfMeasureUnit")]
        public string UnitName { get; set; }
    }

    //[Table("vObservationExpansion")]
    public class VObservationExpansion : IntIdEntity
    {
        public Guid ImportBatchId { get; set; }
        public Guid SiteId { get; set; }
        public string SiteCode { get; set; }
        public string SiteName { get; set; }
        public string SiteDescription { get; set; }
        public string SiteUrl { get; set; }
        public Guid StationId { get; set; }
        public string StationCode { get; set; }
        public string StationName { get; set; }
        public string StationDescription { get; set; }
        public string StationUrl { get; set; }
        public Guid InstrumentId { get; set; }
        public string InstrumentCode { get; set; }
        public string InstrumentName { get; set; }
        public string InstrumentDescription { get; set; }
        public string InstrumentUrl { get; set; }
        public Guid SensorId { get; set; }
        public string SensorCode { get; set; }
        public string SensorName { get; set; }
        public string SensorDescription { get; set; }
        public string SensorUrl { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Elevation { get; set; }
        public Guid PhenomenonId { get; set; }
        public string PhenomenonCode { get; set; }
        public string PhenomenonName { get; set; }
        public string PhenomenonDescription { get; set; }
        public Guid PhenomenonOfferingId { get; set; }
        public Guid OfferingId { get; set; }
        public string OfferingCode { get; set; }
        public string OfferingName { get; set; }
        public string OfferingDescription { get; set; }
        [Column("PhenomenonUOMID")]
        public Guid PhenomenonUnitId { get; set; }
        [Column("UnitOfMeasureID")]
        public Guid UnitId { get; set; }
        [Column("UnitOfMeasureCode")]
        public string UnitCode { get; set; }
        [Column("UnitOfMeasureUnit")]
        public string UnitName { get; set; }
        [Column("UnitOfMeasureSymbol")]
        public string UnitSymbol { get; set; }
        public DateTime ValueDate { get; set; }
        public DateTime ValueDay { get; set; }
        public int ValueYear { get; set; }
        public int ValueDecade { get; set; }
        public double? RawValue { get; set; }
        public double? DataValue { get; set; }
        public string TextValue { get; set; }
        public string Comment { get; set; }
        public Guid? CorrelationId { get; set; }
        public Guid? StatusId { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        public string StatusDescription { get; set; }
        public Guid? StatusReasonId { get; set; }
        public string StatusReasonCode { get; set; }
        public string StatusReasonName { get; set; }
        public string StatusReasonDescription { get; set; }
    }

    //[Table("vStationDatasets")]
    //[Keyless]
    public class Dataset : BaseEntity
    {
        // Remove once OData allows Keyless views
        [Key]
        public long Id { get; set; }
        public Guid StationId { get; set; }
        public string StationCode { get; set; }
        public string StationName { get; set; }
        public string StationDescription { get; set; }
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
        public int? Count { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? LatitudeNorth { get; set; }
        public double? LatitudeSouth { get; set; }
        public double? LongitudeWest { get; set; }
        public double? LongitudeEast { get; set; }
        public double? ElevationMinimum { get; set; }
        public double? ElevationMaximum { get; set; }

        // Navigation
        [JsonIgnore, SwaggerIgnore]
        public Station Station { get; set; }
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

    #region SensorThingsAPI

    [Table("vSensorThingsAPIDatastreams")]
    public class SensorThingsDatastream : GuidIdEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid InstrumentId { get; set; }
        public string InstrumentCode { get; set; }
        public string InstrumentName { get; set; }
        public Guid PhenomenonId { get; set; }
        public string PhenomenonName { get; set; }
        public string PhenomenonDescription { get; set; }
        public string PhenomenonUrl { get; set; }
        public Guid PhenomenonOfferingId { get; set; }
        public Guid OfferingId { get; set; }
        public string OfferingName { get; set; }
        public string OfferingDescription { get; set; }
        public Guid PhenomenonUnitId { get; set; }
        public Guid UnitOfMeasureId { get; set; }
        public string UnitOfMeasureUnit { get; set; }
        public string UnitOfMeasureSymbol { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? LatitudeNorth { get; set; }
        public double? LatitudeSouth { get; set; }
        public double? LongitudeWest { get; set; }
        public double? LongitudeEast { get; set; }
    }

    [Table("vSensorThingsAPIFeaturesOfInterest")]
    public class SensorThingsFeatureOfInterest : GuidIdEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Elevation { get; set; }
    }

    [Table("vSensorThingsAPIHistoricalLocations")]
    public class SensorThingsHistoricalLocation : GuidIdEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Elevation { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    [Table("vSensorThingsAPILocations")]
    public class SensorThingsLocation : GuidIdEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Elevation { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    [Table("vSensorThingsAPIObservations")]
    public class SensorThingsObservation : IntIdEntity
    {
        public Guid SensorId { get; set; }
        public Guid PhenomenonOfferingID { get; set; }
        public Guid PhenomenonUnitId { get; set; }
        [Column("ValueDate")]
        public DateTime Date { get; set; }
        [Column("DataValue")]
        public double? Value { get; set; } = null;
    }

    [Table("vSensorThingsAPIObservedProperties")]
    public class SensorThingsObservedProperty : GuidIdEntity
    {
        public string PhenomenonCode { get; set; }
        public string PhenomenonName { get; set; }
        public string PhenomenonDescription { get; set; }
        public string PhenomenonUrl { get; set; }
        public string OfferingCode { get; set; }
        public string OfferingName { get; set; }
        public string OfferingDescription { get; set; }
    }

    [Table("vSensorThingsAPISensors")]
    public class SensorThingsSensor : GuidIdEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public Guid PhenomenonOfferingId { get; set; }
    }

    [Table("vSensorThingsAPIThings")]
    public class SensorThingsThing : GuidIdEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Kind { get; set; }
        public string Url { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
    #endregion

    #region ManyToManyTables

    public class InstrumentSensor
    {
        public Guid InstrumentId { get; set; }
        public Guid SensorId { get; set; }

        // Navigation

        public Instrument Instrument { get; set; }
        public Sensor Sensor { get; set; }
    }

    public class OrganisationInstrument
    {
        public Guid OrganisationId { get; set; }
        public Guid InstrumentId { get; set; }

        // Navigation
        public Organisation Organisation { get; set; }
        public Instrument Instrument { get; set; }
    }

    public class OrganisationSite
    {
        public Guid OrganisationId { get; set; }
        public Guid SiteId { get; set; }

        // Navigation
        public Organisation Organisation { get; set; }
        public Site Site { get; set; }
    }

    public class OrganisationStation
    {
        public Guid OrganisationId { get; set; }
        public Guid StationId { get; set; }

        // Navigation
        public Organisation Organisation { get; set; }
        public Station Station { get; set; }
    }

    public class PhenomenonOffering
    {
        public Guid PhenomenonId { get; set; }
        public Guid OfferingId { get; set; }

        // Navigation

        public Phenomenon Phenomenon { get; set; }
        public Offering Offering { get; set; }
    }

    public class PhenomenonUnit
    {
        public Guid PhenomenonId { get; set; }
        [Column("UnitOfMeasureID")]
        public Guid UnitId { get; set; }

        // Navigation

        public Phenomenon Phenomenon { get; set; }
        public Unit Unit { get; set; }
    }

    public class ProjectStation
    {
        public Guid ProjectId { get; set; }
        public Guid StationId { get; set; }

        // Navigation
        public Project Project { get; set; }
        public Station Station { get; set; }
    }

    public class StationInstrument
    {
        public Guid StationId { get; set; }
        public Guid InstrumentId { get; set; }

        // Navigation
        public Station Station { get; set; }
        public Instrument Instrument { get; set; }
    }
    #endregion

    public class ObservationsDbContext : DbContext
    {
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContectAccessor;

        public ObservationsDbContext(DbContextOptions<ObservationsDbContext> options, IConfiguration config, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _config = config;
            _httpContectAccessor = httpContextAccessor;
            Database.SetCommandTimeout(30 * 60);
        }

        public DbSet<DataSchema> DataSchemas { get; set; }
        public DbSet<DataSource> DataSources { get; set; }
        public DbSet<DataSourceType> DataSourceTypes { get; set; }
        public DbSet<DigitalObjectIdentifier> DigitalObjectIdentifiers { get; set; }
        public DbSet<ImportBatch> ImportBatch { get; set; }
        public DbSet<ImportBatchSummary> ImportBatchSummary { get; set; }
        public DbSet<Instrument> Instruments { get; set; }
        public DbSet<Offering> Offerings { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<OrganisationRole> OrganisationRoles { get; set; }
        public DbSet<Phenomenon> Phenomena { get; set; }
        public DbSet<Programme> Programmes { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Dataset> Datasets { get; set; }
        public DbSet<Observation> Observations { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<UserDownload> UserDownloads { get; set; }
        public DbSet<UserQuery> UserQueries { get; set; }

        // Views
        public DbSet<VLocation> VLocations { get; set; }
        public DbSet<VFeature> VFeatures { get; set; }
        public DbSet<VImportBatchSummary> VImportBatchSummary { get; set; }
        public DbSet<InventoryDataset> InventoryDatasets { get; set; }
        public DbSet<InventorySensor> InventorySensors { get; set; }
        public DbSet<VObservationExpansion> VObservationExpansions { get; set; }

        // SensorThings
        public DbSet<SensorThingsDatastream> SensorThingsDatastreams { get; set; }
        public DbSet<SensorThingsFeatureOfInterest> SensorThingsFeaturesOfInterest { get; set; }
        public DbSet<SensorThingsHistoricalLocation> SensorThingsHistoricalLocations { get; set; }
        public DbSet<SensorThingsLocation> SensorThingsLocations { get; set; }
        public DbSet<SensorThingsObservation> SensorThingsObservations { get; set; }
        public DbSet<SensorThingsObservedProperty> SensorThingsObservedProperies { get; set; }
        public DbSet<SensorThingsSensor> SensorThingsSensors { get; set; }
        public DbSet<SensorThingsThing> SensorThingsThings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                var tenant = TenantAuthenticationHandler.GetTenantFromHeaders(_httpContectAccessor.HttpContext.Request, _config);
                var connectionString = _config.GetConnectionString(tenant);
                optionsBuilder.UseSqlServer(connectionString, options =>
                {
                    options.EnableRetryOnFailure();
                });
                //SAEONLogs.Debug("Tenant: {Tenant} ConnectionString: {ConnectionString}", tenant, connectionString);
                base.OnConfiguring(optionsBuilder);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<VFeature>().HasNoKey().ToView("vFeatures");
            modelBuilder.Entity<VLocation>().HasNoKey().ToView("vLocations");
            modelBuilder.Entity<InventoryDataset>().HasNoKey().ToView("vInventoryDatasets");
            modelBuilder.Entity<InventorySensor>().HasNoKey().ToView("vInventorySensors");
            modelBuilder.Entity<Dataset>().HasNoKey().ToView("vStationDatasets");
            modelBuilder.Entity<VObservationExpansion>().ToView("vObservationExpansion");
            modelBuilder.Entity<DigitalObjectIdentifier>().Property("DOIType").HasConversion<byte>();
            modelBuilder.Entity<DigitalObjectIdentifier>().HasOne(i => i.Parent).WithMany(i => i.Children).HasForeignKey(i => i.ParentId);
            // Many to Many
            modelBuilder.Entity<Organisation>()
                .HasMany(i => i.Instruments)
                .WithMany(i => i.Organisations)
                .UsingEntity<OrganisationInstrument>(
                    oi => oi.HasOne<Instrument>().WithMany().HasForeignKey(i => i.InstrumentId),
                    oi => oi.HasOne<Organisation>().WithMany().HasForeignKey(i => i.OrganisationId))
                .ToTable("Organisation_Instrument")
                .HasKey(i => new { i.OrganisationId, i.InstrumentId });
            modelBuilder.Entity<Organisation>()
                .HasMany(i => i.Sites)
                .WithMany(i => i.Organisations)
                .UsingEntity<OrganisationSite>(
                    os => os.HasOne<Site>().WithMany().HasForeignKey(i => i.SiteId),
                    os => os.HasOne<Organisation>().WithMany().HasForeignKey(i => i.OrganisationId))
                .ToTable("Organisation_Site")
                .HasKey(i => new { i.OrganisationId, i.SiteId });
            modelBuilder.Entity<Organisation>()
                .HasMany(i => i.Stations)
                .WithMany(i => i.Organisations)
                .UsingEntity<OrganisationStation>(
                    os => os.HasOne<Station>().WithMany().HasForeignKey(i => i.StationId),
                    os => os.HasOne<Organisation>().WithMany().HasForeignKey(i => i.OrganisationId))
                .ToTable("Organisation_Station")
                .HasKey(i => new { i.OrganisationId, i.StationId });
            modelBuilder.Entity<Project>()
                .HasMany(i => i.Stations)
                .WithMany(i => i.Projects)
                .UsingEntity<ProjectStation>(
                    ps => ps.HasOne<Station>().WithMany().HasForeignKey(i => i.StationId),
                    ps => ps.HasOne<Project>().WithMany().HasForeignKey(i => i.ProjectId))
                .ToTable("Project_Station")
                .HasKey(i => new { i.ProjectId, i.StationId });
            modelBuilder.Entity<Station>()
                .HasMany(i => i.Instruments)
                .WithMany(i => i.Stations)
                .UsingEntity<StationInstrument>(
                    si => si.HasOne<Instrument>().WithMany().HasForeignKey(i => i.InstrumentId),
                    si => si.HasOne<Station>().WithMany().HasForeignKey(i => i.StationId))
                .ToTable("Station_Instrument")
                .HasKey(i => new { i.StationId, i.InstrumentId });
            modelBuilder.Entity<Instrument>()
                .HasMany(i => i.Sensors)
                .WithMany(i => i.Instruments)
                .UsingEntity<InstrumentSensor>(
                    si => si.HasOne<Sensor>().WithMany().HasForeignKey(i => i.SensorId),
                    si => si.HasOne<Instrument>().WithMany().HasForeignKey(i => i.InstrumentId))
                .ToTable("Instrument_Sensor")
                .HasKey(i => new { i.InstrumentId, i.SensorId });
            modelBuilder.Entity<Phenomenon>()
                .HasMany(i => i.Offerings)
                .WithMany(i => i.Phenomena)
                .UsingEntity<PhenomenonOffering>(
                    i => i.HasOne<Offering>().WithMany().HasForeignKey(i => i.OfferingId),
                    i => i.HasOne<Phenomenon>().WithMany().HasForeignKey(i => i.PhenomenonId))
                .ToTable("PhenomenonOffering")
                .HasKey(i => new { i.PhenomenonId, i.OfferingId });
            modelBuilder.Entity<Phenomenon>()
                .HasMany(i => i.Units)
                .WithMany(i => i.Phenomena)
                .UsingEntity<PhenomenonUnit>(
                    i => i.HasOne<Unit>().WithMany().HasForeignKey(i => i.UnitId),
                    i => i.HasOne<Phenomenon>().WithMany().HasForeignKey(i => i.PhenomenonId))
                .ToTable("PhenomenonUOM")
                .HasKey(i => new { i.PhenomenonId, i.UnitId });
        }
    }
}
