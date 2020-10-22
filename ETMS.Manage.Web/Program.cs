using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ETMS.Manage.Web.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ETMS.Manage.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
                   .UseWindowsService()
                   .UseServiceProviderFactory(new EtmsServiceProviderFactory())
                       .ConfigureServices((hostContext, services) =>
                       {
                           hostContext.HostingEnvironment.ContentRootPath = AppContext.BaseDirectory;
                           services.AddHostedService<EtmsJobService>();
                       });

            var config = new ConfigurationBuilder()
                .AddJsonFile("hosting.json")
                .Build();

            hostBuilder.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseConfiguration(config);
                webBuilder.UseStartup<Startup>();
            });
            return hostBuilder;
        }
    }
}
