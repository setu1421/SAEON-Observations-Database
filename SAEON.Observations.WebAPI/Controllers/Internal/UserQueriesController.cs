using Microsoft.AspNetCore.Mvc;
using SAEON.AspNet.Auth;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    public class UserQueriesController : InternalWriteController<UserQuery>
    {
        protected override List<Expression<Func<UserQuery, bool>>> GetWheres()
        {
            var userId = User.UserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("Logged in UserId");
            }
            var list = base.GetWheres();
            list.Add(i => i.UserId == userId);
            return list;
        }

        protected override List<OrderBy<UserQuery>> GetOrderBys()
        {
            var result = base.GetOrderBys();
            result.Add(new OrderBy<UserQuery>(i => i.Name));
            return result;
        }

        protected override bool IsEntityOk(UserQuery item, bool isPost)
        {
            var userId = User.UserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new NullReferenceException("Not logged in");
            }
            return (isPost || (item.UserId == userId));
        }

        protected override void SetEntity(ref UserQuery item, bool isPost)
        {
            if (isPost && (item.Id == Guid.Empty))
            {
                item.Id = new Guid();
            }
            var userId = User.UserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new NullReferenceException("Not logged in");
            }
            item.UserId = userId;
            var now = DateTime.Now;
            if (isPost)
            {
                item.AddedBy = userId;
                item.AddedAt = now;
            }
            item.UpdatedBy = userId;
            item.UpdatedAt = now;
        }

        protected override void UpdateEntity(ref UserQuery item, UserQuery delta)
        {
            if (!string.IsNullOrEmpty(item.Name)) item.Name = delta.Name;
            if (!string.IsNullOrEmpty(item.Description)) item.Description = delta.Description;
        }

        public override Task<ActionResult> PutById(Guid id, [FromBody, Bind("Id", "Name", "Description", "UserId")] UserQuery delta)
        {
            return base.PutById(id, delta);
        }
    }
}
