with Duplicates
as
(
Select
  SensorID, ValueDate, PhenomenonOfferingID, PhenomenonUOMID, Elevation,
  Row_Number() over 
	(partition by SensorID, ValueDate, PhenomenonOfferingID, PhenomenonUOMID, Elevation 
	 order by SensorID, ValueDate, PhenomenonOfferingID, PhenomenonUOMID, Elevation) RowNum 
from 
  Observation
)
--select * from Duplicates where RowNum > 1
delete Duplicates where RowNum > 1