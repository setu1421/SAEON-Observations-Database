--> Added 2.0.13 20161010 TimPN
/*
Do not change the database path or name variables.
Any sqlcmd variables will be properly substituted during 
build and deployment.
*/
ALTER DATABASE [$(DatabaseName)] ADD FILEGROUP [Authentication];
GO
ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [Authentication],
		FILENAME = '$(DefaultDataPath)$(DefaultFilePrefix)_Authentication.ndf'
	) TO FILEGROUP [Authentication];
GO
--< Added 2.0.13 20161010 TimPN
