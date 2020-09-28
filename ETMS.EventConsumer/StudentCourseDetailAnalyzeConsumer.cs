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
    [QueueConsumerAttribution("StudentCourseDetailAnalyzeQueue")]
    public class StudentCourseDetailAnalyzeConsumer : ConsumerBase<StudentCourseDetailAnalyzeEvent>
    {
        protected override async Task Receive(StudentCourseDetailAnalyzeEvent eEvent)
        {
            var distributedLockDAL = CustomServiceLocator.GetInstance<IDistributedLockDAL>();
            var lockKey = new StudentCourseDetailAnalyzeToken(eEvent.TenantId, eEvent.StudentId, eEvent.CourseId);
            if (distributedLockDAL.LockTake(lockKey))
            {
                try
                {
                    var studentCourseAnalyzeBLL = CustomServiceLocator.GetInstance<IStudentCourseAnalyzeBLL>();
                    studentCourseAnalyzeBLL.InitTenantId(eEvent.TenantId);
                    await studentCourseAnalyzeBLL.CourseDetailAnalyze(eEvent);
                }
                finally
                {
                    distributedLockDAL.LockRelease(lockKey);
                }
            }
            else
            {
                Log.Debug($"【StudentCourseDetailAnalyzeConsumer】丢弃请求,参数:{JsonConvert.SerializeObject(eEvent)}", this.GetType());
            }
        }
    }
}
