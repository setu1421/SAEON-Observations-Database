--> Added 2.0.0.5 20160512 TimPN
CREATE VIEW [dbo].[vSiteOrganisation] AS 
SELECT 
  src.*, 
  [Organisation].Name OrganisationName, 
  [OrganisationRole].Name [OrganisationRoleName]
FROM 
  [Site_Organisation] src
  inner join [Organisation]
    on (src.OrganisationID = [Organisation].ID)
  inner join [OrganisationRole]
    on (src.OrganisationRoleID = [OrganisationRole].ID)
--< Added 2.0.0.5 20160512 TimPN

