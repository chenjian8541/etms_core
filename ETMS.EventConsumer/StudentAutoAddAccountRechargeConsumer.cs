using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.EventConsumer.Lib;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("StudentAutoAddAccountRechargeQueue")]
    public class StudentAutoAddAccountRechargeConsumer : ConsumerBase<StudentAutoAddAccountRechargeEvent>
    {
        protected override async Task Receive(StudentAutoAddAccountRechargeEvent eEvent)
        {
            var _lockKey = new SyncClassTimesStudentConsumerToken(eEvent.TenantId, eEvent.StudentId);
            var evStudentAccountRechargeBLL = CustomServiceLocator.GetInstance<IEvStudentAccountRechargeBLL>();
            evStudentAccountRechargeBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<SyncClassTimesStudentConsumerToken, StudentAutoAddAccountRechargeEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await evStudentAccountRechargeBLL.StudentAutoAddAccountRechargeConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();
        }
    }
}
