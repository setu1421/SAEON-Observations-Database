using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SAEON.Observations.Core.Entities;
using System;
using System.Linq;

namespace SAEON.Observations.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            using (var scope = host.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                try
                {
                    // Requires using RazorPagesMovie.Models;
                    InitializeDb(serviceProvider);
                }
                catch (Exception ex)
                {
                    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }
            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights()
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddJsonFile("connectionStrings.json", optional: false, reloadOnChange: true);
                })
                .UseStartup<Startup>()
                .Build();

        public static void InitializeDb(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<BlogDbContext>();
            context.Tags.Add(new Tag { TagId = "Tim" });
            context.Tags.Add(new Tag { TagId = "Shirley" });
            context.Posts.Add(new Post { PostId = 1, Title = "Spring has sprung", Content = "At last spring has sprung" });
            context.Posts.Add(new Post { PostId = 2, Title = "Winter is on the way", Content = "Winter is on the way" });
            context.SaveChanges();
            var post1 = context.Posts.FirstOrDefault();
            var post2 = context.Posts.LastOrDefault();
            var tag1 = context.Tags.FirstOrDefault();
            var tag2 = context.Tags.LastOrDefault();
            context.PostTags.Add(new PostTag { PostId = post1.PostId, TagId = tag1.TagId });
            context.PostTags.Add(new PostTag { PostId = post2.PostId, TagId = tag1.TagId });
            context.PostTags.Add(new PostTag { PostId = post2.PostId, TagId = tag2.TagId });
            context.SaveChanges();
        }
    }
}
