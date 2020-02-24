using SAEON.Observations.Core.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    /// <summary>
    /// </summary>
    [RoutePrefix("Api/Sensors")]
    public class SensorsController : CodedApiController<Sensor>
    {
        //protected override List<Expression<Func<Sensor, object>>> GetIncludes()
        //{
        //    var list = base.GetIncludes();
        //    list.Add(i => i.Instruments);
        //    list.Add(i => i.Phenomenon);
        //    return list;
        //}

        /// <summary>
        /// All Sensors
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
        public override async Task<IHttpActionResult> GetByIdAsync([FromUri] Guid id)
        {
            return await base.GetByIdAsync(id);
        }

        /// <summary>
        /// Sensor by Code
        /// </summary>
        /// <param name="code">The Code of the Sensor</param>
        /// <returns>Sensor</returns>
        [ResponseType(typeof(Sensor))]
        public override async Task<IHttpActionResult> GetByCodeAsync([FromUri] string code)
        {
            return await base.GetByCodeAsync(code);
        }


        /// <summary>
        /// Sensor by Name
        /// </summary>
        /// <param name="name">The Name of the Sensor</param>
        /// <returns>Sensor</returns>
        [ResponseType(typeof(Sensor))]
        public override async Task<IHttpActionResult> GetByNameAsync([FromUri] string name)
        {
            return await base.GetByNameAsync(name);
        }


        // GET: Sensors/5/Phenomenon
        /// <summary>
        /// Phenomenon of a Sensor
        /// </summary>
        /// <param name="id">The Id of the Sensor</param>
        /// <returns>Phenomenon</returns>
        [Route("{id:guid}/Phenomenon")]
        [ResponseType(typeof(Phenomenon))]
        public async Task<IHttpActionResult> GetPhenomenonAsync(Guid id)
        {
            return await GetSingleAsync<Phenomenon>(id, s => s.Phenomenon);
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
            return GetMany<Instrument>(id, s => s.Instruments);
        }

    }
}
