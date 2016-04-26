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
    /// Strongly-typed collection for the VDataSourceOrganisation class.
    /// </summary>
    [Serializable]
    public partial class VDataSourceOrganisationCollection : ReadOnlyList<VDataSourceOrganisation, VDataSourceOrganisationCollection>
    {        
        public VDataSourceOrganisationCollection() {}
    }
    /// <summary>
    /// This is  Read-only wrapper class for the vDataSourceOrganisation view.
    /// </summary>
    [Serializable]
    public partial class VDataSourceOrganisation : ReadOnlyRecord<VDataSourceOrganisation>, IReadOnlyRecord
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
                TableSchema.Table schema = new TableSchema.Table("vDataSourceOrganisation", TableType.View, DataService.GetInstance("ObservationsDB"));
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
                
                TableSchema.TableColumn colvarOrganisationID = new TableSchema.TableColumn(schema);
                colvarOrganisationID.ColumnName = "OrganisationID";
                colvarOrganisationID.DataType = DbType.Guid;
                colvarOrganisationID.MaxLength = 0;
                colvarOrganisationID.AutoIncrement = false;
                colvarOrganisationID.IsNullable = false;
                colvarOrganisationID.IsPrimaryKey = false;
                colvarOrganisationID.IsForeignKey = false;
                colvarOrganisationID.IsReadOnly = false;
                
                schema.Columns.Add(colvarOrganisationID);
                
                TableSchema.TableColumn colvarOrganisationRoleID = new TableSchema.TableColumn(schema);
                colvarOrganisationRoleID.ColumnName = "OrganisationRoleID";
                colvarOrganisationRoleID.DataType = DbType.Guid;
                colvarOrganisationRoleID.MaxLength = 0;
                colvarOrganisationRoleID.AutoIncrement = false;
                colvarOrganisationRoleID.IsNullable = false;
                colvarOrganisationRoleID.IsPrimaryKey = false;
                colvarOrganisationRoleID.IsForeignKey = false;
                colvarOrganisationRoleID.IsReadOnly = false;
                
                schema.Columns.Add(colvarOrganisationRoleID);
                
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
                
                TableSchema.TableColumn colvarOrganisationName = new TableSchema.TableColumn(schema);
                colvarOrganisationName.ColumnName = "OrganisationName";
                colvarOrganisationName.DataType = DbType.AnsiString;
                colvarOrganisationName.MaxLength = 150;
                colvarOrganisationName.AutoIncrement = false;
                colvarOrganisationName.IsNullable = false;
                colvarOrganisationName.IsPrimaryKey = false;
                colvarOrganisationName.IsForeignKey = false;
                colvarOrganisationName.IsReadOnly = false;
                
                schema.Columns.Add(colvarOrganisationName);
                
                TableSchema.TableColumn colvarOrganisationRoleName = new TableSchema.TableColumn(schema);
                colvarOrganisationRoleName.ColumnName = "OrganisationRoleName";
                colvarOrganisationRoleName.DataType = DbType.AnsiString;
                colvarOrganisationRoleName.MaxLength = 150;
                colvarOrganisationRoleName.AutoIncrement = false;
                colvarOrganisationRoleName.IsNullable = false;
                colvarOrganisationRoleName.IsPrimaryKey = false;
                colvarOrganisationRoleName.IsForeignKey = false;
                colvarOrganisationRoleName.IsReadOnly = false;
                
                schema.Columns.Add(colvarOrganisationRoleName);
                
                
                BaseSchema = schema;
                //add this schema to the provider
                //so we can query it later
                DataService.Providers["ObservationsDB"].AddSchema("vDataSourceOrganisation",schema);
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
	    public VDataSourceOrganisation()
	    {
            SetSQLProps();
            SetDefaults();
            MarkNew();
        }
        public VDataSourceOrganisation(bool useDatabaseDefaults)
	    {
		    SetSQLProps();
		    if(useDatabaseDefaults)
		    {
				ForceDefaults();
			}
			MarkNew();
	    }
	    
	    public VDataSourceOrganisation(object keyID)
	    {
		    SetSQLProps();
		    LoadByKey(keyID);
	    }
    	 
	    public VDataSourceOrganisation(string columnName, object columnValue)
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
	      
        [XmlAttribute("OrganisationID")]
        [Bindable(true)]
        public Guid OrganisationID 
	    {
		    get
		    {
			    return GetColumnValue<Guid>("OrganisationID");
		    }
            set 
		    {
			    SetColumnValue("OrganisationID", value);
            }
        }
	      
        [XmlAttribute("OrganisationRoleID")]
        [Bindable(true)]
        public Guid OrganisationRoleID 
	    {
		    get
		    {
			    return GetColumnValue<Guid>("OrganisationRoleID");
		    }
            set 
		    {
			    SetColumnValue("OrganisationRoleID", value);
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
	      
        [XmlAttribute("OrganisationName")]
        [Bindable(true)]
        public string OrganisationName 
	    {
		    get
		    {
			    return GetColumnValue<string>("OrganisationName");
		    }
            set 
		    {
			    SetColumnValue("OrganisationName", value);
            }
        }
	      
        [XmlAttribute("OrganisationRoleName")]
        [Bindable(true)]
        public string OrganisationRoleName 
	    {
		    get
		    {
			    return GetColumnValue<string>("OrganisationRoleName");
		    }
            set 
		    {
			    SetColumnValue("OrganisationRoleName", value);
            }
        }
	    
	    #endregion
    
	    #region Columns Struct
	    public struct Columns
	    {
		    
		    
            public static string Id = @"ID";
            
            public static string DataSourceID = @"DataSourceID";
            
            public static string OrganisationID = @"OrganisationID";
            
            public static string OrganisationRoleID = @"OrganisationRoleID";
            
            public static string StartDate = @"StartDate";
            
            public static string EndDate = @"EndDate";
            
            public static string UserId = @"UserId";
            
            public static string UpdatedAt = @"UpdatedAt";
            
            public static string OrganisationName = @"OrganisationName";
            
            public static string OrganisationRoleName = @"OrganisationRoleName";
            
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
