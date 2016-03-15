CREATE TABLE [dbo].[Observation] (
    [ID]                    INT              IDENTITY (1, 1) NOT NULL,
    [SensorProcedureID]     UNIQUEIDENTIFIER NOT NULL,
    [ValueDate]             DATETIME         NOT NULL,
    [RawValue]              FLOAT (53)       NULL,
    [DataValue]             FLOAT (53)       NULL,
    [Comment]               VARCHAR (250)    NULL,
    [PhenonmenonOfferingID] UNIQUEIDENTIFIER NOT NULL,
    [PhenonmenonUOMID]      UNIQUEIDENTIFIER NOT NULL,
    [ImportBatchID]         INT              NOT NULL,
    [UserId]                UNIQUEIDENTIFIER NOT NULL,
    [AddedDate]             DATETIME         CONSTRAINT [DF_Observation_AddedDate] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_Observation] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_Observation_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [FK_Observation_Observation] FOREIGN KEY ([ImportBatchID]) REFERENCES [dbo].[ImportBatch] ([ID]),
    CONSTRAINT [FK_Observation_PhenomenonOffering] FOREIGN KEY ([PhenonmenonOfferingID]) REFERENCES [dbo].[PhenomenonOffering] ([ID]),
    CONSTRAINT [FK_Observation_PhenomenonUOM] FOREIGN KEY ([PhenonmenonUOMID]) REFERENCES [dbo].[PhenomenonUOM] ([ID]),
    CONSTRAINT [FK_Observation_SensorProcedure] FOREIGN KEY ([SensorProcedureID]) REFERENCES [dbo].[SensorProcedure] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_Observation]
    ON [dbo].[Observation]([SensorProcedureID] ASC, [ValueDate] ASC, [RawValue] ASC) WITH (FILLFACTOR = 80);


GO
CREATE NONCLUSTERED INDEX [IX_Observation_BatchID]
    ON [dbo].[Observation]([ImportBatchID] ASC) WITH (FILLFACTOR = 80);

