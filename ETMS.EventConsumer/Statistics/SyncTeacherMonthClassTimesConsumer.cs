using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Statistics;
using ETMS.EventConsumer.Lib;
using ETMS.IBusiness;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer.Statistics
{
    [QueueConsumerAttribution("SyncTeacherMonthClassTimesQueue")]
    public class SyncTeacherMonthClassTimesConsumer : ConsumerBase<SyncTeacherMonthClassTimesEvent>
    {
        protected override async Task Receive(SyncTeacherMonthClassTimesEvent eEvent)
        {
            var evEducationBLL = CustomServiceLocator.GetInstance<IEvEducationBLL>();
            evEducationBLL.InitTenantId(eEvent.TenantId);
            await evEducationBLL.SyncTeacherMonthClassTimesConsumerEvent(eEvent);
        }
    }
}
