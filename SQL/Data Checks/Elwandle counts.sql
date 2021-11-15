with 
VerifiedDatasets
as
(
Select 
	* 
from 
	vInventoryDatasets
where
	(StationCode like 'ELW%') and
	(VerifiedCount > 0)  and 
	(LatitudeNorth is not null) and (LatitudeSouth is not null) and 
	(LongitudeEast is not null) and (LongitudeWest is not null)
),
VerifiedImportBatchSummaries
as
(
Select
	*
from
	vImportBatchSummary
where
	(StationCode like 'ELW%') and
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
	(Select Count(*) from vVariables) Variables,
	(Select Count(*) from VerifiedDatasets) Datasets,
	(Select Sum(VerifiedCount) from VerifiedImportBatchSummaries) Observations,
	(Select Count(*) from UserDownloads) Downloads;
