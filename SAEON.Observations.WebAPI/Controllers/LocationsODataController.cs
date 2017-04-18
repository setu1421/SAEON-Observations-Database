using System;
using System.Linq;
using System.Web.OData;
using System.Web.OData.Routing;
using SAEON.Observations.Core;
using System.Net.Http;
using System.Web.Http.Description;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers
{
    [ODataRoutePrefix("Locations")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    public class LocationsODataController : ODataController
    {
        ObservationsDbContext db = new ObservationsDbContext();

        [EnableQuery, ODataRoute]
        public IQueryable<Location> GetAll()
        {
            using (Logging.MethodCall<Location>(GetType()))
            {
                try
                {
                    Logging.Verbose("Request.Uri: {uri}", Request.RequestUri);
                    Logging.Verbose("QueryString: {querystring}", string.Join(", ", Request.GetQueryNameValuePairs().Select(kv => $"{kv.Key}: {kv.Value}")));
                    return LocationsHelper.GetLocations(db);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get all");
                    throw;
                }
            }
        }

        //[EnableQuery, ODataRoute("({id})")]
        //public SingleResult<Location> GetById([FromODataUri] Guid id)
        //{
        //    using (Logging.MethodCall<Location>(this.GetType(), new ParameterList { { "Id", id } }))
        //    {
        //        try
        //        {
        //            return SingleResult.Create(GetLocations().Where(i => (i.Id == id)));
        //        }
        //        catch (Exception ex)
        //        {
        //            Logging.Exception(ex, "Unable to get {id}", id);
        //            throw;
        //        }
        //    }
        //}

        //[EnableQuery, ODataRoute("({parentId})")]
        //public SingleResult<Location> GetByParentId([FromODataUri] Guid parentId)
        //{
        //    using (Logging.MethodCall<Location>(this.GetType(), new ParameterList { { "ParentId", parentId } }))
        //    {
        //        try
        //        {
        //            return SingleResult.Create(GetLocations().Where(i => (i.ParentId == parentId)));
        //        }
        //        catch (Exception ex)
        //        {
        //            Logging.Exception(ex, "Unable to get {id}", parentId);
        //            throw;
        //        }
        //    }
        //}

        //private static ODataValidationSettings _validationSettings = new ODataValidationSettings();

        //// GET: odata/Locations
        //public async Task<IHttpActionResult> GetLocations(ODataQueryOptions<Location> queryOptions)
        //{
        //    // validate the query.
        //    try
        //    {
        //        queryOptions.Validate(_validationSettings);
        //    }
        //    catch (ODataException ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }

        //    return Ok<IEnumerable<Location>>(GetAll().AsEnumerable());
        //}

        //// GET: odata/LocationsOData(5)
        //public async Task<IHttpActionResult> GetLocation([FromODataUri] System.Guid key, ODataQueryOptions<Location> queryOptions)
        //{
        //    // validate the query.
        //    try
        //    {
        //        queryOptions.Validate(_validationSettings);
        //    }
        //    catch (ODataException ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }

        //    // return Ok<Location>(location);
        //    return StatusCode(HttpStatusCode.NotImplemented);
        //}

    }
}
