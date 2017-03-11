using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.OData;
using System.Web.OData.Routing;

namespace SAEON.Observations.WebAPI.Controllers
{
    /// <summary>
    /// Organisations
    /// </summary>
    [ODataRoutePrefix("Organisations")]
    public class OrganisationsODataController : BaseODataController<Organisation>
    {
        /// <summary>
        /// All Organisations
        /// </summary>
        /// <returns>ListOf(Organisation)</returns>
        [EnableQuery, ODataRoute]
        public override IQueryable<Organisation> GetAll()
        {
            return base.GetAll();
        }

        // GET: odata/Organisations(5)
        /// <summary>
        /// Organisation by Id
        /// </summary>
        /// <param name="id">Id of Organisation</param>
        /// <returns>Organisation</returns>
        [EnableQuery, ODataRoute("({id})")]
        public override SingleResult<Organisation> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }


        // GET: odata/Organisations(5)
        /// <summary>
        /// Organisation by Name
        /// </summary>
        /// <param name="name">Name of Organisation</param>
        /// <returns>Organisation</returns>
        [EnableQuery, ODataRoute("({name})")]
        public override SingleResult<Organisation> GetByName([FromODataUri] string name)
        {
            return base.GetByName(name);
        }

        //GET: odata/Organisations(5)/Sites
        /// <summary>
        /// Sites for the Organisation
        /// </summary>
        /// <param name="id">Id of Organisation</param>
        /// <returns>ListOf(Site)</returns>
        [EnableQuery, ODataRoute("({id})/Sites")]
        public IQueryable<Site> GetSites([FromODataUri] Guid id)
        {
            return GetMany<Site>(id, s => s.Sites, i => i.Organisations);
        }
    }
}