using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Toolkit.Diagnostics;
using SAEON.AspNet.Auth;
using SAEON.Logs;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    public class StationsController : CodedNamedApiController<Station>
    {
        //protected override IQueryable<Station> GetQuery(Expression<Func<Station, bool>> extraWhere = null)
        //{
        //    return base.GetQuery(extraWhere).Include(i => i.Site).Include(i => i.Instruments);
        //}

        /// <summary>
        /// All Stations
        /// </summary>
        /// <returns>ListOf(Station)</returns>
        public override Task<IEnumerable<Station>> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Station by Id
        /// </summary>
        /// <param name="id">The Id of the Station</param>
        /// <returns>Station</returns>
        public override Task<ActionResult<Station>> GetByIdAsync(Guid id)
        {
            return base.GetByIdAsync(id);
        }

        /// <summary>
        /// Station by Code
        /// </summary>
        /// <param name="code">The Code of the Station</param>
        /// <returns>Station</returns>
        public override Task<ActionResult<Station>> GetByCodeAsync(string code)
        {
            return base.GetByCodeAsync(code);
        }

        /// <summary>
        /// Station by Name
        /// </summary>
        /// <param name="name">The Name of the Station</param>
        /// <returns>Station</returns>
        public override Task<ActionResult<Station>> GetByNameAsync(string name)
        {
            return base.GetByNameAsync(name);
        }

        /// <summary>
        /// Site of a Station
        /// </summary>
        /// <param name="id">The Id of the Station</param>
        /// <returns>Site</returns>
        [HttpGet("{id:guid}/Site")]
        public async Task<ActionResult<Site>> GetSiteAsync(Guid id)
        {
            return await GetSingleAsync<Site>(id, s => s.Site);
        }

        /// <summary>
        /// Instruments of the Station
        /// </summary>
        /// <param name="id">Id of the Station</param>
        /// <returns>ListOf(Instrument)</returns>
        [HttpGet("{id:guid}/Instruments")]
        public IQueryable<Instrument> GetInstruments(Guid id)
        {
            return GetManyWithGuidId<Instrument>(id, s => s.Instruments);
        }

        /// <summary>
        /// Organisations of the Station
        /// </summary>
        /// <param name="id">Id of the Station</param>
        /// <returns>ListOf(Organisation)</returns>
        [HttpGet("{id:guid}/Organisations")]
        public IQueryable<Organisation> GetOrganisations(Guid id)
        {
            var site = GetSingle(id, s => s.Site);
            var siteOrganiations = DbContext.Sites.Where(i => i.Id == site.Id).SelectMany(i => i.Organisations);
            return GetManyWithGuidId(id, s => s.Organisations).Union(siteOrganiations); ;
        }

        /// <summary>
        /// Projects of the Station
        /// </summary>
        /// <param name="id">Id of the Station</param>
        /// <returns>ListOf(Project)</returns>
        [HttpGet("{id:guid}/Projects")]
        public IQueryable<Project> GetProjects(Guid id)
        {
            return GetManyWithGuidId<Project>(id, s => s.Projects);
        }

        ///// <summary>
        ///// Datasets of the Station
        ///// </summary>
        ///// <param name="id">Id of the Station</param>
        ///// <returns>ListOf(DataStream)</returns>
        //@@@
        //[HttpGet("{id:guid}/Datasets")]
        //public IQueryable<Dataset> GetDatasets(Guid id)
        //{
        //    return GetManyWithLongId<Dataset>(id, s => s.Datasets);
        //}

        /// <summary>
        /// Observations of a variable (phenomenon, offering, unit) at the Station
        /// </summary>
        /// <param name="id">Id of the Station</param>
        /// <param name="phenomenonId">PhenomenonId of the Observations</param>
        /// <param name="offeringId">OfferingId of the Observations</param>
        /// <param name="unitId">UnitId of the Observations</param>
        /// <returns>ListOf(Observation)</returns>
        [HttpGet("{id:guid}/Observations/{phenomenonId:guid}/{offeringId:guid}/{unitId:guid}")]
        [Authorize(Policy = ODPAuthenticationDefaults.IdTokenPolicy)]
        public async Task<List<Observation>> GetObservations(Guid id, Guid phenomenonId, Guid offeringId, Guid unitId)
        {
            var result = GetManyWithIntId<Observation>(id, s => s.Observations).Where(i => (i.PhenomenonId == phenomenonId) && (i.OfferingId == offeringId) && (i.UnitId == unitId)).Take(100).ToList(); //@@@
            try
            {
                await RequestLogger.LogAsync(DbContext, Request);
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
            }
            return result;
        }

        /// <summary>
        /// Observations of a variable (phenomenon, offering, unit) at the Station over a date range
        /// </summary>
        /// <param name="id">Id of the Station</param>
        /// <param name="input">Input of the range of observations</param>
        /// <returns>ListOf(Observation)</returns>
        [HttpPost("{id:guid}/Observations")]
        [Authorize(Policy = ODPAuthenticationDefaults.IdTokenPolicy)]
        public async Task<List<Observation>> GetObservationsDateRange(Guid id, [FromBody] ObservationInput input)
        {
            Guard.IsNotNull(input, nameof(input));
            var q = GetManyWithIntId<Observation>(id, s => s.Observations).Where(i => (i.PhenomenonId == input.PhenomenonId) && (i.OfferingId == input.OfferingId));
            if (input.StartDate.HasValue && input.EndDate.HasValue)
            {
                q = q.Where(i => (i.ValueDate >= input.StartDate) && (i.ValueDate <= input.EndDate));
            }
            else if (input.StartDate.HasValue)
            {
                q = q.Where(i => (i.ValueDate >= input.StartDate));
            }
            else if (input.EndDate.HasValue)
            {
                q = q.Where(i => (i.ValueDate <= input.EndDate));
            }
            var result = q.Take(100).ToList(); //@@@
            try
            {
                await RequestLogger.LogAsync(DbContext, Request);
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
            }
            return result;
        }
    }
}
