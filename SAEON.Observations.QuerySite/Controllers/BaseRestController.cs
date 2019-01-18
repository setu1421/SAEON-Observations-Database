using IdentityModel.Client;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Thinktecture.IdentityModel.Mvc;

namespace SAEON.Observations.QuerySite.Controllers
{
    [HandleError, HandleForbidden]
    public class BaseRestController<TEntity> : Controller where TEntity : NamedEntity, new()
    {
        protected int TimeOut { get; set; } = 1; // In minutes
        private readonly string apiBaseUrl = Properties.Settings.Default.WebAPIUrl;

        private string resource = null;
        protected string Resource { get { return resource; } set { resource = value; } }


        protected List<string> ModelStateErrors
        {
            get
            {
                return ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception).ToList();
            }
        }

        private async Task<HttpClient> GetWebAPIClientAsync()
        {
            var discoClient = new HttpClient();
            var disco = await discoClient.GetDiscoveryDocumentAsync(Properties.Settings.Default.IdentityServerUrl);
            if (disco.IsError)
            {
                Logging.Error("Error: {error}", disco.Error);
                throw new HttpException(disco.Error);
            }
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Logging.Verbose("Claims: {claims}", string.Join("; ", User.GetClaims()));
            var token = (User as ClaimsPrincipal)?.FindFirst("access_token")?.Value;
            if (token == null)
            {
                var tokenClient = new HttpClient();
                var tokenResponse = await tokenClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "SAEON.Observations.QuerySite",
                    ClientSecret = "It6fWPU5J708",
                    Scope = "SAEON.Observations.WebAPI"
                });

