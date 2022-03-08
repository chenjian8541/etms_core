using ETMS.Event.DataContract;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("SyncClassCategoryIdQueue")]
    public class SyncClassCategoryIdConsumer : ConsumerBase<SyncClassCategoryIdEvent>
    {
        protected override async Task Receive(SyncClassCategoryIdEvent @event)
        {
            var evClassBLL = CustomServiceLocator.GetInstance<IEvClassBLL>();
            evClassBLL.InitTenantId(@event.TenantId);
            await evClassBLL.SyncClassCategoryIdConsumerEvent(@event);
        }
    }
}
