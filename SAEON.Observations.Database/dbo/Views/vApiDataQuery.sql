--> Added 2.0.32 20170527 TimPN
CREATE VIEW [dbo].[vApiDataQuery]
AS
Select
  vObservationExpansion.*,
  PhenomenonName + ', ' + OfferingName + ', ' + UnitOfMeasureSymbol FeatureCaption,
  Replace(PhenomenonName + '_' + OfferingName + '_' + UnitOfMeasureUnit,' ','') FeatureName,
  IsNull(StatusName, 'No Status') Status
from
  vObservationExpansion
where
  (StatusName = 'Verified')
--> Added 2.0.32 20170527 TimPN
