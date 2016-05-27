--> Added 2.0.0.5 20160527 TimPN
CREATE VIEW [dbo].[vOrganisationStation]
AS 
SELECT 
  src.*,
  [Station].Code StationCode,
  [Station].Name StationName,
  [OrganisationRole].Code OrganisationRoleCode,
  [OrganisationRole].Name OrganisationRoleName
FROM 
  [Station_Organisation] src
  inner join [Station]
    on (src.StationID = [Station].ID)
  inner join [OrganisationRole]
    on (src.OrganisationRoleID = [OrganisationRole].ID)
--< Added 2.0.0.5 20160527 TimPN
