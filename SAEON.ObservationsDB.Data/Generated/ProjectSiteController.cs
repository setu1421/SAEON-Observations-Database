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
    /// Controller class for ProjectSite
    /// </summary>
    [System.ComponentModel.DataObject]
    public partial class ProjectSiteController
    {
        // Preload our schema..
        ProjectSite thisSchemaLoad = new ProjectSite();
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
        public ProjectSiteCollection FetchAll()
        {
            ProjectSiteCollection coll = new ProjectSiteCollection();
            Query qry = new Query(ProjectSite.Schema);
            coll.LoadAndCloseReader(qry.ExecuteReader());
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public ProjectSiteCollection FetchByID(object Id)
        {
            ProjectSiteCollection coll = new ProjectSiteCollection().Where("ID", Id).Load();
            return coll;
        }
		
		[DataObjectMethod(DataObjectMethodType.Select, false)]
        public ProjectSiteCollection FetchByQuery(Query qry)
        {
            ProjectSiteCollection coll = new ProjectSiteCollection();
            coll.LoadAndCloseReader(qry.ExecuteReader()); 
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public bool Delete(object Id)
        {
            return (ProjectSite.Delete(Id) == 1);
        }
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Destroy(object Id)
        {
            return (ProjectSite.Destroy(Id) == 1);
        }
        
        
    	
	    /// <summary>
	    /// Inserts a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
	    public void Insert(Guid Id,string Code,string Name,string Description,Guid OrganisationID,Guid UserId)
	    {
		    ProjectSite item = new ProjectSite();
		    
            item.Id = Id;
            
            item.Code = Code;
            
            item.Name = Name;
            
            item.Description = Description;
            
            item.OrganisationID = OrganisationID;
            
            item.UserId = UserId;
            
	    
		    item.Save(UserName);
	    }
    	
	    /// <summary>
	    /// Updates a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Update, true)]
	    public void Update(Guid Id,string Code,string Name,string Description,Guid OrganisationID,Guid UserId)
	    {
		    ProjectSite item = new ProjectSite();
	        item.MarkOld();
	        item.IsLoaded = true;
		    
			item.Id = Id;
				
			item.Code = Code;
				
			item.Name = Name;
				
			item.Description = Description;
				
			item.OrganisationID = OrganisationID;
				
			item.UserId = UserId;
				
	        item.Save(UserName);
	    }
    }
}
