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
    /// UnitOfMeasures
    /// </summary>
    [RoutePrefix("UnitsOfMeasure")]
    public class UnitsOfMeasureApiController : BaseApiController<UnitOfMeasure>
    {
        protected override List<Expression<Func<UnitOfMeasure, object>>> GetIncludes()
        {
            var list = base.GetIncludes();
            list.Add(i => i.Phenomena);
            return list;
        }

        /// <summary>
        /// All UnitsOfMeasure
        /// </summary>
        /// <returns>ListOf(UnitOfMeasure)</returns>
        public override IQueryable<UnitOfMeasure> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// UnitOfMeasure by Id
        /// </summary>
        /// <param name="id">The Id of the UnitOfMeasure</param>
        /// <returns>UnitOfMeasure</returns>
        [ResponseType(typeof(UnitOfMeasure))]
        public override async Task<IHttpActionResult> GetById(Guid id)
        {
            return await base.GetById(id);
        }

        /// <summary>
        /// UnitOfMeasure by Name
        /// </summary>
        /// <param name="name">The Name of the UnitOfMeasure</param>
        /// <returns>UnitOfMeasure</returns>
        [ResponseType(typeof(UnitOfMeasure))]
        public override async Task<IHttpActionResult> GetByName(string name)
        {
            return await base.GetByName(name);
        }

        // GET: UnitsOfMeasure/5/Phenomena
        /// <summary>
        /// Phenomena for the UnitOfMeaure
        /// </summary>
        /// <param name="id">Id of the UnitOfMeasure</param>
        /// <returns>ListOf(Phenomenon)</returns>
        [Route("{id:guid}/Phenomena")]
        public IQueryable<Phenomenon> GetPhenomena([FromUri] Guid id)
        {
            return GetMany<Phenomenon>(id, s => s.Phenomena, i => i.UnitsOfMeasure);
        }
    }
}
