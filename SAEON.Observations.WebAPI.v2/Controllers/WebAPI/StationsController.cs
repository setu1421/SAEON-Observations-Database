using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAEON.Observations.Core.Entities;
using SAEON.Observations.WebAPI.Controllers.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.v2.Controllers.WebAPI
{
    [Produces("application/json")]
    [Route("api/Stations")]
    public class StationsController : BaseAPIController<Station>
    {
        public StationsController(ObservationsDbContext context) : base(context) { }

        protected override IQueryable<Station> ApplyIncludes(IQueryable<Station> query)
        {
            return query
                .Include(i => i.OrganisationStations)
                    .ThenInclude(i => i.Organisation)
                .Include(i => i.ProjectStations)
                    .ThenInclude(i => i.Project)
                .Include(i => i.ProjectStations)
                    .ThenInclude(i => i.Station)
                .Include(i => i.StationInstruments)
                    .ThenInclude(i => i.Instrument)
                .Include(i => i.Site);
        }


        [HttpGet("{id}/Instruments")]
        public IEnumerable<Instrument> GetInstruments(Guid id)
        {
            return GetMany(id, sel => sel.StationInstruments.Select(i => i.Instrument), inc => inc.StationInstruments.Select(i => i.Station));
        }

        [HttpGet("{id}/Organisations")]
        public IEnumerable<Organisation> GetOrganisations(Guid id)
        {
            return GetMany(id, sel => sel.OrganisationStations.Select(i => i.Organisation), inc => inc.OrganisationStations.Select(i => i.Station));
        }

        [HttpGet("{id}/Projects")]
        public IEnumerable<Project> GetProjects(Guid id)
        {
            return GetMany(id, sel => sel.ProjectStations.Select(i => i.Project), inc => inc.ProjectStations.Select(i => i.Station));
        }

        [HttpGet("{id}/Site")]
        public async Task<IActionResult> GetSite(Guid id)
        {
            return await GetSingle(id, sel => sel.Site, inc => inc.Stations);
        }

    }
}