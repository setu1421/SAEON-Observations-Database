using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    public class UnitsController : CodedApiController<Unit>
    {
        //protected override IQueryable<Unit> GetQuery(Expression<Func<Unit, bool>> extraWhere = null)
        //{
        //    return base.GetQuery(extraWhere).Include(i => i.Phenomena);
        //}

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
        public override Task<ActionResult<Unit>> GetByIdAsync(Guid id)
        {
            return base.GetByIdAsync(id);

        }
        /// <summary>
        /// Unit by Code
        /// </summary>
        /// <param name="code">The Code of the Unit</param>
        /// <returns>Unit</returns>
        public override Task<ActionResult<Unit>> GetByCodeAsync(string code)
        {
            return base.GetByCodeAsync(code);
        }

        /// <summary>
        /// Phenomena of the Unit
        /// </summary>
        /// <param name="id">Id of the Unit</param>
        /// <returns>ListOf(Phenomenon)</returns>
        [HttpGet("{id:guid}/Phenomena")]
        public IQueryable<Phenomenon> GetPhenomena(Guid id)
        {
            return GetManyWithGuidId<Phenomenon>(id, s => s.Phenomena);
        }
    }
}
