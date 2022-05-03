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
	/// Strongly-typed collection for the Observation class.
	/// </summary>
    [Serializable]
	public partial class ObservationCollection : ActiveList<Observation, ObservationCollection>
	{	   
		public ObservationCollection() {}
        
        /// <summary>
		/// Filters an existing collection based on the set criteria. This is an in-memory filter
		/// Thanks to developingchris for this!
        /// </summary>
        /// <returns>ObservationCollection</returns>
		public ObservationCollection Filter()
        {
            for (int i = this.Count - 1; i > -1; i--)
            {
                Observation o = this[i];
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
	/// This is an ActiveRecord class which wraps the Observation table.
	/// </summary>
	[Serializable]
	public partial class Observation : ActiveRecord<Observation>, IActiveRecord
	{
		#region .ctors and Default Settings
		
		public Observation()
		{
		  SetSQLProps();
		  InitSetDefaults();
		  MarkNew();
		}
		
		private void InitSetDefaults() { SetDefaults(); }
		
		public Observation(bool useDatabaseDefaults)
		{
			SetSQLProps();
			if(useDatabaseDefaults)
				ForceDefaults();
			MarkNew();
		}
        
		public Observation(object keyID)
		{
			SetSQLProps();
			InitSetDefaults();
			LoadByKey(keyID);
		}
		 
		public Observation(string columnName, object columnValue)
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
				TableSchema.Table schema = new TableSchema.Table("Observation", TableType.Table, DataService.GetInstance("ObservationsDB"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				//columns
				
				TableSchema.TableColumn colvarSensorID = new TableSchema.TableColumn(schema);
				colvarSensorID.ColumnName = "SensorID";
				colvarSensorID.DataType = DbType.Guid;
				colvarSensorID.MaxLength = 0;
				colvarSensorID.AutoIncrement = false;
				colvarSensorID.IsNullable = false;
				colvarSensorID.IsPrimaryKey = false;
				colvarSensorID.IsForeignKey = true;
				colvarSensorID.IsReadOnly = false;
				colvarSensorID.DefaultSetting = @"";
				
					colvarSensorID.ForeignKeyTableName = "Sensor";
				schema.Columns.Add(colvarSensorID);
				
				TableSchema.TableColumn colvarValueDate = new TableSchema.TableColumn(schema);
				colvarValueDate.ColumnName = "ValueDate";
				colvarValueDate.DataType = DbType.DateTime;
				colvarValueDate.MaxLength = 0;
				colvarValueDate.AutoIncrement = false;
				colvarValueDate.IsNullable = false;
				colvarValueDate.IsPrimaryKey = false;
				colvarValueDate.IsForeignKey = false;
				colvarValueDate.IsReadOnly = false;
				colvarValueDate.DefaultSetting = @"";
				colvarValueDate.ForeignKeyTableName = "";
				schema.Columns.Add(colvarValueDate);
				
				TableSchema.TableColumn colvarRawValue = new TableSchema.TableColumn(schema);
				colvarRawValue.ColumnName = "RawValue";
				colvarRawValue.DataType = DbType.Double;
				colvarRawValue.MaxLength = 0;
				colvarRawValue.AutoIncrement = false;
				colvarRawValue.IsNullable = true;
				colvarRawValue.IsPrimaryKey = false;
				colvarRawValue.IsForeignKey = false;
				colvarRawValue.IsReadOnly = false;
				colvarRawValue.DefaultSetting = @"";
				colvarRawValue.ForeignKeyTableName = "";
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
				colvarDataValue.DefaultSetting = @"";
				colvarDataValue.ForeignKeyTableName = "";
				schema.Columns.Add(colvarDataValue);
				
				TableSchema.TableColumn colvarComment = new TableSchema.TableColumn(schema);
				colvarComment.ColumnName = "Comment";
				colvarComment.DataType = DbType.AnsiString;
				colvarComment.MaxLength = 250;
				colvarComment.AutoIncrement = false;
				colvarComment.IsNullable = true;
				colvarComment.IsPrimaryKey = false;
				colvarComment.IsForeignKey = false;
				colvarComment.IsReadOnly = false;
				colvarComment.DefaultSetting = @"";
				colvarComment.ForeignKeyTableName = "";
				schema.Columns.Add(colvarComment);
				
				TableSchema.TableColumn colvarPhenomenonOfferingID = new TableSchema.TableColumn(schema);
				colvarPhenomenonOfferingID.ColumnName = "PhenomenonOfferingID";
				colvarPhenomenonOfferingID.DataType = DbType.Guid;
				colvarPhenomenonOfferingID.MaxLength = 0;
				colvarPhenomenonOfferingID.AutoIncrement = false;
				colvarPhenomenonOfferingID.IsNullable = false;
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
				colvarPhenomenonUOMID.IsNullable = false;
				colvarPhenomenonUOMID.IsPrimaryKey = false;
				colvarPhenomenonUOMID.IsForeignKey = true;
				colvarPhenomenonUOMID.IsReadOnly = false;
				colvarPhenomenonUOMID.DefaultSetting = @"";
				
					colvarPhenomenonUOMID.ForeignKeyTableName = "PhenomenonUOM";
				schema.Columns.Add(colvarPhenomenonUOMID);
				
				TableSchema.TableColumn colvarImportBatchID = new TableSchema.TableColumn(schema);
				colvarImportBatchID.ColumnName = "ImportBatchID";
				colvarImportBatchID.DataType = DbType.Guid;
				colvarImportBatchID.MaxLength = 0;
				colvarImportBatchID.AutoIncrement = false;
				colvarImportBatchID.IsNullable = false;
				colvarImportBatchID.IsPrimaryKey = false;
				colvarImportBatchID.IsForeignKey = true;
				colvarImportBatchID.IsReadOnly = false;
				colvarImportBatchID.DefaultSetting = @"";
				
					colvarImportBatchID.ForeignKeyTableName = "ImportBatch";
				schema.Columns.Add(colvarImportBatchID);
				
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
				
				TableSchema.TableColumn colvarCorrelationID = new TableSchema.TableColumn(schema);
				colvarCorrelationID.ColumnName = "CorrelationID";
				colvarCorrelationID.DataType = DbType.Guid;
				colvarCorrelationID.MaxLength = 0;
				colvarCorrelationID.AutoIncrement = false;
				colvarCorrelationID.IsNullable = true;
				colvarCorrelationID.IsPrimaryKey = false;
				colvarCorrelationID.IsForeignKey = false;
				colvarCorrelationID.IsReadOnly = false;
				colvarCorrelationID.DefaultSetting = @"";
				colvarCorrelationID.ForeignKeyTableName = "";
				schema.Columns.Add(colvarCorrelationID);
				
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
				
				TableSchema.TableColumn colvarAddedDate = new TableSchema.TableColumn(schema);
				colvarAddedDate.ColumnName = "AddedDate";
				colvarAddedDate.DataType = DbType.DateTime;
				colvarAddedDate.MaxLength = 0;
				colvarAddedDate.AutoIncrement = false;
				colvarAddedDate.IsNullable = false;
				colvarAddedDate.IsPrimaryKey = false;
				colvarAddedDate.IsForeignKey = false;
				colvarAddedDate.IsReadOnly = false;
				
						colvarAddedDate.DefaultSetting = @"(getdate())";
				colvarAddedDate.ForeignKeyTableName = "";
				schema.Columns.Add(colvarAddedDate);
				
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
				
				TableSchema.TableColumn colvarId = new TableSchema.TableColumn(schema);
				colvarId.ColumnName = "ID";
				colvarId.DataType = DbType.Int32;
				colvarId.MaxLength = 0;
				colvarId.AutoIncrement = true;
				colvarId.IsNullable = false;
				colvarId.IsPrimaryKey = true;
				colvarId.IsForeignKey = false;
				colvarId.IsReadOnly = false;
				colvarId.DefaultSetting = @"";
				colvarId.ForeignKeyTableName = "";
				schema.Columns.Add(colvarId);
				
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
				
				TableSchema.TableColumn colvarTextValue = new TableSchema.TableColumn(schema);
				colvarTextValue.ColumnName = "TextValue";
				colvarTextValue.DataType = DbType.AnsiString;
				colvarTextValue.MaxLength = 10;
				colvarTextValue.AutoIncrement = false;
				colvarTextValue.IsNullable = true;
				colvarTextValue.IsPrimaryKey = false;
				colvarTextValue.IsForeignKey = false;
				colvarTextValue.IsReadOnly = false;
				colvarTextValue.DefaultSetting = @"";
				colvarTextValue.ForeignKeyTableName = "";
				schema.Columns.Add(colvarTextValue);
				
				TableSchema.TableColumn colvarElevation = new TableSchema.TableColumn(schema);
				colvarElevation.ColumnName = "Elevation";
				colvarElevation.DataType = DbType.Double;
				colvarElevation.MaxLength = 0;
				colvarElevation.AutoIncrement = false;
				colvarElevation.IsNullable = true;
				colvarElevation.IsPrimaryKey = false;
				colvarElevation.IsForeignKey = false;
				colvarElevation.IsReadOnly = false;
				colvarElevation.DefaultSetting = @"";
				colvarElevation.ForeignKeyTableName = "";
				schema.Columns.Add(colvarElevation);
				
				TableSchema.TableColumn colvarLatitude = new TableSchema.TableColumn(schema);
				colvarLatitude.ColumnName = "Latitude";
				colvarLatitude.DataType = DbType.Double;
				colvarLatitude.MaxLength = 0;
				colvarLatitude.AutoIncrement = false;
				colvarLatitude.IsNullable = true;
				colvarLatitude.IsPrimaryKey = false;
				colvarLatitude.IsForeignKey = false;
				colvarLatitude.IsReadOnly = false;
				colvarLatitude.DefaultSetting = @"";
				colvarLatitude.ForeignKeyTableName = "";
				schema.Columns.Add(colvarLatitude);
				
				TableSchema.TableColumn colvarLongitude = new TableSchema.TableColumn(schema);
				colvarLongitude.ColumnName = "Longitude";
				colvarLongitude.DataType = DbType.Double;
				colvarLongitude.MaxLength = 0;
				colvarLongitude.AutoIncrement = false;
				colvarLongitude.IsNullable = true;
				colvarLongitude.IsPrimaryKey = false;
				colvarLongitude.IsForeignKey = false;
				colvarLongitude.IsReadOnly = false;
				colvarLongitude.DefaultSetting = @"";
				colvarLongitude.ForeignKeyTableName = "";
				schema.Columns.Add(colvarLongitude);
				
				TableSchema.TableColumn colvarValueDay = new TableSchema.TableColumn(schema);
				colvarValueDay.ColumnName = "ValueDay";
				colvarValueDay.DataType = DbType.Date;
				colvarValueDay.MaxLength = 0;
				colvarValueDay.AutoIncrement = false;
				colvarValueDay.IsNullable = true;
				colvarValueDay.IsPrimaryKey = false;
				colvarValueDay.IsForeignKey = false;
				colvarValueDay.IsReadOnly = true;
				colvarValueDay.DefaultSetting = @"";
				colvarValueDay.ForeignKeyTableName = "";
				schema.Columns.Add(colvarValueDay);
				
				TableSchema.TableColumn colvarValueYear = new TableSchema.TableColumn(schema);
				colvarValueYear.ColumnName = "ValueYear";
				colvarValueYear.DataType = DbType.Int32;
				colvarValueYear.MaxLength = 0;
				colvarValueYear.AutoIncrement = false;
				colvarValueYear.IsNullable = true;
				colvarValueYear.IsPrimaryKey = false;
				colvarValueYear.IsForeignKey = false;
				colvarValueYear.IsReadOnly = true;
				colvarValueYear.DefaultSetting = @"";
				colvarValueYear.ForeignKeyTableName = "";
				schema.Columns.Add(colvarValueYear);
				
				TableSchema.TableColumn colvarValueDecade = new TableSchema.TableColumn(schema);
				colvarValueDecade.ColumnName = "ValueDecade";
				colvarValueDecade.DataType = DbType.Int32;
				colvarValueDecade.MaxLength = 0;
				colvarValueDecade.AutoIncrement = false;
				colvarValueDecade.IsNullable = true;
				colvarValueDecade.IsPrimaryKey = false;
				colvarValueDecade.IsForeignKey = false;
				colvarValueDecade.IsReadOnly = true;
				colvarValueDecade.DefaultSetting = @"";
				colvarValueDecade.ForeignKeyTableName = "";
				schema.Columns.Add(colvarValueDecade);
				
				TableSchema.TableColumn colvarVerifiedBy = new TableSchema.TableColumn(schema);
				colvarVerifiedBy.ColumnName = "VerifiedBy";
				colvarVerifiedBy.DataType = DbType.Guid;
				colvarVerifiedBy.MaxLength = 0;
				colvarVerifiedBy.AutoIncrement = false;
				colvarVerifiedBy.IsNullable = true;
				colvarVerifiedBy.IsPrimaryKey = false;
				colvarVerifiedBy.IsForeignKey = false;
				colvarVerifiedBy.IsReadOnly = false;
				colvarVerifiedBy.DefaultSetting = @"";
				colvarVerifiedBy.ForeignKeyTableName = "";
				schema.Columns.Add(colvarVerifiedBy);
				
				TableSchema.TableColumn colvarVerifiedAt = new TableSchema.TableColumn(schema);
				colvarVerifiedAt.ColumnName = "VerifiedAt";
				colvarVerifiedAt.DataType = DbType.DateTime;
				colvarVerifiedAt.MaxLength = 0;
				colvarVerifiedAt.AutoIncrement = false;
				colvarVerifiedAt.IsNullable = true;
				colvarVerifiedAt.IsPrimaryKey = false;
				colvarVerifiedAt.IsForeignKey = false;
				colvarVerifiedAt.IsReadOnly = false;
				colvarVerifiedAt.DefaultSetting = @"";
				colvarVerifiedAt.ForeignKeyTableName = "";
				schema.Columns.Add(colvarVerifiedAt);
				
				TableSchema.TableColumn colvarUnverifiedBy = new TableSchema.TableColumn(schema);
				colvarUnverifiedBy.ColumnName = "UnverifiedBy";
				colvarUnverifiedBy.DataType = DbType.Guid;
				colvarUnverifiedBy.MaxLength = 0;
				colvarUnverifiedBy.AutoIncrement = false;
				colvarUnverifiedBy.IsNullable = true;
				colvarUnverifiedBy.IsPrimaryKey = false;
				colvarUnverifiedBy.IsForeignKey = false;
				colvarUnverifiedBy.IsReadOnly = false;
				colvarUnverifiedBy.DefaultSetting = @"";
				colvarUnverifiedBy.ForeignKeyTableName = "";
				schema.Columns.Add(colvarUnverifiedBy);
				
				TableSchema.TableColumn colvarUnverifiedAt = new TableSchema.TableColumn(schema);
				colvarUnverifiedAt.ColumnName = "UnverifiedAt";
				colvarUnverifiedAt.DataType = DbType.DateTime;
				colvarUnverifiedAt.MaxLength = 0;
				colvarUnverifiedAt.AutoIncrement = false;
				colvarUnverifiedAt.IsNullable = true;
				colvarUnverifiedAt.IsPrimaryKey = false;
				colvarUnverifiedAt.IsForeignKey = false;
				colvarUnverifiedAt.IsReadOnly = false;
				colvarUnverifiedAt.DefaultSetting = @"";
				colvarUnverifiedAt.ForeignKeyTableName = "";
				schema.Columns.Add(colvarUnverifiedAt);
				
				BaseSchema = schema;
				//add this schema to the provider
				//so we can query it later
				DataService.Providers["ObservationsDB"].AddSchema("Observation",schema);
			}
		}
		#endregion
		
		#region Props
		  
		[XmlAttribute("SensorID")]
		[Bindable(true)]
		public Guid SensorID 
		{
			get { return GetColumnValue<Guid>(Columns.SensorID); }
			set { SetColumnValue(Columns.SensorID, value); }
		}
		  
		[XmlAttribute("ValueDate")]
		[Bindable(true)]
		public DateTime ValueDate 
		{
			get { return GetColumnValue<DateTime>(Columns.ValueDate); }
			set { SetColumnValue(Columns.ValueDate, value); }
		}
		  
		[XmlAttribute("RawValue")]
		[Bindable(true)]
		public double? RawValue 
		{
			get { return GetColumnValue<double?>(Columns.RawValue); }
			set { SetColumnValue(Columns.RawValue, value); }
		}
		  
		[XmlAttribute("DataValue")]
		[Bindable(true)]
		public double? DataValue 
		{
			get { return GetColumnValue<double?>(Columns.DataValue); }
			set { SetColumnValue(Columns.DataValue, value); }
		}
		  
		[XmlAttribute("Comment")]
		[Bindable(true)]
		public string Comment 
		{
			get { return GetColumnValue<string>(Columns.Comment); }
			set { SetColumnValue(Columns.Comment, value); }
		}
		  
		[XmlAttribute("PhenomenonOfferingID")]
		[Bindable(true)]
		public Guid PhenomenonOfferingID 
		{
			get { return GetColumnValue<Guid>(Columns.PhenomenonOfferingID); }
			set { SetColumnValue(Columns.PhenomenonOfferingID, value); }
		}
		  
		[XmlAttribute("PhenomenonUOMID")]
		[Bindable(true)]
		public Guid PhenomenonUOMID 
		{
			get { return GetColumnValue<Guid>(Columns.PhenomenonUOMID); }
			set { SetColumnValue(Columns.PhenomenonUOMID, value); }
		}
		  
		[XmlAttribute("ImportBatchID")]
		[Bindable(true)]
		public Guid ImportBatchID 
		{
			get { return GetColumnValue<Guid>(Columns.ImportBatchID); }
			set { SetColumnValue(Columns.ImportBatchID, value); }
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
		  
		[XmlAttribute("CorrelationID")]
		[Bindable(true)]
		public Guid? CorrelationID 
		{
			get { return GetColumnValue<Guid?>(Columns.CorrelationID); }
			set { SetColumnValue(Columns.CorrelationID, value); }
		}
		  
		[XmlAttribute("UserId")]
		[Bindable(true)]
		public Guid UserId 
		{
			get { return GetColumnValue<Guid>(Columns.UserId); }
			set { SetColumnValue(Columns.UserId, value); }
		}
		  
		[XmlAttribute("AddedDate")]
		[Bindable(true)]
		public DateTime AddedDate 
		{
			get { return GetColumnValue<DateTime>(Columns.AddedDate); }
			set { SetColumnValue(Columns.AddedDate, value); }
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
		  
		[XmlAttribute("Id")]
		[Bindable(true)]
		public int Id 
		{
			get { return GetColumnValue<int>(Columns.Id); }
			set { SetColumnValue(Columns.Id, value); }
		}
		  
		[XmlAttribute("RowVersion")]
		[Bindable(true)]
		public byte[] RowVersion 
		{
			get { return GetColumnValue<byte[]>(Columns.RowVersion); }
			set { SetColumnValue(Columns.RowVersion, value); }
		}
		  
		[XmlAttribute("TextValue")]
		[Bindable(true)]
		public string TextValue 
		{
			get { return GetColumnValue<string>(Columns.TextValue); }
			set { SetColumnValue(Columns.TextValue, value); }
		}
		  
		[XmlAttribute("Elevation")]
		[Bindable(true)]
		public double? Elevation 
		{
			get { return GetColumnValue<double?>(Columns.Elevation); }
			set { SetColumnValue(Columns.Elevation, value); }
		}
		  
		[XmlAttribute("Latitude")]
		[Bindable(true)]
		public double? Latitude 
		{
			get { return GetColumnValue<double?>(Columns.Latitude); }
			set { SetColumnValue(Columns.Latitude, value); }
		}
		  
		[XmlAttribute("Longitude")]
		[Bindable(true)]
		public double? Longitude 
		{
			get { return GetColumnValue<double?>(Columns.Longitude); }
			set { SetColumnValue(Columns.Longitude, value); }
		}
		  
		[XmlAttribute("ValueDay")]
		[Bindable(true)]
		public DateTime? ValueDay 
		{
			get { return GetColumnValue<DateTime?>(Columns.ValueDay); }
			set { SetColumnValue(Columns.ValueDay, value); }
		}
		  
		[XmlAttribute("ValueYear")]
		[Bindable(true)]
		public int? ValueYear 
		{
			get { return GetColumnValue<int?>(Columns.ValueYear); }
			set { SetColumnValue(Columns.ValueYear, value); }
		}
		  
		[XmlAttribute("ValueDecade")]
		[Bindable(true)]
		public int? ValueDecade 
		{
			get { return GetColumnValue<int?>(Columns.ValueDecade); }
			set { SetColumnValue(Columns.ValueDecade, value); }
		}
		  
		[XmlAttribute("VerifiedBy")]
		[Bindable(true)]
		public Guid? VerifiedBy 
		{
			get { return GetColumnValue<Guid?>(Columns.VerifiedBy); }
			set { SetColumnValue(Columns.VerifiedBy, value); }
		}
		  
		[XmlAttribute("VerifiedAt")]
		[Bindable(true)]
		public DateTime? VerifiedAt 
		{
			get { return GetColumnValue<DateTime?>(Columns.VerifiedAt); }
			set { SetColumnValue(Columns.VerifiedAt, value); }
		}
		  
		[XmlAttribute("UnverifiedBy")]
		[Bindable(true)]
		public Guid? UnverifiedBy 
		{
			get { return GetColumnValue<Guid?>(Columns.UnverifiedBy); }
			set { SetColumnValue(Columns.UnverifiedBy, value); }
		}
		  
		[XmlAttribute("UnverifiedAt")]
		[Bindable(true)]
		public DateTime? UnverifiedAt 
		{
			get { return GetColumnValue<DateTime?>(Columns.UnverifiedAt); }
			set { SetColumnValue(Columns.UnverifiedAt, value); }
		}
		
		#endregion
		
		
			
		
		#region ForeignKey Properties
		
        private SAEON.Observations.Data.AspnetUser _AspnetUser = null;
		/// <summary>
		/// Returns a AspnetUser ActiveRecord object related to this Observation
		/// 
		/// </summary>
		public SAEON.Observations.Data.AspnetUser AspnetUser
		{
//			get { return SAEON.Observations.Data.AspnetUser.FetchByID(this.UserId); }  
			get { return _AspnetUser ?? (_AspnetUser = SAEON.Observations.Data.AspnetUser.FetchByID(this.UserId)); }
			set { SetColumnValue("UserId", value.UserId); }
		}
		
		
        private SAEON.Observations.Data.ImportBatch _ImportBatch = null;
		/// <summary>
		/// Returns a ImportBatch ActiveRecord object related to this Observation
		/// 
		/// </summary>
		public SAEON.Observations.Data.ImportBatch ImportBatch
		{
//			get { return SAEON.Observations.Data.ImportBatch.FetchByID(this.ImportBatchID); }  
			get { return _ImportBatch ?? (_ImportBatch = SAEON.Observations.Data.ImportBatch.FetchByID(this.ImportBatchID)); }
			set { SetColumnValue("ImportBatchID", value.Id); }
		}
		
		
        private SAEON.Observations.Data.PhenomenonOffering _PhenomenonOffering = null;
		/// <summary>
		/// Returns a PhenomenonOffering ActiveRecord object related to this Observation
		/// 
		/// </summary>
		public SAEON.Observations.Data.PhenomenonOffering PhenomenonOffering
		{
//			get { return SAEON.Observations.Data.PhenomenonOffering.FetchByID(this.PhenomenonOfferingID); }  
			get { return _PhenomenonOffering ?? (_PhenomenonOffering = SAEON.Observations.Data.PhenomenonOffering.FetchByID(this.PhenomenonOfferingID)); }
			set { SetColumnValue("PhenomenonOfferingID", value.Id); }
		}
		
		
        private SAEON.Observations.Data.PhenomenonUOM _PhenomenonUOM = null;
		/// <summary>
		/// Returns a PhenomenonUOM ActiveRecord object related to this Observation
		/// 
		/// </summary>
		public SAEON.Observations.Data.PhenomenonUOM PhenomenonUOM
		{
//			get { return SAEON.Observations.Data.PhenomenonUOM.FetchByID(this.PhenomenonUOMID); }  
			get { return _PhenomenonUOM ?? (_PhenomenonUOM = SAEON.Observations.Data.PhenomenonUOM.FetchByID(this.PhenomenonUOMID)); }
			set { SetColumnValue("PhenomenonUOMID", value.Id); }
		}
		
		
        private SAEON.Observations.Data.Sensor _Sensor = null;
		/// <summary>
		/// Returns a Sensor ActiveRecord object related to this Observation
		/// 
		/// </summary>
		public SAEON.Observations.Data.Sensor Sensor
		{
//			get { return SAEON.Observations.Data.Sensor.FetchByID(this.SensorID); }  
			get { return _Sensor ?? (_Sensor = SAEON.Observations.Data.Sensor.FetchByID(this.SensorID)); }
			set { SetColumnValue("SensorID", value.Id); }
		}
		
		
        private SAEON.Observations.Data.Status _Status = null;
		/// <summary>
		/// Returns a Status ActiveRecord object related to this Observation
		/// 
		/// </summary>
		public SAEON.Observations.Data.Status Status
		{
//			get { return SAEON.Observations.Data.Status.FetchByID(this.StatusID); }  
			get { return _Status ?? (_Status = SAEON.Observations.Data.Status.FetchByID(this.StatusID)); }
			set { SetColumnValue("StatusID", value.Id); }
		}
		
		
        private SAEON.Observations.Data.StatusReason _StatusReason = null;
		/// <summary>
		/// Returns a StatusReason ActiveRecord object related to this Observation
		/// 
		/// </summary>
		public SAEON.Observations.Data.StatusReason StatusReason
		{
//			get { return SAEON.Observations.Data.StatusReason.FetchByID(this.StatusReasonID); }  
			get { return _StatusReason ?? (_StatusReason = SAEON.Observations.Data.StatusReason.FetchByID(this.StatusReasonID)); }
			set { SetColumnValue("StatusReasonID", value.Id); }
		}
		
		
		#endregion
		
		
		
		//no ManyToMany tables defined (0)
		
        
        
		#region ObjectDataSource support
		
		
		/// <summary>
		/// Inserts a record, can be used with the Object Data Source
		/// </summary>
		public static void Insert(Guid varSensorID,DateTime varValueDate,double? varRawValue,double? varDataValue,string varComment,Guid varPhenomenonOfferingID,Guid varPhenomenonUOMID,Guid varImportBatchID,Guid? varStatusID,Guid? varStatusReasonID,Guid? varCorrelationID,Guid varUserId,DateTime varAddedDate,DateTime? varAddedAt,DateTime? varUpdatedAt,byte[] varRowVersion,string varTextValue,double? varElevation,double? varLatitude,double? varLongitude,DateTime? varValueDay,int? varValueYear,int? varValueDecade,Guid? varVerifiedBy,DateTime? varVerifiedAt,Guid? varUnverifiedBy,DateTime? varUnverifiedAt)
		{
			Observation item = new Observation();
			
			item.SensorID = varSensorID;
			
			item.ValueDate = varValueDate;
			
			item.RawValue = varRawValue;
			
			item.DataValue = varDataValue;
			
			item.Comment = varComment;
			
			item.PhenomenonOfferingID = varPhenomenonOfferingID;
			
			item.PhenomenonUOMID = varPhenomenonUOMID;
			
			item.ImportBatchID = varImportBatchID;
			
			item.StatusID = varStatusID;
			
			item.StatusReasonID = varStatusReasonID;
			
			item.CorrelationID = varCorrelationID;
			
			item.UserId = varUserId;
			
			item.AddedDate = varAddedDate;
			
			item.AddedAt = varAddedAt;
			
			item.UpdatedAt = varUpdatedAt;
			
			item.RowVersion = varRowVersion;
			
			item.TextValue = varTextValue;
			
			item.Elevation = varElevation;
			
			item.Latitude = varLatitude;
			
			item.Longitude = varLongitude;
			
			item.ValueDay = varValueDay;
			
			item.ValueYear = varValueYear;
			
			item.ValueDecade = varValueDecade;
			
			item.VerifiedBy = varVerifiedBy;
			
			item.VerifiedAt = varVerifiedAt;
			
			item.UnverifiedBy = varUnverifiedBy;
			
			item.UnverifiedAt = varUnverifiedAt;
			
		
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		
		/// <summary>
		/// Updates a record, can be used with the Object Data Source
		/// </summary>
		public static void Update(Guid varSensorID,DateTime varValueDate,double? varRawValue,double? varDataValue,string varComment,Guid varPhenomenonOfferingID,Guid varPhenomenonUOMID,Guid varImportBatchID,Guid? varStatusID,Guid? varStatusReasonID,Guid? varCorrelationID,Guid varUserId,DateTime varAddedDate,DateTime? varAddedAt,DateTime? varUpdatedAt,int varId,byte[] varRowVersion,string varTextValue,double? varElevation,double? varLatitude,double? varLongitude,DateTime? varValueDay,int? varValueYear,int? varValueDecade,Guid? varVerifiedBy,DateTime? varVerifiedAt,Guid? varUnverifiedBy,DateTime? varUnverifiedAt)
		{
			Observation item = new Observation();
			
				item.SensorID = varSensorID;
			
				item.ValueDate = varValueDate;
			
				item.RawValue = varRawValue;
			
				item.DataValue = varDataValue;
			
				item.Comment = varComment;
			
				item.PhenomenonOfferingID = varPhenomenonOfferingID;
			
				item.PhenomenonUOMID = varPhenomenonUOMID;
			
				item.ImportBatchID = varImportBatchID;
			
				item.StatusID = varStatusID;
			
				item.StatusReasonID = varStatusReasonID;
			
				item.CorrelationID = varCorrelationID;
			
				item.UserId = varUserId;
			
				item.AddedDate = varAddedDate;
			
				item.AddedAt = varAddedAt;
			
				item.UpdatedAt = varUpdatedAt;
			
				item.Id = varId;
			
				item.RowVersion = varRowVersion;
			
				item.TextValue = varTextValue;
			
				item.Elevation = varElevation;
			
				item.Latitude = varLatitude;
			
				item.Longitude = varLongitude;
			
				item.ValueDay = varValueDay;
			
				item.ValueYear = varValueYear;
			
				item.ValueDecade = varValueDecade;
			
				item.VerifiedBy = varVerifiedBy;
			
				item.VerifiedAt = varVerifiedAt;
			
				item.UnverifiedBy = varUnverifiedBy;
			
				item.UnverifiedAt = varUnverifiedAt;
			
			item.IsNew = false;
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		#endregion
        
        
        
        #region Typed Columns
        
        
        public static TableSchema.TableColumn SensorIDColumn
        {
            get { return Schema.Columns[0]; }
        }
        
        
        
        public static TableSchema.TableColumn ValueDateColumn
        {
            get { return Schema.Columns[1]; }
        }
        
        
        
        public static TableSchema.TableColumn RawValueColumn
        {
            get { return Schema.Columns[2]; }
        }
        
        
        
        public static TableSchema.TableColumn DataValueColumn
        {
            get { return Schema.Columns[3]; }
        }
        
        
        
        public static TableSchema.TableColumn CommentColumn
        {
            get { return Schema.Columns[4]; }
        }
        
        
        
        public static TableSchema.TableColumn PhenomenonOfferingIDColumn
        {
            get { return Schema.Columns[5]; }
        }
        
        
        
        public static TableSchema.TableColumn PhenomenonUOMIDColumn
        {
            get { return Schema.Columns[6]; }
        }
        
        
        
        public static TableSchema.TableColumn ImportBatchIDColumn
        {
            get { return Schema.Columns[7]; }
        }
        
        
        
        public static TableSchema.TableColumn StatusIDColumn
        {
            get { return Schema.Columns[8]; }
        }
        
        
        
        public static TableSchema.TableColumn StatusReasonIDColumn
        {
            get { return Schema.Columns[9]; }
        }
        
        
        
        public static TableSchema.TableColumn CorrelationIDColumn
        {
            get { return Schema.Columns[10]; }
        }
        
        
        
        public static TableSchema.TableColumn UserIdColumn
        {
            get { return Schema.Columns[11]; }
        }
        
        
        
        public static TableSchema.TableColumn AddedDateColumn
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
        
        
        
        public static TableSchema.TableColumn IdColumn
        {
            get { return Schema.Columns[15]; }
        }
        
        
        
        public static TableSchema.TableColumn RowVersionColumn
        {
            get { return Schema.Columns[16]; }
        }
        
        
        
        public static TableSchema.TableColumn TextValueColumn
        {
            get { return Schema.Columns[17]; }
        }
        
        
        
        public static TableSchema.TableColumn ElevationColumn
        {
            get { return Schema.Columns[18]; }
        }
        
        
        
        public static TableSchema.TableColumn LatitudeColumn
        {
            get { return Schema.Columns[19]; }
        }
        
        
        
        public static TableSchema.TableColumn LongitudeColumn
        {
            get { return Schema.Columns[20]; }
        }
        
        
        
        public static TableSchema.TableColumn ValueDayColumn
        {
            get { return Schema.Columns[21]; }
        }
        
        
        
        public static TableSchema.TableColumn ValueYearColumn
        {
            get { return Schema.Columns[22]; }
        }
        
        
        
        public static TableSchema.TableColumn ValueDecadeColumn
        {
            get { return Schema.Columns[23]; }
        }
        
        
        
        public static TableSchema.TableColumn VerifiedByColumn
        {
            get { return Schema.Columns[24]; }
        }
        
        
        
        public static TableSchema.TableColumn VerifiedAtColumn
        {
            get { return Schema.Columns[25]; }
        }
        
        
        
        public static TableSchema.TableColumn UnverifiedByColumn
        {
            get { return Schema.Columns[26]; }
        }
        
        
        
        public static TableSchema.TableColumn UnverifiedAtColumn
        {
            get { return Schema.Columns[27]; }
        }
        
        
        
        #endregion
		#region Columns Struct
		public struct Columns
		{
			 public static string SensorID = @"SensorID";
			 public static string ValueDate = @"ValueDate";
			 public static string RawValue = @"RawValue";
			 public static string DataValue = @"DataValue";
			 public static string Comment = @"Comment";
			 public static string PhenomenonOfferingID = @"PhenomenonOfferingID";
			 public static string PhenomenonUOMID = @"PhenomenonUOMID";
			 public static string ImportBatchID = @"ImportBatchID";
			 public static string StatusID = @"StatusID";
			 public static string StatusReasonID = @"StatusReasonID";
			 public static string CorrelationID = @"CorrelationID";
			 public static string UserId = @"UserId";
			 public static string AddedDate = @"AddedDate";
			 public static string AddedAt = @"AddedAt";
			 public static string UpdatedAt = @"UpdatedAt";
			 public static string Id = @"ID";
			 public static string RowVersion = @"RowVersion";
			 public static string TextValue = @"TextValue";
			 public static string Elevation = @"Elevation";
			 public static string Latitude = @"Latitude";
			 public static string Longitude = @"Longitude";
			 public static string ValueDay = @"ValueDay";
			 public static string ValueYear = @"ValueYear";
			 public static string ValueDecade = @"ValueDecade";
			 public static string VerifiedBy = @"VerifiedBy";
			 public static string VerifiedAt = @"VerifiedAt";
			 public static string UnverifiedBy = @"UnverifiedBy";
			 public static string UnverifiedAt = @"UnverifiedAt";
						
		}
		#endregion
		
		#region Update PK Collections
		
        #endregion
    
        #region Deep Save
		
        #endregion
	}
}
