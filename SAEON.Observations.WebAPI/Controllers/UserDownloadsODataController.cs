using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.ModelBinding;
using System.Web.OData;
using System.Web.OData.Query;
using System.Web.OData.Routing;
using SAEON.Observations.Core;
using System.Web.Http.Description;
using Serilog.Context;
using Serilog;
using Microsoft.AspNet.Identity;
using System.Linq.Expressions;

namespace SAEON.Observations.WebAPI.Controllers
{
    /// <summary>
    /// Users have to be logged in to download data in the QuerySite. Any downloads are saved for later re-downloads.
    /// </summary>
    [ODataRoutePrefix("UserDownloads")]
    public class UserDownloadsODataController : BaseODataController<UserDownload>
    {
        protected override List<Expression<Func<UserDownload, bool>>> GetWheres()
        {
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("Logged in UserId");
            }
            var list = base.GetWheres();
            list.Add(i => i.UserId == userId);
            return list;
        }

        /// <summary>
        /// Check UserId is logged in UserId
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override bool IsEntityOk(UserDownload item)
        {
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new NullReferenceException("Not logged in");
            }
            return base.IsEntityOk(item) && (item.UserId == userId);
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
                throw new ArgumentNullException("Logged in UserId");
            }
            item.UserId = userId;
        }

        // GET: odata/UserDownloads
        /// <summary>
        /// Get a list of UserDownloads
        /// </summary>
        /// <returns>A list of UserDownload</returns>
        [EnableQuery, ODataRoute]
        public override IQueryable<UserDownload> GetAll()
        {
            return base.GetAll();
        }

        // GET: odata/UserDownloads(5)
        /// <summary>
        /// Get a UserDownload by Id
        /// </summary>
        /// <param name="id">Id of UserDownload</param>
        /// <returns>UserDownload</returns>
        [EnableQuery, ODataRoute("({id})")]
        public override SingleResult<UserDownload> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }


        // GET: odata/UserDownloads(5)
        /// <summary>
        /// Get a UserDownload by Name
        /// </summary>
        /// <param name="name">Name of UserDownload</param>
        /// <returns>UserDownload</returns>
        [EnableQuery, ODataRoute("({name})")]
        public override SingleResult<UserDownload> GetByName([FromODataUri] string name)
        {
            return base.GetByName(name);
        }
    }
}
