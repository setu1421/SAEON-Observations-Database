CREATE TABLE [dbo].[Module] (
    [ID]          UNIQUEIDENTIFIER CONSTRAINT [DF_Module_ID] DEFAULT (newid()) NOT NULL,
    [Name]        VARCHAR (100)    NOT NULL,
    [Description] VARCHAR (500)    NOT NULL,
    [Url]         VARCHAR (250)    NOT NULL,
    [Icon]        INT              NOT NULL,
    [ModuleID]    UNIQUEIDENTIFIER NULL,
    [iOrder]      INT              NULL,
    CONSTRAINT [PK_Module] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_Module_Module] FOREIGN KEY ([ModuleID]) REFERENCES [dbo].[Module] ([ID])
);
--> Added 20160329 TimPN
GO
CREATE INDEX [IX_Module_ModuleID] ON [dbo].[Module] ([ModuleID])
--< Added 20160329 TimPN
