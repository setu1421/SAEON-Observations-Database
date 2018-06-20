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
    /// Logged in users can save frequently used queries in the QueryUserQuery for later use
    /// </summary>
    [RoutePrefix("Api/UserQueries")]
    [Authorize]
    public class UserQueriesController : BaseApiWriteController<UserQuery>
    {
        protected override List<Expression<Func<UserQuery, bool>>> GetWheres()
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
        protected override bool IsEntityOk(UserQuery item, bool isPost)
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
        protected override void SetEntity(ref UserQuery item, bool isPost)
        {
            base.SetEntity(ref item, isPost);
            if (isPost && (item.Id == Guid.Empty))
            {
                item.Id = new Guid();
            }
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new NullReferenceException("Not logged in");
            }
            item.UserId = userId;
            if (isPost)
            {
                item.AddedBy = userId;
            }
            item.UpdatedBy = userId;
        }

        /// <summary>
        /// All UserQueries for logged in user
        /// </summary>
        /// <returns>ListOf(UserQuery)</returns>
        public override IQueryable<UserQuery> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// UserQuery by Id for logged in user
        /// </summary>
        /// <param name="id">The Id of the UserQuery</param>
        /// <returns>UserQuery</returns>
        [ResponseType(typeof(UserQuery))]
        public override async Task<IHttpActionResult> GetById([FromUri] Guid id)
        {
            return await base.GetById(id);
        }

        /// <summary>
        /// UserQuery by Name for logged in user
        /// </summary>
        /// <param name="name">The Name of the UserQuery</param>
        /// <returns>UserQuery</returns>
        [ResponseType(typeof(UserQuery))]
        public override async Task<IHttpActionResult> GetByName([FromUri] string name)
        {
            return await base.GetByName(name);
        }

        /// <summary>
        /// Create a UserQuery for logged in user
        /// </summary>
        /// <param name="item">The UserQuery to be created</param>
        [ResponseType(typeof(UserQuery))]
        [Route]
        public override async Task<IHttpActionResult> Post([FromBody]UserQuery item)
        {
            return await base.Post(item);
        }

        /// <summary>
        /// Update a UserQuery by Id for logged in user
        /// </summary>
        /// <param name="id">Id of UserQuery</param>
        /// <param name="delta">The new UserQuery</param>
        /// <returns></returns>
        [Route("{id:guid}")]
        public override Task<IHttpActionResult> PutById(Guid id, [FromBody] UserQuery delta)
        {
            return base.PutById(id, delta);
        }

        /// <summary>
        /// Update a UserQuery by Name for logged in user
        /// </summary>
        /// <param name="name">Name of UserQuery</param>
        /// <param name="delta">The new UserQuery</param>
        /// <returns></returns>
        [Route]
        public override Task<IHttpActionResult> PutByName(string name, [FromBody] UserQuery delta)
        {
            return base.PutByName(name, delta);
        }

        /// <summary>
        /// Delete a UserQuery by Id for logged in user
        /// </summary>
        /// <param name="id">Id of UserQuery</param>
        /// <returns></returns>
        [Route("{id:guid}")]
        public override Task<IHttpActionResult> DeleteById(Guid id)
        {
            return base.DeleteById(id);
        }

        /// <summary>
        /// Delete a UserQuery by Name for logged in user
        /// </summary>
        /// <param name="name">Name of UserQuery</param>
        /// <returns></returns>
        [Route]
        public override Task<IHttpActionResult> DeleteByName(string name)
        {
            return base.DeleteByName(name);
        }
    }
    */
}