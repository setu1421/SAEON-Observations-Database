--> Added 2.0.32 20170527 TimPN
CREATE VIEW [dbo].[vApiTemporalCoverage]
AS
Select
  Observation.ID,
  Site.Name SiteName,
  Station.ID StationID,
  Station.Name StationName,
  Phenomenon.ID PhenomenonID,
  Phenomenon.Code PhenomenonCode,
  Phenomenon.Name PhenomenonName,
  PhenomenonOffering.ID PhenomenonOfferingID,
  Offering.ID OfferingID,
  Offering.Code OfferingCode,
  Offering.Name OfferingName,
  PhenomenonUOM.ID PhenomenonUnitOfMeasureID,
  UnitOfMeasure.ID UnitOfMeasureID,
  UnitOfMeasure.Code UnitOfMeasureCode,
  UnitOfMeasure.Unit UnitOfMeasureUnit,
  UnitOfMeasure.UnitSymbol UnitOfMeasureSymbol,
  Phenomenon.Name + ', ' + Offering.Name + ', ' + UnitOfMeasure.UnitSymbol FeatureCaption,
  Replace(Phenomenon.Name + '_' + Offering.Name + '_' + UnitOfMeasure.Unit,' ','') FeatureName,
  Observation.ValueDate,
  Observation.ValueDay,
  Observation.DataValue Value,
  Status = case 
    when Status.Name is Null then 'No Status'
	when Status.Name = 'Verified' then Status.Name
	when Status.Name = 'Unverified' then Status.Name
	else 'Being Verified'
  end 
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
       ((Instrument_Sensor.StartDate is null) or (Observation.ValueDay >= Instrument_Sensor.StartDate)) and
       ((Instrument_Sensor.EndDate is null) or (Observation.ValueDay <= Instrument_Sensor.EndDate))
  inner join Instrument
    on (Instrument_Sensor.InstrumentID = Instrument.ID) and
       ((Instrument.StartDate is null) or (Observation.ValueDay >= Instrument.StartDate )) and
       ((Instrument.EndDate is null) or (Observation.ValueDay <= Instrument.EndDate))
  inner join Station_Instrument
    on (Station_Instrument.InstrumentID = Instrument.ID) and
       ((Station_Instrument.StartDate is null) or (Observation.ValueDay >= Station_Instrument.StartDate)) and
       ((Station_Instrument.EndDate is null) or (Observation.ValueDay <= Station_Instrument.EndDate))
  inner join Station
    on (Station_Instrument.StationID = Station.ID) and
       ((Station.StartDate is null) or (Observation.ValueDay = Station.StartDate)) and
       ((Station.EndDate is null) or (Observation.ValueDay <= Station.EndDate))
  inner join Site
    on (Station.SiteID = Site.ID) and
       ((Site.StartDate is null) or (Observation.ValueDay >= Site.StartDate)) and
       ((Site.EndDate is null) or (Observation.ValueDay <= Site.EndDate))
  left join Status
    on (Observation.StatusID = Status.ID)
--> Added 2.0.32 20170527 TimPN
