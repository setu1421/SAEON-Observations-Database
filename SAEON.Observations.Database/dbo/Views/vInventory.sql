--> Changed 2.0.3 20160503 TimPN
--Renamed SensorProcedure to Sensor
--< Changed 2.0.3 20160503 TimPN
--> Removed 2.0.17 20161128 TimPN
--CREATE VIEW [dbo].[vInventory]
--AS
--Select 
-- ps.Name Site,
-- s.Name Station,
-- sp.Name Sensor,
-- p.Name Phenomenon,
-- d.StartDate,
-- d.EndDate
--FROM Station s with (nolock)
-- INNER Join ProjectSite ps with (nolock)
-- on  ps.ID=  s.ProjectSiteID
--INNER Join Sensor sp with (nolock)
-- on s.ID = sp.StationID
--INNER Join Phenomenon p with (nolock)
-- on  sp.PhenomenonID = p.ID 

--INNER JOIN 
--(
-- SELECT SensorID,MIN(ValueDate) StartDate,MAX(ValueDate) EndDate
--  FROM Observation with (nolock)
-- Group By SensorID
--)d
--ON sp.ID = d.SensorID
--< Removed 2.0.17 20161128 TimPN
--> Added 2.0.17 20161128 TimPN
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
     SELECT SensorID,MIN(ValueDate) StartDate,MAX(ValueDate) EndDate
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
	   ((Station.StartDate is null) or (Cast(d.StartDate as Date) >= Cast(Station.StartDate as Date))) and
	   ((Station.EndDate is null) or (Cast(d.EndDate as Date) <= Cast(Station.EndDate as Date)))
  inner join Site
    on (Station.SiteID = Site.ID) and
	   ((Site.StartDate is null) or  (Cast(d.StartDate as Date) >= Cast(Site.StartDate as Date))) and
	   ((Site.EndDate is null) or  (Cast(d.EndDate as Date) <= Cast(Site.EndDate as Date)))
  inner join Phenomenon p 
   on (Sensor.PhenomenonID = p.ID )
--< Added 2.0.17 20161128 TimPN
