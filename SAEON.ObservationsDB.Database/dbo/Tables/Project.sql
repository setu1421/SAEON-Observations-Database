CREATE TABLE [dbo].[Project]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Project_ID] DEFAULT newid(), 
    [Code] VARCHAR(50) NOT NULL, 
    [Name] VARCHAR(150) NOT NULL, 
    [Description] VARCHAR(5000) NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Project] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [UX_Project_Code] UNIQUE ([Code] ASC),
    CONSTRAINT [UX_Project_Name] UNIQUE ([Name] ASC),
    CONSTRAINT [FK_Project_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
)
