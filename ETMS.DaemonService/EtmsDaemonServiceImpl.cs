using ETMS.LOG;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ETMS.DaemonService
{
    public class EtmsDaemonServiceImpl : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                Log.Info("[服务]正在启动消费者守护程序...", this.GetType());
                Console.WriteLine("[服务]正在启动消费者守护程序...");
                ServiceProvider.Process();
                Log.Info("[服务]启动成功...", this.GetType());
                Console.WriteLine("[服务]启动成功...");
            }
            catch (Exception ex)
            {
                Log.Error($"[服务]启动失败:{ex.Message}", ex, typeof(Program));
                Console.WriteLine(ex.Message);
                throw ex;
            }
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Info("[服务]停止了", this.GetType());
            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            Log.Info("[服务]Dispose", this.GetType());
            base.Dispose();
        }
    }
}
