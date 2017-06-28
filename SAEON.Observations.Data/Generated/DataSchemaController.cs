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
    /// Controller class for DataSchema
    /// </summary>
    [System.ComponentModel.DataObject]
    public partial class DataSchemaController
    {
        // Preload our schema..
        DataSchema thisSchemaLoad = new DataSchema();
        private string userName = String.Empty;
        protected string UserName
        {
            get
            {
				if (userName.Length == 0) 
				{
    				if (System.Web.HttpContext.Current != null)
    				{
						userName=System.Web.HttpContext.Current.User.Identity.Name;
					}
					else
					{
						userName=System.Threading.Thread.CurrentPrincipal.Identity.Name;
					}
				}
				return userName;
            }
        }
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public DataSchemaCollection FetchAll()
        {
            DataSchemaCollection coll = new DataSchemaCollection();
            Query qry = new Query(DataSchema.Schema);
            coll.LoadAndCloseReader(qry.ExecuteReader());
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public DataSchemaCollection FetchByID(object Id)
        {
            DataSchemaCollection coll = new DataSchemaCollection().Where("ID", Id).Load();
            return coll;
        }
		
		[DataObjectMethod(DataObjectMethodType.Select, false)]
        public DataSchemaCollection FetchByQuery(Query qry)
        {
            DataSchemaCollection coll = new DataSchemaCollection();
            coll.LoadAndCloseReader(qry.ExecuteReader()); 
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public bool Delete(object Id)
        {
            return (DataSchema.Delete(Id) == 1);
        }
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Destroy(object Id)
        {
            return (DataSchema.Destroy(Id) == 1);
        }
        
        
    	
	    /// <summary>
	    /// Inserts a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
	    public void Insert(Guid Id,string Code,string Name,string Description,Guid DataSourceTypeID,int IgnoreFirst,bool? HasColumnNames,int IgnoreLast,string Condition,string DataSchemaX,Guid UserId,string Delimiter,string SplitSelector,int? SplitIndex,DateTime? AddedAt,DateTime? UpdatedAt,byte[] RowVersion)
	    {
		    DataSchema item = new DataSchema();
		    
            item.Id = Id;
            
            item.Code = Code;
            
            item.Name = Name;
            
            item.Description = Description;
            
            item.DataSourceTypeID = DataSourceTypeID;
            
            item.IgnoreFirst = IgnoreFirst;
            
            item.HasColumnNames = HasColumnNames;
            
            item.IgnoreLast = IgnoreLast;
            
            item.Condition = Condition;
            
            item.DataSchemaX = DataSchemaX;
            
            item.UserId = UserId;
            
            item.Delimiter = Delimiter;
            
            item.SplitSelector = SplitSelector;
            
            item.SplitIndex = SplitIndex;
            
            item.AddedAt = AddedAt;
            
            item.UpdatedAt = UpdatedAt;
            
            item.RowVersion = RowVersion;
            
	    
		    item.Save(UserName);
	    }
    	
	    /// <summary>
	    /// Updates a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Update, true)]
	    public void Update(Guid Id,string Code,string Name,string Description,Guid DataSourceTypeID,int IgnoreFirst,bool? HasColumnNames,int IgnoreLast,string Condition,string DataSchemaX,Guid UserId,string Delimiter,string SplitSelector,int? SplitIndex,DateTime? AddedAt,DateTime? UpdatedAt,byte[] RowVersion)
	    {
		    DataSchema item = new DataSchema();
	        item.MarkOld();
	        item.IsLoaded = true;
		    
			item.Id = Id;
				
			item.Code = Code;
				
			item.Name = Name;
				
			item.Description = Description;
				
			item.DataSourceTypeID = DataSourceTypeID;
				
			item.IgnoreFirst = IgnoreFirst;
				
			item.HasColumnNames = HasColumnNames;
				
			item.IgnoreLast = IgnoreLast;
				
			item.Condition = Condition;
				
			item.DataSchemaX = DataSchemaX;
				
			item.UserId = UserId;
				
			item.Delimiter = Delimiter;
				
			item.SplitSelector = SplitSelector;
				
			item.SplitIndex = SplitIndex;
				
			item.AddedAt = AddedAt;
				
			item.UpdatedAt = UpdatedAt;
				
			item.RowVersion = RowVersion;
				
	        item.Save(UserName);
	    }
    }
}
