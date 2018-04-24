#if NET461
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Web.Mvc;
#else
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
#endif
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SAEON.Observations.Core.Entities
{
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
        public Guid Id { get; set; }
        /// Name of the Entity
        /// </summary>
        [Required, StringLength(150)]
        public string Name { get; set; }

        [NotMapped, JsonIgnore]
        public NamedItem AsNamedItem { get { return new NamedItem { Id = Id, Name = Name }; } }
    }

    public class NamedItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

    }

    /// <summary>
    /// Instrument entity
    /// </summary>
    [Table("Instrument")]
    public class Instrument : BaseEntity
    {
        /// <summary>
        /// Code of the Instrument
        /// </summary>
        [Required, StringLength(50)]
        public string Code { get; set; }
        /// <summary>
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
        /// Logitude of the Instrument
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
#if NET461
        public List<Organisation> Organisations { get; set; }
#else
        [NotMapped, JsonIgnore]
        public List<Organisation> Organisations { get { return OrganisationInstruments?.Select(i => i.Organisation).Where(i => i != null).ToList(); } }
        [JsonProperty("organisations")]
        public List<NamedItem> OrganisationsList { get { return Organisations?.Select(i => i.AsNamedItem).ToList(); } }
        [JsonIgnore]
        public List<OrganisationInstrument> OrganisationInstruments { get; set; }
#endif

        /// <summary>
        /// Sensors linked to this Instrument
        /// </summary>
        //public List<Sensor> Sensors { get; set; }
        //@public List<InstrumentSensor> InstrumentSensors { get; set; }

        /// <summary>
        /// Stations linked to this Instrument
        /// </summary>
#if NET461
        public List<Station> Stations { get; set; }
#else
        [NotMapped, JsonIgnore]
        public List<Station> Stations { get { return StationInstruments?.Select(i => i.Station).Where(i => i != null).ToList(); } }
        [JsonProperty("stations")]
        public List<NamedItem> StationsList { get { return Stations?.Select(i => i.AsNamedItem).ToList(); } }
        [JsonIgnore]
        public List<StationInstrument> StationInstruments { get; set; }
#endif

        /// <summary>
        /// Sensors linked to this Instrument
        /// </summary>
#if NET461
        //@public List<Sensor> Sensors { get; set; }
#else
#endif
    }

    /*
    /// <summary>
    /// Offering entity
    /// </summary>
    public class Offering : BaseEntity
    {
        /// <summary>
        /// Code of the Offering
        /// </summary>
        [Required, StringLength(50)]
        public string Code { get; set; }
        /// <summary>
        /// <summary>
        /// Description of the Offering
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }

        // Navigation
        /// <summary>
        /// PhenomenaOfferings of this Offering
        /// </summary>
        public List<PhenomenonOffering> PhenomenaOfferings { get; set; }
        /// <summary>
        /// Phenomena of this Offering
        /// </summary>
        [NotMapped]
        public List<Phenomenon> Phenomena { get { return PhenomenaOfferings.Select(i => i.Phenomenon).ToList(); } }
    }
    */

    /// <summary>
    /// Organisation entity
    /// </summary>
    [Table("Organisation")]
    public class Organisation : BaseEntity
    {
        /// <summary>
        /// Code of the Site
        /// </summary>
        [Required, StringLength(50)]
        public string Code { get; set; }
        /// <summary>
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
#if NET461
        public List<Instrument> Instruments { get; set; }
#else
        [NotMapped, JsonIgnore]
        public List<Instrument> Instruments { get { return OrganisationInstruments?.Select(i => i.Instrument).Where(i => i != null).ToList(); } }
        [JsonProperty("instruments")]
        public List<NamedItem> InstrumentsList { get { return Instruments?.Select(i => i.AsNamedItem).ToList(); } }
        [JsonIgnore]
        public List<OrganisationInstrument> OrganisationInstruments { get; set; }
#endif

        /// <summary>
        /// The Sites linked to this Organisation
        /// </summary>
#if NET461
        public List<Site> Sites { get; set; }
#else
        [NotMapped, JsonIgnore]
        public List<Site> Sites { get { return OrganisationSites?.Select(i => i.Site).Where(i => i != null).ToList(); } }
        [JsonProperty("sites")]
        public List<NamedItem> SitesList { get { return Sites?.Select(i => i.AsNamedItem).ToList(); } }
        [JsonIgnore]
        public List<OrganisationSite> OrganisationSites { get; set; }
#endif

        /// <summary>
        /// The Stations linked to this Organisation
        /// </summary>
#if NET461
        public List<Station> Stations { get; set; }
#else
        [NotMapped, JsonIgnore]
        public List<Station> Stations { get { return OrganisationStations?.Select(i => i.Station).Where(i => i != null).ToList(); } }
        [JsonProperty("stations")]
        public List<NamedItem> StationsList { get { return Stations?.Select(i => i.AsNamedItem).ToList(); } }
        [JsonIgnore]
        public List<OrganisationStation> OrganisationStations { get; set; }
#endif
    }

    /*
    /// <summary>
    /// Phenomenon entity
    /// </summary>
    public class Phenomenon : BaseEntity
    {
        /// <summary>
        /// Code of the Phenomenon
        /// </summary>
        [Required, StringLength(50)]
        public string Code { get; set; }
        /// <summary>
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
        /// PhenomenonOfferings linked to this Phenomenon
        /// </summary>
        public List<PhenomenonOffering> PhenomenonOfferings { get; set; }
        /// <summary>
        /// PhenomenonUnits linked to this Phenomenon
        /// </summary>
        public List<PhenomenonUnit> PhenomenonUnits { get; set; }
        /// <summary>
        /// Sensors linked to this Phenomenon
        /// </summary>
        public List<Sensor> Sensors { get; set; }
    }

    /// <summary>
    /// PhenomenonOffering entity
    /// </summary>
    public class PhenomenonOffering
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false), HiddenInput]
        public Guid Id { get; set; }
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
    public class PhenomenonUnit
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false), HiddenInput]
        public Guid Id { get; set; }
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
    */

    /// <summary>
    /// Programme entity
    /// </summary>
    [Table("Programme")]
    public class Programme : BaseEntity
    {
        /// <summary>
        /// Code of the Programme
        /// </summary>
        [Required, StringLength(50)]
        public string Code { get; set; }
        /// <summary>
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

        //public bool HasStations { get { return Stations?.Any() ?? false; } }
        // Navigation

/// <summary>
/// The Projects linked to this Programme
/// </summary>
#if NET461
#else
        [JsonProperty("projects")]
        public List<NamedItem> ProjectList { get { return Projects?.Select(i => i.AsNamedItem).Where(i => i != null).ToList(); } }
        [JsonIgnore]
#endif
        public List<Project> Projects { get; set; }
    }

    /// <summary>
    /// Project entity
    /// </summary>
    [Table("Project")]
    public class Project : BaseEntity
    {
        /// <summary>
        /// The Programme of the Project
        /// </summary>
        [Required, JsonIgnore]
        public Guid ProgrammeId { get; set; }

        /// <summary>
        /// Code of the Project
        /// </summary>
        [Required, StringLength(50)]
        public string Code { get; set; }
        /// <summary>
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

        //public bool HasStations { get { return Stations?.Any() ?? false; } }
        // Navigation

/// <summary>
/// The Programme of the Project
/// </summary>
#if NET461
#else
        [JsonProperty("programme")]
        public NamedItem ProgrammeName { get { return Programme?.AsNamedItem; } }
        [JsonIgnore]
#endif
        public Programme Programme { get; set; }

        /// <summary>
        /// The Stations linked to this Project
        /// </summary>
#if NET461
        public List<Station> Stations { get; set; }
#else
        [NotMapped, JsonIgnore]
        public List<Station> Stations { get { return ProjectStations?.Select(i => i.Station).Where(i => i != null).ToList(); } }
        [JsonProperty("stations")]
        public List<NamedItem> StationsList { get { return Stations.Select(i => i.AsNamedItem).ToList(); } }
        [JsonIgnore]
        public List<ProjectStation> ProjectStations { get; set; }
#endif
    }

    /*
    /// <summary>
    /// Sensor entity
    /// </summary>
    public class Sensor : BaseEntity
    {
        /// <summary>
        /// Code of the Sensor
        /// </summary>
        [Required, StringLength(50)]
        public string Code { get; set; }
        /// <summary>
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
        //public List<Instrument> Instruments { get; set; }
        public List<InstrumentSensor> InstrumentSensors { get; set; }
        /// <summary>
        /// Phenomenon of the Sensor
        /// </summary>
        public Phenomenon Phenomenon { get; set; }
    }
    */

    /// <summary>
    /// Site entity
    /// </summary>
    [Table("Site")]
    public class Site : BaseEntity
    {
        /// <summary>
        /// Code of the Site
        /// </summary>
        [Required, StringLength(50)]
        public string Code { get; set; }
        /// <summary>
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
#if NET461
        public List<Organisation> Organisations { get; set; }
#else
        [NotMapped,JsonIgnore]
        public List<Organisation> Organisations { get { return OrganisationSites?.Select(i => i.Organisation).Where(i => i != null).ToList(); } }
        [JsonProperty("Organisations")]
        public List<NamedItem> OrganisationsList { get { return Organisations?.Select(i => i.AsNamedItem).ToList(); } }
        [JsonIgnore]
        public List<OrganisationSite> OrganisationSites { get; set; }
#endif

/// <summary>
/// The Stations linked to this Site
/// </summary>
#if NET461
#else
        [JsonProperty("stations")]
        public List<NamedItem> StationsList { get { return Stations?.Select(i => i.AsNamedItem).ToList(); } }
        [JsonIgnore]
#endif
        public List<Station> Stations { get; set; }
    }

    /// <summary>
    /// Station entity
    /// </summary>
    [Table("Station")]
    public class Station : BaseEntity
    {
        /// <summary>
        /// Code of the Station
        /// </summary>
        [Required, StringLength(50)]
        public string Code { get; set; }
        /// <summary>
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
/// Site of the Station
/// </summary>
#if NET461
#else
        [JsonProperty("site")]
        public NamedItem SiteName { get { return Site?.AsNamedItem; } }
        [JsonIgnore]
#endif
        public Site Site { get; set; }

        /// <summary>
        /// Instruments linked to this Station
        /// </summary>
#if NET461
        public List<Instrument> Instruments { get; set; }
#else
        [NotMapped, JsonIgnore]
        public List<Instrument> Instruments { get { return StationInstruments?.Select(i => i.Instrument).Where(i => i != null).ToList(); } }
        [JsonProperty("instruments")]
        public List<NamedItem> InstrumentsList { get { return Instruments?.Select(i => i.AsNamedItem).ToList(); } }
        [JsonIgnore]
        public List<StationInstrument> StationInstruments { get; set; }
#endif

        /// <summary>
        /// The Projects linked to this Station
        /// </summary>
#if NET461
        public List<Project> Projects { get; set; }
#else
        [NotMapped, JsonIgnore]
        public List<Project> Projects { get { return ProjectStations?.Select(i => i.Project).Where(i => i != null).ToList(); } }
        [JsonProperty("projects")]
        public List<NamedItem> ProjectsList { get { return Projects?.Select(i => i.AsNamedItem).ToList(); } }
        [JsonIgnore]
        public List<ProjectStation> ProjectStations { get; set; }
#endif

        /// <summary>
        /// The Organisations linked to this Station
        /// </summary>
#if NET461
        public List<Organisation> Organisations { get; set; }
#else
        [NotMapped, JsonIgnore]
        public List<Organisation> Organisations { get { return OrganisationStations?.Select(i => i.Organisation).Where(i => i != null).ToList(); } }
        [JsonProperty("organisations")]
        public List<NamedItem> OrganisationsList { get { return Organisations?.Select(i => i.AsNamedItem).ToList(); } }
        [JsonIgnore]
        public List<OrganisationStation> OrganisationStations { get; set; }
#endif
    }

    /*
    /// <summary>
    /// Unit Entity
    /// </summary>
    public class Unit : BaseEntity
    {
        /// <summary>
        /// Symbol of the Entity
        /// </summary>
        [Required, StringLength(20), Column("UnitSymbol")]
        public string Symbol { get; set; }

        // Navigation
        /// <summary>
        /// PhenomenaUnits of this Unit
        /// </summary>
        public List<PhenomenonUnit> PhenomenonUnits { get; set; }
        /// <summary>
        /// Phenomena of this Unit
        /// </summary>
        [NotMapped]
        public List<Phenomenon> Phenomena { get { return PhenomenonUnits?.Where(i => i.PhenomenonId == Id).Select(i => i.Phenomenon).ToList(); } }
    }
    */

    /*
    /// <summary>
    /// UserDownload entity
    /// </summary>
    public class UserDownload : BaseEntity
    {
        /// <summary>
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
    public class UserQuery : BaseEntity
    {
        /// <summary>
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
    */

