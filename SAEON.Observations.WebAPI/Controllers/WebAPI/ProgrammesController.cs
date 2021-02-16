using Microsoft.AspNetCore.Mvc;
using SAEON.Observations.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    public class ProgrammesController : NamedApiController<Programme>
    {
        //protected override IQueryable<Programme> GetQuery(Expression<Func<Programme, bool>> extraWhere = null)
        //{
        //    return base.GetQuery(extraWhere).Include(i => i.Projects); ;
        //}

        /// <summary>
        /// All Programmes
        /// </summary>
        /// <returns>ListOf(Programme)</returns>
        public override IQueryable<Programme> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Programme by Id
        /// </summary>
        /// <param name="id">The Id of the Programme</param>
        /// <returns>Programme</returns>
        public override Task<ActionResult<Programme>> GetByIdAsync(Guid id)
        {
            return base.GetByIdAsync(id);
        }

        /// <summary>
        /// Programme by Code
        /// </summary>
        /// <param name="code">The Code of the Programme</param>
        /// <returns>Programme</returns>
        public override Task<ActionResult<Programme>> GetByCodeAsync(string code)
        {
            return base.GetByCodeAsync(code);
        }

        /// <summary>
        /// Programme by Name
        /// </summary> 
        /// <param name="name">The Name of the Programme</param>
        /// <returns>Programme</returns>
        public override Task<ActionResult<Programme>> GetByNameAsync(string name)
        {
            return base.GetByNameAsync(name);
        }

        /// <summary>
        /// Projects of the Programme
        /// </summary>
        /// <param name="id">Id of Programme</param>
        /// <returns>ListOf(Project)</returns>
        [HttpGet("{id:guid}/Projects")]
        public IQueryable<Project> GetProjects(Guid id)
        {
            return GetManyWithGuidId<Project>(id, s => s.Projects);
        }
    }
}
