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
    public class ProgrammesController : CodedEntityController<ProgrammesController, Programme>
    {
        protected override IQueryable<Programme> AddIncludes(IQueryable<Programme> query)
        {
            return base.AddIncludes(query)
                .Include(s => s.Projects);
        }

        /// <summary>
        /// All Programmes
        /// </summary>
        /// <returns>List&lt;Programme&gt;</returns>
        public override Task<List<Programme>> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Programme by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Programme</returns>
        public override Task<ActionResult<Programme>> GetByIdAsync(Guid id)
        {
            return base.GetByIdAsync(id);
        }

        /// <summary>
        /// Programme by Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Programme</returns>
        public override Task<ActionResult<Programme>> GetByNameAsync(string name)
        {
            return base.GetByNameAsync(name);
        }

        /// <summary>
        /// Programme by Code
        /// </summary>
        /// <param name="code"></param>
        /// <returns>Programme</returns>
        public override Task<ActionResult<Programme>> GetByCodeAsync(string code)
        {
            return base.GetByCodeAsync(code);
        }

        /// <summary>
        /// This Programme's projects
        /// </summary>
        /// <param name="id">Id of the Programme</param>
        /// <returns>List&lt;Project&gt;</returns>
        [HttpGet("{id:guid}/Projects")]
        public async Task<List<Project>> GetProjectsAsync(Guid id)
        {
            return await GetManyAsync(id, s => s.Projects);
        }
    }
}