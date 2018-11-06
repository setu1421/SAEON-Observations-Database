use Observations
/*
Select
  SensorID, ValueDate, PhenomenonOfferingID, PhenomenonUOMID, DataValue, Count(ID) Count
from
  Observation
group by
  SensorID, ValueDate, PhenomenonOfferingID, PhenomenonUOMID, DataValue
having
  (Count(ID) > 1)
order by
  SensorID, ValueDate, PhenomenonOfferingID, PhenomenonUOMID, DataValue
*/

Select
  ImportBatch.Code, o1.* 
from
  Observation o1
  inner join ImportBatch
    on (o1.ImportBatchID = ImportBatch.ID)
  inner join
	(
	Select
	  SensorID, ValueDate, PhenomenonOfferingID, PhenomenonUOMID, DataValue, Count(ID) Count
	from
	  Observation
	group by
	  SensorID, ValueDate, PhenomenonOfferingID, PhenomenonUOMID, DataValue
	having
	  (Count(ID) > 1)
	) as o2
  on (o1.SensorID = o2.SensorID) and
	 (o1.ValueDate = o2.ValueDate) and
	 (o1.PhenomenonOfferingID = o2.PhenomenonOfferingID) and
	 (o1.PhenomenonUOMID = o2.PhenomenonUOMID) 
order by
  SensorID, ValueDate, PhenomenonOfferingID, PhenomenonUOMID
