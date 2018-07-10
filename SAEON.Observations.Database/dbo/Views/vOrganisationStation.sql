CREATE VIEW [dbo].[vOrganisationStation]
AS 
SELECT 
  src.*,
  [Organisation].Code OrganisationCode,
  [Organisation].Name OrganisationName,
  [Station].Code StationCode,
  [Station].Name StationName,
  [OrganisationRole].Code OrganisationRoleCode,
  [OrganisationRole].Name OrganisationRoleName
FROM 
  [Organisation_Station] src
  inner join [Organisation]
    on (src.OrganisationID = [Organisation].ID)
  inner join [Station]
    on (src.StationID = [Station].ID)
  inner join [OrganisationRole]
    on (src.OrganisationRoleID = [OrganisationRole].ID)
