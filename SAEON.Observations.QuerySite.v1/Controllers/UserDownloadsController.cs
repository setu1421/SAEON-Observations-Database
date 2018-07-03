using System.Web.Mvc;
using System.Threading.Tasks;
using SAEON.Observations.Core.Entities;

namespace SAEON.Observations.QuerySite.Controllers
{
    public class UserDownloadsController : BaseRestController<UserDownload>
    {
        public UserDownloadsController() :base()
        {
            Resource = "UserDownloads";
        }

        //public override async Task<ActionResult> Create([Bind(Include = "UserId,Name,Description,QueryInput,DownloadURI")]UserDownload item)
        //{
        //    return await base.Create(item);
        //}

        public override async Task<ActionResult> Edit([Bind(Include = "Id,UserId,Name,Description,QueryInput,DownloadURI")] UserDownload delta)
        {
            return await base.Edit(delta);
        }
    }
}