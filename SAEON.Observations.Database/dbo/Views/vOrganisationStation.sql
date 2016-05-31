--> Added 2.0.5 20160527 TimPN
CREATE VIEW [dbo].[vOrganisationStation]
AS 
SELECT 
  src.*,
  [Station].Code StationCode,
  [Station].Name StationName,
  [OrganisationRole].Code OrganisationRoleCode,
  [OrganisationRole].Name OrganisationRoleName
FROM 
  [Organisation_Station] src
  inner join [Station]
    on (src.StationID = [Station].ID)
  inner join [OrganisationRole]
    on (src.OrganisationRoleID = [OrganisationRole].ID)
--< Added 2.0.5 20160527 TimPN
