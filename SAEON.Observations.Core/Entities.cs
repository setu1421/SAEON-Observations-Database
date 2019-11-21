#if NET472
using Newtonsoft.Json;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Configuration;
using System.Web.Mvc;
#else
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
#endif
using SAEON.Logs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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
    /// Absolute base class
    /// </summary>
    public abstract class BaseEntity { }

    public abstract class BaseIDEntity : BaseEntity
    {
        /// <summary>
        /// Id of the Entity
        /// </summary>
        //[Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false), HiddenInput]
#if NET472
        [JsonProperty(Order = -99)]
#endif
        public Guid Id { get; set; }
    }

    /// <summary>
    /// Base for entities
    /// </summary>
    public abstract class IDEntity : BaseIDEntity
    {
        [JsonIgnore, Timestamp, Column(Order = 10000), ConcurrencyCheck, ScaffoldColumn(false), HiddenInput]
        public byte[] RowVersion { get; set; }
        [JsonIgnore, Required]
        public Guid UserId { get; set; }

        [NotMapped, JsonIgnore]
        public virtual EntityListItem AsEntityListItem => new EntityListItem { Id = Id };
    }

    public abstract class NamedEntity : IDEntity
    {
        /// <summary>
        /// Name of the Entity
        /// </summary>
        [Required, StringLength(150)]
#if NET472
        [JsonProperty(Order = -97)]
#endif
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
        [Required, StringLength(50)]
#if NET472
        [JsonProperty(Order = -98)]
#endif
        public string Code { get; set; }
        [NotMapped, JsonIgnore]
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
    /// Data Schema entity
    /// </summary>
    [Table("DataSchema")]
    public class DataSchema : CodedEntity
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
    public class DataSource : CodedEntity
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
    public class DataSourceType : BaseEntity
    {
        /// <summary>
        /// Id of the Entity
        /// </summary>
        //[Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false), HiddenInput]
#if NET472
        [JsonProperty(Order = -99)]
#endif
        public Guid Id { get; set; }
        [Timestamp, Column(Order = 10000), ConcurrencyCheck, ScaffoldColumn(false), HiddenInput]


        public byte[] RowVersion { get; set; }
        public Guid? UserId { get; set; }

        //public EntityListItem AsEntityListItem => new EntityListItem { Id = Id, Code = Code };

        /// <summary>
        /// Code of the Entity
        /// </summary>
        [Required, StringLength(50)]
#if NET472
        [JsonProperty(Order = -98)]
#endif
        public string Code { get; set; }
        /// <summary>
        /// Description of the Instrument
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }

        // Navigation
        public List<DataSchema> DataSchemas { get; set; }
    }

    /// <summary>
    /// DigitalObjectIdentifiers entity
    /// </summary>
    public class DigitalObjectIdentifier : BaseEntity
    {
        /// <summary>
        /// Id of the DigitalObjectIdentifier
        /// </summary>
        //[Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false), HiddenInput]
        public int Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string DOI { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed), Display(Name = "DOI Url")]
        public string DOIUrl { get; set; }
        /// <summary>
        /// Name of the DigitalObjectIdentifier
        /// </summary>
        [Required, StringLength(1000)]
        public string Name { get; set; }
        /// <summary>
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
        [Timestamp, ConcurrencyCheck, ScaffoldColumn(false), HiddenInput]
        public byte[] RowVersion { get; set; }

        // Navigation
        public List<UserDownload> UserDownloads { get; set; }
    }

    /// <summary>
    /// Instrument entity
    /// </summary>
    [Table("Instrument")]
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
#if NET472
        /// <summary>
        /// The Organisations linked to this Instrument
        /// </summary>
        [JsonIgnore]
        public List<Organisation> Organisations { get; set; }
        [JsonProperty("Organisations")]
        public List<EntityListItem> OrganisationsList => Organisations?.Select(i => i.AsEntityListItem).ToList();

        /// <summary>
        /// Stations linked to this Instrument
        /// </summary>
        [JsonIgnore]
        public List<Station> Stations { get; set; }
        [JsonProperty("Stations")]
        public List<EntityListItem> StationsList => Stations?.Select(i => i.AsEntityListItem).ToList();

        /// <summary>
        /// Sensors linked to this Instrument
        /// </summary>
        [JsonIgnore]
        public List<Sensor> Sensors { get; set; }
        [JsonProperty("Sensors")]
        public List<EntityListItem> SensorsList => Sensors?.Select(i => i.AsEntityListItem).ToList();
