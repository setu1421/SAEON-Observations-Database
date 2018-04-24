using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAEON.Observations.Core.Entities;
using SAEON.Observations.WebAPI.Controllers.WebAPI;

namespace SAEON.Observations.WebAPI.v2.Controllers.WebAPI
{
    [Produces("application/json")]
    [Route("api/Projects")]
    public class ProjectsController : BaseAPIController<Project>
    {
        public ProjectsController(ObservationsDbContext context) : base(context) { }

        protected override IQueryable<Project> ApplyIncludes(IQueryable<Project> query)
        {
            return query
                .Include(i => i.ProjectStations)
                    .ThenInclude(i => i.Project)
                .Include(i => i.ProjectStations)
                    .ThenInclude(i => i.Station)
                .Include(i => i.Programme);
        }

        [HttpGet("{id}/Programme")]
        public async Task<IActionResult> GetProgramme(Guid id)
        {
            return await GetSingle(id, sel => sel.Programme, inc => inc.Projects);
        }

        [HttpGet("{id}/Stations")]
        public IEnumerable<Station> GetStations(Guid id)
        {
            return GetMany(id, sel => sel.ProjectStations.Select(i => i.Station), inc => inc.ProjectStations.Select(i => i.Project));
        }

    }
}