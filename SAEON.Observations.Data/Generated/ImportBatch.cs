using System; 
using System.Text; 
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration; 
using System.Xml; 
using System.Xml.Serialization;
using SubSonic; 
using SubSonic.Utilities;
// <auto-generated />
namespace SAEON.Observations.Data
{
	/// <summary>
	/// Strongly-typed collection for the ImportBatch class.
	/// </summary>
    [Serializable]
	public partial class ImportBatchCollection : ActiveList<ImportBatch, ImportBatchCollection>
	{	   
		public ImportBatchCollection() {}
        
        /// <summary>
		/// Filters an existing collection based on the set criteria. This is an in-memory filter
		/// Thanks to developingchris for this!
        /// </summary>
        /// <returns>ImportBatchCollection</returns>
		public ImportBatchCollection Filter()
        {
            for (int i = this.Count - 1; i > -1; i--)
            {
                ImportBatch o = this[i];
                foreach (SubSonic.Where w in this.wheres)
                {
                    bool remove = false;
                    System.Reflection.PropertyInfo pi = o.GetType().GetProperty(w.ColumnName);
                    if (pi.CanRead)
                    {
                        object val = pi.GetValue(o, null);
                        switch (w.Comparison)
                        {
                            case SubSonic.Comparison.Equals:
                                if (!val.Equals(w.ParameterValue))
                                {
                                    remove = true;
                                }
                                break;
                        }
                    }
                    if (remove)
                    {
                        this.Remove(o);
                        break;
                    }
                }
            }
            return this;
        }
		
		
	}
	/// <summary>
	/// This is an ActiveRecord class which wraps the ImportBatch table.
	/// </summary>
	[Serializable]
	public partial class ImportBatch : ActiveRecord<ImportBatch>, IActiveRecord
	{
		#region .ctors and Default Settings
		
		public ImportBatch()
		{
		  SetSQLProps();
		  InitSetDefaults();
		  MarkNew();
		}
		
		private void InitSetDefaults() { SetDefaults(); }
		
		public ImportBatch(bool useDatabaseDefaults)
		{
			SetSQLProps();
			if(useDatabaseDefaults)
				ForceDefaults();
			MarkNew();
		}
        
		public ImportBatch(object keyID)
		{
			SetSQLProps();
			InitSetDefaults();
			LoadByKey(keyID);
		}
		 
		public ImportBatch(string columnName, object columnValue)
		{
			SetSQLProps();
			InitSetDefaults();
			LoadByParam(columnName,columnValue);
		}
		
		protected static void SetSQLProps() { GetTableSchema(); }
		
		#endregion
		
		#region Schema and Query Accessor	
		public static Query CreateQuery() { return new Query(Schema); }
		public static TableSchema.Table Schema
		{
			get
			{
				if (BaseSchema == null)
					SetSQLProps();
				return BaseSchema;
			}
		}
		
