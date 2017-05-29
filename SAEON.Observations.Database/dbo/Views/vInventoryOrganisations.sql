--> Added 20170523 2.0.32 TimPN
CREATE VIEW [dbo].[vInventoryOrganisations]
AS 
Select
  Coalesce(InstrumentOrganisation.Name, StationOrganisation.Name, SiteOrganisation.Name)+'~'+IsNull(Status.Name,'') SurrogateKey,
  --Station.ID StationID, PhenomenonOffering.ID PhenomenonOfferingID, 
  Coalesce(InstrumentOrganisation.Name, StationOrganisation.Name, SiteOrganisation.Name) Name, IsNull(Status.Name,'No status') Status, 
  Count(*) Count, Min(DataValue) Minimum, Max(DataValue) Maximum, Avg(DataValue) Average, StDev(DataValue) StandardDeviation, Var(DataValue) Variance
from
  Observation
  inner join Sensor
    on (Observation.SensorID = Sensor.ID)
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
  --inner join PhenomenonOffering
  --  on (Observation.PhenomenonOfferingID = PhenomenonOffering.ID)
  inner join Site
    on (Station.SiteID = Site.ID)
  left join Organisation_Site
    on (Organisation_Site.SiteID = Site.ID) and
       ((Organisation_Site.StartDate is null) or (Observation.ValueDay >= Instrument.StartDate )) and
       ((Organisation_Site.EndDate is null) or (Observation.ValueDay <= Instrument.EndDate))
  left join Organisation SiteOrganisation
    on (Organisation_Site.OrganisationID = SiteOrganisation.ID)  		
  left join Organisation_Station
    on (Organisation_Station.StationID = Station.ID) and
       ((Organisation_Station.StartDate is null) or (Observation.ValueDay >= Instrument.StartDate )) and
       ((Organisation_Station.EndDate is null) or (Observation.ValueDay <= Instrument.EndDate))
  left join Organisation StationOrganisation
    on (Organisation_Station.OrganisationID = StationOrganisation.ID)  		
  left join Organisation_Instrument
    on (Organisation_Instrument.InstrumentID = Instrument.ID) and
       ((Organisation_Instrument.StartDate is null) or (Observation.ValueDay >= Instrument.StartDate )) and
       ((Organisation_Instrument.EndDate is null) or (Observation.ValueDay <= Instrument.EndDate))
  left join Organisation InstrumentOrganisation
    on (Organisation_Instrument.OrganisationID = InstrumentOrganisation.ID)  		
  left join Status
    on (Observation.StatusID = Status.ID)
group by
  Coalesce(InstrumentOrganisation.Name, StationOrganisation.Name, SiteOrganisation.Name),
  Status.Name
--< Added 20170523 2.0.32 TimPN

