using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using System.Linq.Expressions;
using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    /// <summary>
    /// Users have to be logged in to download data in the QuerySite. Any downloads are saved for later re-downloads.
    /// </summary>
    [ODataRoutePrefix("UserDownloads")]
    public class UserDownloadsController : BaseODataController<UserDownload>
    {
        protected override List<Expression<Func<UserDownload, bool>>> GetWheres()
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("Logged in UserId");
            }
            var list = base.GetWheres();
            list.Add(i => i.UserId == userId);
            return list;
        }

        ///// <summary>
        ///// Check UserId is logged in UserId
        ///// </summary>
        ///// <param name="item"></param>
        ///// <returns></returns>
        //protected override bool IsEntityOk(UserDownload item)
        //{
        //    var userId = User.GetUserId();
        //    if (string.IsNullOrEmpty(userId))
        //    {
        //        throw new NullReferenceException("Not logged in");
        //    }
        //    return base.IsEntityOk(item) && (item.UserId == userId);
        //}

        ///// <summary>
        ///// Check UserId is logged in UserId
        ///// </summary>
        ///// <param name="item"></param>
        ///// <returns></returns>
        //protected override void SetEntity(ref UserDownload item)
        //{
        //    base.SetEntity(ref item);
        //    var userId = User.GetUserId();
        //    if (string.IsNullOrEmpty(userId))
        //    {
        //        throw new ArgumentNullException("Logged in UserId");
        //    }
        //    item.UserId = userId;
        //}

        // GET: odata/UserDownloads
        /// <summary>
        /// All UserDownloads for logged in user
        /// </summary>
        /// <returns>ListOf(UserDownload)</returns>
        [EnableQuery, ODataRoute]
        public override IQueryable<UserDownload> GetAll()
        {
            return base.GetAll();
        }

        // GET: odata/UserDownloads(5)
        /// <summary>
        /// UserDownload by Id for logged in user
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
        /// UserDownload by Name for logged in user
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
