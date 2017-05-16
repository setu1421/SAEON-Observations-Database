using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    /// <summary>
    /// Organisations
    /// </summary>
    [RoutePrefix("Organisations")]
    public class OrganisationsController : BaseApiController<Organisation>
    {
        protected override List<Expression<Func<Organisation, object>>> GetIncludes()
        {
            var list = base.GetIncludes();
            list.Add(i => i.Sites);
            return list;
        }

        /// <summary>
        /// all Organisations
        /// </summary>
        /// <returns>ListOf(Organisation)</returns>
        public override IQueryable<Organisation> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Organisation by Id
        /// </summary>
        /// <param name="id">The Id of the Organisation</param>
        /// <returns>Organisation</returns>
        [ResponseType(typeof(Organisation))]
        public override async Task<IHttpActionResult> GetById(Guid id)
        {
            return await base.GetById(id);
        }

        /// <summary>
        /// Organisation by Name
        /// </summary>
        /// <param name="name">The Name of the Organisation</param>
        /// <returns>Organisation</returns>
        [ResponseType(typeof(Organisation))]
        public override async Task<IHttpActionResult> GetByName(string name)
        {
            return await base.GetByName(name);
        }

        //GET: Organisations/5/Sites
        /// <summary>
        /// Sites for the Organisation
        /// </summary>
        /// <param name="id">Id of Organisation</param>
        /// <returns>ListOf(Site)</returns>
        [Route("{id:guid}/Sites")]
        public IQueryable<Site> GetSites([FromUri] Guid id)
        {
            return GetMany<Site>(id, s => s.Sites, i => i.Organisations);
        }

    }
}
