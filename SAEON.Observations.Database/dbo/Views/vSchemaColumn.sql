CREATE VIEW [dbo].[vSchemaColumn]
AS 
SELECT 
  SchemaColumn.*,
  SchemaColumnType.Name ColumnTypeName, 
  Phenomenon.Name PhenomenonName,
  Offering.Name OfferingName,
  UnitOfMeasure.Unit UnitOfMeasureUnit
FROM 
  SchemaColumn
  inner join SchemaColumnType 
    on (SchemaColumn.SchemaColumnTypeID = SchemaColumnType.ID)
  left join Phenomenon
    on (SchemaColumn.PhenomenonID = Phenomenon.ID)
  left join PhenomenonOffering
    on (SchemaColumn.PhenomenonOfferingID = PhenomenonOffering.ID)
  left join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
  left join PhenomenonUOM 
    on (SchemaColumn.PhenomenonUOMID = PhenomenonUOM.ID)
  left join  UnitOfMeasure
    on (PhenomenonUOM.UnitOfMeasureID = UnitOfMeasure.ID)
    
