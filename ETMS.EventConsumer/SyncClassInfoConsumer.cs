using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("SyncClassInfoQueue")]
    public class SyncClassInfoConsumer : ConsumerBase<SyncClassInfoEvent>
    {
        protected override async Task Receive(SyncClassInfoEvent eEvent)
        {
            var classBLL = CustomServiceLocator.GetInstance<IClassBLL>();
            classBLL.InitTenantId(eEvent.TenantId);
            await classBLL.SyncClassInfoProcessEvent(eEvent);
        }
    }
}
