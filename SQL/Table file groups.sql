SELECT    tbl.name AS [Table Name], 
          CASE WHEN dsidx.type='FG' THEN dsidx.name ELSE '(Partitioned)' END AS [File Group] 
FROM      sys.tables AS tbl 
JOIN      sys.indexes AS idx 
ON        idx.object_id = tbl.object_id 
AND       idx.index_id <= 1 
LEFT JOIN sys.data_spaces AS dsidx 
ON        dsidx.data_space_id = idx.data_space_id 
ORDER BY  [Table Name], [File Group]