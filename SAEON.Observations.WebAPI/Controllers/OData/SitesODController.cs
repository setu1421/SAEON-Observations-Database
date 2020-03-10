using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using SAEON.Observations.Core.Entities;
using System;
using System.Linq;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    /// <summary>
    /// Sites
    /// </summary>
    [ODataRoutePrefix("Sites")]
    public class SitesODController : NamedController<Site>
    {
        /// <summary>
        /// All Sites
        /// </summary>
        /// <returns>ListOf(Site)</returns>
        [ODataRoute]
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
        [ODataRoute("({id})")]
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
        [ODataRoute("({id})/Organisations")]
        [EnableQuery(PageSize = PageSize, MaxTop = MaxTop)]
        public IQueryable<Organisation> GetOrganisations([FromODataUri] Guid id)
        {
            return GetMany<Organisation>(id, s => s.Organisations);
        }

        //GET: odata/Sites(5)/Stations
        /// <summary>
        /// Stations for the Site
        /// </summary>
        /// <param name="id">Id of Site</param>
        /// <returns>ListOf(Station)</returns>
        [ODataRoute("({id})/Stations")]
        [EnableQuery(PageSize = PageSize, MaxTop = MaxTop)]
        public IQueryable<Station> GetStations([FromODataUri] Guid id)
        {
            return GetMany<Station>(id, s => s.Stations);
        }
    }
}
