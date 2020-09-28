using System;
using ETMS.LOG;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ETMS.DaemonService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[服务]准备开始启动...");
            Log.Info("[服务]准备开始启动...", typeof(Program));
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .ConfigureServices((hostContext, services) =>
            {
                hostContext.HostingEnvironment.ContentRootPath = AppContext.BaseDirectory;
                services.AddHostedService<EtmsDaemonServiceImpl>();
            });
    }
}