		private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("ImportBatch", TableType.Table, DataService.GetInstance("ObservationsDB"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				//columns
				
				TableSchema.TableColumn colvarId = new TableSchema.TableColumn(schema);
				colvarId.ColumnName = "ID";
				colvarId.DataType = DbType.Guid;
				colvarId.MaxLength = 0;
				colvarId.AutoIncrement = false;
				colvarId.IsNullable = false;
				colvarId.IsPrimaryKey = true;
				colvarId.IsForeignKey = false;
				colvarId.IsReadOnly = false;
				
						colvarId.DefaultSetting = @"(newid())";
				colvarId.ForeignKeyTableName = "";
				schema.Columns.Add(colvarId);
				
				TableSchema.TableColumn colvarCode = new TableSchema.TableColumn(schema);
				colvarCode.ColumnName = "Code";
				colvarCode.DataType = DbType.Int32;
				colvarCode.MaxLength = 0;
				colvarCode.AutoIncrement = true;
				colvarCode.IsNullable = false;
				colvarCode.IsPrimaryKey = false;
				colvarCode.IsForeignKey = false;
				colvarCode.IsReadOnly = false;
				colvarCode.DefaultSetting = @"";
				colvarCode.ForeignKeyTableName = "";
				schema.Columns.Add(colvarCode);
				
				TableSchema.TableColumn colvarDataSourceID = new TableSchema.TableColumn(schema);
				colvarDataSourceID.ColumnName = "DataSourceID";
				colvarDataSourceID.DataType = DbType.Guid;
				colvarDataSourceID.MaxLength = 0;
				colvarDataSourceID.AutoIncrement = false;
				colvarDataSourceID.IsNullable = false;
				colvarDataSourceID.IsPrimaryKey = false;
				colvarDataSourceID.IsForeignKey = true;
				colvarDataSourceID.IsReadOnly = false;
				colvarDataSourceID.DefaultSetting = @"";
				
					colvarDataSourceID.ForeignKeyTableName = "DataSource";
				schema.Columns.Add(colvarDataSourceID);
				
				TableSchema.TableColumn colvarImportDate = new TableSchema.TableColumn(schema);
				colvarImportDate.ColumnName = "ImportDate";
				colvarImportDate.DataType = DbType.DateTime;
				colvarImportDate.MaxLength = 0;
				colvarImportDate.AutoIncrement = false;
				colvarImportDate.IsNullable = false;
				colvarImportDate.IsPrimaryKey = false;
				colvarImportDate.IsForeignKey = false;
				colvarImportDate.IsReadOnly = false;
				
						colvarImportDate.DefaultSetting = @"(getdate())";
				colvarImportDate.ForeignKeyTableName = "";
				schema.Columns.Add(colvarImportDate);
				
				TableSchema.TableColumn colvarStatus = new TableSchema.TableColumn(schema);
				colvarStatus.ColumnName = "Status";
				colvarStatus.DataType = DbType.Int32;
				colvarStatus.MaxLength = 0;
				colvarStatus.AutoIncrement = false;
				colvarStatus.IsNullable = false;
				colvarStatus.IsPrimaryKey = false;
				colvarStatus.IsForeignKey = false;
				colvarStatus.IsReadOnly = false;
				colvarStatus.DefaultSetting = @"";
				colvarStatus.ForeignKeyTableName = "";
				schema.Columns.Add(colvarStatus);
				
				TableSchema.TableColumn colvarUserId = new TableSchema.TableColumn(schema);
				colvarUserId.ColumnName = "UserId";
				colvarUserId.DataType = DbType.Guid;
				colvarUserId.MaxLength = 0;
				colvarUserId.AutoIncrement = false;
				colvarUserId.IsNullable = false;
				colvarUserId.IsPrimaryKey = false;
				colvarUserId.IsForeignKey = true;
				colvarUserId.IsReadOnly = false;
				colvarUserId.DefaultSetting = @"";
				
					colvarUserId.ForeignKeyTableName = "aspnet_Users";
				schema.Columns.Add(colvarUserId);
				
				TableSchema.TableColumn colvarFileName = new TableSchema.TableColumn(schema);
				colvarFileName.ColumnName = "FileName";
				colvarFileName.DataType = DbType.AnsiString;
				colvarFileName.MaxLength = 250;
				colvarFileName.AutoIncrement = false;
				colvarFileName.IsNullable = true;
				colvarFileName.IsPrimaryKey = false;
				colvarFileName.IsForeignKey = false;
				colvarFileName.IsReadOnly = false;
				colvarFileName.DefaultSetting = @"";
				colvarFileName.ForeignKeyTableName = "";
				schema.Columns.Add(colvarFileName);
				
				TableSchema.TableColumn colvarLogFileName = new TableSchema.TableColumn(schema);
				colvarLogFileName.ColumnName = "LogFileName";
				colvarLogFileName.DataType = DbType.AnsiString;
				colvarLogFileName.MaxLength = 250;
				colvarLogFileName.AutoIncrement = false;
				colvarLogFileName.IsNullable = true;
				colvarLogFileName.IsPrimaryKey = false;
				colvarLogFileName.IsForeignKey = false;
				colvarLogFileName.IsReadOnly = false;
				colvarLogFileName.DefaultSetting = @"";
				colvarLogFileName.ForeignKeyTableName = "";
				schema.Columns.Add(colvarLogFileName);
				
				TableSchema.TableColumn colvarComment = new TableSchema.TableColumn(schema);
				colvarComment.ColumnName = "Comment";
				colvarComment.DataType = DbType.AnsiString;
				colvarComment.MaxLength = 8000;
				colvarComment.AutoIncrement = false;
				colvarComment.IsNullable = true;
				colvarComment.IsPrimaryKey = false;
				colvarComment.IsForeignKey = false;
				colvarComment.IsReadOnly = false;
				colvarComment.DefaultSetting = @"";
				colvarComment.ForeignKeyTableName = "";
				schema.Columns.Add(colvarComment);
				
				TableSchema.TableColumn colvarStatusID = new TableSchema.TableColumn(schema);
				colvarStatusID.ColumnName = "StatusID";
				colvarStatusID.DataType = DbType.Guid;
				colvarStatusID.MaxLength = 0;
				colvarStatusID.AutoIncrement = false;
				colvarStatusID.IsNullable = true;
				colvarStatusID.IsPrimaryKey = false;
				colvarStatusID.IsForeignKey = true;
				colvarStatusID.IsReadOnly = false;
				colvarStatusID.DefaultSetting = @"";
				
					colvarStatusID.ForeignKeyTableName = "Status";
				schema.Columns.Add(colvarStatusID);
				
				TableSchema.TableColumn colvarStatusReasonID = new TableSchema.TableColumn(schema);
				colvarStatusReasonID.ColumnName = "StatusReasonID";
				colvarStatusReasonID.DataType = DbType.Guid;
				colvarStatusReasonID.MaxLength = 0;
				colvarStatusReasonID.AutoIncrement = false;
				colvarStatusReasonID.IsNullable = true;
				colvarStatusReasonID.IsPrimaryKey = false;
				colvarStatusReasonID.IsForeignKey = true;
				colvarStatusReasonID.IsReadOnly = false;
				colvarStatusReasonID.DefaultSetting = @"";
				
					colvarStatusReasonID.ForeignKeyTableName = "StatusReason";
				schema.Columns.Add(colvarStatusReasonID);
				
				TableSchema.TableColumn colvarIssues = new TableSchema.TableColumn(schema);
				colvarIssues.ColumnName = "Issues";
				colvarIssues.DataType = DbType.AnsiString;
				colvarIssues.MaxLength = 1000;
				colvarIssues.AutoIncrement = false;
				colvarIssues.IsNullable = true;
				colvarIssues.IsPrimaryKey = false;
				colvarIssues.IsForeignKey = false;
				colvarIssues.IsReadOnly = false;
				colvarIssues.DefaultSetting = @"";
				colvarIssues.ForeignKeyTableName = "";
				schema.Columns.Add(colvarIssues);
				
				TableSchema.TableColumn colvarAddedAt = new TableSchema.TableColumn(schema);
				colvarAddedAt.ColumnName = "AddedAt";
				colvarAddedAt.DataType = DbType.DateTime;
				colvarAddedAt.MaxLength = 0;
				colvarAddedAt.AutoIncrement = false;
				colvarAddedAt.IsNullable = true;
				colvarAddedAt.IsPrimaryKey = false;
				colvarAddedAt.IsForeignKey = false;
				colvarAddedAt.IsReadOnly = false;
				
						colvarAddedAt.DefaultSetting = @"(getdate())";
				colvarAddedAt.ForeignKeyTableName = "";
				schema.Columns.Add(colvarAddedAt);
				
				TableSchema.TableColumn colvarUpdatedAt = new TableSchema.TableColumn(schema);
				colvarUpdatedAt.ColumnName = "UpdatedAt";
				colvarUpdatedAt.DataType = DbType.DateTime;
				colvarUpdatedAt.MaxLength = 0;
				colvarUpdatedAt.AutoIncrement = false;
				colvarUpdatedAt.IsNullable = true;
				colvarUpdatedAt.IsPrimaryKey = false;
				colvarUpdatedAt.IsForeignKey = false;
				colvarUpdatedAt.IsReadOnly = false;
				
						colvarUpdatedAt.DefaultSetting = @"(getdate())";
				colvarUpdatedAt.ForeignKeyTableName = "";
				schema.Columns.Add(colvarUpdatedAt);
				
				TableSchema.TableColumn colvarRowVersion = new TableSchema.TableColumn(schema);
				colvarRowVersion.ColumnName = "RowVersion";
				colvarRowVersion.DataType = DbType.Binary;
				colvarRowVersion.MaxLength = 0;
				colvarRowVersion.AutoIncrement = false;
				colvarRowVersion.IsNullable = false;
				colvarRowVersion.IsPrimaryKey = false;
				colvarRowVersion.IsForeignKey = false;
				colvarRowVersion.IsReadOnly = true;
				colvarRowVersion.DefaultSetting = @"";
				colvarRowVersion.ForeignKeyTableName = "";
				schema.Columns.Add(colvarRowVersion);
				
				BaseSchema = schema;
				//add this schema to the provider
				//so we can query it later
				DataService.Providers["ObservationsDB"].AddSchema("ImportBatch",schema);
			}
		}
		#endregion
		
