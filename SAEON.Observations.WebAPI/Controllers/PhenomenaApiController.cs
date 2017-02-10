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
    /// Phenomena
    /// </summary>
    [RoutePrefix("Phenomena")]
    public class PhenomenaApiController : BaseApiController<Phenomenon>
    {
        /// <summary>
        /// Return a Phenomenon by Id
        /// </summary>
        /// <param name="id">The Id of the Phenomenon</param>
        /// <returns>Phenomenon</returns>
        [ResponseType(typeof(Phenomenon))]
        public override async Task<IHttpActionResult> GetById(Guid id)
        {
            return await base.GetById(id);
        }

        /// <summary>
        /// Return a Phenomenon by Name
        /// </summary>
        /// <param name="name">The Name of the Phenomenon</param>
        /// <returns>Phenomenon</returns>
        [ResponseType(typeof(Phenomenon))]
        public override async Task<IHttpActionResult> GetByName(string name)
        {
            return await base.GetByName(name);
        }

    }
}
