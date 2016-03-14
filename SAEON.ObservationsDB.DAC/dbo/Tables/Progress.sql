CREATE TABLE [dbo].[Progress] (
    [ImportBatchID]         INT              NULL,
    [StartDate]             DATETIME         NULL,
    [EndDate]               DATETIME         NULL,
    [DateUploaded]          DATETIME         NULL,
    [Observations]          BIGINT           NULL,
    [UserId]                UNIQUEIDENTIFIER NULL,
    [SensorProcedureID]     UNIQUEIDENTIFIER NULL,
    [PhenonmenonOfferingID] UNIQUEIDENTIFIER NULL
);

