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
    [RoutePrefix("Api/Offerings")]
    public class OfferingsController : CodedApiController<Offering>
    {
        protected override List<Expression<Func<Offering, object>>> GetIncludes()
        {
            var list = base.GetIncludes();
            list.Add(i => i.Phenomena);
            return list;
        }

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
        [ResponseType(typeof(Offering))]
        public override async Task<IHttpActionResult> GetById([FromUri] Guid id)
        {
            return await base.GetById(id);
        }

        /// <summary>
        /// Offering by Code
        /// </summary>
        /// <param name="code">The Code of the Offering</param>
        /// <returns>Offering</returns>
        [ResponseType(typeof(Offering))]
        public override async Task<IHttpActionResult> GetByCode([FromUri] string code)
        {
            return await base.GetByCode(code);
        }

        /// <summary>
        /// Offering by Name
        /// </summary>
        /// <param name="name">The Name of the Offering</param>
        /// <returns>Offering</returns>
        [ResponseType(typeof(Offering))]
        public override async Task<IHttpActionResult> GetByName([FromUri] string name)
        {
            return await base.GetByName(name);
        }

        // GET: Offerings/5/Phenomena
        /// <summary>
        /// Phenomena for the Offering
        /// </summary>
        /// <param name="id">Id of the Offering</param>
        /// <returns>ListOf(Phenomemon)</returns>
        [Route("{id:guid}/Phenomena")]
        public IQueryable<Phenomenon> GetPhenomena([FromUri] Guid id)
        {
            return GetMany<Phenomenon>(id, s => s.Phenomena, i => i.Offerings);
        }
       
    }
    
}
