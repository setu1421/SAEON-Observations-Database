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
    /// Strongly-typed collection for the VObservation class.
    /// </summary>
    [Serializable]
    public partial class VObservationCollection : ReadOnlyList<VObservation, VObservationCollection>
    {        
        public VObservationCollection() {}
    }
    /// <summary>
    /// This is  Read-only wrapper class for the vObservation view.
    /// </summary>
    [Serializable]
    public partial class VObservation : ReadOnlyRecord<VObservation>, IReadOnlyRecord
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
                TableSchema.Table schema = new TableSchema.Table("vObservation", TableType.View, DataService.GetInstance("ObservationsDB"));
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
                
                TableSchema.TableColumn colvarSensorID = new TableSchema.TableColumn(schema);
                colvarSensorID.ColumnName = "SensorID";
                colvarSensorID.DataType = DbType.Guid;
                colvarSensorID.MaxLength = 0;
                colvarSensorID.AutoIncrement = false;
                colvarSensorID.IsNullable = false;
                colvarSensorID.IsPrimaryKey = false;
                colvarSensorID.IsForeignKey = false;
                colvarSensorID.IsReadOnly = false;
                
                schema.Columns.Add(colvarSensorID);
                
                TableSchema.TableColumn colvarPhenomenonOfferingID = new TableSchema.TableColumn(schema);
                colvarPhenomenonOfferingID.ColumnName = "PhenomenonOfferingID";
                colvarPhenomenonOfferingID.DataType = DbType.Guid;
                colvarPhenomenonOfferingID.MaxLength = 0;
                colvarPhenomenonOfferingID.AutoIncrement = false;
                colvarPhenomenonOfferingID.IsNullable = false;
                colvarPhenomenonOfferingID.IsPrimaryKey = false;
                colvarPhenomenonOfferingID.IsForeignKey = false;
                colvarPhenomenonOfferingID.IsReadOnly = false;
                
                schema.Columns.Add(colvarPhenomenonOfferingID);
                
                TableSchema.TableColumn colvarPhenomenonUOMID = new TableSchema.TableColumn(schema);
                colvarPhenomenonUOMID.ColumnName = "PhenomenonUOMID";
                colvarPhenomenonUOMID.DataType = DbType.Guid;
                colvarPhenomenonUOMID.MaxLength = 0;
                colvarPhenomenonUOMID.AutoIncrement = false;
                colvarPhenomenonUOMID.IsNullable = false;
                colvarPhenomenonUOMID.IsPrimaryKey = false;
                colvarPhenomenonUOMID.IsForeignKey = false;
                colvarPhenomenonUOMID.IsReadOnly = false;
                
                schema.Columns.Add(colvarPhenomenonUOMID);
                
                TableSchema.TableColumn colvarRawValue = new TableSchema.TableColumn(schema);
                colvarRawValue.ColumnName = "RawValue";
                colvarRawValue.DataType = DbType.Double;
                colvarRawValue.MaxLength = 0;
                colvarRawValue.AutoIncrement = false;
                colvarRawValue.IsNullable = true;
                colvarRawValue.IsPrimaryKey = false;
                colvarRawValue.IsForeignKey = false;
                colvarRawValue.IsReadOnly = false;
                
                schema.Columns.Add(colvarRawValue);
                
                TableSchema.TableColumn colvarDataValue = new TableSchema.TableColumn(schema);
                colvarDataValue.ColumnName = "DataValue";
                colvarDataValue.DataType = DbType.Double;
                colvarDataValue.MaxLength = 0;
                colvarDataValue.AutoIncrement = false;
                colvarDataValue.IsNullable = true;
                colvarDataValue.IsPrimaryKey = false;
                colvarDataValue.IsForeignKey = false;
                colvarDataValue.IsReadOnly = false;
                
                schema.Columns.Add(colvarDataValue);
                
                TableSchema.TableColumn colvarImportBatchID = new TableSchema.TableColumn(schema);
                colvarImportBatchID.ColumnName = "ImportBatchID";
                colvarImportBatchID.DataType = DbType.Guid;
                colvarImportBatchID.MaxLength = 0;
                colvarImportBatchID.AutoIncrement = false;
                colvarImportBatchID.IsNullable = false;
                colvarImportBatchID.IsPrimaryKey = false;
                colvarImportBatchID.IsForeignKey = false;
                colvarImportBatchID.IsReadOnly = false;
                
                schema.Columns.Add(colvarImportBatchID);
                
                TableSchema.TableColumn colvarValueDate = new TableSchema.TableColumn(schema);
                colvarValueDate.ColumnName = "ValueDate";
                colvarValueDate.DataType = DbType.DateTime;
                colvarValueDate.MaxLength = 0;
                colvarValueDate.AutoIncrement = false;
                colvarValueDate.IsNullable = false;
                colvarValueDate.IsPrimaryKey = false;
                colvarValueDate.IsForeignKey = false;
                colvarValueDate.IsReadOnly = false;
                
                schema.Columns.Add(colvarValueDate);
                
                TableSchema.TableColumn colvarComment = new TableSchema.TableColumn(schema);
                colvarComment.ColumnName = "Comment";
                colvarComment.DataType = DbType.AnsiString;
                colvarComment.MaxLength = 250;
                colvarComment.AutoIncrement = false;
                colvarComment.IsNullable = true;
                colvarComment.IsPrimaryKey = false;
                colvarComment.IsForeignKey = false;
                colvarComment.IsReadOnly = false;
                
                schema.Columns.Add(colvarComment);
                
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
                
                TableSchema.TableColumn colvarUserName = new TableSchema.TableColumn(schema);
                colvarUserName.ColumnName = "UserName";
                colvarUserName.DataType = DbType.String;
                colvarUserName.MaxLength = 256;
                colvarUserName.AutoIncrement = false;
                colvarUserName.IsNullable = false;
                colvarUserName.IsPrimaryKey = false;
                colvarUserName.IsForeignKey = false;
                colvarUserName.IsReadOnly = false;
                
                schema.Columns.Add(colvarUserName);
                
                TableSchema.TableColumn colvarStatusName = new TableSchema.TableColumn(schema);
                colvarStatusName.ColumnName = "StatusName";
                colvarStatusName.DataType = DbType.AnsiString;
                colvarStatusName.MaxLength = 150;
                colvarStatusName.AutoIncrement = false;
                colvarStatusName.IsNullable = true;
                colvarStatusName.IsPrimaryKey = false;
                colvarStatusName.IsForeignKey = false;
                colvarStatusName.IsReadOnly = false;
                
                schema.Columns.Add(colvarStatusName);
                
                TableSchema.TableColumn colvarStatusReasonName = new TableSchema.TableColumn(schema);
                colvarStatusReasonName.ColumnName = "StatusReasonName";
                colvarStatusReasonName.DataType = DbType.AnsiString;
                colvarStatusReasonName.MaxLength = 150;
                colvarStatusReasonName.AutoIncrement = false;
                colvarStatusReasonName.IsNullable = true;
                colvarStatusReasonName.IsPrimaryKey = false;
                colvarStatusReasonName.IsForeignKey = false;
                colvarStatusReasonName.IsReadOnly = false;
                
                schema.Columns.Add(colvarStatusReasonName);
                
                TableSchema.TableColumn colvarOfferingID = new TableSchema.TableColumn(schema);
                colvarOfferingID.ColumnName = "OfferingID";
                colvarOfferingID.DataType = DbType.Guid;
                colvarOfferingID.MaxLength = 0;
                colvarOfferingID.AutoIncrement = false;
                colvarOfferingID.IsNullable = false;
                colvarOfferingID.IsPrimaryKey = false;
                colvarOfferingID.IsForeignKey = false;
                colvarOfferingID.IsReadOnly = false;
                
                schema.Columns.Add(colvarOfferingID);
                
                TableSchema.TableColumn colvarOfferingName = new TableSchema.TableColumn(schema);
                colvarOfferingName.ColumnName = "OfferingName";
                colvarOfferingName.DataType = DbType.AnsiString;
                colvarOfferingName.MaxLength = 150;
                colvarOfferingName.AutoIncrement = false;
                colvarOfferingName.IsNullable = false;
                colvarOfferingName.IsPrimaryKey = false;
                colvarOfferingName.IsForeignKey = false;
                colvarOfferingName.IsReadOnly = false;
                
                schema.Columns.Add(colvarOfferingName);
                
                TableSchema.TableColumn colvarUnitOfMeasureID = new TableSchema.TableColumn(schema);
                colvarUnitOfMeasureID.ColumnName = "UnitOfMeasureID";
                colvarUnitOfMeasureID.DataType = DbType.Guid;
                colvarUnitOfMeasureID.MaxLength = 0;
                colvarUnitOfMeasureID.AutoIncrement = false;
                colvarUnitOfMeasureID.IsNullable = false;
                colvarUnitOfMeasureID.IsPrimaryKey = false;
                colvarUnitOfMeasureID.IsForeignKey = false;
                colvarUnitOfMeasureID.IsReadOnly = false;
                
                schema.Columns.Add(colvarUnitOfMeasureID);
                
                TableSchema.TableColumn colvarUnitOfMeasureUnit = new TableSchema.TableColumn(schema);
                colvarUnitOfMeasureUnit.ColumnName = "UnitOfMeasureUnit";
                colvarUnitOfMeasureUnit.DataType = DbType.AnsiString;
                colvarUnitOfMeasureUnit.MaxLength = 100;
                colvarUnitOfMeasureUnit.AutoIncrement = false;
                colvarUnitOfMeasureUnit.IsNullable = false;
                colvarUnitOfMeasureUnit.IsPrimaryKey = false;
                colvarUnitOfMeasureUnit.IsForeignKey = false;
                colvarUnitOfMeasureUnit.IsReadOnly = false;
                
                schema.Columns.Add(colvarUnitOfMeasureUnit);
                
                TableSchema.TableColumn colvarUnitOfMeasureSymbol = new TableSchema.TableColumn(schema);
                colvarUnitOfMeasureSymbol.ColumnName = "UnitOfMeasureSymbol";
                colvarUnitOfMeasureSymbol.DataType = DbType.AnsiString;
                colvarUnitOfMeasureSymbol.MaxLength = 20;
                colvarUnitOfMeasureSymbol.AutoIncrement = false;
                colvarUnitOfMeasureSymbol.IsNullable = false;
                colvarUnitOfMeasureSymbol.IsPrimaryKey = false;
                colvarUnitOfMeasureSymbol.IsForeignKey = false;
                colvarUnitOfMeasureSymbol.IsReadOnly = false;
                
                schema.Columns.Add(colvarUnitOfMeasureSymbol);
                
                TableSchema.TableColumn colvarSensorName = new TableSchema.TableColumn(schema);
                colvarSensorName.ColumnName = "SensorName";
                colvarSensorName.DataType = DbType.AnsiString;
                colvarSensorName.MaxLength = 150;
                colvarSensorName.AutoIncrement = false;
                colvarSensorName.IsNullable = false;
                colvarSensorName.IsPrimaryKey = false;
                colvarSensorName.IsForeignKey = false;
                colvarSensorName.IsReadOnly = false;
                
                schema.Columns.Add(colvarSensorName);
                
                TableSchema.TableColumn colvarPhenomenonID = new TableSchema.TableColumn(schema);
                colvarPhenomenonID.ColumnName = "PhenomenonID";
                colvarPhenomenonID.DataType = DbType.Guid;
                colvarPhenomenonID.MaxLength = 0;
                colvarPhenomenonID.AutoIncrement = false;
                colvarPhenomenonID.IsNullable = false;
                colvarPhenomenonID.IsPrimaryKey = false;
                colvarPhenomenonID.IsForeignKey = false;
                colvarPhenomenonID.IsReadOnly = false;
                
                schema.Columns.Add(colvarPhenomenonID);
                
                TableSchema.TableColumn colvarPhenomenonName = new TableSchema.TableColumn(schema);
                colvarPhenomenonName.ColumnName = "PhenomenonName";
                colvarPhenomenonName.DataType = DbType.AnsiString;
                colvarPhenomenonName.MaxLength = 150;
                colvarPhenomenonName.AutoIncrement = false;
                colvarPhenomenonName.IsNullable = false;
                colvarPhenomenonName.IsPrimaryKey = false;
                colvarPhenomenonName.IsForeignKey = false;
                colvarPhenomenonName.IsReadOnly = false;
                
                schema.Columns.Add(colvarPhenomenonName);
                
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
                
                TableSchema.TableColumn colvarStationID = new TableSchema.TableColumn(schema);
                colvarStationID.ColumnName = "StationID";
                colvarStationID.DataType = DbType.Guid;
                colvarStationID.MaxLength = 0;
                colvarStationID.AutoIncrement = false;
                colvarStationID.IsNullable = false;
                colvarStationID.IsPrimaryKey = false;
                colvarStationID.IsForeignKey = false;
                colvarStationID.IsReadOnly = false;
                
                schema.Columns.Add(colvarStationID);
                
                TableSchema.TableColumn colvarStationName = new TableSchema.TableColumn(schema);
                colvarStationName.ColumnName = "StationName";
                colvarStationName.DataType = DbType.AnsiString;
                colvarStationName.MaxLength = 150;
                colvarStationName.AutoIncrement = false;
                colvarStationName.IsNullable = false;
                colvarStationName.IsPrimaryKey = false;
                colvarStationName.IsForeignKey = false;
                colvarStationName.IsReadOnly = false;
                
                schema.Columns.Add(colvarStationName);
                
                TableSchema.TableColumn colvarSiteID = new TableSchema.TableColumn(schema);
                colvarSiteID.ColumnName = "SiteID";
                colvarSiteID.DataType = DbType.Guid;
                colvarSiteID.MaxLength = 0;
                colvarSiteID.AutoIncrement = false;
                colvarSiteID.IsNullable = false;
                colvarSiteID.IsPrimaryKey = false;
                colvarSiteID.IsForeignKey = false;
                colvarSiteID.IsReadOnly = false;
                
                schema.Columns.Add(colvarSiteID);
                
                TableSchema.TableColumn colvarSiteName = new TableSchema.TableColumn(schema);
                colvarSiteName.ColumnName = "SiteName";
                colvarSiteName.DataType = DbType.AnsiString;
                colvarSiteName.MaxLength = 150;
                colvarSiteName.AutoIncrement = false;
                colvarSiteName.IsNullable = false;
                colvarSiteName.IsPrimaryKey = false;
                colvarSiteName.IsForeignKey = false;
                colvarSiteName.IsReadOnly = false;
                
                schema.Columns.Add(colvarSiteName);
                
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
                
                
                BaseSchema = schema;
                //add this schema to the provider
                //so we can query it later
                DataService.Providers["ObservationsDB"].AddSchema("vObservation",schema);
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
	    public VObservation()
	    {
            SetSQLProps();
            SetDefaults();
            MarkNew();
        }
        public VObservation(bool useDatabaseDefaults)
	    {
		    SetSQLProps();
		    if(useDatabaseDefaults)
		    {
				ForceDefaults();
			}
			MarkNew();
	    }
	    
	    public VObservation(object keyID)
	    {
		    SetSQLProps();
		    LoadByKey(keyID);
	    }
    	 
	    public VObservation(string columnName, object columnValue)
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
	      
        [XmlAttribute("SensorID")]
        [Bindable(true)]
        public Guid SensorID 
	    {
		    get
		    {
			    return GetColumnValue<Guid>("SensorID");
		    }
            set 
		    {
			    SetColumnValue("SensorID", value);
            }
        }
	      
        [XmlAttribute("PhenomenonOfferingID")]
        [Bindable(true)]
        public Guid PhenomenonOfferingID 
	    {
		    get
		    {
			    return GetColumnValue<Guid>("PhenomenonOfferingID");
		    }
            set 
		    {
			    SetColumnValue("PhenomenonOfferingID", value);
            }
        }
	      
        [XmlAttribute("PhenomenonUOMID")]
        [Bindable(true)]
        public Guid PhenomenonUOMID 
	    {
		    get
		    {
			    return GetColumnValue<Guid>("PhenomenonUOMID");
		    }
            set 
		    {
			    SetColumnValue("PhenomenonUOMID", value);
            }
        }
	      
        [XmlAttribute("RawValue")]
        [Bindable(true)]
        public double? RawValue 
	    {
		    get
		    {
			    return GetColumnValue<double?>("RawValue");
		    }
            set 
		    {
			    SetColumnValue("RawValue", value);
            }
        }
	      
        [XmlAttribute("DataValue")]
        [Bindable(true)]
        public double? DataValue 
	    {
		    get
		    {
			    return GetColumnValue<double?>("DataValue");
		    }
            set 
		    {
			    SetColumnValue("DataValue", value);
            }
        }
	      
        [XmlAttribute("ImportBatchID")]
        [Bindable(true)]
        public Guid ImportBatchID 
	    {
		    get
		    {
			    return GetColumnValue<Guid>("ImportBatchID");
		    }
            set 
		    {
			    SetColumnValue("ImportBatchID", value);
            }
        }
	      
        [XmlAttribute("ValueDate")]
        [Bindable(true)]
        public DateTime ValueDate 
	    {
		    get
		    {
			    return GetColumnValue<DateTime>("ValueDate");
		    }
            set 
		    {
			    SetColumnValue("ValueDate", value);
            }
        }
	      
        [XmlAttribute("Comment")]
        [Bindable(true)]
        public string Comment 
	    {
		    get
		    {
			    return GetColumnValue<string>("Comment");
		    }
            set 
		    {
			    SetColumnValue("Comment", value);
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
	      
        [XmlAttribute("UserName")]
        [Bindable(true)]
        public string UserName 
	    {
		    get
		    {
			    return GetColumnValue<string>("UserName");
		    }
            set 
		    {
			    SetColumnValue("UserName", value);
            }
        }
	      
        [XmlAttribute("StatusName")]
        [Bindable(true)]
        public string StatusName 
	    {
		    get
		    {
			    return GetColumnValue<string>("StatusName");
		    }
            set 
		    {
			    SetColumnValue("StatusName", value);
            }
        }
	      
        [XmlAttribute("StatusReasonName")]
        [Bindable(true)]
        public string StatusReasonName 
	    {
		    get
		    {
			    return GetColumnValue<string>("StatusReasonName");
		    }
            set 
		    {
			    SetColumnValue("StatusReasonName", value);
            }
        }
	      
        [XmlAttribute("OfferingID")]
        [Bindable(true)]
        public Guid OfferingID 
	    {
		    get
		    {
			    return GetColumnValue<Guid>("OfferingID");
		    }
            set 
		    {
			    SetColumnValue("OfferingID", value);
            }
        }
	      
        [XmlAttribute("OfferingName")]
        [Bindable(true)]
        public string OfferingName 
	    {
		    get
		    {
			    return GetColumnValue<string>("OfferingName");
		    }
            set 
		    {
			    SetColumnValue("OfferingName", value);
            }
        }
	      
        [XmlAttribute("UnitOfMeasureID")]
        [Bindable(true)]
        public Guid UnitOfMeasureID 
	    {
		    get
		    {
			    return GetColumnValue<Guid>("UnitOfMeasureID");
		    }
            set 
		    {
			    SetColumnValue("UnitOfMeasureID", value);
            }
        }
	      
        [XmlAttribute("UnitOfMeasureUnit")]
        [Bindable(true)]
        public string UnitOfMeasureUnit 
	    {
		    get
		    {
			    return GetColumnValue<string>("UnitOfMeasureUnit");
		    }
            set 
		    {
			    SetColumnValue("UnitOfMeasureUnit", value);
            }
        }
	      
        [XmlAttribute("UnitOfMeasureSymbol")]
        [Bindable(true)]
        public string UnitOfMeasureSymbol 
	    {
		    get
		    {
			    return GetColumnValue<string>("UnitOfMeasureSymbol");
		    }
            set 
		    {
			    SetColumnValue("UnitOfMeasureSymbol", value);
            }
        }
	      
        [XmlAttribute("SensorName")]
        [Bindable(true)]
        public string SensorName 
	    {
		    get
		    {
			    return GetColumnValue<string>("SensorName");
		    }
            set 
		    {
			    SetColumnValue("SensorName", value);
            }
        }
	      
        [XmlAttribute("PhenomenonID")]
        [Bindable(true)]
        public Guid PhenomenonID 
	    {
		    get
		    {
			    return GetColumnValue<Guid>("PhenomenonID");
		    }
            set 
		    {
			    SetColumnValue("PhenomenonID", value);
            }
        }
	      
        [XmlAttribute("PhenomenonName")]
        [Bindable(true)]
        public string PhenomenonName 
	    {
		    get
		    {
			    return GetColumnValue<string>("PhenomenonName");
		    }
            set 
		    {
			    SetColumnValue("PhenomenonName", value);
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
	      
        [XmlAttribute("StationID")]
        [Bindable(true)]
        public Guid StationID 
	    {
		    get
		    {
			    return GetColumnValue<Guid>("StationID");
		    }
            set 
		    {
			    SetColumnValue("StationID", value);
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
	      
        [XmlAttribute("SiteID")]
        [Bindable(true)]
        public Guid SiteID 
	    {
		    get
		    {
			    return GetColumnValue<Guid>("SiteID");
		    }
            set 
		    {
			    SetColumnValue("SiteID", value);
            }
        }
	      
        [XmlAttribute("SiteName")]
        [Bindable(true)]
        public string SiteName 
	    {
		    get
		    {
			    return GetColumnValue<string>("SiteName");
		    }
            set 
		    {
			    SetColumnValue("SiteName", value);
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
	    
	    #endregion
    
	    #region Columns Struct
	    public struct Columns
	    {
		    
		    
            public static string Id = @"ID";
            
            public static string SensorID = @"SensorID";
            
            public static string PhenomenonOfferingID = @"PhenomenonOfferingID";
            
            public static string PhenomenonUOMID = @"PhenomenonUOMID";
            
            public static string RawValue = @"RawValue";
            
            public static string DataValue = @"DataValue";
            
            public static string ImportBatchID = @"ImportBatchID";
            
            public static string ValueDate = @"ValueDate";
            
            public static string Comment = @"Comment";
            
            public static string UserId = @"UserId";
            
            public static string UserName = @"UserName";
            
            public static string StatusName = @"StatusName";
            
            public static string StatusReasonName = @"StatusReasonName";
            
            public static string OfferingID = @"OfferingID";
            
            public static string OfferingName = @"OfferingName";
            
            public static string UnitOfMeasureID = @"UnitOfMeasureID";
            
            public static string UnitOfMeasureUnit = @"UnitOfMeasureUnit";
            
            public static string UnitOfMeasureSymbol = @"UnitOfMeasureSymbol";
            
            public static string SensorName = @"SensorName";
            
            public static string PhenomenonID = @"PhenomenonID";
            
            public static string PhenomenonName = @"PhenomenonName";
            
            public static string DataSourceID = @"DataSourceID";
            
            public static string DataSourceName = @"DataSourceName";
            
            public static string DataSchemaName = @"DataSchemaName";
            
            public static string InstrumentID = @"InstrumentID";
            
            public static string InstrumentName = @"InstrumentName";
            
            public static string StationID = @"StationID";
            
            public static string StationName = @"StationName";
            
            public static string SiteID = @"SiteID";
            
            public static string SiteName = @"SiteName";
            
            public static string OrganisationID = @"OrganisationID";
            
            public static string OrganisationName = @"OrganisationName";
            
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
