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
namespace Observations.Data
{
    /// <summary>
    /// Strongly-typed collection for the SensorProcedure class.
    /// </summary>
    [Serializable]
    public partial class SensorProcedureCollection : ActiveList<SensorProcedure, SensorProcedureCollection>
    {	   
        public SensorProcedureCollection() {}
        
        /// <summary>
        /// Filters an existing collection based on the set criteria. This is an in-memory filter
        /// Thanks to developingchris for this!
        /// </summary>
        /// <returns>SensorProcedureCollection</returns>
        public SensorProcedureCollection Filter()
        {
            for (int i = this.Count - 1; i > -1; i--)
            {
                SensorProcedure o = this[i];
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
    /// This is an ActiveRecord class which wraps the SensorProcedure table.
    /// </summary>
    [Serializable]
    public partial class SensorProcedure : ActiveRecord<SensorProcedure>, IActiveRecord
    {
        #region .ctors and Default Settings
        
        public SensorProcedure()
        {
          SetSQLProps();
          InitSetDefaults();
          MarkNew();
        }
        
        private void InitSetDefaults() { SetDefaults(); }
        
        public SensorProcedure(bool useDatabaseDefaults)
        {
            SetSQLProps();
            if(useDatabaseDefaults)
                ForceDefaults();
            MarkNew();
        }
        
        public SensorProcedure(object keyID)
        {
            SetSQLProps();
            InitSetDefaults();
            LoadByKey(keyID);
        }
         
        public SensorProcedure(string columnName, object columnValue)
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
                TableSchema.Table schema = new TableSchema.Table("SensorProcedure", TableType.Table, DataService.GetInstance("SqlDataProvider"));
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
                
                TableSchema.TableColumn colvarName = new TableSchema.TableColumn(schema);
                colvarName.ColumnName = "Name";
                colvarName.DataType = DbType.AnsiString;
                colvarName.MaxLength = 150;
                colvarName.AutoIncrement = false;
                colvarName.IsNullable = false;
                colvarName.IsPrimaryKey = false;
                colvarName.IsForeignKey = false;
                colvarName.IsReadOnly = false;
                colvarName.DefaultSetting = @"";
                colvarName.ForeignKeyTableName = "";
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
                colvarDescription.DefaultSetting = @"";
                colvarDescription.ForeignKeyTableName = "";
                schema.Columns.Add(colvarDescription);
                
                TableSchema.TableColumn colvarUrl = new TableSchema.TableColumn(schema);
                colvarUrl.ColumnName = "Url";
                colvarUrl.DataType = DbType.AnsiString;
                colvarUrl.MaxLength = 250;
                colvarUrl.AutoIncrement = false;
                colvarUrl.IsNullable = true;
                colvarUrl.IsPrimaryKey = false;
                colvarUrl.IsForeignKey = false;
                colvarUrl.IsReadOnly = false;
                colvarUrl.DefaultSetting = @"";
                colvarUrl.ForeignKeyTableName = "";
                schema.Columns.Add(colvarUrl);
                
                TableSchema.TableColumn colvarStationID = new TableSchema.TableColumn(schema);
                colvarStationID.ColumnName = "StationID";
                colvarStationID.DataType = DbType.Guid;
                colvarStationID.MaxLength = 0;
                colvarStationID.AutoIncrement = false;
                colvarStationID.IsNullable = false;
                colvarStationID.IsPrimaryKey = false;
                colvarStationID.IsForeignKey = true;
                colvarStationID.IsReadOnly = false;
                colvarStationID.DefaultSetting = @"";
                
                    colvarStationID.ForeignKeyTableName = "Station";
                schema.Columns.Add(colvarStationID);
                
                TableSchema.TableColumn colvarPhenomenonID = new TableSchema.TableColumn(schema);
                colvarPhenomenonID.ColumnName = "PhenomenonID";
                colvarPhenomenonID.DataType = DbType.Guid;
                colvarPhenomenonID.MaxLength = 0;
                colvarPhenomenonID.AutoIncrement = false;
                colvarPhenomenonID.IsNullable = false;
                colvarPhenomenonID.IsPrimaryKey = false;
                colvarPhenomenonID.IsForeignKey = true;
                colvarPhenomenonID.IsReadOnly = false;
                colvarPhenomenonID.DefaultSetting = @"";
                
                    colvarPhenomenonID.ForeignKeyTableName = "Phenomenon";
                schema.Columns.Add(colvarPhenomenonID);
                
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
                
                TableSchema.TableColumn colvarDataSchemaID = new TableSchema.TableColumn(schema);
                colvarDataSchemaID.ColumnName = "DataSchemaID";
                colvarDataSchemaID.DataType = DbType.Guid;
                colvarDataSchemaID.MaxLength = 0;
                colvarDataSchemaID.AutoIncrement = false;
                colvarDataSchemaID.IsNullable = true;
                colvarDataSchemaID.IsPrimaryKey = false;
                colvarDataSchemaID.IsForeignKey = true;
                colvarDataSchemaID.IsReadOnly = false;
                colvarDataSchemaID.DefaultSetting = @"";
                
                    colvarDataSchemaID.ForeignKeyTableName = "DataSchema";
                schema.Columns.Add(colvarDataSchemaID);
                
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
                
                BaseSchema = schema;
                //add this schema to the provider
                //so we can query it later
                DataService.Providers["SqlDataProvider"].AddSchema("SensorProcedure",schema);
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
          
        [XmlAttribute("Name")]
        [Bindable(true)]
        public string Name 
        {
            get { return GetColumnValue<string>(Columns.Name); }
            set { SetColumnValue(Columns.Name, value); }
        }
          
        [XmlAttribute("Description")]
        [Bindable(true)]
        public string Description 
        {
            get { return GetColumnValue<string>(Columns.Description); }
            set { SetColumnValue(Columns.Description, value); }
        }
          
        [XmlAttribute("Url")]
        [Bindable(true)]
        public string Url 
        {
            get { return GetColumnValue<string>(Columns.Url); }
            set { SetColumnValue(Columns.Url, value); }
        }
          
        [XmlAttribute("StationID")]
        [Bindable(true)]
        public Guid StationID 
        {
            get { return GetColumnValue<Guid>(Columns.StationID); }
            set { SetColumnValue(Columns.StationID, value); }
        }
          
        [XmlAttribute("PhenomenonID")]
        [Bindable(true)]
        public Guid PhenomenonID 
        {
            get { return GetColumnValue<Guid>(Columns.PhenomenonID); }
            set { SetColumnValue(Columns.PhenomenonID, value); }
        }
          
        [XmlAttribute("DataSourceID")]
        [Bindable(true)]
        public Guid DataSourceID 
        {
            get { return GetColumnValue<Guid>(Columns.DataSourceID); }
            set { SetColumnValue(Columns.DataSourceID, value); }
        }
          
        [XmlAttribute("DataSchemaID")]
        [Bindable(true)]
        public Guid? DataSchemaID 
        {
            get { return GetColumnValue<Guid?>(Columns.DataSchemaID); }
            set { SetColumnValue(Columns.DataSchemaID, value); }
        }
          
        [XmlAttribute("UserId")]
        [Bindable(true)]
        public Guid UserId 
        {
            get { return GetColumnValue<Guid>(Columns.UserId); }
            set { SetColumnValue(Columns.UserId, value); }
        }
        
        #endregion
        
        
        #region PrimaryKey Methods		
        
        protected override void SetPrimaryKey(object oValue)
        {
            base.SetPrimaryKey(oValue);
            
            SetPKValues();
        }
        
        
        public Observations.Data.ObservationCollection ObservationRecords()
        {
            return new Observations.Data.ObservationCollection().Where(Observation.Columns.SensorProcedureID, Id).Load();
        }
        public Observations.Data.DataLogCollection DataLogRecords()
        {
            return new Observations.Data.DataLogCollection().Where(DataLog.Columns.SensorProcedureID, Id).Load();
        }
        #endregion
        
            
        
        #region ForeignKey Properties
        
        /// <summary>
        /// Returns a AspnetUser ActiveRecord object related to this SensorProcedure
        /// 
        /// </summary>
        public Observations.Data.AspnetUser AspnetUser
        {
            get { return Observations.Data.AspnetUser.FetchByID(this.UserId); }
            set { SetColumnValue("UserId", value.UserId); }
        }
        
        
        /// <summary>
        /// Returns a DataSchema ActiveRecord object related to this SensorProcedure
        /// 
        /// </summary>
        public Observations.Data.DataSchema DataSchema
        {
            get { return Observations.Data.DataSchema.FetchByID(this.DataSchemaID); }
            set { SetColumnValue("DataSchemaID", value.Id); }
        }
        
        
        /// <summary>
        /// Returns a DataSource ActiveRecord object related to this SensorProcedure
        /// 
        /// </summary>
        public Observations.Data.DataSource DataSource
        {
            get { return Observations.Data.DataSource.FetchByID(this.DataSourceID); }
            set { SetColumnValue("DataSourceID", value.Id); }
        }
        
        
        /// <summary>
        /// Returns a Phenomenon ActiveRecord object related to this SensorProcedure
        /// 
        /// </summary>
        public Observations.Data.Phenomenon Phenomenon
        {
            get { return Observations.Data.Phenomenon.FetchByID(this.PhenomenonID); }
            set { SetColumnValue("PhenomenonID", value.Id); }
        }
        
        
        /// <summary>
        /// Returns a Station ActiveRecord object related to this SensorProcedure
        /// 
        /// </summary>
        public Observations.Data.Station Station
        {
            get { return Observations.Data.Station.FetchByID(this.StationID); }
            set { SetColumnValue("StationID", value.Id); }
        }
        
        
        #endregion
        
        
        
        //no ManyToMany tables defined (0)
        
        
        
        #region ObjectDataSource support
        
        
        /// <summary>
        /// Inserts a record, can be used with the Object Data Source
        /// </summary>
        public static void Insert(Guid varId,string varCode,string varName,string varDescription,string varUrl,Guid varStationID,Guid varPhenomenonID,Guid varDataSourceID,Guid? varDataSchemaID,Guid varUserId)
        {
            SensorProcedure item = new SensorProcedure();
            
            item.Id = varId;
            
            item.Code = varCode;
            
            item.Name = varName;
            
            item.Description = varDescription;
            
            item.Url = varUrl;
            
            item.StationID = varStationID;
            
            item.PhenomenonID = varPhenomenonID;
            
            item.DataSourceID = varDataSourceID;
            
            item.DataSchemaID = varDataSchemaID;
            
            item.UserId = varUserId;
            
        
            if (System.Web.HttpContext.Current != null)
                item.Save(System.Web.HttpContext.Current.User.Identity.Name);
            else
                item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
        }
        
        /// <summary>
        /// Updates a record, can be used with the Object Data Source
        /// </summary>
        public static void Update(Guid varId,string varCode,string varName,string varDescription,string varUrl,Guid varStationID,Guid varPhenomenonID,Guid varDataSourceID,Guid? varDataSchemaID,Guid varUserId)
        {
            SensorProcedure item = new SensorProcedure();
            
                item.Id = varId;
            
                item.Code = varCode;
            
                item.Name = varName;
            
                item.Description = varDescription;
            
                item.Url = varUrl;
            
                item.StationID = varStationID;
            
                item.PhenomenonID = varPhenomenonID;
            
                item.DataSourceID = varDataSourceID;
            
                item.DataSchemaID = varDataSchemaID;
            
                item.UserId = varUserId;
            
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
        
        
        
        public static TableSchema.TableColumn NameColumn
        {
            get { return Schema.Columns[2]; }
        }
        
        
        
        public static TableSchema.TableColumn DescriptionColumn
        {
            get { return Schema.Columns[3]; }
        }
        
        
        
        public static TableSchema.TableColumn UrlColumn
        {
            get { return Schema.Columns[4]; }
        }
        
        
        
        public static TableSchema.TableColumn StationIDColumn
        {
            get { return Schema.Columns[5]; }
        }
        
        
        
        public static TableSchema.TableColumn PhenomenonIDColumn
        {
            get { return Schema.Columns[6]; }
        }
        
        
        
        public static TableSchema.TableColumn DataSourceIDColumn
        {
            get { return Schema.Columns[7]; }
        }
        
        
        
        public static TableSchema.TableColumn DataSchemaIDColumn
        {
            get { return Schema.Columns[8]; }
        }
        
        
        
        public static TableSchema.TableColumn UserIdColumn
        {
            get { return Schema.Columns[9]; }
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
             public static string StationID = @"StationID";
             public static string PhenomenonID = @"PhenomenonID";
             public static string DataSourceID = @"DataSourceID";
             public static string DataSchemaID = @"DataSchemaID";
             public static string UserId = @"UserId";
                        
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
