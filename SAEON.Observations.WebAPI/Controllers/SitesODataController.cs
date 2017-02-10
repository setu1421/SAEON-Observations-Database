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
    /// Sites
    /// </summary>
    [ODataRoutePrefix("Sites")]
    public class SitesODataController : BaseODataController<Site>
    {
        /// <summary>
        /// Get a list of Sites
        /// </summary>
        /// <returns>A list of Site</returns>
        [EnableQuery, ODataRoute]
        public override IQueryable<Site> GetAll()
        {
            return base.GetAll();
        }

        // GET: odata/Sites(5)
        /// <summary>
        /// Get a Site by Id
        /// </summary>
        /// <param name="id">Id of Site</param>
        /// <returns>Site</returns>
        [EnableQuery, ODataRoute]
        public override SingleResult<Site> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }


        // GET: odata/Sites(5)
        /// <summary>
        /// Get a Site by Name
        /// </summary>
        /// <param name="name">Name of Site</param>
        /// <returns>Site</returns>
        [EnableQuery, ODataRoute]
        public override SingleResult<Site> GetByName([FromODataUri] string name)
        {
            return base.GetByName(name);
        }

        // PUT: odata/Sites(5)
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

        // POST: odata/Sites
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

        // PATCH: odata/Sites(5)
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

        // DELETE: odata/Sites(5)
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

        //GET: odata/Sites(5)/Stations
        [EnableQuery]
        public IQueryable<Station> GetStations([FromODataUri] Guid key)
        {
            return db.Sites.Where(m => m.Id == key).SelectMany(m => m.Stations);
        }
    }
}
