using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SAEON.Observations.Core;
using System.Linq.Expressions;
using SAEON.Observations.QuerySite.Controllers;
//using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace SAEON.Observations.DownloadSite.Controllers
{
    [RoutePrefix("UserDownloads"), Route("{action=index}")]
    public class UserDownloadsController : BaseController<UserDownload>
    {
        /// <summary>
        /// Filter only for logged in user
        /// </summary>
        /// <returns></returns>
        protected override Expression<Func<UserDownload, bool>> EntityFilter()
        {
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("Logged in UserId");
            }
            return (i => i.UserId == userId);
        }

        /// <summary>
        /// Check UserId is logged in UserId
        /// </summary>
        /// <param name="item">UserDownload</param>
        /// <param name="isInsert">True if insert</param>
        /// <returns></returns>
        protected override bool IsEntityOk(UserDownload item, bool isInsert = false)
        {
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new NullReferenceException("Not logged in");
            }
            return base.IsEntityOk(item, isInsert) && (isInsert || (item.UserId == userId));
        }

        /// <summary>
        /// Check UserId is logged in UserId
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override void SetEntity(ref UserDownload item)
        {
            base.SetEntity(ref item);
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new NullReferenceException("Not logged in");
            }
            item.UserId = userId;
            if (string.IsNullOrEmpty(item.AddedBy))
            {
                item.AddedBy = userId;
            }
            item.UpdatedBy = userId;
        }

        // GET: UserDownloads
        public override Task<ActionResult> Index()
        {
            return base.Index();
        }

        // GET: UserDownloads/Details/5
        public override Task<ActionResult> Details(Guid? id)
        {
            return base.Details(id);
        }

        public override Task<ActionResult> Create([Bind(Include = "UserId,Description,QueryURI,DownloadURI,Name")]UserDownload item)
        {
            return base.Create(item);
        }

        // GET: UserDownloads/Edit/5
        public override Task<ActionResult> Edit(Guid? id)
        {
            return base.Edit(id);
        }

        // POST: UserDownloads/Edit/5
        public override Task<ActionResult> Edit([Bind(Include = "Id,UserId,Name,Description,QueryURI,DownloadURI")] UserDownload delta)
        {
            return base.Edit(delta);
        }

        // GET: UserDownloads/Delete/5
        public override Task<ActionResult> Delete(Guid? id)
        {
            return base.Delete(id);
        }

        // POST: UserDownloads/Delete/5
        public override Task<ActionResult> DeleteConfirmed(Guid id)
        {
            return base.DeleteConfirmed(id);
        }

    }
}
