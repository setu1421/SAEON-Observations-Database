using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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

    }
}
