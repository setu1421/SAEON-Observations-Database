using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    public class UserQueriesController : BaseRestController<UserQuery>
    {
        public UserQueriesController() : base()
        {
            Resource = "UserQueries";
        }

        public override Task<ActionResult> Create([Bind(Include = "UserId,Name,Description,QueryURI")]UserQuery item)
        {
            return base.Create(item);
        }

        public override Task<ActionResult> Edit([Bind(Include = "Id,UserId,Name,Description,QueryURI")] UserQuery delta)
        {
            return base.Edit(delta);
        }
    }
}