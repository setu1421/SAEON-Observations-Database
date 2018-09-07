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
    /// <summary>
    /// </summary>
    [RoutePrefix("Api/Inventory")]
    public class InventoryController : BaseEntityController<Inventory>
    {
        /// <summary>
        /// All Inventory
        /// </summary>
        /// <returns>ListOf(Inventory)</returns>
        public override IQueryable<Inventory> GetAll()
        {
            return base.GetAll();
        }

    }
}
