use Observations;
go
Create View NewDepthPhenomenonOfferings as
--Alter View NewDepthPhenomenonOfferings as
Select
  PhenomenonOffering.ID NewPhenomenonOfferingID, OldPhenomenonOfferingID,
  case Source
    when 'At' then Abs(Convert(float, Replace(Replace(Left(OldOfferingName,CharIndex(' at ',OldOfferingName)),'Depth ',''),'M','')))*-1.0 
	when 'Interval' then Replace(SubString(OldOfferingName,CharIndex(', Ht ',OldOfferingName)+5,1000),'m','')
	else null
  end Depth
from
  NewDepthOfferings
  inner join PhenomenonOffering
    on (PhenomenonOffering.PhenomenonID = NewDepthOfferings.PhenomenonID) and
	   (PhenomenonOffering.OfferingID = (Select ID from Offering where Code = NewDepthOfferings.NewOfferingCode))