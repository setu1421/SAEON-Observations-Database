using System;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using SAEON.Observations.Core;

namespace SAEON.Observations.WebAPI.Controllers
{
    /// <summary>
    /// Phenomenona
    /// </summary>
    [ODataRoutePrefix("Phenomena")]
    public class PhenomenaODataController : BaseODataController<Phenomenon>
    {
        // GET: odata/Phenomena
        /// <summary>
        /// Get a list of Phenomena
        /// </summary>
        /// <returns>A list of Phenomenon</returns>
        [EnableQuery, ODataRoute]
        public override IQueryable<Phenomenon> GetAll()
        {
            return base.GetAll();
        }

        // GET: odata/Phenomena(5)
        /// <summary>
        /// Get an Phenomenon by Id
        /// </summary>
        /// <param name="id">Id of Phenomenon</param>
        /// <returns>Phenomenon</returns>
        [EnableQuery, ODataRoute("({id})")]
        public override SingleResult<Phenomenon> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }

        // GET: odata/Phenomena(5)
        /// <summary>
        /// Get an Phenomenon by Name
        /// </summary>
        /// <param name="name">Name of Phenomenon</param>
        /// <returns>Phenomenon</returns>
        [EnableQuery, ODataRoute("({name})")]
        public override SingleResult<Phenomenon> GetByName([FromODataUri] string name)
        {
            return base.GetByName(name);
        }

        // GET: odata/Phenomena(5)/Sensors
        [EnableQuery, ODataRoute("({id})/Sensors")]
        public IQueryable<Sensor> GetSensors([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.Sensors, i => i.Phenomenon);
        }

        // GET: odata/Phenomena(5)/Offerings
        [EnableQuery, ODataRoute("({id})/Offerings")]
        public IQueryable<Offering> GetOfferings([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.Offerings, i => i.Phenomena);
        }

        // GET: odata/Phenomena(5)/UnitsOfMeasure
        [EnableQuery, ODataRoute("({id})/UnitsOfMeasure")]
        public IQueryable<UnitOfMeasure> GetUnitsOfMeasure([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.UnitsOfMeasure, i => i.Phenomena);
        }
    }
}
