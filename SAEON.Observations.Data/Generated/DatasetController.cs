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
    /// Controller class for Datasets
    /// </summary>
    [System.ComponentModel.DataObject]
    public partial class DatasetController
    {
        // Preload our schema..
        Dataset thisSchemaLoad = new Dataset();
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
        public DatasetCollection FetchAll()
        {
            DatasetCollection coll = new DatasetCollection();
            Query qry = new Query(Dataset.Schema);
            coll.LoadAndCloseReader(qry.ExecuteReader());
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public DatasetCollection FetchByID(object Id)
        {
            DatasetCollection coll = new DatasetCollection().Where("ID", Id).Load();
            return coll;
        }
		
		[DataObjectMethod(DataObjectMethodType.Select, false)]
        public DatasetCollection FetchByQuery(Query qry)
        {
            DatasetCollection coll = new DatasetCollection();
            coll.LoadAndCloseReader(qry.ExecuteReader()); 
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public bool Delete(object Id)
        {
            return (Dataset.Delete(Id) == 1);
        }
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Destroy(object Id)
        {
            return (Dataset.Destroy(Id) == 1);
        }
        
        
    	
	    /// <summary>
	    /// Inserts a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
	    public void Insert(Guid Id,string Code,string Name,string Description,string Title,Guid StationID,Guid PhenomenonOfferingID,Guid PhenomenonUOMID,int? DigitalObjectIdentifierID,int? Count,int? ValueCount,int? NullCount,int? VerifiedCount,int? UnverifiedCount,DateTime? StartDate,DateTime? EndDate,double? LatitudeNorth,double? LatitudeSouth,double? LongitudeWest,double? LongitudeEast,double? ElevationMinimum,double? ElevationMaximum,int HashCode,bool? NeedsUpdate,DateTime? AddedAt,string AddedBy,DateTime? UpdatedAt,string UpdatedBy,Guid UserId,byte[] RowVersion,string CSVFileName,string ExcelFileName,string NetCDFFileName)
	    {
		    Dataset item = new Dataset();
		    
            item.Id = Id;
            
            item.Code = Code;
            
            item.Name = Name;
            
            item.Description = Description;
            
            item.Title = Title;
            
            item.StationID = StationID;
            
            item.PhenomenonOfferingID = PhenomenonOfferingID;
            
            item.PhenomenonUOMID = PhenomenonUOMID;
            
            item.DigitalObjectIdentifierID = DigitalObjectIdentifierID;
            
            item.Count = Count;
            
            item.ValueCount = ValueCount;
            
            item.NullCount = NullCount;
            
            item.VerifiedCount = VerifiedCount;
            
            item.UnverifiedCount = UnverifiedCount;
            
            item.StartDate = StartDate;
            
            item.EndDate = EndDate;
            
            item.LatitudeNorth = LatitudeNorth;
            
            item.LatitudeSouth = LatitudeSouth;
            
            item.LongitudeWest = LongitudeWest;
            
            item.LongitudeEast = LongitudeEast;
            
            item.ElevationMinimum = ElevationMinimum;
            
            item.ElevationMaximum = ElevationMaximum;
            
            item.HashCode = HashCode;
            
            item.NeedsUpdate = NeedsUpdate;
            
            item.AddedAt = AddedAt;
            
            item.AddedBy = AddedBy;
            
            item.UpdatedAt = UpdatedAt;
            
            item.UpdatedBy = UpdatedBy;
            
            item.UserId = UserId;
            
            item.RowVersion = RowVersion;
            
            item.CSVFileName = CSVFileName;
            
            item.ExcelFileName = ExcelFileName;
            
            item.NetCDFFileName = NetCDFFileName;
            
	    
		    item.Save(UserName);
	    }
    	
	    /// <summary>
	    /// Updates a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Update, true)]
	    public void Update(Guid Id,string Code,string Name,string Description,string Title,Guid StationID,Guid PhenomenonOfferingID,Guid PhenomenonUOMID,int? DigitalObjectIdentifierID,int? Count,int? ValueCount,int? NullCount,int? VerifiedCount,int? UnverifiedCount,DateTime? StartDate,DateTime? EndDate,double? LatitudeNorth,double? LatitudeSouth,double? LongitudeWest,double? LongitudeEast,double? ElevationMinimum,double? ElevationMaximum,int HashCode,bool? NeedsUpdate,DateTime? AddedAt,string AddedBy,DateTime? UpdatedAt,string UpdatedBy,Guid UserId,byte[] RowVersion,string CSVFileName,string ExcelFileName,string NetCDFFileName)
	    {
		    Dataset item = new Dataset();
	        item.MarkOld();
	        item.IsLoaded = true;
		    
			item.Id = Id;
				
			item.Code = Code;
				
			item.Name = Name;
				
			item.Description = Description;
				
			item.Title = Title;
				
			item.StationID = StationID;
				
			item.PhenomenonOfferingID = PhenomenonOfferingID;
				
			item.PhenomenonUOMID = PhenomenonUOMID;
				
			item.DigitalObjectIdentifierID = DigitalObjectIdentifierID;
				
			item.Count = Count;
				
			item.ValueCount = ValueCount;
				
			item.NullCount = NullCount;
				
			item.VerifiedCount = VerifiedCount;
				
			item.UnverifiedCount = UnverifiedCount;
				
			item.StartDate = StartDate;
				
			item.EndDate = EndDate;
				
			item.LatitudeNorth = LatitudeNorth;
				
			item.LatitudeSouth = LatitudeSouth;
				
			item.LongitudeWest = LongitudeWest;
				
			item.LongitudeEast = LongitudeEast;
				
			item.ElevationMinimum = ElevationMinimum;
				
			item.ElevationMaximum = ElevationMaximum;
				
			item.HashCode = HashCode;
				
			item.NeedsUpdate = NeedsUpdate;
				
			item.AddedAt = AddedAt;
				
			item.AddedBy = AddedBy;
				
			item.UpdatedAt = UpdatedAt;
				
			item.UpdatedBy = UpdatedBy;
				
			item.UserId = UserId;
				
			item.RowVersion = RowVersion;
				
			item.CSVFileName = CSVFileName;
				
			item.ExcelFileName = ExcelFileName;
				
			item.NetCDFFileName = NetCDFFileName;
				
	        item.Save(UserName);
	    }
    }
}
