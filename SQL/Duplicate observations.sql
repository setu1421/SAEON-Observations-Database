with Duplicates
as
(
Select
  SensorID, ValueDate, PhenomenonOfferingID, PhenomenonUOMID, Elevation, Count(*) Count
from 
  Observation
group by
  SensorID, ValueDate, PhenomenonOfferingID, PhenomenonUOMID, Elevation
having
  (Count(*) > 1)
)
Select * from Duplicates order by SensorID, ValueDate, PhenomenonOfferingID, PhenomenonUOMID, Elevation
--Select 
--  o.* 
--from 
--  Duplicates d
--  inner join vObservationExpansion o
--    on (d.SensorID = o.SensorID) and
--	   (d.ValueDate = o.ValueDate) and
--	   (d.PhenomenonOfferingID = o.PhenomenonOfferingID) and
--	   (d.PhenomenonUOMID = o.PhenomenonUOMID) and
--	   (((d.Elevation is null) and (o.Elevation is null)) or (d.Elevation = o.Elevation))
--order by
--  ImportBatchCode, SiteName, StationName, InstrumentName, SensorName, ValueDate, DataValue, OfferingName, UnitOfMeasureUnit, Elevation