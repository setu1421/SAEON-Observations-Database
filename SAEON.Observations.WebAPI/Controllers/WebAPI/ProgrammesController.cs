using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    /// <summary>
    /// </summary>
    [RoutePrefix("Api/Programmes")]
    public class ProgrammesController :  CodedApiController<Programme>
    {
        protected override List<Expression<Func<Programme, object>>> GetIncludes()
        {
            var list = base.GetIncludes();
            list.Add(i => i.Projects);
            return list;
        }

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
        [ResponseType(typeof(Programme))]
        public override async Task<IHttpActionResult> GetById([FromUri] Guid id)
        {
            return await base.GetById(id);
        }

        /// <summary>
        /// Programme by Code
        /// </summary>
        /// <param name="code">The Code of the Programme</param>
        /// <returns>Programme</returns>
        [ResponseType(typeof(Programme))]
        public override async Task<IHttpActionResult> GetByCode([FromUri] string code)
        {
            return await base.GetByCode(code);
        }

        /// <summary>
        /// Programme by Name
        /// </summary> 
        /// <param name="name">The Name of the Programme</param>
        /// <returns>Programme</returns>
        [ResponseType(typeof(Programme))]
        public override async Task<IHttpActionResult> GetByName([FromUri] string name)
        {
            return await base.GetByName(name);
        }

        //GET: Programmes/5/Projects
        /// <summary>
        /// Projects for the Programme
        /// </summary>
        /// <param name="id">Id of Programme</param>
        /// <returns>ListOf(Project)</returns>
        [Route("{id:guid}/Projects")]
        public IQueryable<Project> GetProjects([FromUri] Guid id)
        {
            return GetMany<Project>(id, s => s.Projects, i => i.Programme);
        }

    }
}