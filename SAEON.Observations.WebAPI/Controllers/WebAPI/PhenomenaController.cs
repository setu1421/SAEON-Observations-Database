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
    [RoutePrefix("Api/Phenomena")]
    public class PhenomenaController : CodedApiController<Phenomenon>
    {
        protected override List<Expression<Func<Phenomenon, object>>> GetIncludes()
        {
            var list = base.GetIncludes();
            list.Add(i => i.Offerings);
            list.Add(i => i.Units);
            list.Add(i => i.Sensors);
            return list;
        }

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
        [ResponseType(typeof(Phenomenon))]
        public override async Task<IHttpActionResult> GetById([FromUri] Guid id)
        {
            return await base.GetById(id);
        }

        /// <summary>
        /// Phenomenon by Code
        /// </summary>
        /// <param name="code">The Code of the Phenomenon</param>
        /// <returns>Phenomenon</returns>
        [ResponseType(typeof(Phenomenon))]
        public override async Task<IHttpActionResult> GetByCode([FromUri] string code)
        {
            return await base.GetByCode(code);
        }

        /// <summary>
        /// Phenomenon by Name
        /// </summary>
        /// <param name="name">The Name of the Phenomenon</param>
        /// <returns>Phenomenon</returns>
        [ResponseType(typeof(Phenomenon))]
        public override async Task<IHttpActionResult> GetByName([FromUri] string name)
        {
            return await base.GetByName(name);
        }

        // GET: Phenomena/5/Offerings
        /// <summary>
        /// Offerings for the Phenomenon
        /// </summary>
        /// <param name="id">Id of the Phenomenon</param>
        /// <returns>ListOf(Offering)</returns>
        [Route("{id:guid}/Offerings")]
        public IQueryable<Offering> GetOfferings([FromUri] Guid id)
        {
            return GetMany<Offering>(id, s => s.Offerings, i => i.Phenomena);
        }

        // GET: Phenomena/5/Units
        /// <summary>
        /// Units for the Phenomenon
        /// </summary>
        /// <param name="id">Id of the Phenomenon</param>
        /// <returns>ListOf(Unit)</returns>
        [Route("{id:guid}/Units")]
        public IQueryable<Unit> GetUnits([FromUri] Guid id)
        {
            return GetMany<Unit>(id, s => s.Units, i => i.Phenomena);
        }

        // GET: Phenomena/5/Sensors
        /// <summary>
        /// Sensors for the Phenomenon
        /// </summary>
        /// <param name="id">Id of the Phenomenon</param>
        /// <returns>ListOf(Sensor)</returns>
        [Route("{id:guid}/Sensors")]
        public IQueryable<Sensor> GetSensors([FromUri] Guid id)
        {
            return GetMany<Sensor>(id, s => s.Sensors, i => i.Phenomenon);
        }

    }
}
