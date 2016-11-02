﻿/*
Deployment script for Observations

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "Observations"
:setvar DefaultFilePrefix "Observations"
:setvar DefaultDataPath "C:\Program Files\Microsoft SQL Server\MSSQL13.SAEON\MSSQL\DATA\"
:setvar DefaultLogPath "C:\Program Files\Microsoft SQL Server\MSSQL13.SAEON\MSSQL\DATA\"

GO
:on error exit
GO
/*
Detect SQLCMD mode and disable script execution if SQLCMD mode is not supported.
To re-enable the script after enabling SQLCMD mode, execute the following:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'SQLCMD mode must be enabled to successfully execute this script.';
        SET NOEXEC ON;
    END


GO
--IF (SELECT is_default
--    FROM   [$(DatabaseName)].[sys].[filegroups]
--    WHERE  [name] = N'Documents') = 0
--    BEGIN
--        ALTER DATABASE [$(DatabaseName)]
--            MODIFY FILEGROUP [Documents] DEFAULT;
--    END


GO
USE [$(DatabaseName)];


GO
PRINT N'Dropping [dbo].[TR_PhenomenonUPM_Insert]...';


GO
DROP TRIGGER [dbo].[TR_PhenomenonUPM_Insert];


GO
PRINT N'Altering [dbo].[TR_AuditLog_Insert]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_AuditLog_Insert] ON [dbo].[AuditLog]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update 
        src 
    set 
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        AuditLog src
		inner join inserted ins 
            on (ins.ID = src.ID)
END
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_AuditLog_Update]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_AuditLog_Update] ON [dbo].[AuditLog]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update 
        src 
    set 
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        AuditLog src
		inner join inserted ins 
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--> Changed 2.0.15 20161102 TimPN
--< Added 2.0.3 20160421 TimPN
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_DataLog_Insert]...';


GO
ALTER TRIGGER [dbo].[TR_DataLog_Insert] ON [dbo].[DataLog]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update 
        src 
    set 
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        DataLog src
        inner join inserted ins 
            on (ins.ID = src.ID)
END
GO
PRINT N'Altering [dbo].[TR_DataLog_Update]...';


GO
ALTER TRIGGER [dbo].[TR_DataLog_Update] ON [dbo].[DataLog]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update 
        src 
    set 
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        DataLog src
        inner join inserted ins 
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.8 20160708 TimPN
GO
PRINT N'Altering [dbo].[TR_DataSchema_Insert]...';


GO
ALTER TRIGGER [dbo].[TR_DataSchema_Insert] ON [dbo].[DataSchema]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update 
        src 
    set 
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        DataSchema src
        inner join inserted ins 
            on (ins.ID = src.ID)
END
GO
PRINT N'Altering [dbo].[TR_DataSchema_Update]...';


GO
ALTER TRIGGER [dbo].[TR_DataSchema_Update] ON [dbo].[DataSchema]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update 
        src 
    set 
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        DataSchema src
        inner join inserted ins 
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.8 20160715 TimPN
GO
PRINT N'Altering [dbo].[TR_DataSource_Insert]...';


GO
ALTER TRIGGER [dbo].[TR_DataSource_Insert] ON [dbo].[DataSource]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        DataSource src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
PRINT N'Altering [dbo].[TR_DataSource_Update]...';


GO
ALTER TRIGGER [dbo].[TR_DataSource_Update] ON [dbo].[DataSource]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        DataSource src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.3 20160421 TimPN
GO
PRINT N'Altering [dbo].[TR_DataSourceRole_Insert]...';


GO
ALTER TRIGGER [dbo].[TR_DataSourceRole_Insert] ON [dbo].[DataSourceRole]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        DataSourceRole src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
PRINT N'Altering [dbo].[TR_DataSourceRole_Update]...';


GO
ALTER TRIGGER [dbo].[TR_DataSourceRole_Update] ON [dbo].[DataSourceRole]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        DataSourceRole src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.8 20160715 TimPN
GO
PRINT N'Altering [dbo].[TR_DataSourceTransformation_Insert]...';


GO
ALTER TRIGGER [dbo].[TR_DataSourceTransformation_Insert] ON [dbo].[DataSourceTransformation]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        DataSourceTransformation src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
PRINT N'Altering [dbo].[TR_DataSourceTransformation_Update]...';


GO
ALTER TRIGGER [dbo].[TR_DataSourceTransformation_Update] ON [dbo].[DataSourceTransformation]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        DataSourceTransformation src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--> Changed 2.0.15 20161102 TimPN
--< Added 2.0.8 20160715 TimPN
GO
PRINT N'Altering [dbo].[TR_DataSourceType_Insert]...';


GO
ALTER TRIGGER [dbo].[TR_DataSourceType_Insert] ON [dbo].[DataSourceType]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        DataSourceType src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
PRINT N'Altering [dbo].[TR_DataSourceType_Update]...';


GO
ALTER TRIGGER [dbo].[TR_DataSourceType_Update] ON [dbo].[DataSourceType]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        DataSourceType src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--> Changed 2.0.15 20161102 TimPN
--< Added 2.0.8 20160715 TimPN
GO
PRINT N'Altering [dbo].[TR_ImportBatch_Insert]...';


GO
ALTER TRIGGER [dbo].[TR_ImportBatch_Insert] ON [dbo].[ImportBatch]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        ImportBatch src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
PRINT N'Altering [dbo].[TR_ImportBatch_Update]...';


GO
ALTER TRIGGER [dbo].[TR_ImportBatch_Update] ON [dbo].[ImportBatch]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        ImportBatch src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.8 20160715 TimPN
GO
PRINT N'Altering [dbo].[TR_Instrument_Insert]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_Instrument_Insert] ON [dbo].[Instrument]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update 
        src 
    set 
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        Instrument src
        inner join inserted ins 
            on (ins.ID = src.ID)
END
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_Instrument_Update]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_Instrument_Update] ON [dbo].[Instrument]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update 
        src 
    set 
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        Instrument src
        inner join inserted ins 
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--> Changed 2.0.15 20161102 TimPN
--< Added 2.0.4 20160508 TimPN
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_Instrument_DataSource_Insert]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_Instrument_DataSource_Insert] ON [dbo].[Instrument_DataSource]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        Instrument_DataSource src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_Instrument_DataSource_Update]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_Instrument_DataSource_Update] ON [dbo].[Instrument_DataSource]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        Instrument_DataSource src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--> Changed 2.0.15 20161102 TimPN
--< Added 2.0.9 20160727 TimPN
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_Instrument_Sensor_Insert]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_Instrument_Sensor_Insert] ON [dbo].[Instrument_Sensor]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        Instrument_Sensor src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_Instrument_Sensor_Update]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_Instrument_Sensor_Update] ON [dbo].[Instrument_Sensor]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        Instrument_Sensor src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--> Added 2.0.5 20160530 TimPN
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_Observation_Insert]...';


GO
ALTER TRIGGER [dbo].[TR_Observation_Insert] ON [dbo].[Observation]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        Observation src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
PRINT N'Altering [dbo].[TR_Observation_Update]...';


GO
ALTER TRIGGER [dbo].[TR_Observation_Update] ON [dbo].[Observation]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        Observation src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.8 20160718 TimPN
GO
PRINT N'Altering [dbo].[TR_Offering_Insert]...';


GO
ALTER TRIGGER [dbo].[TR_Offering_Insert] ON [dbo].[Offering]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        Offering src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
PRINT N'Altering [dbo].[TR_Offering_Update]...';


GO
ALTER TRIGGER [dbo].[TR_Offering_Update] ON [dbo].[Offering]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        Offering src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--> Changed 2.0.15 20161102 TimPN
--< Added 2.0.8 20160718 TimPN
GO
PRINT N'Altering [dbo].[TR_Organisation_Insert]...';


GO
ALTER TRIGGER [dbo].[TR_Organisation_Insert] ON [dbo].[Organisation]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update 
        src 
    set 
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        Organisation src
        inner join inserted ins 
            on (ins.ID = src.ID)
END
GO
PRINT N'Altering [dbo].[TR_Organisation_Update]...';


GO
ALTER TRIGGER [dbo].[TR_Organisation_Update] ON [dbo].[Organisation]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update 
        src 
    set 
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        Organisation src
        inner join inserted ins 
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--> Changed 2.0.15 20161102 TimPN
--< Added 2.0.4 20160508 TimPN
GO
PRINT N'Altering [dbo].[TR_Organisation_Instrument_Insert]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_Organisation_Instrument_Insert] ON [dbo].[Organisation_Instrument]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        Organisation_Instrument src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_Organisation_Instrument_Update]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_Organisation_Instrument_Update] ON [dbo].[Organisation_Instrument]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        Organisation_Instrument src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.5 20160530 TimPN
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_Organisation_Site_Insert]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_Organisation_Site_Insert] ON [dbo].[Organisation_Site]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        Organisation_Site src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_Organisation_Site_Update]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_Organisation_Site_Update] ON [dbo].[Organisation_Site]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        Organisation_Site src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.5 20160530 TimPN
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_Organisation_Station_Insert]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_Organisation_Station_Insert] ON [dbo].[Organisation_Station]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        Organisation_Station src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_Organisation_Station_Update]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_Organisation_Station_Update] ON [dbo].[Organisation_Station]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        Organisation_Station src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.5 20160530 TimPN
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_OrganisationRole]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_OrganisationRole] ON [dbo].[OrganisationRole]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        OrganisationRole src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_OrganisationRole_Update]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_OrganisationRole_Update] ON [dbo].[OrganisationRole]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        OrganisationRole src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.8 20160718 TimPN
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_Phenomenon_Insert]...';


GO
ALTER TRIGGER [dbo].[TR_Phenomenon_Insert] ON [dbo].[Phenomenon]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        Phenomenon src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
PRINT N'Altering [dbo].[TR_Phenomenon_Update]...';


GO
ALTER TRIGGER [dbo].[TR_Phenomenon_Update] ON [dbo].[Phenomenon]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        Phenomenon src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.8 20160718 TimPN
GO
PRINT N'Altering [dbo].[TR_PhenomenonOffering_Insert]...';


GO
ALTER TRIGGER [dbo].[TR_PhenomenonOffering_Insert] ON [dbo].[PhenomenonOffering]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        PhenomenonOffering src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
PRINT N'Altering [dbo].[TR_PhenomenonOffering_Update]...';


GO
ALTER TRIGGER [dbo].[TR_PhenomenonOffering_Update] ON [dbo].[PhenomenonOffering]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        PhenomenonOffering src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.8 20160718 TimPN
GO
PRINT N'Altering [dbo].[TR_PhenomenonUOM_Update]...';


GO
ALTER TRIGGER [dbo].[TR_PhenomenonUOM_Update] ON [dbo].[PhenomenonUOM]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        PhenomenonUOM src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.8 20160718 TimPN
GO
PRINT N'Altering [dbo].[TR_Programme_Insert]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_Programme_Insert] ON [dbo].[Programme]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update 
        src 
    set 
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        Programme src
        inner join inserted ins 
            on (ins.ID = src.ID)
END
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_Programme_Update]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_Programme_Update] ON [dbo].[Programme]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update 
        src 
    set 
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        Programme src
        inner join inserted ins 
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.5 20160511 TimPN
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_Project_Insert]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_Project_Insert] ON [dbo].[Project]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update 
        src 
    set 
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        Project src
        inner join inserted ins 
            on (ins.ID = src.ID)
END
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_Project_Update]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_Project_Update] ON [dbo].[Project]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update 
        src 
    set 
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        Project src
        inner join inserted ins 
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.5 20160511 TimPN
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_Project_Station_Insert]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_Project_Station_Insert] ON [dbo].[Project_Station]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update 
        src 
    set 
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        Project_Station src
        inner join inserted ins 
            on (ins.ID = src.ID)
END
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_Project_Station_Update]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_Project_Station_Update] ON [dbo].[Project_Station]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update 
        src 
    set 
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        Project_Station src
        inner join inserted ins 
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.5 20160527 TimPN
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_SchemaColumn_Insert]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_SchemaColumn_Insert] ON [dbo].[SchemaColumn]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        SchemaColumn src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_SchemaColumn_Update]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_SchemaColumn_Update] ON [dbo].[SchemaColumn]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        SchemaColumn src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.11 20160908 TimPN
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_SchemaColumnType_Insert]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_SchemaColumnType_Insert] ON [dbo].[SchemaColumnType]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        SchemaColumnType src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_SchemaColumnType_Update]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_SchemaColumnType_Update] ON [dbo].[SchemaColumnType]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        SchemaColumnType src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.11 20160908 TimPN
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_Sensor_Insert]...';


GO
ALTER TRIGGER [dbo].[TR_Sensor_Insert] ON [dbo].[Sensor]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        Sensor src
        inner join inserted ins 
            on (ins.ID = src.ID)
END
GO
PRINT N'Altering [dbo].[TR_Sensor_Update]...';


GO
ALTER TRIGGER [dbo].[TR_Sensor_Update] ON [dbo].[Sensor]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        Sensor src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.8 20160718 TimPN
GO
PRINT N'Altering [dbo].[TR_Site_Insert]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_Site_Insert] ON [dbo].[Site]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update 
        src 
    set 
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        Site src
        inner join inserted ins 
            on (ins.ID = src.ID)
END
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_Site_Update]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_Site_Update] ON [dbo].[Site]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update 
        src 
    set 
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        Site src
        inner join inserted ins 
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.3 20160421 TimPN
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_Station_Insert]...';


GO
ALTER TRIGGER [dbo].[TR_Station_Insert] ON [dbo].[Station]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        Station src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
PRINT N'Altering [dbo].[TR_Station_Update]...';


GO
ALTER TRIGGER [dbo].[TR_Station_Update] ON [dbo].[Station]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        Station src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.3 20160421 TimPN
GO
PRINT N'Altering [dbo].[TR_Station_Instrument_Insert]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_Station_Instrument_Insert] ON [dbo].[Station_Instrument]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        Station_Instrument src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_Station_Instrument_Update]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_Station_Instrument_Update] ON [dbo].[Station_Instrument]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        Station_Instrument src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.5 20160512 TimPN
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_Status_Insert]...';


GO
ALTER TRIGGER [dbo].[TR_Status_Insert] ON [dbo].[Status]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        Status src
        inner join inserted ins 
            on (ins.ID = src.ID)
END
GO
PRINT N'Altering [dbo].[TR_Status_Update]...';


GO
ALTER TRIGGER [dbo].[TR_Status_Update] ON [dbo].[Status]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        Status src
        inner join inserted ins 
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.8 20160718 TimPN
GO
PRINT N'Altering [dbo].[TR_StatusReason_Insert]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_StatusReason_Insert] ON [dbo].[StatusReason]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        StatusReason src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_StatusReason_Update]...';


GO
SET ANSI_NULLS, QUOTED_IDENTIFIER OFF;


GO
ALTER TRIGGER [dbo].[TR_StatusReason_Update] ON [dbo].[StatusReason]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        StatusReason src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--> Added 2.0.9 20160823 TimPN
GO
SET ANSI_NULLS, QUOTED_IDENTIFIER ON;


GO
PRINT N'Altering [dbo].[TR_TransformationType_Insert]...';


GO
ALTER TRIGGER [dbo].[TR_TransformationType_Insert] ON [dbo].[TransformationType]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        TransformationType src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
PRINT N'Altering [dbo].[TR_TransformationType_Update]...';


GO
ALTER TRIGGER [dbo].[TR_TransformationType_Update] ON [dbo].[TransformationType]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        TransformationType src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.8 20160718 TimPN
GO
PRINT N'Altering [dbo].[TR_UnitOfMeasure_Insert]...';


GO
ALTER TRIGGER [dbo].[TR_UnitOfMeasure_Insert] ON [dbo].[UnitOfMeasure]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        UnitOfMeasure src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
PRINT N'Altering [dbo].[TR_UnitOfMeasure_Update]...';


GO
ALTER TRIGGER [dbo].[TR_UnitOfMeasure_Update] ON [dbo].[UnitOfMeasure]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        UnitOfMeasure src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.8 20160718 TimPN
GO
PRINT N'Creating [dbo].[TR_PhenomenonUOM_Insert]...';


GO
CREATE TRIGGER [dbo].[TR_PhenomenonUOM_Insert] ON [dbo].[PhenomenonUOM]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        PhenomenonUOM src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
PRINT N'Update complete.';


GO
