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
    [RoutePrefix("UserQueriesAsync")]
    [Authorize(Roles = "Administrators,DataReaders")]
    public class UserQueriesAsyncController : ApiController
    {
        /// <summary>
        /// Return all UserQueries for the logged in user
        /// </summary>
        /// <returns></returns>
        [Route]
        [ResponseType(typeof(List<UserQueryModel>))]
        public async Task<IHttpActionResult> GetAsync()
        {
            List<UserQueryModel> downloads = await Task.Run(() => new List<UserQueryModel>());
            return Ok(downloads);
        }

        /// <summary>
        /// Return a UserQuery for for the logged in user
        /// </summary>
        /// <param name="id">The id of the UserQuery</param>
        /// <returns></returns>
        [Route("{id:guid}")]
        [ResponseType(typeof(UserQueryModel))]
        public async Task<IHttpActionResult> GetAsync(Guid id)
        {
            var download = await Task.Run(() => new UserQueryModel());
            if (download == null)
            {
                return NotFound();
            }

            return Ok(download);
        }

        /// <summary>
        /// Return a UserQuery for for the logged in user
        /// </summary>
        /// <param name="name">The name of the UserQuery</param>
        /// <returns></returns>
        [Route("{name}")]
        [ResponseType(typeof(UserQueryModel))]
        public async Task<IHttpActionResult> GetByNameAsync(string name)
        {
            var download = await Task.Run(() => new UserQueryModel());
            if (download == null)
            {
                return NotFound();
            }

            return Ok(download);
        }

        /// <summary>
        /// Create a UserQuery
        /// </summary>
        /// <param name="value">The UserQuery values to be created</param>
        [Route]
        [Authorize(Roles = "QuerySite")]
        [ResponseType(typeof(UserQueryModel))]
        public async Task<IHttpActionResult> PostAsync([FromBody]UserQueryModel value)
        {
            var download = await Task.Run(() => new UserQueryModel());
            return CreatedAtRoute("UserQueriesAsync", new { id = download.Id }, download);
        }

        /// <summary>
        /// Update a UserQuery for the logged in user
        /// </summary>
        /// <param name="id">The id of the UserQuery</param>
        /// <param name="value">The UserQuery values to be updated</param>
        [Route("{id:guid}")]
        [Authorize(Roles = "QuerySite")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Put(Guid id, [FromBody]UserQueryModel value)
        {
            await Task.Run(() => new UserQueryModel());
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Update a UserQuery for the logged in user
        /// </summary>
        /// <param name="name">The name of the UserQuery</param>
        /// <param name="value">The UserQuery values to be updated</param>
        [Route("{name}")]
        [Authorize(Roles = "QuerySite")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutByName(string name, [FromBody]UserQueryModel value)
        {
            await Task.Run(() => new UserQueryModel());
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Delete a UserQuery for the logged in user
        /// </summary>
        /// <param name="id">The id of the UserQuery</param>
        [Route("{id:guid}")]
        [Authorize(Roles = "QuerySite")]
        [ResponseType(typeof(UserQueryModel))]
        public async Task<IHttpActionResult> Delete(Guid id)
        {
            var download = await Task.Run(() => new UserQueryModel());
            return Ok(download);
        }

        /// <summary>
        /// Delete a UserQuery for the logged in user
        /// </summary>
        /// <param name="name">The name of the UserQuery</param>
        [Route("{name}")]
        [Authorize(Roles = "QuerySite")]
        [ResponseType(typeof(UserQueryModel))]
        public async Task<IHttpActionResult> DeleteByName(string name)
        {
            var download = await Task.Run(() => new UserQueryModel());
            return Ok(download);
        }
    }
}