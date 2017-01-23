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
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.OData.Builder;
    using System.Web.OData.Extensions;
    using SAEON.Observations.Core;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Station>("StationsOData");
    builder.EntitySet<Site>("Sites"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    [ApiExplorerSettings(IgnoreApi = true)]
    public class StationsODataController : ODataController
    {
        private ObservationsDbContext db = new ObservationsDbContext();

        // GET: odata/StationsOData
        [EnableQuery]
        public IQueryable<Station> GetStations()
        {
            return db.Stations;
        }

        // GET: odata/StationsOData(5)
        [EnableQuery]
        public SingleResult<Station> GetStation([FromODataUri] Guid key)
        {
            return SingleResult.Create(db.Stations.Where(station => station.Id == key));
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool StationExists(Guid key)
        {
            return db.Stations.Any(e => e.Id == key);
        }
    }
}