		#region Props
		  
		[XmlAttribute("Id")]
		[Bindable(true)]
		public Guid Id 
		{
			get { return GetColumnValue<Guid>(Columns.Id); }
			set { SetColumnValue(Columns.Id, value); }
		}
		  
		[XmlAttribute("Code")]
		[Bindable(true)]
		public int Code 
		{
			get { return GetColumnValue<int>(Columns.Code); }
			set { SetColumnValue(Columns.Code, value); }
		}
		  
		[XmlAttribute("DataSourceID")]
		[Bindable(true)]
		public Guid DataSourceID 
		{
			get { return GetColumnValue<Guid>(Columns.DataSourceID); }
			set { SetColumnValue(Columns.DataSourceID, value); }
		}
		  
		[XmlAttribute("ImportDate")]
		[Bindable(true)]
		public DateTime ImportDate 
		{
			get { return GetColumnValue<DateTime>(Columns.ImportDate); }
			set { SetColumnValue(Columns.ImportDate, value); }
		}
		  
		[XmlAttribute("Status")]
		[Bindable(true)]
		public int Status 
		{
			get { return GetColumnValue<int>(Columns.Status); }
			set { SetColumnValue(Columns.Status, value); }
		}
		  
		[XmlAttribute("UserId")]
		[Bindable(true)]
		public Guid UserId 
		{
			get { return GetColumnValue<Guid>(Columns.UserId); }
			set { SetColumnValue(Columns.UserId, value); }
		}
		  
