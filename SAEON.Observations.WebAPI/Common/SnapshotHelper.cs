using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.WebAPI.Hubs;
using System;
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
                    var sb = new StringBuilder();
                    await AddLineAsync("Creating Snapshots");
                    var inventorySnapshot = (await dbContext.InventorySnapshots.FromSqlRaw("spCreateInventorySnapshot").ToListAsync()).FirstOrDefault();
                    SAEONLogs.Information("InventorySnapshot: {InventorySnapshot}", inventorySnapshot);
                    await AddLineAsync("Done");
                    return sb.ToString();

                    async Task AddLineAsync(string line)
                    {
                        sb.AppendLine(line);
                        SAEONLogs.Information(line);
                        await adminHub.Clients.All.SendAsync(SignalRDefaults.CreateSnapshotsStatusUpdate, line);
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