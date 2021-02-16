using Microsoft.AspNetCore.Mvc;
using SAEON.Observations.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    public class ProjectsController : NamedApiController<Project>
    {
        //protected override IQueryable<Project> GetQuery(Expression<Func<Project, bool>> extraWhere = null)
        //{
        //    return base.GetQuery(extraWhere).Include(i => i.Programme).Include(i => i.Stations);
        //}

        /// <summary>
        /// All Projects
        /// </summary>
        /// <returns>ListOf(Project)</returns>
        public override IQueryable<Project> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Project by Id
        /// </summary>
        /// <param name="id">The Id of the Project</param>
        /// <returns>Project</returns>
        public override Task<ActionResult<Project>> GetByIdAsync(Guid id)
        {
            return base.GetByIdAsync(id);
        }

        /// <summary>
        /// Project by Code
        /// </summary>
        /// <param name="code">The Code of the Project</param>
        /// <returns>Project</returns>
        public override Task<ActionResult<Project>> GetByCodeAsync(string code)
        {
            return base.GetByCodeAsync(code);
        }

        /// <summary>
        /// Project by Name
        /// </summary>
        /// <param name="name">The Name of the Project</param>
        /// <returns>Project</returns>
        public override Task<ActionResult<Project>> GetByNameAsync(string name)
        {
            return base.GetByNameAsync(name);
        }

        /// <summary>
        /// Programme of the Project
        /// </summary>
        /// <param name="id">Id of Project</param>
        /// <returns>Programme</returns>
        [HttpGet("{id:guid}/Programme")]
        public async Task<ActionResult<Programme>> ProgrammeAsync(Guid id)
        {
            return await GetSingleAsync(id, s => s.Programme);
        }

        /// <summary>
        /// Stations of the Project
        /// </summary>
        /// <param name="id">Id of Project</param>
        /// <returns>ListOf(Station)</returns>
        [HttpGet("{id:guid}/Stations")]
        public IQueryable<Station> GetStations(Guid id)
        {
            return GetManyWithGuidId<Station>(id, s => s.Stations);
        }
    }
}
