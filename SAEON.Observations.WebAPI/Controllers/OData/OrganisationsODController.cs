using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using SAEON.Observations.Core.Entities;
using System;
using System.Linq;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    /// <summary>
    /// Organisations
    /// </summary>
    [ODataRoutePrefix("Organisations")]
    public class OrganisationsODController : NamedController<Organisation>
    {
        /// <summary>
        /// All Organisations
        /// </summary>
        /// <returns>ListOf(Organisation)</returns>
        [ODataRoute]
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
        [ODataRoute("({id})")]
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
        [ODataRoute("({id})/Sites")]
        public IQueryable<Site> GetSites([FromODataUri] Guid id)
        {
            return GetMany<Site>(id, s => s.Sites);
        }

        //GET: odata/Organisations(5)/Stations
        /// <summary>
        /// Stations for the Organisation
        /// </summary>
        /// <param name="id">Id of Organisation</param>
        /// <returns>ListOf(Station)</returns>
        [ODataRoute("({id})/Stations")]
        public IQueryable<Station> GetStations([FromODataUri] Guid id)
        {
            var siteStations = GetMany<Site>(id, s => s.Sites).SelectMany(i => i.Stations);
            return GetMany<Station>(id, s => s.Stations).Union(siteStations);
        }

        //GET: odata/Organisations(5)/Instruments
        /// <summary>
        /// Instruments for the Organisation
        /// </summary>
        /// <param name="id">Id of Organisation</param>
        /// <returns>ListOf(Instrument)</returns>
        [ODataRoute("({id})/Instruments")]
        public IQueryable<Instrument> GetInstruments([FromODataUri] Guid id)
        {
            var siteInstruments = GetMany<Site>(id, s => s.Sites).SelectMany(i => i.Stations).SelectMany(i => i.Instruments);
            var stationInstruments = GetMany<Station>(id, s => s.Stations).SelectMany(i => i.Instruments);
            return GetMany<Instrument>(id, s => s.Instruments).Union(siteInstruments).Union(stationInstruments);
        }
    }
}