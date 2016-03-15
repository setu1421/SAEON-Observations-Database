CREATE View [dbo].[vDataSource]
AS

Select d.ID,  d.Code,  d.Name,
 d.[Description],
 d.[Url],d.DefaultNullValue, 
 d.ErrorEstimate, 
 d.UpdateFreq, 
 d.StartDate, 
 d.LastUpdate,
 d.DataSchemaID, 
 d.UserId,
 t.[Name] DataSchemaName from DataSource d
	LEFT JOIN DataSchema t
	ON d.DataSchemaID = t.ID


