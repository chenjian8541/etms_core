using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.IDataAccess;
using ETMS.IOC;
using ETMS.LOG;
using ETMS.Manage.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Manage.Jobs
{
    public abstract class BaseJob : IJob
    {
        public void Execute(JobExecutionContext context)
        {
            if (context.JobConfig.IsCanParallelProcess)
            {
                PreProcess(context).Wait();
            }
            else
            {
                var lockKey = new RecurringJobToken(context.JobConfig.Name);
                var distributedLockDAL = CustomServiceLocator.GetInstance<IDistributedLockDAL>();
                if (distributedLockDAL.LockTake(lockKey))
                {
                    try
                    {
                        PreProcess(context).Wait();
                    }
                    finally
                    {
                        distributedLockDAL.LockRelease(lockKey);
                    }
                }
                else
                {
                    Log.Warn($"【{context.JobConfig.Title}】无法并行执行，任务被丢弃", this.GetType());
                }
            }
        }

        private async Task PreProcess(JobExecutionContext context)
        {
            try
            {
                Log.Debug($"【{context.JobConfig.Title}】Job准备执行", this.GetType());
                await Process(context);
                Log.Debug($"【{context.JobConfig.Title}】Job执行完成", this.GetType());
            }
            catch (Exception ex)
            {
                Log.Error($"【{context.JobConfig.Title}】Job执行错误", ex, this.GetType());
                if (context.JobConfig.IsCanTryAgain)
                {
                    throw ex;
                }
            }
        }

        public abstract Task Process(JobExecutionContext context);
    }
}
