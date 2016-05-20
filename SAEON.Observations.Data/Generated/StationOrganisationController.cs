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
    /// Controller class for Station_Organisation
    /// </summary>
    [System.ComponentModel.DataObject]
    public partial class StationOrganisationController
    {
        // Preload our schema..
        StationOrganisation thisSchemaLoad = new StationOrganisation();
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
        public StationOrganisationCollection FetchAll()
        {
            StationOrganisationCollection coll = new StationOrganisationCollection();
            Query qry = new Query(StationOrganisation.Schema);
            coll.LoadAndCloseReader(qry.ExecuteReader());
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public StationOrganisationCollection FetchByID(object Id)
        {
            StationOrganisationCollection coll = new StationOrganisationCollection().Where("ID", Id).Load();
            return coll;
        }
		
		[DataObjectMethod(DataObjectMethodType.Select, false)]
        public StationOrganisationCollection FetchByQuery(Query qry)
        {
            StationOrganisationCollection coll = new StationOrganisationCollection();
            coll.LoadAndCloseReader(qry.ExecuteReader()); 
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public bool Delete(object Id)
        {
            return (StationOrganisation.Delete(Id) == 1);
        }
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Destroy(object Id)
        {
            return (StationOrganisation.Destroy(Id) == 1);
        }
        
        
    	
	    /// <summary>
	    /// Inserts a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
	    public void Insert(Guid Id,Guid StationID,Guid OrganisationID,Guid OrganisationRoleID,DateTime? StartDate,DateTime? EndDate,Guid UserId,DateTime? AddedAt,DateTime? UpdatedAt)
	    {
		    StationOrganisation item = new StationOrganisation();
		    
            item.Id = Id;
            
            item.StationID = StationID;
            
            item.OrganisationID = OrganisationID;
            
            item.OrganisationRoleID = OrganisationRoleID;
            
            item.StartDate = StartDate;
            
            item.EndDate = EndDate;
            
            item.UserId = UserId;
            
            item.AddedAt = AddedAt;
            
            item.UpdatedAt = UpdatedAt;
            
	    
		    item.Save(UserName);
	    }
    	
	    /// <summary>
	    /// Updates a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Update, true)]
	    public void Update(Guid Id,Guid StationID,Guid OrganisationID,Guid OrganisationRoleID,DateTime? StartDate,DateTime? EndDate,Guid UserId,DateTime? AddedAt,DateTime? UpdatedAt)
	    {
		    StationOrganisation item = new StationOrganisation();
	        item.MarkOld();
	        item.IsLoaded = true;
		    
			item.Id = Id;
				
			item.StationID = StationID;
				
			item.OrganisationID = OrganisationID;
				
			item.OrganisationRoleID = OrganisationRoleID;
				
			item.StartDate = StartDate;
				
			item.EndDate = EndDate;
				
			item.UserId = UserId;
				
			item.AddedAt = AddedAt;
				
			item.UpdatedAt = UpdatedAt;
				
	        item.Save(UserName);
	    }
    }
}