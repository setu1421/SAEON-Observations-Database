using SAEON.Observations.Auth;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    public class UserDownloadsController : InternalWriteController<UserDownload>
    {
        protected override List<Expression<Func<UserDownload, object>>> GetIncludes()
        {
            var result = base.GetIncludes();
            result.Add(i => i.DigitalObjectIdentifier);
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
            if (isPost)
            {
                item.AddedBy = userId;
            }
            item.UpdatedBy = userId;
        }
    }
}
