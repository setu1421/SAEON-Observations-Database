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
	/// Strongly-typed collection for the SchemaColumn class.
	/// </summary>
    [Serializable]
	public partial class SchemaColumnCollection : ActiveList<SchemaColumn, SchemaColumnCollection>
	{	   
		public SchemaColumnCollection() {}
        
        /// <summary>
		/// Filters an existing collection based on the set criteria. This is an in-memory filter
		/// Thanks to developingchris for this!
        /// </summary>
        /// <returns>SchemaColumnCollection</returns>
		public SchemaColumnCollection Filter()
        {
            for (int i = this.Count - 1; i > -1; i--)
            {
                SchemaColumn o = this[i];
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
	/// This is an ActiveRecord class which wraps the SchemaColumn table.
	/// </summary>
	[Serializable]
	public partial class SchemaColumn : ActiveRecord<SchemaColumn>, IActiveRecord
	{
		#region .ctors and Default Settings
		
		public SchemaColumn()
		{
		  SetSQLProps();
		  InitSetDefaults();
		  MarkNew();
		}
		
		private void InitSetDefaults() { SetDefaults(); }
		
		public SchemaColumn(bool useDatabaseDefaults)
		{
			SetSQLProps();
			if(useDatabaseDefaults)
				ForceDefaults();
			MarkNew();
		}
        
		public SchemaColumn(object keyID)
		{
			SetSQLProps();
			InitSetDefaults();
			LoadByKey(keyID);
		}
		 
		public SchemaColumn(string columnName, object columnValue)
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
				TableSchema.Table schema = new TableSchema.Table("SchemaColumn", TableType.Table, DataService.GetInstance("ObservationsDB"));
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
				
				TableSchema.TableColumn colvarDataSchemaID = new TableSchema.TableColumn(schema);
				colvarDataSchemaID.ColumnName = "DataSchemaID";
				colvarDataSchemaID.DataType = DbType.Guid;
				colvarDataSchemaID.MaxLength = 0;
				colvarDataSchemaID.AutoIncrement = false;
				colvarDataSchemaID.IsNullable = false;
				colvarDataSchemaID.IsPrimaryKey = false;
				colvarDataSchemaID.IsForeignKey = true;
				colvarDataSchemaID.IsReadOnly = false;
				colvarDataSchemaID.DefaultSetting = @"";
				
					colvarDataSchemaID.ForeignKeyTableName = "DataSchema";
				schema.Columns.Add(colvarDataSchemaID);
				
				TableSchema.TableColumn colvarNumber = new TableSchema.TableColumn(schema);
				colvarNumber.ColumnName = "Number";
				colvarNumber.DataType = DbType.Int32;
				colvarNumber.MaxLength = 0;
				colvarNumber.AutoIncrement = false;
				colvarNumber.IsNullable = false;
				colvarNumber.IsPrimaryKey = false;
				colvarNumber.IsForeignKey = false;
				colvarNumber.IsReadOnly = false;
				colvarNumber.DefaultSetting = @"";
				colvarNumber.ForeignKeyTableName = "";
				schema.Columns.Add(colvarNumber);
				
				TableSchema.TableColumn colvarName = new TableSchema.TableColumn(schema);
				colvarName.ColumnName = "Name";
				colvarName.DataType = DbType.AnsiString;
				colvarName.MaxLength = 100;
				colvarName.AutoIncrement = false;
				colvarName.IsNullable = false;
				colvarName.IsPrimaryKey = false;
				colvarName.IsForeignKey = false;
				colvarName.IsReadOnly = false;
				colvarName.DefaultSetting = @"";
				colvarName.ForeignKeyTableName = "";
				schema.Columns.Add(colvarName);
				
				TableSchema.TableColumn colvarSchemaColumnTypeID = new TableSchema.TableColumn(schema);
				colvarSchemaColumnTypeID.ColumnName = "SchemaColumnTypeID";
				colvarSchemaColumnTypeID.DataType = DbType.Guid;
				colvarSchemaColumnTypeID.MaxLength = 0;
				colvarSchemaColumnTypeID.AutoIncrement = false;
				colvarSchemaColumnTypeID.IsNullable = false;
				colvarSchemaColumnTypeID.IsPrimaryKey = false;
				colvarSchemaColumnTypeID.IsForeignKey = true;
				colvarSchemaColumnTypeID.IsReadOnly = false;
				colvarSchemaColumnTypeID.DefaultSetting = @"";
				
					colvarSchemaColumnTypeID.ForeignKeyTableName = "SchemaColumnType";
				schema.Columns.Add(colvarSchemaColumnTypeID);
				
				TableSchema.TableColumn colvarWidth = new TableSchema.TableColumn(schema);
				colvarWidth.ColumnName = "Width";
				colvarWidth.DataType = DbType.Int32;
				colvarWidth.MaxLength = 0;
				colvarWidth.AutoIncrement = false;
				colvarWidth.IsNullable = true;
				colvarWidth.IsPrimaryKey = false;
				colvarWidth.IsForeignKey = false;
				colvarWidth.IsReadOnly = false;
				colvarWidth.DefaultSetting = @"";
				colvarWidth.ForeignKeyTableName = "";
				schema.Columns.Add(colvarWidth);
				
				TableSchema.TableColumn colvarFormat = new TableSchema.TableColumn(schema);
				colvarFormat.ColumnName = "Format";
				colvarFormat.DataType = DbType.AnsiString;
				colvarFormat.MaxLength = 50;
				colvarFormat.AutoIncrement = false;
				colvarFormat.IsNullable = true;
				colvarFormat.IsPrimaryKey = false;
				colvarFormat.IsForeignKey = false;
				colvarFormat.IsReadOnly = false;
				colvarFormat.DefaultSetting = @"";
				colvarFormat.ForeignKeyTableName = "";
				schema.Columns.Add(colvarFormat);
				
				TableSchema.TableColumn colvarPhenomenonID = new TableSchema.TableColumn(schema);
				colvarPhenomenonID.ColumnName = "PhenomenonID";
				colvarPhenomenonID.DataType = DbType.Guid;
				colvarPhenomenonID.MaxLength = 0;
				colvarPhenomenonID.AutoIncrement = false;
				colvarPhenomenonID.IsNullable = true;
				colvarPhenomenonID.IsPrimaryKey = false;
				colvarPhenomenonID.IsForeignKey = true;
				colvarPhenomenonID.IsReadOnly = false;
				colvarPhenomenonID.DefaultSetting = @"";
				
					colvarPhenomenonID.ForeignKeyTableName = "Phenomenon";
				schema.Columns.Add(colvarPhenomenonID);
				
				TableSchema.TableColumn colvarPhenomenonOfferingID = new TableSchema.TableColumn(schema);
				colvarPhenomenonOfferingID.ColumnName = "PhenomenonOfferingID";
				colvarPhenomenonOfferingID.DataType = DbType.Guid;
				colvarPhenomenonOfferingID.MaxLength = 0;
				colvarPhenomenonOfferingID.AutoIncrement = false;
				colvarPhenomenonOfferingID.IsNullable = true;
				colvarPhenomenonOfferingID.IsPrimaryKey = false;
				colvarPhenomenonOfferingID.IsForeignKey = true;
				colvarPhenomenonOfferingID.IsReadOnly = false;
				colvarPhenomenonOfferingID.DefaultSetting = @"";
				
					colvarPhenomenonOfferingID.ForeignKeyTableName = "PhenomenonOffering";
				schema.Columns.Add(colvarPhenomenonOfferingID);
				
				TableSchema.TableColumn colvarPhenomenonUOMID = new TableSchema.TableColumn(schema);
				colvarPhenomenonUOMID.ColumnName = "PhenomenonUOMID";
				colvarPhenomenonUOMID.DataType = DbType.Guid;
				colvarPhenomenonUOMID.MaxLength = 0;
				colvarPhenomenonUOMID.AutoIncrement = false;
				colvarPhenomenonUOMID.IsNullable = true;
				colvarPhenomenonUOMID.IsPrimaryKey = false;
				colvarPhenomenonUOMID.IsForeignKey = true;
				colvarPhenomenonUOMID.IsReadOnly = false;
				colvarPhenomenonUOMID.DefaultSetting = @"";
				
					colvarPhenomenonUOMID.ForeignKeyTableName = "PhenomenonUOM";
				schema.Columns.Add(colvarPhenomenonUOMID);
				
				TableSchema.TableColumn colvarFixedTime = new TableSchema.TableColumn(schema);
				colvarFixedTime.ColumnName = "FixedTime";
				colvarFixedTime.DataType = DbType.Int32;
				colvarFixedTime.MaxLength = 0;
				colvarFixedTime.AutoIncrement = false;
				colvarFixedTime.IsNullable = true;
				colvarFixedTime.IsPrimaryKey = false;
				colvarFixedTime.IsForeignKey = false;
				colvarFixedTime.IsReadOnly = false;
				colvarFixedTime.DefaultSetting = @"";
				colvarFixedTime.ForeignKeyTableName = "";
				schema.Columns.Add(colvarFixedTime);
				
				TableSchema.TableColumn colvarEmptyValue = new TableSchema.TableColumn(schema);
				colvarEmptyValue.ColumnName = "EmptyValue";
				colvarEmptyValue.DataType = DbType.AnsiString;
				colvarEmptyValue.MaxLength = 50;
				colvarEmptyValue.AutoIncrement = false;
				colvarEmptyValue.IsNullable = true;
				colvarEmptyValue.IsPrimaryKey = false;
				colvarEmptyValue.IsForeignKey = false;
				colvarEmptyValue.IsReadOnly = false;
				colvarEmptyValue.DefaultSetting = @"";
				colvarEmptyValue.ForeignKeyTableName = "";
				schema.Columns.Add(colvarEmptyValue);
				
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
				
				BaseSchema = schema;
				//add this schema to the provider
				//so we can query it later
				DataService.Providers["ObservationsDB"].AddSchema("SchemaColumn",schema);
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
		  
		[XmlAttribute("DataSchemaID")]
		[Bindable(true)]
		public Guid DataSchemaID 
		{
			get { return GetColumnValue<Guid>(Columns.DataSchemaID); }
			set { SetColumnValue(Columns.DataSchemaID, value); }
		}
		  
		[XmlAttribute("Number")]
		[Bindable(true)]
		public int Number 
		{
			get { return GetColumnValue<int>(Columns.Number); }
			set { SetColumnValue(Columns.Number, value); }
		}
		  
		[XmlAttribute("Name")]
		[Bindable(true)]
		public string Name 
		{
			get { return GetColumnValue<string>(Columns.Name); }
			set { SetColumnValue(Columns.Name, value); }
		}
		  
		[XmlAttribute("SchemaColumnTypeID")]
		[Bindable(true)]
		public Guid SchemaColumnTypeID 
		{
			get { return GetColumnValue<Guid>(Columns.SchemaColumnTypeID); }
			set { SetColumnValue(Columns.SchemaColumnTypeID, value); }
		}
		  
		[XmlAttribute("Width")]
		[Bindable(true)]
		public int? Width 
		{
			get { return GetColumnValue<int?>(Columns.Width); }
			set { SetColumnValue(Columns.Width, value); }
		}
		  
		[XmlAttribute("Format")]
		[Bindable(true)]
		public string Format 
		{
			get { return GetColumnValue<string>(Columns.Format); }
			set { SetColumnValue(Columns.Format, value); }
		}
		  
		[XmlAttribute("PhenomenonID")]
		[Bindable(true)]
		public Guid? PhenomenonID 
		{
			get { return GetColumnValue<Guid?>(Columns.PhenomenonID); }
			set { SetColumnValue(Columns.PhenomenonID, value); }
		}
		  
		[XmlAttribute("PhenomenonOfferingID")]
		[Bindable(true)]
		public Guid? PhenomenonOfferingID 
		{
			get { return GetColumnValue<Guid?>(Columns.PhenomenonOfferingID); }
			set { SetColumnValue(Columns.PhenomenonOfferingID, value); }
		}
		  
		[XmlAttribute("PhenomenonUOMID")]
		[Bindable(true)]
		public Guid? PhenomenonUOMID 
		{
			get { return GetColumnValue<Guid?>(Columns.PhenomenonUOMID); }
			set { SetColumnValue(Columns.PhenomenonUOMID, value); }
		}
		  
		[XmlAttribute("FixedTime")]
		[Bindable(true)]
		public int? FixedTime 
		{
			get { return GetColumnValue<int?>(Columns.FixedTime); }
			set { SetColumnValue(Columns.FixedTime, value); }
		}
		  
		[XmlAttribute("EmptyValue")]
		[Bindable(true)]
		public string EmptyValue 
		{
			get { return GetColumnValue<string>(Columns.EmptyValue); }
			set { SetColumnValue(Columns.EmptyValue, value); }
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
		
		#endregion
		
		
			
		
		#region ForeignKey Properties
		
		/// <summary>
		/// Returns a AspnetUser ActiveRecord object related to this SchemaColumn
		/// 
		/// </summary>
		public SAEON.Observations.Data.AspnetUser AspnetUser
		{
			get { return SAEON.Observations.Data.AspnetUser.FetchByID(this.UserId); }
			set { SetColumnValue("UserId", value.UserId); }
		}
		
		
		/// <summary>
		/// Returns a DataSchema ActiveRecord object related to this SchemaColumn
		/// 
		/// </summary>
		public SAEON.Observations.Data.DataSchema DataSchema
		{
			get { return SAEON.Observations.Data.DataSchema.FetchByID(this.DataSchemaID); }
			set { SetColumnValue("DataSchemaID", value.Id); }
		}
		
		
		/// <summary>
		/// Returns a Phenomenon ActiveRecord object related to this SchemaColumn
		/// 
		/// </summary>
		public SAEON.Observations.Data.Phenomenon Phenomenon
		{
			get { return SAEON.Observations.Data.Phenomenon.FetchByID(this.PhenomenonID); }
			set { SetColumnValue("PhenomenonID", value.Id); }
		}
		
		
		/// <summary>
		/// Returns a PhenomenonOffering ActiveRecord object related to this SchemaColumn
		/// 
		/// </summary>
		public SAEON.Observations.Data.PhenomenonOffering PhenomenonOffering
		{
			get { return SAEON.Observations.Data.PhenomenonOffering.FetchByID(this.PhenomenonOfferingID); }
			set { SetColumnValue("PhenomenonOfferingID", value.Id); }
		}
		
		
		/// <summary>
		/// Returns a PhenomenonUOM ActiveRecord object related to this SchemaColumn
		/// 
		/// </summary>
		public SAEON.Observations.Data.PhenomenonUOM PhenomenonUOM
		{
			get { return SAEON.Observations.Data.PhenomenonUOM.FetchByID(this.PhenomenonUOMID); }
			set { SetColumnValue("PhenomenonUOMID", value.Id); }
		}
		
		
		/// <summary>
		/// Returns a SchemaColumnType ActiveRecord object related to this SchemaColumn
		/// 
		/// </summary>
		public SAEON.Observations.Data.SchemaColumnType SchemaColumnType
		{
			get { return SAEON.Observations.Data.SchemaColumnType.FetchByID(this.SchemaColumnTypeID); }
			set { SetColumnValue("SchemaColumnTypeID", value.Id); }
		}
		
		
		#endregion
		
		
		
		//no ManyToMany tables defined (0)
		
        
        
		#region ObjectDataSource support
		
		
		/// <summary>
		/// Inserts a record, can be used with the Object Data Source
		/// </summary>
		public static void Insert(Guid varId,Guid varDataSchemaID,int varNumber,string varName,Guid varSchemaColumnTypeID,int? varWidth,string varFormat,Guid? varPhenomenonID,Guid? varPhenomenonOfferingID,Guid? varPhenomenonUOMID,int? varFixedTime,string varEmptyValue,Guid varUserId,DateTime? varAddedAt,DateTime? varUpdatedAt)
		{
			SchemaColumn item = new SchemaColumn();
			
			item.Id = varId;
			
			item.DataSchemaID = varDataSchemaID;
			
			item.Number = varNumber;
			
			item.Name = varName;
			
			item.SchemaColumnTypeID = varSchemaColumnTypeID;
			
			item.Width = varWidth;
			
			item.Format = varFormat;
			
			item.PhenomenonID = varPhenomenonID;
			
			item.PhenomenonOfferingID = varPhenomenonOfferingID;
			
			item.PhenomenonUOMID = varPhenomenonUOMID;
			
			item.FixedTime = varFixedTime;
			
			item.EmptyValue = varEmptyValue;
			
			item.UserId = varUserId;
			
			item.AddedAt = varAddedAt;
			
			item.UpdatedAt = varUpdatedAt;
			
		
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		
		/// <summary>
		/// Updates a record, can be used with the Object Data Source
		/// </summary>
		public static void Update(Guid varId,Guid varDataSchemaID,int varNumber,string varName,Guid varSchemaColumnTypeID,int? varWidth,string varFormat,Guid? varPhenomenonID,Guid? varPhenomenonOfferingID,Guid? varPhenomenonUOMID,int? varFixedTime,string varEmptyValue,Guid varUserId,DateTime? varAddedAt,DateTime? varUpdatedAt)
		{
			SchemaColumn item = new SchemaColumn();
			
				item.Id = varId;
			
				item.DataSchemaID = varDataSchemaID;
			
				item.Number = varNumber;
			
				item.Name = varName;
			
				item.SchemaColumnTypeID = varSchemaColumnTypeID;
			
				item.Width = varWidth;
			
				item.Format = varFormat;
			
				item.PhenomenonID = varPhenomenonID;
			
				item.PhenomenonOfferingID = varPhenomenonOfferingID;
			
				item.PhenomenonUOMID = varPhenomenonUOMID;
			
				item.FixedTime = varFixedTime;
			
				item.EmptyValue = varEmptyValue;
			
				item.UserId = varUserId;
			
				item.AddedAt = varAddedAt;
			
				item.UpdatedAt = varUpdatedAt;
			
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
        
        
        
        public static TableSchema.TableColumn DataSchemaIDColumn
        {
            get { return Schema.Columns[1]; }
        }
        
        
        
        public static TableSchema.TableColumn NumberColumn
        {
            get { return Schema.Columns[2]; }
        }
        
        
        
        public static TableSchema.TableColumn NameColumn
        {
            get { return Schema.Columns[3]; }
        }
        
        
        
        public static TableSchema.TableColumn SchemaColumnTypeIDColumn
        {
            get { return Schema.Columns[4]; }
        }
        
        
        
        public static TableSchema.TableColumn WidthColumn
        {
            get { return Schema.Columns[5]; }
        }
        
        
        
        public static TableSchema.TableColumn FormatColumn
        {
            get { return Schema.Columns[6]; }
        }
        
        
        
        public static TableSchema.TableColumn PhenomenonIDColumn
        {
            get { return Schema.Columns[7]; }
        }
        
        
        
        public static TableSchema.TableColumn PhenomenonOfferingIDColumn
        {
            get { return Schema.Columns[8]; }
        }
        
        
        
        public static TableSchema.TableColumn PhenomenonUOMIDColumn
        {
            get { return Schema.Columns[9]; }
        }
        
        
        
        public static TableSchema.TableColumn FixedTimeColumn
        {
            get { return Schema.Columns[10]; }
        }
        
        
        
        public static TableSchema.TableColumn EmptyValueColumn
        {
            get { return Schema.Columns[11]; }
        }
        
        
        
        public static TableSchema.TableColumn UserIdColumn
        {
            get { return Schema.Columns[12]; }
        }
        
        
        
        public static TableSchema.TableColumn AddedAtColumn
        {
            get { return Schema.Columns[13]; }
        }
        
        
        
        public static TableSchema.TableColumn UpdatedAtColumn
        {
            get { return Schema.Columns[14]; }
        }
        
        
        
        #endregion
		#region Columns Struct
		public struct Columns
		{
			 public static string Id = @"ID";
			 public static string DataSchemaID = @"DataSchemaID";
			 public static string Number = @"Number";
			 public static string Name = @"Name";
			 public static string SchemaColumnTypeID = @"SchemaColumnTypeID";
			 public static string Width = @"Width";
			 public static string Format = @"Format";
			 public static string PhenomenonID = @"PhenomenonID";
			 public static string PhenomenonOfferingID = @"PhenomenonOfferingID";
			 public static string PhenomenonUOMID = @"PhenomenonUOMID";
			 public static string FixedTime = @"FixedTime";
			 public static string EmptyValue = @"EmptyValue";
			 public static string UserId = @"UserId";
			 public static string AddedAt = @"AddedAt";
			 public static string UpdatedAt = @"UpdatedAt";
						
		}
		#endregion
		
		#region Update PK Collections
		
        #endregion
    
        #region Deep Save
		
        #endregion
	}
}