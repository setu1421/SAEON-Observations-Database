Set NoCount ON

--Alter table RoleModule drop constraint FK_RoleModule_aspnet_Roles;

Declare @schemaName varchar(200)
set @schemaName=''
Declare @constraintName varchar(200)
set @constraintName=''
Declare @tableName varchar(200)
set @tableName=''

While exists
(   
    SELECT c.name
    FROM sys.objects AS c
    INNER JOIN sys.tables AS t
    ON c.parent_object_id = t.[object_id]
    INNER JOIN sys.schemas AS s 
    ON t.[schema_id] = s.[schema_id]
    WHERE c.[type] IN ('D','C','F','PK','UQ')
    and t.[name] NOT IN ('__RefactorLog', 'sysdiagrams')
	and t.name like 'aspnet_%'
    and c.name > @constraintName
)

Begin   
    -- First get the Constraint
    SELECT 
        @constraintName=min(c.name)
    FROM sys.objects AS c
    INNER JOIN sys.tables AS t
    ON c.parent_object_id = t.[object_id]
    INNER JOIN sys.schemas AS s 
    ON t.[schema_id] = s.[schema_id]
    WHERE c.[type] IN ('D','C','F','PK','UQ')
    and t.[name] NOT IN ('__RefactorLog', 'sysdiagrams')
    and t.name like 'aspnet_%'
    and c.name > @constraintName

    -- Then select the Table and Schema associated to the current constraint
    SELECT 
        @tableName = t.name,
        @schemaName = s.name
    FROM sys.objects AS c
    INNER JOIN sys.tables AS t
    ON c.parent_object_id = t.[object_id]
    INNER JOIN sys.schemas AS s 
    ON t.[schema_id] = s.[schema_id]
    WHERE c.name = @constraintName

    -- Then Print to the output and drop the constraint
    Print 'Dropping constraint ' + @constraintName + '...'
    Exec('ALTER TABLE [' + @schemaName + N'].[' + @tableName + N'] DROP CONSTRAINT [' + @constraintName + ']')
End

Set NoCount OFF