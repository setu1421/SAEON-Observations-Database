--> Added 2.0.0.5 20160512 TimPN
CREATE VIEW [dbo].[vStationOrganisation] AS 
SELECT 
  src.*, 
  [Station].Code StationCode,
  [Station].Name StationName,
  [Organisation].Code OrganisationCode, 
  [Organisation].Name OrganisationName, 
  [OrganisationRole].Code [OrganisationRoleCode],
  [OrganisationRole].Name [OrganisationRoleName]
FROM 
  [Station_Organisation] src
  inner join [Station]
    on (src.StationID = [Station].ID)
  inner join [Organisation]
    on (src.OrganisationID = [Organisation].ID)
  inner join [OrganisationRole]
    on (src.OrganisationRoleID = [OrganisationRole].ID)
--< Added 2.0.0.5 20160512 TimPN

