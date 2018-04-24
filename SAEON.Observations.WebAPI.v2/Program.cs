using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SAEON.Observations.Core.Entities;
using Serilog;
using System;
using System.Linq;

namespace SAEON.Observations.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights()
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddJsonFile("connectionStrings.json", optional: false, reloadOnChange: true);
                })
                .UseStartup<Startup>()
                .UseSerilog()
                .Build();

    }
}
