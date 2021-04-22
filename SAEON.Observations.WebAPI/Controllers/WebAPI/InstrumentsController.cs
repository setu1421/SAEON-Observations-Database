using Microsoft.AspNetCore.Mvc;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    public class InstrumentsController : CodedNamedApiController<Instrument>
    {
        //protected override IQueryable<Instrument> GetQuery(Expression<Func<Instrument, bool>> extraWhere = null)
        //{
        //    return base.GetQuery(extraWhere)
        //        .Include(i => i.Organisations)
        //        .Include(i => i.Sensors)
        //        .Include(i => i.Stations)
        //        .Include(i => i.Stations).ThenInclude(i => i.Organisations)
        //        .Include(i => i.Stations).ThenInclude(i => i.Site)
        //        .Include(i => i.Stations).ThenInclude(i => i.Site).ThenInclude(i => i.Organisations);
        //}

        /// <summary>
        /// All Instruments
        /// </summary>
        /// <returns>ListOf(Instrument)</returns>
        //public override Task<ActionResult<IQueryable<Instrument>>> GetAll()
        //{
        //    return base.GetAll();
        //}
        public override Task<IEnumerable<Instrument>> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Instrument by Id
        /// </summary>
        /// <param name="id">The Id of the Instrument</param>
        /// <returns>Instrument</returns>
        public override Task<ActionResult<Instrument>> GetByIdAsync(Guid id)
        {
            return base.GetByIdAsync(id);
        }

        /// <summary>
        /// Instrument by Code
        /// </summary>
        /// <param name="code">The Code of the Instrument</param>
        /// <returns>Instrument</returns>
        public override Task<ActionResult<Instrument>> GetByCodeAsync(string code)
        {
            return base.GetByCodeAsync(code);
        }

        /// <summary>
        /// Instrument by Name
        /// </summary>
        /// <param name="name">The Name of the Instrument</param>
        /// <returns>Instrument</returns>
        public override Task<ActionResult<Instrument>> GetByNameAsync(string name)
        {
            return base.GetByNameAsync(name);
        }

        /// <summary>
        /// Organisations of the Instrument
        /// </summary>
        /// <param name="id">Id of the Instrument</param>
        /// <returns>ListOf(Organisation)</returns>
        [HttpGet("{id:guid}/Organisations")]
        public IQueryable<Organisation> GetOrganisations(Guid id)
        {
            var siteOrganisations = GetManyWithGuidId(id, s => s.Stations).Select(i => i.Site).SelectMany(i => i.Organisations);
            var stationOrganisations = GetManyWithGuidId(id, s => s.Stations).SelectMany(i => i.Organisations);
            return GetManyWithGuidId(id, s => s.Organisations).Union(stationOrganisations).Union(siteOrganisations);
        }

        /// <summary>
        /// Stations of the Instrument
        /// </summary>
        /// <param name="id">Id of the Instrument</param>
        /// <returns>ListOf(Station)</returns>
        [HttpGet("{id:guid}/Stations")]
        public IQueryable<Station> GetStations(Guid id)
        {
            return GetManyWithGuidId<Station>(id, s => s.Stations);
        }

        /// <summary>
        /// Sensors of the Instrument
        /// </summary>
        /// <param name="id">Id of the Instrument</param>
        /// <returns>ListOf(Sensor)</returns>
        [HttpGet("{id:guid}/Sensors")]
        public IQueryable<Sensor> GetSensors(Guid id)
        {
            return GetManyWithGuidId<Sensor>(id, s => s.Sensors);
        }

    }
}
