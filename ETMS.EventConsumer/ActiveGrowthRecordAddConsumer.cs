using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("ActiveGrowthRecordClassAddQueue")]
    public class ActiveGrowthRecordAddConsumer : ConsumerBase<ActiveGrowthRecordAddEvent>
    {
        protected override async Task Receive(ActiveGrowthRecordAddEvent eEvent)
        {
            var activeGrowthRecordBLL = CustomServiceLocator.GetInstance<IActiveGrowthRecordBLL>();
            activeGrowthRecordBLL.InitTenantId(eEvent.TenantId);
            await activeGrowthRecordBLL.ActiveGrowthRecordAddConsumerEvent(eEvent);
        }
    }
}
