--> Added 2.0.0.5 20160512 TimPN
CREATE VIEW [dbo].[vSiteOrganisation] AS 
SELECT 
  src.*, 
  [Site].Code SiteCode,
  [Site].Name SiteName,
  [Organisation].Code OrganisationCode, 
  [Organisation].Name OrganisationName, 
  [OrganisationRole].Code [OrganisationRoleCode],
  [OrganisationRole].Name [OrganisationRoleName]
FROM 
  [Site_Organisation] src
  inner join [Site]
    on (src.SiteID = [Site].ID)
  inner join [Organisation]
    on (src.OrganisationID = [Organisation].ID)
  inner join [OrganisationRole]
    on (src.OrganisationRoleID = [OrganisationRole].ID)
--< Added 2.0.0.5 20160512 TimPN

