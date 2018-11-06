-- Fixes to bad data
Update Station Set Code = 'SACTN Ballito EKZNW', Name = 'SACTN Ballito EKZNW' where Code = 'SACTN Ballito EKZNW UTR'
Update Station Set Code = 'SACTN Mangold''s Pool DEA', Name = 'SACTN Mangold''s Pool DEA' where Code = 'SACTN Mangold''s Pool DEA UTR'
Insert into Organisation_Site
  (OrganisationID, SiteID, OrganisationRoleID, UserId)
Values
  ((Select ID from Organisation where Code = 'SAWS'),
   (Select ID from Site where Code = 'SACTN Knysna'),
   (Select ID from OrganisationRole where Code = 'DataProvider'),
   (Select UserId from aspnet_Users where UserName = 'TimPN'))
--Select
--  Station.Code, Site.Code, Organisation.Code, OrganisationID,Station.ID,OrganisationRoleID, Organisation_Site.StartDate, 
--  Organisation_Site.EndDate, Organisation_Site.UserId
--from
--  Station
--  inner join Site
--    on (Station.SiteID = Site.ID)
--  inner join Organisation_Site
--    on (Organisation_Site.SiteID = Site.ID)
--  inner join Organisation
--    on (Organisation_Site.OrganisationID = Organisation.ID)
--where
--  (Site.Code like 'SACTN %') and
--  (Station.Code like '%'+Organisation.Code)
--order by
--  Station.Code, Site.Code, Organisation.Code
-- Move site organisation to station
Insert into Organisation_Station
  (OrganisationID, StationID, OrganisationRoleID, StartDate, EndDate, UserId)
Select
  Organisation.ID, Station.ID, OrganisationRoleID, Organisation_Site.StartDate, 
  Organisation_Site.EndDate, Organisation_Site.UserId
from
  Station
  inner join Site
    on (Station.SiteID = Site.ID)
  inner join Organisation_Site
    on (Organisation_Site.SiteID = Site.ID)
  inner join Organisation
    on (Organisation_Site.OrganisationID = Organisation.ID)
where
  (Site.Code like 'SACTN %') and
  (Station.Code like '%'+Organisation.Code)
  
Delete
  Organisation_Site
from
  Organisation_Site
  inner join Site
    on (Organisation_Site.SiteID = Site.ID)
where
  (Site.Code like 'SACTN %')
