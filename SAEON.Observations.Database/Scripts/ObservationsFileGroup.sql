--> Added 2.0.13 20161010 TimPN
/*
Do not change the database path or name variables.
Any sqlcmd variables will be properly substituted during 
build and deployment.
*/
ALTER DATABASE [$(DatabaseName)] ADD FILEGROUP [Observations];
GO
ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [Observations],
		FILENAME = '$(DefaultDataPath)$(DefaultFilePrefix)_Observations.ndf',
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 1GB
	) TO FILEGROUP [Observations];
GO
--< Added 2.0.13 20161010 TimPN
