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
    [RoutePrefix("Api/Phenomena")]
    public class PhenomenaController : CodedApiController<Phenomenon>
    {
        //protected override List<Expression<Func<Phenomenon, object>>> GetIncludes()
        //{
        //    var list = base.GetIncludes();
        //    list.Add(i => i.Offerings);
        //    list.Add(i => i.Units);
        //    list.Add(i => i.Sensors);
        //    return list;
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
        [ResponseType(typeof(Phenomenon))]
        public override async Task<IHttpActionResult> GetByIdAsync([FromUri] Guid id)
        {
            return await base.GetByIdAsync(id);
        }

        /// <summary>
        /// Phenomenon by Code
        /// </summary>
        /// <param name="code">The Code of the Phenomenon</param>
        /// <returns>Phenomenon</returns>
        [ResponseType(typeof(Phenomenon))]
        public override async Task<IHttpActionResult> GetByCodeAsync([FromUri] string code)
        {
            return await base.GetByCodeAsync(code);
        }

        /// <summary>
        /// Phenomenon by Name
        /// </summary>
        /// <param name="name">The Name of the Phenomenon</param>
        /// <returns>Phenomenon</returns>
        [ResponseType(typeof(Phenomenon))]
        public override async Task<IHttpActionResult> GetByNameAsync([FromUri] string name)
        {
            return await base.GetByNameAsync(name);
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
            return GetManyWithGuidId<PhenomenonOffering>(id, s => s.PhenomenonOfferings).Select(i => i.Offering);
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
            return GetManyWithGuidId<PhenomenonUnit>(id, s => s.PhenomenonUnits).Select(i => i.Unit);
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
            return GetManyWithGuidId<Sensor>(id, s => s.Sensors);
        }

    }
}
