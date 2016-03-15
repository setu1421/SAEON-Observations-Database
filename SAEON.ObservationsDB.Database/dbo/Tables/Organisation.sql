CREATE TABLE [dbo].[Organisation] (
    [ID]          UNIQUEIDENTIFIER CONSTRAINT [DF_Table_1_Organisation] DEFAULT (newid()) NOT NULL,
    [Code]        VARCHAR (50)     NOT NULL,
    [Name]        VARCHAR (150)    NOT NULL,
    [Description] VARCHAR (5000)   NULL,
    [UserId]      UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Organisation] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_Organisation_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [IX_Organisation_Code] UNIQUE NONCLUSTERED ([Code] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [IX_Organisation_Name] UNIQUE NONCLUSTERED ([Name] ASC) WITH (FILLFACTOR = 80)
);

