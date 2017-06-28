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
namespace SAEON.Observations.Data{
    /// <summary>
    /// Strongly-typed collection for the VInstrumentDataSource class.
    /// </summary>
    [Serializable]
    public partial class VInstrumentDataSourceCollection : ReadOnlyList<VInstrumentDataSource, VInstrumentDataSourceCollection>
    {        
        public VInstrumentDataSourceCollection() {}
    }
    /// <summary>
    /// This is  Read-only wrapper class for the vInstrumentDataSource view.
    /// </summary>
    [Serializable]
    public partial class VInstrumentDataSource : ReadOnlyRecord<VInstrumentDataSource>, IReadOnlyRecord
    {
    
	    #region Default Settings
	    protected static void SetSQLProps() 
	    {
		    GetTableSchema();
	    }
	    #endregion
        #region Schema Accessor
	    public static TableSchema.Table Schema
        {
            get
            {
                if (BaseSchema == null)
                {
                    SetSQLProps();
                }
                return BaseSchema;
            }
        }
    	
        private static void GetTableSchema() 
        {
            if(!IsSchemaInitialized)
            {
                //Schema declaration
                TableSchema.Table schema = new TableSchema.Table("vInstrumentDataSource", TableType.View, DataService.GetInstance("ObservationsDB"));
                schema.Columns = new TableSchema.TableColumnCollection();
                schema.SchemaName = @"dbo";
                //columns
                
                TableSchema.TableColumn colvarId = new TableSchema.TableColumn(schema);
                colvarId.ColumnName = "ID";
                colvarId.DataType = DbType.Guid;
                colvarId.MaxLength = 0;
                colvarId.AutoIncrement = false;
                colvarId.IsNullable = false;
                colvarId.IsPrimaryKey = false;
                colvarId.IsForeignKey = false;
                colvarId.IsReadOnly = false;
                
                schema.Columns.Add(colvarId);
                
                TableSchema.TableColumn colvarInstrumentID = new TableSchema.TableColumn(schema);
                colvarInstrumentID.ColumnName = "InstrumentID";
                colvarInstrumentID.DataType = DbType.Guid;
                colvarInstrumentID.MaxLength = 0;
                colvarInstrumentID.AutoIncrement = false;
                colvarInstrumentID.IsNullable = false;
                colvarInstrumentID.IsPrimaryKey = false;
                colvarInstrumentID.IsForeignKey = false;
                colvarInstrumentID.IsReadOnly = false;
                
                schema.Columns.Add(colvarInstrumentID);
                
                TableSchema.TableColumn colvarDataSourceID = new TableSchema.TableColumn(schema);
                colvarDataSourceID.ColumnName = "DataSourceID";
                colvarDataSourceID.DataType = DbType.Guid;
                colvarDataSourceID.MaxLength = 0;
                colvarDataSourceID.AutoIncrement = false;
                colvarDataSourceID.IsNullable = false;
                colvarDataSourceID.IsPrimaryKey = false;
                colvarDataSourceID.IsForeignKey = false;
                colvarDataSourceID.IsReadOnly = false;
                
                schema.Columns.Add(colvarDataSourceID);
                
                TableSchema.TableColumn colvarStartDate = new TableSchema.TableColumn(schema);
                colvarStartDate.ColumnName = "StartDate";
                colvarStartDate.DataType = DbType.Date;
                colvarStartDate.MaxLength = 0;
                colvarStartDate.AutoIncrement = false;
                colvarStartDate.IsNullable = true;
                colvarStartDate.IsPrimaryKey = false;
                colvarStartDate.IsForeignKey = false;
                colvarStartDate.IsReadOnly = false;
                
                schema.Columns.Add(colvarStartDate);
                
                TableSchema.TableColumn colvarEndDate = new TableSchema.TableColumn(schema);
                colvarEndDate.ColumnName = "EndDate";
                colvarEndDate.DataType = DbType.Date;
                colvarEndDate.MaxLength = 0;
                colvarEndDate.AutoIncrement = false;
                colvarEndDate.IsNullable = true;
                colvarEndDate.IsPrimaryKey = false;
                colvarEndDate.IsForeignKey = false;
                colvarEndDate.IsReadOnly = false;
                
                schema.Columns.Add(colvarEndDate);
                
                TableSchema.TableColumn colvarUserId = new TableSchema.TableColumn(schema);
                colvarUserId.ColumnName = "UserId";
                colvarUserId.DataType = DbType.Guid;
                colvarUserId.MaxLength = 0;
                colvarUserId.AutoIncrement = false;
                colvarUserId.IsNullable = false;
                colvarUserId.IsPrimaryKey = false;
                colvarUserId.IsForeignKey = false;
                colvarUserId.IsReadOnly = false;
                
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
                
                schema.Columns.Add(colvarRowVersion);
                
                TableSchema.TableColumn colvarInstrumentCode = new TableSchema.TableColumn(schema);
                colvarInstrumentCode.ColumnName = "InstrumentCode";
                colvarInstrumentCode.DataType = DbType.AnsiString;
                colvarInstrumentCode.MaxLength = 50;
                colvarInstrumentCode.AutoIncrement = false;
                colvarInstrumentCode.IsNullable = false;
                colvarInstrumentCode.IsPrimaryKey = false;
                colvarInstrumentCode.IsForeignKey = false;
                colvarInstrumentCode.IsReadOnly = false;
                
                schema.Columns.Add(colvarInstrumentCode);
                
                TableSchema.TableColumn colvarInstrumentName = new TableSchema.TableColumn(schema);
                colvarInstrumentName.ColumnName = "InstrumentName";
                colvarInstrumentName.DataType = DbType.AnsiString;
                colvarInstrumentName.MaxLength = 150;
                colvarInstrumentName.AutoIncrement = false;
                colvarInstrumentName.IsNullable = false;
                colvarInstrumentName.IsPrimaryKey = false;
                colvarInstrumentName.IsForeignKey = false;
                colvarInstrumentName.IsReadOnly = false;
                
                schema.Columns.Add(colvarInstrumentName);
                
                TableSchema.TableColumn colvarDataSourceCode = new TableSchema.TableColumn(schema);
                colvarDataSourceCode.ColumnName = "DataSourceCode";
                colvarDataSourceCode.DataType = DbType.AnsiString;
                colvarDataSourceCode.MaxLength = 50;
                colvarDataSourceCode.AutoIncrement = false;
                colvarDataSourceCode.IsNullable = false;
                colvarDataSourceCode.IsPrimaryKey = false;
                colvarDataSourceCode.IsForeignKey = false;
                colvarDataSourceCode.IsReadOnly = false;
                
                schema.Columns.Add(colvarDataSourceCode);
                
                TableSchema.TableColumn colvarDataSourceName = new TableSchema.TableColumn(schema);
                colvarDataSourceName.ColumnName = "DataSourceName";
                colvarDataSourceName.DataType = DbType.AnsiString;
                colvarDataSourceName.MaxLength = 150;
                colvarDataSourceName.AutoIncrement = false;
                colvarDataSourceName.IsNullable = false;
                colvarDataSourceName.IsPrimaryKey = false;
                colvarDataSourceName.IsForeignKey = false;
                colvarDataSourceName.IsReadOnly = false;
                
                schema.Columns.Add(colvarDataSourceName);
                
                
                BaseSchema = schema;
                //add this schema to the provider
                //so we can query it later
                DataService.Providers["ObservationsDB"].AddSchema("vInstrumentDataSource",schema);
            }
        }
        #endregion
        
