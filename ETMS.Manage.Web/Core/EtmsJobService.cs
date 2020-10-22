using ETMS.LOG;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ETMS.Manage.Web.Core
{
    public class EtmsJobService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Info("EtmsJobService is starting.", this.GetType());

            stoppingToken.Register(() => Log.Info("EtmsJobService is stopping.", this.GetType()));

            while (!stoppingToken.IsCancellationRequested)
            {
                Log.Info("EtmsJobService is doing background work.", this.GetType());
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }

            Log.Info("EtmsJobService has stopped.", this.GetType());
        }
    }
}
