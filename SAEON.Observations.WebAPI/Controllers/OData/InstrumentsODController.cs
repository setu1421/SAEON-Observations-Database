using System;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using SAEON.Observations.Core.Entities;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    /// <summary>
    /// Instruments
    /// </summary>
    [ODataRoutePrefix("Instruments")]
    public class InstrumentsODController : BaseODataController<Instrument>
    {

        // GET: odata/Instruments
        /// <summary>
        /// Get all Instruments
        /// </summary>
        /// <returns>ListOf(Instrument)</returns>
        [EnableQuery, ODataRoute]
        public override IQueryable<Instrument> GetAll()
        {
            return base.GetAll();
        }

        // GET: odata/Instruments(5)
        /// <summary>
        /// Instrument by Id
        /// </summary>
        /// <param name="id">Id of Instrument</param>
        /// <returns>Instrument</returns>
        [EnableQuery, ODataRoute("({id})")]
        public override SingleResult<Instrument> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }

        // GET: odata/Instruments(5)/Organisations
        /// <summary>
        /// Get Organisations for the Instrument
        /// </summary>
        /// <param name="id">Id of Instrument</param>
        /// <returns>ListOf(Organisation)</returns>
        [EnableQuery, ODataRoute("({id})/Organisations")]
        public IQueryable<Organisation> GetOrganisations([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.Organisations, i => i.Instruments);
        }

        // GET: odata/Instruments(5)/Stations
        /// <summary>
        /// Get Stations for the Instrument
        /// </summary>
        /// <param name="id">Id of Instrument</param>
        /// <returns>ListOf(Station)</returns>
        [EnableQuery, ODataRoute("({id})/Stations")]
        public IQueryable<Station> GetStations([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.Stations, i => i.Instruments);
        }

        // GET: odata/Instruments(5)/Sensors
        /// <summary>
        /// Get Sensors for the Instrument
        /// </summary>
        /// <param name="id">Id of Instrument</param>
        /// <returns>ListOf(Sensor)</returns>
        [EnableQuery, ODataRoute("({id})/Sensors")]
        public IQueryable<Sensor> GetSensors([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.Sensors, i => i.Instruments);
        }

    }
}
