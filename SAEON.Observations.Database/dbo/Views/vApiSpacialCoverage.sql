--> Added 2.0.32 20170527 TimPN
CREATE VIEW [dbo].[vApiSpacialCoverage]
AS
Select
  vObservationExpansion.*,
  PhenomenonName + ', ' + OfferingName + ', ' + UnitOfMeasureSymbol FeatureCaption,
  Replace(PhenomenonName + '_' + OfferingName + '_' + UnitOfMeasureUnit,' ','') FeatureName,
  Status = case 
    when StatusName is Null then 'No Status'
    when StatusName = 'Verified' then StatusName
    when StatusName = 'Unverified' then StatusName
    else 'Being Verified'
  end 
from
  vObservationExpansion
where
  (Latitude is not null) and (Longitude is not null)
--> Added 2.0.32 20170527 TimPN
