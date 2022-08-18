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

namespace ETMS.EventConsumer.SyncData
{
    [QueueConsumerAttribution("SyncClassTimesRuleStudentInfoQueue")]
    public class SyncClassTimesRuleStudentInfoConsumer : ConsumerBase<SyncClassTimesRuleStudentInfoEvent>
    {
        protected override async Task Receive(SyncClassTimesRuleStudentInfoEvent eEvent)
        {
            var _lockKey = new SyncClassTimesRuleStudentInfoToken(eEvent.TenantId, eEvent.ClassId, eEvent.RuleId);
            var evClassBLL = CustomServiceLocator.GetInstance<IEvClassBLL>();
            evClassBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<SyncClassTimesRuleStudentInfoToken, SyncClassTimesRuleStudentInfoEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await evClassBLL.SyncClassTimesRuleStudentInfoConsumerEvent(eEvent)
                , false);
            await lockTakeHandler.Process();
        }
    }
}
