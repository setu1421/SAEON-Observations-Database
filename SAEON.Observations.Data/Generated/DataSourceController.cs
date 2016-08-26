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
    /// Controller class for DataSource
    /// </summary>
    [System.ComponentModel.DataObject]
    public partial class DataSourceController
    {
        // Preload our schema..
        DataSource thisSchemaLoad = new DataSource();
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
        public DataSourceCollection FetchAll()
        {
            DataSourceCollection coll = new DataSourceCollection();
            Query qry = new Query(DataSource.Schema);
            coll.LoadAndCloseReader(qry.ExecuteReader());
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public DataSourceCollection FetchByID(object Id)
        {
            DataSourceCollection coll = new DataSourceCollection().Where("ID", Id).Load();
            return coll;
        }
		
		[DataObjectMethod(DataObjectMethodType.Select, false)]
        public DataSourceCollection FetchByQuery(Query qry)
        {
            DataSourceCollection coll = new DataSourceCollection();
            coll.LoadAndCloseReader(qry.ExecuteReader()); 
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public bool Delete(object Id)
        {
            return (DataSource.Delete(Id) == 1);
        }
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Destroy(object Id)
        {
            return (DataSource.Destroy(Id) == 1);
        }
        
        
    	
	    /// <summary>
	    /// Inserts a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
	    public void Insert(Guid Id,string Code,string Name,string Description,string Url,double? DefaultNullValue,double? ErrorEstimate,int UpdateFreq,DateTime? StartDate,DateTime? EndDate,DateTime LastUpdate,Guid? DataSchemaID,Guid UserId,DateTime? AddedAt,DateTime? UpdatedAt)
	    {
		    DataSource item = new DataSource();
		    
            item.Id = Id;
            
            item.Code = Code;
            
            item.Name = Name;
            
            item.Description = Description;
            
            item.Url = Url;
            
            item.DefaultNullValue = DefaultNullValue;
            
            item.ErrorEstimate = ErrorEstimate;
            
            item.UpdateFreq = UpdateFreq;
            
            item.StartDate = StartDate;
            
            item.EndDate = EndDate;
            
            item.LastUpdate = LastUpdate;
            
            item.DataSchemaID = DataSchemaID;
            
            item.UserId = UserId;
            
            item.AddedAt = AddedAt;
            
            item.UpdatedAt = UpdatedAt;
            
	    
		    item.Save(UserName);
	    }
    	
	    /// <summary>
	    /// Updates a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Update, true)]
	    public void Update(Guid Id,string Code,string Name,string Description,string Url,double? DefaultNullValue,double? ErrorEstimate,int UpdateFreq,DateTime? StartDate,DateTime? EndDate,DateTime LastUpdate,Guid? DataSchemaID,Guid UserId,DateTime? AddedAt,DateTime? UpdatedAt)
	    {
		    DataSource item = new DataSource();
	        item.MarkOld();
	        item.IsLoaded = true;
		    
			item.Id = Id;
				
			item.Code = Code;
				
			item.Name = Name;
				
			item.Description = Description;
				
			item.Url = Url;
				
			item.DefaultNullValue = DefaultNullValue;
				
			item.ErrorEstimate = ErrorEstimate;
				
			item.UpdateFreq = UpdateFreq;
				
			item.StartDate = StartDate;
				
			item.EndDate = EndDate;
				
			item.LastUpdate = LastUpdate;
				
			item.DataSchemaID = DataSchemaID;
				
			item.UserId = UserId;
				
			item.AddedAt = AddedAt;
				
			item.UpdatedAt = UpdatedAt;
				
	        item.Save(UserName);
	    }
    }
}
