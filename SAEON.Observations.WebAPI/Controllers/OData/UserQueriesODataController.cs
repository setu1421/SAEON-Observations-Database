using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    /// <summary>
    /// Logged in users can save frequently used queries in the QueryUserQuery for later use
    /// </summary>
    [ODataRoutePrefix("UserQueries")]
    public class UserQueriesODataController : BaseODataController<UserQuery>
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

        ///// <summary>
        ///// Check UserId is logged in UserId
        ///// </summary>
        ///// <param name="item"></param>
        ///// <returns></returns>
        //protected override bool IsEntityOk(UserQuery item)
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
        //protected override void SetEntity(ref UserQuery item)
        //{
        //    base.SetEntity(ref item);
        //    var userId = User.GetUserId();
        //    if (string.IsNullOrEmpty(userId))
        //    {
        //        throw new ArgumentNullException("Logged in UserId");
        //    }
        //    item.UserId = userId;
        //}

        /// <summary>
        /// All UserQueries for logged in user
        /// </summary>
        /// <returns>ListOf(UserQuery)</returns>
        [EnableQuery, ODataRoute]
        public override IQueryable<UserQuery> GetAll()
        {
            return base.GetAll();
        }

        // GET: odata/UserQueries(5)
        /// <summary>
        /// UserQuery by Id for logged in user
        /// </summary>
        /// <param name="id">Id of UserQuery</param>
        /// <returns>UserQuery</returns>
        [EnableQuery, ODataRoute("({id})")]
        public override SingleResult<UserQuery> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }


        // GET: odata/UserQueries(5)
        /// <summary>
        /// UserQuery by Name for logged in user
        /// </summary>
        /// <param name="name">Name of UserQuery</param>
        /// <returns>UserQuery</returns>
        [EnableQuery, ODataRoute("({name})")]
        public override SingleResult<UserQuery> GetByName([FromODataUri] string name)
        {
            return base.GetByName(name);
        }

    }
}
