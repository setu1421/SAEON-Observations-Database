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

namespace SAEON.Observations.WebAPI.Controllers
{
    /// <summary>
    /// Instruments
    /// </summary>
    [ODataRoutePrefix("Instruments")]
    public class InstrumentsODataController : BaseODataController<Instrument>
    {

        // GET: odata/Instruments
        /// <summary>
        /// Get a list of Instruments
        /// </summary>
        /// <returns>A list of Instrument</returns>
        public override IQueryable<Instrument> GetAll()
        {
            return base.GetAll();
        }

        // GET: odata/Instruments(5)
        /// <summary>
        /// Get an Instrument by Id
        /// </summary>
        /// <param name="id">Id of Instrument</param>
        /// <returns>Instrument</returns>
        public override SingleResult<Instrument> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }

        // GET: odata/Instruments(5)
        /// <summary>
        /// Get an Instrument by Name
        /// </summary>
        /// <param name="name">Name of Instrument</param>
        /// <returns>Instrument</returns>
        public override SingleResult<Instrument> GetByName([FromODataUri] string name)
        {
            return base.GetByName(name);
        }

        //// PUT: odata/Instruments(5)
        //public async Task<IHttpActionResult> Put([FromODataUri] Guid key, Delta<Instrument> patch)
        //{
        //    Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    Instrument instrument = await db.Instruments.FindAsync(key);
        //    if (instrument == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Put(instrument);

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!InstrumentExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(instrument);
        //}

        //// POST: odata/Instruments
        //public async Task<IHttpActionResult> Post(Instrument instrument)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.Instruments.Add(instrument);

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (InstrumentExists(instrument.Id))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Created(instrument);
        //}

        //// PATCH: odata/Instruments(5)
        //[AcceptVerbs("PATCH", "MERGE")]
        //public async Task<IHttpActionResult> Patch([FromODataUri] Guid key, Delta<Instrument> patch)
        //{
        //    Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    Instrument instrument = await db.Instruments.FindAsync(key);
        //    if (instrument == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Patch(instrument);

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!InstrumentExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(instrument);
        //}

        //// DELETE: odata/Instruments(5)
        //public async Task<IHttpActionResult> Delete([FromODataUri] Guid key)
        //{
        //    Instrument instrument = await db.Instruments.FindAsync(key);
        //    if (instrument == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Instruments.Remove(instrument);
        //    await db.SaveChangesAsync();

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        // GET: odata/Instruments(5)/Stations
        [EnableQuery]
        public IQueryable<Station> GetStations([FromODataUri] Guid key)
        {
            return db.Instruments.Where(m => m.Id == key).SelectMany(m => m.Stations);
        }

    }
}
