CREATE VIEW [dbo].[vDataSchema]
AS 
--> Changed 20170202 TimPN
--Select d.ID,
--d.Code,
--d.Name,
--d.[Description],
--d.UserId,
--d.DataSourceTypeID,
--d.IgnoreFirst,
--d.IgnoreLast,
--d.Delimiter,
--d.Condition,
--d.SplitSelector,
--d.SplitIndex,
--t.Code AS DataSourceTypeCode,
--t.[Description] as DataSourceTypeDesc

--FROM DataSchema d
--	INNER JOIN DataSourceType t
--ON d.DataSourceTypeID = t.ID

select
  d.*,
  t.Code AS DataSourceTypeCode,
  t.[Description] as DataSourceTypeDesc
FROM 
  DataSchema d
  INNER JOIN DataSourceType t
    ON (d.DataSourceTypeID = t.ID)
--< Changed 20170202 TimPN


