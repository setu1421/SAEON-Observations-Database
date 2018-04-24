using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    /*
    /// <summary>
    /// Users have to be logged in to download data in the QuerySite. Any downloads are saved for later re-downloads.
    /// </summary>
    [RoutePrefix("UserDownloads")]
    [Authorize]
    public class UserDownloadsController : BaseApiWriteController<UserDownload>
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
        /// <param name="isPost"></param>
        /// <returns></returns>
        protected override bool IsEntityOk(UserDownload item, bool isPost = false)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new NullReferenceException("Not logged in");
            }
            return base.IsEntityOk(item, isPost) && (isPost || (item.UserId == userId));
        }

        /// <summary>
        /// Check UserId is logged in UserId
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isPost"></param>
        /// <returns></returns>
        protected override void SetEntity(ref UserDownload item, bool isPost)
        {
            base.SetEntity(ref item, isPost);
            if (isPost && (item.Id == Guid.Empty))
            {
                item.Id = new Guid();
            }
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("Logged in UserId");
            }
            item.UserId = userId;
            if (isPost)
            {
                item.AddedBy = userId;
            }
            item.UpdatedBy = userId;
        }

        /// <summary>
        /// All UserDownloads for logged in user
        /// </summary>
        /// <returns>ListOf(UserDownload)</returns>
        public override IQueryable<UserDownload> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// UserDownload by Id for logged in user
        /// </summary>
        /// <param name="id">The Id of the UserDownload</param>
        /// <returns>UserDownload</returns>
        [ResponseType(typeof(UserDownload))]
        public override async Task<IHttpActionResult> GetById(Guid id)
        {
            return await base.GetById(id);
        }

        /// <summary>
        /// UserDownload by Name for logged in user
        /// </summary>
        /// <param name="name">The Name of the UserDownload</param>
        /// <returns>UserDownload</returns>
        [ResponseType(typeof(UserDownload))]
        public override async Task<IHttpActionResult> GetByName(string name)
        {
            return await base.GetByName(name);
        }

        /// <summary>
        /// Create a UserDownload for logged in user
        /// </summary>
        /// <param name="item">The UserDownload to be created</param>
        [ResponseType(typeof(UserDownload))]
        [Route]
        public override async Task<IHttpActionResult> Post([FromBody]UserDownload item)
        {
            return await base.Post(item);
        }

        /// <summary>
        /// Update a UserDownload by Id for logged in user
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
        /// Update a UserDownload by Name for logged in user
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
        /// Delete a UserDownload by Id for logged in user
        /// </summary>
        /// <param name="id">Id of UserDownload</param>
        /// <returns></returns>
        [Route("{id:guid}")]
        public override Task<IHttpActionResult> DeleteById(Guid id)
        {
            return base.DeleteById(id);
        }

        /// <summary>
        /// Delete a UserDownload by Name for logged in user
        /// </summary>
        /// <param name="name">Name of UserDownload</param>
        /// <returns></returns>
        [Route]
        public override Task<IHttpActionResult> DeleteByName(string name)
        {
            return base.DeleteByName(name);
        }
    }
    */
}