#else
        /// <summary>
        /// The Organisations linked to this Instrument
        /// </summary>
        [NotMapped, JsonIgnore]
        public List<Organisation> Organisations => OrganisationInstruments?.Select(oi => oi.Organisation).ToList();
        [NotMapped, JsonPropertyName("organisations")]
        public List<EntityListItem> OrganisationsList => Organisations?.Select(i => i.AsEntityListItem).ToList();
        [JsonIgnore, SwaggerIgnore]
        public List<OrganisationInstrument> OrganisationInstruments { get; set; }

        /// <summary>
        /// Stations linked to this Instrument
        /// </summary>
        [NotMapped, JsonIgnore]
        public List<Station> Stations => StationInstruments?.Select(si => si.Station).ToList();
        [NotMapped, JsonPropertyName("stations")]
        public List<EntityListItem> StationsList => Stations?.Select(i => i.AsEntityListItem).ToList();
        [JsonIgnore, SwaggerIgnore]
        public List<StationInstrument> StationInstruments { get; set; }

        /// <summary>
        /// Sensors linked to this Instrument
        /// </summary>
        [NotMapped, JsonIgnore]
        public List<Sensor> Sensors => InstrumentSensors?.Select(i => i.Sensor).ToList();
        [NotMapped, JsonPropertyName("sensors")]
        public List<EntityListItem> SensorsList => Sensors?.Select(i => i.AsEntityListItem).ToList();
        [JsonIgnore, SwaggerIgnore]
        public List<InstrumentSensor> InstrumentSensors { get; set; }
#endif
    }

    /// <summary>
    /// Offering entity
    /// </summary>
    [Table("Offering")]
    public class Offering : CodedEntity
    {
        /// Description of the Offering
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }

        // Navigation
#if NET472
        /// <summary>
        /// Phenomena of this Offering
        /// </summary>
        [JsonIgnore]
        public List<Phenomenon> Phenomena { get; set; }
        [JsonProperty("Phenomena")]
        public List<EntityListItem> PhenomenaList => Phenomena?.Select(i => i.AsEntityListItem).ToList();
#else
        /// <summary>
        /// Phenomena of this Offering
        /// </summary>
        [NotMapped, JsonIgnore]
        public List<Phenomenon> Phenomena => PhenomenonOfferings?.Select(po => po.Phenomenon).ToList();
        [NotMapped, JsonPropertyName("phenomena")]
        public List<EntityListItem> PhenomenaList => Phenomena?.Select(i => i.AsEntityListItem).ToList();
        [JsonIgnore, SwaggerIgnore]
        public List<PhenomenonOffering> PhenomenonOfferings { get; set; }
#endif
    }

    /// <summary>
    /// Organisation entity
    /// </summary>
    [Table("Organisation")]
    public class Organisation : CodedEntity
    {
        /// <summary>
        /// Description of the Organisation
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
#if NET472
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

        /// <summary>
        /// The Instruments linked to this Organisation
        /// </summary>
        [JsonIgnore]
        public List<Instrument> Instruments { get; set; }
        [JsonProperty("Instruments")]
        public List<EntityListItem> InstrumentsList => Instruments?.Select(i => i.AsEntityListItem).ToList();
#else
        /// <summary>
        /// The Instruments linked to this Organisation
        /// </summary>
        [NotMapped, JsonIgnore]
        public List<Instrument> Instruments => OrganisationInstruments?.Select(oi => oi.Instrument).ToList();
        [NotMapped, JsonPropertyName("instruments")]
        public List<EntityListItem> InstrumentsList => Instruments?.Select(i => i.AsEntityListItem).ToList();
        [JsonIgnore, SwaggerIgnore]
        public List<OrganisationInstrument> OrganisationInstruments { get; set; }

        /// <summary>
        /// The Sites linked to this Organisation
        /// </summary>
        [NotMapped, JsonIgnore]
        public List<Site> Sites => OrganisationSites?.Select(os => os.Site).ToList();
        [NotMapped, JsonPropertyName("sites")]
        public List<EntityListItem> SitesList => Sites?.Select(i => i.AsEntityListItem).ToList();
        [JsonIgnore, SwaggerIgnore]
        public List<OrganisationSite> OrganisationSites { get; set; }

        /// <summary>
        /// The Stations linked to this Organisation
        /// </summary>
        [NotMapped, JsonIgnore]
        public List<Station> Stations => OrganisationStations?.Select(os => os.Station).ToList();
        [NotMapped, JsonPropertyName("stations")]
        public List<EntityListItem> StationsList => Stations?.Select(i => i.AsEntityListItem).ToList();
        [JsonIgnore, SwaggerIgnore]
        public List<OrganisationStation> OrganisationStations { get; set; }
#endif
    }

    /// <summary>
    /// OrganisationRole entity
    /// </summary>
    public class OrganisationRole : CodedEntity
    {
        // Navigation
        //public List<Organisation> Organisations { get; set; }
    }

    /// <summary>
    /// Phenomenon entity
    /// </summary>
    [Table("Phenomenon")]
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
#if NET472
        /// <summary>
        /// Offerings of this Phenomenon
        /// </summary>
        [JsonIgnore]
        public List<Offering> Offerings { get; set; }
        [JsonProperty("Offerings"), NotMapped]
        public List<EntityListItem> OfferingsList => Offerings?.Select(i => i.AsEntityListItem).ToList();

        /// <summary>
        /// Units of this Phenomenon
        /// </summary>
        [JsonIgnore]
        public List<Unit> Units { get; set; }
        [JsonProperty("Units"), NotMapped]
        public List<EntityListItem> UnitsList => Units?.Select(i => i.AsEntityListItem).ToList();

        /// <summary>
        /// Sensors linked to this Phenomenon
        /// </summary>
        [JsonIgnore]
        public List<Sensor> Sensors { get; set; }
        [JsonProperty("Sensors"), NotMapped]
        public List<EntityListItem> SensorsList => Sensors?.Select(i => i.AsEntityListItem).ToList();
