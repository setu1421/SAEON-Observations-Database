--> Added 2.0.0.5 20160527 TimPN
CREATE VIEW [dbo].[vOrganisationInstrument]
AS 
SELECT 
  src.*,
  [Instrument].Code InstrumentCode,
  [Instrument].Name InstrumentName,
  [OrganisationRole].Code OrganisationRoleCode,
  [OrganisationRole].Name OrganisationRoleName
FROM 
  [Instrument_Organisation] src
  inner join [Instrument]
    on (src.InstrumentID = [Instrument].ID)
  inner join [OrganisationRole]
    on (src.OrganisationRoleID = [OrganisationRole].ID)
--< Added 2.0.0.5 20160527 TimPN
