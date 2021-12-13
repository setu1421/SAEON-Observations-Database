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
        private static async Task<RequestLog> CreateRequestLog(HttpRequest request, string description = null)
        {
            var body = string.Empty;
            using (var reader = new StreamReader(request.Body))
            {
                body = await reader.ReadToEndAsync();
            }
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
