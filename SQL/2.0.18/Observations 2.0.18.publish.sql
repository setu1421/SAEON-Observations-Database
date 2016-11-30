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
USE [$(DatabaseName)];


GO
PRINT N'Altering [dbo].[ImportBatch]...';


GO
ALTER TABLE [dbo].[ImportBatch]
    ADD [SourceFile] VARBINARY (MAX) FILESTREAM NULL,
        [Pass1File]  VARBINARY (MAX) FILESTREAM NULL,
        [Pass2File]  VARBINARY (MAX) FILESTREAM NULL,
        [Pass3File]  VARBINARY (MAX) FILESTREAM NULL,
        [Pass4File]  VARBINARY (MAX) FILESTREAM NULL;


GO
PRINT N'Refreshing [dbo].[vImportBatch]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vImportBatch]';


GO
PRINT N'Update complete.';


GO
