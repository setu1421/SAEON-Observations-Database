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
	/// Strongly-typed collection for the RoleModule class.
	/// </summary>
    [Serializable]
	public partial class RoleModuleCollection : ActiveList<RoleModule, RoleModuleCollection>
	{	   
		public RoleModuleCollection() {}
        
        /// <summary>
		/// Filters an existing collection based on the set criteria. This is an in-memory filter
		/// Thanks to developingchris for this!
        /// </summary>
        /// <returns>RoleModuleCollection</returns>
		public RoleModuleCollection Filter()
        {
            for (int i = this.Count - 1; i > -1; i--)
            {
                RoleModule o = this[i];
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
	/// This is an ActiveRecord class which wraps the RoleModule table.
	/// </summary>
	[Serializable]
	public partial class RoleModule : ActiveRecord<RoleModule>, IActiveRecord
	{
		#region .ctors and Default Settings
		
		public RoleModule()
		{
		  SetSQLProps();
		  InitSetDefaults();
		  MarkNew();
		}
		
		private void InitSetDefaults() { SetDefaults(); }
		
		public RoleModule(bool useDatabaseDefaults)
		{
			SetSQLProps();
			if(useDatabaseDefaults)
				ForceDefaults();
			MarkNew();
		}
        
		public RoleModule(object keyID)
		{
			SetSQLProps();
			InitSetDefaults();
			LoadByKey(keyID);
		}
		 
		public RoleModule(string columnName, object columnValue)
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
				TableSchema.Table schema = new TableSchema.Table("RoleModule", TableType.Table, DataService.GetInstance("SqlDataProvider"));
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
				
				TableSchema.TableColumn colvarRoleId = new TableSchema.TableColumn(schema);
				colvarRoleId.ColumnName = "RoleId";
				colvarRoleId.DataType = DbType.Guid;
				colvarRoleId.MaxLength = 0;
				colvarRoleId.AutoIncrement = false;
				colvarRoleId.IsNullable = false;
				colvarRoleId.IsPrimaryKey = false;
				colvarRoleId.IsForeignKey = true;
				colvarRoleId.IsReadOnly = false;
				colvarRoleId.DefaultSetting = @"";
				
					colvarRoleId.ForeignKeyTableName = "aspnet_Roles";
				schema.Columns.Add(colvarRoleId);
				
				TableSchema.TableColumn colvarModuleID = new TableSchema.TableColumn(schema);
				colvarModuleID.ColumnName = "ModuleID";
				colvarModuleID.DataType = DbType.Guid;
				colvarModuleID.MaxLength = 0;
				colvarModuleID.AutoIncrement = false;
				colvarModuleID.IsNullable = false;
				colvarModuleID.IsPrimaryKey = false;
				colvarModuleID.IsForeignKey = true;
				colvarModuleID.IsReadOnly = false;
				colvarModuleID.DefaultSetting = @"";
				
					colvarModuleID.ForeignKeyTableName = "Module";
				schema.Columns.Add(colvarModuleID);
				
				BaseSchema = schema;
				//add this schema to the provider
				//so we can query it later
				DataService.Providers["SqlDataProvider"].AddSchema("RoleModule",schema);
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
		  
		[XmlAttribute("RoleId")]
		[Bindable(true)]
		public Guid RoleId 
		{
			get { return GetColumnValue<Guid>(Columns.RoleId); }
			set { SetColumnValue(Columns.RoleId, value); }
		}
		  
		[XmlAttribute("ModuleID")]
		[Bindable(true)]
		public Guid ModuleID 
		{
			get { return GetColumnValue<Guid>(Columns.ModuleID); }
			set { SetColumnValue(Columns.ModuleID, value); }
		}
		
		#endregion
		
		
			
		
		#region ForeignKey Properties
		
		/// <summary>
		/// Returns a AspnetRole ActiveRecord object related to this RoleModule
		/// 
		/// </summary>
		public Observations.Data.AspnetRole AspnetRole
		{
			get { return Observations.Data.AspnetRole.FetchByID(this.RoleId); }
			set { SetColumnValue("RoleId", value.RoleId); }
		}
		
		
		/// <summary>
		/// Returns a ModuleX ActiveRecord object related to this RoleModule
		/// 
		/// </summary>
		public Observations.Data.ModuleX ModuleX
		{
			get { return Observations.Data.ModuleX.FetchByID(this.ModuleID); }
			set { SetColumnValue("ModuleID", value.Id); }
		}
		
		
		#endregion
		
		
		
		//no ManyToMany tables defined (0)
		
        
        
		#region ObjectDataSource support
		
		
		/// <summary>
		/// Inserts a record, can be used with the Object Data Source
		/// </summary>
		public static void Insert(Guid varId,Guid varRoleId,Guid varModuleID)
		{
			RoleModule item = new RoleModule();
			
			item.Id = varId;
			
			item.RoleId = varRoleId;
			
			item.ModuleID = varModuleID;
			
		
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		
		/// <summary>
		/// Updates a record, can be used with the Object Data Source
		/// </summary>
		public static void Update(Guid varId,Guid varRoleId,Guid varModuleID)
		{
			RoleModule item = new RoleModule();
			
				item.Id = varId;
			
				item.RoleId = varRoleId;
			
				item.ModuleID = varModuleID;
			
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
        
        
        
        public static TableSchema.TableColumn RoleIdColumn
        {
            get { return Schema.Columns[1]; }
        }
        
        
        
        public static TableSchema.TableColumn ModuleIDColumn
        {
            get { return Schema.Columns[2]; }
        }
        
        
        
        #endregion
		#region Columns Struct
		public struct Columns
		{
			 public static string Id = @"ID";
			 public static string RoleId = @"RoleId";
			 public static string ModuleID = @"ModuleID";
						
		}
		#endregion
		
		#region Update PK Collections
		
        #endregion
    
        #region Deep Save
		
        #endregion
	}
}
