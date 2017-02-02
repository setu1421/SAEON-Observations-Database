using AutoMapper;
using Microsoft.AspNet.Identity;
using SAEON.Observations.Core;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    public class UserDownloadsController : BaseApiController<UserDownload>
    {
        /// <summary>
        /// Filter only for logged in user
        /// </summary>
        /// <returns></returns>
        protected override Expression<Func<UserDownload, bool>> EntityFilter()
        {
            return (i => i.UserId == User.Identity.GetUserId());
        }

        /// <summary>
        /// Return a list of UserDownloads
        /// </summary>
        /// <returns>A list of UserDownload</returns>
        [ResponseType(typeof(List<UserDownload>))]
        public override async Task<IHttpActionResult> GetAll()
        {
            return await base.GetAll();
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

        protected override bool IsEntityOk(UserDownload item)
        {
            return base.IsEntityOk(item) && (item.UserId != User.Identity.GetUserId());
        }

        protected override void SetEntity(ref UserDownload item)
        {
            base.SetEntity(ref item);
            item.UserId = User.Identity.GetUserId();
        }

        /// <summary>
        /// Create a UserDownload
        /// </summary>
        /// <param name="item">The UserDownload to be created</param>
        [ResponseType(typeof(UserDownload))]
        [ApiExplorerSettings(IgnoreApi = false)]
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
        [ApiExplorerSettings(IgnoreApi = false)]
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
        [ApiExplorerSettings(IgnoreApi = false)]
        public override Task<IHttpActionResult> PutByName(string name, [FromBody] UserDownload delta)
        {
            return base.PutByName(name, delta);
        }

        /// <summary>
        /// Delete a UserDownload by Id
        /// </summary>
        /// <param name="id">Id of UserDownload</param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = false)]
        public override Task<IHttpActionResult> DeleteById(Guid id)
        {
            return base.DeleteById(id);
        }

        /// <summary>
        /// Delete a UserDownload by Name
        /// </summary>
        /// <param name="name">Name of UserDownload</param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = false)]
        public override Task<IHttpActionResult> DeleteByName(string name)
        {
            return base.DeleteByName(name);
        }
    }
}