#if NET461
#else
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
#if NET461
        public ObservationsDbContext() : base("Observations")
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
            //Database.Log = Console.Write;
        }
#else
        public ObservationsDbContext(DbContextOptions<ObservationsDbContext> options)
            : base(options)
        {
        }
#endif
        public DbSet<Instrument> Instruments { get; set; }
        //public DbSet<InventoryTotal> InventoryTotals { get; set; }
        //public DbSet<InventoryStation> InventoryStations { get; set; }
        //public DbSet<InventoryInstrument> InventoryInstruments { get; set; }
        //public DbSet<InventoryPhenomenonOffering> InventoryPhenomenaOfferings { get; set; }
        //public DbSet<InventoryYear> InventoryYears { get; set; }
        //public DbSet<InventoryOrganisation> InventoryOrganisations { get; set; }
        //@public DbSet<Offering> Offerings { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        //@public DbSet<Phenomenon> Phenomena { get; set; }
        //@public DbSet<PhenomenonOffering> PhenomenonOfferings { get; set; }
        //@public DbSet<PhenomenonUnit> PhenomenonUnits { get; set; }
        public DbSet<Programme> Programmes { get; set; }
        public DbSet<Project> Projects { get; set; }
        //@public DbSet<Sensor> Sensors { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Station> Stations { get; set; }
        //@public DbSet<Unit> Units { get; set; }
        //public DbSet<UserDownload> UserDownloads { get; set; }
        //public DbSet<UserQuery> UserQueries { get; set; }

        //public DbSet<vApiDataDownload> vApiDataDownloads { get; set; }
        //public DbSet<vApiDataQuery> vApiDataQueries { get; set; }
        //public DbSet<vApiInventory> vApiInventory { get; set; }
        //public DbSet<vApiSpacialCoverage> vApiSpacialCoverages { get; set; }
        //public DbSet<vApiTemporalCoverage> vApiTemporalCoverages { get; set; }
        //public DbSet<vSensorThingsDatastream> vSensorThingsDatastreams { get; set; }

