using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IBusiness.Parent;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("ParentBuyMallGoodsPaySuccessQueue")]
    public class ParentBuyMallGoodsPaySuccessConsumer : ConsumerBase<ParentBuyMallGoodsPaySuccessEvent>
    {
        protected override async Task Receive(ParentBuyMallGoodsPaySuccessEvent request)
        {
            var parentData4BLL = CustomServiceLocator.GetInstance<IParentData4BLL>();
            parentData4BLL.InitTenantId(request.TenantId);
            await parentData4BLL.ParentBuyMallGoodsPaySuccessConsumerEvent(request);
        }
    }
}
