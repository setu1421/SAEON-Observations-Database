CREATE VIEW [dbo].[vUnitOfMeasurePhenomena]
AS 
Select
  PhenomenonUOM.UnitOfMeasureID, Phenomenon.Code, Phenomenon.Name, Phenomenon.Description
from
  PhenomenonUOM
  inner join UnitOfMeasure
    on (PhenomenonUOM.UnitOfMeasureID = UnitOfMeasure.ID)
  inner join Phenomenon
    on (PhenomenonUOM.PhenomenonID = Phenomenon.ID)
