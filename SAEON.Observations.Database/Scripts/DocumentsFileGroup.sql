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
		FILENAME = '$(DefaultDataPath)$(DefaultFilePrefix)_Documents.ndf',
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 1GB
	) TO FILEGROUP [Documents];
GO
