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
    /// </summary>
    [RoutePrefix("Api/Instruments")]
    public class InstrumentsController : CodedApiController<Instrument>
    {
        protected override List<Expression<Func<Instrument, object>>> GetIncludes()
        {
            var list = base.GetIncludes();
            list.Add(i => i.Stations);
            list.Add(i => i.Sensors);
            return list;
        }

        /// <summary>
        /// All Instruments
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
        public override async Task<IHttpActionResult> GetByIdAsync([FromUri] Guid id)
        {
            return await base.GetByIdAsync(id);
        }

        /// <summary>
        /// Instrument by Name
        /// </summary>
        /// <param name="name">The Name of the Instrument</param>
        /// <returns>Instrument</returns>
        [ResponseType(typeof(Instrument))]
        public override async Task<IHttpActionResult> GetByNameAsync([FromUri] string name)
        {
            return await base.GetByNameAsync(name);
        }

        /// <summary>
        /// Instrument by Code
        /// </summary>
        /// <param name="code">The Code of the Instrument</param>
        /// <returns>Instrument</returns>
        [ResponseType(typeof(Instrument))]
        public override async Task<IHttpActionResult> GetByCodeAsync([FromUri] string code)
        {
            return await base.GetByCodeAsync(code);
        }

        // GET: Instruments/5/Organisations
        /// <summary>
        /// Organisations linked to this Instrument
        /// </summary>
        /// <param name="id">Id of the Instrument</param>
        /// <returns>ListOf(Organisation)</returns>
        [Route("{id:guid}/Organisations")]
        public IQueryable<Organisation> Getorganisations([FromUri] Guid id)
        {
            return GetMany<Organisation>(id, s => s.Organisations, i => i.Instruments);
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
