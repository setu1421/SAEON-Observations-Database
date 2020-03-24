using SAEON.AspNet.WebApi;
using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
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
        protected override IQueryable<Sensor> GetQuery(Expression<Func<Sensor, bool>> extraWhere = null)
        {
            return base.GetQuery(extraWhere).Include(i => i.Phenomenon).Include(i => i.Instruments);
        }

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
            return GetManyWithGuidId<Instrument>(id, s => s.Instruments);
        }

        // GET: Sensors/5/Observations
        /// <summary>
        /// Observations for the Sensor
        /// </summary>
        /// <param name="id">Id of the Sensor</param>
        /// <returns>ListOf(Observation)</returns>
        [Route("{id:guid}/Observations")]
        [Authorize]
        [DenyClientAuthorization(Constants.ClientIdPostman, Constants.ClientIdSwagger)]
        public IQueryable<ObservationApi> GetObservations([FromUri] Guid id)
        {
            return GetManyWithIntId<ObservationApi>(id, s => s.ObservationsApi);
        }

    }

}

