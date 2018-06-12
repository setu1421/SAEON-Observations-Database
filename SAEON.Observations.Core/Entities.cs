using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Web.Mvc;
using Newtonsoft.Json; 
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SAEON.Logs;

namespace SAEON.Observations.Core.Entities
{
    /// <summary>
    /// Entity List Item
    /// Used instead of full Entity expansion
    /// </summary>
    public class EntityListItem
    {
        /// <summary>
        /// Id of Entity
        /// </summary>
        public Guid? Id { get; set; } = null;
        /// <summary>
        /// Code of Entity
        /// </summary>
        public string Code { get; set; } = null;
        /// <summary>
        /// Name of Entity
        /// </summary>
        public string Name { get; set; } = null;
    }

    /// <summary>
    /// Base for entities
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Id of the Entity
        /// </summary>
        //[Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false), HiddenInput]
        [JsonProperty(Order = -99)]
        public Guid Id { get; set; }
        public virtual EntityListItem AsEntityListItem => new EntityListItem { Id = Id };
    }

    public abstract class NamedEntity : BaseEntity
    {
        /// <summary>
        /// Name of the Entity
        /// </summary> 
        [Required, StringLength(150), JsonProperty(Order = -97)]
        public string Name { get; set; }
        [NotMapped, JsonIgnore]
        public override EntityListItem AsEntityListItem
        {
            get
            {
                var result = base.AsEntityListItem;
                result.Name = Name;
                return result;
            }
        }
    }

    public abstract class CodedEntity : NamedEntity
    {
        /// <summary>
        /// Code of the Entity
        /// </summary>
        [Required, StringLength(50), JsonProperty(Order = -98)]
        public string Code { get; set; }
        public override EntityListItem AsEntityListItem
        {
            get
            {
                var result = base.AsEntityListItem;
                result.Code = Code;
                return result;
            }
        }
    }

    /// <summary>
    /// Instrument entity
    /// </summary>
    public class Instrument : CodedEntity
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
        /// <summary>
        /// The Organisations linked to this Instrument
        /// </summary>
        [JsonIgnore] 
        public List<Organisation> Organisations { get; set; }
        [JsonProperty("Organisations")]
        public List<EntityListItem> OrganisationsList => Organisations?.Select(i => i.AsEntityListItem).ToList();

        /// <summary>
        /// Sensors linked to this Instrument
        /// </summary>
        [JsonIgnore]
        public List<Sensor> Sensors { get; set; }
        [JsonProperty("Sensors")]
        public List<EntityListItem> SensorsList => Sensors?.Select(i => i.AsEntityListItem).ToList();

        /// <summary>
        /// Stations linked to this Instrument
        /// </summary>
        [JsonIgnore]
        public List<Station> Stations { get; set; }
        [JsonProperty("Stations")]
        public List<EntityListItem> StationsList => Stations?.Select(i => i.AsEntityListItem).ToList();
    }

    /// <summary>
    /// Offering entity
    /// </summary>
    public class Offering : CodedEntity
    {
        /// Description of the Offering
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }

        // Navigation
        /// <summary>
        /// Phenomena of this Offering
        /// </summary>
        [JsonIgnore]
        public List<Phenomenon> Phenomena { get; set; }
        [JsonProperty("Phenomena")]
        public List<EntityListItem> PhenomenaList => Phenomena?.Select(i => i.AsEntityListItem).ToList();
    }

    /// <summary>
    /// Organisation entity
    /// </summary>
    public class Organisation : CodedEntity
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

        //public bool HasSites { get { return Sites?.Any() ?? false; } }
        // Navigation

        /// <summary>
        /// The Instruments linked to this Organisation
        /// </summary>
        [JsonIgnore]
        public List<Instrument> Instruments { get; set; }
        [JsonProperty("Instruments")]
        public List<EntityListItem> InstrumentsList => Instruments?.Select(i => i.AsEntityListItem).ToList();

        /// <summary>
        /// The Sites linked to this Organisation
        /// </summary>
        [JsonIgnore]
        public List<Site> Sites { get; set; }
        [JsonProperty("Sites")]
        public List<EntityListItem> SitesList => Sites?.Select(i => i.AsEntityListItem).ToList();

        /// <summary>
        /// The Stations linked to this Organisation
        /// </summary>
        [JsonIgnore]
        public List<Station> Stations { get; set; }
        [JsonProperty("Stations")]
        public List<EntityListItem> StationsList => Stations?.Select(i => i.AsEntityListItem).ToList();
    }

    /// <summary>
    /// Phenomenon entity
    /// </summary>
    public class Phenomenon : CodedEntity
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

        //public bool HasOfferings { get { return Offerings?.Any() ?? false; } }
        //public bool HasUnits { get { return Units.Any() ?? false; } }

        // Navigation
        /// <summary>
        /// Offerings of this Phenomenon 
        /// </summary>
        [JsonIgnore]
        public List<Offering> Offerings { get; set; }
        [JsonProperty("Offerings"), NotMapped]
        public List<EntityListItem> OfferingsList => Offerings?.Select(i => i.AsEntityListItem).ToList();

        /// <summary>
        /// Sensors linked to this Phenomenon 
        /// </summary>
        [JsonIgnore]
        public List<Sensor> Sensors { get; set; }
        [JsonProperty("Sensors"), NotMapped]
        public List<EntityListItem> SensorsList => Sensors?.Select(i => i.AsEntityListItem).ToList();

        /// <summary>
        /// Units of this Phenomenon 
        /// </summary>
        [JsonIgnore]
        public List<Unit> Units { get; set; }
        [JsonProperty("Units"), NotMapped]
        public List<EntityListItem> UnitsList => Units?.Select(i => i.AsEntityListItem).ToList();

    }

    /// <summary>
    /// PhenomenonOffering entity
    /// </summary>
    public class PhenomenonOffering : BaseEntity
    {
        [Required]
        public Guid PhenomenonId { get; set; }
        [Required]
        public Guid OfferingId { get; set; }

        // Navigation
        /// <summary>
        /// Phenomenon of this PhenomenonOffering
        /// </summary>
        public Phenomenon Phenomenon { get; set; }
        /// <summary>
        /// Offering of this PhenomenonOffering
        /// </summary>
        public Offering Offering { get; set; }
    }

    /// <summary>
    /// PhenomenonUnit entity
    /// </summary>
    [Table("PhenomenonUOM")]
    public class PhenomenonUnit : BaseEntity
    {
        [Required]
        public Guid PhenomenonId { get; set; }
        [Required, Column("UnitOfMeasureID")]
        public Guid UnitId { get; set; }
        // Navigation
        /// <summary>
        /// Phenomenon of this PhenomenonUnit
        /// </summary>
        public Phenomenon Phenomenon { get; set; }
        /// <summary>
        /// Unit of this PhenomenonUnit
        /// </summary>
        public Unit Unit { get; set; }
    }

    /// <summary>
    /// Programme entity
    /// </summary>
    public class Programme : CodedEntity
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

        // Navigation

        /// <summary>
        /// The Projects linked to this Programme
        /// </summary>
        [JsonIgnore]
        public List<Project> Projects { get; set; }
        [JsonProperty("Projects")]
        public List<EntityListItem> ProjectList => Projects?.Select(i => i.AsEntityListItem).Where(i => i != null).ToList();
    }

    /// <summary>
    /// Project entity
    /// </summary>
    public class Project : CodedEntity
    {
        /// <summary>
        /// The Programme of the Project
        /// </summary>
        [Required, JsonIgnore]
        public Guid ProgrammeId { get; set; }
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

        //public bool HasStations { get { return Stations?.Any() ?? false; } }
        // Navigation

        /// <summary>
        /// The Programme of the Project
        /// </summary>
        [JsonIgnore]
        public Programme Programme { get; set; }
        [JsonProperty("Programme")]
        public EntityListItem ProgrammeName => Programme?.AsEntityListItem;

        /// <summary>
        /// The Stations linked to this Project
        /// </summary>
        [JsonIgnore]
        public List<Station> Stations { get; set; }
        [JsonProperty("Stations")]
        public List<EntityListItem> StationsList => Stations.Select(i => i.AsEntityListItem).ToList();
    }

    /// <summary>
    /// Sensor entity
    /// </summary>
    public class Sensor : CodedEntity
    {
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
        /// <summary>
        /// PhenomenonId of the sensor
        /// </summary>
        public Guid PhenomenonId { get; set; }

        // Navigation
        /// <summary>
        /// Instruments linked to this Sensor
        /// </summary>
        [JsonIgnore]
        public List<Instrument> Instruments { get; set; }
        [JsonProperty("Instruments")]
        public List<EntityListItem> InstrumentsList => Instruments?.Select(i => i.AsEntityListItem).ToList();

        /// <summary>
        /// Phenomenon of the Sensor
        /// </summary>
        [JsonIgnore]
        public Phenomenon Phenomenon { get; set; }
        [JsonProperty("Phenomenon")]
        public EntityListItem PhenomenonName => Phenomenon?.AsEntityListItem;
    }

    /// <summary>
    /// Site entity
    /// </summary>
    public class Site : CodedEntity
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

        //public bool HasStations { get { return Stations?.Any() ?? false; } }
        // Navigation


        /// <summary>
        /// The Organisations linked to this Site
        /// </summary>
        [JsonIgnore]
        public List<Organisation> Organisations { get; set; }
        [JsonProperty("Organisations")]
        public List<EntityListItem> OrganisationsList => Organisations?.Select(i => i.AsEntityListItem).ToList();

        /// <summary>
        /// The Stations linked to this Site
        /// </summary>
        [JsonIgnore]
        public List<Station> Stations { get; set; }
        [JsonProperty("Stations")]
        public List<EntityListItem> StationsList => Stations?.Select(i => i.AsEntityListItem).ToList();
    }

    /// <summary>
    /// Station entity
    /// </summary>
    public class Station : CodedEntity
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
        /// Logitude of the Station
        /// </summary>
        public double? Longitude { get; set; }
        /// <summary>
        /// Elevation of the Station, positive above sea level, negative below sea level
        /// </summary>
        public double? Elevation { get; set; }

        // Navigation

        /// <summary>
        /// Instruments linked to this Station
        /// </summary>
        [JsonIgnore]
        public List<Instrument> Instruments { get; set; }
        [JsonProperty("Instruments")]
        public List<EntityListItem> InstrumentsList => Instruments?.Select(i => i.AsEntityListItem).ToList();

        /// <summary>
        /// The Organisations linked to this Station
        /// </summary>
        [JsonIgnore]
        public List<Organisation> Organisations { get; set; }
        [JsonIgnore]
        [JsonProperty("Organisations")]
        public List<EntityListItem> OrganisationsList => Organisations?.Select(i => i.AsEntityListItem).ToList();

        /// <summary>
        /// The Projects linked to this Station
        /// </summary>
        [JsonIgnore]
        public List<Project> Projects { get; set; }
        [JsonProperty("Projects")]
        public List<EntityListItem> ProjectsList => Projects?.Select(i => i.AsEntityListItem).ToList();

        /// <summary>
        /// Site of the Station
        /// </summary>
        [JsonIgnore]
        public Site Site { get; set; }
        [JsonProperty("Site")]
        public EntityListItem SiteName => Site?.AsEntityListItem;
    }

    /// <summary>
    /// Unit Entity
    /// </summary>
    [Table("UnitOfMeasure")]
    public class Unit : CodedEntity
    {
        /// <summary>
        /// Symbol of the Unit
        /// </summary>
        [Required, StringLength(20), Column("UnitSymbol")]
        public string Symbol { get; set; }

        // Navigation
        /// <summary>
        /// Phenomena of this Unit
        /// </summary>
        [JsonIgnore]
        public List<Phenomenon> Phenomena { get; set; }
        [JsonProperty("Phenomena")]
        public List<EntityListItem> PhenomenaList => Phenomena?.Select(i => i.AsEntityListItem).ToList();
    }

    /// <summary>
    /// UserDownload entity
    /// </summary>
    [Table("UserDownloads")]
    public class UserDownload : NamedEntity
    {
        /// <summary>
        /// Description of the UserDownload
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }
        /// <summary>
        /// The URI of the original query that generated the download
        /// </summary>
        [StringLength(5000)]
        public string QueryInput { get; set; }
        /// <summary>
        /// URI of the user download
        /// </summary>
        [Required, StringLength(500)]
        public string DownloadURI { get; set; }
        /// <summary>
        /// UserId of UserDownload
        /// </summary>
        [StringLength(128), ScaffoldColumn(false), HiddenInput]
        public string UserId { get; set; }
        /// <summary>
        /// UserId of user who added the UserDownload
        /// </summary>
        [StringLength(128), ScaffoldColumn(false)]
        public string AddedBy { get; set; }
        /// <summary>
        /// UserId of user who last updated the UserDownload
        /// </summary>
        [StringLength(128), ScaffoldColumn(false)]
        public string UpdatedBy { get; set; }
    }

    /// <summary>
    /// UserQuery entity
    /// </summary>
    [Table("UserQueries")]
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
        [Required, StringLength(5000)]
        public string QueryInput { get; set; }
        /// <summary>
        /// UserId of UserQuery
        /// </summary>
        [StringLength(128), ScaffoldColumn(false), HiddenInput]
        public string UserId { get; set; }
        /// <summary>
        /// UserId of user who added the UserQuery
        /// </summary>
        [StringLength(128), ScaffoldColumn(false)]
        public string AddedBy { get; set; }
        /// <summary>
        /// UserId of user who last updated the UserQuery
        /// </summary>
        [StringLength(128), ScaffoldColumn(false)]
        public string UpdatedBy { get; set; }
    }

