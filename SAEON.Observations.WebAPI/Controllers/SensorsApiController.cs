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
    /// Sensors
    /// </summary>
    [RoutePrefix("Sensors")]
    public class SensorsApiController : BaseApiController<Sensor>
    {
        protected override List<Expression<Func<Sensor, object>>> GetIncludes()
        {
            var list = base.GetIncludes();
            list.Add(i => i.Instruments);
            list.Add(i => i.Phenomenon);
            return list;
        }

        /// <summary>
        /// Sensors
        /// </summary>
        /// <returns>ListOf(Sensor)</returns>
        public override IQueryable<Sensor> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Sensor by Id
        /// </summary>
        /// <param name="id">The Id of the Sensor</param>
        /// <returns>Sensor</returns>
        [ResponseType(typeof(Sensor))]
        public override async Task<IHttpActionResult> GetById(Guid id)
        {
            return await base.GetById(id);
        }

        /// <summary>
        /// Sensor by Name
        /// </summary>
        /// <param name="name">The Name of the Sensor</param>
        /// <returns>Sensor</returns>
        [ResponseType(typeof(Sensor))]
        public override async Task<IHttpActionResult> GetByName(string name)
        {
            return await base.GetByName(name);
        }


        // GET: Sensors/5/Instruments
        /// <summary>
        /// Instruments for the Sensor
        /// </summary>
        /// <param name="id">Id of the Sensor</param>
        /// <returns>ListOf(Instrument)</returns>
        [Route("{id:guid}/Instruments")]
        public IQueryable<Instrument> GetInstruments([FromUri] Guid id)
        {
            return GetMany<Instrument>(id, s => s.Instruments, i => i.Sensors);
        }

        // GET: Sensors/5/Phenomenon
        /// <summary>
        /// Phenomena of a Sensor
        /// </summary>
        /// <param name="id">The Id of the Sensor</param>
        /// <returns>Phenomenon</returns>
        [Route("{id:guid}/Phenomenon")]
        [ResponseType(typeof(Phenomenon))]
        public async Task<IHttpActionResult> GetPhenomenon(Guid id)
        {
            return await GetSingle<Phenomenon>(id, s => s.Phenomenon, i => i.Sensors);
        }

    }
}
