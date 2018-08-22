using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using SAEON.Observations.Core.Entities;
using System;
using System.Linq;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    /// <summary>
    /// Stations
    /// </summary>
    [ODataRoutePrefix("Stations")]
    public class StationsODController : BaseController<Station>
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

        // GET: odata/Stations(5)/Organisations
        /// <summary>
        /// Organisations for the Station
        /// </summary>
        /// <param name="id">Id of the Station</param>
        /// <returns>ListOf(Organisation(</returns>
        [EnableQuery, ODataRoute("({id})/Organisations")]
        public IQueryable<Organisation> GetOrganisations([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.Organisations, i => i.Stations);
        }

        // GET: odata/Stations(5)/Projects
        /// <summary>
        /// Projects for the Station
        /// </summary>
        /// <param name="id">Id of the Station</param>
        /// <returns>ListOf(Project(</returns>
        [EnableQuery, ODataRoute("({id})/Projects")]
        public IQueryable<Project> GetProjects([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.Projects, i => i.Stations);
        }

        // GET: odata/Stations(5)/Instruments
        /// <summary>
        /// Instruments for the Station
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
