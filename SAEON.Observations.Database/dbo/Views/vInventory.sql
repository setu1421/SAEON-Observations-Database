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
  inner join Instrument_Sensor
	on (Instrument_Sensor.SensorID = Sensor.ID)
  inner join Instrument
	on (Instrument_Sensor.InstrumentID = Instrument.ID)
  inner join Station_Instrument
    on (Station_Instrument.InstrumentID = Instrument.ID)
  inner join Station 
    on (Station_Instrument.StationID = Station.ID)
  inner join Site
    on (Station.SiteID = Site.ID)
  inner join Phenomenon p 
   on (Sensor.PhenomenonID = p.ID )

INNER JOIN 
(
 SELECT SensorID,MIN(ValueDate) StartDate,MAX(ValueDate) EndDate
  FROM Observation with (nolock)
 Group By SensorID
)d
ON (Sensor.ID = d.SensorID)
--< Added 2.0.17 20161128 TimPN
