use Observations;
Insert into PhenomenonOffering
  (PhenomenonID, OfferingID, UserId)
Select distinct
  NewDepthOfferings.PhenomenonID,
  (Select Id from Offering where Code = NewOfferingCode) OfferingID,
  (Select UserId from aspnet_Users where UserName='TimPN')
from 
  NewDepthOfferings
where
  not exists(
	Select 
	  * 
	from 
	  PhenomenonOffering 
	where
	  (PhenomenonID = NewDepthOfferings.PhenomenonID) and
	  (OfferingID = (Select ID from Offering where Code = NewOfferingCode)))
