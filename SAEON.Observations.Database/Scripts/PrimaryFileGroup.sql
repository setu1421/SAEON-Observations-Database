--> Added 2.0.13 20161010 TimPN
/*
Do not change the database path or name variables.
Any sqlcmd variables will be properly substituted during 
build and deployment.
*/
ALTER DATABASE [$(DatabaseName)] ADD FILEGROUP [Primary];
GO
ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [Primary],
		FILENAME = '$(DefaultDataPath)$(DefaultFilePrefix).mdf',
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 1GB
	) TO FILEGROUP [Primary];
--< Added 2.0.13 20161010 TimPN
	
