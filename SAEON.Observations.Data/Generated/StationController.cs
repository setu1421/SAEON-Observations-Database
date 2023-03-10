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
    /// Controller class for Station
    /// </summary>
    [System.ComponentModel.DataObject]
    public partial class StationController
    {
        // Preload our schema..
        Station thisSchemaLoad = new Station();
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
        public StationCollection FetchAll()
        {
            StationCollection coll = new StationCollection();
            Query qry = new Query(Station.Schema);
            coll.LoadAndCloseReader(qry.ExecuteReader());
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public StationCollection FetchByID(object Id)
        {
            StationCollection coll = new StationCollection().Where("ID", Id).Load();
            return coll;
        }
		
		[DataObjectMethod(DataObjectMethodType.Select, false)]
        public StationCollection FetchByQuery(Query qry)
        {
            StationCollection coll = new StationCollection();
            coll.LoadAndCloseReader(qry.ExecuteReader()); 
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public bool Delete(object Id)
        {
            return (Station.Delete(Id) == 1);
        }
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Destroy(object Id)
        {
            return (Station.Destroy(Id) == 1);
        }
        
        
    	
	    /// <summary>
	    /// Inserts a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
	    public void Insert(Guid Id,string Code,string Name,string Description,string Url,double? Latitude,double? Longitude,double? Elevation,Guid UserId,Guid SiteID,DateTime? StartDate,DateTime? EndDate,DateTime? AddedAt,DateTime? UpdatedAt,byte[] RowVersion)
	    {
		    Station item = new Station();
		    
            item.Id = Id;
            
            item.Code = Code;
            
            item.Name = Name;
            
            item.Description = Description;
            
            item.Url = Url;
            
            item.Latitude = Latitude;
            
            item.Longitude = Longitude;
            
            item.Elevation = Elevation;
            
            item.UserId = UserId;
            
            item.SiteID = SiteID;
            
            item.StartDate = StartDate;
            
            item.EndDate = EndDate;
            
            item.AddedAt = AddedAt;
            
            item.UpdatedAt = UpdatedAt;
            
            item.RowVersion = RowVersion;
            
	    
		    item.Save(UserName);
	    }
    	
	    /// <summary>
	    /// Updates a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Update, true)]
	    public void Update(Guid Id,string Code,string Name,string Description,string Url,double? Latitude,double? Longitude,double? Elevation,Guid UserId,Guid SiteID,DateTime? StartDate,DateTime? EndDate,DateTime? AddedAt,DateTime? UpdatedAt,byte[] RowVersion)
	    {
		    Station item = new Station();
	        item.MarkOld();
	        item.IsLoaded = true;
		    
			item.Id = Id;
				
			item.Code = Code;
				
			item.Name = Name;
				
			item.Description = Description;
				
			item.Url = Url;
				
			item.Latitude = Latitude;
				
			item.Longitude = Longitude;
				
			item.Elevation = Elevation;
				
			item.UserId = UserId;
				
			item.SiteID = SiteID;
				
			item.StartDate = StartDate;
				
			item.EndDate = EndDate;
				
			item.AddedAt = AddedAt;
				
			item.UpdatedAt = UpdatedAt;
				
			item.RowVersion = RowVersion;
				
	        item.Save(UserName);
	    }
    }
}
