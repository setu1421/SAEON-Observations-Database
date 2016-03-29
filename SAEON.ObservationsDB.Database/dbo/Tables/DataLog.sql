CREATE TABLE [dbo].[DataLog] (
    [ID]                         INT              IDENTITY (1, 1) NOT NULL,
    [SensorProcedureID]          UNIQUEIDENTIFIER NOT NULL,
    [ImportDate]                 DATETIME         CONSTRAINT [DF_DataLog_ImportDate] DEFAULT (getdate()) NOT NULL,
    [ValueDate]                  DATETIME         NULL,
    [ValueTime]                  DATETIME         NULL,
    [ValueText]                  VARCHAR (50)     NOT NULL,
    [TransformValueText]         VARCHAR (50)     NULL,
    [RawValue]                   FLOAT (53)       NULL,
    [DataValue]                  FLOAT (53)       NULL,
    [Comment]                    VARCHAR (250)    NULL,
    [InvalidDateValue]           VARCHAR (50)     NULL,
    [InvalidTimeValue]           VARCHAR (50)     NULL,
    [InvalidOffering]            VARCHAR (50)     NULL,
    [InvalidUOM]                 VARCHAR (50)     NULL,
    [DataSourceTransformationID] UNIQUEIDENTIFIER NULL,
    [StatusID]                   UNIQUEIDENTIFIER NOT NULL,
    [ImportStatus]               VARCHAR (500)    NOT NULL,
    [UserId]                     UNIQUEIDENTIFIER NOT NULL,
    [PhenomenonOfferingID]       UNIQUEIDENTIFIER NULL,
    [PhenonmenonUOMID]           UNIQUEIDENTIFIER NULL,
    [ImportBatchID]              INT              NOT NULL,
    [RawRecordData]              VARCHAR (500)    NULL,
    [RawFieldValue]              VARCHAR (50)     NOT NULL,
    CONSTRAINT [PK_DataLog] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_DataLog_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [FK_DataLog_DataSourceTransformation] FOREIGN KEY ([DataSourceTransformationID]) REFERENCES [dbo].[DataSourceTransformation] ([ID]),
    CONSTRAINT [FK_DataLog_ImportBatch] FOREIGN KEY ([ImportBatchID]) REFERENCES [dbo].[ImportBatch] ([ID]),
    CONSTRAINT [FK_DataLog_PhenomenonOffering] FOREIGN KEY ([PhenomenonOfferingID]) REFERENCES [dbo].[PhenomenonOffering] ([ID]),
    CONSTRAINT [FK_DataLog_PhenomenonUOM] FOREIGN KEY ([PhenonmenonUOMID]) REFERENCES [dbo].[PhenomenonUOM] ([ID]),
    CONSTRAINT [FK_DataLog_SensorProcedure] FOREIGN KEY ([SensorProcedureID]) REFERENCES [dbo].[SensorProcedure] ([ID]),
    CONSTRAINT [FK_DataLog_Status] FOREIGN KEY ([StatusID]) REFERENCES [dbo].[Status] ([ID])
);
GO
CREATE INDEX [IX_DataLog] ON [dbo].[DataLog]([ImportBatchID]);
--> Added 20160329 TimPN
GO
CREATE INDEX [IX_DataLog_SensorProcedureID] ON [dbo].[DataLog] ([SensorProcedureID]);
GO
CREATE INDEX [IX_DataLog_DataSourceTransformationID] ON [dbo].[DataLog] ([DataSourceTransformationID])
GO
CREATE INDEX [IX_DataLog_PhenomenonOfferingID] ON [dbo].[DataLog] ([PhenomenonOfferingID])
GO
CREATE INDEX [IX_DataLog_PhenomenonUOMID] ON [dbo].[DataLog] ([PhenonmenonUOMID])
GO
CREATE INDEX [IX_DataLog_StatusID] ON [dbo].[DataLog] ([StatusID])
GO
CREATE INDEX [IX_DataLog_UserId] ON [dbo].[DataLog] ([UserId])
--< Added 20160329 TimPN

