CREATE VIEW [dbo].[vSiteOrganisation] AS 
SELECT 
  so.*, o.Name OrganisationName, r.Name [OrganisationRoleName]
FROM 
  [Site_Organisation] so
  inner join [Organisation] o
    on (so.OrganisationID = o.ID)
  inner join [OrganisationRole] r
    on (so.OrganisationRoleID = r.ID)

