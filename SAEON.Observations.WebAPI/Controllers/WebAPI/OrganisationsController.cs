using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    public class OrganisationsController : NamedApiController<Organisation>
    {
        //protected override IQueryable<Organisation> GetQuery(Expression<Func<Organisation, bool>> extraWhere = null)
        //{
        //    return base.GetQuery(extraWhere)
        //        .Include(i => i.Sites).ThenInclude(i => i.Stations).ThenInclude(i => i.Instruments)
        //        .Include(i => i.Stations).ThenInclude(i => i.Instruments)
        //        .Include(i => i.Instruments);
        //}

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
        public override Task<ActionResult<Organisation>> GetByIdAsync(Guid id)
        {
            return base.GetByIdAsync(id);
        }

        /// <summary>
        /// Organisation by Code
        /// </summary>
        /// <param name="code">The Code of the Organisation</param>
        /// <returns>Organisation</returns>
        public override Task<ActionResult<Organisation>> GetByCodeAsync(string code)
        {
            return base.GetByCodeAsync(code);
        }

        /// <summary>
        /// Organisation by Name
        /// </summary>
        /// <param name="name">The Name of the Organisation</param>
        /// <returns>Organisation</returns>
        public override Task<ActionResult<Organisation>> GetByNameAsync(string name)
        {
            return base.GetByNameAsync(name);
        }

        /// <summary>
        /// Sites of the Organisation
        /// </summary>
        /// <param name="id">Id of Organisation</param>
        /// <returns>ListOf(Site)</returns>
        [HttpGet("{id:guid}/Sites")]
        public IQueryable<Site> GetSites(Guid id)
        {
            return GetManyWithGuidId<Site>(id, s => s.Sites);
        }

        //GET: Organisations/5/Stations
        /// <summary>
        /// Stations of the Organisation
        /// </summary>
        /// <param name="id">Id of Organisation</param>
        /// <returns>ListOf(Station)</returns>
        [HttpGet("{id:guid}/Stations")]
        public IQueryable<Station> GetStations(Guid id)
        {
            var siteStations = GetManyWithGuidId<Site>(id, s => s.Sites).SelectMany(i => i.Stations);
            return GetManyWithGuidId<Station>(id, s => s.Stations).Union(siteStations);
        }

        /// <summary>
        /// Instruments of the Organisation
        /// </summary>
        /// <param name="id">Id of Organisation</param>
        /// <returns>ListOf(Instrument)</returns>
        [HttpGet("{id:guid}/Instruments")]
        public IQueryable<Instrument> GetInstruments(Guid id)
        {
            var siteInstruments = GetManyWithGuidId<Site>(id, s => s.Sites).SelectMany(i => i.Stations).SelectMany(i => i.Instruments);
            var stationInstruments = GetManyWithGuidId<Station>(id, s => s.Stations).SelectMany(i => i.Instruments);
            return GetManyWithGuidId<Instrument>(id, s => s.Instruments).Union(siteInstruments).Union(stationInstruments);
        }
    }
}
