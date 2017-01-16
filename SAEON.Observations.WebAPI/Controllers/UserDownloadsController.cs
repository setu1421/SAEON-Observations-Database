using Microsoft.Web.Http;
using SAEON.Observations.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers
{
    /// <summary>
    /// Users have to be logged in to download data in the QuerySite. Any downloads are saved for later re-downloads.
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("UserDownloads")]
    [Authorize(Roles = "Administrators,DataReaders")]
    public class UserDownloadsController : ApiController
    {
        /// <summary>
        /// Return all UserDownloads for the logged in user
        /// </summary>
        /// <returns></returns>
        [Route]
        public IEnumerable<UserDownloadModel> Get()
        {
            return new UserDownloadModel[] { };
        }

        /// <summary>
        /// Return a UserDownload for for the logged in user
        /// </summary>
        /// <param name="id">The id of the UserDownload</param>
        /// <returns></returns>
        [Route("{id:guid}")]
        public UserDownloadModel Get(Guid id)
        {
            return null;
        }

        /// <summary>
        /// Return a UserDownload for for the logged in user
        /// </summary>
        /// <param name="name">The name of the UserDownload</param>
        /// <returns></returns>
        [Route("{name}")]
        public UserDownloadModel GetByName(string name)
        {
            return null;
        }

        /// <summary>
        /// Create a UserDownload
        /// </summary>
        /// <param name="value">The UserDownload values to be created</param>
        [Route]
        [Authorize(Roles = "QuerySite")]
        public void Post([FromBody]UserDownloadModel value)
        {
        }

        /// <summary>
        /// Update a UserDownload for the logged in user
        /// </summary>
        /// <param name="id">The id of the UserDownload</param>
        /// <param name="value">The UserDownload values to be updated</param>
        [Route("{id:guid}")]
        [Authorize(Roles = "QuerySite")]
        public void Put(Guid id, [FromBody]UserDownloadModel value)
        {
        }

        /// <summary>
        /// Update a UserDownload for the logged in user
        /// </summary>
        /// <param name="name">The name of the UserDownload</param>
        /// <param name="value">The UserDownload values to be updated</param>
        [Route("{name}")]
        [Authorize(Roles = "QuerySite")]
        public void PutByName(string name, [FromBody]UserDownloadModel value)
        {
        }

        /// <summary>
        /// Delete a UserDownload for the logged in user
        /// </summary>
        /// <param name="id">The id of the UserDownload</param>
        [Route("{id:guid}")]
        [Authorize(Roles = "QuerySite")]
        public void Delete(Guid id)
        { }

        /// <summary>
        /// Delete a UserDownload for the logged in user
        /// </summary>
        /// <param name="name">The name of the UserDownload</param>
        [Route("{name}")]
        [Authorize(Roles = "QuerySite")]
        public void DeleteByName(string name)
        {
        }
    }
}