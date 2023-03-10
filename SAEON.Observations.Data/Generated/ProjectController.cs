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
    /// Controller class for Project
    /// </summary>
    [System.ComponentModel.DataObject]
    public partial class ProjectController
    {
        // Preload our schema..
        Project thisSchemaLoad = new Project();
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
        public ProjectCollection FetchAll()
        {
            ProjectCollection coll = new ProjectCollection();
            Query qry = new Query(Project.Schema);
            coll.LoadAndCloseReader(qry.ExecuteReader());
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public ProjectCollection FetchByID(object Id)
        {
            ProjectCollection coll = new ProjectCollection().Where("ID", Id).Load();
            return coll;
        }
		
		[DataObjectMethod(DataObjectMethodType.Select, false)]
        public ProjectCollection FetchByQuery(Query qry)
        {
            ProjectCollection coll = new ProjectCollection();
            coll.LoadAndCloseReader(qry.ExecuteReader()); 
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public bool Delete(object Id)
        {
            return (Project.Delete(Id) == 1);
        }
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Destroy(object Id)
        {
            return (Project.Destroy(Id) == 1);
        }
        
        
    	
	    /// <summary>
	    /// Inserts a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
	    public void Insert(Guid Id,Guid ProgrammeID,string Code,string Name,string Description,string Url,DateTime? StartDate,DateTime? EndDate,Guid UserId,DateTime? AddedAt,DateTime? UpdatedAt,byte[] RowVersion)
	    {
		    Project item = new Project();
		    
            item.Id = Id;
            
            item.ProgrammeID = ProgrammeID;
            
            item.Code = Code;
            
            item.Name = Name;
            
            item.Description = Description;
            
            item.Url = Url;
            
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
	    public void Update(Guid Id,Guid ProgrammeID,string Code,string Name,string Description,string Url,DateTime? StartDate,DateTime? EndDate,Guid UserId,DateTime? AddedAt,DateTime? UpdatedAt,byte[] RowVersion)
	    {
		    Project item = new Project();
	        item.MarkOld();
	        item.IsLoaded = true;
		    
			item.Id = Id;
				
			item.ProgrammeID = ProgrammeID;
				
			item.Code = Code;
				
			item.Name = Name;
				
			item.Description = Description;
				
			item.Url = Url;
				
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
