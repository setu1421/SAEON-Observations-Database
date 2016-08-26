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
    /// Controller class for OrganisationRole
    /// </summary>
    [System.ComponentModel.DataObject]
    public partial class OrganisationRoleController
    {
        // Preload our schema..
        OrganisationRole thisSchemaLoad = new OrganisationRole();
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
        public OrganisationRoleCollection FetchAll()
        {
            OrganisationRoleCollection coll = new OrganisationRoleCollection();
            Query qry = new Query(OrganisationRole.Schema);
            coll.LoadAndCloseReader(qry.ExecuteReader());
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public OrganisationRoleCollection FetchByID(object Id)
        {
            OrganisationRoleCollection coll = new OrganisationRoleCollection().Where("ID", Id).Load();
            return coll;
        }
		
		[DataObjectMethod(DataObjectMethodType.Select, false)]
        public OrganisationRoleCollection FetchByQuery(Query qry)
        {
            OrganisationRoleCollection coll = new OrganisationRoleCollection();
            coll.LoadAndCloseReader(qry.ExecuteReader()); 
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public bool Delete(object Id)
        {
            return (OrganisationRole.Delete(Id) == 1);
        }
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Destroy(object Id)
        {
            return (OrganisationRole.Destroy(Id) == 1);
        }
        
        
    	
	    /// <summary>
	    /// Inserts a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
	    public void Insert(Guid Id,string Code,string Name,string Description,Guid UserId,DateTime? AddedAt,DateTime? UpdatedAt)
	    {
		    OrganisationRole item = new OrganisationRole();
		    
            item.Id = Id;
            
            item.Code = Code;
            
            item.Name = Name;
            
            item.Description = Description;
            
            item.UserId = UserId;
            
            item.AddedAt = AddedAt;
            
            item.UpdatedAt = UpdatedAt;
            
	    
		    item.Save(UserName);
	    }
    	
	    /// <summary>
	    /// Updates a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Update, true)]
	    public void Update(Guid Id,string Code,string Name,string Description,Guid UserId,DateTime? AddedAt,DateTime? UpdatedAt)
	    {
		    OrganisationRole item = new OrganisationRole();
	        item.MarkOld();
	        item.IsLoaded = true;
		    
			item.Id = Id;
				
			item.Code = Code;
				
			item.Name = Name;
				
			item.Description = Description;
				
			item.UserId = UserId;
				
			item.AddedAt = AddedAt;
				
			item.UpdatedAt = UpdatedAt;
				
	        item.Save(UserName);
	    }
    }
}
