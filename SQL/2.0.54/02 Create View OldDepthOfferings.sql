use Observations;
Go
Create View OldDepthOfferings as
--Alter View OldDepthOfferings as
Select distinct
  Phenomenon.ID PhenomenonID,
  Phenomenon.Code PhenomenonCode, 
  Phenomenon.Name PhenomenonName,
  Offering.ID OldOfferingID,
  Offering.Code	OldOfferingCode,
  Offering.Name OldOfferingName,
  Offering.Description OldOfferingDescription,
  PhenomenonOffering.ID OldPhenomenonOfferingID,
  'At' Source
from
  PhenomenonOffering
  inner join Phenomenon
    on (PhenomenonOffering.PhenomenonID = Phenomenon.ID)
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
where
  (CharIndex(' at ',Offering.Name) > 0)
union
Select distinct
  Phenomenon.ID PhenomenonID,
  Phenomenon.Code PhenomenonCode, 
  Phenomenon.Name PhenomenonName, 
  Offering.ID OldOfferingID,
  Offering.Code	OldOfferingCode,
  Offering.Name OldOfferingName,
  Offering.Description OldOfferingDescription,
  PhenomenonOffering.ID OldPhenomenonOfferingID,
  'Interval' Source
from
  PhenomenonOffering
  inner join Phenomenon
    on (PhenomenonOffering.PhenomenonID = Phenomenon.ID)
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
where
  (Offering.Name like 'Interval [0-9]min, ht [-][0-9]m') or
  (Offering.Name like 'Interval [0-9]min, ht [-][0-9][0-9]m') or
  (Offering.Name like 'Interval [0-9]min, ht [-][0-9][0-9].[0-9]m') or
  (Offering.Name like 'Interval [0-9][0-9]min, ht [-][0-9]m') or
  (Offering.Name like 'Interval [0-9][0-9]min, ht [-][0-9][0-9]m') or
  (Offering.Name like 'Interval [0-9][0-9]min, ht [-][0-9][0-9].[0-9]m')



