CREATE TABLE [dbo].[TransformationType] (
    [ID]          UNIQUEIDENTIFIER CONSTRAINT [DF_TransformationType_ID] DEFAULT (newid()) NOT NULL,
    [Code]        VARCHAR (50)     NOT NULL,
    [Name]        VARCHAR (150)    NOT NULL,
    [Description] VARCHAR (500)    NOT NULL,
    [iorder]      INT              NULL,
--> Added 20160329 TimPN
    [UserId] UNIQUEIDENTIFIER NOT NULL, 
--< Added 20160329 TimPN
    CONSTRAINT [PK_TransformationType] PRIMARY KEY CLUSTERED ([ID]),
--> Added 20160329 TimPN
    CONSTRAINT [UX_TransformationType_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_TransformationType_Name] UNIQUE ([Name]),
    CONSTRAINT [FK_TransformationType_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
--< Added 20160329 TimPN
);
--> Added 20160329 TimPN
GO
CREATE INDEX [IX_TransformationType_UserId] ON [dbo].[TransformationType] ([UserId])
--< Added 20160329 TimPN
