using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace SAEON.Observations.QuerySite.Controllers
{
    public class UserDownloadsController : BaseRestController<UserDownload>
    {
        public UserDownloadsController() :base()
        {
            Resource = "UserDownloads";
        }

        public override Task<ActionResult> Create([Bind(Include = "UserId,Name,Description,QueryInput,DownloadURI")]UserDownload item)
        {
            return base.Create(item);
        }

        public override Task<ActionResult> Edit([Bind(Include = "Id,UserId,Name,Description,QueryInput,DownloadURI")] UserDownload delta)
        {
            return base.Edit(delta);
        }
    }
}