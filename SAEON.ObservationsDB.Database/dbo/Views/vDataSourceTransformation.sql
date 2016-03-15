

CREATE VIEW [dbo].[vDataSourceTransformation]
AS
Select dt.ID,
	   dt.TransformationTypeID,
	   dt.PhenomenonID,
	   dt.StartDate,
	   dt.EndDate,
	   dt.DataSourceID,
	   dt.[Definition],
	   p.Name as PhenomenonName,
	   tt.Name as TransformationName,
	   po.ID as PhenomenonOfferingId,
	   o.Name as OfferingName,
	   pu.ID as UnitOfMeasureId,
	   uom.Unit as UnitofMeasure,
	   dt.NewPhenomenonOfferingID,
	   dt.NewPhenomenonUOMID,
	   tt.iorder,
	   dt.Rank
	   
From DataSourceTransformation dt
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
 LEFT JOIN UnitOfMeasure uom
    on pu.UnitOfMeasureID = uom.ID


