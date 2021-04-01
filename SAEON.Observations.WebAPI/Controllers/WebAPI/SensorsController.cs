using Microsoft.AspNetCore.Mvc;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    public class SensorsController : CodedNamedApiController<Sensor>
    {
        //protected override IQueryable<Sensor> GetQuery(Expression<Func<Sensor, bool>> extraWhere = null)
        //{
        //    return base.GetQuery(extraWhere).Include(i => i.Phenomenon).Include(i => i.Instruments);
        //}

        /// <summary>
        /// All Sensors
        /// </summary>
        /// <returns>ListOf(Sensor)</returns>
        public override Task<IEnumerable<Sensor>> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Sensor by Id
        /// </summary>
        /// <param name="id">The Id of the Sensor</param>
        /// <returns>Sensor</returns>
        public override Task<ActionResult<Sensor>> GetByIdAsync(Guid id)
        {
            return base.GetByIdAsync(id);
        }

        /// <summary>
        /// Sensor by Code
        /// </summary>
        /// <param name="code">The Code of the Sensor</param>
        /// <returns>Sensor</returns>
        public override Task<ActionResult<Sensor>> GetByCodeAsync(string code)
        {
            return base.GetByCodeAsync(code);
        }

        /// <summary>
        /// Sensor by Name
        /// </summary>
        /// <param name="name">The Name of the Sensor</param>
        /// <returns>Sensor</returns>
        public override Task<ActionResult<Sensor>> GetByNameAsync(string name)
        {
            return base.GetByNameAsync(name);
        }

        /// <summary>
        /// Phenomenon of the Sensor
        /// </summary>
        /// <param name="id">The Id of the Sensor</param>
        /// <returns>Phenomenon</returns>
        [HttpGet("{id:guid}/Phenomenon")]
        public async Task<ActionResult<Phenomenon>> GetPhenomenonAsync(Guid id)
        {
            return await GetSingleAsync<Phenomenon>(id, s => s.Phenomenon);
        }

        /// <summary>
        /// Instruments of the Sensor
        /// </summary>
        /// <param name="id">Id of the Sensor</param>
        /// <returns>ListOf(Instrument)</returns>
        [HttpGet("{id:guid}/Instruments")]
        public IQueryable<Instrument> GetInstruments(Guid id)
        {
            return GetManyWithGuidId<Instrument>(id, s => s.Instruments);
        }

        /*  @@
        /// <summary>
        /// Observations of the Sensor
        /// </summary>
        /// <param name="id">Id of the Sensor</param>
        /// <returns>ListOf(Observation)</returns>
        [HttpGet("{id:guid}/Observations")]
        [Authorize]
        [DenyClientAuthorization(Constants.ClientIdPostman, Constants.ClientIdSwagger)]
        public IQueryable<SensorObservation> GetObservations(Guid id)
        {
            return GetManyWithIntId<SensorObservation>(id, s => s.SensorObservations);
        }
        */
    }
}
