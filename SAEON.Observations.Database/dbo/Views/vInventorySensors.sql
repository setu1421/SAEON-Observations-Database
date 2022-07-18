CREATE VIEW [dbo].[vInventorySensors]
AS
Select
  Row_Number() over (order by SiteName, StationName, InstrumentName, SensorName, PhenomenonName, OfferingName, UnitOfMeasureUnit) ID, s.*,
  Cast((case when (OrganisationCode in ('SAEON','SMCRI','EFTEON')) and (ProgrammeCode <> 'SACTN') and (VerifiedCount > 0) and (LatitudeNorth is not null) and (LatitudeSouth is not null) and (LongitudeWest is not null) and (LongitudeEast is not null) then 1 else 0 end) as bit) IsValid
from
(
Select
  OrganisationID, OrganisationCode, OrganisationName, OrganisationDescription, OrganisationUrl,
  ProgrammeID, ProgrammeCode, ProgrammeName, ProgrammeDescription, ProgrammeUrl,
  ProjectID, ProjectCode, ProjectName, ProjectDescription, ProjectUrl,
  SiteID, SiteCode, SiteName, SiteDescription, SiteUrl,
  StationID, StationCode, StationName, StationDescription, StationUrl,
  InstrumentID, InstrumentCode, InstrumentName, InstrumentDescription, InstrumentUrl,
  SensorID, SensorCode, SensorName, SensorDescription, SensorUrl,
  PhenomenonID, PhenomenonCode, PhenomenonName, PhenomenonDescription, PhenomenonUrl,
  PhenomenonOfferingID, OfferingCode, OfferingName, OfferingDescription,
  PhenomenonUOMID, UnitOfMeasureCode, UnitOfMeasureUnit, UnitOfMeasureSymbol,
  Sum(Count) Count, 
  Sum(ValueCount) ValueCount,
  Sum(NullCount) NullCount,
  Sum(VerifiedCount) VerifiedCount,
  Sum(UnverifiedCount) UnverifiedCount,
  Min(StartDate) StartDate, Max(EndDate) EndDate,
  Max(LatitudeNorth) LatitudeNorth, Min(LatitudeSouth) LatitudeSouth,
  Min(LongitudeWest) LongitudeWest, Max(LongitudeEast) LongitudeEast
from
  vImportBatchSummary
group by
  OrganisationID, OrganisationCode, OrganisationName, OrganisationDescription, OrganisationUrl,
  ProgrammeID, ProgrammeCode, ProgrammeName, ProgrammeDescription, ProgrammeUrl,
  ProjectID, ProjectCode, ProjectName, ProjectDescription, ProjectUrl,
  SiteID, SiteCode, SiteName, SiteDescription, SiteUrl,
  StationID, StationCode, StationName, StationDescription, StationUrl,
  InstrumentID, InstrumentCode, InstrumentName, InstrumentDescription, InstrumentUrl,
  SensorID, SensorCode, SensorName, SensorDescription, SensorUrl,
  PhenomenonID, PhenomenonCode, PhenomenonName, PhenomenonDescription, PhenomenonUrl,
  PhenomenonOfferingID, OfferingCode, OfferingName, OfferingDescription,
  PhenomenonUOMID, UnitOfMeasureCode, UnitOfMeasureUnit, UnitOfMeasureSymbol
) s