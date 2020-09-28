using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IOC;
using ETMS.LOG;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("ConsumeStudentCourseQueue")]
    public class ConsumeStudentCourseConsumer : ConsumerBase<ConsumeStudentCourseEvent>
    {
        protected override async Task Receive(ConsumeStudentCourseEvent eEvent)
        {
            var analyzeClassTimesBLL = CustomServiceLocator.GetInstance<IJobAnalyzeBLL>();
            analyzeClassTimesBLL.InitTenantId(eEvent.TenantId);
            var lockKey = new ConsumeStudentCourseToken(eEvent.TenantId, eEvent.StudentCourseDetailId);
            var distributedLockDAL = CustomServiceLocator.GetInstance<IDistributedLockDAL>();
            if (distributedLockDAL.LockTake(lockKey))
            {
                try
                {
                    await analyzeClassTimesBLL.ConsumeStudentCourseProcessEvent(eEvent);
                }
                finally
                {
                    distributedLockDAL.LockRelease(lockKey);
                }
            }
            else
            {
                Log.Debug($"【ConsumeStudentCourseConsumer】丢弃请求,参数:{JsonConvert.SerializeObject(eEvent)}", this.GetType());
            }
        }
    }
}
