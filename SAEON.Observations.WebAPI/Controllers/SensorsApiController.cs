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
    /// Sensors
    /// </summary>
    [RoutePrefix("Sensors")]
    public class SensorsApiController : BaseApiController<Sensor>
    {
        /// <summary>
        /// Return a Sensor by Id
        /// </summary>
        /// <param name="id">The Id of the Sensor</param>
        /// <returns>Sensor</returns>
        [ResponseType(typeof(Sensor))]
        public override async Task<IHttpActionResult> GetById(Guid id)
        {
            return await base.GetById(id);
        }

        /// <summary>
        /// Return a Sensor by Name
        /// </summary>
        /// <param name="name">The Name of the Sensor</param>
        /// <returns>Sensor</returns>
        [ResponseType(typeof(Sensor))]
        public override async Task<IHttpActionResult> GetByName(string name)
        {
            return await base.GetByName(name);
        }

    }
}
