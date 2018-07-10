CREATE VIEW [dbo].[vApiInventory]
AS
Select
  vObservationExpansion.*,
  Replace(PhenomenonName + '_' + OfferingName + '_' + UnitOfMeasureUnit,' ','') FeatureName,
  IsNull(StatusName, 'No Status') Status
from
  vObservationExpansion
