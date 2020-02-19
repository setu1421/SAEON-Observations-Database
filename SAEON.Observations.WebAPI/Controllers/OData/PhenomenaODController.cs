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
        public IQueryable<Offering> GetOfferings([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.Offerings);
        }

        // GET: odata/Phenomena(5)/Units
        /// <summary>
        /// Units for the Phenomenon
        /// </summary>
        /// <param name="id">Id of the Phenomenon</param>
        /// <returns>ListOf(Unit)</returns>
        [ODataRoute("({id})/Units")]
        public IQueryable<Unit> GetUnits([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.Units);
        }

        // GET: odata/Phenomena(5)/Sensors
        /// <summary>
        /// Sensors for the Phenomenon
        /// </summary>
        /// <param name="id">Id of the Phenomenon</param>
        /// <returns>ListOf(Sensor)</returns>
        [ODataRoute("({id})/Sensors")]
        public IQueryable<Sensor> GetSensors([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.Sensors);
        }
    }
}
