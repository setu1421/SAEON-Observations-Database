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
namespace SAEON.ObservationsDB.Data
{
	/// <summary>
	/// Strongly-typed collection for the SiteOrganisation class.
	/// </summary>
    [Serializable]
	public partial class SiteOrganisationCollection : ActiveList<SiteOrganisation, SiteOrganisationCollection>
	{	   
		public SiteOrganisationCollection() {}
        
        /// <summary>
		/// Filters an existing collection based on the set criteria. This is an in-memory filter
		/// Thanks to developingchris for this!
        /// </summary>
        /// <returns>SiteOrganisationCollection</returns>
		public SiteOrganisationCollection Filter()
        {
            for (int i = this.Count - 1; i > -1; i--)
            {
                SiteOrganisation o = this[i];
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
	/// This is an ActiveRecord class which wraps the Site_Organisation table.
	/// </summary>
	[Serializable]
	public partial class SiteOrganisation : ActiveRecord<SiteOrganisation>, IActiveRecord
	{
		#region .ctors and Default Settings
		
		public SiteOrganisation()
		{
		  SetSQLProps();
		  InitSetDefaults();
		  MarkNew();
		}
		
		private void InitSetDefaults() { SetDefaults(); }
		
		public SiteOrganisation(bool useDatabaseDefaults)
		{
			SetSQLProps();
			if(useDatabaseDefaults)
				ForceDefaults();
			MarkNew();
		}
        
		public SiteOrganisation(object keyID)
		{
			SetSQLProps();
			InitSetDefaults();
			LoadByKey(keyID);
		}
		 
		public SiteOrganisation(string columnName, object columnValue)
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
				TableSchema.Table schema = new TableSchema.Table("Site_Organisation", TableType.Table, DataService.GetInstance("ObservationsDB"));
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
				
				TableSchema.TableColumn colvarOrganisationID = new TableSchema.TableColumn(schema);
				colvarOrganisationID.ColumnName = "OrganisationID";
				colvarOrganisationID.DataType = DbType.Guid;
				colvarOrganisationID.MaxLength = 0;
				colvarOrganisationID.AutoIncrement = false;
				colvarOrganisationID.IsNullable = false;
				colvarOrganisationID.IsPrimaryKey = false;
				colvarOrganisationID.IsForeignKey = true;
				colvarOrganisationID.IsReadOnly = false;
				colvarOrganisationID.DefaultSetting = @"";
				
					colvarOrganisationID.ForeignKeyTableName = "Organisation";
				schema.Columns.Add(colvarOrganisationID);
				
				TableSchema.TableColumn colvarOrganisationRoleID = new TableSchema.TableColumn(schema);
				colvarOrganisationRoleID.ColumnName = "OrganisationRoleID";
				colvarOrganisationRoleID.DataType = DbType.Guid;
				colvarOrganisationRoleID.MaxLength = 0;
				colvarOrganisationRoleID.AutoIncrement = false;
				colvarOrganisationRoleID.IsNullable = false;
				colvarOrganisationRoleID.IsPrimaryKey = false;
				colvarOrganisationRoleID.IsForeignKey = true;
				colvarOrganisationRoleID.IsReadOnly = false;
				colvarOrganisationRoleID.DefaultSetting = @"";
				
					colvarOrganisationRoleID.ForeignKeyTableName = "OrganisationRole";
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
				DataService.Providers["ObservationsDB"].AddSchema("Site_Organisation",schema);
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
		  
		[XmlAttribute("SiteID")]
		[Bindable(true)]
		public Guid SiteID 
		{
			get { return GetColumnValue<Guid>(Columns.SiteID); }
			set { SetColumnValue(Columns.SiteID, value); }
		}
		  
		[XmlAttribute("OrganisationID")]
		[Bindable(true)]
		public Guid OrganisationID 
		{
			get { return GetColumnValue<Guid>(Columns.OrganisationID); }
			set { SetColumnValue(Columns.OrganisationID, value); }
		}
		  
		[XmlAttribute("OrganisationRoleID")]
		[Bindable(true)]
		public Guid OrganisationRoleID 
		{
			get { return GetColumnValue<Guid>(Columns.OrganisationRoleID); }
			set { SetColumnValue(Columns.OrganisationRoleID, value); }
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
		  
		[XmlAttribute("UserId")]
		[Bindable(true)]
		public Guid UserId 
		{
			get { return GetColumnValue<Guid>(Columns.UserId); }
			set { SetColumnValue(Columns.UserId, value); }
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
		/// Returns a AspnetUser ActiveRecord object related to this SiteOrganisation
		/// 
		/// </summary>
		public SAEON.ObservationsDB.Data.AspnetUser AspnetUser
		{
			get { return SAEON.ObservationsDB.Data.AspnetUser.FetchByID(this.UserId); }
			set { SetColumnValue("UserId", value.UserId); }
		}
		
		
		/// <summary>
		/// Returns a Organisation ActiveRecord object related to this SiteOrganisation
		/// 
		/// </summary>
		public SAEON.ObservationsDB.Data.Organisation Organisation
		{
			get { return SAEON.ObservationsDB.Data.Organisation.FetchByID(this.OrganisationID); }
			set { SetColumnValue("OrganisationID", value.Id); }
		}
		
		
		/// <summary>
		/// Returns a OrganisationRole ActiveRecord object related to this SiteOrganisation
		/// 
		/// </summary>
		public SAEON.ObservationsDB.Data.OrganisationRole OrganisationRole
		{
			get { return SAEON.ObservationsDB.Data.OrganisationRole.FetchByID(this.OrganisationRoleID); }
			set { SetColumnValue("OrganisationRoleID", value.Id); }
		}
		
		
		/// <summary>
		/// Returns a Site ActiveRecord object related to this SiteOrganisation
		/// 
		/// </summary>
		public SAEON.ObservationsDB.Data.Site Site
		{
			get { return SAEON.ObservationsDB.Data.Site.FetchByID(this.SiteID); }
			set { SetColumnValue("SiteID", value.Id); }
		}
		
		
		#endregion
		
		
		
		//no ManyToMany tables defined (0)
		
        
        
		#region ObjectDataSource support
		
		
		/// <summary>
		/// Inserts a record, can be used with the Object Data Source
		/// </summary>
		public static void Insert(Guid varId,Guid varSiteID,Guid varOrganisationID,Guid varOrganisationRoleID,DateTime? varStartDate,DateTime? varEndDate,Guid varUserId,DateTime? varUpdatedAt)
		{
			SiteOrganisation item = new SiteOrganisation();
			
			item.Id = varId;
			
			item.SiteID = varSiteID;
			
			item.OrganisationID = varOrganisationID;
			
			item.OrganisationRoleID = varOrganisationRoleID;
			
			item.StartDate = varStartDate;
			
			item.EndDate = varEndDate;
			
			item.UserId = varUserId;
			
			item.UpdatedAt = varUpdatedAt;
			
		
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		
		/// <summary>
		/// Updates a record, can be used with the Object Data Source
		/// </summary>
		public static void Update(Guid varId,Guid varSiteID,Guid varOrganisationID,Guid varOrganisationRoleID,DateTime? varStartDate,DateTime? varEndDate,Guid varUserId,DateTime? varUpdatedAt)
		{
			SiteOrganisation item = new SiteOrganisation();
			
				item.Id = varId;
			
				item.SiteID = varSiteID;
			
				item.OrganisationID = varOrganisationID;
			
				item.OrganisationRoleID = varOrganisationRoleID;
			
				item.StartDate = varStartDate;
			
				item.EndDate = varEndDate;
			
				item.UserId = varUserId;
			
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
        
        
        
        public static TableSchema.TableColumn SiteIDColumn
        {
            get { return Schema.Columns[1]; }
        }
        
        
        
        public static TableSchema.TableColumn OrganisationIDColumn
        {
            get { return Schema.Columns[2]; }
        }
        
        
        
        public static TableSchema.TableColumn OrganisationRoleIDColumn
        {
            get { return Schema.Columns[3]; }
        }
        
        
        
        public static TableSchema.TableColumn StartDateColumn
        {
            get { return Schema.Columns[4]; }
        }
        
        
        
        public static TableSchema.TableColumn EndDateColumn
        {
            get { return Schema.Columns[5]; }
        }
        
        
        
        public static TableSchema.TableColumn UserIdColumn
        {
            get { return Schema.Columns[6]; }
        }
        
        
        
        public static TableSchema.TableColumn UpdatedAtColumn
        {
            get { return Schema.Columns[7]; }
        }
        
        
        
        #endregion
		#region Columns Struct
		public struct Columns
		{
			 public static string Id = @"ID";
			 public static string SiteID = @"SiteID";
			 public static string OrganisationID = @"OrganisationID";
			 public static string OrganisationRoleID = @"OrganisationRoleID";
			 public static string StartDate = @"StartDate";
			 public static string EndDate = @"EndDate";
			 public static string UserId = @"UserId";
			 public static string UpdatedAt = @"UpdatedAt";
						
		}
		#endregion
		
		#region Update PK Collections
		
        #endregion
    
        #region Deep Save
		
        #endregion
	}
}
