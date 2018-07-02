using AutoMapper;
using SAEON.AspNet.WebApi;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    [RoutePrefix("Api/Inventory")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ClientAuthorization("SAEON.Observations.QuerySite")]
    [RoleAuthorization("SAEON.Observations.Admin")]
    public class InventoryController : ApiController
    {
        protected ObservationsDbContext db = null;

        public InventoryController() : base()
        {
            using (Logging.MethodCall(GetType()))
            {
                db = new ObservationsDbContext();
                db.Configuration.AutoDetectChangesEnabled = false;
                db.Database.CommandTimeout = 0;
            }
        }

        protected override void Dispose(bool disposing)
        {
            using (Logging.MethodCall(GetType()))
            {
                if (disposing)
                {
                    if (db != null)
                    {
                        db.Dispose();
                    }
                }
                base.Dispose(disposing);
            }
        }

        [HttpPost]
        [Route]
        public async Task<InventoryOutput> Inventory(InventoryInput input)
        {
            using (Logging.MethodCall(GetType(), new ParameterList { { "Params", input } }))
            {
                try
                {
                    Logging.Verbose("Input: {@input}", input);
                    if (input == null) throw new ArgumentNullException("input");
                    var output = new InventoryOutput();
                    output.Success = true;
                    /*
                    output.Totals.AddRange((await db.InventoryTotals
                        .OrderBy(i => i.Status)
                        .ToListAsync())
                        .Select(i => Mapper.Map<InventoryTotalItem>(i)));
                    output.Stations.AddRange((await db.InventoryStations
                        .OrderBy(i => i.Name)
                        .ThenBy(i => i.Status)
                        .ToListAsync())
                        .Select(i => Mapper.Map<InventoryStationItem>(i)));
                    output.PhenomenaOfferings.AddRange((await db.InventoryPhenomenaOfferings
                        .OrderBy(i => i.Phenomenon)
                        .OrderBy(i => i.Offering)
                        .ThenBy(i => i.Status)
                        .ToListAsync())
                        .Select(i => Mapper.Map<InventoryPhenomenonOfferingItem>(i)));
                    output.Instruments.AddRange((await db.InventoryInstruments
                        .OrderBy(i => i.Name)
                        .ThenBy(i => i.Status)
                        .ToListAsync())
                        .Select(i => Mapper.Map<InventoryInstrumentItem>(i)));
                    output.Years.AddRange((await db.InventoryYears
                        .OrderBy(i => i.Year)
                        .ThenBy(i => i.Status)
                        .ToListAsync())
                        .Select(i => Mapper.Map<InventoryYearItem>(i)));
                    output.Organisations.AddRange((await db.InventoryOrganisations
                        .OrderBy(i => i.Name)
                        .ThenBy(i => i.Status)
                        .ToListAsync())
                        .Select(i => Mapper.Map<InventoryOrganisationItem>(i)));
                    //Logging.Verbose("Output: {@output}", output);
                    */
                    return output;
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }
    }
}
