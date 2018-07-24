using SAEON.Observations.Core.Entities;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    /// <summary>
    /// Projects
    /// </summary>
    [ODataRoutePrefix("Projects")]
    public class ProjectsODController : BaseController<Project>
    {

        // GET: odata/Projects
        /// <summary>
        /// Get all Projects
        /// </summary>
        /// <returns>ListOf(Project)</returns>
        [EnableQuery, ODataRoute]
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
        [EnableQuery, ODataRoute("({id})")]
        public override SingleResult<Project> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }

        // GET: odata/Projects(5)/Programme
        /// <summary>
        /// Programme for the Project
        /// </summary>
        /// <param name="id">Id of the Project</param>
        /// <returns>Programme</returns>
        [EnableQuery, ODataRoute("({id})/Programme")]
        public SingleResult<Programme> GetProgramme([FromODataUri] Guid id)
        {
            return GetSingle(id, s => s.Programme, i => i.Projects);
        }

        // GET: odata/Projects(5)/Stations
        /// <summary>
        /// Stations for the Project
        /// </summary>
        /// <param name="id">Id of the Project</param>
        /// <returns>ListOf(Station(</returns>
        [EnableQuery, ODataRoute("({id})/Stations")]
        public IQueryable<Station> GetStations([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.Stations, i => i.Projects);
        }

    }
}
