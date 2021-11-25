using ETMS.Event.DataContract;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("SyncHomeworkReadAndFinishCountQueue")]
    public class SyncHomeworkReadAndFinishCountConsumer : ConsumerBase<SyncHomeworkReadAndFinishCountEvent>
    {
        protected override async Task Receive(SyncHomeworkReadAndFinishCountEvent request)
        {
            var homeworkBLL = CustomServiceLocator.GetInstance<IEvHomeworkBLL>();
            homeworkBLL.InitTenantId(request.TenantId);
            await homeworkBLL.SyncHomeworkReadAndFinishCountConsumerEvent(request);
        }
    }
}
