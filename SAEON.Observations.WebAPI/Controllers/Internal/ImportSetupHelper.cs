using Microsoft.AspNetCore.Http;
using SAEON.Logs;
using SAEON.Observations.Core;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    public static class ImportSetupHelper
    {
        public static async Task<string> ImportFromSpreadsheet(ObservationsDbContext dbContext, HttpContext httpContext, string fileName)
        {
            var sb = new StringBuilder();
            AddLine("$Importing setup from {fileName}");

            AddLine("Done");
            return sb.ToString();

            void AddLine(string line)
            {
                sb.AppendLine(line);
                SAEONLogs.Information(line);
            }
        }
    }
}
