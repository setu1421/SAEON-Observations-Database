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
    public class StationsODController : NamedController<Station>
    {
        /// <summary>
        /// All Stations
        /// </summary>
        /// <returns>ListOf(Station)</returns>
        [ODataRoute]
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
        [ODataRoute("({id})")]
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
        [ODataRoute("({id})/Site")]
        [EnableQuery(PageSize = PageSize, MaxTop = MaxTop)]
        public Site GetSite([FromODataUri] Guid id)
        {
            return GetSingle(id, s => s.Site);
        }

        // GET: odata/Stations(5)/Organisations
        /// <summary>
        /// Organisations for the Station
        /// </summary>
        /// <param name="id">Id of the Station</param>
        /// <returns>ListOf(Organisation)</returns>
        [ODataRoute("({id})/Organisations")]
        [EnableQuery(PageSize = PageSize, MaxTop = MaxTop)]
        public IQueryable<Organisation> GetOrganisations([FromODataUri] Guid id)
        {
            var site = GetSingle(id, s => s.Site);
            var siteOrganiations = DbContext.Sites.Where(i => i.Id == site.Id).SelectMany(i => i.Organisations);
            return GetManyWithGuidId(id, s => s.Organisations).Union(siteOrganiations); ;
        }

        // GET: odata/Stations(5)/Projects
        /// <summary>
        /// Projects for the Station
        /// </summary>
        /// <param name="id">Id of the Station</param>
        /// <returns>ListOf(Project)</returns>
        [ODataRoute("({id})/Projects")]
        [EnableQuery(PageSize = PageSize, MaxTop = MaxTop)]
        public IQueryable<Project> GetProjects([FromODataUri] Guid id)
        {
            return GetManyWithGuidId(id, s => s.Projects);
        }

        // GET: odata/Stations(5)/Instruments
        /// <summary>
        /// Instruments for the Station
        /// </summary>
        /// <param name="id">Id of the Station</param>
        /// <returns>ListOf(Instrument)</returns>
        [ODataRoute("({id})/Instruments")]
        [EnableQuery(PageSize = PageSize, MaxTop = MaxTop)]
        public IQueryable<Instrument> GetInstruments([FromODataUri] Guid id)
        {
            return GetManyWithGuidId(id, s => s.Instruments);
        }

        // GET: odata/Stations(5)/DataStreams
        /// <summary>
        /// DataStreams for the Station
        /// </summary>
        /// <param name="id">Id of the Station</param>
        /// <returns>ListOf(DataStream)</returns>
        [ODataRoute("({id})/DataStreams")]
        [EnableQuery(PageSize = PageSize, MaxTop = MaxTop)]
        public IQueryable<DataStream> GetDataStreams([FromODataUri] Guid id)
        {
            return GetManyWithLongId(id, s => s.DataStreams);
        }

        // GET: odata/Stations(5)/Observations
        /// <summary>
        /// Observations for the Station
        /// </summary>
        /// <param name="id">Id of the Station</param>
        /// <returns>ListOf(Observation))</returns>
        [ODataRoute("({id})/Observations")]
        [EnableQuery(PageSize = PageSize, MaxTop = MaxTop)]
        public IQueryable<Observation> GetObservations([FromODataUri] Guid id)
        {
            return GetManyWithIntId(id, s => s.Observations);
        }
    }
}
