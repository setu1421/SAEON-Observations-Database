using System;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using SAEON.Observations.Core.Entities;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    /// <summary>
    /// Sites
    /// </summary>
    [ODataRoutePrefix("Sites")]
    public class SitesODController : BaseODataController<Site>
    {
        /// <summary>
        /// All Sites
        /// </summary>
        /// <returns>ListOf(Site)</returns>
        [EnableQuery, ODataRoute]
        public override IQueryable<Site> GetAll()
        {
            return base.GetAll();
        }

        // GET: odata/Sites(5)
        /// <summary>
        /// Site by Id
        /// </summary>
        /// <param name="id">Id of Site</param>
        /// <returns>Site</returns>
        [EnableQuery, ODataRoute("({id})")]
        public override SingleResult<Site> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }


        //GET: odata/Sites(5)/Organisations
        /// <summary>
        /// Organisations for the Site
        /// </summary>
        /// <param name="id">Id of Site</param>
        /// <returns>ListOf(Organisation)</returns>
        [EnableQuery, ODataRoute("({id})/Organisations")]
        public IQueryable<Organisation> GetOrganisations([FromODataUri] Guid id)
        {
            return GetMany<Organisation>(id, s => s.Organisations, i => i.Sites);
        }

        //GET: odata/Sites(5)/Stations
        /// <summary>
        /// Stations for the Site
        /// </summary>
        /// <param name="id">Id of Site</param>
        /// <returns>ListOf(Station)</returns>
        [EnableQuery, ODataRoute("({id})/Stations")]
        public IQueryable<Station> GetStations([FromODataUri] Guid id)
        {
            return GetMany<Station>(id, s => s.Stations, i => i.Site);
        }
    }
}
