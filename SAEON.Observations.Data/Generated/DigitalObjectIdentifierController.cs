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
    /// Controller class for DigitalObjectIdentifiers
    /// </summary>
    [System.ComponentModel.DataObject]
    public partial class DigitalObjectIdentifierController
    {
        // Preload our schema..
        DigitalObjectIdentifier thisSchemaLoad = new DigitalObjectIdentifier();
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
        public DigitalObjectIdentifierCollection FetchAll()
        {
            DigitalObjectIdentifierCollection coll = new DigitalObjectIdentifierCollection();
            Query qry = new Query(DigitalObjectIdentifier.Schema);
            coll.LoadAndCloseReader(qry.ExecuteReader());
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public DigitalObjectIdentifierCollection FetchByID(object Id)
        {
            DigitalObjectIdentifierCollection coll = new DigitalObjectIdentifierCollection().Where("ID", Id).Load();
            return coll;
        }
		
		[DataObjectMethod(DataObjectMethodType.Select, false)]
        public DigitalObjectIdentifierCollection FetchByQuery(Query qry)
        {
            DigitalObjectIdentifierCollection coll = new DigitalObjectIdentifierCollection();
            coll.LoadAndCloseReader(qry.ExecuteReader()); 
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public bool Delete(object Id)
        {
            return (DigitalObjectIdentifier.Delete(Id) == 1);
        }
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Destroy(object Id)
        {
            return (DigitalObjectIdentifier.Destroy(Id) == 1);
        }
        
        
    	
	    /// <summary>
	    /// Inserts a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
	    public void Insert(string Doi,string DOIUrl,string Name,DateTime? AddedAt,string AddedBy,DateTime? UpdatedAt,string UpdatedBy,byte[] RowVersion,Guid? AlternateID,int? ParentID,byte DOIType,string Code,string MetadataJson,byte[] MetadataJsonSha256,string MetadataHtml,string MetadataUrl,string ObjectStoreUrl,string QueryUrl,Guid? ODPMetadataID,bool? ODPMetadataNeedsUpdate,bool? ODPMetadataIsValid,string ODPMetadataErrors,string Title,string CitationHtml,string Description,string DescriptionHtml,string Citation,bool? ODPMetadataIsPublished,string ODPMetadataPublishErrors,Guid? DatasetID)
	    {
		    DigitalObjectIdentifier item = new DigitalObjectIdentifier();
		    
            item.Doi = Doi;
            
            item.DOIUrl = DOIUrl;
            
            item.Name = Name;
            
            item.AddedAt = AddedAt;
            
            item.AddedBy = AddedBy;
            
            item.UpdatedAt = UpdatedAt;
            
            item.UpdatedBy = UpdatedBy;
            
            item.RowVersion = RowVersion;
            
            item.AlternateID = AlternateID;
            
            item.ParentID = ParentID;
            
            item.DOIType = DOIType;
            
            item.Code = Code;
            
            item.MetadataJson = MetadataJson;
            
            item.MetadataJsonSha256 = MetadataJsonSha256;
            
            item.MetadataHtml = MetadataHtml;
            
            item.MetadataUrl = MetadataUrl;
            
            item.ObjectStoreUrl = ObjectStoreUrl;
            
            item.QueryUrl = QueryUrl;
            
            item.ODPMetadataID = ODPMetadataID;
            
            item.ODPMetadataNeedsUpdate = ODPMetadataNeedsUpdate;
            
            item.ODPMetadataIsValid = ODPMetadataIsValid;
            
            item.ODPMetadataErrors = ODPMetadataErrors;
            
            item.Title = Title;
            
            item.CitationHtml = CitationHtml;
            
            item.Description = Description;
            
            item.DescriptionHtml = DescriptionHtml;
            
            item.Citation = Citation;
            
            item.ODPMetadataIsPublished = ODPMetadataIsPublished;
            
            item.ODPMetadataPublishErrors = ODPMetadataPublishErrors;
            
            item.DatasetID = DatasetID;
            
	    
		    item.Save(UserName);
	    }
    	
	    /// <summary>
	    /// Updates a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Update, true)]
	    public void Update(int Id,string Doi,string DOIUrl,string Name,DateTime? AddedAt,string AddedBy,DateTime? UpdatedAt,string UpdatedBy,byte[] RowVersion,Guid? AlternateID,int? ParentID,byte DOIType,string Code,string MetadataJson,byte[] MetadataJsonSha256,string MetadataHtml,string MetadataUrl,string ObjectStoreUrl,string QueryUrl,Guid? ODPMetadataID,bool? ODPMetadataNeedsUpdate,bool? ODPMetadataIsValid,string ODPMetadataErrors,string Title,string CitationHtml,string Description,string DescriptionHtml,string Citation,bool? ODPMetadataIsPublished,string ODPMetadataPublishErrors,Guid? DatasetID)
	    {
		    DigitalObjectIdentifier item = new DigitalObjectIdentifier();
	        item.MarkOld();
	        item.IsLoaded = true;
		    
			item.Id = Id;
				
			item.Doi = Doi;
				
			item.DOIUrl = DOIUrl;
				
			item.Name = Name;
				
			item.AddedAt = AddedAt;
				
			item.AddedBy = AddedBy;
				
			item.UpdatedAt = UpdatedAt;
				
			item.UpdatedBy = UpdatedBy;
				
			item.RowVersion = RowVersion;
				
			item.AlternateID = AlternateID;
				
			item.ParentID = ParentID;
				
			item.DOIType = DOIType;
				
			item.Code = Code;
				
			item.MetadataJson = MetadataJson;
				
			item.MetadataJsonSha256 = MetadataJsonSha256;
				
			item.MetadataHtml = MetadataHtml;
				
			item.MetadataUrl = MetadataUrl;
				
			item.ObjectStoreUrl = ObjectStoreUrl;
				
			item.QueryUrl = QueryUrl;
				
			item.ODPMetadataID = ODPMetadataID;
				
			item.ODPMetadataNeedsUpdate = ODPMetadataNeedsUpdate;
				
			item.ODPMetadataIsValid = ODPMetadataIsValid;
				
			item.ODPMetadataErrors = ODPMetadataErrors;
				
			item.Title = Title;
				
			item.CitationHtml = CitationHtml;
				
			item.Description = Description;
				
			item.DescriptionHtml = DescriptionHtml;
				
			item.Citation = Citation;
				
			item.ODPMetadataIsPublished = ODPMetadataIsPublished;
				
			item.ODPMetadataPublishErrors = ODPMetadataPublishErrors;
				
			item.DatasetID = DatasetID;
				
	        item.Save(UserName);
	    }
    }
}
