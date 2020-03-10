using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using SAEON.Observations.Core.Entities;
using System;
using System.Linq;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    /// <summary>
    /// Phenomenona
    /// </summary>
    [ODataRoutePrefix("Phenomena")]
    public class PhenomenaODController : NamedController<Phenomenon>
    {
        // GET: odata/Phenomena
        /// <summary>
        /// Get all Phenomena
        /// </summary>
        /// <returns>ListOf(Phenomenon)</returns>
        [ODataRoute]
        public override IQueryable<Phenomenon> GetAll()
        {
            return base.GetAll();
        }

        // GET: odata/Phenomena(5)
        /// <summary>
        /// Phenomenon by Id
        /// </summary>
        /// <param name="id">Id of Phenomenon</param>
        /// <returns>Phenomenon</returns>
        [ODataRoute("({id})")]
        public override SingleResult<Phenomenon> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }

        // GET: odata/Phenomena(5)/Offerings
        /// <summary>
        /// Offerings for the Phenomenon
        /// </summary>
        /// <param name="id">Id of the Phenomenon</param>
        /// <returns>ListOf(Offering)</returns>
        [ODataRoute("({id})/Offerings")]
        [EnableQuery(PageSize = PageSize, MaxTop = MaxTop)]
        public IQueryable<Offering> GetOfferings([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.PhenomenonOfferings).Select(i => i.Offering);
        }

        // GET: odata/Phenomena(5)/Units
        /// <summary>
        /// Units for the Phenomenon
        /// </summary>
        /// <param name="id">Id of the Phenomenon</param>
        /// <returns>ListOf(Unit)</returns>
        [ODataRoute("({id})/Units")]
        [EnableQuery(PageSize = PageSize, MaxTop = MaxTop)]
        public IQueryable<Unit> GetUnits([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.PhenomenonUnits).Select(i => i.Unit);
        }

        // GET: odata/Phenomena(5)/Sensors
        /// <summary>
        /// Sensors for the Phenomenon
        /// </summary>
        /// <param name="id">Id of the Phenomenon</param>
        /// <returns>ListOf(Sensor)</returns>
        [ODataRoute("({id})/Sensors")]
        [EnableQuery(PageSize = PageSize, MaxTop = MaxTop)]
        public IQueryable<Sensor> GetSensors([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.Sensors);
        }
    }
}
