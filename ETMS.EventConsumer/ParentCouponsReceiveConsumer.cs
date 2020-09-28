using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("ParentCouponsReceiveQueue")]
    public class ParentCouponsReceiveConsumer : ConsumerBase<ParentCouponsReceiveEvent>
    {
        protected override async Task Receive(ParentCouponsReceiveEvent eEvent)
        {
            var couponsBLL = CustomServiceLocator.GetInstance<ICouponsBLL>();
            couponsBLL.InitTenantId(eEvent.TenantId);
            await couponsBLL.ParentCouponsReceiveEvent(eEvent);
        }
    }
}