#if !NET461
    //> Remove later once we have proper many to many in Entity Framework Core
    //[Table("Instrument_Sensor")]
    //public class InstrumentSensor
    //{
    //    public Guid InstrumentId { get; set; }
    //    public Instrument Instrument { get; set; }
    //    public Guid SensorId { get; set; }
    //    public Sensor Sensor { get; set; }
    //}

    [Table("Organisation_Instrument")]
    public class OrganisationInstrument
    {
        public Guid OrganisationId { get; set; }
        public Organisation Organisation { get; set; }
        public Guid InstrumentId { get; set; }
        public Instrument Instrument { get; set; }
    }

    [Table("Organisation_Site")]
    public class OrganisationSite
    {
        public Guid OrganisationId { get; set; }
        public Organisation Organisation { get; set; }
        public Guid SiteId { get; set; }
        public Site Site { get; set; }
    }

    [Table("Organisation_Station")]
    public class OrganisationStation
    {
        public Guid OrganisationId { get; set; }
        public Organisation Organisation { get; set; }
        public Guid StationId { get; set; }
        public Station Station { get; set; }
    }

    [Table("Project_Station")]
    public class ProjectStation 
    {
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        public Guid StationId { get; set; }
        public Station Station { get; set; }
    }

    [Table("Station_Instrument")]
    public class StationInstrument
    {
        public Guid StationId { get; set; }
        public Station Station { get; set; }
        public Guid InstrumentId { get; set; }
        public Instrument Instrument { get; set; }
    }
    //< Remove later once we have proper many to many in Entity Framework Core
