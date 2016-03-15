CREATE TABLE [dbo].[Offering] (
    [ID]          UNIQUEIDENTIFIER CONSTRAINT [DF_Offering_ID] DEFAULT (newid()) NOT NULL,
    [Code]        VARCHAR (50)     NOT NULL,
    [Name]        VARCHAR (150)    NOT NULL,
    [Description] VARCHAR (5000)   NULL,
    [UserId]      UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Offering] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_Offering_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [IX_Offering] UNIQUE NONCLUSTERED ([Name] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [IX_Offering_Code] UNIQUE NONCLUSTERED ([Code] ASC) WITH (FILLFACTOR = 80)
);

