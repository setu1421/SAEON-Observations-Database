use Observations;
with DepthOfferings
as
(
Select distinct
  Phenomenon.Code PhenomenonCode, 
  Phenomenon.Name PhenomenonName, 
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

Select * from DepthOfferings --order by OldPhenomenonCode, OldOfferingCode
