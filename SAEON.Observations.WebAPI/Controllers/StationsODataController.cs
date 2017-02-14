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
using System.Web.Http.Description;
using Serilog.Context;
using Serilog;

namespace SAEON.Observations.WebAPI.Controllers
{
    /// <summary>
    /// Stations
    /// </summary>
    [ODataRoutePrefix("Stations")]
    public class StationsODataController : BaseODataController<Station>
    {
        /// <summary>
        /// Get a list of Stations
        /// </summary>
        /// <returns>A list of Station</returns>
        [EnableQuery, ODataRoute]
        public override IQueryable<Station> GetAll()
        {
            return base.GetAll();
        }

        // GET: odata/Stations(5)
        /// <summary>
        /// Get a Station by Id
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
        /// Get a Station by Name
        /// </summary>
        /// <param name="name">Name of Station</param>
        /// <returns>Station</returns>
        [EnableQuery, ODataRoute("({name})")]
        public override SingleResult<Station> GetByName([FromODataUri] string name)
        {
            return base.GetByName(name);
        }

        // GET: odata/Stations(5)/Site
        [EnableQuery, ODataRoute("({id})/Site")]
        public SingleResult<Site> GetSite([FromODataUri] Guid id)
        {
            return GetSingle(id, s => s.Site, i => i.Stations);
        }

        // GET: odata/Stations(5)/Instruments
        [EnableQuery, ODataRoute("({id})/Instruments")]
        public IQueryable<Instrument> GetInstruments([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.Instruments, i => i.Stations);
        }
    }
}
