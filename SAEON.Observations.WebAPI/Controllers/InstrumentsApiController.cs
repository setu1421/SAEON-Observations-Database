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
    /// Instruments
    /// </summary>
    [RoutePrefix("Instruments")]
    public class InstrumentsApiController : BaseApiController<Instrument>
    {
        protected override List<Expression<Func<Instrument, object>>> GetIncludes()
        {
            var list = base.GetIncludes();
            list.Add(i => i.Stations);
            list.Add(i => i.Sensors);
            return list;
        }

        /// <summary>
        /// Return a list of Instruments
        /// </summary>
        /// <returns>A list of Instrument</returns>
        public override IQueryable<Instrument> GetAll()
        {
            return base.GetAll();
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

        // GET: Instruments/5/Stations
        [Route("{id:guid}/Stations")]
        public IQueryable<Station> GetStations([FromUri] Guid id)
        {
            return GetMany<Station>(id, s => s.Stations, i => i.Instruments);
        }

        // GET: Instruments/5/Sensors
        [Route("{id:guid}/Sensors")]
        public IQueryable<Sensor> GetSensors([FromUri] Guid id)
        {
            return GetMany<Sensor>(id, s => s.Sensors, i => i.Instruments);
        }

    }
}
