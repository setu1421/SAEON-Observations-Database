CREATE VIEW [dbo].[vInventoryDatasets]
AS 
Select
  Row_Number() over (order by StationCode, PhenomenonCode, OfferingCode, UnitOfMeasureCode) ID, s.*
from
(
Select
  OrganisationID, OrganisationCode, OrganisationName, OrganisationDescription, OrganisationUrl,
  ProgrammeID, ProgrammeCode, ProgrammeName, ProgrammeDescription, ProgrammeUrl,
  ProjectID, ProjectCode, ProjectName, ProjectDescription, ProjectUrl,
  SiteID, SiteCode, SiteName, SiteDescription, SiteUrl,
  StationID, StationCode, StationName, StationDescription, StationUrl,
  PhenomenonID, PhenomenonCode, PhenomenonName, PhenomenonDescription, PhenomenonUrl,
  PhenomenonOfferingID, OfferingID, OfferingCode, OfferingName, OfferingDescription,
  PhenomenonUOMID, UnitOfMeasureID, UnitOfMeasureCode, UnitOfMeasureUnit, UnitOfMeasureSymbol,
  Sum(Count) Count,
  Sum(ValueCount) ValueCount,
  Sum(NullCount) NullCount,
  Sum(VerifiedCount) VerifiedCount,
  Sum(UnverifiedCount) UnverifiedCount,
  Min(StartDate) StartDate,
  Max(EndDate) EndDate,
  Max(LatitudeNorth) LatitudeNorth,
  Min(LatitudeSouth) LatitudeSouth,
  Min(LongitudeWest) LongitudeWest,
  Max(LongitudeEast) LongitudeEast,
  Min(ElevationMinimum) ElevationMinimum,
  Max(ElevationMaximum) ElevationMaximum
from
  vImportBatchSummary
group by
  OrganisationID, OrganisationCode, OrganisationName, OrganisationDescription, OrganisationUrl,
  ProgrammeID, ProgrammeCode, ProgrammeName, ProgrammeDescription, ProgrammeUrl,
  ProjectID, ProjectCode, ProjectName, ProjectDescription, ProjectUrl,
  SiteID, SiteCode, SiteName, SiteDescription, SiteUrl,
  StationID, StationCode, StationName, StationDescription, StationUrl,
  PhenomenonID, PhenomenonCode, PhenomenonName, PhenomenonDescription, PhenomenonUrl,
  PhenomenonOfferingID, OfferingID, OfferingCode, OfferingName, OfferingDescription,
  PhenomenonUOMID, UnitOfMeasureID, UnitOfMeasureCode, UnitOfMeasureUnit, UnitOfMeasureSymbol
) s

