using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        /// <summary>
        /// Return a UnitOfMeasure by Id
        /// </summary>
        /// <param name="id">The Id of the UnitOfMeasure</param>
        /// <returns>UnitOfMeasure</returns>
        [ResponseType(typeof(UnitOfMeasure))]
        public override async Task<IHttpActionResult> GetById(Guid id)
        {
            return await base.GetById(id);
        }

        /// <summary>
        /// Return a UnitOfMeasure by Name
        /// </summary>
        /// <param name="name">The Name of the UnitOfMeasure</param>
        /// <returns>UnitOfMeasure</returns>
        [ResponseType(typeof(UnitOfMeasure))]
        public override async Task<IHttpActionResult> GetByName(string name)
        {
            return await base.GetByName(name);
        }

    }
}
