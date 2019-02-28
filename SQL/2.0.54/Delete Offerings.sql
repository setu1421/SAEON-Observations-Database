with DepthOfferings
as
(
Select distinct
  Offering.ID,
  Replace(Replace(Replace(
    'ACT_'+Replace(Replace(SubString(Offering.Name,CharIndex(' at ',Offering.Name)+4,10000),' Interval','s'),' ','_'),
	'_Hours','_Hr'),'_Minutes','_Min'),'_Seconds','_Sec') NewOfferingCode,
  Replace(Replace(Replace('Actual '+Replace(SubString(Offering.Name,CharIndex(' at ',Offering.Name)+4,10000),' Interval','s'),
    ' 1 Hours',' 1 Hour'),' Minutes',' Minute'),' Seconds',' Second') NewOfferingName,
  'Actual '+SubString(Offering.Name,CharIndex(' at ',Offering.Name),10000) NewOfferingDescription
from
  Offering
where
  (CharIndex(' at ',Offering.Name) > 0)
)

Delete
  Offering
from
  Offering
  inner join DepthOfferings
    on (DepthOfferings.ID = Offering.ID)	
