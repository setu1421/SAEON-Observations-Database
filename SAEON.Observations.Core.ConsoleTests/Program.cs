using Newtonsoft.Json;
using SAEON.Logs;
using SAEON.Observations.Core.Entities;
using Serilog;
using System;
using System.Data.Entity;
using System.Linq;

namespace SAEON.Observations.Core.ConsoleTests
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Logging
                 .CreateConfiguration(@"Logs\SAEON.Observations.Core.ConsoleTests {Date}.txt")
                 .WriteTo.Console()
                 .Create();
            using (Logging.MethodCall(typeof(Program)))
            {
                try
                {
                    /*
                    var db = new ObservationsDbContext();
                    var col = db.Offerings.Take(5).Include(i => i.Phenomena.Select(j => j.Units)).OrderBy(i => i.Name);
                    //var col = db.Units.Include(i => i.Phenomena).OrderBy(i => i.Name);
                    Logging.Information("Count: {count} Phenomena: {phenomena}", col.Count(), col.SelectMany(i => i.Phenomena).Count());
                    var json = JsonConvert.SerializeObject(col, Formatting.Indented, new JsonSerializerSettings
                    {
                        MaxDepth = 1,
                        NullValueHandling = NullValueHandling.Ignore,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        PreserveReferencesHandling = PreserveReferencesHandling.None
                    });
                    Logging.Information("Length: {length}", json.Length);
                    Logging.Information("Length: {length} json: {json}", json.Length, json);
                    */

                    var dm = new DataMatrix();
                    dm.AddColumn("Name", "Name", MaxtixDataType.String);
                    dm.AddColumn("Value", "Value", MaxtixDataType.Double);
                    dm.AddRow("Test", 1);
                    dm.AddRow("Wow", 2);
                    var json = JsonConvert.SerializeObject(dm, Formatting.Indented, new JsonSerializerSettings {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    Logging.Information("json: {json}", json);
                    Logging.Information("Cols: {cols} Rows: {rows}", dm.Columns.Count, dm.Rows.Count);
                    Logging.Information("Data: {data}", dm.Rows[0]["Name"]);

                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Exception");
                    throw;
                }
            }

        }
    }
}
