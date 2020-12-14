using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    public class OfferingsController : NamedApiController<Offering>
    {
        //protected override IQueryable<Offering> GetQuery(Expression<Func<Offering, bool>> extraWhere = null)
        //{
        //    return base.GetQuery(extraWhere).Include(i => i.Phenomena);
        //}

        /// <summary>
        /// All Offerings
        /// </summary>
        /// <returns>ListOf(Offering)</returns>
        public override IQueryable<Offering> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Offering by Id
        /// </summary>
        /// <param name="id">The Id of the Offering</param>
        /// <returns>Offering</returns>
        public override Task<ActionResult<Offering>> GetByIdAsync(Guid id)
        {
            return base.GetByIdAsync(id);
        }

        /// <summary>
        /// Offering by Code
        /// </summary>
        /// <param name="code">The Code of the Offering</param>
        /// <returns>Offering</returns>
        public override Task<ActionResult<Offering>> GetByCodeAsync(string code)
        {
            return base.GetByCodeAsync(code);
        }

        /// <summary>
        /// Offering by Name
        /// </summary>
        /// <param name="name">The Name of the Offering</param>
        /// <returns>Offering</returns>
        public override Task<ActionResult<Offering>> GetByNameAsync(string name)
        {
            return base.GetByNameAsync(name);
        }

        /// <summary>
        /// Phenomena of the Offering
        /// </summary>
        /// <param name="id">Id of the Offering</param>
        /// <returns>ListOf(Phenomemon)</returns>
        [HttpGet("{id:guid}/Phenomena")]
        public IQueryable<Phenomenon> GetPhenomena(Guid id)
        {
            return GetManyWithGuidId<Phenomenon>(id, s => s.Phenomena);
        }
    }
}
