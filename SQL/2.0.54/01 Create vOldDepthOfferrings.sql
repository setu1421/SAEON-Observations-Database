use Observations;
Go
Create View vOldDepthOfferings as
--Alter View vOldDepthOfferings as
Select distinct
  Offering.ID OldOfferingID,
  Offering.Code	OldOfferingCode,
  Offering.Name OldOfferingName,
  Offering.Description OldOfferingDescription,
  'At' Source
from
  Offering
where
  (CharIndex(' at ',Offering.Name) > 0)
union
Select distinct
  Offering.ID OldOfferingID,
  Offering.Code	OldOfferingCode,
  Offering.Name OldOfferingName,
  Offering.Description OldOfferingDescription,
  'Interval' Source
from
  Offering
where
  (Offering.Name like 'Interval [0-9]min, ht [-][0-9]m') or
  (Offering.Name like 'Interval [0-9]min, ht [-][0-9][0-9]m') or
  (Offering.Name like 'Interval [0-9]min, ht [-][0-9][0-9].[0-9]m') or
  (Offering.Name like 'Interval [0-9][0-9]min, ht [-][0-9]m') or
  (Offering.Name like 'Interval [0-9][0-9]min, ht [-][0-9][0-9]m') or
  (Offering.Name like 'Interval [0-9][0-9]min, ht [-][0-9][0-9].[0-9]m')

