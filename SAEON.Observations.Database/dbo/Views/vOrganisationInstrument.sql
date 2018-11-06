CREATE VIEW [dbo].[vOrganisationInstrument]
AS 
SELECT 
  src.*,
  [Organisation].Code OrganisationCode,
  [Organisation].Name OrganisationName,
  [Instrument].Code InstrumentCode,
  [Instrument].Name InstrumentName,
  [OrganisationRole].Code OrganisationRoleCode,
  [OrganisationRole].Name OrganisationRoleName
FROM 
  [Organisation_Instrument] src
  inner join [Organisation]
    on (src.OrganisationID = [Organisation].ID)
  inner join [Instrument]
    on (src.InstrumentID = [Instrument].ID)
  inner join [OrganisationRole]
    on (src.OrganisationRoleID = [OrganisationRole].ID)
