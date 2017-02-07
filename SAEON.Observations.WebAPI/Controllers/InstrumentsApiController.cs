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
    /// Instruments
    /// </summary>
    [RoutePrefix("Instruments")]
    public class InstrumentsApiController : BaseApiController<Instrument>
    {
        /// <summary>
        /// Return a list of Instruments
        /// </summary>
        /// <returns>A list of Instrument</returns>
        [ResponseType(typeof(List<Instrument>))]
        public override async Task<IHttpActionResult> GetAll()
        {
            return await base.GetAll();
        }

        /// <summary>
        /// Return a Instrument by Id
        /// </summary>
        /// <param name="id">The Id of the Instrument</param>
        /// <returns>Instrument</returns>
        [ResponseType(typeof(Instrument))]
        public override async Task<IHttpActionResult> GetById(Guid id)
        {
            return await base.GetById(id);
        }

        /// <summary>
        /// Return a Instrument by Name
        /// </summary>
        /// <param name="name">The Name of the Instrument</param>
        /// <returns>Instrument</returns>
        [ResponseType(typeof(Instrument))]
        public override async Task<IHttpActionResult> GetByName(string name)
        {
            return await base.GetByName(name);
        }

    }
}
