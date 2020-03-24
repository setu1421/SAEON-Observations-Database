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
    [RoutePrefix("Api/Organisations")]
    public class OrganisationsController : CodedApiController<Organisation>
    {
        protected override IQueryable<Organisation> GetQuery(Expression<Func<Organisation, bool>> extraWhere = null)
        {
            return base.GetQuery(extraWhere)
                .Include(i => i.Sites.Select(s => s.Stations))
                .Include(i => i.Sites.Select(s => s.Stations.Select(ss => ss.Instruments)))
                .Include(i => i.Stations.Select(s => s.Instruments))
                .Include(i => i.Instruments);
        }

        /// <summary>
        /// All Organisations
        /// </summary>
        /// <returns>ListOf(Organisation)</returns>
        public override IQueryable<Organisation> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Organisation by Id
        /// </summary>
        /// <param name="id">The Id of the Organisation</param>
        /// <returns>Organisation</returns>
        [ResponseType(typeof(Organisation))]
        public override async Task<IHttpActionResult> GetByIdAsync([FromUri] Guid id)
        {
            return await base.GetByIdAsync(id);
        }

        /// <summary>
        /// Organisation by Name
        /// </summary>
        /// <param name="name">The Name of the Organisation</param>
        /// <returns>Organisation</returns>
        [ResponseType(typeof(Organisation))]
        public override async Task<IHttpActionResult> GetByNameAsync([FromUri] string name)
        {
            return await base.GetByNameAsync(name);
        }

        /// <summary>
        /// Organisation by Code
        /// </summary>
        /// <param name="code">The Code of the Organisation</param>
        /// <returns>Organisation</returns>
        [ResponseType(typeof(Organisation))]
        public override async Task<IHttpActionResult> GetByCodeAsync([FromUri] string code)
        {
            return await base.GetByCodeAsync(code);
        }

        //GET: Organisations/5/Sites
        /// <summary>
        /// Sites for the Organisation
        /// </summary>
        /// <param name="id">Id of Organisation</param>
        /// <returns>ListOf(Site)</returns>
        [Route("{id:guid}/Sites")]
        public IQueryable<Site> GetSites([FromUri] Guid id)
        {
            return GetManyWithGuidId<Site>(id, s => s.Sites);
        }

        //GET: Organisations/5/Stations
        /// <summary>
        /// Stations for the Organisation
        /// </summary>
        /// <param name="id">Id of Organisation</param>
        /// <returns>ListOf(Station)</returns>
        [Route("{id:guid}/Stations")]
        public IQueryable<Station> GetStations([FromUri] Guid id)
        {
            var siteStations = GetManyWithGuidId<Site>(id, s => s.Sites).SelectMany(i => i.Stations);
            return GetManyWithGuidId<Station>(id, s => s.Stations).Union(siteStations);
        }

        //GET: Organisations/5/Instruments
        /// <summary>
        /// Instruments for the Organisation
        /// </summary>
        /// <param name="id">Id of Organisation</param>
        /// <returns>ListOf(Instrument)</returns>
        [Route("{id:guid}/Instruments")]
        public IQueryable<Instrument> GetInstruments([FromUri] Guid id)
        {
            var siteInstruments = GetManyWithGuidId<Site>(id, s => s.Sites).SelectMany(i => i.Stations).SelectMany(i => i.Instruments);
            var stationInstruments = GetManyWithGuidId<Station>(id, s => s.Stations).SelectMany(i => i.Instruments);
            return GetManyWithGuidId<Instrument>(id, s => s.Instruments).Union(siteInstruments).Union(stationInstruments);
        }

    }
}
