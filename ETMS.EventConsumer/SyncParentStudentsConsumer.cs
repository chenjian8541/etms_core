using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("SyncParentStudentsQueue")]
    public class SyncParentStudentsConsumer : ConsumerBase<SyncParentStudentsEvent>
    {
        protected override async Task Receive(SyncParentStudentsEvent request)
        {
            var jobAnalyzeBLL = CustomServiceLocator.GetInstance<IJobAnalyzeBLL>();
            jobAnalyzeBLL.InitTenantId(request.TenantId);
            await jobAnalyzeBLL.SyncParentStudentsConsumerEvent(request);
        }
    }
}
