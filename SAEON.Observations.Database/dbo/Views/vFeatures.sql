CREATE VIEW [dbo].[vFeatures]
AS 
Select
  Phenomenon.ID PhenomenonID, Phenomenon.Name PhenomenonName, 
  PhenomenonOffering.ID PhenomenonOfferingID, Offering.ID OfferingID, Offering.Name OfferingName, 
  PhenomenonUOM.ID PhenomenonUOMID, UnitOfMeasure.ID UnitOfMeasureID, UnitOfMeasure.Unit UnitOfMeasureUnit
from
  Phenomenon
  inner join PhenomenonOffering
    on (PhenomenonOffering.PhenomenonID = Phenomenon.ID)
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
  inner join PhenomenonUOM
    on (PhenomenonUOM.PhenomenonID = Phenomenon.ID)
  inner join UnitOfMeasure
    on (PhenomenonUOM.UnitOfMeasureID = UnitOfMeasure.ID)
where
  Exists(Select * from ImportBatchSummary where (PhenomenonOfferingID = PhenomenonOffering.ID) and (PhenomenonUOMID = PhenomenonUOM.ID))

