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
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.OData.Builder;
    using System.Web.OData.Extensions;
    using SAEON.Observations.Core;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Instrument>("InstrumentsOData");
    builder.EntitySet<Station>("Stations"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class InstrumentsODataController : ODataController
    {
        private ObservationsDbContext db = new ObservationsDbContext();

        // GET: odata/InstrumentsOData
        [EnableQuery]
        public IQueryable<Instrument> GetInstrumentsOData()
        {
            return db.Instruments;
        }

        // GET: odata/InstrumentsOData(5)
        [EnableQuery]
        public SingleResult<Instrument> GetInstrument([FromODataUri] Guid key)
        {
            return SingleResult.Create(db.Instruments.Where(instrument => instrument.Id == key));
        }

        // PUT: odata/InstrumentsOData(5)
        public async Task<IHttpActionResult> Put([FromODataUri] Guid key, Delta<Instrument> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Instrument instrument = await db.Instruments.FindAsync(key);
            if (instrument == null)
            {
                return NotFound();
            }

            patch.Put(instrument);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InstrumentExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(instrument);
        }

        // POST: odata/InstrumentsOData
        public async Task<IHttpActionResult> Post(Instrument instrument)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Instruments.Add(instrument);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (InstrumentExists(instrument.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(instrument);
        }

        // PATCH: odata/InstrumentsOData(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] Guid key, Delta<Instrument> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Instrument instrument = await db.Instruments.FindAsync(key);
            if (instrument == null)
            {
                return NotFound();
            }

            patch.Patch(instrument);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InstrumentExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(instrument);
        }

        // DELETE: odata/InstrumentsOData(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] Guid key)
        {
            Instrument instrument = await db.Instruments.FindAsync(key);
            if (instrument == null)
            {
                return NotFound();
            }

            db.Instruments.Remove(instrument);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/InstrumentsOData(5)/Stations
        [EnableQuery]
        public IQueryable<Station> GetStations([FromODataUri] Guid key)
        {
            return db.Instruments.Where(m => m.Id == key).SelectMany(m => m.Stations);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool InstrumentExists(Guid key)
        {
            return db.Instruments.Count(e => e.Id == key) > 0;
        }
    }
}
