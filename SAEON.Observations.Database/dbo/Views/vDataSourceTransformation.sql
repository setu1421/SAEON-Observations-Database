CREATE VIEW [dbo].[vDataSourceTransformation]
AS
Select 
  dt.*,
  tt.Name as TransformationName,
  p.Name as PhenomenonName,
  o.Name as OfferingName,
  u.Unit as UnitOfMeasureUnit,
  np.Name NewPhenomenonName,
  NewOffering.Name NewOfferingName,
  NewUnitOfMeasure.Unit NewUnitOfMeasureUnit,
  tt.iorder
From 
  DataSourceTransformation dt
  INNER JOIN DataSource ds
    on dt.DataSourceID = ds.ID
  INNER JOIN TransformationType tt
    on dt.TransformationTypeID = tt.ID
  INNER JOIN Phenomenon p
    on dt.PhenomenonID = p.ID
  LEFT JOIN PhenomenonOffering po
    on dt.PhenomenonOfferingID = po.ID
  LEFT JOIN Offering o
    on po.OfferingID = o.ID
  LEFT JOIN PhenomenonUOM pu
    on dt.PhenomenonUOMID = pu.ID
  LEFT JOIN UnitOfMeasure u
    on pu.UnitOfMeasureID = u.ID
  left join Phenomenon np
    on (dt.NewPhenomenonID = np.ID)
  left join PhenomenonOffering NewPhenomenonOffering
    on (dt.NewPhenomenonOfferingID = NewPhenomenonOffering.ID)
  left join Offering NewOffering
    on (NewPhenomenonOffering.OfferingID = NewOffering.ID)
  left join PhenomenonUOM NewPhenomenonUOM
    on (dt.NewPhenomenonUOMID = NewPhenomenonUOM.ID)
  left join UnitOfMeasure NewUnitOfMeasure
    on (NewPhenomenonUOM.UnitOfMeasureID = NewUnitOfMeasure.ID)
 


