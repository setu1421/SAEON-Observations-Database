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
    public class OrganisationsController : CodedEntityController<OrganisationsController, Organisation>
    {
        protected override IQueryable<Organisation> AddIncludes(IQueryable<Organisation> query)
        {
            return base.AddIncludes(query)
                .Include(s => s.OrganisationSites).ThenInclude(os => os.Site)
                .Include(s => s.OrganisationStations).ThenInclude(os => os.Station)
                .Include(s => s.OrganisationInstruments).ThenInclude(os => os.Instrument);
        }

        /// <summary>
        /// All Organisations
        /// </summary>
        /// <returns>List&lt;Organisation&gt;</returns>
        public override Task<List<Organisation>> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Organisation by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Organisation</returns>
        public override Task<ActionResult<Organisation>> GetByIdAsync(Guid id)
        {
            return base.GetByIdAsync(id);
        }

        /// <summary>
        /// Organisation by Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Organisation</returns>
        public override Task<ActionResult<Organisation>> GetByNameAsync(string name)
        {
            return base.GetByNameAsync(name);
        }

        /// <summary>
        /// Organisation by Code
        /// </summary>
        /// <param name="code"></param>
        /// <returns>Organisation</returns>
        public override Task<ActionResult<Organisation>> GetByCodeAsync(string code)
        {
            return base.GetByCodeAsync(code);
        }

        /// <summary>
        /// Sites linked to this Organisation
        /// </summary>
        /// <param name="id">Id of the Organisation</param>
        /// <returns>List&lt;Site&gt;</returns>
        [HttpGet("{id:guid}/Sites")]
        public async Task<List<Site>> GetSitesAsync(Guid id)
        {
            return await GetManyAsync(id, s => s.OrganisationSites.Select(oi => oi.Site));
        }


        /// <summary>
        /// Stations linked to this Organisation
        /// </summary>
        /// <param name="id">Id of the Organisation</param>
        /// <returns>List&lt;Station&gt;</returns>
        [HttpGet("{id:guid}/Stations")]
        public async Task<List<Station>> GetStationsAsync(Guid id)
        {
            return await GetManyAsync(id, s => s.OrganisationStations.Select(oi => oi.Station));
        }

        /// <summary>
        /// Instruments linked to this Organisation
        /// </summary>
        /// <param name="id">Id of the Organisation</param>
        /// <returns>List&lt;Instrument&gt;</returns>
        [HttpGet("{id:guid}/Instruments")]
        public async Task<List<Instrument>> GetInstrumentsAsync(Guid id)
        {
            return await GetManyAsync(id, s => s.OrganisationInstruments.Select(oi => oi.Instrument));
        }
    }
}