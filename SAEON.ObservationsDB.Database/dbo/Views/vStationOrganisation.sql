CREATE VIEW [dbo].[vStationOrganisation] AS 
SELECT 
  so.*, o.Name OrganisationName, r.Name [OrganisationRoleName]
FROM 
  [Station_Organisation] so
  inner join [Organisation] o
    on (so.OrganisationID = o.ID)
  inner join [OrganisationRole] r
    on (so.OrganisationRoleID = r.ID)

