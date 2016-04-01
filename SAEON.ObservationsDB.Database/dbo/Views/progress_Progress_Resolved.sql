CREATE VIEW dbo.progress_Progress_Resolved
AS
SELECT     dbo.ImportBatch.ImportDate, dbo.ImportBatch.FileName, dbo.Progress.StartDate, dbo.Progress.EndDate, dbo.Progress.Observations, dbo.Progress.DateUploaded, 
                      dbo.aspnet_Users.UserName, dbo.SensorProcedure.Name AS SensorProcedure, dbo.SensorProcedure.StationID, dbo.Station.Name AS Station
FROM         dbo.SensorProcedure FULL OUTER JOIN
                      dbo.Station ON dbo.SensorProcedure.StationID = dbo.Station.ID FULL OUTER JOIN
                      dbo.Progress LEFT OUTER JOIN
                      dbo.aspnet_Users ON dbo.Progress.UserId = dbo.aspnet_Users.UserId LEFT OUTER JOIN
                      dbo.ImportBatch ON dbo.Progress.ImportBatchID = dbo.ImportBatch.ID ON dbo.SensorProcedure.ID = dbo.Progress.SensorProcedureID

GO
