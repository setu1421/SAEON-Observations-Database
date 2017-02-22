using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Linq.Expressions;

namespace SAEON.Observations.WebAPI.Controllers
{
    /// <summary>
    /// Users have to be logged in to download data in the QuerySite. Any downloads are saved for later re-downloads.
    /// </summary>
    [RoutePrefix("UserDownloads")]
    public class UserDownloadsApiController : BaseApiWriteController<UserDownload>
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

        /// <summary>
        /// Check UserId is logged in UserId
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override bool IsEntityOk(UserDownload item)
        {
            var userId = User.GetUserId();
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
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("Logged in UserId");
            }
            item.UserId = userId;
        }

        /// <summary>
        /// Return a list of UserDownloads
        /// </summary>
        /// <returns>A list of UserDownload</returns>
        public override IQueryable<UserDownload> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Return a UserDownload by Id
        /// </summary>
        /// <param name="id">The Id of the UserDownload</param>
        /// <returns>UserDownload</returns>
        [ResponseType(typeof(UserDownload))]
        public override async Task<IHttpActionResult> GetById(Guid id)
        {
            return await base.GetById(id);
        }

        /// <summary>
        /// Return a UserDownload by Name
        /// </summary>
        /// <param name="name">The Name of the UserDownload</param>
        /// <returns>UserDownload</returns>
        [ResponseType(typeof(UserDownload))]
        public override async Task<IHttpActionResult> GetByName(string name)
        {
            return await base.GetByName(name);
        }

        /// <summary>
        /// Create a UserDownload
        /// </summary>
        /// <param name="item">The UserDownload to be created</param>
        [ResponseType(typeof(UserDownload))]
        [Route]
        public override async Task<IHttpActionResult> Post([FromBody]UserDownload item)
        {
            return await base.Post(item);
        }

        /// <summary>
        /// Update a UserDownload by Id
        /// </summary>
        /// <param name="id">Id of UserDownload</param>
        /// <param name="delta">The new UserDownload</param>
        /// <returns></returns>
        [Route("{id:guid}")]
        public override Task<IHttpActionResult> PutById(Guid id, [FromBody] UserDownload delta)
        {
            return base.PutById(id, delta);
        }

        /// <summary>
        /// Update a UserDownload by Name
        /// </summary>
        /// <param name="name">Name of UserDownload</param>
        /// <param name="delta">The new UserDownload</param>
        /// <returns></returns>
        [Route]
        public override Task<IHttpActionResult> PutByName(string name, [FromBody] UserDownload delta)
        {
            return base.PutByName(name, delta);
        }

        /// <summary>
        /// Delete a UserDownload by Id
        /// </summary>
        /// <param name="id">Id of UserDownload</param>
        /// <returns></returns>
        [Route("{id:guid}")]
        public override Task<IHttpActionResult> DeleteById(Guid id)
        {
            return base.DeleteById(id);
        }

        /// <summary>
        /// Delete a UserDownload by Name
        /// </summary>
        /// <param name="name">Name of UserDownload</param>
        /// <returns></returns>
        [Route]
        public override Task<IHttpActionResult> DeleteByName(string name)
        {
            return base.DeleteByName(name);
        }
    }
}