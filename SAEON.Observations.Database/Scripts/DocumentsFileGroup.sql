--> Added 2.0.13 20161010 TimPN
/*
Do not change the database path or name variables.
Any sqlcmd variables will be properly substituted during 
build and deployment.
*/
ALTER DATABASE [$(DatabaseName)] ADD FILEGROUP [Documents] CONTAINS FILESTREAM;
GO
ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [Documents],
		FILENAME = '$(DefaultDataPath)$(DefaultFilePrefix)_Documents.ndf'
	) TO FILEGROUP [Documents];
GO
--< Added 2.0.13 20161010 TimPN