        #region Query Accessor
	    public static Query CreateQuery()
	    {
		    return new Query(Schema);
	    }
	    #endregion
	    
	    #region .ctors
	    public VInstrumentDataSource()
	    {
            SetSQLProps();
            SetDefaults();
            MarkNew();
        }
        public VInstrumentDataSource(bool useDatabaseDefaults)
	    {
		    SetSQLProps();
		    if(useDatabaseDefaults)
		    {
				ForceDefaults();
			}
			MarkNew();
	    }
	    
	    public VInstrumentDataSource(object keyID)
	    {
		    SetSQLProps();
		    LoadByKey(keyID);
	    }
    	 
	    public VInstrumentDataSource(string columnName, object columnValue)
        {
            SetSQLProps();
            LoadByParam(columnName,columnValue);
        }
        
	    #endregion
	    
	    #region Props
	    
          
        [XmlAttribute("Id")]
        [Bindable(true)]
        public Guid Id 
	    {
		    get
		    {
			    return GetColumnValue<Guid>("ID");
		    }
            set 
		    {
			    SetColumnValue("ID", value);
            }
        }
	      
        [XmlAttribute("InstrumentID")]
        [Bindable(true)]
        public Guid InstrumentID 
	    {
		    get
		    {
			    return GetColumnValue<Guid>("InstrumentID");
		    }
            set 
		    {
			    SetColumnValue("InstrumentID", value);
            }
        }
	      
