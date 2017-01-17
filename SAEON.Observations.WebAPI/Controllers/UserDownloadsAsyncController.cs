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
    /// Users have to be logged in to download data in the QuerySite. Any downloads are saved for later re-downloads.
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("UserDownloadsAsync")]
    [Authorize(Roles = "Administrators,DataReaders")]
    public class UserDownloadsAsyncController : ApiController
    {
        /// <summary>
        /// Return all UserDownloads for the logged in user
        /// </summary>
        /// <returns></returns>
        [Route]
        [ResponseType(typeof(List<UserDownloadModel>))]
        public async Task<IHttpActionResult> GetAsync()
        {
            List<UserDownloadModel> downloads = await Task.Run(() => new List<UserDownloadModel>());
            return Ok(downloads);
        }

        /// <summary>
        /// Return a UserDownload for for the logged in user
        /// </summary>
        /// <param name="id">The id of the UserDownload</param>
        /// <returns></returns>
        [Route("{id:guid}")]
        [ResponseType(typeof(UserDownloadModel))]
        public async Task<IHttpActionResult> GetAsync(Guid id)
        {
            var download = await Task.Run(() => new UserDownloadModel());
            if (download == null)
            {
                return NotFound();
            }

            return Ok(download);
        }

        /// <summary>
        /// Return a UserDownload for for the logged in user
        /// </summary>
        /// <param name="name">The name of the UserDownload</param>
        /// <returns></returns>
        [Route("{name}")]
        [ResponseType(typeof(UserDownloadModel))]
        public async Task<IHttpActionResult> GetByNameAsync(string name)
        {
            var download = await Task.Run(() => new UserDownloadModel());
            if (download == null)
            {
                return NotFound();
            }

            return Ok(download);
        }

        /// <summary>
        /// Create a UserDownload
        /// </summary>
        /// <param name="value">The UserDownload values to be created</param>
        [Route]
        [Authorize(Roles = "QuerySite")]
        [ResponseType(typeof(UserDownloadModel))]
        public async Task<IHttpActionResult> PostAsync([FromBody]UserDownloadModel value)
        {
            var download = await Task.Run(() => new UserDownloadModel());
            return CreatedAtRoute("UserDownloadsAsync", new { id = download.Id }, download);
        }

        /// <summary>
        /// Update a UserDownload for the logged in user
        /// </summary>
        /// <param name="id">The id of the UserDownload</param>
        /// <param name="value">The UserDownload values to be updated</param>
        [Route("{id:guid}")]
        [Authorize(Roles = "QuerySite")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Put(Guid id, [FromBody]UserDownloadModel value)
        {
            await Task.Run(() => new UserDownloadModel());
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Update a UserDownload for the logged in user
        /// </summary>
        /// <param name="name">The name of the UserDownload</param>
        /// <param name="value">The UserDownload values to be updated</param>
        [Route("{name}")]
        [Authorize(Roles = "QuerySite")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutByName(string name, [FromBody]UserDownloadModel value)
        {
            await Task.Run(() => new UserDownloadModel());
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Delete a UserDownload for the logged in user
        /// </summary>
        /// <param name="id">The id of the UserDownload</param>
        [Route("{id:guid}")]
        [Authorize(Roles = "QuerySite")]
        [ResponseType(typeof(UserDownloadModel))]
        public async Task<IHttpActionResult> Delete(Guid id)
        {
            var download = await Task.Run(() => new UserDownloadModel());
            return Ok(download);
        }

        /// <summary>
        /// Delete a UserDownload for the logged in user
        /// </summary>
        /// <param name="name">The name of the UserDownload</param>
        [Route("{name}")]
        [Authorize(Roles = "QuerySite")]
        [ResponseType(typeof(UserDownloadModel))]
        public async Task<IHttpActionResult> DeleteByName(string name)
        {
            var download = await Task.Run(() => new UserDownloadModel());
            return Ok(download);
        }
    }
}