using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    /// <summary>
    /// Instruments
    /// </summary>
    [RoutePrefix("Instruments")]
    public class InstrumentsController : BaseApiController<Instrument>
    {
        protected override List<Expression<Func<Instrument, object>>> GetIncludes()
        {
            var list = base.GetIncludes();
            list.Add(i => i.Stations);
            list.Add(i => i.Sensors);
            return list;
        }

        /// <summary>
        /// all Instruments
        /// </summary>
        /// <returns>ListOf(Instrument)</returns>
        public override IQueryable<Instrument> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Instrument by Id
        /// </summary>
        /// <param name="id">The Id of the Instrument</param>
        /// <returns>Instrument</returns>
        [ResponseType(typeof(Instrument))]
        public override async Task<IHttpActionResult> GetById(Guid id)
        {
            return await base.GetById(id);
        }

        /// <summary>
        /// Instrument by Name
        /// </summary>
        /// <param name="name">The Name of the Instrument</param>
        /// <returns>Instrument</returns>
        [ResponseType(typeof(Instrument))]
        public override async Task<IHttpActionResult> GetByName(string name)
        {
            return await base.GetByName(name);
        }

        // GET: Instruments/5/Stations
        /// <summary>
        /// Stations linked to this Instrument
        /// </summary>
        /// <param name="id">Id of the Instrument</param>
        /// <returns>ListOf(Station)</returns>
        [Route("{id:guid}/Stations")]
        public IQueryable<Station> GetStations([FromUri] Guid id)
        {
            return GetMany<Station>(id, s => s.Stations, i => i.Instruments);
        }

        // GET: Instruments/5/Sensors
        /// <summary>
        /// Sensors linked to this Instrument
        /// </summary>
        /// <param name="id">Id of the Instrument</param>
        /// <returns>ListOf(Sensor)</returns>
        [Route("{id:guid}/Sensors")]
        public IQueryable<Sensor> GetSensors([FromUri] Guid id)
        {
            return GetMany<Sensor>(id, s => s.Sensors, i => i.Instruments);
        }

    }
}
