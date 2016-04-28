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
namespace SAEON.ObservationsDB.Data{
    /// <summary>
    /// Strongly-typed collection for the VDataSource class.
    /// </summary>
    [Serializable]
    public partial class VDataSourceCollection : ReadOnlyList<VDataSource, VDataSourceCollection>
    {        
        public VDataSourceCollection() {}
    }
    /// <summary>
    /// This is  Read-only wrapper class for the vDataSource view.
    /// </summary>
    [Serializable]
    public partial class VDataSource : ReadOnlyRecord<VDataSource>, IReadOnlyRecord
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
                TableSchema.Table schema = new TableSchema.Table("vDataSource", TableType.View, DataService.GetInstance("ObservationsDB"));
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
                
                TableSchema.TableColumn colvarCode = new TableSchema.TableColumn(schema);
                colvarCode.ColumnName = "Code";
                colvarCode.DataType = DbType.AnsiString;
                colvarCode.MaxLength = 50;
                colvarCode.AutoIncrement = false;
                colvarCode.IsNullable = false;
                colvarCode.IsPrimaryKey = false;
                colvarCode.IsForeignKey = false;
                colvarCode.IsReadOnly = false;
                
                schema.Columns.Add(colvarCode);
                
                TableSchema.TableColumn colvarName = new TableSchema.TableColumn(schema);
                colvarName.ColumnName = "Name";
                colvarName.DataType = DbType.AnsiString;
                colvarName.MaxLength = 150;
                colvarName.AutoIncrement = false;
                colvarName.IsNullable = false;
                colvarName.IsPrimaryKey = false;
                colvarName.IsForeignKey = false;
                colvarName.IsReadOnly = false;
                
                schema.Columns.Add(colvarName);
                
                TableSchema.TableColumn colvarDescription = new TableSchema.TableColumn(schema);
                colvarDescription.ColumnName = "Description";
                colvarDescription.DataType = DbType.AnsiString;
                colvarDescription.MaxLength = 5000;
                colvarDescription.AutoIncrement = false;
                colvarDescription.IsNullable = true;
                colvarDescription.IsPrimaryKey = false;
                colvarDescription.IsForeignKey = false;
                colvarDescription.IsReadOnly = false;
                
                schema.Columns.Add(colvarDescription);
                
                TableSchema.TableColumn colvarUrl = new TableSchema.TableColumn(schema);
                colvarUrl.ColumnName = "Url";
                colvarUrl.DataType = DbType.AnsiString;
                colvarUrl.MaxLength = 250;
                colvarUrl.AutoIncrement = false;
                colvarUrl.IsNullable = false;
                colvarUrl.IsPrimaryKey = false;
                colvarUrl.IsForeignKey = false;
                colvarUrl.IsReadOnly = false;
                
                schema.Columns.Add(colvarUrl);
                
                TableSchema.TableColumn colvarDefaultNullValue = new TableSchema.TableColumn(schema);
                colvarDefaultNullValue.ColumnName = "DefaultNullValue";
                colvarDefaultNullValue.DataType = DbType.Double;
                colvarDefaultNullValue.MaxLength = 0;
                colvarDefaultNullValue.AutoIncrement = false;
                colvarDefaultNullValue.IsNullable = true;
                colvarDefaultNullValue.IsPrimaryKey = false;
                colvarDefaultNullValue.IsForeignKey = false;
                colvarDefaultNullValue.IsReadOnly = false;
                
                schema.Columns.Add(colvarDefaultNullValue);
                
                TableSchema.TableColumn colvarErrorEstimate = new TableSchema.TableColumn(schema);
                colvarErrorEstimate.ColumnName = "ErrorEstimate";
                colvarErrorEstimate.DataType = DbType.Double;
                colvarErrorEstimate.MaxLength = 0;
                colvarErrorEstimate.AutoIncrement = false;
                colvarErrorEstimate.IsNullable = true;
                colvarErrorEstimate.IsPrimaryKey = false;
                colvarErrorEstimate.IsForeignKey = false;
                colvarErrorEstimate.IsReadOnly = false;
                
                schema.Columns.Add(colvarErrorEstimate);
                
                TableSchema.TableColumn colvarUpdateFreq = new TableSchema.TableColumn(schema);
                colvarUpdateFreq.ColumnName = "UpdateFreq";
                colvarUpdateFreq.DataType = DbType.Int32;
                colvarUpdateFreq.MaxLength = 0;
                colvarUpdateFreq.AutoIncrement = false;
                colvarUpdateFreq.IsNullable = false;
                colvarUpdateFreq.IsPrimaryKey = false;
                colvarUpdateFreq.IsForeignKey = false;
                colvarUpdateFreq.IsReadOnly = false;
                
