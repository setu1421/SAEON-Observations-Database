using Microsoft.AspNetCore.Mvc;
using SAEON.Observations.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    public class SitesController : NamedApiController<Site>
    {
        //protected override IQueryable<Site> GetQuery(Expression<Func<Site, bool>> extraWhere = null)
        //{
        //    return base.GetQuery(extraWhere).Include(i => i.Organisations).Include(i => i.Stations);
        //}

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
        public override Task<ActionResult<Site>> GetByIdAsync(Guid id)
        {
            return base.GetByIdAsync(id);
        }

        /// <summary>
        /// Site by Code
        /// </summary>
        /// <param name="code">The Code of the Site</param>
        /// <returns>Site</returns>
        public override Task<ActionResult<Site>> GetByCodeAsync(string code)
        {
            return base.GetByCodeAsync(code);
        }

        /// <summary>
        /// Site by Name
        /// </summary>
        /// <param name="name">The Name of the Site</param>
        /// <returns>Site</returns>
        public override Task<ActionResult<Site>> GetByNameAsync(string name)
        {
            return base.GetByNameAsync(name);
        }

        /// <summary>
        /// Organisations of the Site
        /// </summary>
        /// <param name="id">Id of Site</param>
        /// <returns>ListOf(Organisation)</returns>
        [HttpGet("{id:guid}/Organisations")]
        public IQueryable<Organisation> GetOrganisations(Guid id)
        {
            return GetManyWithGuidId<Organisation>(id, s => s.Organisations);
        }

        /// <summary>
        /// Stations of the Site
        /// </summary>
        /// <param name="id">Id of Site</param>
        /// <returns>ListOf(Station)</returns>
        [HttpGet("{id:guid}/Stations")]
        public IQueryable<Station> GetStations(Guid id)
        {
            return GetManyWithGuidId<Station>(id, s => s.Stations);
        }
    }
}

