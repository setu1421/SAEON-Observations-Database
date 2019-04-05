use Observations;
go
Create View vNewDepthPhenomenonOfferings as
--Alter View vNewDepthPhenomenonOfferings as
Select 
  OldPhenomenonOfferingID, PhenomenonOffering.ID NewPhenomenonOfferingID,
    case vNewDepthOfferings.Source
    when 'At' then Abs(Convert(float, Replace(Replace(Left(vNewDepthOfferings.OldOfferingName,CharIndex(' at ',vNewDepthOfferings.OldOfferingName)),'Depth ',''),'M','')))*-1.0 
	when 'Interval' then Replace(SubString(vNewDepthOfferings.OldOfferingName,CharIndex(', Ht ',vNewDepthOfferings.OldOfferingName)+5,1000),'m','')
	else null
  end Depth

from 
  vOldDepthPhenomenonOfferings
  inner join vNewDepthOfferings
    on (vOldDepthPhenomenonOfferings.OldOfferingID = vNewDepthOfferings.OldOfferingID)
  inner join PhenomenonOffering
    on (PhenomenonOffering.PhenomenonID = vOldDepthPhenomenonOfferings.PhenomenonID) and
	   (PhenomenonOffering.OfferingID = (Select ID from Offering where Code = vNewDepthOfferings.NewOfferingCode))
