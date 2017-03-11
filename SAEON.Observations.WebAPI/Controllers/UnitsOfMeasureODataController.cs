using System;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using SAEON.Observations.Core;

namespace SAEON.Observations.WebAPI.Controllers
{
    /// <summary>
    /// UnitsOfMeasure
    /// </summary>
    [ODataRoutePrefix("UnitsOfMeasure")]
    public class UnitsOfMeasureODataController : BaseODataController<UnitOfMeasure>
    {

        // GET: odata/UnitsOfMeasure
        /// <summary>
        /// All UnitsOfMeasure
        /// </summary>
        /// <returns>ListOf(UnitOfMeasure)</returns>
        [EnableQuery, ODataRoute]
        public override IQueryable<UnitOfMeasure> GetAll()
        {
            return base.GetAll();
        }

        // GET: odata/UnitsOfMeasure(5)
        /// <summary>
        /// UnitOfMeasure by Id
        /// </summary>
        /// <param name="id">Id of UnitOfMeasure</param>
        /// <returns>UnitOfMeasure</returns>
        [EnableQuery, ODataRoute("({id})")]
        public override SingleResult<UnitOfMeasure> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }

        // GET: odata/UnitsOfMeasure(5)
        /// <summary>
        /// UnitOfMeasure by Name
        /// </summary>
        /// <param name="name">Name of UnitOfMeasure</param>
        /// <returns>UnitOfMeasure</returns>
        [EnableQuery, ODataRoute("({name})")]
        public override SingleResult<UnitOfMeasure> GetByName([FromODataUri] string name)
        {
            return base.GetByName(name);
        }

        // GET: odata/UnitsOfMeasure(5)/Phenomena
        /// <summary>
        /// Phenomena for the UnitOfMeasure
        /// </summary>
        /// <param name="id">Id of the UnitOfMeasure</param>
        /// <returns>ListOf(Phenomenon)</returns>
        [EnableQuery, ODataRoute("({id})/Phenomena")]
        public IQueryable<Phenomenon> GetPhenomena([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.Phenomena, i => i.UnitsOfMeasure);
        }
    }
}
