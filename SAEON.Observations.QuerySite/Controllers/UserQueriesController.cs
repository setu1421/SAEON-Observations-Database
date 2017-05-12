using SAEON.Observations.Core.Entities;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    public class UserQueriesController : BaseRestController<UserQuery>
    {
        public UserQueriesController() : base()
        {
            Resource = "UserQueries";
        }

        public override Task<ActionResult> Create([Bind(Include = "UserId,Name,Description,QueryInput")]UserQuery item)
        {
            return base.Create(item);
        }

        public override Task<ActionResult> Edit([Bind(Include = "Id,UserId,Name,Description,QueryInput")] UserQuery delta)
        {
            return base.Edit(delta);
        }
    }
}