--> Added 2.0.32 20170527 TimPN
CREATE VIEW [dbo].[vApiTemporalCoverage]
AS
Select
  vObservationExpansion.*,
  Status = case 
    when StatusName is Null then 'No Status'
	when StatusName = 'Verified' then StatusName
	when StatusName = 'Unverified' then StatusName
	else 'Being Verified'
  end 
from
  vObservationExpansion
--> Added 2.0.32 20170527 TimPN
