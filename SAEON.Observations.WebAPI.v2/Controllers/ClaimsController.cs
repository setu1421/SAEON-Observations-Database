using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace SAEON.Observations.WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ClaimsController : ControllerBase
    {
        public IActionResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }

        //[HttpGet]
        //[Route]
        //public IQueryable<string> GetAll()
        //{
        //    var result = new List<string>
        //    {
        //        $"UserId: {User.GetUserId()}",
        //        $"UserName: {User.GetUserName()}"
        //    };
        //    var cp = (ClaimsPrincipal)User;
        //    result.AddRange(cp.Claims.Select(i => $"{i.Type} = {i.Value}").AsQueryable());
        //    return result.AsQueryable();
        //}
    }
}