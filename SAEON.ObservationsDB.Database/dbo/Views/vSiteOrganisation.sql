CREATE VIEW [dbo].[vSiteOrganisation] AS 
SELECT 
  so.*, o.Code + ' - ' + o.Name Organisation, r.Code + ' - ' + r.Name [Role]
FROM 
  [Site_Organisation] so
  inner join [Organisation] o
    on (so.OrganisationID = o.ID)
  inner join [OrganisationRole] r
    on (so.OrganisationRoleID = r.ID)

