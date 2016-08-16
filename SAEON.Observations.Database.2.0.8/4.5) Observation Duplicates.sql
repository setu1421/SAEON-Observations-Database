Select
  obs.*, ROW_NUMBER() over(Partition By obs.SensorID, obs.ImportBatchID, obs.PhenomenonOfferingID, obs.PhenomenonUOMID, obs.ValueDate order by AddedDate) RowNumber
from
	(
	select
	  [SensorID], [ImportBatchID], [PhenomenonOfferingID], [PhenomenonUOMID], [ValueDate], Count(*) Count
	from
	  Observation 
	group by
	  [SensorID], [ImportBatchID], [PhenomenonOfferingID], [PhenomenonUOMID], [ValueDate]
	having 
	  (Count(*) > 1)
	) Bad
	inner join Observation obs
	  on (obs.SensorID = Bad.SensorID) and
		 (obs.ImportBatchID = Bad.ImportBatchID) and
		 (obs.PhenomenonOfferingID = Bad.PhenomenonOfferingID) and
		 (obs.PhenomenonUOMID = bad.PhenomenonUOMID) and
		 (obs.ValueDate = bad.ValueDate)
order by
  [SensorID], [ImportBatchID], [PhenomenonOfferingID], [PhenomenonUOMID], [ValueDate]