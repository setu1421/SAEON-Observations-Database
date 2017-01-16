using Microsoft.Web.Http;
using SAEON.Observations.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers
{
    /// <summary>
    /// Logged in users can save frequently used queries in the QuerySite for later use
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("UserQueries")]
    [Authorize(Roles = "Administrators,DataReaders")]
    public class UserQueriesController : ApiController
    {
        /// <summary>
        /// Return all UserQueries for the logged in user
        /// </summary>
        /// <returns></returns>
        [Route]
        public IEnumerable<UserQueryModel> Get()
        {
            return new UserQueryModel[] {};
        }

        //[Route]
        //[ResponseType(typeof(UserQueryModel))]
        //public async Task<IHttpActionResult> GetAsync()
        //{
        //    return NotFound();
        //}

        /// <summary>
        /// Return a UserQuery for for the logged in user
        /// </summary>
        /// <param name="id">The id of the UserQuery</param>
        /// <returns></returns>
        [Route("{id:guid}")]
        public UserQueryModel Get(Guid id)
        {
            return null;
        }

        /// <summary>
        /// Return a UserQuery for for the logged in user
        /// </summary>
        /// <param name="name">The name of the UserQuery</param>
        /// <returns></returns>
        [Route("{name}")]
        public UserQueryModel GetByName(string name)
        {
            return null;
        }

        /// <summary>
        /// Create a UserQuery
        /// </summary>
        /// <param name="value">The UserQuery values to be created</param>
        [Route]
        [Authorize(Roles="QuerySite")]
        public void Post([FromBody]UserQueryModel value)
        {
        }

        /// <summary>
        /// Update a UserQuery for the logged in user
        /// </summary>
        /// <param name="id">The id of the UserQuery</param>
        /// <param name="value">The UserQuery values to be updated</param>
        [Route("{id:guid}")]
        [Authorize(Roles = "QuerySite")]
        public void Put(Guid id, [FromBody]UserQueryModel value)
        {
        }

        /// <summary>
        /// Update a UserQuery for the logged in user
        /// </summary>
        /// <param name="name">The name of the UserQuery</param>
        /// <param name="value">The UserQuery values to be updated</param>
        [Route("{name}")]
        [Authorize(Roles = "QuerySite")]
        public void PutByName(string name, [FromBody]UserQueryModel value)
        {
        }

        /// <summary>
        /// Delete a UserQuery for the logged in user
        /// </summary>
        /// <param name="id">The id of the UserQuery</param>
        [Route("{id:guid}")]
        [Authorize(Roles = "QuerySite")]
        public void Delete(Guid id)
        { }

        /// <summary>
        /// Delete a UserQuery for the logged in user
        /// </summary>
        /// <param name="name">The name of the UserQuery</param>
        [Route("{name}")]
        [Authorize(Roles = "QuerySite")]
        public void DeleteByName(string name)
        {
        }
    }
}