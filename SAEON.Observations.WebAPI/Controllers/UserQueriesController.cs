﻿using AutoMapper;
using Microsoft.AspNet.Identity;
using SAEON.Observations.Core;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers
{
    /// <summary>
    /// Logged in users can save frequently used queries in the QueryUserQuery for later use
    /// </summary>
    [RoutePrefix("UserQueries")]
    public class UserQueriesController : BaseApiController<UserQuery>
    {
        /// <summary>
        /// Filter only for logged in user
        /// </summary>
        /// <returns></returns>
        protected override Expression<Func<UserQuery, bool>> EntityFilter()
        {
            return (i => i.UserId == User.Identity.GetUserId());
        }

        /// <summary>
        /// Return a list of UserQueries
        /// </summary>
        /// <returns>A list of UserQuery</returns>
        [ResponseType(typeof(List<UserQuery>))]
        public override async Task<IHttpActionResult> GetAll()
        {
            return await base.GetAll();
        }

        /// <summary>
        /// Return a UserQuery by Id
        /// </summary>
        /// <param name="id">The Id of the UserQuery</param>
        /// <returns>UserQuery</returns>
        [ResponseType(typeof(UserQuery))]
        public override async Task<IHttpActionResult> GetById(Guid id)
        {
            return await base.GetById(id);
        }

        /// <summary>
        /// Return a UserQuery by Name
        /// </summary>
        /// <param name="name">The Name of the UserQuery</param>
        /// <returns>UserQuery</returns>
        [ResponseType(typeof(UserQuery))]
        public override async Task<IHttpActionResult> GetByName(string name)
        {
            return await base.GetByName(name);
        }

        protected override bool IsEntityOk(UserQuery item)
        {
            return base.IsEntityOk(item) && (item.UserId != User.Identity.GetUserId());
        }

        protected override void SetEntity(ref UserQuery item)
        {
            base.SetEntity(ref item);
            item.UserId = User.Identity.GetUserId();
        }

        /// <summary>
        /// Create a UserQuery
        /// </summary>
        /// <param name="item">The UserQuery to be created</param>
        [ResponseType(typeof(UserQuery))]
        [ApiExplorerSettings(IgnoreApi = false)]
        public override async Task<IHttpActionResult> Post([FromBody]UserQuery item)
        {
            return await base.Post(item);
        }

        /// <summary>
        /// Update a UserQuery by Id
        /// </summary>
        /// <param name="id">Id of UserQuery</param>
        /// <param name="delta">The new UserQuery</param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = false)]
        public override Task<IHttpActionResult> PutById(Guid id, [FromBody] UserQuery delta)
        {
            return base.PutById(id, delta);
        }

        /// <summary>
        /// Update a UserQuery by Name
        /// </summary>
        /// <param name="name">Name of UserQuery</param>
        /// <param name="delta">The new UserQuery</param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = false)]
        public override Task<IHttpActionResult> PutByName(string name, [FromBody] UserQuery delta)
        {
            return base.PutByName(name, delta);
        }

        /// <summary>
        /// Delete a UserQuery by Id
        /// </summary>
        /// <param name="id">Id of UserQuery</param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = false)]
        public override Task<IHttpActionResult> DeleteById(Guid id)
        {
            return base.DeleteById(id);
        }

        /// <summary>
        /// Delete a UserQuery by Name
        /// </summary>
        /// <param name="name">Name of UserQuery</param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = false)]
        public override Task<IHttpActionResult> DeleteByName(string name)
        {
            return base.DeleteByName(name);
        }
    }
}