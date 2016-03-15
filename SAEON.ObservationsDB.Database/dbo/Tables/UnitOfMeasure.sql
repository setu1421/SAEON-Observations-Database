CREATE TABLE [dbo].[UnitOfMeasure] (
    [ID]         UNIQUEIDENTIFIER CONSTRAINT [DF_UnitOfMeasure_ID] DEFAULT (newid()) NOT NULL,
    [Code]       VARCHAR (50)     NOT NULL,
    [Unit]       VARCHAR (100)    NOT NULL,
    [UnitSymbol] VARCHAR (20)     NOT NULL,
    [UserId]     UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_UnitOfMeasure] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_UnitOfMeasure_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [IX_UnitOfMeasure_Code] UNIQUE NONCLUSTERED ([Code] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [IX_UnitOfMeasure_Unit] UNIQUE NONCLUSTERED ([Unit] ASC) WITH (FILLFACTOR = 80)
);

