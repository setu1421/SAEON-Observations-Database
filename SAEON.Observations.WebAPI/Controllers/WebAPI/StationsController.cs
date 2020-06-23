using SAEON.Observations.Core.Entities;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    /// <summary>
    /// </summary>
    [RoutePrefix("Api/Stations")]
    public class StationsController : CodedApiController<Station>
    {
        protected override IQueryable<Station> GetQuery(Expression<Func<Station, bool>> extraWhere = null)
        {
            return base.GetQuery(extraWhere).Include(i => i.Site).Include(i => i.Instruments);
        }

        /// <summary>
        /// All Stations
        /// </summary>
        /// <returns>ListOf(Station)</returns>
        public override IQueryable<Station> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Station by Id
        /// </summary>
        /// <param name="id">The Id of the Station</param>
        /// <returns>Station</returns>
        [ResponseType(typeof(Station))]
        public override async Task<IHttpActionResult> GetByIdAsync([FromUri] Guid id)
        {
            return await base.GetByIdAsync(id);
        }

        /// <summary>
        /// Station by Name
        /// </summary>
        /// <param name="name">The Name of the Station</param>
        /// <returns>Station</returns>
        [ResponseType(typeof(Station))]
        public override async Task<IHttpActionResult> GetByNameAsync([FromUri] string name)
        {
            return await base.GetByNameAsync(name);
        }

        /// <summary>
        /// Station by Code
        /// </summary>
        /// <param name="code">The Code of the Station</param>
        /// <returns>Station</returns>
        [ResponseType(typeof(Station))]
        public override async Task<IHttpActionResult> GetByCodeAsync([FromUri] string code)
        {
            return await base.GetByCodeAsync(code);
        }

        // GET: Stations/5/Site
        /// <summary>
        /// Site of a Station
        /// </summary>
        /// <param name="id">The Id of the Station</param>
        /// <returns>Site</returns>
        [Route("{id:guid}/Site")]
        [ResponseType(typeof(Site))]
        public async Task<IHttpActionResult> GetSiteAsync([FromUri] Guid id)
        {
            return await GetSingleAsync<Site>(id, s => s.Site);
        }

        // GET: Stations/5/Instruments
        /// <summary>
        /// Instruments of the Station
        /// </summary>
        /// <param name="id">Id of the Station</param>
        /// <returns>ListOf(Instrument)</returns>
        [Route("{id:guid}/Instruments")]
        public IQueryable<Instrument> GetInstruments([FromUri] Guid id)
        {
            return GetManyWithGuidId<Instrument>(id, s => s.Instruments);
        }

        // GET: Stations/5/Organisations
        /// <summary>
        /// Organisations of the Station
        /// </summary>
        /// <param name="id">Id of the Station</param>
        /// <returns>ListOf(Organisation)</returns>
        [Route("{id:guid}/Organisations")]
        public IQueryable<Organisation> GetOrganisations([FromUri] Guid id)
        {
            var site = GetSingle(id, s => s.Site);
            var siteOrganiations = DbContext.Sites.Where(i => i.Id == site.Id).SelectMany(i => i.Organisations);
            return GetManyWithGuidId(id, s => s.Organisations).Union(siteOrganiations); ;
        }

        // GET: Stations/5/Projects
        /// <summary>
        /// Projects of the Station
        /// </summary>
        /// <param name="id">Id of the Station</param>
        /// <returns>ListOf(Project)</returns>
        [Route("{id:guid}/Projects")]
        public IQueryable<Project> GetProjects([FromUri] Guid id)
        {
            return GetManyWithGuidId<Project>(id, s => s.Projects);
        }

        // GET: Stations/5/DataStreams
        /// <summary>
        /// DataStreams of the Station
        /// </summary>
        /// <param name="id">Id of the Station</param>
        /// <returns>ListOf(DataStream)</returns>
        [Route("{id:guid}/DataStreams")]
        public IQueryable<DataStream> GetDataStreams([FromUri] Guid id)
        {
            return GetManyWithLongId<DataStream>(id, s => s.DataStreams);
        }

        // GET: Stations/5/Observations
        /// <summary>
        /// Observations of the Station
        /// </summary>
        /// <param name="id">Id of the Station</param>
        /// <param name="phenomenonId">PhenomenonId of the Observations</param>
        /// <param name="offeringId">OfferingId of the Observations</param>
        /// <param name="unitId">UnitId of the Observations</param>
        /// <returns>ListOf(Observation)</returns>
        [Route("{id:guid}/Observations/{phenomenonId:guid}/{offeringId:guid}/{unitId:guid}")]
        public IQueryable<Observation> GetObservations([FromUri] Guid id, [FromUri] Guid phenomenonId, [FromUri] Guid offeringId, [FromUri] Guid unitId)
        {
            return GetManyWithIntId<Observation>(id, s => s.Observations).Where(i => (i.PhenomenonId == phenomenonId) && (i.OfferingId == offeringId) && (i.UnitId == unitId));
        }

    }
}
