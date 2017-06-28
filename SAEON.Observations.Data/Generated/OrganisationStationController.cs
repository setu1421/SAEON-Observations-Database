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
    /// Controller class for Organisation_Station
    /// </summary>
    [System.ComponentModel.DataObject]
    public partial class OrganisationStationController
    {
        // Preload our schema..
        OrganisationStation thisSchemaLoad = new OrganisationStation();
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
        public OrganisationStationCollection FetchAll()
        {
            OrganisationStationCollection coll = new OrganisationStationCollection();
            Query qry = new Query(OrganisationStation.Schema);
            coll.LoadAndCloseReader(qry.ExecuteReader());
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public OrganisationStationCollection FetchByID(object Id)
        {
            OrganisationStationCollection coll = new OrganisationStationCollection().Where("ID", Id).Load();
            return coll;
        }
		
		[DataObjectMethod(DataObjectMethodType.Select, false)]
        public OrganisationStationCollection FetchByQuery(Query qry)
        {
            OrganisationStationCollection coll = new OrganisationStationCollection();
            coll.LoadAndCloseReader(qry.ExecuteReader()); 
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public bool Delete(object Id)
        {
            return (OrganisationStation.Delete(Id) == 1);
        }
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Destroy(object Id)
        {
            return (OrganisationStation.Destroy(Id) == 1);
        }
        
        
    	
	    /// <summary>
	    /// Inserts a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
	    public void Insert(Guid Id,Guid OrganisationID,Guid StationID,Guid OrganisationRoleID,DateTime? StartDate,DateTime? EndDate,Guid UserId,DateTime? AddedAt,DateTime? UpdatedAt,byte[] RowVersion)
	    {
		    OrganisationStation item = new OrganisationStation();
		    
            item.Id = Id;
            
            item.OrganisationID = OrganisationID;
            
            item.StationID = StationID;
            
            item.OrganisationRoleID = OrganisationRoleID;
            
            item.StartDate = StartDate;
            
            item.EndDate = EndDate;
            
            item.UserId = UserId;
            
            item.AddedAt = AddedAt;
            
            item.UpdatedAt = UpdatedAt;
            
            item.RowVersion = RowVersion;
            
	    
		    item.Save(UserName);
	    }
    	
	    /// <summary>
	    /// Updates a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Update, true)]
	    public void Update(Guid Id,Guid OrganisationID,Guid StationID,Guid OrganisationRoleID,DateTime? StartDate,DateTime? EndDate,Guid UserId,DateTime? AddedAt,DateTime? UpdatedAt,byte[] RowVersion)
	    {
		    OrganisationStation item = new OrganisationStation();
	        item.MarkOld();
	        item.IsLoaded = true;
		    
			item.Id = Id;
				
			item.OrganisationID = OrganisationID;
				
			item.StationID = StationID;
				
			item.OrganisationRoleID = OrganisationRoleID;
				
			item.StartDate = StartDate;
				
			item.EndDate = EndDate;
				
			item.UserId = UserId;
				
			item.AddedAt = AddedAt;
				
			item.UpdatedAt = UpdatedAt;
				
			item.RowVersion = RowVersion;
				
	        item.Save(UserName);
	    }
    }
}
