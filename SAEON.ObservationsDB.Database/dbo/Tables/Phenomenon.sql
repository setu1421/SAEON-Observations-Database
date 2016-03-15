CREATE TABLE [dbo].[Phenomenon] (
    [ID]          UNIQUEIDENTIFIER CONSTRAINT [DF_Phenomenon_ID] DEFAULT (newid()) NOT NULL,
    [Code]        VARCHAR (50)     NOT NULL,
    [Name]        VARCHAR (150)    NOT NULL,
    [Description] VARCHAR (5000)   NULL,
    [Url]         VARCHAR (250)    NULL,
    [UserId]      UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Phenomenon] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_Phenomenon_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [IX_Phenomenon_Code] UNIQUE NONCLUSTERED ([Code] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [IX_Phenomenon_Name] UNIQUE NONCLUSTERED ([Name] ASC) WITH (FILLFACTOR = 80)
);

