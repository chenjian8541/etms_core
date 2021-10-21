using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("SyncMallGoodsRelatedNameQueue")]
    public class SyncMallGoodsRelatedNameConsumer : ConsumerBase<SyncMallGoodsRelatedNameEvent>
    {
        protected override async Task Receive(SyncMallGoodsRelatedNameEvent request)
        {
            var evEducationBLL = CustomServiceLocator.GetInstance<IEvEducationBLL>();
            evEducationBLL.InitTenantId(request.TenantId);
            await evEducationBLL.SyncMallGoodsRelatedNameConsumerEvent(request);
        }
    }
}