                if (tokenResponse.IsError)
                {
                    Logging.Error("Error: {error}", tokenResponse.Error);
                    throw new HttpException(tokenResponse.Error);
                }
                token = tokenResponse.AccessToken;
            }
            Logging.Verbose("Token: {token}", token);
            client.SetBearerToken(token);
            client.Timeout = TimeSpan.FromMinutes(TimeOut);
            return client;
        }

        // GET: TEntity
        [HttpGet]
        public virtual async Task<ActionResult> Index()
        {
            using (Logging.MethodCall<TEntity>(GetType()))
            {
                try
                {
                    using (var client = await GetWebAPIClientAsync())
                    {
                        var response = await client.GetAsync($"{apiBaseUrl}/{Resource}");
                        Logging.Verbose("Response: {response}", response);
                        response.EnsureSuccessStatusCode();
                        var data = await response.Content.ReadAsAsync<IEnumerable<TEntity>>();
                        //Logging.Verbose("Data: {data}", data);
                        return View(data);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        // GET: TEntity/Details/Id
        /// <summary>
        /// Return an TEntity by Id
        /// </summary>
        /// <param name="id">The Id of the TEntity</param>
        /// <returns>View(TEntity)</returns>
        [HttpGet]
        [Route("{id:guid}")]
        public virtual async Task<ActionResult> Details(Guid? id)
        {
            using (Logging.MethodCall<TEntity>(GetType(), new ParameterList { { "Id", id } }))
            {
                try
                {
                    if (!id.HasValue) return RedirectToAction("Index");
                    using (var client = await GetWebAPIClientAsync())
                    {
                        var response = await client.GetAsync($"{apiBaseUrl}/{Resource}/{id?.ToString()}");
                        Logging.Verbose("Response: {response}", response);
                        response.EnsureSuccessStatusCode();
                        var data = await response.Content.ReadAsAsync<TEntity>();
                        Logging.Verbose("Data: {data}", data);
                        if (data == null) return RedirectToAction("Index");
                        return View(data);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }

        /*
        // GET: TEntity/Create
        /// <summary>
        /// Create a TEntity
        /// </summary>
        /// <returns>View()</returns>
        [Authorize]
        [HttpGet]
        //[Authorize(Roles = "Administrators, DataWriters")]
        public virtual ActionResult Create()
        {
            using (Logging.MethodCall<TEntity>(GetType()))
            {
                try
                {
                    return View();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        // POST: TEntity/Create
        /// <summary>
        /// Create a TEntity
        /// </summary>
        /// <param name="item">TEntity to create</param>
        /// <returns>View(TEntity)</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public virtual async Task<ActionResult> Create(TEntity item)
        {
            using (Logging.MethodCall<TEntity>(GetType(), new ParameterList { { "Name", item?.Name }, { "Item", item } }))
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        Logging.Error("ModelState.Invalid {ModelStateErrors}", ModelStateErrors);
                        return View(item);
                    }
                    else
                        using (var client = await GetWebAPIClientAsync())
                        {
                            var response = await client.PostAsJsonAsync<TEntity>($"{apiBaseUrl}/{Resource}", item);
                            Logging.Verbose("Response: {response}", response);
                            response.EnsureSuccessStatusCode();
                            return RedirectToAction("Index");
                        }
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to add {Name}", item.Name);
                    throw;
                }
            }
        }
        */

        // GET: TEntity/Edit/Id
        /// <summary>
        /// Edit a TEntity
        /// </summary>
        /// <param name="id">id of TEntity</param>
        /// <returns>View(TEntity)</returns>
        [HttpGet]
        [Route("{id:guid}")]
        [Authorize]
        public virtual async Task<ActionResult> Edit(Guid? id)
        {
            using (Logging.MethodCall<TEntity>(GetType(), new ParameterList { { "Id", id } }))
            {
                try
                {
                    if (!id.HasValue) return RedirectToAction("Index");
                    using (var client = await GetWebAPIClientAsync())
                    {
                        var response = await client.GetAsync($"{apiBaseUrl}/{Resource}/{id?.ToString()}");
                        Logging.Verbose("Response: {response}", response);
                        response.EnsureSuccessStatusCode();
                        var data = await response.Content.ReadAsAsync<TEntity>();
                        Logging.Verbose("Data: {data}", data);
                        if (data == null) RedirectToAction("Index");
                        return View(data);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }

        // POST: TEntity/Edit
        /// <summary>
        /// Edit a TEntity
        /// </summary>
        /// <param name="delta">TEntity to edit</param>
        /// <returns>View(TEntity)</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public virtual async Task<ActionResult> Edit(TEntity delta)
        {
            using (Logging.MethodCall<TEntity>(GetType(), new ParameterList { { "Id", delta?.Id }, { "Delta", delta } }))
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        Logging.Error("ModelState.Invalid {ModelStateErrors}", ModelStateErrors);
                        return View(delta);
                    }
                    else
                    {
                        using (var client = await GetWebAPIClientAsync())
                        {
                            var response = await client.PutAsJsonAsync<TEntity>($"{apiBaseUrl}/{Resource}/{delta?.Id}", delta);
                            Logging.Verbose("Response: {response}", response);
                            response.EnsureSuccessStatusCode();
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to edit {id}", delta?.Id);
                    throw;
                }
            }

        }

        // GET: TEntity/Delete/Id
        /// <summary>
        /// Delete a TEntity
        /// </summary>
        /// <param name="id">Id of the TEntity</param>
        /// <returns>View(item)</returns>
        [HttpGet]
        [Route("{id:guid}")]
        [Authorize]
        public virtual async Task<ActionResult> Delete(Guid? id)
        {
            using (Logging.MethodCall<TEntity>(GetType(), new ParameterList { { "Id", id } }))
            {
                try
                {
                    if (!id.HasValue) return RedirectToAction("Index");
                    using (var client = await GetWebAPIClientAsync())
                    {
                        var response = await client.GetAsync($"{apiBaseUrl}/{Resource}/{id?.ToString()}");
                        Logging.Verbose("Response: {response}", response);
                        response.EnsureSuccessStatusCode();
                        var data = await response.Content.ReadAsAsync<TEntity>();
                        Logging.Verbose("Data: {data}", data);
                        if (data == null) return RedirectToAction("Index");
                        return View(data);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to delete {id}", id);
                    throw;
                }
            }
        }

        // POST: TEntity/Delete/Id
        /// <summary>
        /// Delete a TEnity
        /// </summary>
        /// <param name="id">If of TEntity</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public virtual async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            using (Logging.MethodCall<TEntity>(GetType(), new ParameterList { { "Id", id } }))
            {
                try
                {
                    using (var client = await GetWebAPIClientAsync())
                    {
                        var response = await client.DeleteAsync($"{apiBaseUrl}/{Resource}/{id}");
                        Logging.Verbose("Response: {response}", response);
                        response.EnsureSuccessStatusCode();
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to delete {id}", id);
                    throw;
                }
            }
        }
    }
}