using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.WebAPI.Hubs;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI
{
    public static class ImportBatchSummaryHelper
    {
        public static async Task<string> CreateImportBatchSummaries(ObservationsDbContext dbContext, IHubContext<AdminHub> adminHub)
        {
            using (SAEONLogs.MethodCall(typeof(DOIHelper)))
            {
                try
                {
                    var sb = new StringBuilder();
                    await AddLineAsync("Creating ImportBatchSummaries");
                    await dbContext.ImportBatchSummaries.FromSqlRaw("spCreateImportBatchSummaries").ToListAsync();
                    await AddLineAsync("Done");
                    return sb.ToString();

                    async Task AddLineAsync(string line)
                    {
                        sb.AppendLine(line);
                        SAEONLogs.Information(line);
                        await adminHub.Clients.All.SendAsync(SignalRDefaults.CreateImportBatchSummariesStatusUpdate, line);
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }
    }
}

