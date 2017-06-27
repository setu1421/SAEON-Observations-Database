using SAEON.Logs;
using SAEON.Observations.Core;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    [RoutePrefix("SpacialCoverage")]
    [ApiExplorerSettings(IgnoreApi = true)]
    //[ClaimsAuthorization("client_id","SAEON.Observations.QuerySite")]
    public class SpacialCoverageController : BaseController
    {
        public SpacialCoverageController() : base()
        {
            db.Database.CommandTimeout = 0;
        }

        [HttpPost]
        [Route]
        public async Task<SpacialCoverageOutput> Execute(SpacialCoverageInput input)
        {
            using (Logging.MethodCall(GetType(), new ParameterList { { "Params", input } }))
            {
                try
                {
                    Logging.Verbose("Input: {@input}", input);
                    if (input == null) throw new ArgumentNullException("input");
                    if (input.Stations == null) throw new ArgumentNullException("input.Stations");
                    if (!input.Stations.Any()) throw new ArgumentOutOfRangeException("input.Stations");
                    if (input.PhenomenaOfferings == null) throw new ArgumentNullException("input.PhenomenaOfferings");
                    if (!input.PhenomenaOfferings.Any()) throw new ArgumentOutOfRangeException("input.PhenomenaOfferings");
                    var output = new SpacialCoverageOutput();
                    var dataList = await db.vApiSpacialCoverages
                        .Where(i => input.Stations.Contains(i.StationId))
                        .Where(i => input.PhenomenaOfferings.Contains(i.PhenomenonOfferingId))
                        .Where(i => i.ValueDay >= input.StartDate)
                        .Where(i => i.ValueDay <= input.EndDate)
                        .OrderBy(i => i.SiteName)
                        .ThenBy(i => i.StationName)
                        .ThenBy(i => i.ValueDate)
                        .ToListAsync();
                    Logging.Verbose("DataList: {count}", dataList.Count);
                    //Logging.Verbose("DataList: {count} {@dataList}", dataList.Count, dataList);
                    var stations = dataList.Select(i => new SpacialStation{ Name = i.StationName, Latitude = i.Latitude, Longitude = i.Longitude, Elevation = i.Elevation }).Distinct();
                    foreach (var station in stations)
                    {
                        station.NoStatus = dataList.Where(i => (i.StationName == station.Name) && (i.Status == "No Status")).Count();
                        station.Unverified = dataList.Where(i => (i.StationName == station.Name) && (i.Status == "Unverified")).Count();
                        station.BeingVerified = dataList.Where(i => (i.StationName == station.Name) && (i.Status == "Being Verified")).Count();
                        station.Verified = dataList.Where(i => (i.StationName == station.Name) && (i.Status == "Verified")).Count();
                        if (station.NoStatus > 0)
                            station.Status = SpacialStatus.NoStatus;
                        else if (station.Unverified > 0)
                            station.Status = SpacialStatus.Unverified;
                        else if (station.BeingVerified > 0)
                            station.Status = SpacialStatus.BeingVerified;
                        else if (station.Verified > 0)
                            station.Status = SpacialStatus.Verified;
                        else
                            station.Status = SpacialStatus.NoStatus;
                        output.Stations.Add(station);
                    }
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