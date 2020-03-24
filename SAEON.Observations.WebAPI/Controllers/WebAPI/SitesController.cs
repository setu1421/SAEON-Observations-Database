using SAEON.Observations.Core.Entities;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    /// <summary>
    /// </summary>
    [RoutePrefix("Api/Sites")]
    public class SitesController : CodedApiController<Site>
    {
        protected override IQueryable<Site> GetQuery(Expression<Func<Site, bool>> extraWhere = null)
        {
            return base.GetQuery(extraWhere).Include(i => i.Organisations).Include(i => i.Stations);
        }

        /// <summary>
        /// All Sites
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
        public override async Task<IHttpActionResult> GetByIdAsync([FromUri] Guid id)
        {
            return await base.GetByIdAsync(id);
        }

        /// <summary>
        /// Site by Name
        /// </summary>
        /// <param name="name">The Name of the Site</param>
        /// <returns>Site</returns>
        [ResponseType(typeof(Site))]
        public override async Task<IHttpActionResult> GetByNameAsync([FromUri] string name)
        {
            return await base.GetByNameAsync(name);
        }

        /// <summary>
        /// Site by Code
        /// </summary>
        /// <param name="code">The Code of the Site</param>
        /// <returns>Site</returns>
        [ResponseType(typeof(Site))]
        public override async Task<IHttpActionResult> GetByCodeAsync([FromUri] string code)
        {
            return await base.GetByCodeAsync(code);
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
            return GetManyWithGuidId<Organisation>(id, s => s.Organisations);
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
            return GetManyWithGuidId<Station>(id, s => s.Stations);
        }

    }
}