		[XmlAttribute("FileName")]
		[Bindable(true)]
		public string FileName 
		{
			get { return GetColumnValue<string>(Columns.FileName); }
			set { SetColumnValue(Columns.FileName, value); }
		}
		  
		[XmlAttribute("LogFileName")]
		[Bindable(true)]
		public string LogFileName 
		{
			get { return GetColumnValue<string>(Columns.LogFileName); }
			set { SetColumnValue(Columns.LogFileName, value); }
		}
		  
		[XmlAttribute("Comment")]
		[Bindable(true)]
		public string Comment 
		{
			get { return GetColumnValue<string>(Columns.Comment); }
			set { SetColumnValue(Columns.Comment, value); }
		}
		  
		[XmlAttribute("StatusID")]
		[Bindable(true)]
		public Guid? StatusID 
		{
			get { return GetColumnValue<Guid?>(Columns.StatusID); }
			set { SetColumnValue(Columns.StatusID, value); }
		}
		  
		[XmlAttribute("StatusReasonID")]
		[Bindable(true)]
		public Guid? StatusReasonID 
		{
			get { return GetColumnValue<Guid?>(Columns.StatusReasonID); }
			set { SetColumnValue(Columns.StatusReasonID, value); }
		}
		  
		[XmlAttribute("Issues")]
		[Bindable(true)]
		public string Issues 
		{
			get { return GetColumnValue<string>(Columns.Issues); }
			set { SetColumnValue(Columns.Issues, value); }
		}
		  
		[XmlAttribute("AddedAt")]
		[Bindable(true)]
		public DateTime? AddedAt 
		{
			get { return GetColumnValue<DateTime?>(Columns.AddedAt); }
			set { SetColumnValue(Columns.AddedAt, value); }
		}
		  
