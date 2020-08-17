using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAEON.Observations.Core.Entities;
using SAEON.Observations.WebAPI.v2.Controllers.WebApi;

namespace SAEON.Observations.WebAPI.Controllers.WebApi
{
    public class SitesController : CodedEntityController<SitesController, Site>
    {
        protected override IQueryable<Site> AddIncludes(IQueryable<Site> query)
        {
            return base.AddIncludes(query)
                .Include(s => s.OrganisationSites).ThenInclude(os => os.Organisation)
                .Include(s => s.Stations);
        }

        /// <summary>
        /// All Sites
        /// </summary>
        /// <returns>List&lt;Site&gt;</returns>
        public override Task<List<Site>> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Site by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Site</returns>
        public override Task<ActionResult<Site>> GetByIdAsync(Guid id)
        {
            return base.GetByIdAsync(id);
        }

        /// <summary>
        /// Site by Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Site</returns>
        public override Task<ActionResult<Site>> GetByNameAsync(string name)
        {
            return base.GetByNameAsync(name);
        }

        /// <summary>
        /// Site by Code
        /// </summary>
        /// <param name="code"></param>
        /// <returns>Site</returns>
        public override Task<ActionResult<Site>> GetByCodeAsync(string code)
        {
            return base.GetByCodeAsync(code);
        }

        /// <summary>
        /// Organisations linked to this Site
        /// </summary>
        /// <param name="id">Id of the Site</param>
        /// <returns>List&lt;Organisation&gt;</returns>
        [HttpGet("{id:guid}/Organisations")]
        public async Task<List<Organisation>> GetOrganisationsAsync(Guid id)
        {
            return await GetManyAsync(id, s => s.OrganisationSites.Select(oi => oi.Organisation));
        }

        /// <summary>
        /// Stations linked to this Site
        /// </summary>
        /// <param name="id">Id of the Site</param>
        /// <returns>List&lt;Station&gt;</returns>
        [HttpGet("{id:guid}/Stations")]
        public async Task<List<Station>> GetStationsAsync(Guid id)
        {
            return await GetManyAsync<Station>(id, s => s.Stations);
        }
    }
}
