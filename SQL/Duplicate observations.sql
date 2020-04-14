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
Select 
  ImportBatch.Code, o.* 
from 
  Duplicates d
  inner join Observation o
    on (d.SensorID = o.SensorID) and
	   (d.ValueDate = o.ValueDate) and
	   (d.PhenomenonOfferingID = o.PhenomenonOfferingID) and
	   (d.PhenomenonUOMID = o.PhenomenonUOMID) and
	   (((d.Elevation is null) and (o.Elevation is null)) or (d.Elevation = o.Elevation))
  inner join ImportBatch
    on (o.ImportBatchID = ImportBatch.ID)
order by 
  SensorID, ValueDate, PhenomenonOfferingID, PhenomenonUOMID, Elevation
--order by
--  ImportBatchCode, SiteName, StationName, InstrumentName, SensorName, ValueDate, DataValue, OfferingName, UnitOfMeasureUnit, Elevation