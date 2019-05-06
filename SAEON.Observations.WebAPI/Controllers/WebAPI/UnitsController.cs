using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    /// <summary>
    /// </summary>
    [RoutePrefix("Api/Units")]
    public class UnitsController : CodedApiController<Unit>
    {
        protected override List<Expression<Func<Unit, object>>> GetIncludes()
        {
            var list = base.GetIncludes();
            list.Add(i => i.Phenomena);
            return list;
        }

        /// <summary>
        /// All Units
        /// </summary>
        /// <returns>ListOf(Unit)</returns>
        public override IQueryable<Unit> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Unit by Id
        /// </summary>
        /// <param name="id">The Id of the Unit</param>
        /// <returns>Unit</returns>
        [ResponseType(typeof(Unit))]
        public override async Task<IHttpActionResult> GetByIdAsync([FromUri] Guid id)
        {
            return await base.GetByIdAsync(id);
        }

        /// <summary>
        /// Unit by Code
        /// </summary>
        /// <param name="code">The Code of the Unit</param>
        /// <returns>Unit</returns>
        [ResponseType(typeof(Unit))]
        public override async Task<IHttpActionResult> GetByCodeAsync([FromUri] string code)
        {
            return await base.GetByCodeAsync(code);
        }

        /// <summary>
        /// Unit by Name
        /// </summary>
        /// <param name="name">The Name of the Unit</param>
        /// <returns>Unit</returns>
        [ResponseType(typeof(Unit))]
        public override async Task<IHttpActionResult> GetByNameAsync([FromUri] string name)
        {
            return await base.GetByNameAsync(name);
        }

        // GET: Units/5/Phenomena
        /// <summary>
        /// Phenomena for the Unit
        /// </summary>
        /// <param name="id">Id of the Unit</param>
        /// <returns>ListOf(Phenomenon)</returns>
        [Route("{id:guid}/Phenomena")]
        public IQueryable<Phenomenon> GetPhenomena([FromUri] Guid id)
        {
            return GetMany<Phenomenon>(id, s => s.Phenomena, i => i.Units);
        }
    }
}
