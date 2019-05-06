using SAEON.Observations.Core.Entities;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    public class UserQueriesController : BaseRestController<UserQuery>
    {
        public UserQueriesController() : base()
        {
            Resource = "Internal/UserQueries";
        }

        //public override async Task<ActionResult> Create([Bind(Include = "UserId,Name,Description,QueryInput")]UserQuery item)
        //{
        //    return await base.Create(item);
        //}

        public override async Task<ActionResult> EditAsync([Bind(Include = "Id,UserId,Name,Description,QueryInput")] UserQuery delta)
        {
            return await base.EditAsync(delta);
        }
    }
}