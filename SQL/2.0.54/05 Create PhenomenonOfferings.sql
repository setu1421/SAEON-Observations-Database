use Observations;
Insert into PhenomenonOffering
  (PhenomenonID, OfferingID, UserId)
Select distinct
  PhenomenonOffering.PhenomenonID,
  (Select Id from Offering where Code = NewOfferingCode) OfferingID,
  (Select UserId from aspnet_Users where UserName='TimPN')
from 
  vNewDepthOfferings
  inner join PhenomenonOffering
    on (PhenomenonOffering.OfferingID = OldOfferingID)
where
  not exists(
	Select 
	  * 
	from 
	  PhenomenonOffering 
	where
	  (PhenomenonID = PhenomenonOffering.PhenomenonID) and
	  (OfferingID = (Select ID from Offering where Code = NewOfferingCode)))
