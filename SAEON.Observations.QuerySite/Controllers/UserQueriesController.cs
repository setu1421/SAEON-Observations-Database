using SAEON.Observations.Core;
using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    [RoutePrefix("UserQueries")]

    public class UserQueriesController : BaseRestController<UserQuery>
    {
        public UserQueriesController() : base()
        {
            Resource = "Internal/UserQueries";
        }
    }
}