                schema.Columns.Add(colvarUpdateFreq);
                
                TableSchema.TableColumn colvarStartDate = new TableSchema.TableColumn(schema);
                colvarStartDate.ColumnName = "StartDate";
                colvarStartDate.DataType = DbType.DateTime;
                colvarStartDate.MaxLength = 0;
                colvarStartDate.AutoIncrement = false;
                colvarStartDate.IsNullable = true;
                colvarStartDate.IsPrimaryKey = false;
                colvarStartDate.IsForeignKey = false;
                colvarStartDate.IsReadOnly = false;
                
                schema.Columns.Add(colvarStartDate);
                
                TableSchema.TableColumn colvarEndDate = new TableSchema.TableColumn(schema);
                colvarEndDate.ColumnName = "EndDate";
                colvarEndDate.DataType = DbType.DateTime;
                colvarEndDate.MaxLength = 0;
                colvarEndDate.AutoIncrement = false;
                colvarEndDate.IsNullable = true;
                colvarEndDate.IsPrimaryKey = false;
                colvarEndDate.IsForeignKey = false;
                colvarEndDate.IsReadOnly = false;
                
                schema.Columns.Add(colvarEndDate);
                
                TableSchema.TableColumn colvarLastUpdate = new TableSchema.TableColumn(schema);
                colvarLastUpdate.ColumnName = "LastUpdate";
                colvarLastUpdate.DataType = DbType.DateTime;
                colvarLastUpdate.MaxLength = 0;
                colvarLastUpdate.AutoIncrement = false;
                colvarLastUpdate.IsNullable = false;
                colvarLastUpdate.IsPrimaryKey = false;
                colvarLastUpdate.IsForeignKey = false;
                colvarLastUpdate.IsReadOnly = false;
                
                schema.Columns.Add(colvarLastUpdate);
                
                TableSchema.TableColumn colvarDataSchemaID = new TableSchema.TableColumn(schema);
                colvarDataSchemaID.ColumnName = "DataSchemaID";
                colvarDataSchemaID.DataType = DbType.Guid;
                colvarDataSchemaID.MaxLength = 0;
                colvarDataSchemaID.AutoIncrement = false;
                colvarDataSchemaID.IsNullable = true;
                colvarDataSchemaID.IsPrimaryKey = false;
                colvarDataSchemaID.IsForeignKey = false;
                colvarDataSchemaID.IsReadOnly = false;
                
                schema.Columns.Add(colvarDataSchemaID);
                
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
                
                TableSchema.TableColumn colvarStationID = new TableSchema.TableColumn(schema);
                colvarStationID.ColumnName = "StationID";
                colvarStationID.DataType = DbType.Guid;
                colvarStationID.MaxLength = 0;
                colvarStationID.AutoIncrement = false;
                colvarStationID.IsNullable = true;
                colvarStationID.IsPrimaryKey = false;
                colvarStationID.IsForeignKey = false;
                colvarStationID.IsReadOnly = false;
                
                schema.Columns.Add(colvarStationID);
                
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
                
                TableSchema.TableColumn colvarDataSchemaName = new TableSchema.TableColumn(schema);
                colvarDataSchemaName.ColumnName = "DataSchemaName";
                colvarDataSchemaName.DataType = DbType.AnsiString;
                colvarDataSchemaName.MaxLength = 100;
                colvarDataSchemaName.AutoIncrement = false;
                colvarDataSchemaName.IsNullable = true;
                colvarDataSchemaName.IsPrimaryKey = false;
                colvarDataSchemaName.IsForeignKey = false;
                colvarDataSchemaName.IsReadOnly = false;
                
                schema.Columns.Add(colvarDataSchemaName);
                
                TableSchema.TableColumn colvarStationName = new TableSchema.TableColumn(schema);
                colvarStationName.ColumnName = "StationName";
                colvarStationName.DataType = DbType.AnsiString;
                colvarStationName.MaxLength = 203;
                colvarStationName.AutoIncrement = false;
                colvarStationName.IsNullable = false;
                colvarStationName.IsPrimaryKey = false;
                colvarStationName.IsForeignKey = false;
                colvarStationName.IsReadOnly = false;
                
                schema.Columns.Add(colvarStationName);
                
                
                BaseSchema = schema;
                //add this schema to the provider
                //so we can query it later
                DataService.Providers["ObservationsDB"].AddSchema("vDataSource",schema);
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
	    public VDataSource()
	    {
            SetSQLProps();
            SetDefaults();
            MarkNew();
        }
        public VDataSource(bool useDatabaseDefaults)
	    {
		    SetSQLProps();
		    if(useDatabaseDefaults)
		    {
				ForceDefaults();
			}
			MarkNew();
	    }
	    
