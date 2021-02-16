using SAEON.Observations.Core;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SAEON.Observations.WebAPI
{
    /// <summary>
    /// OrganisationRole entity
    /// </summary>
    [Table("OrganisationRole")]
    public class OrganisationRole : NamedEntity
    {
        // Navigation
        //public List<Organisation> Organisations { get; set; }
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

}
