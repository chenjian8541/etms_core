using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.EventConsumer.Lib;
using ETMS.IBusiness;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("SyncStudentLastGoClassTimeQueue")]
    public class SyncStudentLastGoClassTimeConsumer : ConsumerBase<SyncStudentLastGoClassTimeEvent>
    {
        protected override async Task Receive(SyncStudentLastGoClassTimeEvent eEvent)
        {
            var _lockKey = new SyncStudentLastGoClassTimeToken(eEvent.TenantId, eEvent.StudentId);
            var evStudentBLL = CustomServiceLocator.GetInstance<IEvStudentBLL>();
            evStudentBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<SyncStudentLastGoClassTimeToken, SyncStudentLastGoClassTimeEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await evStudentBLL.SyncStudentLastGoClassTimeConsumerEvent(eEvent)
                , false);
            await lockTakeHandler.Process();
        }
    }
}
