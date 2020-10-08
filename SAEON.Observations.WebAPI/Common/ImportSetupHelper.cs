using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Http;
using SAEON.Logs;
using SAEON.OpenXML;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI
{
    public static class ImportSetupHelper
    {
        public static async Task<string> ImportFromSpreadsheet(ObservationsDbContext dbContext, IFormFile fileData)
        {
            if (dbContext is null) throw new System.ArgumentNullException(nameof(dbContext));
            if (fileData is null) throw new System.ArgumentNullException(nameof(fileData));

            var sb = new StringBuilder();
            AddLine($"Importing setup from {fileData.FileName}");
            using (var stream = fileData.OpenReadStream())
            using (var document = SpreadsheetDocument.Open(stream, false))
            {
                // Programmes
                AddLine("Adding Programmes");
                var programmesData = ExcelHelper.GetNameValues(document, "ProgrammesData");
                var programmesList = ExcelHelper.GetTableValues(document, "Table_Programmes");
                SAEONLogs.Information("ProgrammesData: {ProgrammesData} ProgrammesList: {ProgrammesList}", programmesData.Length, programmesList.Length);
                // Projects
                AddLine("Adding Projects");
                // Sites
                AddLine("Adding Sites");
                // Stations
                AddLine("Adding Stations");
                // Instruments
                AddLine("Adding Instruments");
                // Phenomena
                AddLine("Adding Phenomena");

            }
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
