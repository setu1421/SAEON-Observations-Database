DECLARE @SQLCmd nvarchar(1000) 
DECLARE @Trig varchar(500)
DECLARE @sch varchar(500)

DECLARE TGCursor CURSOR FOR

SELECT ISNULL(tbl.name, vue.name) AS [schemaName]
     , trg.name AS triggerName
FROM sys.triggers trg
LEFT OUTER JOIN (SELECT tparent.object_id, ts.name 
                 FROM sys.tables tparent 
                 INNER JOIN sys.schemas ts ON TS.schema_id = tparent.SCHEMA_ID) 
                 AS tbl ON tbl.OBJECT_ID = trg.parent_id
LEFT OUTER JOIN (SELECT vparent.object_id, vs.name 
                 FROM sys.views vparent 
                 INNER JOIN sys.schemas vs ON vs.schema_id = vparent.SCHEMA_ID) 
                 AS vue ON vue.OBJECT_ID = trg.parent_id
 
OPEN TGCursor
FETCH NEXT FROM TGCursor INTO @sch,@Trig
WHILE @@FETCH_STATUS = 0
BEGIN

SET @SQLCmd = N'DROP TRIGGER [' + @sch + '].[' + @Trig + ']'
EXEC sp_executesql @SQLCmd
PRINT @SQLCmd

FETCH next FROM TGCursor INTO @sch,@Trig
END

CLOSE TGCursor
DEALLOCATE TGCursor

