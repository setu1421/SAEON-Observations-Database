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
    /// Controller class for ImportBatch
    /// </summary>
    [System.ComponentModel.DataObject]
    public partial class ImportBatchController
    {
        // Preload our schema..
        ImportBatch thisSchemaLoad = new ImportBatch();
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
        public ImportBatchCollection FetchAll()
        {
            ImportBatchCollection coll = new ImportBatchCollection();
            Query qry = new Query(ImportBatch.Schema);
            coll.LoadAndCloseReader(qry.ExecuteReader());
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public ImportBatchCollection FetchByID(object Id)
        {
            ImportBatchCollection coll = new ImportBatchCollection().Where("ID", Id).Load();
            return coll;
        }
		
		[DataObjectMethod(DataObjectMethodType.Select, false)]
        public ImportBatchCollection FetchByQuery(Query qry)
        {
            ImportBatchCollection coll = new ImportBatchCollection();
            coll.LoadAndCloseReader(qry.ExecuteReader()); 
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public bool Delete(object Id)
        {
            return (ImportBatch.Delete(Id) == 1);
        }
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Destroy(object Id)
        {
            return (ImportBatch.Destroy(Id) == 1);
        }
        
        
    	
	    /// <summary>
	    /// Inserts a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
	    public void Insert(Guid Guid,Guid DataSourceID,DateTime ImportDate,int Status,Guid UserId,string FileName,string LogFileName)
	    {
		    ImportBatch item = new ImportBatch();
		    
            item.Guid = Guid;
            
            item.DataSourceID = DataSourceID;
            
            item.ImportDate = ImportDate;
            
            item.Status = Status;
            
            item.UserId = UserId;
            
            item.FileName = FileName;
            
            item.LogFileName = LogFileName;
            
	    
		    item.Save(UserName);
	    }
    	
	    /// <summary>
	    /// Updates a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Update, true)]
	    public void Update(int Id,Guid Guid,Guid DataSourceID,DateTime ImportDate,int Status,Guid UserId,string FileName,string LogFileName)
	    {
		    ImportBatch item = new ImportBatch();
	        item.MarkOld();
	        item.IsLoaded = true;
		    
			item.Id = Id;
				
			item.Guid = Guid;
				
			item.DataSourceID = DataSourceID;
				
			item.ImportDate = ImportDate;
				
			item.Status = Status;
				
			item.UserId = UserId;
				
			item.FileName = FileName;
				
			item.LogFileName = LogFileName;
				
	        item.Save(UserName);
	    }
    }
}
