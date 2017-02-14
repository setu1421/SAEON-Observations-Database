using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.ModelBinding;
using System.Web.OData;
using System.Web.OData.Query;
using System.Web.OData.Routing;
using SAEON.Observations.Core;

namespace SAEON.Observations.WebAPI.Controllers
{
    /// <summary>
    /// Sensors
    /// </summary>
    [ODataRoutePrefix("Sensors")]
    public class SensorsODataController : BaseODataController<Sensor>
    {

        // GET: odata/Sensors
        /// <summary>
        /// Get a list of Sensors
        /// </summary>
        /// <returns>A list of Sensor</returns>
        [EnableQuery, ODataRoute]
        public override IQueryable<Sensor> GetAll()
        {
            return base.GetAll();
        }

        // GET: odata/Sensors(5)
        /// <summary>
        /// Get an Sensor by Id
        /// </summary>
        /// <param name="id">Id of Sensor</param>
        /// <returns>Sensor</returns>
        [EnableQuery, ODataRoute("({id})")]
        public override SingleResult<Sensor> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }

        // GET: odata/Sensors(5)
        /// <summary>
        /// Get an Sensor by Name
        /// </summary>
        /// <param name="name">Name of Sensor</param>
        /// <returns>Sensor</returns>
        [EnableQuery, ODataRoute("({name})")]
        public override SingleResult<Sensor> GetByName([FromODataUri] string name)
        {
            return base.GetByName(name);
        }

        // GET: odata/Sensors(5)/Instruments
        [EnableQuery, ODataRoute("({id})/Instruments")]
        public IQueryable<Instrument> GetInstruments([FromODataUri] Guid id)
        {
            return GetMany<Instrument>(id, s => s.Instruments, i => i.Sensors);
        }

        // GET: odata/Sensors(5)/Phenomenon
        [EnableQuery, ODataRoute("({id})/Phenomenon")]
        public SingleResult<Phenomenon> GetPhenomenon([FromODataUri] Guid id)
        {
            return GetSingle(id, s => s.Phenomenon, i => i.Sensors);
        }

    }
}
