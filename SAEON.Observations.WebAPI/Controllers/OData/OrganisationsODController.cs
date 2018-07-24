using SAEON.Observations.Core.Entities;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    /// <summary>
    /// Organisations
    /// </summary>
    [ODataRoutePrefix("Organisations")]
    public class OrganisationsODController : BaseController<Organisation>
    {
        /// <summary>
        /// All Organisations
        /// </summary>
        /// <returns>ListOf(Organisation)</returns>
        [EnableQuery, ODataRoute]
        public override IQueryable<Organisation> GetAll()
        {
            return base.GetAll();
        }

        // GET: odata/Organisations(5)
        /// <summary>
        /// Organisation by Id
        /// </summary>
        /// <param name="id">Id of Organisation</param>
        /// <returns>Organisation</returns>
        [EnableQuery, ODataRoute("({id})")]
        public override SingleResult<Organisation> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }


        //GET: odata/Organisations(5)/Sites
        /// <summary>
        /// Sites for the Organisation
        /// </summary>
        /// <param name="id">Id of Organisation</param>
        /// <returns>ListOf(Site)</returns>
        [EnableQuery, ODataRoute("({id})/Sites")]
        public IQueryable<Site> GetSites([FromODataUri] Guid id)
        {
            return GetMany<Site>(id, s => s.Sites, i => i.Organisations);
        }

        //GET: odata/Organisations(5)/Stations
        /// <summary>
        /// Stations for the Organisation
        /// </summary>
        /// <param name="id">Id of Organisation</param>
        /// <returns>ListOf(Station)</returns>
        [EnableQuery, ODataRoute("({id})/Stations")]
        public IQueryable<Station> GetStations([FromODataUri] Guid id)
        {
            return GetMany<Station>(id, s => s.Stations, i => i.Organisations);
        }

        //GET: odata/Organisations(5)/Instruments
        /// <summary>
        /// Instruments for the Organisation
        /// </summary>
        /// <param name="id">Id of Organisation</param>
        /// <returns>ListOf(Instrument)</returns>
        [EnableQuery, ODataRoute("({id})/Instruments")]
        public IQueryable<Instrument> GetInstruments([FromODataUri] Guid id)
        {
            return GetMany<Instrument>(id, s => s.Instruments, i => i.Organisations);
        }
    }
}