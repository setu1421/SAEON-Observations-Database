use Observations;
with DepthOfferings
as
(
Select distinct
  Phenomenon.Code PhenomenonCode, 
  Phenomenon.Name PhenomenonName, 
  --Offering.Code OldOfferingCode,
  --Offering.Name OldOfferingName,
  --Abs(Convert(float, Replace(Replace(Left(Offering.Name,CharIndex(' at ',Offering.Name)),'Depth ',''),'M','')))*-1.0 Depth,
  Replace(Replace(Replace(
    'ACT_'+Replace(Replace(SubString(Offering.Name,CharIndex(' at ',Offering.Name)+4,10000),' Interval','s'),' ','_'),
	'_Hours','_Hr'),'_Minutes','_Min'),'_Seconds','_Sec') NewOfferingCode,
  Replace(Replace(Replace('Actual '+Replace(SubString(Offering.Name,CharIndex(' at ',Offering.Name)+4,10000),' Interval','s'),
    ' 1 Hours',' 1 Hour'),' Minutes',' Minute'),' Seconds',' Second') NewOfferingName,
  'Actual '+SubString(Offering.Name,CharIndex(' at ',Offering.Name),10000) NewOfferingDescription
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
  (Select Id from Phenomenon where Name = DepthOfferings.PhenomenonName) PhenomenonID,
  (Select Id from Offering where Name = DepthOfferings.NewOfferingName) OfferingID,
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
	  (PhenomenonID = (Select Id from Phenomenon where Name = DepthOfferings.PhenomenonName)) and
	  (OfferingID = (Select Id from Offering where Name = DepthOfferings.NewOfferingName)))
