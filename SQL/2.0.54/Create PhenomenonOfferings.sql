with DepthOfferings
as
(
Select distinct
  Phenomenon.Name Phenomenon, 
  SubString(Offering.Name,CharIndex(' at ',Offering.Name),10000) Offering, 
  Replace(SubString(Offering.Name,CharIndex(' at ',Offering.Name)+4,10000),' Interval','s') Interval,
  'Actual_'+Replace(Replace(SubString(Offering.Name,CharIndex(' at ',Offering.Name)+4,10000),' Interval','s'),' ','_') Code,
  'Actual '+Replace(SubString(Offering.Name,CharIndex(' at ',Offering.Name)+4,10000),' Interval','s') Name,
  'Actual '+SubString(Offering.Name,CharIndex(' at ',Offering.Name),10000) Description
from
  PhenomenonOffering
  inner join Phenomenon
    on (PhenomenonOffering.PhenomenonID = Phenomenon.ID)
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
where
  (CharIndex(' at ',Offering.Name) > 0)
)

Insert into PhenomenonOffering
  (PhenomenonID, OfferingID, UserId)
Select 
  (Select Id from Phenomenon where Name = DepthOfferings.Phenomenon) PhenomenonID,
  (Select Id from Offering where Name = DepthOfferings.Name) OfferingID,
  (Select UserId from aspnet_Users where UserName='TimPN')
from 
  DepthOfferings
where
  not exists(
	Select 
	  * 
	from 
	  PhenomenonOffering 
	where
	  (PhenomenonID = (Select Id from Phenomenon where Name = DepthOfferings.Phenomenon)) and
	  (OfferingID = (Select Id from Offering where Name = DepthOfferings.Name)))
