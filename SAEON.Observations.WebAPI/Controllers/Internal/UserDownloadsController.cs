using Microsoft.AspNetCore.Mvc;
using SAEON.Observations.Auth;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    public class UserDownloadsController : InternalWriteController<UserDownload>
    {
        protected override List<Expression<Func<UserDownload, object>>> GetIncludes()
        {
            var result = base.GetIncludes();
            //result.Add(i => i.DigitalObjectIdentifier);
            return result;
        }

        protected override List<Expression<Func<UserDownload, bool>>> GetWheres()
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

        protected override List<OrderBy<UserDownload>> GetOrderBys()
        {
            var result = base.GetOrderBys();
            result.Add(new OrderBy<UserDownload>(i => i.Name));
            return result;
        }

        protected override bool IsEntityOk(UserDownload item, bool isPost)
        {
            var userId = User.UserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new NullReferenceException("Not logged in");
            }
            return (isPost || (item.UserId == userId));
        }

        protected override void SetEntity(ref UserDownload item, bool isPost)
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

        protected override void UpdateEntity(ref UserDownload item, UserDownload delta)
        {
            if (!string.IsNullOrEmpty(item.Name)) item.Name = delta.Name;
            // No longer editable
            //if (!string.IsNullOrEmpty(item.Description)) item.Description = delta.Description;
        }

        public override Task<ActionResult> PutById(Guid id, [FromBody, Bind("Id", "Name", "Description", "UserId")] UserDownload delta)
        {
            return base.PutById(id, delta);
        }
    }
}