#endif

    /*
    public class vApiDataBase
    {
        [Key]
        public int Id { get; set; }
        public string SiteName { get; set; }
        public Guid StationId { get; set; }
        public string StationName { get; set; }
        public Guid PhenomenonId { get; set; }
        public string PhenomenonCode { get; set; }
        public string PhenomenonName { get; set; }
        public Guid PhenomenonOfferingId { get; set; }
        public Guid OfferingId { get; set; }
        public string OfferingCode { get; set; }
        public string OfferingName { get; set; }
        public Guid UnitOfMeasureId { get; set; }
        public string UnitOfMeasureCode { get; set; }
        public string UnitOfMeasureUnit { get; set; }
        public string UnitOfMeasureSymbol { get; set; }
        public string FeatureCaption { get; set; }
        public string FeatureName { get; set; }
        public DateTime ValueDate { get; set; }
        public DateTime ValueDay { get; set; }
        public double? Value { get; set; }
    }

    [Table("vApiDataDownload")]
    public class vApiDataDownload : vApiDataBase
    {
        public Guid SiteID { get; set; }
        public string SiteCode { get; set; }
        public string SiteDescription { get; set; }
        public string SiteUrl { get; set; }
        public string StationCode { get; set; }
        public string StationDescription { get; set; }
        public string StationUrl { get; set; }
        public double? StationLatitude { get; set; }
        public double? StationLongitude { get; set; }
        public int? StationElevation { get; set; }
        public Guid InstrumentId { get; set; }
        public string InstrumentCode { get; set; }
        public string InstrumentDescription { get; set; }
        public string InstrumentUrl { get; set; }
        public string SensorCode { get; set; }
        public string SensorDescription { get; set; }
        public string SensorUrl { get; set; }
        public string PhenomenonDescription { get; set; }
        public string PhenomenonUrl { get; set; }
        public string OfferingDescription { get; set; }
        public string Comment { get; set; }
        public Guid? CorrelationId { get; set; }
    }

    [Table("vApiDataQuery")]
    public class vApiDataQuery : vApiDataBase { }

    [Table("vApiInventory")]
    public class vApiInventory : vApiDataBase
    {
        public string Status { get; set; }
    }

    public class InventoryBase
    {
        [Key]
        public string SurrogateKey { get; set; }
    }

    /// <summary>
    /// Inventory Totals
    /// </summary>
    [Table("vInventoryTotals")]
    public class InventoryTotal : InventoryBase
    {
        public string Status { get; set; }
        public int? Count { get; set; }
        public double? Minimum { get; set; }
        public double? Maximum { get; set; }
        public double? Average { get; set; }
        public double? StandardDeviation { get; set; }
        public double? Variance { get; set; }
    }

    /// <summary>
    /// Inventory Stations
    /// </summary>
    [Table("vInventoryStations")]
    public class InventoryStation : InventoryBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Status { get; set; }
        public int? Count { get; set; }
        public double? Minimum { get; set; }
        public double? Maximum { get; set; }
        public double? Average { get; set; }
        public double? StandardDeviation { get; set; }
        public double? Variance { get; set; }
    }

    /// <summary>
    /// Inventory Phenomena Offerings
    /// </summary>
    [Table("vInventoryPhenomenaOfferings")]
    public class InventoryPhenomenonOffering : InventoryBase
    {
        public string Phenomenon { get; set; }
        public string Offering { get; set; }
        public string Status { get; set; }
        public int? Count { get; set; }
        public double? Minimum { get; set; }
        public double? Maximum { get; set; }
        public double? Average { get; set; }
        public double? StandardDeviation { get; set; }
        public double? Variance { get; set; }
    }

    /// <summary>
    /// Inventory Instruments
    /// </summary>
    [Table("vInventoryInstruments")]
    public class InventoryInstrument : InventoryBase
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public int? Count { get; set; }
        public double? Minimum { get; set; }
        public double? Maximum { get; set; }
        public double? Average { get; set; }
        public double? StandardDeviation { get; set; }
        public double? Variance { get; set; }
    }

    /// <summary>
    /// Inventory Years
    /// </summary>
    [Table("vInventoryYears")]
    public class InventoryYear : InventoryBase
    {
        public int Year { get; set; }
        public string Status { get; set; }
        public int? Count { get; set; }
        public double? Minimum { get; set; }
        public double? Maximum { get; set; }
        public double? Average { get; set; }
        public double? StandardDeviation { get; set; }
        public double? Variance { get; set; }
    }

    /// <summary>
    /// Inventory Organisations
    /// </summary>
    [Table("vInventoryOrganisations")]
    public class InventoryOrganisation : InventoryBase
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public int? Count { get; set; }
        public double? Minimum { get; set; }
        public double? Maximum { get; set; }
        public double? Average { get; set; }
        public double? StandardDeviation { get; set; }
        public double? Variance { get; set; }
    }

    [Table("vApiSpacialCoverage")]
    public class vApiSpacialCoverage : vApiDataBase
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Elevation { get; set; }
        public string Status { get; set; }
    }

    [Table("vApiTemporalCoverage")]
    public class vApiTemporalCoverage : vApiDataBase
    {
        public string Status { get; set; }
    }

    [Table("vSensorThingsDatastreams")]
    public class vSensorThingsDatastream
    {
        [Key]
        public Guid Id { get; set; }
        public Guid SensorId { get; set; }
        public string Sensor { get; set; }
        public string Phenomenon { get; set; }
        public string Offering { get; set; }
        public string Unit { get; set; }
        public string Symbol { get; set; }
        public string Url { get; set; }
    }
    */

    public class ObservationsDbContext : DbContext
    {
        public ObservationsDbContext() : base("Observations")
        {
            Configuration.ProxyCreationEnabled = false; 
            Configuration.LazyLoadingEnabled = false;
            Database.Log = s => Logging.Verbose(s);
            Database.CommandTimeout = 30 * 60;
        }
        public DbSet<Instrument> Instruments { get; set; }
        //public DbSet<InventoryTotal> InventoryTotals { get; set; }
        //public DbSet<InventoryStation> InventoryStations { get; set; }
        //public DbSet<InventoryInstrument> InventoryInstruments { get; set; }
        //public DbSet<InventoryPhenomenonOffering> InventoryPhenomenaOfferings { get; set; }
        //public DbSet<InventoryYear> InventoryYears { get; set; }
        //public DbSet<InventoryOrganisation> InventoryOrganisations { get; set; }
        public DbSet<Offering> Offerings { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<Phenomenon> Phenomena { get; set; }
        public DbSet<Programme> Programmes { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<UserDownload> UserDownloads { get; set; }
        public DbSet<UserQuery> UserQueries { get; set; }

        //public DbSet<vApiDataDownload> vApiDataDownloads { get; set; }
        //public DbSet<vApiDataQuery> vApiDataQueries { get; set; }
        //public DbSet<vApiInventory> vApiInventory { get; set; }
        //public DbSet<vApiSpacialCoverage> vApiSpacialCoverages { get; set; }
        //public DbSet<vApiTemporalCoverage> vApiTemporalCoverages { get; set; }
        //public DbSet<vSensorThingsDatastream> vSensorThingsDatastreams { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<Organisation>()
                .HasMany<Site>(l => l.Sites)
                .WithMany(r => r.Organisations)
                .Map(cs =>
                {
                    cs.MapLeftKey("OrganisationID");
                    cs.MapRightKey("SiteID");
                    cs.ToTable("Organisation_Site");
                });
            modelBuilder.Entity<Project>()
                .HasMany<Station>(l => l.Stations)
                .WithMany(r => r.Projects)
                .Map(cs =>
                {
                    cs.MapLeftKey("ProjectID");
                    cs.MapRightKey("StationID");
                    cs.ToTable("Project_Station");
                });
            modelBuilder.Entity<Station>()
                .HasMany<Instrument>(l => l.Instruments)
                .WithMany(r => r.Stations)
                .Map(cs =>
                {
                    cs.MapLeftKey("StationID");
                    cs.MapRightKey("InstrumentID"); 
                    cs.ToTable("Station_Instrument");
                });

            modelBuilder.Entity<Instrument>()
                .HasMany<Sensor>(l => l.Sensors)
                .WithMany(r => r.Instruments)
                .Map(cs =>
                {
                    cs.MapLeftKey("InstrumentID");
                    cs.MapRightKey("SensorID");
                    cs.ToTable("Instrument_Sensor");
                });  
            modelBuilder.Entity<Phenomenon>() 
                .HasMany<Offering>(l => l.Offerings)
                .WithMany(r => r.Phenomena)
                .Map(cs =>
                {
                    cs.MapLeftKey("PhenomenonID");
                    cs.MapRightKey("OfferingID");
                    cs.ToTable("PhenomenonOffering");
                });
            modelBuilder.Entity<Unit>().Property(p => p.Name).HasColumnName("Unit");
            modelBuilder.Entity<Phenomenon>()
                .HasMany<Unit>(l => l.Units)
                .WithMany(r => r.Phenomena)
                .Map(cs =>
                {
                    cs.MapLeftKey("PhenomenonID");
                    cs.MapRightKey("UnitOfMeasureID");
                    cs.ToTable("PhenomenonUOM");
                });
        }
    }

}