#if NET461
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
            modelBuilder.Entity<Station>()
                .HasMany<Instrument>(l => l.Instruments)
                .WithMany(r => r.Stations)
                .Map(cs =>
                {
                    cs.MapLeftKey("StationID");
                    cs.MapRightKey("InstrumentID");
                    cs.ToTable("Station_Instrument");
                });
            /*
            modelBuilder.Entity<Instrument>()
                .HasMany<Sensor>(l => l.Sensors)
                .WithMany(r => r.Instruments)
                .Map(cs =>
                {
                    cs.MapLeftKey("InstrumentID");
                    cs.MapRightKey("SensorID");
                    cs.ToTable("Instrument_Sensor");
                });
            modelBuilder.Entity<Phenomenon>().ToTable("Phenomenon");
            //modelBuilder.Entity<Phenomenon>()
            //    .HasMany<Offering>(l => l.Offerings)
            //    .WithMany(r => r.Phenomena)
            //    .Map(cs =>
            //    {
            //        cs.MapLeftKey("PhenomenonID");
            //        cs.MapRightKey("OfferingID");
            //        cs.ToTable("PhenomenonOffering");
            //    });
            modelBuilder.Entity<UnitOfMeasure>().ToTable("UnitOfMeasure");
            modelBuilder.Entity<UnitOfMeasure>().Property(p => p.Name).HasColumnName("Unit");
            modelBuilder.Entity<UnitOfMeasure>().Property(p => p.Symbol).HasColumnName("UnitSymbol");
            modelBuilder.Entity<Phenomenon>()
                .HasMany<UnitOfMeasure>(l => l.UnitsOfMeasure)
                .WithMany(r => r.Phenomena)
                .Map(cs =>
                {
                    cs.MapLeftKey("PhenomenonID");
                    cs.MapRightKey("UnitOfMeasureID");
                    cs.ToTable("PhenomenonUOM");
                });
            modelBuilder.Entity<UserDownload>().ToTable("UserDownloads");
            modelBuilder.Entity<UserQuery>().ToTable("UserQueries");
            */
        }
