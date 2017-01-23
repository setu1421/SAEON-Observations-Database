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
    builder.EntitySet<Site>("SitesOData");
    builder.EntitySet<Station>("Stations"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SitesODataController : ODataController
    {
        private ObservationsDbContext db = new ObservationsDbContext();

        // GET: odata/SitesOData
        [EnableQuery]
        public IQueryable<Site> GetSites()
        {
            return db.Sites;
        }

        // GET: odata/SitesOData(5)
        [EnableQuery]
        public SingleResult<Site> GetSite([FromODataUri] Guid key)
        {
            return SingleResult.Create(db.Sites.Where(site => site.Id == key));
        }

        // PUT: odata/SitesOData(5)
        //public async Task<IHttpActionResult> Put([FromODataUri] Guid key, Delta<Site> patch)
        //{
        //    Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    Site site = await db.Sites.FindAsync(key);
        //    if (site == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Put(site);

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!SiteExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(site);
        //}

        // POST: odata/SitesOData
        //public async Task<IHttpActionResult> Post(Site site)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.Sites.Add(site);

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (SiteExists(site.Id))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Created(site);
        //}

        // PATCH: odata/SitesOData(5)
        //[AcceptVerbs("PATCH", "MERGE")]
        //public async Task<IHttpActionResult> Patch([FromODataUri] Guid key, Delta<Site> patch)
        //{
        //    Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    Site site = await db.Sites.FindAsync(key);
        //    if (site == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Patch(site);

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!SiteExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(site);
        //}

        // DELETE: odata/SitesOData(5)
        //public async Task<IHttpActionResult> Delete([FromODataUri] Guid key)
        //{
        //    Site site = await db.Sites.FindAsync(key);
        //    if (site == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Sites.Remove(site);
        //    await db.SaveChangesAsync();

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        // GET: odata/SitesOData(5)/Stations
        [EnableQuery]
        public IQueryable<Station> GetStations([FromODataUri] Guid key)
        {
            return db.Sites.Where(m => m.Id == key).SelectMany(m => m.Stations);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SiteExists(Guid key)
        {
            return db.Sites.Any(e => e.Id == key);
        }
    }
}
