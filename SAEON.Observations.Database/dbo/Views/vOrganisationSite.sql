CREATE VIEW [dbo].[vOrganisationSite]
AS 
SELECT 
  src.*,
  [Organisation].Code OrganisationCode,
  [Organisation].Name OrganisationName,
  [Site].Code SiteCode,
  [Site].Name SiteName,
  [OrganisationRole].Code OrganisationRoleCode,
  [OrganisationRole].Name OrganisationRoleName
FROM 
  [Organisation_Site] src
  inner join [Organisation]
    on (src.OrganisationID = [Organisation].ID)
  inner join [Site]
    on (src.SiteID = [Site].ID)
  inner join [OrganisationRole]
    on (src.OrganisationRoleID = [OrganisationRole].ID)
