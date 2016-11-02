--> Added 2.0.13 20161010 TimPN
/*
Do not change the database path or name variables.
Any sqlcmd variables will be properly substituted during 
build and deployment.
*/
--GO
--ALTER DATABASE [$(DatabaseName)] ADD FILEGROUP [PRIMARY];
--GO
--ALTER DATABASE [$(DatabaseName)]
--	ADD FILE
--	(
--		NAME = [PRIMARY],
--		FILENAME = '$(DefaultDataPath)$(DefaultFilePrefix).mdf',
--		MAXSIZE = UNLIMITED,
--		FILEGROWTH = 1GB
--	) TO FILEGROUP [PRIMARY];
--GO
--ALTER DATABASE [$(DatabaseName)] MODIFY FILEGROUP [PRIMARY] DEFAULT;
--GO
--< Added 2.0.13 20161010 TimPN
	
