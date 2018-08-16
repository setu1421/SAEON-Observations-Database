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
                    foreach (var orgId in input.Organisations)
                    {
                        input.Sites.AddRange(db.Organisations.Where(i => i.Id == orgId).SelectMany(i => i.Sites).Select(i => i.Id));
                        input.Stations.AddRange(db.Organisations.Where(i => i.Id == orgId).SelectMany(i => i.Stations).Select(i => i.Id));
                    }
                    foreach (var phenomenonId in input.Phenomena)
                    {
                        
                        input.Offerings.AddRange(db.PhenomenonOfferings.Where(i => i.PhenomenonId == phenomenonId).Select(i => i.Id));
                        input.Units.AddRange(db.PhenomenonUnits.Where(i => i.PhenomenonId == phenomenonId).Select(i => i.Id));
                    }
                    input.StartDate = input.StartDate.Date;
                    input.EndDate = input.EndDate.Date.AddDays(1);
                    Logging.Verbose("Processed Input: {@Input}", input);
                    IQueryable<ImportBatchSummary> sites = db.ImportBatchSummary.Where(i => input.Sites.Contains(i.SiteId));
                    IQueryable<ImportBatchSummary> stations = db.ImportBatchSummary.Where(i => input.Stations.Contains(i.StationId));
                    IQueryable<ImportBatchSummary> qLocations = sites.Union(stations).Distinct();
                    //Logging.Verbose("Locations Sql: {Sql}", qLocations.ToString());
                    var locationRows = qLocations.ToList().Sum(i => i.Count);
                    Logging.Verbose("Location Rows: {Rows}", locationRows);
                    IQueryable<ImportBatchSummary> offerings = db.ImportBatchSummary.Where(i => input.Offerings.Contains(i.PhenomenonOfferingId));
                    IQueryable<ImportBatchSummary> units = db.ImportBatchSummary.Where(i => input.Units.Contains(i.PhenomenonUnitId));
                    IQueryable<ImportBatchSummary> qFeatures = offerings.Union(units).Distinct();
                    //Logging.Verbose("Features Sql: {Sql}", qFeatures.ToString());
                    var featureRows = qFeatures.ToList().Sum(i => i.Count);
                    Logging.Verbose("Feature Rows: {Rows}", featureRows);
                    var qCombined = qLocations.Intersect(qFeatures);
                    //Logging.Verbose("Combined Sql: {Sql}", qCombined.ToString());
                    var combinedRows = qCombined.ToList().Sum(i => i.Count);
                    Logging.Verbose("Combined Rows: {Rows}", combinedRows);
                    var startDate = input.StartDate;
                    var endDate = input.EndDate;
                    var qDates = qCombined.Where(i => i.StartDate >= startDate && i.EndDate < endDate);
                    var dateRows = qDates.ToList().Sum(i => i.Count);
                    Logging.Verbose("Date Rows: {Rows}", combinedRows);
                    var result = new DataWizardApproximation
                    {
                        RowCount = dateRows
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
