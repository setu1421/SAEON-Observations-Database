using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.ModelBinding;
using System.Web.OData;
using System.Web.OData.Query;
using System.Web.OData.Routing;
using SAEON.Observations.Core;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers
{
    /// <summary>
    /// Stations
    /// </summary>
    [ODataRoutePrefix("Stations")]
    public class StationsODataController : BaseODataController<Station>
    {
        /// <summary>
        /// Get a list of Stations
        /// </summary>
        /// <returns>A list of Station</returns>
        public override IQueryable<Station> GetAll()
        {
            return base.GetAll();
        }

        // GET: odata/Stations(5)
        /// <summary>
        /// Get a Station by Id
        /// </summary>
        /// <param name="id">Id of Station</param>
        /// <returns>Station</returns>
        public override SingleResult<Station> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }


        // GET: odata/Stations(5)
        /// <summary>
        /// Get a Station by Name
        /// </summary>
        /// <param name="name">Name of Station</param>
        /// <returns>Station</returns>
        public override SingleResult<Station> GetByName([FromODataUri] string name)
        {
            return base.GetByName(name);
        }

        // PUT: odata/StationsOData(5)
        //public async Task<IHttpActionResult> Put([FromODataUri] Guid key, Delta<Station> patch)
        //{
        //    Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    Station station = await db.Stations.FindAsync(key);
        //    if (station == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Put(station);

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!StationExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(station);
        //}

        // POST: odata/StationsOData
        //public async Task<IHttpActionResult> Post(Station station)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.Stations.Add(station);

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (StationExists(station.Id))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Created(station);
        //}

        // PATCH: odata/StationsOData(5)
        //[AcceptVerbs("PATCH", "MERGE")]
        //public async Task<IHttpActionResult> Patch([FromODataUri] Guid key, Delta<Station> patch)
        //{
        //    Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    Station station = await db.Stations.FindAsync(key);
        //    if (station == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Patch(station);

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!StationExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(station);
        //}

        // DELETE: odata/StationsOData(5)
        //public async Task<IHttpActionResult> Delete([FromODataUri] Guid key)
        //{
        //    Station station = await db.Stations.FindAsync(key);
        //    if (station == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Stations.Remove(station);
        //    await db.SaveChangesAsync();

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        // GET: odata/StationsOData(5)/Site
        [EnableQuery]
        public SingleResult<Site> GetSite([FromODataUri] Guid key)
        {
            return SingleResult.Create(db.Stations.Where(m => m.Id == key).Select(m => m.Site));
        }

    }
}
