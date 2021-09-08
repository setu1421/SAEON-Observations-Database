using Microsoft.AspNetCore.Mvc;
using SAEON.AspNet.Auth;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    public class UserDownloadsController : InternalWriteController<UserDownload, UserDownloadPatch>
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

        protected override bool IsEntityPatchOk(UserDownloadPatch item)
        {
            var userId = User.UserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new NullReferenceException("Not logged in");
            }
            return (item.UserId == userId);
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

        protected override void UpdateEntity(ref UserDownload item, UserDownload newItem)
        {
            if (!string.IsNullOrEmpty(newItem.Name)) item.Name = newItem.Name;
        }

        protected override void PatchEntity(ref UserDownload item, UserDownloadPatch patchItem)
        {
            if (!string.IsNullOrEmpty(patchItem.Name)) item.Name = patchItem.Name;
        }

        public override Task<ActionResult> PutById(Guid id, [FromBody, Bind("Id", "Name", "UserId")] UserDownload updateItem)
        {
            return base.PutById(id, updateItem);
        }

        public override Task<ActionResult> PatchById(Guid id, [FromBody, Bind("Id", "Name", "UserId")] UserDownloadPatch patchItem)
        {
            return base.PatchById(id, patchItem);
        }
    }
}
