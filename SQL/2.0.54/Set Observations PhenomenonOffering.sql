use Observations;
with DepthOfferings
as
(
Select distinct
  OldPhenomenonOffering.ID OldPhenomenonOfferingID,
  OldPhenomenon.ID OldPhenomenonID, 
  OldPhenomenon.Code OldPhenomenonCode, 
  OldPhenomenon.Name OldPhenomenonName, 
  OldOffering.Code OldOfferingCode,
  OldOffering.Name OldOfferingName,
  Abs(Convert(float, Replace(Replace(Left(OldOffering.Name,CharIndex(' at ',OldOffering.Name)),'Depth ',''),'M','')))*-1.0 Depth,
  NewPhenomenonOffering.ID NewPhenomenonOfferingID,
  NewPhenomenon.Code NewPhenomenonCode, 
  NewPhenomenon.Name NewPhenomenonName, 
  NewOffering.Code NewOfferingCode,
  NewOffering.Name NewOfferingName
from
  PhenomenonOffering OldPhenomenonOffering
  inner join Phenomenon OldPhenomenon
    on (OldPhenomenonOffering.PhenomenonID = OldPhenomenon.ID)
  inner join Offering OldOffering
    on (OldPhenomenonOffering.OfferingID = OldOffering.ID)
  inner join PhenomenonOffering NewPhenomenonOffering
    on (NewPhenomenonOffering.PhenomenonID = OldPhenomenonOffering.PhenomenonID)
  inner join Phenomenon NewPhenomenon
    on (NewPhenomenonOffering.PhenomenonID = NewPhenomenon.ID)
  inner join Offering NewOffering
    on (NewPhenomenonOffering.OfferingID = NewOffering.ID)
where
  (CharIndex(' at ',OldOffering.Name) > 0) and
  (NewOffering.Code = Replace(Replace(Replace(
    'ACT_'+Replace(Replace(SubString(OldOffering.Name,CharIndex(' at ',OldOffering.Name)+4,10000),' Interval','s'),' ','_'),
	'_Hours','_Hr'),'_Minutes','_Min'),'_Seconds','_Sec'))
)

Update
  Observation
set
  PhenomenonOfferingID = NewPhenomenonOfferingID
from
  Observation
  inner join DepthOfferings
    on (Observation.PhenomenonOfferingID = DepthOfferings.OldPhenomenonOfferingID)
