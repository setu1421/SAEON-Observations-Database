using Newtonsoft.Json;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using System;
using System.Linq;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    [RoutePrefix("Internal/DataWizard")]
    public class DataWizardController : BaseController
    {

        private DataWizardApproximation CalculateApproximation(DataWizardInput input)
        {
            using (Logging.MethodCall<DataWizardApproximation>(GetType()))
            {
                try
                {
                    Logging.Verbose("Input: {@Input}", input);
                    if (input.Organisations.Any())
                    {
                        foreach (var orgId in input.Organisations)
                        {
                            input.Sites.AddRange(db.Organisations.Where(i => i.Id == orgId).SelectMany(i => i.Sites).Select(i => i.Id));
                            input.Stations.AddRange(db.Organisations.Where(i => i.Id == orgId).SelectMany(i => i.Stations).Select(i => i.Id));
                        }
                    }
                    Logging.Verbose("Input: {@Input}", input);
                    //IQueryable<Inventory> sites = db.Inventory.Where(i => input.Sites.Contains(i.SiteId));
                    //IQueryable<Inventory> stations = db.Inventory.Where(i => input.Stations.Contains(i.StationId));
                    //IQueryable<Inventory> q = sites.Union(stations);
                    IQueryable<ImportBatchSummary> sites = db.ImportBatchSummary.Where(i => input.Sites.Contains(i.SiteId));
                    IQueryable<ImportBatchSummary> stations = db.ImportBatchSummary.Where(i => input.Stations.Contains(i.StationId));
                    IQueryable<ImportBatchSummary> q = sites.Union(stations);
                    Logging.Verbose("Sql: {Sql}",q.ToString());
                    var result = new DataWizardApproximation
                    {
                        RowCount = q?.LongCount() ?? 0
                    };
                    if (!(input.Organisations.Any() || input.Sites.Any() || input.Stations.Any()))
                    {
                        result.Errors.Add("Please select at least one Organisation, Site or Station");
                    }
                    if (!(input.Phenomena.Any() || input.Offerings.Any() || input.Units.Any()))
                    {
                        result.Errors.Add("Please select at least one Phenomenon, Offering or Unit");
                    }
                    Logging.Verbose("Result: {@Result}", result);
                    return result;
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("Approximation")]
        public DataWizardApproximation ApproximationGet([FromUri] string json)
        {
            using (Logging.MethodCall<DataWizardApproximation>(GetType()))
            {
                try
                {
                    Logging.Verbose("Uri: {Uri}", Request.RequestUri);
                    Logging.Verbose("Json: {Json}", json);
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        throw new ArgumentNullException(nameof(json));
                    }
                    return CalculateApproximation(JsonConvert.DeserializeObject<DataWizardInput>(json));
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get approximation");
                    throw;
                }
            }
        }

        [HttpPost]
        [Route("Approximation")]
        public DataWizardApproximation ApproximationPost([FromBody] DataWizardInput input)
        {
            using (Logging.MethodCall<DataWizardApproximation>(GetType()))
            {
                try
                {
                    Logging.Verbose("Uri: {Uri}", Request.RequestUri);
                    if (input == null)
                    {
                        throw new ArgumentNullException(nameof(input));
                    }
                    return CalculateApproximation(input);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get approximation");
                    throw;
                }
            }
        }
    }
}
