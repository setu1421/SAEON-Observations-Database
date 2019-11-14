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
    public class ProjectsController : CodedEntityController<ProjectsController, Project>
    {
        protected override IQueryable<Project> AddIncludes(IQueryable<Project> query)
        {
            return base.AddIncludes(query)
                .Include(s => s.ProjectStations).ThenInclude(ps => ps.Station)
                .Include(s => s.Programme);
        }

        /// <summary>
        /// All Projects
        /// </summary>
        /// <returns>List&lt;Project&gt;</returns>
        public override Task<List<Project>> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Project by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Project</returns>
        public override Task<ActionResult<Project>> GetByIdAsync(Guid id)
        {
            return base.GetByIdAsync(id);
        }

        /// <summary>
        /// Project by Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Project</returns>
        public override Task<ActionResult<Project>> GetByNameAsync(string name)
        {
            return base.GetByNameAsync(name);
        }

        /// <summary>
        /// Project by Code
        /// </summary>
        /// <param name="code"></param>
        /// <returns>Project</returns>
        public override Task<ActionResult<Project>> GetByCodeAsync(string code)
        {
            return base.GetByCodeAsync(code);
        }

        /// <summary>
        /// Stations linked to this Project
        /// </summary>
        /// <param name="id">Id of the Project</param>
        /// <returns>List&lt;Station&gt;</returns>
        [HttpGet("{id:guid}/Stations")]
        public async Task<List<Station>> GetStationsAsync(Guid id)
        {
            return await GetManyAsync(id, s => s.ProjectStations.Select(oi => oi.Station));
        }

        /// <summary>
        /// This Project's Programme
        /// </summary>
        /// <param name="id">Id of the Project</param>
        /// <returns>Programme</returns>
        [HttpGet("{id:guid}/Programme")]
        public async Task<ActionResult<Programme>> GetProgrammeAsync(Guid id)
        {
            return await GetSingleAsync(id, s => s.Programme);
        }
    }
}