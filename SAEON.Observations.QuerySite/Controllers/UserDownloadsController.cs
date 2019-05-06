using SAEON.Observations.Core.Entities;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    public class UserDownloadsController : BaseRestController<UserDownload>
    {
        public UserDownloadsController() : base()
        {
            Resource = "Internal/UserDownloads";
        }

        //public override async Task<ActionResult> Create([Bind(Include = "UserId,Name,Description,QueryInput,DownloadURI")]UserDownload item)
        //{
        //    return await base.Create(item);
        //}

        public override async Task<ActionResult> EditAsync([Bind(Include = "Id,UserId,Name,Description,QueryInput,DownloadURI")] UserDownload delta)
        {
            return await base.EditAsync(delta);
        }
    }
}