SELECT 
  object_schema_name(i.Object_ID) + '.'+ object_name(i.Object_ID) AS Thetable,
  convert(DECIMAL(9,2),(sum(a.total_pages) * 8.00) / 1024.00)	AS 'Index_MB',
  max(row_count) AS 'Rows',
  count(*) AS Index_count
FROM sys.indexes i
INNER JOIN
  (SELECT object_ID,Index_ID, sum(rows) AS Row_count 
	 FROM sys.partitions GROUP BY object_ID,Index_ID)f
  ON f.object_ID=i.object_ID AND f.index_ID=i.index_ID
INNER JOIN sys.partitions p 
  ON i.object_id = p.object_id
	AND i.index_id = p.index_id
INNER JOIN sys.allocation_units a 
  ON p.partition_id = a.container_id
  WHERE objectproperty(i.object_id, 'IsUserTable') = 1
GROUP BY i.object_id
order by object_name(i.object_id);