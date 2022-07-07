CREATE VIEW [dbo].[vVariables]
AS 
Select distinct
  PhenomenonID, PhenomenonName, PhenomenonUrl,
  PhenomenonOfferingID, OfferingID, OfferingName,
  PhenomenonUOMID, UnitOfMeasureID, UnitOfMeasureUnit
from
  vDatasetsExpansion
where
  (IsValid = 1)	
  --(VerifiedCount > 0) and (LatitudeNorth is not null) and (LatitudeSouth is not null) and (LongitudeWest is not null) and (LongitudeEast is not null)
