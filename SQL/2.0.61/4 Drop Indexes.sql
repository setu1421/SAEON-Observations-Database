declare @ltr nvarchar(1024);
SELECT @ltr = ( select 'alter table '+o.name+' drop constraint '+i.name+';'
  from sys.indexes i join sys.objects o on  i.object_id=o.object_id
  where o.type<>'S' and is_primary_key=1 and o.name like 'aspnet_%'
  FOR xml path('') );
select @ltr;
exec sp_executesql @ltr;

declare @qry nvarchar(1024);
select @qry = (select 'drop index '+o.name+'.'+i.name+';'
  from sys.indexes i join sys.objects o on  i.object_id=o.object_id
  where o.type<>'S' and is_primary_key<>1 and index_id>0 and o.name like 'aspnet_%'
for xml path(''));
select @qry;
exec sp_executesql @qry