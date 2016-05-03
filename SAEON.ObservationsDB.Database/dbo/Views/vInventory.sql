--> Changed 2.0.0.3 20160503 TimPN
--Renamed SensorProcedure to Sensor
--< Changed 2.0.0.3 20160503 TimPN
CREATE VIEW [dbo].[vInventory]
AS
Select 
 ps.Name Site,
 s.Name Station,
 sp.Name Sensor,
 p.Name Phenomenon,
 d.StartDate,
 d.EndDate
FROM Station s with (nolock)
 INNER Join ProjectSite ps with (nolock)
 on  ps.ID=  s.ProjectSiteID
INNER Join Sensor sp with (nolock)
 on s.ID = sp.StationID
INNER Join Phenomenon p with (nolock)
 on  sp.PhenomenonID = p.ID 

INNER JOIN 
(
 SELECT SensorID,MIN(ValueDate) StartDate,MAX(ValueDate) EndDate
  FROM Observation with (nolock)
 Group By SensorID
)d
ON sp.ID = d.SensorID
