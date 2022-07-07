using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SAEON.Core;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.WebAPI.Hubs;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI
{
    public static class SnapshotHelper
    {
        public static async Task<string> CreateSnapshots(ObservationsDbContext dbContext, IHubContext<AdminHub> adminHub)
        {
            using (SAEONLogs.MethodCall(typeof(DOIHelper)))
            {
                try
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var sb = new StringBuilder();
                    await AddLineAsync("Creating Snapshots");
                    var inventorySnapshot = (await dbContext.InventorySnapshots.FromSqlRaw("spCreateInventorySnapshot").ToListAsync()).FirstOrDefault();
                    SAEONLogs.Information("InventorySnapshot: {InventorySnapshot}", inventorySnapshot);
                    await AddLineAsync($"Done in {stopwatch.Elapsed.TimeStr()}");
                    return sb.ToString();

                    async Task AddLineAsync(string line)
                    {
                        sb.AppendLine(line);
                        SAEONLogs.Information(line);
                        await adminHub.Clients.All.SendAsync(SignalRDefaults.CreateSnapshotsStatus, line);
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