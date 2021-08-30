using SAEON.Observations.Core;
using System.Collections.Generic;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    public class HomeDashboardController : InternalListController<HomeDashboard>
    {
        protected override List<HomeDashboard> GetList()
        {
            var result = base.GetList();
            result.AddRange(DbContext.HomeDashboards);
            return result;
        }
    }
}
