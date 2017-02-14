using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.ModelBinding;
using System.Web.OData;
using System.Web.OData.Query;
using System.Web.OData.Routing;
using SAEON.Observations.Core;

namespace SAEON.Observations.WebAPI.Controllers
{
    /// <summary>
    /// Offerings
    /// </summary>
    [ODataRoutePrefix("Offerings")]
    public class OfferingsODataController : BaseODataController<Offering>
    {

        // GET: odata/Offerings
        /// <summary>
        /// Get a list of Offerings
        /// </summary>
        /// <returns>A list of Offering</returns>
        [EnableQuery, ODataRoute]
        public override IQueryable<Offering> GetAll()
        {
            return base.GetAll();
        }

        // GET: odata/Offerings(5)
        /// <summary>
        /// Get an Offering by Id
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
        /// Get an Offering by Name
        /// </summary>
        /// <param name="name">Name of Offering</param>
        /// <returns>Offering</returns>
        [EnableQuery, ODataRoute("({name})")]
        public override SingleResult<Offering> GetByName([FromODataUri] string name)
        {
            return base.GetByName(name);
        }

        // GET: odata/Offerings(5)/Phenomena
        [EnableQuery, ODataRoute("({id})/Phenomena")]
        public IQueryable<Phenomenon> GetPhenomena([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.Phenomena, i => i.Offerings);
        }
    }
}
