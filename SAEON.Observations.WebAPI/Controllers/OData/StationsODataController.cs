using System;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using SAEON.Observations.Core.Entities;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    /// <summary>
    /// Stations
    /// </summary>
    [ODataRoutePrefix("Stations")]
    public class StationsODataController : BaseODataController<Station>
    {
        /// <summary>
        /// All Stations
        /// </summary>
        /// <returns>ListOf(Station)</returns>
        [EnableQuery, ODataRoute]
        public override IQueryable<Station> GetAll()
        {
            return base.GetAll();
        }

        // GET: odata/Stations(5)
        /// <summary>
        /// Station by Id
        /// </summary>
        /// <param name="id">Id of Station</param>
        /// <returns>Station</returns>
        [EnableQuery, ODataRoute("({id})")]
        public override SingleResult<Station> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }


        // GET: odata/Stations(5)
        /// <summary>
        /// Station by Name
        /// </summary>
        /// <param name="name">Name of Station</param>
        /// <returns>Station</returns>
        [EnableQuery, ODataRoute("({name})")]
        public override SingleResult<Station> GetByName([FromODataUri] string name)
        {
            return base.GetByName(name);
        }

        // GET: odata/Stations(5)/Site
        /// <summary>
        /// Site for the Station
        /// </summary>
        /// <param name="id">Id of the Station</param>
        /// <returns>Site</returns>
        [EnableQuery, ODataRoute("({id})/Site")]
        public SingleResult<Site> GetSite([FromODataUri] Guid id)
        {
            return GetSingle(id, s => s.Site, i => i.Stations);
        }

        // GET: odata/Stations(5)/Instruments
        /// <summary>
        /// Instrumenst for the Station
        /// </summary>
        /// <param name="id">Id of the Station</param>
        /// <returns>ListOf(Instrument(</returns>
        [EnableQuery, ODataRoute("({id})/Instruments")]
        public IQueryable<Instrument> GetInstruments([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.Instruments, i => i.Stations);
        }
    }
}