#else
        /// <summary>
        /// Offerings of this Phenomenon
        /// </summary>
        [NotMapped, JsonIgnore]
        public List<Offering> Offerings => PhenomenonOfferings?.Select(ph => ph.Offering).ToList();
        [NotMapped, JsonPropertyName("offerings")]
        public List<EntityListItem> OfferingsList => Offerings?.Select(i => i.AsEntityListItem).ToList();
        [JsonIgnore, SwaggerIgnore]
        public List<PhenomenonOffering> PhenomenonOfferings { get; set; }

        /// <summary>
        /// Units of this Phenomenon
        /// </summary>
        [NotMapped, JsonIgnore]
        public List<Unit> Units => PhenomenonUnits?.Select(pu => pu.Unit).ToList();
        [NotMapped, JsonPropertyName("units")]
        public List<EntityListItem> UnitsList => Units?.Select(i => i.AsEntityListItem).ToList();
        [JsonIgnore, SwaggerIgnore]
        public List<PhenomenonUnit> PhenomenonUnits { get; set; }

        /// <summary>
        /// Sensors linked to this Phenomenon
        /// </summary>
        [NotMapped, JsonPropertyName("sensors")]
        public List<EntityListItem> SensorsList => Sensors?.Select(i => i.AsEntityListItem).ToList();
        [JsonIgnore, SwaggerIgnore]
        public List<Sensor> Sensors { get; set; }
#endif
    }

    /// <summary>
    /// Programme entity
    /// </summary>
    [Table("Programme")]
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
#if NET472
        /// <summary>
        /// The Projects linked to this Programme
        /// </summary>
        [JsonIgnore]
        public List<Project> Projects { get; set; }
        [JsonProperty("Projects"), NotMapped]
        public List<EntityListItem> ProjectList => Projects?.Select(i => i.AsEntityListItem).Where(i => i != null).ToList();
#else
        /// <summary>
        /// The Projects linked to this Programme
        /// </summary>
        [NotMapped, JsonPropertyName("projects")]
        public List<EntityListItem> ProjectList => Projects?.Select(i => i.AsEntityListItem).ToList();
        [JsonIgnore]
        public List<Project> Projects { get; set; }
#endif
    }

    /// <summary>
    /// Project entity
    /// </summary>
    [Table("Project")]
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
#if NET472
        /// <summary>
        /// The Programme of the Project
        /// </summary>
        [JsonIgnore]
        public Programme Programme { get; set; }
        [JsonProperty("Programme"), NotMapped]
        public EntityListItem ProgrammeItem => Programme?.AsEntityListItem;

        /// <summary>
        /// The Stations linked to this Project
        /// </summary>
        [JsonIgnore]
        public List<Station> Stations { get; set; }
        [JsonProperty("Stations"), NotMapped]
        public List<EntityListItem> StationsList => Stations?.Select(i => i.AsEntityListItem).ToList();
#else
        /// <summary>
        /// The Programme of the Project
        /// </summary>
        [JsonIgnore]
        public Programme Programme { get; set; }
        [NotMapped, JsonPropertyName("programme")]
        public EntityListItem ProgrammeItem => Programme?.AsEntityListItem;

        /// <summary>
        /// The Stations linked to this Project
        /// </summary>
        [NotMapped, JsonIgnore]
        public List<Station> Stations => ProjectStations?.Select(ps => ps.Station).ToList();
        [NotMapped, JsonPropertyName("stations")]
        public List<EntityListItem> StationsList => Stations?.Select(i => i.AsEntityListItem).ToList();
        [JsonIgnore, SwaggerIgnore]
        public List<ProjectStation> ProjectStations { get; set; }
#endif
    }

    /// <summary>
    /// Sensor entity
    /// </summary>
    [Table("Sensor")]
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
        [JsonIgnore]
        public DataSource DataSource { get; set; }

#if NET472
        /// <summary>
        /// Phenomenon of the Sensor
        /// </summary>
        [JsonIgnore]
        public Phenomenon Phenomenon { get; set; }
        [JsonProperty("Phenomenon")]
        public EntityListItem PhenomenonName => Phenomenon?.AsEntityListItem;

        /// <summary>
        /// Instruments linked to this Sensor
        /// </summary>
        [JsonIgnore]
        public List<Instrument> Instruments { get; set; }
        [JsonProperty("Instruments")]
        public List<EntityListItem> InstrumentsList => Instruments?.Select(i => i.AsEntityListItem).ToList();
