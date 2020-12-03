CREATE VIEW [dbo].[vLocations]
AS
Select distinct
  OrganisationID, OrganisationName, OrganisationUrl,
  ProgrammeID, ProgrammeName, ProgrammeUrl,
  ProjectID, ProjectName, ProjectUrl,
  SiteID, SiteName, SiteUrl,
  StationID, StationName, StationUrl,
  (LatitudeNorth + LatitudeSouth) / 2 Latitude,
  (LongitudeWest + LongitudeEast) / 2 Longitude,
  (ElevationMaximum + ElevationMinimum) / 2 Elevation
from
  vImportBatchSummary
where
  (Count > 0) and (ProjectID is not null)
  --and 
  --(LatitudeNorth is not null) and (LatitudeSouth is not null) and
  --(LongitudeWest is not null) and (LongitudeEast is not null)