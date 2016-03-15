-- =============================================
-- Author:		Wim Hugo
-- Create date: 20-06-2014
-- Description:	Create Progress Table - Raw Data
-- =============================================
CREATE PROCEDURE progress_Status_Raw 
	-- Add the parameters for the stored procedure here

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DROP Table dbo.Progress
	
	CREATE TABLE [dbo].Progress(
	[ImportBatchID] [int],
	[StartDate] [datetime],
	[EndDate] [datetime],
	[DateUploaded] [datetime],
	[Observations] [bigint],
	[UserId] [uniqueidentifier],
	[SensorProcedureID] [uniqueidentifier],
	[PhenonmenonOfferingID] [uniqueidentifier]
	)

    -- Insert statements for procedure here
    INSERT INTO [dbo].[Progress]
    (
    [ImportBatchID],
	[StartDate],
	[EndDate],
	[DateUploaded],
	[Observations],
	[UserId],
	[SensorProcedureID],
	[PhenonmenonOfferingID]
    )
   
	SELECT     
		ImportBatchID, 
		MIN(ValueDate) AS StartDate, 
		MAX(ValueDate) AS EndDate, 
		MIN(AddedDate) AS DateUploaded, 
		COUNT(ID) AS Observations, 
		UserId, 
        SensorProcedureID, 
        PhenonmenonOfferingID
FROM    dbo.Observation
GROUP BY PhenonmenonOfferingID, PhenonmenonUOMID, ImportBatchID, UserId, SensorProcedureID
END