#else
        /// <summary>
        /// Phenomenon of the Sensor
        /// </summary>
        [JsonIgnore]
        public Phenomenon Phenomenon { get; set; }
        [NotMapped, JsonPropertyName("phenomenon")]
        public EntityListItem PhenomenonName => Phenomenon?.AsEntityListItem;

        /// <summary>
        /// Instruments linked to this Sensor
        /// </summary>
        [NotMapped, JsonIgnore]
        public List<Instrument> Instruments => InstrumentSensors?.Select(i => i.Instrument).ToList();
        [NotMapped, JsonPropertyName("instruments")]
        public List<EntityListItem> InstrumentsList => Instruments?.Select(i => i.AsEntityListItem).ToList();
        [JsonIgnore, SwaggerIgnore]
        public List<InstrumentSensor> InstrumentSensors { get; set; }
#endif
    }

    /// <summary>
    /// Site entity
    /// </summary>
    [Table("Site")]
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
#if NET472
        /// <summary>
        /// The Organisations linked to this Site
        /// </summary>
        [JsonIgnore]
        public List<Organisation> Organisations { get; set; }
        [JsonProperty("Organisations"), NotMapped]
        public List<EntityListItem> OrganisationsList => Organisations?.Select(i => i.AsEntityListItem).ToList();

        /// <summary>
        /// The Stations linked to this Site
        /// </summary>
        [JsonIgnore]
        public List<Station> Stations { get; set; }
        [JsonProperty("Stations"), NotMapped]
        public List<EntityListItem> StationsList => Stations?.Select(i => i.AsEntityListItem).ToList();
#else
        /// <summary>
        /// The Organisations linked to this Site
        /// </summary>
        [NotMapped, JsonIgnore]
        public List<Organisation> Organisations => OrganisationSites?.Select(os => os.Organisation).ToList();
        [NotMapped, JsonPropertyName("organisations")]
        public List<EntityListItem> OrganisationsList => Organisations?.Select(i => i.AsEntityListItem).ToList();
        [JsonIgnore, SwaggerIgnore]
        public List<OrganisationSite> OrganisationSites { get; set; }

        /// <summary>
        /// The Stations linked to this Site
        /// </summary>
        [NotMapped, JsonPropertyName("stations")]
        public List<EntityListItem> StationsList => Stations?.Select(i => i.AsEntityListItem).ToList();
        [JsonIgnore, SwaggerIgnore]
        public List<Station> Stations { get; set; }
#endif
    }

    /// <summary>
    /// Station entity
    /// </summary>
    [Table("Station")]
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
#if NET472
        /// <summary>
        /// Site of the Station
        /// </summary>
        [JsonIgnore]
        public Site Site { get; set; }
        [JsonProperty("Site"), NotMapped]
        public EntityListItem SiteName => Site?.AsEntityListItem;

        /// <summary>
        /// The Organisations linked to this Station
        /// </summary>
        [JsonIgnore]
        public List<Organisation> Organisations { get; set; }
        [JsonIgnore]
        [JsonProperty("Organisations"), NotMapped]
        public List<EntityListItem> OrganisationsList => Organisations?.Select(i => i.AsEntityListItem).ToList();

        /// <summary>
        /// The Projects linked to this Station
        /// </summary>
        [JsonIgnore]
        public List<Project> Projects { get; set; }
        [JsonProperty("Projects"), NotMapped]
        public List<EntityListItem> ProjectsList => Projects?.Select(i => i.AsEntityListItem).ToList();

        /// <summary>
        /// Instruments linked to this Station
        /// </summary>
        [JsonIgnore]
        public List<Instrument> Instruments { get; set; }
        [JsonProperty("Instruments"), NotMapped]
        public List<EntityListItem> InstrumentsList => Instruments?.Select(i => i.AsEntityListItem).ToList();
#else
        /// <summary>
        /// Site of the Station
        /// </summary>
        [JsonIgnore]
        public Site Site { get; set; }
        [NotMapped, JsonPropertyName("site")]
        public EntityListItem SiteName => Site?.AsEntityListItem;

        /// <summary>
        /// The Organisations linked to this Station
        /// </summary>
        [NotMapped, JsonIgnore]
        public List<Organisation> Organisations => OrganisationStations?.Select(os => os.Organisation).ToList();
        [NotMapped, JsonPropertyName("organisations")]
        public List<EntityListItem> OrganisationsList => Organisations?.Select(i => i.AsEntityListItem).ToList();
        [JsonIgnore, SwaggerIgnore]
        public List<OrganisationStation> OrganisationStations { get; set; }

        /// <summary>
        /// The Projects linked to this Station
        /// </summary>
        [NotMapped, JsonIgnore]
        public List<Project> Projects => ProjectStations?.Select(ps => ps.Project).ToList();
        [NotMapped, JsonPropertyName("projects")]
        public List<EntityListItem> ProjectsList => Projects?.Select(i => i.AsEntityListItem).ToList();
        [JsonIgnore, SwaggerIgnore]
        public List<ProjectStation> ProjectStations { get; set; }

        /// <summary>
        /// Instruments linked to this Station
        /// </summary>
        [NotMapped, JsonIgnore]
        public List<Instrument> Instruments => StationInstruments?.Select(si => si.Instrument).ToList();
        [NotMapped, JsonPropertyName("instruments")]
        public List<EntityListItem> InstrumentsList => Instruments?.Select(i => i.AsEntityListItem).ToList();
        [JsonIgnore, SwaggerIgnore]
        public List<StationInstrument> StationInstruments { get; set; }
