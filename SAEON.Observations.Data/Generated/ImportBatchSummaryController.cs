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
    /// Controller class for ImportBatchSummary
    /// </summary>
    [System.ComponentModel.DataObject]
    public partial class ImportBatchSummaryController
    {
        // Preload our schema..
        ImportBatchSummary thisSchemaLoad = new ImportBatchSummary();
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
        public ImportBatchSummaryCollection FetchAll()
        {
            ImportBatchSummaryCollection coll = new ImportBatchSummaryCollection();
            Query qry = new Query(ImportBatchSummary.Schema);
            coll.LoadAndCloseReader(qry.ExecuteReader());
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public ImportBatchSummaryCollection FetchByID(object Id)
        {
            ImportBatchSummaryCollection coll = new ImportBatchSummaryCollection().Where("ID", Id).Load();
            return coll;
        }
		
		[DataObjectMethod(DataObjectMethodType.Select, false)]
        public ImportBatchSummaryCollection FetchByQuery(Query qry)
        {
            ImportBatchSummaryCollection coll = new ImportBatchSummaryCollection();
            coll.LoadAndCloseReader(qry.ExecuteReader()); 
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public bool Delete(object Id)
        {
            return (ImportBatchSummary.Delete(Id) == 1);
        }
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Destroy(object Id)
        {
            return (ImportBatchSummary.Destroy(Id) == 1);
        }
        
        
    	
	    /// <summary>
	    /// Inserts a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
	    public void Insert(Guid Id,Guid ImportBatchID,Guid SensorID,Guid InstrumentID,Guid StationID,Guid SiteID,Guid PhenomenonOfferingID,Guid PhenomenonUOMID,int Count,double? Minimum,double? Maximum,double? Average,double? StandardDeviation,double? Variance,double? LatitudeNorth,double? LatitudeSouth,double? LongitudeWest,double? LongitudeEast,double? ElevationMinimum,double? ElevationMaximum,DateTime? StartDate,DateTime? EndDate,int? VerifiedCount)
	    {
		    ImportBatchSummary item = new ImportBatchSummary();
		    
            item.Id = Id;
            
            item.ImportBatchID = ImportBatchID;
            
            item.SensorID = SensorID;
            
            item.InstrumentID = InstrumentID;
            
            item.StationID = StationID;
            
            item.SiteID = SiteID;
            
            item.PhenomenonOfferingID = PhenomenonOfferingID;
            
            item.PhenomenonUOMID = PhenomenonUOMID;
            
            item.Count = Count;
            
            item.Minimum = Minimum;
            
            item.Maximum = Maximum;
            
            item.Average = Average;
            
            item.StandardDeviation = StandardDeviation;
            
            item.Variance = Variance;
            
            item.LatitudeNorth = LatitudeNorth;
            
            item.LatitudeSouth = LatitudeSouth;
            
            item.LongitudeWest = LongitudeWest;
            
            item.LongitudeEast = LongitudeEast;
            
            item.ElevationMinimum = ElevationMinimum;
            
            item.ElevationMaximum = ElevationMaximum;
            
            item.StartDate = StartDate;
            
            item.EndDate = EndDate;
            
            item.VerifiedCount = VerifiedCount;
            
	    
		    item.Save(UserName);
	    }
    	
	    /// <summary>
	    /// Updates a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Update, true)]
	    public void Update(Guid Id,Guid ImportBatchID,Guid SensorID,Guid InstrumentID,Guid StationID,Guid SiteID,Guid PhenomenonOfferingID,Guid PhenomenonUOMID,int Count,double? Minimum,double? Maximum,double? Average,double? StandardDeviation,double? Variance,double? LatitudeNorth,double? LatitudeSouth,double? LongitudeWest,double? LongitudeEast,double? ElevationMinimum,double? ElevationMaximum,DateTime? StartDate,DateTime? EndDate,int? VerifiedCount)
	    {
		    ImportBatchSummary item = new ImportBatchSummary();
	        item.MarkOld();
	        item.IsLoaded = true;
		    
			item.Id = Id;
				
			item.ImportBatchID = ImportBatchID;
				
			item.SensorID = SensorID;
				
			item.InstrumentID = InstrumentID;
				
			item.StationID = StationID;
				
			item.SiteID = SiteID;
				
			item.PhenomenonOfferingID = PhenomenonOfferingID;
				
			item.PhenomenonUOMID = PhenomenonUOMID;
				
			item.Count = Count;
				
			item.Minimum = Minimum;
				
			item.Maximum = Maximum;
				
			item.Average = Average;
				
			item.StandardDeviation = StandardDeviation;
				
			item.Variance = Variance;
				
			item.LatitudeNorth = LatitudeNorth;
				
			item.LatitudeSouth = LatitudeSouth;
				
			item.LongitudeWest = LongitudeWest;
				
			item.LongitudeEast = LongitudeEast;
				
			item.ElevationMinimum = ElevationMinimum;
				
			item.ElevationMaximum = ElevationMaximum;
				
			item.StartDate = StartDate;
				
			item.EndDate = EndDate;
				
			item.VerifiedCount = VerifiedCount;
				
	        item.Save(UserName);
	    }
    }
}