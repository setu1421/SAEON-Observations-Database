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
    /// Logged in users can save frequently used queries in the QueryUserQuery for later use
    /// </summary>
    [ODataRoutePrefix("UserQueries")]
    public class UserQueriesODataController : BaseODataController<UserQuery>
    {
        /// <summary>
        /// Filter only for logged in user
        /// </summary>
        /// <returns></returns>
        protected override Expression<Func<UserQuery, bool>> EntityFilter()
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
        /// <param name="item"></param>
        /// <returns></returns>
        protected override bool IsEntityOk(UserQuery item)
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
        protected override void SetEntity(ref UserQuery item)
        {
            base.SetEntity(ref item);
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("Logged in UserId");
            }
            item.UserId = userId;
        }

        /// <summary>
        /// Get a list of UserQueries
        /// </summary>
        /// <returns>A list of UserQuery</returns>
        [EnableQuery, ODataRoute]
        public override IQueryable<UserQuery> GetAll()
        {
            return base.GetAll();
        }

        // GET: odata/UserQueries(5)
        /// <summary>
        /// Get a UserQuery by Id
        /// </summary>
        /// <param name="id">Id of UserQuery</param>
        /// <returns>UserQuery</returns>
        [EnableQuery, ODataRoute]
        public override SingleResult<UserQuery> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }


        // GET: odata/UserQueries(5)
        /// <summary>
        /// Get a UserQuery by Name
        /// </summary>
        /// <param name="name">Name of UserQuery</param>
        /// <returns>UserQuery</returns>
        [EnableQuery, ODataRoute]
        public override SingleResult<UserQuery> GetByName([FromODataUri] string name)
        {
            return base.GetByName(name);
        }

    }
}
