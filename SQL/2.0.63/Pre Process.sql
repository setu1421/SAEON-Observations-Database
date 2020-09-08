Update
  Organisation
Set
  Url = 'https://wwww.saeon.ac.za'
where
  Code = 'SAEON'
Update
  Programme
Set
  Description = 'South African Environmental Observation Network'
where
  Code = 'SAEON'
Delete DigitalObjectIdentifiers
--DBCC CheckIdent('DigitalObjectIdentifiers')
DBCC CheckIdent('DigitalObjectIdentifiers',Reseed, 0)
drop view if exists vStationDataStreams