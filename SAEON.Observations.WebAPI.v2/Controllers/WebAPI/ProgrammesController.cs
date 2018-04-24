using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAEON.Observations.Core.Entities;
using SAEON.Observations.WebAPI.Controllers.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SAEON.Observations.WebAPI.v2.Controllers.WebAPI
{
    [Produces("application/json")]
    [Route("api/Programmes")]
    public class ProgrammesController : BaseAPIController<Programme>
    {
        public ProgrammesController(ObservationsDbContext context) : base(context) { }

        protected override IQueryable<Programme> ApplyIncludes(IQueryable<Programme> query)
        {
            return query
                .Include(i => i.Projects);
        }

        [HttpGet("{id}/Projects")]
        public IEnumerable<Project> GetProjects(Guid id)
        {
            return GetMany(id, sel => sel.Projects, inc => inc.Programme);
        }

    }
}