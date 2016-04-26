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
    /// Controller class for DataSource_Organisation
    /// </summary>
    [System.ComponentModel.DataObject]
    public partial class DataSourceOrganisationController
    {
        // Preload our schema..
        DataSourceOrganisation thisSchemaLoad = new DataSourceOrganisation();
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
        public DataSourceOrganisationCollection FetchAll()
        {
            DataSourceOrganisationCollection coll = new DataSourceOrganisationCollection();
            Query qry = new Query(DataSourceOrganisation.Schema);
            coll.LoadAndCloseReader(qry.ExecuteReader());
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public DataSourceOrganisationCollection FetchByID(object Id)
        {
            DataSourceOrganisationCollection coll = new DataSourceOrganisationCollection().Where("ID", Id).Load();
            return coll;
        }
		
		[DataObjectMethod(DataObjectMethodType.Select, false)]
        public DataSourceOrganisationCollection FetchByQuery(Query qry)
        {
            DataSourceOrganisationCollection coll = new DataSourceOrganisationCollection();
            coll.LoadAndCloseReader(qry.ExecuteReader()); 
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public bool Delete(object Id)
        {
            return (DataSourceOrganisation.Delete(Id) == 1);
        }
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Destroy(object Id)
        {
            return (DataSourceOrganisation.Destroy(Id) == 1);
        }
        
        
    	
	    /// <summary>
	    /// Inserts a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
	    public void Insert(Guid Id,Guid DataSourceID,Guid OrganisationID,Guid OrganisationRoleID,DateTime? StartDate,DateTime? EndDate,Guid UserId,DateTime? UpdatedAt)
	    {
		    DataSourceOrganisation item = new DataSourceOrganisation();
		    
            item.Id = Id;
            
            item.DataSourceID = DataSourceID;
            
            item.OrganisationID = OrganisationID;
            
            item.OrganisationRoleID = OrganisationRoleID;
            
            item.StartDate = StartDate;
            
            item.EndDate = EndDate;
            
            item.UserId = UserId;
            
            item.UpdatedAt = UpdatedAt;
            
	    
		    item.Save(UserName);
	    }
    	
	    /// <summary>
	    /// Updates a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Update, true)]
	    public void Update(Guid Id,Guid DataSourceID,Guid OrganisationID,Guid OrganisationRoleID,DateTime? StartDate,DateTime? EndDate,Guid UserId,DateTime? UpdatedAt)
	    {
		    DataSourceOrganisation item = new DataSourceOrganisation();
	        item.MarkOld();
	        item.IsLoaded = true;
		    
			item.Id = Id;
				
			item.DataSourceID = DataSourceID;
				
			item.OrganisationID = OrganisationID;
				
			item.OrganisationRoleID = OrganisationRoleID;
				
			item.StartDate = StartDate;
				
			item.EndDate = EndDate;
				
			item.UserId = UserId;
				
			item.UpdatedAt = UpdatedAt;
				
	        item.Save(UserName);
	    }
    }
}
