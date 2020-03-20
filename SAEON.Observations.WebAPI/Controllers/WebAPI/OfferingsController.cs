using SAEON.Observations.Core.Entities;
using System;
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
        protected override IQueryable<Offering> GetQuery(Expression<Func<Offering, bool>> extraWhere = null)
        {
            return base.GetQuery(extraWhere);//.Include(i => i.PhenomenonOfferings.Select(po => po.Phenomenon));
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
        public override async Task<IHttpActionResult> GetByIdAsync([FromUri] Guid id)
        {
            return await base.GetByIdAsync(id);
        }

        /// <summary>
        /// Offering by Code
        /// </summary>
        /// <param name="code">The Code of the Offering</param>
        /// <returns>Offering</returns>
        [ResponseType(typeof(Offering))]
        public override async Task<IHttpActionResult> GetByCodeAsync([FromUri] string code)
        {
            return await base.GetByCodeAsync(code);
        }

        /// <summary>
        /// Offering by Name
        /// </summary>
        /// <param name="name">The Name of the Offering</param>
        /// <returns>Offering</returns>
        [ResponseType(typeof(Offering))]
        public override async Task<IHttpActionResult> GetByNameAsync([FromUri] string name)
        {
            return await base.GetByNameAsync(name);
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
            return GetManyIdEntity<PhenomenonOffering>(id, s => s.PhenomenonOfferings).Select(i => i.Phenomenon);
        }

    }

}