#else
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //@modelBuilder.Entity<Unit>().Property(p => p.Name).HasColumnName("Unit");

            //> Remove later once we have proper many to many in Entity Framework Core
            //@modelBuilder.Entity<InstrumentSensor>().HasKey(i => new { i.InstrumentId, i.SensorId });
            modelBuilder.Entity<OrganisationInstrument>().HasKey(i => new { i.OrganisationId, i.InstrumentId });
            modelBuilder.Entity<OrganisationSite>().HasKey(i => new { i.OrganisationId, i.SiteId });
            modelBuilder.Entity<OrganisationStation>().HasKey(i => new { i.OrganisationId, i.StationId });
            modelBuilder.Entity<ProjectStation>().HasKey(i => new { i.ProjectId, i.StationId });
            modelBuilder.Entity<StationInstrument>().HasKey(i => new { i.StationId, i.InstrumentId });
            //< Remove later once we have proper many to many in Entity Framework Core

            //    modelBuilder.Entity<Organisation>()
            //        .HasMany<Site>(l => l.Sites)
            //        .WithMany(r => r.Organisations)
            //        .Map(cs =>
            //        {
            //            cs.MapLeftKey("OrganisationID");
            //            cs.MapRightKey("SiteID");
            //            cs.ToTable("Organisation_Site");
            //        });
            //    modelBuilder.Entity<Station>()
            //        .HasMany<Instrument>(l => l.Instruments)
            //        .WithMany(r => r.Stations)
            //        .Map(cs =>
            //        {
            //            cs.MapLeftKey("StationID");
            //            cs.MapRightKey("InstrumentID");
            //            cs.ToTable("Station_Instrument");
            //        });
            //    modelBuilder.Entity<Instrument>()
            //        .HasMany<Sensor>(l => l.Sensors)
            //        .WithMany(r => r.Instruments)
            //        .Map(cs =>
            //        {
            //            cs.MapLeftKey("InstrumentID");
            //            cs.MapRightKey("SensorID");
            //            cs.ToTable("Instrument_Sensor");
            //        });
            //    modelBuilder.Entity<Phenomenon>().ToTable("Phenomenon");
            //    //modelBuilder.Entity<Phenomenon>()
            //    //    .HasMany<Offering>(l => l.Offerings)
            //    //    .WithMany(r => r.Phenomena)
            //    //    .Map(cs =>
            //    //    {
            //    //        cs.MapLeftKey("PhenomenonID");
            //    //        cs.MapRightKey("OfferingID");
            //    //        cs.ToTable("PhenomenonOffering");
            //    //    });
            //    modelBuilder.Entity<UnitOfMeasure>().ToTable("UnitOfMeasure");
            //    modelBuilder.Entity<UnitOfMeasure>().Property(p => p.Name).HasColumnName("Unit");
            //    modelBuilder.Entity<UnitOfMeasure>().Property(p => p.Symbol).HasColumnName("UnitSymbol");
            //    modelBuilder.Entity<Phenomenon>()
            //        .HasMany<UnitOfMeasure>(l => l.UnitsOfMeasure)
            //        .WithMany(r => r.Phenomena)
            //        .Map(cs =>
            //        {
            //            cs.MapLeftKey("PhenomenonID");
            //            cs.MapRightKey("UnitOfMeasureID");
            //            cs.ToTable("PhenomenonUOM");
            //        });
            //    modelBuilder.Entity<UserDownload>().ToTable("UserDownloads");
            //    modelBuilder.Entity<UserQuery>().ToTable("UserQueries");

            //    modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
#endif
    }

}
