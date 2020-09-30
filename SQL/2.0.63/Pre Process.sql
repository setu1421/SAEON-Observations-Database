Update
  Organisation
Set
  Url = 'https://wwww.saeon.ac.za'
where
  Code = 'SAEON'
Update Organisation Set Url = null where Url = ''
Update
  Programme
Set
  Description = 'South African Environmental Observation Network'
where
  Code = 'SAEON'
Update Programme Set Url = null where Url = ''
Update Project Set Url = null where Url = ''
Update Site Set Url = null where Url = ''
Update Station Set Url = null where Url = ''
Update Instrument Set Url = null where Url = ''
Delete DigitalObjectIdentifiers
--DBCC CheckIdent('DigitalObjectIdentifiers')
DBCC CheckIdent('DigitalObjectIdentifiers',Reseed, 0)
drop view if exists vStationDataStreams