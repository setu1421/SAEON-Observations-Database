using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAEON.Core;
using SAEON.Logs;
using System;
using System.Threading;

namespace SAEON.Observations.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SAEONLogs.CreateConfiguration().Initialize();
            try
            {
                SAEONLogs.Information("Starting {ApplicationName}", ApplicationHelper.ApplicationName);
                using (Mutex mutex = new Mutex(true, ApplicationHelper.ApplicationName, out bool isFirst))
                {
                    if (isFirst)
                    {
                        CreateHostBuilder(args).Build().Run();
                        mutex.ReleaseMutex();
                    }
                    else
                    {
                        SAEONLogs.Warning("Application already running, shutting down.");
                    }
                }
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                throw;
            }
            finally
            {
                SAEONLogs.ShutDown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddJsonFile("secrets.json", optional: true, reloadOnChange: true);
                })
                .UseSAEONLogs()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                });
    }
}
