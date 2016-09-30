Delete
  Observation
from
(
Select
  obs.ID, ROW_NUMBER() over(Partition By obs.SensorID, obs.ImportBatchID, obs.PhenomenonOfferingID, obs.PhenomenonUOMID, obs.ValueDate order by AddedDate) RowNumber
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
) dups
inner join Observation
  on (Observation.ID = dups.ID)
where
  (dups.RowNumber > 1)

Delete
  RoleModule
from
(
Select
  rm.ID, ROW_NUMBER() over(Partition By rm.RoleId, rm.ModuleID order by ID) RowNumber
from
	(
	select
	  RoleID, ModuleID, Count(*) Count
	from
	  RoleModule 
	group by
	  RoleID, ModuleID
	having 
	  (Count(*) > 1)
	) Bad
	inner join RoleModule rm
	  on (rm.RoleID = Bad.RoleID) and
		 (rm.ModuleID = Bad.ModuleID)
) dups
inner join RoleModule
  on (RoleModule.ID = dups.ID)
where
  (dups.RowNumber > 1)
