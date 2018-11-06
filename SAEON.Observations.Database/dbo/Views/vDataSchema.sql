CREATE VIEW [dbo].[vDataSchema]
AS 
select
  d.*,
  t.Code AS DataSourceTypeCode,
  t.[Description] as DataSourceTypeDesc
FROM 
  DataSchema d
  INNER JOIN DataSourceType t
    ON (d.DataSourceTypeID = t.ID)


