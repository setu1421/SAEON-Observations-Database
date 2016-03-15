
CREATE PROCEDURE [dbo].[ExecuteView]
	@View varchar(50),
	@Schema bit = 0,
	@Export bit = 0,
	@SortColumn varchar(50), -- = 'Phenomenon',
	@SortOrder varchar(4), -- = 'DESC',
	@PageNumber int, --= 1,
	@PageSize int, -- 20,
	@Filter varchar(5000) = '' 
AS

DECLARE @sql NVARCHAR(MAX),
		@paramlist nvarchar(4000);

 SET @sql = ' SET ANSI_WARNINGS OFF SET NOCOUNT ON; ' + CHAR(13)
 
  IF @Schema = 0
   BEGIN
   SET @sql = ' WITH Paging AS (SELECT * ,ROW_NUMBER() OVER (ORDER BY ['+@SortColumn+'] '+ @SortOrder +') AS RowNo FROM  '
   END
  ELSE
   BEGIN
	SET @sql = ' WITH Paging AS (SELECT TOP 1 *,2 As RowNo FROM  '
   END
  
 
 SET @sql = @sql + @View  + ' WHERE 1 = 1 '
 
  IF LEN(@Filter) > 0
  BEGIN
	SET @Sql = @Sql +  @Filter
  END
 
SET  @Sql = @Sql  + ')'  + CHAR(13)
 SET @sql = @sql + ' Select *,(select COUNT(*) from Paging) AS CNT from Paging ' + CHAR(13)
 IF @Export = 0
   BEGIN
	SET @sql = @sql + ' Where RowNo between @PageNumber and @PageNumber + @PageSize '
   END
   


SELECT @paramlist = '@PageNumber int,
					 @PageSize int';
				
 
exec sp_executesql @Sql,@paramlist,
				   @PageNumber,
				   @PageSize
				     





