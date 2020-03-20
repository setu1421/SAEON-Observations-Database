using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAEON.Observations.Core.Entities;

namespace SAEON.Observations.WebAPI.v2.Controllers.WebApi
{
    public class StationsController : CodedEntityController<StationsController, Station>
    {
        protected override IQueryable<Station> AddIncludes(IQueryable<Station> query)
        {
            return base.AddIncludes(query)
                .Include(s => s.OrganisationStations).ThenInclude(os => os.Organisation)
                .Include(s => s.Site);
        }

        /// <summary>
        /// All Stations
        /// </summary>
        /// <returns>List&lt;Station&gt;</returns>
        public override Task<List<Station>> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Station by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Station</returns>
        public override Task<ActionResult<Station>> GetByIdAsync(Guid id)
        {
            return base.GetByIdAsync(id);
        }

        /// <summary>
        /// Station by Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Station</returns>
        public override Task<ActionResult<Station>> GetByNameAsync(string name)
        {
            return base.GetByNameAsync(name);
        }

        /// <summary>
        /// Station by Code
        /// </summary>
        /// <param name="code"></param>
        /// <returns>Station</returns>
        public override Task<ActionResult<Station>> GetByCodeAsync(string code)
        {
            return base.GetByCodeAsync(code);
        }

        /// <summary>
        /// Organisations linked to this Station
        /// </summary>
        /// <param name="id">Id of the Station</param>
        /// <returns>List&lt;Organisation&gt;</returns>
        [HttpGet("{id:guid}/Organisations")]
        public async Task<List<Organisation>> GetOrganisationsAsync(Guid id)
        {
            return await GetManyAsync(id, s => s.OrganisationStations.Select(oi => oi.Organisation));
        }

        /// <summary>
        /// This Station's Site
        /// </summary>
        /// <param name="id">Id of the Station</param>
        /// <returns>Site</returns>
        [HttpGet("{id:guid}/Site")]
        public async Task<ActionResult<Site>> GetSiteAsync(Guid id)
        {
            return await GetSingleAsync(id, s => s.Site);
        }
    }
}