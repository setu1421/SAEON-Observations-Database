using System;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using SAEON.Observations.Core.Entities;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    /*
    /// <summary>
    /// Offerings
    /// </summary>
    [ODataRoutePrefix("Offerings")]
    public class OfferingsODataController : BaseODataController<Offering>
    {

        // GET: odata/Offerings
        /// <summary>
        /// Get all Offerings
        /// </summary>
        /// <returns>ListOf(Offering)</returns>
        [EnableQuery, ODataRoute]
        public override IQueryable<Offering> GetAll()
        {
            return base.GetAll();
        }

        // GET: odata/Offerings(5)
        /// <summary>
        /// Offering by Id
        /// </summary>
        /// <param name="id">Id of Offering</param>
        /// <returns>Offering</returns>
        [EnableQuery, ODataRoute("({id})")]
        public override SingleResult<Offering> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }

        // GET: odata/Offerings(5)
        /// <summary>
        /// Offering by Name
        /// </summary>
        /// <param name="name">Name of Offering</param>
        /// <returns>Offering</returns>
        [EnableQuery, ODataRoute("({name})")]
        public override SingleResult<Offering> GetByName([FromODataUri] string name)
        {
            return base.GetByName(name);
        }

        // GET: odata/Offerings(5)/Phenomena
        /// <summary>
        /// Phenomena for the Offering
        /// </summary>
        /// <param name="id">Id of the Offering</param>
        /// <returns>ListOf(Phenomenon)</returns>
        [EnableQuery, ODataRoute("({id})/Phenomena")]
        public IQueryable<Phenomenon> GetPhenomena([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.Phenomena, i => i.Offerings);
        }
    }
    */
}
