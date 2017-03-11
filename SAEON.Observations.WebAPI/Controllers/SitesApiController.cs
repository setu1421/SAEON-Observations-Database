using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Linq.Expressions;

namespace SAEON.Observations.WebAPI.Controllers
{

    /// <summary>
    /// Sites
    /// </summary>
    [RoutePrefix("Sites")]
    public class SitesApiController : BaseApiController<Site>
    {
        protected override List<Expression<Func<Site, object>>> GetIncludes()
        {
            var list = base.GetIncludes();
            list.Add(i => i.Organisations);
            list.Add(i => i.Stations);
            return list;
        }

        /// <summary>
        /// all Sites
        /// </summary>
        /// <returns>ListOf(Site)</returns>
        public override IQueryable<Site> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Site by Id
        /// </summary>
        /// <param name="id">The Id of the Site</param>
        /// <returns>Site</returns>
        [ResponseType(typeof(Site))]
        public override async Task<IHttpActionResult> GetById(Guid id)
        {
            return await base.GetById(id);
        }

        /// <summary>
        /// Site by Name
        /// </summary>
        /// <param name="name">The Name of the Site</param>
        /// <returns>Site</returns>
        [ResponseType(typeof(Site))]
        public override async Task<IHttpActionResult> GetByName(string name)
        {
            return await base.GetByName(name);
        }

        //GET: Sites/5/Organisations
        /// <summary>
        /// Organisations for the Site
        /// </summary>
        /// <param name="id">Id of Site</param>
        /// <returns>ListOf(Organisation)</returns>
        [Route("{id:guid}/Organisations")]
        public IQueryable<Organisation> GetOrganisations([FromUri] Guid id)
        {
            return GetMany<Organisation>(id, s => s.Organisations, i => i.Sites);
        }

        //GET: Sites/5/Stations
        /// <summary>
        /// Stations for the Site
        /// </summary>
        /// <param name="id">Id of Site</param>
        /// <returns>ListOf(Station)</returns>
        [Route("{id:guid}/Stations")]
        public IQueryable<Station> GetStations([FromUri] Guid id)
        {
            return GetMany<Station>(id, s => s.Stations, i => i.Site);
        }

    }
}