#endif
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
#if NET472
        /// <summary>
        /// Phenomena of this Unit
        /// </summary>
        [JsonIgnore]
        public List<Phenomenon> Phenomena { get; set; }
        [JsonProperty("Phenomena")]
        public List<EntityListItem> PhenomenaList => Phenomena?.Select(i => i.AsEntityListItem).ToList();
#else
        /// <summary>
        /// Phenomena of this Unit
        /// </summary>
        [NotMapped, JsonIgnore]
        public List<Phenomenon> Phenomena => PhenomenonUnits?.Select(pu => pu.Phenomenon).ToList();
        [NotMapped, JsonPropertyName("phenomena")]
        public List<EntityListItem> PhenomenaList => Phenomena?.Select(i => i.AsEntityListItem).ToList();
        [JsonIgnore, SwaggerIgnore]
        public List<PhenomenonUnit> PhenomenonUnits { get; set; }
#endif
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
        /// <summary>
        /// DigitalObjectIdentifierID of the download
        /// </summary>
        [Required, Display(Name = "Digital Object Identifier (DOI)")]
        public int DigitalObjectIdentifierId { get; set; }
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
        /// ODI Id for the download
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
        [ScaffoldColumn(false), HiddenInput]
        public new string UserId { get; set; }
        /// <summary>
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
        public DigitalObjectIdentifier DigitalObjectIdentifier { get; set; }
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
        [ScaffoldColumn(false), HiddenInput]
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

    [Table("vImportBatchSummary")]
    public class ImportBatchSummary
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ImportBatchId { get; set; }
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
    }

    [Table("vInventory")]
    public class Inventory : BaseEntity
    {
        [Key]
        public long Id { get; set; }
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

        // Navigation
        //public Site Site { get; set; }
        //public Station Station { get; set; }
        //public Instrument Instrument { get; set; }
        //public Sensor Sensor { get; set; }
        //public PhenomenonOffering PhenomenonOffering { get; set; }
        //[ForeignKey("PhenomenonUnitId")]
        //public PhenomenonUnit PhenomenonUnit { get; set; }
    }

#if NET472
    [Table("vLocations")]
