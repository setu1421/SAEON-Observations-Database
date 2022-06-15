using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SAEON.Core;
using SAEON.Logs;
using System;

namespace SAEON.Observations.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SAEONLogs.CreateConfiguration().Initialize();
            try
            {
                SAEONLogs.Information("Starting {Application}", ApplicationHelper.ApplicationName);
                CreateHostBuilder(args).Build().Run();
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
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(30);
                    });
                });
    }
}
