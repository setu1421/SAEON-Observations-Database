CREATE VIEW [dbo].[vSensorThingsAPIObservedProperties]
AS
Select Distinct
  PhenomenonOfferingID ID,
  PhenomenonCode, PhenomenonName, PhenomenonDescription, PhenomenonUrl,
  OfferingCode, OfferingName, OfferingDescription
from
  vInventorySensors