#endif
    public class Location
    {
#if NET472
        [Key, Column(Order = 1)]
#endif
        public Guid OrganisationID { get; set; }
        public string OrganisationName { get; set; }
#if NET472
        [Key, Column(Order = 2)]
#endif
        public Guid SiteID { get; set; }
        public string SiteName { get; set; }
#if NET472
        [Key, Column(Order = 3)]
#endif
        public Guid StationID { get; set; }
        public string StationName { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Elevation { get; set; }
        public string Url { get; set; }

        // Navigation
        //public Organisation Organisation { get; set; }
        //public Site Site { get; set; }
        //public Station Station { get; set; }
    }

#if NET472
    [Table("vFeatures")]
#endif
    public class Feature
    {
#if NET472
        [Key, Column(Order = 1)]
#endif
        public Guid PhenomenonID { get; set; }
        public string PhenomenonName { get; set; }
#if NET472
        [Key, Column(Order = 2)]
#endif
        public Guid PhenomenonOfferingID { get; set; }
        public Guid OfferingID { get; set; }
        public string OfferingName { get; set; }
#if NET472
        [Key, Column("PhenomenonUOMID", Order = 3)]
#endif
        public Guid PhenomenonUnitID { get; set; }
        [Column("UnitOfMeasureID")]
        public Guid UnitID { get; set; }
        [Column("UnitOfMeasureUnit")]
        public string UnitName { get; set; }
    }

    [Table("vObservationExpansion")]
    public class Observation
    {
        [Key]
        public int Id { get; set; }
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

#region SensorThingsAPI

    [Table("vSensorThingsAPIDatastreams")]
    public class SensorThingsDatastream : BaseIDEntity
    {
        [Required, MaxLength(50)]
        public string Code { get; set; }
        [Required, MaxLength(150)]
        public string Name { get; set; }
        [MaxLength(5000)]
        public string Description { get; set; }
        [MaxLength(150)]
        public string PhenomenonName { get; set; }
        [MaxLength(5000)]
        public string PhenomenonDescription { get; set; }
        [MaxLength(2500)]
        public string PhenomenonUrl { get; set; }
        [MaxLength(150)]
        public string OfferingName { get; set; }
        [MaxLength(5000)]
        public string OfferingDescription { get; set; }
        [MaxLength(100)]
        public string UnitOfMeasureUnit { get; set; }
        [MaxLength(20)]
        public string UnitOfMeasureSymbol { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? LatitudeNorth { get; set; }
        public double? LatitudeSouth { get; set; }
        public double? LongitudeWest { get; set; }
        public double? LongitudeEast { get; set; }
    }

    [Table("vSensorThingsAPILocations")]
    public class SensorThingsLocation : BaseIDEntity
    {
        [Required, MaxLength(50)]
        public string Code { get; set; }
        [Required, MaxLength(150)]
        public string Name { get; set; }
        [MaxLength(5000)]
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Elevation { get; set; }
    }

    [Table("vSensorThingsAPIObserverdProperty")]
    public class SensorThingsObservedPropery : BaseIDEntity
    {
        [Required, MaxLength(50)]
        public string Code { get; set; }
        [Required, MaxLength(150)]
        public string Name { get; set; }
        [MaxLength(5000)]
        public string Description { get; set; }
        [MaxLength(250)]
        public string Url { get; set; }
    }

    [Table("vSensorThingsAPISensors")]
    public class SensorThingsSensor : BaseIDEntity
    {
        [Required, MaxLength(50)]
        public string Code { get; set; }
        [Required, MaxLength(150)]
        public string Name { get; set; }
        [MaxLength(5000)]
        public string Description { get; set; }
        [MaxLength(250)]
        public string Url { get; set; }
    }

    [Table("vSensorThingsAPIThings")]
    public class SensorThingsThing : BaseIDEntity
    {
        [Required, MaxLength(50)]
        public string Code { get; set; }
        [Required, MaxLength(150)]
        public string Name { get; set; }
        [MaxLength(5000)]
        public string Description { get; set; }
        [MaxLength(10)]
        public string Kind { get; set; }
        [MaxLength(250)]
        public string Url { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
#endregion

#region ManyToManyTables
    //> Remove once EFCore has many to many
    //[ApiExplorerSettings(IgnoreApi = true)]
    [Table("Instrument_Sensor")]
    public class InstrumentSensor
    {
        public Guid InstrumentId { get; set; }
        public Guid SensorId { get; set; }
        // Navigation
        public Instrument Instrument { get; set; }
        public Sensor Sensor { get; set; }
    }

    //[ApiExplorerSettings(IgnoreApi = true)]
    [Table("Organisation_Instrument")]
    public class OrganisationInstrument : IDEntity
    {
        public Guid OrganisationId { get; set; }
        public Guid InstrumentId { get; set; }
        // Navigation
        public Organisation Organisation { get; set; }
        public Instrument Instrument { get; set; }
    }

    //[ApiExplorerSettings(IgnoreApi = true)]
    [Table("Organisation_Site")]
    public class OrganisationSite : IDEntity
    {
        public Guid OrganisationId { get; set; }
        public Guid SiteId { get; set; }
        // Navigation
        public Organisation Organisation { get; set; }
        public Site Site { get; set; }
    }

    //[ApiExplorerSettings(IgnoreApi = true)]
    [Table("Organisation_Station")]
    public class OrganisationStation : IDEntity
    {
        public Guid OrganisationId { get; set; }
        public Guid StationId { get; set; }
        // Navigation
        public Organisation Organisation { get; set; }
        public Station Station { get; set; }
    }

    //[ApiExplorerSettings(IgnoreApi = true)]
    [Table("PhenomenonOffering")]
    public class PhenomenonOffering : IDEntity
    {
        [Required]
        public Guid PhenomenonId { get; set; }
        [Required]
        public Guid OfferingId { get; set; }

        // Navigation
        public Phenomenon Phenomenon { get; set; }
        public Offering Offering { get; set; }
    }

    //[ApiExplorerSettings(IgnoreApi = true)]
    [Table("PhenomenonUOM")]
    public class PhenomenonUnit : IDEntity
    {
        [Required]
        public Guid PhenomenonId { get; set; }
        [Required, Column("UnitOfMeasureID")]
        public Guid UnitId { get; set; }

        // Navigation
        public Phenomenon Phenomenon { get; set; }
        public Unit Unit { get; set; }
    }

    //[ApiExplorerSettings(IgnoreApi = true)]
    [Table("Project_Station")]
    public class ProjectStation : IDEntity
    {
        public Guid ProjectId { get; set; }
        public Guid StationId { get; set; }
        // Navigation
        public Project Project { get; set; }
        public Station Station { get; set; }
    }

    //[ApiExplorerSettings(IgnoreApi = true)]
    [Table("Station_Instrument")]
    public class StationInstrument : IDEntity
    {
        public Guid StationId { get; set; }
        public Guid InstrumentId { get; set; }
        // Navigation
        public Station Station { get; set; }
        public Instrument Instrument { get; set; }
    }
    //< Remove once EFCore has many to many
#endregion

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
#if NET472
        public ObservationsDbContext(string tenantConnectionString) : base(tenantConnectionString)
        {
            //Configuration.ProxyCreationEnabled = false;
            //Configuration.LazyLoadingEnabled = false;
            //Configuration.AutoDetectChangesEnabled = false;
            var logLevel = ConfigurationManager.AppSettings["EntityFrameworkLogging"] ?? "Information";
            if (logLevel.Equals("Verbose", StringComparison.CurrentCultureIgnoreCase))
                Database.Log = s => Logging.Verbose(s);
            //else
            //    Database.Log = s => Logging.Information(s);
            Database.CommandTimeout = 30 * 60;
        }
#else
        private readonly IConfiguration _config;
        private readonly ILogger<ObservationsDbContext> _logger;
        private readonly IHttpContextAccessor _httpContectAccessor;

        public ObservationsDbContext(DbContextOptions<ObservationsDbContext> options, IConfiguration config, ILogger<ObservationsDbContext> logger, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _config = config;
            _logger = logger;
            _httpContectAccessor = httpContextAccessor;
            Database.SetCommandTimeout(30 * 60);
        }
#endif

        public DbSet<DataSchema> DataSchemas { get; set; }
        public DbSet<DataSource> DataSources { get; set; }
        public DbSet<DataSourceType> DataSourceTypes { get; set; }
        public DbSet<DigitalObjectIdentifier> DigitalObjectIdentifiers { get; set; }
        public DbSet<Instrument> Instruments { get; set; }
        //public DbSet<InventoryTotal> InventoryTotals { get; set; }
        //public DbSet<InventoryStation> InventoryStations { get; set; }
        //public DbSet<InventoryInstrument> InventoryInstruments { get; set; }
        //public DbSet<InventoryPhenomenonOffering> InventoryPhenomenaOfferings { get; set; }
        //public DbSet<InventoryYear> InventoryYears { get; set; }
        //public DbSet<InventoryOrganisation> InventoryOrganisations { get; set; }
        public DbSet<Offering> Offerings { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<OrganisationRole> OrganisationRoles { get; set; }
        public DbSet<Phenomenon> Phenomena { get; set; }
        public DbSet<PhenomenonOffering> PhenomenonOfferings { get; set; }
        public DbSet<PhenomenonUnit> PhenomenonUnits { get; set; }
        public DbSet<Programme> Programmes { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<UserDownload> UserDownloads { get; set; }
        public DbSet<UserQuery> UserQueries { get; set; }

        public DbSet<ImportBatchSummary> ImportBatchSummary { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<Observation> Observations { get; set; }

        // SensorThings
        public DbSet<SensorThingsDatastream> SensorThingsDatastreams { get; set; }
        public DbSet<SensorThingsLocation> SensorThingsLocations { get; set; }
        public DbSet<SensorThingsObservedPropery> SensorThingsObservedProperies { get; set; }
        public DbSet<SensorThingsSensor> SensorThingsSensors { get; set; }
        public DbSet<SensorThingsThing> SensorThingsThings { get; set; }

        //public DbSet<vApiDataDownload> vApiDataDownloads { get; set; }
        //public DbSet<vApiDataQuery> vApiDataQueries { get; set; }
        //public DbSet<vApiInventory> vApiInventory { get; set; }
        //public DbSet<vApiSpacialCoverage> vApiSpacialCoverages { get; set; }
        //public DbSet<vApiTemporalCoverage> vApiTemporalCoverages { get; set; }
        //public DbSet<vSensorThingsDatastream> vSensorThingsDatastreams { get; set; }

#if NET472
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<Instrument>()
                .HasMany<Sensor>(l => l.Sensors)
                .WithMany(r => r.Instruments)
                .Map(cs =>
                {
                    cs.MapLeftKey("InstrumentID");
                    cs.MapRightKey("SensorID");
                    cs.ToTable("Instrument_Sensor");
                });
            modelBuilder.Entity<Organisation>()
                .HasMany<Site>(l => l.Sites)
                .WithMany(r => r.Organisations)
                .Map(cs =>
                {
                    cs.MapLeftKey("OrganisationID");
                    cs.MapRightKey("SiteID");
                    cs.ToTable("Organisation_Site");
                });
            modelBuilder.Entity<Organisation>()
                .HasMany<Station>(l => l.Stations)
                .WithMany(r => r.Organisations)
                .Map(cs =>
                {
                    cs.MapLeftKey("OrganisationID");
                    cs.MapRightKey("StationID");
                    cs.ToTable("Organisation_Station");
                });
            modelBuilder.Entity<Organisation>()
                .HasMany<Instrument>(l => l.Instruments)
                .WithMany(r => r.Organisations)
                .Map(cs =>
                {
                    cs.MapLeftKey("OrganisationID");
                    cs.MapRightKey("InstrumentID");
                    cs.ToTable("Organisation_Instrument");
                });
            //modelBuilder.Entity<Phenomenon>()
            //    .HasMany<Offering>(l => l.Offerings)
            //    .WithMany(r => r.Phenomena)
            //    .Map(cs =>
            //    {
            //        cs.MapLeftKey("PhenomenonID");
            //        cs.MapRightKey("OfferingID");
            //        cs.ToTable("PhenomenonOffering");
            //    });
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
            modelBuilder.Entity<Unit>().Property(p => p.Name).HasColumnName("Unit");
            //modelBuilder.Entity<Phenomenon>()
            //    .HasMany<Unit>(l => l.Units)
            //    .WithMany(r => r.Phenomena)
            //    .Map(cs =>
            //    {
            //        cs.MapLeftKey("PhenomenonID");
            //        cs.MapRightKey("UnitOfMeasureID");
            //        cs.ToTable("PhenomenonUOM");
            //    });
        }
#else
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            using (_logger.MethodCall(GetType()))
            {
                var tenant = TenantAuthorizationHandler.GetTenantFromHeaders(_httpContectAccessor.HttpContext.Request, _config);
                var connectionString = _config.GetConnectionString(tenant);
                _logger.LogDebug("Tenant: {Tenant} ConnectionString: {ConnectionString}", tenant, connectionString);
                optionsBuilder.UseSqlServer(connectionString);
                base.OnConfiguring(optionsBuilder);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Feature>().HasNoKey().ToView("vFeatures");
            modelBuilder.Entity<Location>().HasNoKey().ToView("vLocations");
            //> Remove once EFCore has many to many
            modelBuilder.Entity<InstrumentSensor>().HasKey(e => new { e.InstrumentId, e.SensorId });
            modelBuilder.Entity<InstrumentSensor>().HasOne(i => i.Instrument).WithMany(i => i.InstrumentSensors).HasForeignKey(pt => pt.InstrumentId);
            modelBuilder.Entity<InstrumentSensor>().HasOne(i => i.Sensor).WithMany(i => i.InstrumentSensors).HasForeignKey(pt => pt.SensorId);
            modelBuilder.Entity<OrganisationInstrument>().HasKey(e => new { e.OrganisationId, e.InstrumentId });
            modelBuilder.Entity<OrganisationInstrument>().HasOne(i => i.Organisation).WithMany(i => i.OrganisationInstruments).HasForeignKey(i => i.OrganisationId);
            modelBuilder.Entity<OrganisationInstrument>().HasOne(i => i.Instrument).WithMany(i => i.OrganisationInstruments).HasForeignKey(i => i.InstrumentId);
            modelBuilder.Entity<OrganisationSite>().HasKey(e => new { e.OrganisationId, e.SiteId });
            modelBuilder.Entity<OrganisationSite>().HasOne(i => i.Organisation).WithMany(i => i.OrganisationSites).HasForeignKey(i => i.OrganisationId);
            modelBuilder.Entity<OrganisationSite>().HasOne(i => i.Site).WithMany(i => i.OrganisationSites).HasForeignKey(i => i.SiteId);
            modelBuilder.Entity<OrganisationStation>().HasKey(e => new { e.OrganisationId, e.StationId });
            modelBuilder.Entity<OrganisationStation>().HasOne(i => i.Organisation).WithMany(i => i.OrganisationStations).HasForeignKey(i => i.OrganisationId);
            modelBuilder.Entity<OrganisationStation>().HasOne(i => i.Station).WithMany(i => i.OrganisationStations).HasForeignKey(i => i.StationId);
            modelBuilder.Entity<PhenomenonOffering>().HasKey(e => new { e.PhenomenonId, e.OfferingId });
            modelBuilder.Entity<PhenomenonOffering>().HasOne(i => i.Phenomenon).WithMany(i => i.PhenomenonOfferings).HasForeignKey(i => i.PhenomenonId);
            modelBuilder.Entity<PhenomenonOffering>().HasOne(i => i.Offering).WithMany(i => i.PhenomenonOfferings).HasForeignKey(i => i.OfferingId);
            modelBuilder.Entity<PhenomenonUnit>().HasKey(e => new { e.PhenomenonId, e.UnitId });
            modelBuilder.Entity<PhenomenonUnit>().HasOne(i => i.Phenomenon).WithMany(i => i.PhenomenonUnits).HasForeignKey(i => i.PhenomenonId);
            modelBuilder.Entity<PhenomenonUnit>().HasOne(i => i.Unit).WithMany(i => i.PhenomenonUnits).HasForeignKey(i => i.UnitId);
            modelBuilder.Entity<ProjectStation>().HasKey(e => new { e.ProjectId, e.StationId });
            modelBuilder.Entity<ProjectStation>().HasOne(i => i.Project).WithMany(i => i.ProjectStations).HasForeignKey(i => i.ProjectId);
            modelBuilder.Entity<ProjectStation>().HasOne(i => i.Station).WithMany(i => i.ProjectStations).HasForeignKey(i => i.StationId);
            modelBuilder.Entity<StationInstrument>().HasKey(e => new { e.StationId, e.InstrumentId });
            modelBuilder.Entity<StationInstrument>().HasOne(i => i.Station).WithMany(i => i.StationInstruments).HasForeignKey(i => i.StationId);
            modelBuilder.Entity<StationInstrument>().HasOne(i => i.Instrument).WithMany(i => i.StationInstruments).HasForeignKey(i => i.InstrumentId);
            //< Remove once EFCore has many to many
        }
#endif
    }
}
