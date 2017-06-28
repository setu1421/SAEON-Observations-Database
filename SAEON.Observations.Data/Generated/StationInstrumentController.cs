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
    /// Controller class for Station_Instrument
    /// </summary>
    [System.ComponentModel.DataObject]
    public partial class StationInstrumentController
    {
        // Preload our schema..
        StationInstrument thisSchemaLoad = new StationInstrument();
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
        public StationInstrumentCollection FetchAll()
        {
            StationInstrumentCollection coll = new StationInstrumentCollection();
            Query qry = new Query(StationInstrument.Schema);
            coll.LoadAndCloseReader(qry.ExecuteReader());
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public StationInstrumentCollection FetchByID(object Id)
        {
            StationInstrumentCollection coll = new StationInstrumentCollection().Where("ID", Id).Load();
            return coll;
        }
		
		[DataObjectMethod(DataObjectMethodType.Select, false)]
        public StationInstrumentCollection FetchByQuery(Query qry)
        {
            StationInstrumentCollection coll = new StationInstrumentCollection();
            coll.LoadAndCloseReader(qry.ExecuteReader()); 
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public bool Delete(object Id)
        {
            return (StationInstrument.Delete(Id) == 1);
        }
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Destroy(object Id)
        {
            return (StationInstrument.Destroy(Id) == 1);
        }
        
        
    	
	    /// <summary>
	    /// Inserts a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
	    public void Insert(Guid Id,Guid StationID,Guid InstrumentID,DateTime? StartDate,DateTime? EndDate,double? Latitude,double? Longitude,double? Elevation,Guid UserId,DateTime? AddedAt,DateTime? UpdatedAt,byte[] RowVersion)
	    {
		    StationInstrument item = new StationInstrument();
		    
            item.Id = Id;
            
            item.StationID = StationID;
            
            item.InstrumentID = InstrumentID;
            
            item.StartDate = StartDate;
            
            item.EndDate = EndDate;
            
            item.Latitude = Latitude;
            
            item.Longitude = Longitude;
            
            item.Elevation = Elevation;
            
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
	    public void Update(Guid Id,Guid StationID,Guid InstrumentID,DateTime? StartDate,DateTime? EndDate,double? Latitude,double? Longitude,double? Elevation,Guid UserId,DateTime? AddedAt,DateTime? UpdatedAt,byte[] RowVersion)
	    {
		    StationInstrument item = new StationInstrument();
	        item.MarkOld();
	        item.IsLoaded = true;
		    
			item.Id = Id;
				
			item.StationID = StationID;
				
			item.InstrumentID = InstrumentID;
				
			item.StartDate = StartDate;
				
			item.EndDate = EndDate;
				
			item.Latitude = Latitude;
				
			item.Longitude = Longitude;
				
			item.Elevation = Elevation;
				
			item.UserId = UserId;
				
			item.AddedAt = AddedAt;
				
			item.UpdatedAt = UpdatedAt;
				
			item.RowVersion = RowVersion;
				
	        item.Save(UserName);
	    }
    }
}
