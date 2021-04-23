using SAEON.Observations.Core;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;

namespace SAEON.Observations.QuerySite.Controllers
{
    [RoutePrefix("UserQueries")]
    [OutputCache(Duration = 0, Location = OutputCacheLocation.None, NoStore = true)]
    [Authorize]
    public class UserQueriesController : BaseRestController<UserQuery>
    {
        public UserQueriesController() : base()
        {
            Resource = "Internal/UserQueries";
        }

        public override Task<ActionResult> Edit([Bind(Include = "Id,Name,Description,UserId")] UserQuery delta)
        {
            return base.Edit(delta);
        }
    }
}