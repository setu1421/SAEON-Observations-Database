Use Observations;
with VerifiedDatasets as
(
Select 
	* 
from 
	vDatasetsExpansion
where
  (IsValid = 1) and (StationCode not like 'ELW_%')
	--(VerifiedCount > 0)  and 
	--(LatitudeNorth is not null) and (LatitudeSouth is not null) and 
	--(LongitudeEast is not null) and (LongitudeWest is not null)
),
VerifiedImportBatchSummaries as
(
Select
	*
from
	vImportBatchSummary
where
    (OrganisationCode in ('SAEON','SMCRI','EFTEON')) and (ProgrammeCode <> 'SACTN') and (StationCode not like 'ELW_%') and 
	(VerifiedCount > 0)  and 
	(LatitudeNorth is not null) and (LatitudeSouth is not null) and 
	(LongitudeEast is not null) and (LongitudeWest is not null)
)
Select 
	(Select Count(distinct OrganisationCode) from VerifiedDatasets) Organisations,
	(Select Count(distinct ProgrammeCode) from VerifiedDatasets) Programmes,
	(Select Count(distinct ProjectCode) from VerifiedDatasets) Projects,
	(Select Count(distinct SiteCode) from VerifiedDatasets) Sites,
	(Select Count(distinct StationCode) from VerifiedDatasets) Stations,
	(Select Count(distinct InstrumentCode) from VerifiedImportBatchSummaries) Instruments,
	(Select Count(distinct SensorCode) from VerifiedImportBatchSummaries) Sensors,
	(Select Count(distinct PhenomenonCode) from VerifiedDatasets) Phenomena,
	(Select Count(distinct OfferingCode) from VerifiedDatasets) Offerings,
	(Select Count(distinct UnitOfMeasureCode) from VerifiedDatasets) UnitsOfMeasure,
	(Select Count(*) from vVariables where (PhenomenonName not like 'EOV_%')) Variables,
	(Select Count(*) from VerifiedDatasets) Datasets,
	(Select Sum(VerifiedCount) from VerifiedImportBatchSummaries) Observations,
	(Select Count(*) from UserDownloads) Downloads


