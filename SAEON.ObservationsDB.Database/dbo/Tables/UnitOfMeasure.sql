CREATE TABLE [dbo].[UnitOfMeasure] (
    [ID]         UNIQUEIDENTIFIER CONSTRAINT [DF_UnitOfMeasure_ID] DEFAULT (newid()) NOT NULL,
    [Code]       VARCHAR (50)     NOT NULL,
    [Unit]       VARCHAR (100)    NOT NULL,
    [UnitSymbol] VARCHAR (20)     NOT NULL,
    [UserId]     UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_UnitOfMeasure] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_UnitOfMeasure_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
--> Changed 20160329 TimPN
--    CONSTRAINT [IX_UnitOfMeasure_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_UnitOfMeasure_Code] UNIQUE ([Code]),
--< Changed 20160329 TimPN
--> Changed 20160329 TimPN
--    CONSTRAINT [IX_UnitOfMeasure_Unit] UNIQUE ([Unit])
    CONSTRAINT [UX_UnitOfMeasure_Unit] UNIQUE ([Unit])
--< Changed 20160329 TimPN
);
--> Added 20160329 TimPN
GO
CREATE INDEX [IX_UnitOfMeasure_UserId] ON [dbo].[UnitOfMeasure] ([UserId])
--< Added 20160329 TimPN
