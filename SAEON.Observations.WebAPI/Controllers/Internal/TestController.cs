using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    [RoutePrefix("Internal/Test")]
    public class TestController : BaseWriteController<UserDownload>
    {
        public override IQueryable<UserDownload> GetAll()
        {
            return base.GetAll(); ;
        }
    }
}
