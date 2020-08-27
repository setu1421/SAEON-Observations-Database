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
	/// Strongly-typed collection for the UnitOfMeasure class.
	/// </summary>
    [Serializable]
	public partial class UnitOfMeasureCollection : ActiveList<UnitOfMeasure, UnitOfMeasureCollection>
	{	   
		public UnitOfMeasureCollection() {}
        
        /// <summary>
		/// Filters an existing collection based on the set criteria. This is an in-memory filter
		/// Thanks to developingchris for this!
        /// </summary>
        /// <returns>UnitOfMeasureCollection</returns>
		public UnitOfMeasureCollection Filter()
        {
            for (int i = this.Count - 1; i > -1; i--)
            {
                UnitOfMeasure o = this[i];
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
	/// This is an ActiveRecord class which wraps the UnitOfMeasure table.
	/// </summary>
	[Serializable]
	public partial class UnitOfMeasure : ActiveRecord<UnitOfMeasure>, IActiveRecord
	{
		#region .ctors and Default Settings
		
		public UnitOfMeasure()
		{
		  SetSQLProps();
		  InitSetDefaults();
		  MarkNew();
		}
		
		private void InitSetDefaults() { SetDefaults(); }
		
		public UnitOfMeasure(bool useDatabaseDefaults)
		{
			SetSQLProps();
			if(useDatabaseDefaults)
				ForceDefaults();
			MarkNew();
		}
        
		public UnitOfMeasure(object keyID)
		{
			SetSQLProps();
			InitSetDefaults();
			LoadByKey(keyID);
		}
		 
		public UnitOfMeasure(string columnName, object columnValue)
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
				TableSchema.Table schema = new TableSchema.Table("UnitOfMeasure", TableType.Table, DataService.GetInstance("ObservationsDB"));
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
				colvarCode.DataType = DbType.AnsiString;
				colvarCode.MaxLength = 50;
				colvarCode.AutoIncrement = false;
				colvarCode.IsNullable = false;
				colvarCode.IsPrimaryKey = false;
				colvarCode.IsForeignKey = false;
				colvarCode.IsReadOnly = false;
				colvarCode.DefaultSetting = @"";
				colvarCode.ForeignKeyTableName = "";
				schema.Columns.Add(colvarCode);
				
				TableSchema.TableColumn colvarUnit = new TableSchema.TableColumn(schema);
				colvarUnit.ColumnName = "Unit";
				colvarUnit.DataType = DbType.AnsiString;
				colvarUnit.MaxLength = 100;
				colvarUnit.AutoIncrement = false;
				colvarUnit.IsNullable = false;
				colvarUnit.IsPrimaryKey = false;
				colvarUnit.IsForeignKey = false;
				colvarUnit.IsReadOnly = false;
				colvarUnit.DefaultSetting = @"";
				colvarUnit.ForeignKeyTableName = "";
				schema.Columns.Add(colvarUnit);
				
				TableSchema.TableColumn colvarUnitSymbol = new TableSchema.TableColumn(schema);
				colvarUnitSymbol.ColumnName = "UnitSymbol";
				colvarUnitSymbol.DataType = DbType.AnsiString;
				colvarUnitSymbol.MaxLength = 20;
				colvarUnitSymbol.AutoIncrement = false;
				colvarUnitSymbol.IsNullable = false;
				colvarUnitSymbol.IsPrimaryKey = false;
				colvarUnitSymbol.IsForeignKey = false;
				colvarUnitSymbol.IsReadOnly = false;
				colvarUnitSymbol.DefaultSetting = @"";
				colvarUnitSymbol.ForeignKeyTableName = "";
				schema.Columns.Add(colvarUnitSymbol);
				
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
				DataService.Providers["ObservationsDB"].AddSchema("UnitOfMeasure",schema);
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
		public string Code 
		{
			get { return GetColumnValue<string>(Columns.Code); }
			set { SetColumnValue(Columns.Code, value); }
		}
		  
		[XmlAttribute("Unit")]
		[Bindable(true)]
		public string Unit 
		{
			get { return GetColumnValue<string>(Columns.Unit); }
			set { SetColumnValue(Columns.Unit, value); }
		}
		  
		[XmlAttribute("UnitSymbol")]
		[Bindable(true)]
		public string UnitSymbol 
		{
			get { return GetColumnValue<string>(Columns.UnitSymbol); }
			set { SetColumnValue(Columns.UnitSymbol, value); }
		}
		  
		[XmlAttribute("UserId")]
		[Bindable(true)]
		public Guid UserId 
		{
			get { return GetColumnValue<Guid>(Columns.UserId); }
			set { SetColumnValue(Columns.UserId, value); }
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
        
		
		public SAEON.Observations.Data.PhenomenonUOMCollection PhenomenonUOMRecords()
		{
			return new SAEON.Observations.Data.PhenomenonUOMCollection().Where(PhenomenonUOM.Columns.UnitOfMeasureID, Id).Load();
		}
		#endregion
		
			
		
		#region ForeignKey Properties
		
        private SAEON.Observations.Data.AspnetUser _AspnetUser = null;
		/// <summary>
		/// Returns a AspnetUser ActiveRecord object related to this UnitOfMeasure
		/// 
		/// </summary>
		public SAEON.Observations.Data.AspnetUser AspnetUser
		{
//			get { return SAEON.Observations.Data.AspnetUser.FetchByID(this.UserId); }  
			get { return _AspnetUser ?? (_AspnetUser = SAEON.Observations.Data.AspnetUser.FetchByID(this.UserId)); }
			set { SetColumnValue("UserId", value.UserId); }
		}
		
		
		#endregion
		
		
		
		//no ManyToMany tables defined (0)
		
        
        
		#region ObjectDataSource support
		
		
		/// <summary>
		/// Inserts a record, can be used with the Object Data Source
		/// </summary>
		public static void Insert(Guid varId,string varCode,string varUnit,string varUnitSymbol,Guid varUserId,DateTime? varAddedAt,DateTime? varUpdatedAt,byte[] varRowVersion)
		{
			UnitOfMeasure item = new UnitOfMeasure();
			
			item.Id = varId;
			
			item.Code = varCode;
			
			item.Unit = varUnit;
			
			item.UnitSymbol = varUnitSymbol;
			
			item.UserId = varUserId;
			
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
		public static void Update(Guid varId,string varCode,string varUnit,string varUnitSymbol,Guid varUserId,DateTime? varAddedAt,DateTime? varUpdatedAt,byte[] varRowVersion)
		{
			UnitOfMeasure item = new UnitOfMeasure();
			
				item.Id = varId;
			
				item.Code = varCode;
			
				item.Unit = varUnit;
			
				item.UnitSymbol = varUnitSymbol;
			
				item.UserId = varUserId;
			
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
        
        
        
        public static TableSchema.TableColumn UnitColumn
        {
            get { return Schema.Columns[2]; }
        }
        
        
        
        public static TableSchema.TableColumn UnitSymbolColumn
        {
            get { return Schema.Columns[3]; }
        }
        
        
        
        public static TableSchema.TableColumn UserIdColumn
        {
            get { return Schema.Columns[4]; }
        }
        
        
        
        public static TableSchema.TableColumn AddedAtColumn
        {
            get { return Schema.Columns[5]; }
        }
        
        
        
        public static TableSchema.TableColumn UpdatedAtColumn
        {
            get { return Schema.Columns[6]; }
        }
        
        
        
        public static TableSchema.TableColumn RowVersionColumn
        {
            get { return Schema.Columns[7]; }
        }
        
        
        
        #endregion
		#region Columns Struct
		public struct Columns
		{
			 public static string Id = @"ID";
			 public static string Code = @"Code";
			 public static string Unit = @"Unit";
			 public static string UnitSymbol = @"UnitSymbol";
			 public static string UserId = @"UserId";
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
