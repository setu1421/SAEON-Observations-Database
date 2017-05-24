using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Thinktecture.IdentityModel.WebApi;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    [RoutePrefix("Inventory")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ResourceAuthorize("Observations.Admin", "DataQuery")]
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
                    var totalRecords = db.Database.SqlQuery<TotalItem>(totalsSql);
                    await totalRecords.ForEachAsync(i => output.TotalRecords.Add(i.Name, i.Count));
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
