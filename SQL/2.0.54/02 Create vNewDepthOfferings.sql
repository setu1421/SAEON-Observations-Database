use Observations;
Go
Create View vNewDepthOfferings as
--Alter View vNewDepthOfferings as
Select distinct
  OldOfferingID,
  OldOfferingCode,
  OldOfferingName,
  OldOfferingDescription,
  Source,
  Replace(Replace(Replace(
    'ACT_'+Replace(Replace(SubString(OldOfferingName,CharIndex(' at ',OldOfferingName)+4,10000),' Interval','s'),' ','_'),
	'_Hours','_Hr'),'_Minutes','_Min'),'_Seconds','_Sec') NewOfferingCode,
  Replace(Replace(Replace('Actual '+Replace(SubString(OldOfferingName,CharIndex(' at ',OldOfferingName)+4,10000),' Interval','s'),
    ' 1 Hours',' 1 Hour'),' Minutes',' Minute'),' Seconds',' Second') NewOfferingName,
  'Actual '+SubString(OldOfferingName,CharIndex(' at ',OldOfferingName)+1,10000) NewOfferingDescription
from
  vOldDepthOfferings
where
  (Source = 'at')
union
Select distinct
  OldOfferingID,
  OldOfferingCode,
  OldOfferingName,
  OldOfferingDescription,
  Source,
  'ACT_'+Replace(SubString(OldOfferingName,1,CharIndex('min,',OldOfferingName)-1),'Interval ','')+'_Min' NewOfferingCode,
  'Actual '+Replace(SubString(OldOfferingName,1,CharIndex('min,',OldOfferingName)-1),'Interval ','')+' Minute' NewOfferingName,
  'Actual at '+Replace(SubString(OldOfferingName,1,CharIndex('min,',OldOfferingName)-1),'Interval ','')+' Minute Interval' NewOfferingDescription
from
  vOldDepthOfferings
where
  (Source = 'interval')
