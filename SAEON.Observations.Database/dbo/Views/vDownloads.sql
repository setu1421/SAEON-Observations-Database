--> Added 2.0.26 20170130 TimPN
CREATE VIEW [dbo].[vDownloads]
AS
select top (1000)
  Site.Code SiteCode,
  Site.Name SiteName,
  Site.Description SiteDescription,
  Site.Url SiteUrl,
  Station.Code StationCode,
  Station.Name StationName,
  Station.Description StationDescription,
  Station.Url StationUrl,
  Station.Latitude StationLatitude,
  Station.Longitude StationLongitude,
  Station.Elevation StationElevation,
  Instrument.Code InstrumentCode,
  Instrument.Name InstrumentName,
  Instrument.Description InstrumentDescription,
  Instrument.Url InstrumentUrl,
  Sensor.Code SensorCode,
  Sensor.Name SensorName,
  Sensor.Description SensorDescription,
  Sensor.Url SensorUrl,
  Phenomenon.Code PhenomenonCode,
  Phenomenon.Name PhenomenonName,
  Phenomenon.Description PhenomenonDescription,
  Phenomenon.Url PhenomenonUrl,
  Offering.Code OfferingCode,
  Offering.Name OfferingName,
  Offering.Description OfferingDescription,
  UnitOfMeasure.Code UnitOfMeasureCode,
  UnitOfMeasure.Unit UnitOfMeasureUnit,
  UnitOfMeasure.UnitSymbol UnitOfMeasureSymbol,
  Observation.ValueDate,
  Observation.RawValue,
  Observation.DataValue,
  Observation.Comment,
  Observation.CorrelationID
from
  Observation
  inner join Sensor
    on (Observation.SensorID = Sensor.ID)
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
  inner join Instrument_Sensor
    on (Instrument_Sensor.SensorID = Sensor.ID) and
       ((Instrument_Sensor.StartDate is null) or (Cast(Observation.ValueDate as Date) >= Instrument_Sensor.StartDate)) and
       ((Instrument_Sensor.EndDate is null) or (Cast(Observation.ValueDate as Date) <= Instrument_Sensor.EndDate))
  inner join Instrument
    on (Instrument_Sensor.InstrumentID = Instrument.ID) and
       ((Instrument.StartDate is null) or (Cast(Observation.ValueDate as Date) >= Instrument.StartDate )) and
       ((Instrument.EndDate is null) or (Cast(Observation.ValueDate as Date) <= Instrument.EndDate))
  inner join Station_Instrument
    on (Station_Instrument.InstrumentID = Instrument.ID) and
       ((Station_Instrument.StartDate is null) or (Cast(Observation.ValueDate as Date) >= Station_Instrument.StartDate)) and
       ((Station_Instrument.EndDate is null) or (Cast(Observation.ValueDate as Date) <= Station_Instrument.EndDate))
  inner join Station
    on (Station_Instrument.StationID = Station.ID) and
       ((Station.StartDate is null) or (Cast(Observation.ValueDate as Date) >= Cast(Station.StartDate as Date))) and
       ((Station.EndDate is null) or (Cast(Observation.ValueDate as Date) <= Cast(Station.EndDate as Date)))
  inner join Site
    on (Station.SiteID = Site.ID) and
       ((Site.StartDate is null) or (Cast(Observation.ValueDate as Date) >= Cast(Site.StartDate as Date))) and
       ((Site.EndDate is null) or (Cast(Observation.ValueDate as Date) <= Cast(Site.EndDate as Date)))
where
  (Observation.StatusID = (Select ID from Status where Name = 'Verified'))
--< Added 2.0.26 20170130 TimPN
