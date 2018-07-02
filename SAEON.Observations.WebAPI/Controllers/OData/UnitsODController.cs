using System;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using SAEON.Observations.Core.Entities;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    /// <summary>
    /// Units
    /// </summary>
    [ODataRoutePrefix("Units")]
    public class UnitsODController : BaseODataController<Unit>
    {

        // GET: odata/Units
        /// <summary>
        /// All Units
        /// </summary>
        /// <returns>ListOf(Unit)</returns>
        [EnableQuery, ODataRoute]
        public override IQueryable<Unit> GetAll()
        {
            return base.GetAll();
        }

        // GET: odata/Units(5)
        /// <summary>
        /// Unit by Id
        /// </summary>
        /// <param name="id">Id of Unit</param>
        /// <returns>Unit</returns>
        [EnableQuery, ODataRoute("({id})")]
        public override SingleResult<Unit> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }

        // GET: odata/Units(5)/Phenomena
        /// <summary>
        /// Phenomena for the Unit
        /// </summary>
        /// <param name="id">Id of the Unit</param>
        /// <returns>ListOf(Phenomenon)</returns>
        [EnableQuery, ODataRoute("({id})/Phenomena")]
        public IQueryable<Phenomenon> GetPhenomena([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.Phenomena, i => i.Units);
        }
    }
}
