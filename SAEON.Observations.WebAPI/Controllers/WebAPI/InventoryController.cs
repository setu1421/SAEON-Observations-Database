using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Thinktecture.IdentityModel.WebApi;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    [RoutePrefix("Inventory")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ResourceAuthorize("Observations.Admin", "Inventory")]
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

        private readonly string totalsSql =
            "Select" + Environment.NewLine +
            "  Status.Name, count(*) Count" + Environment.NewLine +
            "from" + Environment.NewLine +
            "  Observation" + Environment.NewLine +
            "  left join Status" + Environment.NewLine +
            "    on(Observation.StatusID = Status.ID)" + Environment.NewLine +
            "group by" + Environment.NewLine +
            "  Status.Name";

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
                    output.TotalRecords.AddRange(await db.InventoryTotals
                        .OrderBy(i => i.Status)
                        .Select(i => new InventoryTotalItem { Status = i.Status, Count = i.Count })
                        .ToListAsync());
                    output.Stations.AddRange(await db.InventoryStations
                        .OrderBy(i => i.Name)
                        .ThenBy(i => i.Status)
                        .Select(i => new InventoryStationItem { Name = i.Name, Id = i.Id, Latitude = i.Latitude, Longitude = i.Longitude, Status = i.Status, Count = i.Count })
                        .ToListAsync());
                    output.PhenomenaOfferings.AddRange(await db.InventoryPhenomenaOfferings
                        .OrderBy(i => i.Phenomenon)
                        .OrderBy(i => i.Offering)
                        .ThenBy(i => i.Status)
                        .Select(i => new InventoryPhenomenonOfferingItem { Phenomenon = i.Phenomenon, Offering = i.Offering, Status = i.Status, Count = i.Count })
                        .ToListAsync());
                    output.Instruments.AddRange(await db.InventoryInstruments
                        .OrderBy(i => i.Name)
                        .ThenBy(i => i.Status)
                        .Select(i => new InventoryInstrumentItem { Name = i.Name, Status = i.Status, Count = i.Count })
                        .ToListAsync());
                    output.Years.AddRange(await db.InventoryYears
                        .OrderBy(i => i.Year)
                        .ThenBy(i => i.Status)
                        .Select(i => new InventoryYearItem { Year = i.Year, Status = i.Status, Count = i.Count })
                        .ToListAsync());
                    output.Organisations.AddRange(await db.InventoryOrganisations
                        .OrderBy(i => i.Name)
                        .ThenBy(i => i.Status)
                        .Select(i => new InventoryOrganisationItem { Name = i.Name, Status = i.Status, Count = i.Count })
                        .ToListAsync());
                    Logging.Verbose("Output: {@output}", output);
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