		[XmlAttribute("UpdatedAt")]
		[Bindable(true)]
		public DateTime? UpdatedAt 
		{
			get { return GetColumnValue<DateTime?>(Columns.UpdatedAt); }
			set { SetColumnValue(Columns.UpdatedAt, value); }
		}
		  
		[XmlAttribute("RowVersion")]
		[Bindable(true)]
		public byte[] RowVersion 
		{
			get { return GetColumnValue<byte[]>(Columns.RowVersion); }
			set { SetColumnValue(Columns.RowVersion, value); }
		}
		
		#endregion
		
		
		#region PrimaryKey Methods		
		
        protected override void SetPrimaryKey(object oValue)
        {
            base.SetPrimaryKey(oValue);
            
            SetPKValues();
        }
        
		
		public SAEON.Observations.Data.DataLogCollection DataLogRecords()
		{
			return new SAEON.Observations.Data.DataLogCollection().Where(DataLog.Columns.ImportBatchID, Id).Load();
		}
		public SAEON.Observations.Data.ImportBatchSummaryCollection ImportBatchSummaryRecords()
		{
			return new SAEON.Observations.Data.ImportBatchSummaryCollection().Where(ImportBatchSummary.Columns.ImportBatchID, Id).Load();
		}
		public SAEON.Observations.Data.ObservationCollection ObservationRecords()
		{
			return new SAEON.Observations.Data.ObservationCollection().Where(Observation.Columns.ImportBatchID, Id).Load();
		}
		#endregion
		
			
		
		#region ForeignKey Properties
		
		/// <summary>
		/// Returns a AspnetUser ActiveRecord object related to this ImportBatch
		/// 
		/// </summary>
		public SAEON.Observations.Data.AspnetUser AspnetUser
		{
			get { return SAEON.Observations.Data.AspnetUser.FetchByID(this.UserId); }
			set { SetColumnValue("UserId", value.UserId); }
		}
		
		
		/// <summary>
		/// Returns a DataSource ActiveRecord object related to this ImportBatch
		/// 
		/// </summary>
		public SAEON.Observations.Data.DataSource DataSource
		{
			get { return SAEON.Observations.Data.DataSource.FetchByID(this.DataSourceID); }
			set { SetColumnValue("DataSourceID", value.Id); }
		}
		
		
		/// <summary>
		/// Returns a Status ActiveRecord object related to this ImportBatch
		/// 
		/// </summary>
		public SAEON.Observations.Data.Status StatusRecord
		{
			get { return SAEON.Observations.Data.Status.FetchByID(this.StatusID); }
			set { SetColumnValue("StatusID", value.Id); }
		}
		
		
		/// <summary>
		/// Returns a StatusReason ActiveRecord object related to this ImportBatch
		/// 
		/// </summary>
		public SAEON.Observations.Data.StatusReason StatusReason
		{
			get { return SAEON.Observations.Data.StatusReason.FetchByID(this.StatusReasonID); }
			set { SetColumnValue("StatusReasonID", value.Id); }
		}
		
		
		#endregion
		
		
		
		//no ManyToMany tables defined (0)
		
        
        
