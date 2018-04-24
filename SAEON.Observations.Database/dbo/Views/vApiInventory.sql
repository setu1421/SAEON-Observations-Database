--> Added 2.0.32 20170527 TimPN
CREATE VIEW [dbo].[vApiInventory]
AS
Select
  vObservationExpansion.*,
  Replace(PhenomenonName + '_' + OfferingName + '_' + UnitOfMeasureUnit,' ','') FeatureName,
  IsNull(StatusName, 'No Status') Status
from
  vObservationExpansion
--> Added 2.0.32 20170527 TimPN
