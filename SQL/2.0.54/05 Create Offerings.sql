use Observations;
Insert  Offering
  (Code, Name, Description, UserId)
Select distinct
  NewOfferingCode,
  NewOfferingName,
  NewOfferingDescription,
  (Select UserId from aspnet_Users where UserName='TimPN')
from
  NewDepthOfferings
where
  not Exists(Select * from Offering where Code = NewOfferingCode)
