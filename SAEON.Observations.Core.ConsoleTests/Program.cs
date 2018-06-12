using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SAEON.Logs;
using SAEON.Observations.Core.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.Core.ConsoleTests
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var log = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.Seq("http://localhost:5341")
            .CreateLogger())
            {
                try
                {
                    var db = new ObservationsDbContext();
                    var col = db.Offerings.Take(5).Include(i => i.Phenomena.Select(j => j.Units)).OrderBy(i => i.Name);
                    //var col = db.Units.Include(i => i.Phenomena).OrderBy(i => i.Name);
                    log.Information("Count: {count} Phenomena: {phenomena}", col.Count(), col.SelectMany(i => i.Phenomena).Count());
                    var json = JsonConvert.SerializeObject(col, Formatting.Indented, new JsonSerializerSettings
                    {
                        MaxDepth = 1,
                        NullValueHandling = NullValueHandling.Ignore,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        PreserveReferencesHandling = PreserveReferencesHandling.None
                    });
                    log.Information("Length: {length}", json.Length);
                    log.Information("Length: {length} json: {json}", json.Length, json);
                }
                catch (Exception ex)
                {
                    Log.Error(ex,"Exception");
                    throw;
                }
            }
        }
    }
}
