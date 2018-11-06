CREATE TABLE [dbo].[Status] (
    [ID]          UNIQUEIDENTIFIER CONSTRAINT [DF_Status_ID] DEFAULT (newid()) NOT NULL,
    [Code]        VARCHAR (50)     NOT NULL,
    [Name]        VARCHAR (150)    NOT NULL,
    [Description] VARCHAR (500)    NOT NULL,
    [UserId] UNIQUEIDENTIFIER NULL, 
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Status_AddedAt] DEFAULT (getdate()), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Status_UpdatedAt] DEFAULT (getdate()), 
    [RowVersion] RowVersion not null,
    CONSTRAINT [PK_Status] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [UX_Status_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_Status_Name] UNIQUE ([Name]),
    CONSTRAINT [FK_Status_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId])
);
GO
CREATE INDEX [IX_Status_UserId] ON [dbo].[Status] ([UserId])
GO
CREATE TRIGGER [dbo].[TR_Status_Insert] ON [dbo].[Status]
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
        Status src
        inner join inserted ins 
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Status_Update] ON [dbo].[Status]
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
        Status src
        inner join inserted ins 
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
