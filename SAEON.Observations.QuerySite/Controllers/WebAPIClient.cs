using IdentityModel.Client;
using SAEON.AspNet.Common;
using SAEON.Logs;
using SAEON.Observations.Core;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace SAEON.Observations.QuerySite.Controllers
{
    public static class WebAPIClient
    {
        public static int TimeOut { get; set; } = 30; // In minutes

        public static async Task<HttpClient> GetWebAPIClientAsync(HttpRequestBase request, HttpSessionStateBase session, ClaimsPrincipal user, bool isLocal)
        {
            if (user == null)
            {
                Logging.Error("User cannot be null");
                throw new HttpException("User cannot be null");
            }
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(AspNetConstants.ApplicationJson));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Zip));
            client.DefaultRequestHeaders.Add(AspNetConstants.TenantHeader, session[AspNetConstants.TenantSession].ToString());
            //Logging.Verbose("Headers: {@Headers}", client.DefaultRequestHeaders);
            Logging.Verbose("IsAuthenticated: {IsAuthenticated} Claims: {claims}", user?.Identity.IsAuthenticated, string.Join("; ", user.GetClaims()));
            var idToken = user.FindFirst("id_token")?.Value;
            var accessToken = user.FindFirst("access_token")?.Value;
            var refreshToken = user.FindFirst("refresh_token")?.Value;
            var expiresAtClaim = user.FindFirst("expires_at")?.Value;
            var expiresAt = string.IsNullOrEmpty(expiresAtClaim) ? default(DateTimeOffset?) : DateTimeOffset.Parse(expiresAtClaim);
            Logging.Verbose("ClaimsPrincipal: IdToken: {IdToken} AccessToken: {AccessToken} RefreshToken: {RefreshToken} ExpiresAt: {ExpiresAt}", idToken, accessToken, refreshToken, expiresAt);
            if (accessToken == null)
            {
                Logging.Verbose("Getting AccessToken");
                var discoClient = new HttpClient();
                var disco = await discoClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
                {
                    Address = Properties.Settings.Default.IdentityServerUrl,
                    Policy = { RequireHttps = Properties.Settings.Default.RequireHTTPS && !isLocal }
                });
                if (disco.IsError)
                {
                    Logging.Error("Disco Error: {error}", disco.Error);
                    throw new HttpException(disco.Error);
                }
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
                    Logging.Error("Token Error: {error}", tokenResponse.Error);
                    throw new HttpException(tokenResponse.Error);
                }
                accessToken = tokenResponse.AccessToken;
                expiresAt = DateTimeOffset.Now.AddSeconds(tokenResponse.ExpiresIn);
            }
            //if ((refreshToken != null) && expiresAt.HasValue)// Refresh if about to expire
            //{
            //    var refreshAt = expiresAt.Value.Subtract(TimeSpan.FromSeconds(int.Parse(ConfigurationManager.AppSettings[Constants.RefreshTokens] ?? "600")));
            //    if (refreshAt < DateTimeOffset.UtcNow)
            //    {
            //        Logging.Verbose("Refreshing Tokens");
            //        var discoClient = new HttpClient();
            //        var disco = await discoClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            //        {
            //            Address = Properties.Settings.Default.IdentityServerUrl,
            //            Policy = { RequireHttps = Properties.Settings.Default.RequireHTTPS && !isLocal }
            //        });
            //        if (disco.IsError)
            //        {
            //            Logging.Error("Disco Error: {error}", disco.Error);
            //            throw new HttpException(disco.Error);
            //        }
            //        var tokenClient = new HttpClient();
            //        var tokenResponse = await tokenClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            //        {
            //            Address = disco.TokenEndpoint,
            //            ClientId = "SAEON.Observations.QuerySite",
            //            ClientSecret = "It6fWPU5J708",
            //            RefreshToken = refreshToken
            //        });
            //        if (tokenResponse.IsError)
            //        {
            //            Logging.Error("Token Error: {error}", tokenResponse.Error);
            //            throw new HttpException(tokenResponse.Error);
            //        }
            //        accessToken = tokenResponse.AccessToken;
            //        refreshToken = tokenResponse.RefreshToken;
            //        expiresAt = DateTimeOffset.Now.AddSeconds(tokenResponse.ExpiresIn);
            //        var claims = user.Claims.Where(i => i.Type != "access_token" && i.Type != "refresh_token" && i.Type != "expires_at").ToList();
            //        claims.Add(new Claim("access_token", accessToken));
            //        claims.Add(new Claim("expires_at", expiresAt.Value.ToString("o")));
            //        claims.Add(new Claim("refresh_token", refreshToken));
            //        var newIdentity = new ClaimsIdentity(claims, "Cookies");
            //        request.GetOwinContext().Authentication.SignIn(new AuthenticationProperties() { IsPersistent = true }, newIdentity);
            //    }
            //}
            Logging.Verbose("IdToken: {IdToken} AccessToken: {AccessToken} RefreshToken: {RefreshToken} ExpiresAt: {ExpiresAt}", idToken, accessToken, refreshToken, expiresAt);
            client.SetBearerToken(accessToken);
            client.Timeout = TimeSpan.FromMinutes(TimeOut);
            return client;
        }
    }

}