        [XmlAttribute("DataSourceID")]
        [Bindable(true)]
        public Guid DataSourceID 
	    {
		    get
		    {
			    return GetColumnValue<Guid>("DataSourceID");
		    }
            set 
		    {
			    SetColumnValue("DataSourceID", value);
            }
        }
	      
        [XmlAttribute("StartDate")]
        [Bindable(true)]
        public DateTime? StartDate 
	    {
		    get
		    {
			    return GetColumnValue<DateTime?>("StartDate");
		    }
            set 
		    {
			    SetColumnValue("StartDate", value);
            }
        }
	      
        [XmlAttribute("EndDate")]
        [Bindable(true)]
        public DateTime? EndDate 
	    {
		    get
		    {
			    return GetColumnValue<DateTime?>("EndDate");
		    }
            set 
		    {
			    SetColumnValue("EndDate", value);
            }
        }
	      
        [XmlAttribute("UserId")]
        [Bindable(true)]
        public Guid UserId 
	    {
		    get
		    {
			    return GetColumnValue<Guid>("UserId");
		    }
            set 
		    {
			    SetColumnValue("UserId", value);
            }
        }
	      
        [XmlAttribute("AddedAt")]
        [Bindable(true)]
        public DateTime? AddedAt 
	    {
		    get
		    {
			    return GetColumnValue<DateTime?>("AddedAt");
		    }
            set 
		    {
			    SetColumnValue("AddedAt", value);
            }
        }
	      
        [XmlAttribute("UpdatedAt")]
        [Bindable(true)]
        public DateTime? UpdatedAt 
	    {
		    get
		    {
			    return GetColumnValue<DateTime?>("UpdatedAt");
		    }
            set 
		    {
			    SetColumnValue("UpdatedAt", value);
            }
        }
	      
        [XmlAttribute("RowVersion")]
        [Bindable(true)]
        public byte[] RowVersion 
	    {
		    get
		    {
			    return GetColumnValue<byte[]>("RowVersion");
		    }
            set 
		    {
			    SetColumnValue("RowVersion", value);
            }
        }
	      
        [XmlAttribute("InstrumentCode")]
        [Bindable(true)]
        public string InstrumentCode 
	    {
		    get
		    {
			    return GetColumnValue<string>("InstrumentCode");
		    }
            set 
		    {
			    SetColumnValue("InstrumentCode", value);
            }
        }
	      
        [XmlAttribute("InstrumentName")]
        [Bindable(true)]
        public string InstrumentName 
	    {
		    get
		    {
			    return GetColumnValue<string>("InstrumentName");
		    }
            set 
		    {
			    SetColumnValue("InstrumentName", value);
            }
        }
	      
        [XmlAttribute("DataSourceCode")]
        [Bindable(true)]
        public string DataSourceCode 
	    {
		    get
		    {
			    return GetColumnValue<string>("DataSourceCode");
		    }
            set 
		    {
			    SetColumnValue("DataSourceCode", value);
            }
        }
	      
        [XmlAttribute("DataSourceName")]
        [Bindable(true)]
        public string DataSourceName 
	    {
		    get
		    {
			    return GetColumnValue<string>("DataSourceName");
		    }
            set 
		    {
			    SetColumnValue("DataSourceName", value);
            }
        }
	    
	    #endregion
    
	    #region Columns Struct
	    public struct Columns
	    {
		    
		    
            public static string Id = @"ID";
            
            public static string InstrumentID = @"InstrumentID";
            
            public static string DataSourceID = @"DataSourceID";
            
            public static string StartDate = @"StartDate";
            
            public static string EndDate = @"EndDate";
            
            public static string UserId = @"UserId";
            
            public static string AddedAt = @"AddedAt";
            
            public static string UpdatedAt = @"UpdatedAt";
            
            public static string RowVersion = @"RowVersion";
            
            public static string InstrumentCode = @"InstrumentCode";
            
            public static string InstrumentName = @"InstrumentName";
            
            public static string DataSourceCode = @"DataSourceCode";
            
            public static string DataSourceName = @"DataSourceName";
            
	    }
	    #endregion
	    
	    
	    #region IAbstractRecord Members
        public new CT GetColumnValue<CT>(string columnName) {
            return base.GetColumnValue<CT>(columnName);
        }
        public object GetColumnValue(string columnName) {
            return base.GetColumnValue<object>(columnName);
        }
        #endregion
	    
    }
}
