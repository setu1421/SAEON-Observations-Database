--> Added 2.0.38 20180418 TimPN
CREATE VIEW [dbo].[vObservationExpansion]
AS
Select
  Observation.ID, Observation.ImportBatchID,  
  Observation.ValueDate, Observation.ValueDay, Observation.ValueYear, Observation.ValueDecade, 
  Observation.RawValue, Observation.DataValue, Observation.TextValue, 
  Observation.Comment, Observation.CorrelationID,
  Site.ID SiteID, Site.Code SiteCode, Site.Name SiteName, Site.Description SiteDescription, Site.Url SiteUrl,
  Station.ID StationID, Station.Code StationCode, Station.Name StationName, Station.Description StationDescription, Station.Url StationUrl,
  Instrument.ID InstrumentID, Instrument.Code InstrumentCode, Instrument.Name InstrumentName, Instrument.Description InstrumentDescription, Instrument.Url InstrumentUrl,
  Observation.SensorID, Sensor.Code SensorCode, Sensor.Name SensorName, Sensor.Description SensorDescription, Sensor.Url SensorUrl,
  Coalesce(Observation.Latitude, Sensor.Latitude, Instrument_Sensor.Latitude, Instrument.Latitude, Station_Instrument.Latitude, Station.Latitude) Latitude,
  Coalesce(Observation.Longitude, Sensor.Longitude, Instrument_Sensor.Longitude, Instrument.Longitude, Station_Instrument.Longitude, Station.Longitude) Longitude,
  Coalesce(Observation.Elevation, Sensor.Elevation, Instrument_Sensor.Elevation, Instrument.Elevation, Station_Instrument.Elevation, Station.Elevation) Elevation,
  Observation.PhenomenonOfferingID, Phenomenon.ID PhenomenonID, Phenomenon.Code PhenomenonCode, Phenomenon.Name PhenomenonName, Phenomenon.Description PhenomenonDescription, Phenomenon.Url PhenomenonUrl,
  Offering.ID OfferingID, Offering.Code OfferingCode, Offering.Name OfferingName, Offering.Description OfferingDescription,
  Observation.PhenomenonUOMID, UnitOfMeasure.ID UnitOfMeasureID, UnitOfMeasure.Code UnitOfMeasureCode, UnitOfMeasure.Unit UnitOfMeasureUnit, UnitOfMeasure.UnitSymbol UnitOfMeasureSymbol,
  Observation.StatusID, Status.Code StatusCode, Status.Name StatusName, Status.Description StatusDescription,
  Observation.StatusReasonID, StatusReason.Code StatusReasonCode, StatusReason.Name StatusReasonName, StatusReason.Description StatusReasonDescription
from
  Observation
  inner join Sensor
    on (Observation.SensorID = Sensor.ID)
  inner join Instrument_Sensor
    on (Observation.SensorID = Instrument_Sensor.SensorID) and
       ((Instrument_Sensor.StartDate is null) or (Observation.ValueDay >= Instrument_Sensor.StartDate)) and
       ((Instrument_Sensor.EndDate is null) or (Observation.ValueDay <= Instrument_Sensor.EndDate))
  inner join Instrument
    on (Instrument_Sensor.InstrumentID = Instrument.ID) and
       ((Instrument.StartDate is null) or (Observation.ValueDay >= Instrument.StartDate)) and
       ((Instrument.EndDate is null) or (Observation.ValueDay <= Instrument.EndDate))
  inner join Station_Instrument
    on (Station_Instrument.InstrumentID = Instrument_Sensor.InstrumentID) and
       ((Station_Instrument.StartDate is null) or (Observation.ValueDay >= Station_Instrument.StartDate)) and
       ((Station_Instrument.EndDate is null) or (Observation.ValueDay <= Station_Instrument.EndDate))
  inner join Station
    on (Station_Instrument.StationID = Station.ID) and
       ((Station.StartDate is null) or (Observation.ValueDay >= Station.StartDate))  and
       ((Station.EndDate is null) or (Observation.ValueDay <= Station.EndDate))
  inner join Site
    on (Station.SiteID = Site.ID) and
       ((Site.StartDate is null) or (Observation.ValueDay >= Site.StartDate)) and
       ((Site.EndDate is null) or (Observation.ValueDay <= Site.EndDate))
  inner join PhenomenonOffering
    on (Observation.PhenomenonOfferingID = PhenomenonOffering.ID)
  inner join Phenomenon
    on (PhenomenonOffering.PhenomenonID = Phenomenon.ID)
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
  inner join PhenomenonUOM
    on (Observation.PhenomenonUOMID = PhenomenonUOM.ID)
  inner join UnitOfMeasure
    on (PhenomenonUOM.UnitOfMeasureID = UnitOfMeasure.ID)
  left join Status
    on (Observation.StatusID = Status.ID)
  left join StatusReason
    on (Observation.StatusReasonID = StatusReason.ID)
--< Added 2.0.38 20180418 TimPN

