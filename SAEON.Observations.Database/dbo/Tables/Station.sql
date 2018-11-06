CREATE TABLE [dbo].[Station] (
    [ID]            UNIQUEIDENTIFIER CONSTRAINT [DF_Station_ID] DEFAULT (newid()) NOT NULL,
    [Code]          VARCHAR (50)     NOT NULL,
    [Name]          VARCHAR (150)    NOT NULL,
    [Description]   VARCHAR (5000)   NULL,
    [Url]           VARCHAR (250)    NULL,
    [Latitude]      FLOAT            NULL,
    [Longitude]     FLOAT            NULL,
    [Elevation] Float Null,
    [UserId]        UNIQUEIDENTIFIER NOT NULL,
    [SiteID] UNIQUEIDENTIFIER NOT NULL, 
    [StartDate]        DATE         NULL,
    [EndDate]        DATE         NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Station_AddedAt] DEFAULT (getdate()), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Station_UpdatedAt] DEFAULT (getdate()), 
    [RowVersion] RowVersion not null,
    CONSTRAINT [PKStation] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_Station_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [FK_Station_Site] FOREIGN KEY ([SiteID]) REFERENCES [dbo].[Site] ([ID]),
    CONSTRAINT [UX_Station_SiteID_Code] UNIQUE ([SiteID], [Code]),
    CONSTRAINT [UX_Station_SiteID_Name] UNIQUE ([SiteID],[Name])
);
GO
CREATE INDEX [IX_Station_UserId] ON [dbo].[Station] ([UserId])
GO
CREATE INDEX [IX_Station_SiteID] ON [dbo].[Station] ([SiteID])
GO
CREATE INDEX [IX_Station_Latitude] ON [dbo].[Station] ([Latitude])
GO
CREATE INDEX [IX_Station_Longitude] ON [dbo].[Station] ([Longitude])
GO
CREATE INDEX [IX_Station_Elevation] ON [dbo].[Station] ([Elevation])
GO
CREATE INDEX [IX_Station_StartDate] ON [dbo].[Station] ([StartDate])
GO
CREATE INDEX [IX_Station_EndDate] ON [dbo].[Station] ([EndDate])
GO
CREATE TRIGGER [dbo].[TR_Station_Insert] ON [dbo].[Station]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GetDate(),
        UpdatedAt = NULL
    from
        Station src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Station_Update] ON [dbo].[Station]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = Coalesce(del.AddedAt, ins.AddedAt, GetDate()),
        UpdatedAt = GETDATE()
    from
        Station src
        inner join inserted ins
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
