using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers
{
    /// <summary>
    /// Offerings
    /// </summary>
    [RoutePrefix("Offerings")]
    public class OfferingsApiController : BaseApiController<Offering>
    {
        protected override List<Expression<Func<Offering, object>>> GetIncludes()
        {
            var list = base.GetIncludes();
            list.Add(i => i.Phenomena);
            return list;
        }

        /// <summary>
        /// Return a list of Offerings
        /// </summary>
        /// <returns>A list of Offering</returns>
        public override IQueryable<Offering> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Return a Offering by Id
        /// </summary>
        /// <param name="id">The Id of the Offering</param>
        /// <returns>Offering</returns>
        [ResponseType(typeof(Offering))]
        public override async Task<IHttpActionResult> GetById(Guid id)
        {
            return await base.GetById(id);
        }

        /// <summary>
        /// Return a Offering by Name
        /// </summary>
        /// <param name="name">The Name of the Offering</param>
        /// <returns>Offering</returns>
        [ResponseType(typeof(Offering))]
        public override async Task<IHttpActionResult> GetByName(string name)
        {
            return await base.GetByName(name);
        }

        // GET: Offerings/5/Phenomena
        [Route("{id:guid}/Phenomena")]
        public IQueryable<Phenomenon> GetPhenomena([FromUri] Guid id)
        {
            return GetMany<Phenomenon>(id, s => s.Phenomena, i => i.Offerings);
        }
    }
}
