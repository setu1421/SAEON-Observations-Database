--> Changed 2.0.3 20160503 TimPN
--Renamed SensorProcedure to Sensor
--< Changed 2.0.3 20160503 TimPN
--> Removed 2.0.17 20161128 TimPN
--CREATE VIEW dbo.progress_Progress_Resolved
--AS
--SELECT     dbo.ImportBatch.ImportDate, dbo.ImportBatch.FileName, dbo.Progress.StartDate, dbo.Progress.EndDate, dbo.Progress.Observations, dbo.Progress.DateUploaded, 
--                      dbo.aspnet_Users.UserName, dbo.Sensor.Name AS Sensor, dbo.Sensor.StationID, dbo.Station.Name AS Station
--FROM         dbo.Sensor FULL OUTER JOIN
--                      dbo.Station ON dbo.Sensor.StationID = dbo.Station.ID FULL OUTER JOIN
--                      dbo.Progress LEFT OUTER JOIN
--                      dbo.aspnet_Users ON dbo.Progress.UserId = dbo.aspnet_Users.UserId LEFT OUTER JOIN
--                      dbo.ImportBatch ON dbo.Progress.ImportBatchID = dbo.ImportBatch.ID ON dbo.Sensor.ID = dbo.Progress.SensorID
--> Removed 2.0.17 20161128 TimPN

GO
