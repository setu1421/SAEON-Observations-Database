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
    /// Instruments
    /// </summary>
    [ODataRoutePrefix("Instruments")]
    public class InstrumentsODataController : BaseODataController<Instrument>
    {

        // GET: odata/Instruments
        /// <summary>
        /// Get a list of Instruments
        /// </summary>
        /// <returns>A list of Instrument</returns>
        [EnableQuery, ODataRoute]
        public override IQueryable<Instrument> GetAll()
        {
            return base.GetAll();
        }

        // GET: odata/Instruments(5)
        /// <summary>
        /// Get an Instrument by Id
        /// </summary>
        /// <param name="id">Id of Instrument</param>
        /// <returns>Instrument</returns>
        [EnableQuery, ODataRoute]
        public override SingleResult<Instrument> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }

        // GET: odata/Instruments(5)
        /// <summary>
        /// Get an Instrument by Name
        /// </summary>
        /// <param name="name">Name of Instrument</param>
        /// <returns>Instrument</returns>
        [EnableQuery, ODataRoute]
        public override SingleResult<Instrument> GetByName([FromODataUri] string name)
        {
            return base.GetByName(name);
        }

        // GET: odata/Instruments(5)/Stations
        [EnableQuery]
        public IQueryable<Station> GetStations([FromODataUri] Guid key)
        {
            return db.Instruments.Where(m => m.Id == key).SelectMany(m => m.Stations);
        }

    }
}
