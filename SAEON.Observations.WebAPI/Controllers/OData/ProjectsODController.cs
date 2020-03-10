using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using SAEON.Observations.Core.Entities;
using System;
using System.Linq;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    /// <summary>
    /// Projects
    /// </summary>
    [ODataRoutePrefix("Projects")]
    public class ProjectsODController : NamedController<Project>
    {
        // GET: odata/Projects
        /// <summary>
        /// Get all Projects
        /// </summary>
        /// <returns>ListOf(Project)</returns>
        [ODataRoute]
        public override IQueryable<Project> GetAll()
        {
            return base.GetAll();
        }

        // GET: odata/Projects(5)
        /// <summary>
        /// Project by Id
        /// </summary>
        /// <param name="id">Id of Project</param>
        /// <returns>Project</returns>
        [ODataRoute("({id})")]
        public override SingleResult<Project> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }

        //// GET: odata/Projects(5)/Programme
        ///// <summary>
        ///// Programme for the Project
        ///// </summary>
        ///// <param name="id">Id of the Project</param>
        ///// <returns>Programme</returns>
        [ODataRoute("({id})/Programme")]
        [EnableQuery(PageSize = PageSize, MaxTop = MaxTop)]
        public Programme GetProgramme([FromODataUri] Guid id)
        {
            return GetSingle(id, i => i.Programme);
        }

        // GET: odata/Projects(5)/Stations
        /// <summary>
        /// Stations for the Project
        /// </summary>
        /// <param name="id">Id of the Project</param>
        /// <returns>ListOf(Station(</returns>
        [EnableQuery(PageSize = PageSize, MaxTop = MaxTop)]
        [ODataRoute("({id})/Stations")]
        public IQueryable<Station> GetStations([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.Stations);
        }

    }
}
