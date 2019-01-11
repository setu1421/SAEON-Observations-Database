﻿/*
Deployment script for ObservationsSACTN

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "ObservationsSACTN"
:setvar DefaultFilePrefix "ObservationsSACTN"
:setvar DefaultDataPath "D:\Program Files\Microsoft SQL Server\MSSQL14.SAEON\MSSQL\DATA\"
:setvar DefaultLogPath "D:\Program Files\Microsoft SQL Server\MSSQL14.SAEON\MSSQL\DATA\"

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
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET TEMPORAL_HISTORY_RETENTION ON 
            WITH ROLLBACK IMMEDIATE;
    END


GO
PRINT N'Altering [dbo].[DataSourceTransformation]...';


GO
ALTER TABLE [dbo].[DataSourceTransformation]
    ADD [NewPhenomenonID] UNIQUEIDENTIFIER NULL;


GO
PRINT N'Creating [dbo].[DataSourceTransformation].[IX_DataSourceTransformation_NewPhenomenonID]...';


GO
CREATE NONCLUSTERED INDEX [IX_DataSourceTransformation_NewPhenomenonID]
    ON [dbo].[DataSourceTransformation]([NewPhenomenonID] ASC);


GO
PRINT N'Creating [dbo].[FK_DataSourceTransformation_NewPhenomenon]...';


GO
ALTER TABLE [dbo].[DataSourceTransformation] WITH NOCHECK
    ADD CONSTRAINT [FK_DataSourceTransformation_NewPhenomenon] FOREIGN KEY ([NewPhenomenonID]) REFERENCES [dbo].[Phenomenon] ([ID]);


GO
PRINT N'Refreshing [dbo].[vDataLog]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[vDataLog]';


GO
PRINT N'Altering [dbo].[vDataSourceTransformation]...';


GO
ALTER VIEW [dbo].[vDataSourceTransformation]
AS
Select 
  dt.*,
  p.Name as PhenomenonName,
  tt.Name as TransformationName,
  o.Name as OfferingName,
  uom.Unit as UnitOfMeasureUnit,
  np.Name NewPhenomenonName,
  NewOffering.Name NewOfferingName,
  NewUnitOfMeasure.Unit NewUnitOfMeasureUnit,
  tt.iorder
From 
  DataSourceTransformation dt
  INNER JOIN DataSource ds
    on dt.DataSourceID = ds.ID
  INNER JOIN TransformationType tt
    on dt.TransformationTypeID = tt.ID
  INNER JOIN Phenomenon p
    on dt.PhenomenonID = p.ID
  LEFT JOIN PhenomenonOffering po
    on dt.PhenomenonOfferingID = po.ID
  LEFT JOIN Offering o
    on po.OfferingID = o.ID
  LEFT JOIN PhenomenonUOM pu
    on dt.PhenomenonUOMID = pu.ID
  LEFT JOIN UnitOfMeasure uom
    on pu.UnitOfMeasureID = uom.ID
  left join Phenomenon np
    on (dt.NewPhenomenonID = np.ID)
  left join PhenomenonOffering NewPhenomenonOffering
    on (dt.NewPhenomenonOfferingID = NewPhenomenonOffering.ID)
  left join Offering NewOffering
    on (NewPhenomenonOffering.OfferingID = NewOffering.ID)
  left join PhenomenonUOM NewPhenomenonUOM
    on (dt.NewPhenomenonUOMID = NewPhenomenonUOM.ID)
  left join UnitOfMeasure NewUnitOfMeasure
    on (NewPhenomenonUOM.UnitOfMeasureID = NewUnitOfMeasure.ID)
GO
PRINT N'Checking existing data against newly created constraints';


GO
USE [$(DatabaseName)];


GO
ALTER TABLE [dbo].[DataSourceTransformation] WITH CHECK CHECK CONSTRAINT [FK_DataSourceTransformation_NewPhenomenon];


GO
PRINT N'Update complete.';


GO
