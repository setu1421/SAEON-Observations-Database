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
	/// Strongly-typed collection for the ImportBatchSummary class.
	/// </summary>
    [Serializable]
	public partial class ImportBatchSummaryCollection : ActiveList<ImportBatchSummary, ImportBatchSummaryCollection>
	{	   
		public ImportBatchSummaryCollection() {}
        
        /// <summary>
		/// Filters an existing collection based on the set criteria. This is an in-memory filter
		/// Thanks to developingchris for this!
        /// </summary>
        /// <returns>ImportBatchSummaryCollection</returns>
		public ImportBatchSummaryCollection Filter()
        {
            for (int i = this.Count - 1; i > -1; i--)
            {
                ImportBatchSummary o = this[i];
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
	/// This is an ActiveRecord class which wraps the ImportBatchSummary table.
	/// </summary>
	[Serializable]
	public partial class ImportBatchSummary : ActiveRecord<ImportBatchSummary>, IActiveRecord
	{
		#region .ctors and Default Settings
		
		public ImportBatchSummary()
		{
		  SetSQLProps();
		  InitSetDefaults();
		  MarkNew();
		}
		
		private void InitSetDefaults() { SetDefaults(); }
		
		public ImportBatchSummary(bool useDatabaseDefaults)
		{
			SetSQLProps();
			if(useDatabaseDefaults)
				ForceDefaults();
			MarkNew();
		}
        
		public ImportBatchSummary(object keyID)
		{
			SetSQLProps();
			InitSetDefaults();
			LoadByKey(keyID);
		}
		 
		public ImportBatchSummary(string columnName, object columnValue)
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
				TableSchema.Table schema = new TableSchema.Table("ImportBatchSummary", TableType.Table, DataService.GetInstance("ObservationsDB"));
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
				
				TableSchema.TableColumn colvarInstrumentID = new TableSchema.TableColumn(schema);
				colvarInstrumentID.ColumnName = "InstrumentID";
				colvarInstrumentID.DataType = DbType.Guid;
				colvarInstrumentID.MaxLength = 0;
				colvarInstrumentID.AutoIncrement = false;
				colvarInstrumentID.IsNullable = false;
				colvarInstrumentID.IsPrimaryKey = false;
				colvarInstrumentID.IsForeignKey = true;
				colvarInstrumentID.IsReadOnly = false;
				colvarInstrumentID.DefaultSetting = @"";
				
					colvarInstrumentID.ForeignKeyTableName = "Instrument";
				schema.Columns.Add(colvarInstrumentID);
				
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
				
				TableSchema.TableColumn colvarSiteID = new TableSchema.TableColumn(schema);
				colvarSiteID.ColumnName = "SiteID";
				colvarSiteID.DataType = DbType.Guid;
				colvarSiteID.MaxLength = 0;
				colvarSiteID.AutoIncrement = false;
				colvarSiteID.IsNullable = false;
				colvarSiteID.IsPrimaryKey = false;
				colvarSiteID.IsForeignKey = true;
				colvarSiteID.IsReadOnly = false;
				colvarSiteID.DefaultSetting = @"";
				
					colvarSiteID.ForeignKeyTableName = "Site";
				schema.Columns.Add(colvarSiteID);
				
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
				
				TableSchema.TableColumn colvarCount = new TableSchema.TableColumn(schema);
				colvarCount.ColumnName = "Count";
				colvarCount.DataType = DbType.Int32;
				colvarCount.MaxLength = 0;
				colvarCount.AutoIncrement = false;
				colvarCount.IsNullable = false;
				colvarCount.IsPrimaryKey = false;
				colvarCount.IsForeignKey = false;
				colvarCount.IsReadOnly = false;
				colvarCount.DefaultSetting = @"";
				colvarCount.ForeignKeyTableName = "";
				schema.Columns.Add(colvarCount);
				
				TableSchema.TableColumn colvarMinimum = new TableSchema.TableColumn(schema);
				colvarMinimum.ColumnName = "Minimum";
				colvarMinimum.DataType = DbType.Double;
				colvarMinimum.MaxLength = 0;
				colvarMinimum.AutoIncrement = false;
				colvarMinimum.IsNullable = true;
				colvarMinimum.IsPrimaryKey = false;
				colvarMinimum.IsForeignKey = false;
				colvarMinimum.IsReadOnly = false;
				colvarMinimum.DefaultSetting = @"";
				colvarMinimum.ForeignKeyTableName = "";
				schema.Columns.Add(colvarMinimum);
				
				TableSchema.TableColumn colvarMaximum = new TableSchema.TableColumn(schema);
				colvarMaximum.ColumnName = "Maximum";
				colvarMaximum.DataType = DbType.Double;
				colvarMaximum.MaxLength = 0;
				colvarMaximum.AutoIncrement = false;
				colvarMaximum.IsNullable = true;
				colvarMaximum.IsPrimaryKey = false;
				colvarMaximum.IsForeignKey = false;
				colvarMaximum.IsReadOnly = false;
				colvarMaximum.DefaultSetting = @"";
				colvarMaximum.ForeignKeyTableName = "";
				schema.Columns.Add(colvarMaximum);
				
				TableSchema.TableColumn colvarAverage = new TableSchema.TableColumn(schema);
				colvarAverage.ColumnName = "Average";
				colvarAverage.DataType = DbType.Double;
				colvarAverage.MaxLength = 0;
				colvarAverage.AutoIncrement = false;
				colvarAverage.IsNullable = true;
				colvarAverage.IsPrimaryKey = false;
				colvarAverage.IsForeignKey = false;
				colvarAverage.IsReadOnly = false;
				colvarAverage.DefaultSetting = @"";
				colvarAverage.ForeignKeyTableName = "";
				schema.Columns.Add(colvarAverage);
				
				TableSchema.TableColumn colvarStandardDeviation = new TableSchema.TableColumn(schema);
				colvarStandardDeviation.ColumnName = "StandardDeviation";
				colvarStandardDeviation.DataType = DbType.Double;
				colvarStandardDeviation.MaxLength = 0;
				colvarStandardDeviation.AutoIncrement = false;
				colvarStandardDeviation.IsNullable = true;
				colvarStandardDeviation.IsPrimaryKey = false;
				colvarStandardDeviation.IsForeignKey = false;
				colvarStandardDeviation.IsReadOnly = false;
				colvarStandardDeviation.DefaultSetting = @"";
				colvarStandardDeviation.ForeignKeyTableName = "";
				schema.Columns.Add(colvarStandardDeviation);
				
				TableSchema.TableColumn colvarVariance = new TableSchema.TableColumn(schema);
				colvarVariance.ColumnName = "Variance";
				colvarVariance.DataType = DbType.Double;
				colvarVariance.MaxLength = 0;
				colvarVariance.AutoIncrement = false;
				colvarVariance.IsNullable = true;
				colvarVariance.IsPrimaryKey = false;
				colvarVariance.IsForeignKey = false;
				colvarVariance.IsReadOnly = false;
				colvarVariance.DefaultSetting = @"";
				colvarVariance.ForeignKeyTableName = "";
				schema.Columns.Add(colvarVariance);
				
				TableSchema.TableColumn colvarTopLatitude = new TableSchema.TableColumn(schema);
				colvarTopLatitude.ColumnName = "TopLatitude";
				colvarTopLatitude.DataType = DbType.Double;
				colvarTopLatitude.MaxLength = 0;
				colvarTopLatitude.AutoIncrement = false;
				colvarTopLatitude.IsNullable = true;
				colvarTopLatitude.IsPrimaryKey = false;
				colvarTopLatitude.IsForeignKey = false;
				colvarTopLatitude.IsReadOnly = false;
				colvarTopLatitude.DefaultSetting = @"";
				colvarTopLatitude.ForeignKeyTableName = "";
				schema.Columns.Add(colvarTopLatitude);
				
				TableSchema.TableColumn colvarBottomLatitude = new TableSchema.TableColumn(schema);
				colvarBottomLatitude.ColumnName = "BottomLatitude";
				colvarBottomLatitude.DataType = DbType.Double;
				colvarBottomLatitude.MaxLength = 0;
				colvarBottomLatitude.AutoIncrement = false;
				colvarBottomLatitude.IsNullable = true;
				colvarBottomLatitude.IsPrimaryKey = false;
				colvarBottomLatitude.IsForeignKey = false;
				colvarBottomLatitude.IsReadOnly = false;
				colvarBottomLatitude.DefaultSetting = @"";
				colvarBottomLatitude.ForeignKeyTableName = "";
				schema.Columns.Add(colvarBottomLatitude);
				
				TableSchema.TableColumn colvarLeftLongitude = new TableSchema.TableColumn(schema);
				colvarLeftLongitude.ColumnName = "LeftLongitude";
				colvarLeftLongitude.DataType = DbType.Double;
				colvarLeftLongitude.MaxLength = 0;
				colvarLeftLongitude.AutoIncrement = false;
				colvarLeftLongitude.IsNullable = true;
				colvarLeftLongitude.IsPrimaryKey = false;
				colvarLeftLongitude.IsForeignKey = false;
				colvarLeftLongitude.IsReadOnly = false;
				colvarLeftLongitude.DefaultSetting = @"";
				colvarLeftLongitude.ForeignKeyTableName = "";
				schema.Columns.Add(colvarLeftLongitude);
				
				TableSchema.TableColumn colvarRightLongitude = new TableSchema.TableColumn(schema);
				colvarRightLongitude.ColumnName = "RightLongitude";
				colvarRightLongitude.DataType = DbType.Double;
				colvarRightLongitude.MaxLength = 0;
				colvarRightLongitude.AutoIncrement = false;
				colvarRightLongitude.IsNullable = true;
				colvarRightLongitude.IsPrimaryKey = false;
				colvarRightLongitude.IsForeignKey = false;
				colvarRightLongitude.IsReadOnly = false;
				colvarRightLongitude.DefaultSetting = @"";
				colvarRightLongitude.ForeignKeyTableName = "";
				schema.Columns.Add(colvarRightLongitude);
				
				TableSchema.TableColumn colvarStartDate = new TableSchema.TableColumn(schema);
				colvarStartDate.ColumnName = "StartDate";
				colvarStartDate.DataType = DbType.DateTime;
				colvarStartDate.MaxLength = 0;
				colvarStartDate.AutoIncrement = false;
				colvarStartDate.IsNullable = true;
				colvarStartDate.IsPrimaryKey = false;
				colvarStartDate.IsForeignKey = false;
				colvarStartDate.IsReadOnly = false;
				colvarStartDate.DefaultSetting = @"";
				colvarStartDate.ForeignKeyTableName = "";
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
				colvarEndDate.DefaultSetting = @"";
				colvarEndDate.ForeignKeyTableName = "";
				schema.Columns.Add(colvarEndDate);
				
				BaseSchema = schema;
				//add this schema to the provider
				//so we can query it later
				DataService.Providers["ObservationsDB"].AddSchema("ImportBatchSummary",schema);
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
		  
		[XmlAttribute("ImportBatchID")]
		[Bindable(true)]
		public Guid ImportBatchID 
		{
			get { return GetColumnValue<Guid>(Columns.ImportBatchID); }
			set { SetColumnValue(Columns.ImportBatchID, value); }
		}
		  
		[XmlAttribute("SensorID")]
		[Bindable(true)]
		public Guid SensorID 
		{
			get { return GetColumnValue<Guid>(Columns.SensorID); }
			set { SetColumnValue(Columns.SensorID, value); }
		}
		  
		[XmlAttribute("InstrumentID")]
		[Bindable(true)]
		public Guid InstrumentID 
		{
			get { return GetColumnValue<Guid>(Columns.InstrumentID); }
			set { SetColumnValue(Columns.InstrumentID, value); }
		}
		  
		[XmlAttribute("StationID")]
		[Bindable(true)]
		public Guid StationID 
		{
			get { return GetColumnValue<Guid>(Columns.StationID); }
			set { SetColumnValue(Columns.StationID, value); }
		}
		  
		[XmlAttribute("SiteID")]
		[Bindable(true)]
		public Guid SiteID 
		{
			get { return GetColumnValue<Guid>(Columns.SiteID); }
			set { SetColumnValue(Columns.SiteID, value); }
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
		  
		[XmlAttribute("Count")]
		[Bindable(true)]
		public int Count 
		{
			get { return GetColumnValue<int>(Columns.Count); }
			set { SetColumnValue(Columns.Count, value); }
		}
		  
		[XmlAttribute("Minimum")]
		[Bindable(true)]
		public double? Minimum 
		{
			get { return GetColumnValue<double?>(Columns.Minimum); }
			set { SetColumnValue(Columns.Minimum, value); }
		}
		  
		[XmlAttribute("Maximum")]
		[Bindable(true)]
		public double? Maximum 
		{
			get { return GetColumnValue<double?>(Columns.Maximum); }
			set { SetColumnValue(Columns.Maximum, value); }
		}
		  
		[XmlAttribute("Average")]
		[Bindable(true)]
		public double? Average 
		{
			get { return GetColumnValue<double?>(Columns.Average); }
			set { SetColumnValue(Columns.Average, value); }
		}
		  
		[XmlAttribute("StandardDeviation")]
		[Bindable(true)]
		public double? StandardDeviation 
		{
			get { return GetColumnValue<double?>(Columns.StandardDeviation); }
			set { SetColumnValue(Columns.StandardDeviation, value); }
		}
		  
		[XmlAttribute("Variance")]
		[Bindable(true)]
		public double? Variance 
		{
			get { return GetColumnValue<double?>(Columns.Variance); }
			set { SetColumnValue(Columns.Variance, value); }
		}
		  
		[XmlAttribute("TopLatitude")]
		[Bindable(true)]
		public double? TopLatitude 
		{
			get { return GetColumnValue<double?>(Columns.TopLatitude); }
			set { SetColumnValue(Columns.TopLatitude, value); }
		}
		  
		[XmlAttribute("BottomLatitude")]
		[Bindable(true)]
		public double? BottomLatitude 
		{
			get { return GetColumnValue<double?>(Columns.BottomLatitude); }
			set { SetColumnValue(Columns.BottomLatitude, value); }
		}
		  
		[XmlAttribute("LeftLongitude")]
		[Bindable(true)]
		public double? LeftLongitude 
		{
			get { return GetColumnValue<double?>(Columns.LeftLongitude); }
			set { SetColumnValue(Columns.LeftLongitude, value); }
		}
		  
		[XmlAttribute("RightLongitude")]
		[Bindable(true)]
		public double? RightLongitude 
		{
			get { return GetColumnValue<double?>(Columns.RightLongitude); }
			set { SetColumnValue(Columns.RightLongitude, value); }
		}
		  
		[XmlAttribute("StartDate")]
		[Bindable(true)]
		public DateTime? StartDate 
		{
			get { return GetColumnValue<DateTime?>(Columns.StartDate); }
			set { SetColumnValue(Columns.StartDate, value); }
		}
		  
		[XmlAttribute("EndDate")]
		[Bindable(true)]
		public DateTime? EndDate 
		{
			get { return GetColumnValue<DateTime?>(Columns.EndDate); }
			set { SetColumnValue(Columns.EndDate, value); }
		}
		
		#endregion
		
		
			
		
		#region ForeignKey Properties
		
		/// <summary>
		/// Returns a ImportBatch ActiveRecord object related to this ImportBatchSummary
		/// 
		/// </summary>
		public SAEON.Observations.Data.ImportBatch ImportBatch
		{
			get { return SAEON.Observations.Data.ImportBatch.FetchByID(this.ImportBatchID); }
			set { SetColumnValue("ImportBatchID", value.Id); }
		}
		
		
		/// <summary>
		/// Returns a Instrument ActiveRecord object related to this ImportBatchSummary
		/// 
		/// </summary>
		public SAEON.Observations.Data.Instrument Instrument
		{
			get { return SAEON.Observations.Data.Instrument.FetchByID(this.InstrumentID); }
			set { SetColumnValue("InstrumentID", value.Id); }
		}
		
		
		/// <summary>
		/// Returns a PhenomenonOffering ActiveRecord object related to this ImportBatchSummary
		/// 
		/// </summary>
		public SAEON.Observations.Data.PhenomenonOffering PhenomenonOffering
		{
			get { return SAEON.Observations.Data.PhenomenonOffering.FetchByID(this.PhenomenonOfferingID); }
			set { SetColumnValue("PhenomenonOfferingID", value.Id); }
		}
		
		
		/// <summary>
		/// Returns a PhenomenonUOM ActiveRecord object related to this ImportBatchSummary
		/// 
		/// </summary>
		public SAEON.Observations.Data.PhenomenonUOM PhenomenonUOM
		{
			get { return SAEON.Observations.Data.PhenomenonUOM.FetchByID(this.PhenomenonUOMID); }
			set { SetColumnValue("PhenomenonUOMID", value.Id); }
		}
		
		
		/// <summary>
		/// Returns a Sensor ActiveRecord object related to this ImportBatchSummary
		/// 
		/// </summary>
		public SAEON.Observations.Data.Sensor Sensor
		{
			get { return SAEON.Observations.Data.Sensor.FetchByID(this.SensorID); }
			set { SetColumnValue("SensorID", value.Id); }
		}
		
		
		/// <summary>
		/// Returns a Site ActiveRecord object related to this ImportBatchSummary
		/// 
		/// </summary>
		public SAEON.Observations.Data.Site Site
		{
			get { return SAEON.Observations.Data.Site.FetchByID(this.SiteID); }
			set { SetColumnValue("SiteID", value.Id); }
		}
		
		
		/// <summary>
		/// Returns a Station ActiveRecord object related to this ImportBatchSummary
		/// 
		/// </summary>
		public SAEON.Observations.Data.Station Station
		{
			get { return SAEON.Observations.Data.Station.FetchByID(this.StationID); }
			set { SetColumnValue("StationID", value.Id); }
		}
		
		
		#endregion
		
		
		
		//no ManyToMany tables defined (0)
		
        
        
		#region ObjectDataSource support
		
		
		/// <summary>
		/// Inserts a record, can be used with the Object Data Source
		/// </summary>
		public static void Insert(Guid varId,Guid varImportBatchID,Guid varSensorID,Guid varInstrumentID,Guid varStationID,Guid varSiteID,Guid varPhenomenonOfferingID,Guid varPhenomenonUOMID,int varCount,double? varMinimum,double? varMaximum,double? varAverage,double? varStandardDeviation,double? varVariance,double? varTopLatitude,double? varBottomLatitude,double? varLeftLongitude,double? varRightLongitude,DateTime? varStartDate,DateTime? varEndDate)
		{
			ImportBatchSummary item = new ImportBatchSummary();
			
			item.Id = varId;
			
			item.ImportBatchID = varImportBatchID;
			
			item.SensorID = varSensorID;
			
			item.InstrumentID = varInstrumentID;
			
			item.StationID = varStationID;
			
			item.SiteID = varSiteID;
			
			item.PhenomenonOfferingID = varPhenomenonOfferingID;
			
			item.PhenomenonUOMID = varPhenomenonUOMID;
			
			item.Count = varCount;
			
			item.Minimum = varMinimum;
			
			item.Maximum = varMaximum;
			
			item.Average = varAverage;
			
			item.StandardDeviation = varStandardDeviation;
			
			item.Variance = varVariance;
			
			item.TopLatitude = varTopLatitude;
			
			item.BottomLatitude = varBottomLatitude;
			
			item.LeftLongitude = varLeftLongitude;
			
			item.RightLongitude = varRightLongitude;
			
			item.StartDate = varStartDate;
			
			item.EndDate = varEndDate;
			
		
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		
		/// <summary>
		/// Updates a record, can be used with the Object Data Source
		/// </summary>
		public static void Update(Guid varId,Guid varImportBatchID,Guid varSensorID,Guid varInstrumentID,Guid varStationID,Guid varSiteID,Guid varPhenomenonOfferingID,Guid varPhenomenonUOMID,int varCount,double? varMinimum,double? varMaximum,double? varAverage,double? varStandardDeviation,double? varVariance,double? varTopLatitude,double? varBottomLatitude,double? varLeftLongitude,double? varRightLongitude,DateTime? varStartDate,DateTime? varEndDate)
		{
			ImportBatchSummary item = new ImportBatchSummary();
			
				item.Id = varId;
			
				item.ImportBatchID = varImportBatchID;
			
				item.SensorID = varSensorID;
			
				item.InstrumentID = varInstrumentID;
			
				item.StationID = varStationID;
			
				item.SiteID = varSiteID;
			
				item.PhenomenonOfferingID = varPhenomenonOfferingID;
			
				item.PhenomenonUOMID = varPhenomenonUOMID;
			
				item.Count = varCount;
			
				item.Minimum = varMinimum;
			
				item.Maximum = varMaximum;
			
				item.Average = varAverage;
			
				item.StandardDeviation = varStandardDeviation;
			
				item.Variance = varVariance;
			
				item.TopLatitude = varTopLatitude;
			
				item.BottomLatitude = varBottomLatitude;
			
				item.LeftLongitude = varLeftLongitude;
			
				item.RightLongitude = varRightLongitude;
			
				item.StartDate = varStartDate;
			
				item.EndDate = varEndDate;
			
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
        
        
        
        public static TableSchema.TableColumn ImportBatchIDColumn
        {
            get { return Schema.Columns[1]; }
        }
        
        
        
        public static TableSchema.TableColumn SensorIDColumn
        {
            get { return Schema.Columns[2]; }
        }
        
        
        
        public static TableSchema.TableColumn InstrumentIDColumn
        {
            get { return Schema.Columns[3]; }
        }
        
        
        
        public static TableSchema.TableColumn StationIDColumn
        {
            get { return Schema.Columns[4]; }
        }
        
        
        
        public static TableSchema.TableColumn SiteIDColumn
        {
            get { return Schema.Columns[5]; }
        }
        
        
        
        public static TableSchema.TableColumn PhenomenonOfferingIDColumn
        {
            get { return Schema.Columns[6]; }
        }
        
        
        
        public static TableSchema.TableColumn PhenomenonUOMIDColumn
        {
            get { return Schema.Columns[7]; }
        }
        
        
        
        public static TableSchema.TableColumn CountColumn
        {
            get { return Schema.Columns[8]; }
        }
        
        
        
        public static TableSchema.TableColumn MinimumColumn
        {
            get { return Schema.Columns[9]; }
        }
        
        
        
        public static TableSchema.TableColumn MaximumColumn
        {
            get { return Schema.Columns[10]; }
        }
        
        
        
        public static TableSchema.TableColumn AverageColumn
        {
            get { return Schema.Columns[11]; }
        }
        
        
        
        public static TableSchema.TableColumn StandardDeviationColumn
        {
            get { return Schema.Columns[12]; }
        }
        
        
        
        public static TableSchema.TableColumn VarianceColumn
        {
            get { return Schema.Columns[13]; }
        }
        
        
        
        public static TableSchema.TableColumn TopLatitudeColumn
        {
            get { return Schema.Columns[14]; }
        }
        
        
        
        public static TableSchema.TableColumn BottomLatitudeColumn
        {
            get { return Schema.Columns[15]; }
        }
        
        
        
        public static TableSchema.TableColumn LeftLongitudeColumn
        {
            get { return Schema.Columns[16]; }
        }
        
        
        
        public static TableSchema.TableColumn RightLongitudeColumn
        {
            get { return Schema.Columns[17]; }
        }
        
        
        
        public static TableSchema.TableColumn StartDateColumn
        {
            get { return Schema.Columns[18]; }
        }
        
        
        
        public static TableSchema.TableColumn EndDateColumn
        {
            get { return Schema.Columns[19]; }
        }
        
        
        
        #endregion
		#region Columns Struct
		public struct Columns
		{
			 public static string Id = @"ID";
			 public static string ImportBatchID = @"ImportBatchID";
			 public static string SensorID = @"SensorID";
			 public static string InstrumentID = @"InstrumentID";
			 public static string StationID = @"StationID";
			 public static string SiteID = @"SiteID";
			 public static string PhenomenonOfferingID = @"PhenomenonOfferingID";
			 public static string PhenomenonUOMID = @"PhenomenonUOMID";
			 public static string Count = @"Count";
			 public static string Minimum = @"Minimum";
			 public static string Maximum = @"Maximum";
			 public static string Average = @"Average";
			 public static string StandardDeviation = @"StandardDeviation";
			 public static string Variance = @"Variance";
			 public static string TopLatitude = @"TopLatitude";
			 public static string BottomLatitude = @"BottomLatitude";
			 public static string LeftLongitude = @"LeftLongitude";
			 public static string RightLongitude = @"RightLongitude";
			 public static string StartDate = @"StartDate";
			 public static string EndDate = @"EndDate";
						
		}
		#endregion
		
		#region Update PK Collections
		
        #endregion
    
        #region Deep Save
		
        #endregion
	}
}
