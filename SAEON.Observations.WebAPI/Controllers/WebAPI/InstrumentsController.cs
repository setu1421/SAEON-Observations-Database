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
    [RoutePrefix("Api/Instruments")]
    public class InstrumentsController : CodedApiController<Instrument>
    {
        //protected override List<Expression<Func<Instrument, object>>> GetIncludes()
        //{
        //    var list = base.GetIncludes();
        //    list.Add(i => i.Stations);
        //    list.Add(i => i.Sensors);
        //    return list;
        //}

        protected override IQueryable<Instrument> GetQuery(Expression<Func<Instrument, bool>> extraWhere = null)
        {
            return base.GetQuery(extraWhere)
                .Include(i => i.Organisations)
                .Include(i => i.Sensors)
                .Include(i => i.Stations)
                .Include(i => i.Stations.Select(s => s.Organisations))
                .Include(i => i.Stations.Select(s => s.Site))
                .Include(i => i.Stations.Select(s => s.Site).Select(ss => ss.Organisations));
        }

        /// <summary>
        /// All Instruments
        /// </summary>
        /// <returns>ListOf(Instrument)</returns>
        public override IQueryable<Instrument> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Instrument by Id
        /// </summary>
        /// <param name="id">The Id of the Instrument</param>
        /// <returns>Instrument</returns>
        [ResponseType(typeof(Instrument))]
        public override async Task<IHttpActionResult> GetByIdAsync([FromUri] Guid id)
        {
            return await base.GetByIdAsync(id);
        }

        /// <summary>
        /// Instrument by Name
        /// </summary>
        /// <param name="name">The Name of the Instrument</param>
        /// <returns>Instrument</returns>
        [ResponseType(typeof(Instrument))]
        public override async Task<IHttpActionResult> GetByNameAsync([FromUri] string name)
        {
            return await base.GetByNameAsync(name);
        }

        /// <summary>
        /// Instrument by Code
        /// </summary>
        /// <param name="code">The Code of the Instrument</param>
        /// <returns>Instrument</returns>
        [ResponseType(typeof(Instrument))]
        public override async Task<IHttpActionResult> GetByCodeAsync([FromUri] string code)
        {
            return await base.GetByCodeAsync(code);
        }

        // GET: Instruments/5/Organisations
        /// <summary>
        /// Organisations linked to this Instrument
        /// </summary>
        /// <param name="id">Id of the Instrument</param>
        /// <returns>ListOf(Organisation)</returns>
        [Route("{id:guid}/Organisations")]
        public IQueryable<Organisation> GetOrganisations([FromUri] Guid id)
        {
            var siteOrganisations = GetMany(id, s => s.Stations).Select(i => i.Site).SelectMany(i => i.Organisations);
            var stationOrganisations = GetMany(id, s => s.Stations).SelectMany(i => i.Organisations);
            return GetMany(id, s => s.Organisations).Union(stationOrganisations).Union(siteOrganisations);
        }

        // GET: Instruments/5/Stations
        /// <summary>
        /// Stations linked to this Instrument
        /// </summary>
        /// <param name="id">Id of the Instrument</param>
        /// <returns>ListOf(Station)</returns>
        [Route("{id:guid}/Stations")]
        public IQueryable<Station> GetStations([FromUri] Guid id)
        {
            return GetMany<Station>(id, s => s.Stations);
        }

        // GET: Instruments/5/Sensors
        /// <summary>
        /// Sensors linked to this Instrument
        /// </summary>
        /// <param name="id">Id of the Instrument</param>
        /// <returns>ListOf(Sensor)</returns>
        [Route("{id:guid}/Sensors")]
        public IQueryable<Sensor> GetSensors([FromUri] Guid id)
        {
            return GetMany<Sensor>(id, s => s.Sensors);
        }

    }
}
