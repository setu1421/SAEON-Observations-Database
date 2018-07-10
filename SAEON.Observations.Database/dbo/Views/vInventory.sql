CREATE VIEW [dbo].[vInventory]
AS
Select 
  Site.Name Site,
  Station.Name Station,
  Instrument.Name Instrument,
  Sensor.Name Sensor,
  p.Name Phenomenon,
  d.StartDate,
  d.EndDate
from
  Sensor 
  INNER JOIN 
  (
     SELECT SensorID,MIN(Cast(ValueDate as Date)) StartDate,MAX(Cast(ValueDate as Date)) EndDate
     FROM Observation
     Group By SensorID
  ) d
    ON (Sensor.ID = d.SensorID)
  inner join Instrument_Sensor
    on (Instrument_Sensor.SensorID = Sensor.ID) and
	   ((Instrument_Sensor.StartDate is null) or (d.StartDate >= Instrument_Sensor.StartDate)) and
	   ((Instrument_Sensor.EndDate is null) or (d.EndDate <= Instrument_Sensor.EndDate))
  inner join Instrument
    on (Instrument_Sensor.InstrumentID = Instrument.ID) and
	   ((Instrument.StartDate is null) or (d.StartDate >= Instrument.StartDate )) and
	   ((Instrument.EndDate is null) or (d.EndDate <= Instrument.EndDate))
  inner join Station_Instrument
    on (Station_Instrument.InstrumentID = Instrument.ID) and
	   ((Station_Instrument.StartDate is null) or (d.StartDate >= Station_Instrument.StartDate)) and
	   ((Station_Instrument.EndDate is null) or (d.EndDate <= Station_Instrument.EndDate))
  inner join Station 
    on (Station_Instrument.StationID = Station.ID) and
	   ((Station.StartDate is null) or (d.StartDate >= Station.StartDate)) and
	   ((Station.EndDate is null) or (d.EndDate <= Station.EndDate))
  inner join Site
    on (Station.SiteID = Site.ID) and
	   ((Site.StartDate is null) or  (d.StartDate >= Site.StartDate)) and
	   ((Site.EndDate is null) or  (d.EndDate <= Site.EndDate))
  inner join Phenomenon p 
   on (Sensor.PhenomenonID = p.ID )