	    public VDataSource(object keyID)
	    {
		    SetSQLProps();
		    LoadByKey(keyID);
	    }
    	 
	    public VDataSource(string columnName, object columnValue)
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
	      
        [XmlAttribute("Code")]
        [Bindable(true)]
        public string Code 
	    {
		    get
		    {
			    return GetColumnValue<string>("Code");
		    }
            set 
		    {
			    SetColumnValue("Code", value);
            }
        }
	      
        [XmlAttribute("Name")]
        [Bindable(true)]
        public string Name 
	    {
		    get
		    {
			    return GetColumnValue<string>("Name");
		    }
            set 
		    {
			    SetColumnValue("Name", value);
            }
        }
	      
        [XmlAttribute("Description")]
        [Bindable(true)]
        public string Description 
	    {
		    get
		    {
			    return GetColumnValue<string>("Description");
		    }
            set 
		    {
			    SetColumnValue("Description", value);
            }
        }
	      
        [XmlAttribute("Url")]
        [Bindable(true)]
        public string Url 
	    {
		    get
		    {
			    return GetColumnValue<string>("Url");
		    }
            set 
		    {
			    SetColumnValue("Url", value);
            }
        }
	      
        [XmlAttribute("DefaultNullValue")]
        [Bindable(true)]
        public double? DefaultNullValue 
	    {
		    get
		    {
			    return GetColumnValue<double?>("DefaultNullValue");
		    }
            set 
		    {
			    SetColumnValue("DefaultNullValue", value);
            }
        }
	      
        [XmlAttribute("ErrorEstimate")]
        [Bindable(true)]
        public double? ErrorEstimate 
	    {
		    get
		    {
			    return GetColumnValue<double?>("ErrorEstimate");
		    }
            set 
		    {
			    SetColumnValue("ErrorEstimate", value);
            }
        }
	      
        [XmlAttribute("UpdateFreq")]
        [Bindable(true)]
        public int UpdateFreq 
	    {
		    get
		    {
			    return GetColumnValue<int>("UpdateFreq");
		    }
            set 
		    {
			    SetColumnValue("UpdateFreq", value);
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
	      
        [XmlAttribute("LastUpdate")]
        [Bindable(true)]
        public DateTime LastUpdate 
	    {
		    get
		    {
			    return GetColumnValue<DateTime>("LastUpdate");
		    }
            set 
		    {
			    SetColumnValue("LastUpdate", value);
            }
        }
	      
        [XmlAttribute("DataSchemaID")]
        [Bindable(true)]
        public Guid? DataSchemaID 
	    {
		    get
		    {
			    return GetColumnValue<Guid?>("DataSchemaID");
		    }
            set 
		    {
			    SetColumnValue("DataSchemaID", value);
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
	      
        [XmlAttribute("StationID")]
        [Bindable(true)]
        public Guid? StationID 
	    {
		    get
		    {
			    return GetColumnValue<Guid?>("StationID");
		    }
            set 
		    {
			    SetColumnValue("StationID", value);
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
	      
        [XmlAttribute("DataSchemaName")]
        [Bindable(true)]
        public string DataSchemaName 
	    {
		    get
		    {
			    return GetColumnValue<string>("DataSchemaName");
		    }
            set 
		    {
			    SetColumnValue("DataSchemaName", value);
            }
        }
	      
        [XmlAttribute("StationName")]
        [Bindable(true)]
        public string StationName 
	    {
		    get
		    {
			    return GetColumnValue<string>("StationName");
		    }
            set 
		    {
			    SetColumnValue("StationName", value);
            }
        }
	    
	    #endregion
    
	    #region Columns Struct
	    public struct Columns
	    {
		    
		    
            public static string Id = @"ID";
            
            public static string Code = @"Code";
            
            public static string Name = @"Name";
            
            public static string Description = @"Description";
            
            public static string Url = @"Url";
            
            public static string DefaultNullValue = @"DefaultNullValue";
            
            public static string ErrorEstimate = @"ErrorEstimate";
            
            public static string UpdateFreq = @"UpdateFreq";
            
            public static string StartDate = @"StartDate";
            
            public static string EndDate = @"EndDate";
            
            public static string LastUpdate = @"LastUpdate";
            
            public static string DataSchemaID = @"DataSchemaID";
            
            public static string UserId = @"UserId";
            
            public static string StationID = @"StationID";
            
            public static string UpdatedAt = @"UpdatedAt";
            
            public static string DataSchemaName = @"DataSchemaName";
            
            public static string StationName = @"StationName";
            
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
