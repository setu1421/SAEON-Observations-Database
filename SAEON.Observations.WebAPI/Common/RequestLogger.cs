using Microsoft.AspNetCore.Http;
using SAEON.Logs;
using SAEON.Observations.Core;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI
{
    public static class RequestLogger
    {
        public static async Task<string> GetRawBodyAsync(
            this HttpRequest request,
            Encoding encoding = null)
        {
            if (!request.Body.CanSeek)
            {
                // We only do this if the stream isn't *already* seekable,
                // as EnableBuffering will create a new stream instance
                // each time it's called
                request.EnableBuffering();
            }

            request.Body.Position = 0;
            var reader = new StreamReader(request.Body, encoding ?? Encoding.UTF8);
            var body = await reader.ReadToEndAsync().ConfigureAwait(false);
            request.Body.Position = 0;
            return body;
        }

        private static async Task<RequestLog> CreateRequestLog(HttpRequest request, string description = null)
        {
            var body = await request.GetRawBodyAsync();
            var headersBuilder = new StringBuilder();
            foreach (var header in request.Headers)
            {
                headersBuilder.AppendLine($"{header.Key}: {header.Value}");
            }
            var callLog = new RequestLog
            {
                Method = request.Method,
                Path = request.Path,
                QueryString = request.QueryString.ToString(),
                Headers = headersBuilder.ToString(),
                Body = body,
                IPAddress = request.HttpContext.Connection.RemoteIpAddress?.ToString(),
                Description = description
            };
            return callLog;
        }

        public static async Task LogAsync(ObservationsDbContext dbContext, HttpRequest request, string description = null)
        {
            using (SAEONLogs.MethodCall(typeof(RequestLogger)))
            {
                try
                {
                    dbContext.RequestLogs.Add(await CreateRequestLog(request, description));
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                }
            }
        }
    }
}
