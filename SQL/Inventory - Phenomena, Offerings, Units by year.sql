use Observations
Select
  Phenomenon.Name Phenomenon, Offering.Name Offering, UnitOfMeasure.Unit Unit, Site.Name Site, Station.Name Station, /*Status.Name Status,*/ Observation.ValueYear Year, Count(*) Count
from
  Observation
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
	   ((Station.StartDate is null) or (Observation.ValueDay >= Station.StartDate))	and
	   ((Station.EndDate is null) or (Observation.ValueDay <= Station.EndDate))
  inner join Site
    on (Station.SiteID = Site.ID) and
	   ((Site.StartDate is null) or (Observation.ValueDay >= Site.StartDate)) and
	   ((Site.EndDate is null) or (Observation.ValueDay <= Site.EndDate))
  left join Status
    on (Observation.StatusID = Status.ID)
where
  (Phenomenon.Name not in ('Current Direction','Current speed','Depth')) --and
  --(Status.Name = 'Verified')
group by
  Phenomenon.Name, Offering.Name, UnitOfMeasure.Unit, Site.Name, Station.Name, /*Status.Name,*/ Observation.ValueYear
order by
  Phenomenon.Name, Offering.Name, UnitOfMeasure.Unit, Site.Name, Station.Name, /*Status.Name,*/ Observation.ValueYear
