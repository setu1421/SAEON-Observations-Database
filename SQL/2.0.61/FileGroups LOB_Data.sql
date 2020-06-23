SELECT
    au.*,
    ds.name AS [data_space_name],
    ds.type AS [data_space_type],
    p.rows,
    o.name AS [object_name]
FROM sys.allocation_units au
    INNER JOIN sys.data_spaces ds
        ON au.data_space_id = ds.data_space_id
    INNER JOIN sys.partitions p
        ON au.container_id = p.partition_id
    INNER JOIN sys.objects o
        ON p.object_id = o.object_id
WHERE au.type_desc = 'LOB_DATA'