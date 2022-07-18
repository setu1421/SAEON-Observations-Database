use Observations
Select Count(*) DigitalObjectIdentifiers from DigitalObjectIdentifiers
Select Count(*) Datasets from vDatasetsExpansion
Select Count(*) ValidDatasets from vDatasetsExpansion where IsValid = 1
Select Count(*) SACTN from DigitalObjectIdentifiers where Code like 'SACTN%'