--> Added 2.0.5 20160527 TimPN
CREATE VIEW [dbo].[vOrganisationSite]
AS 
SELECT 
  src.*,
  [Site].Code SiteCode,
  [Site].Name SiteName,
  [OrganisationRole].Code OrganisationRoleCode,
  [OrganisationRole].Name OrganisationRoleName
FROM 
  [Organisation_Site] src
  inner join [Site]
    on (src.SiteID = [Site].ID)
  inner join [OrganisationRole]
    on (src.OrganisationRoleID = [OrganisationRole].ID)
--< Added 2.0.5 20160527 TimPN
