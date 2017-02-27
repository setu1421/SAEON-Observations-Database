using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SAEON.Observations.Core;
using System.Linq.Expressions;
//using Microsoft.AspNet.Identity;

namespace SAEON.Observations.QuerySite.Controllers
{
    [RoutePrefix("UserQueries"), Route("{action=index}")]
    public class UserQueriesController : BaseEntityController<UserQuery>
    {
        /// <summary>
        /// Filter only for logged in user
        /// </summary>
        /// <returns></returns>
        protected override Expression<Func<UserQuery, bool>> EntityFilter()
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("Logged in UserId");
            }
            return (i => i.UserId == userId);
        }

        /// <summary>
        /// Check UserId is logged in UserId
        /// </summary>
        /// <param name="item">UserQuery</param>
        /// <param name="isInsert">True if insert</param>
        /// <returns></returns>
        protected override bool IsEntityOk(UserQuery item, bool isInsert = false)
        {
            var userId = User.GetUserId();
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
        protected override void SetEntity(ref UserQuery item)
        {
            base.SetEntity(ref item);
            var userId = User.GetUserId();
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

        // GET: UserQueries
        public override Task<ActionResult> Index()
        {
            return base.Index();
        }

        // GET: UserQueries/Details/5
        public override Task<ActionResult> Details(Guid? id)
        {
            return base.Details(id);
        }

        public override Task<ActionResult> Create([Bind(Include = "UserId,Description,QueryURI,Name")]UserQuery item)
        {
            return base.Create(item);
        }

        // GET: UserQueries/Edit/5
        public override Task<ActionResult> Edit(Guid? id)
        {
            return base.Edit(id);
        }

        // POST: UserQueries/Edit/5
        public override Task<ActionResult> Edit([Bind(Include = "Id,UserId,Name,Description,QueryURI")] UserQuery delta)
        {
            return base.Edit(delta);
        }

        // GET: UserQueries/Delete/5
        public override Task<ActionResult> Delete(Guid? id)
        {
            return base.Delete(id);
        }

        // POST: UserQueries/Delete/5
        public override Task<ActionResult> DeleteConfirmed(Guid id)
        {
            return base.DeleteConfirmed(id);
        }

    }
}
