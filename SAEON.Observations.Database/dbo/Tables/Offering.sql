CREATE TABLE [dbo].[Offering] (
    [ID]          UNIQUEIDENTIFIER CONSTRAINT [DF_Offering_ID] DEFAULT (newid()) NOT NULL,
    [Code]        VARCHAR (50)     NOT NULL,
    [Name]        VARCHAR (150)    NOT NULL,
    [Description] VARCHAR (5000)   NULL,
    [UserId]      UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Offering_AddedAt] DEFAULT (getdate()), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Offering_UpdatedAt] DEFAULT (getdate()), 
    [RowVersion] RowVersion not null,
    CONSTRAINT [PK_Offering] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_Offering_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_Offering_Name] UNIQUE ([Name]),
    CONSTRAINT [UX_Offering_Code] UNIQUE ([Code])
);
GO
CREATE INDEX [IX_Offering_UserId] ON [dbo].[Offering] ([UserId])
GO
CREATE TRIGGER [dbo].[TR_Offering_Insert] ON [dbo].[Offering]
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
        Offering src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Offering_Update] ON [dbo].[Offering]
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
        Offering src
        inner join inserted ins
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END

