using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using SAEON.Observations.Core.Entities;
using System;
using System.Linq;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    /// <summary>
    /// Offerings
    /// </summary>
    [ODataRoutePrefix("Offerings")]
    public class OfferingsODController : NamedController<Offering>
    {

        // GET: odata/Offerings
        /// <summary>
        /// Get all Offerings
        /// </summary>
        /// <returns>ListOf(Offering)</returns>
        [ODataRoute]
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
        [ODataRoute("({id})")]
        public override SingleResult<Offering> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }

        // GET: odata/Offerings(5)/Phenomena
        /// <summary>
        /// Phenomena for the Offering
        /// </summary>
        /// <param name="id">Id of the Offering</param>
        /// <returns>ListOf(Phenomenon)</returns>
        [ODataRoute("({id})/Phenomena")]
        public IQueryable<Phenomenon> GetPhenomena([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.PhenomenonOfferings).Select(i => i.Phenomenon);
        }
    }

}
