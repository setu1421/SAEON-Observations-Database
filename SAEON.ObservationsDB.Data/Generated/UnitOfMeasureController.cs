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
    /// Controller class for UnitOfMeasure
    /// </summary>
    [System.ComponentModel.DataObject]
    public partial class UnitOfMeasureController
    {
        // Preload our schema..
        UnitOfMeasure thisSchemaLoad = new UnitOfMeasure();
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
        public UnitOfMeasureCollection FetchAll()
        {
            UnitOfMeasureCollection coll = new UnitOfMeasureCollection();
            Query qry = new Query(UnitOfMeasure.Schema);
            coll.LoadAndCloseReader(qry.ExecuteReader());
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public UnitOfMeasureCollection FetchByID(object Id)
        {
            UnitOfMeasureCollection coll = new UnitOfMeasureCollection().Where("ID", Id).Load();
            return coll;
        }
		
		[DataObjectMethod(DataObjectMethodType.Select, false)]
        public UnitOfMeasureCollection FetchByQuery(Query qry)
        {
            UnitOfMeasureCollection coll = new UnitOfMeasureCollection();
            coll.LoadAndCloseReader(qry.ExecuteReader()); 
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public bool Delete(object Id)
        {
            return (UnitOfMeasure.Delete(Id) == 1);
        }
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Destroy(object Id)
        {
            return (UnitOfMeasure.Destroy(Id) == 1);
        }
        
        
    	
	    /// <summary>
	    /// Inserts a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
	    public void Insert(Guid Id,string Code,string Unit,string UnitSymbol,Guid UserId)
	    {
		    UnitOfMeasure item = new UnitOfMeasure();
		    
            item.Id = Id;
            
            item.Code = Code;
            
            item.Unit = Unit;
            
            item.UnitSymbol = UnitSymbol;
            
            item.UserId = UserId;
            
	    
		    item.Save(UserName);
	    }
    	
	    /// <summary>
	    /// Updates a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Update, true)]
	    public void Update(Guid Id,string Code,string Unit,string UnitSymbol,Guid UserId)
	    {
		    UnitOfMeasure item = new UnitOfMeasure();
	        item.MarkOld();
	        item.IsLoaded = true;
		    
			item.Id = Id;
				
			item.Code = Code;
				
			item.Unit = Unit;
				
			item.UnitSymbol = UnitSymbol;
				
			item.UserId = UserId;
				
	        item.Save(UserName);
	    }
    }
}
