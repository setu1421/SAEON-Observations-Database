using Microsoft.AspNetCore.Mvc;
using SAEON.Observations.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    public class PhenomenaController : CodedNamedApiController<Phenomenon>
    {
        //protected override IQueryable<Phenomenon> GetQuery(Expression<Func<Phenomenon, bool>> extraWhere = null)
        //{
        //    return base.GetQuery(extraWhere);
        //}

        /// <summary>
        /// All Phenomena
        /// </summary>
        /// <returns>ListOf(Phenomenon)</returns>
        public override IQueryable<Phenomenon> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Phenomenon by Id
        /// </summary>
        /// <param name="id">The Id of the Phenomenon</param>
        /// <returns>Phenomenon</returns>
        public override Task<ActionResult<Phenomenon>> GetByIdAsync(Guid id)
        {
            return base.GetByIdAsync(id);
        }

        /// <summary>
        /// Phenomenon by Code
        /// </summary>
        /// <param name="code">The Code of the Phenomenon</param>
        /// <returns>Phenomenon</returns>
        public override Task<ActionResult<Phenomenon>> GetByCodeAsync(string code)
        {
            return base.GetByCodeAsync(code);
        }

        /// <summary>
        /// Phenomenon by Name
        /// </summary>
        /// <param name="name">The Name of the Phenomenon</param>
        /// <returns>Phenomenon</returns>
        public override Task<ActionResult<Phenomenon>> GetByNameAsync(string name)
        {
            return base.GetByNameAsync(name);
        }

        /// <summary>
        /// Offerings of the Phenomenon
        /// </summary>
        /// <param name="id">Id of the Phenomenon</param>
        /// <returns>ListOf(Offering)</returns>
        [HttpGet("{id:guid}/Offerings")]
        public IQueryable<Offering> GetOfferings(Guid id)
        {
            return GetManyWithGuidId<Offering>(id, s => s.Offerings);
        }

        /// <summary>
        /// Units of the Phenomenon
        /// </summary>
        /// <param name="id">Id of the Phenomenon</param>
        /// <returns>ListOf(Unit)</returns>
        [HttpGet("{id:guid}/Units")]
        public IQueryable<Unit> GetUnits(Guid id)
        {
            return GetManyWithGuidId<Unit>(id, s => s.Units);
        }

        /// <summary>
        /// Sensors of the Phenomenon
        /// </summary>
        /// <param name="id">Id of the Phenomenon</param>
        /// <returns>ListOf(Sensor)</returns>
        [HttpGet("{id:guid}/Sensors")]
        public IQueryable<Sensor> GetSensors(Guid id)
        {
            return GetManyWithGuidId<Sensor>(id, s => s.Sensors);
        }
    }
}