		#region ObjectDataSource support
		
		
		/// <summary>
		/// Inserts a record, can be used with the Object Data Source
		/// </summary>
		public static void Insert(Guid varId,Guid varDataSourceID,DateTime varImportDate,int varStatus,Guid varUserId,string varFileName,string varLogFileName,string varComment,Guid? varStatusID,Guid? varStatusReasonID,string varIssues,DateTime? varAddedAt,DateTime? varUpdatedAt,byte[] varRowVersion)
		{
			ImportBatch item = new ImportBatch();
			
			item.Id = varId;
			
			item.DataSourceID = varDataSourceID;
			
			item.ImportDate = varImportDate;
			
			item.Status = varStatus;
			
			item.UserId = varUserId;
			
			item.FileName = varFileName;
			
			item.LogFileName = varLogFileName;
			
			item.Comment = varComment;
			
			item.StatusID = varStatusID;
			
			item.StatusReasonID = varStatusReasonID;
			
			item.Issues = varIssues;
			
			item.AddedAt = varAddedAt;
			
			item.UpdatedAt = varUpdatedAt;
			
			item.RowVersion = varRowVersion;
			
		
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		
		/// <summary>
		/// Updates a record, can be used with the Object Data Source
		/// </summary>
		public static void Update(Guid varId,int varCode,Guid varDataSourceID,DateTime varImportDate,int varStatus,Guid varUserId,string varFileName,string varLogFileName,string varComment,Guid? varStatusID,Guid? varStatusReasonID,string varIssues,DateTime? varAddedAt,DateTime? varUpdatedAt,byte[] varRowVersion)
		{
			ImportBatch item = new ImportBatch();
			
				item.Id = varId;
			
				item.Code = varCode;
			
				item.DataSourceID = varDataSourceID;
			
				item.ImportDate = varImportDate;
			
				item.Status = varStatus;
			
				item.UserId = varUserId;
			
				item.FileName = varFileName;
			
				item.LogFileName = varLogFileName;
			
				item.Comment = varComment;
			
				item.StatusID = varStatusID;
			
				item.StatusReasonID = varStatusReasonID;
			
				item.Issues = varIssues;
			
				item.AddedAt = varAddedAt;
			
				item.UpdatedAt = varUpdatedAt;
			
				item.RowVersion = varRowVersion;
			
			item.IsNew = false;
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		#endregion
        
        
        
        #region Typed Columns
        
        
        public static TableSchema.TableColumn IdColumn
        {
            get { return Schema.Columns[0]; }
        }
        
        
        
        public static TableSchema.TableColumn CodeColumn
        {
            get { return Schema.Columns[1]; }
        }
        
        
        
        public static TableSchema.TableColumn DataSourceIDColumn
        {
            get { return Schema.Columns[2]; }
        }
        
        
        
        public static TableSchema.TableColumn ImportDateColumn
        {
            get { return Schema.Columns[3]; }
        }
        
        
        
        public static TableSchema.TableColumn StatusColumn
        {
            get { return Schema.Columns[4]; }
        }
        
        
        
        public static TableSchema.TableColumn UserIdColumn
        {
            get { return Schema.Columns[5]; }
        }
        
        
        
        public static TableSchema.TableColumn FileNameColumn
        {
            get { return Schema.Columns[6]; }
        }
        
        
        
        public static TableSchema.TableColumn LogFileNameColumn
        {
            get { return Schema.Columns[7]; }
        }
        
        
        
        public static TableSchema.TableColumn CommentColumn
        {
            get { return Schema.Columns[8]; }
        }
        
        
        
        public static TableSchema.TableColumn StatusIDColumn
        {
            get { return Schema.Columns[9]; }
        }
        
        
        
        public static TableSchema.TableColumn StatusReasonIDColumn
        {
            get { return Schema.Columns[10]; }
        }
        
        
        
        public static TableSchema.TableColumn IssuesColumn
        {
            get { return Schema.Columns[11]; }
        }
        
        
        
        public static TableSchema.TableColumn AddedAtColumn
        {
            get { return Schema.Columns[12]; }
        }
        
        
        
        public static TableSchema.TableColumn UpdatedAtColumn
        {
            get { return Schema.Columns[13]; }
        }
        
        
        
        public static TableSchema.TableColumn RowVersionColumn
        {
            get { return Schema.Columns[14]; }
        }
        
        
        
        #endregion
		#region Columns Struct
		public struct Columns
		{
			 public static string Id = @"ID";
			 public static string Code = @"Code";
			 public static string DataSourceID = @"DataSourceID";
			 public static string ImportDate = @"ImportDate";
			 public static string Status = @"Status";
			 public static string UserId = @"UserId";
			 public static string FileName = @"FileName";
			 public static string LogFileName = @"LogFileName";
			 public static string Comment = @"Comment";
			 public static string StatusID = @"StatusID";
			 public static string StatusReasonID = @"StatusReasonID";
			 public static string Issues = @"Issues";
			 public static string AddedAt = @"AddedAt";
			 public static string UpdatedAt = @"UpdatedAt";
			 public static string RowVersion = @"RowVersion";
						
		}
		#endregion
		
		#region Update PK Collections
		
        public void SetPKValues()
        {
}
        #endregion
    
        #region Deep Save
		
        public void DeepSave()
        {
            Save();
            
}
        #endregion
	}
}
