CREATE VIEW [dbo].[vApiDataDownload]
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
