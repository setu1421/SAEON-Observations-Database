using SAEON.Observations.Core;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;

namespace SAEON.Observations.QuerySite.Controllers
{
    [RoutePrefix("UserDownloads")]
    [OutputCache(Duration = 0, Location = OutputCacheLocation.None, NoStore = true)]
    [Authorize]
    public class UserDownloadsController : BaseRestController<UserDownload>
    {
        public UserDownloadsController() : base()
        {
            Resource = "Internal/UserDownloads";
        }

        public override Task<ActionResult> Edit([Bind(Include = "Id,UserId,Name")] UserDownload delta)
        {
            return base.Edit(delta);
        }
    }
}