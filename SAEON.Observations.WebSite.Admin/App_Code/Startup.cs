using SAEON.Logs;
using SAEON.Observations.Azure;
using System.Configuration;
using System.IO;
using System.Web.Hosting;

/// <summary>
/// Summary description for Startup
/// </summary>
public static class Startup
{
    public static void Run()
    {
        SAEONLogs
            .CreateConfiguration(HostingEnvironment.MapPath(@"~/App_Data/Logs/SAEON.Observations.WebSite.Admin {Date}.txt"))
            .Initialize();
        string docPath = ConfigurationManager.AppSettings["DocumentsPath"];
        if (!string.IsNullOrEmpty(docPath))
        {
            string path = HostingEnvironment.MapPath(docPath);
            if (!string.IsNullOrEmpty(path))
            {
                Directory.CreateDirectory(Path.Combine(path, "Uploads"));
                //Directory.CreateDirectory(Path.Combine(path, "Downloads"));
            }
        }
        new ObservationsAzure().Initialize();
    }
}