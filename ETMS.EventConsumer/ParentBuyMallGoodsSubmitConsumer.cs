using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.EventConsumer.Lib;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("ParentBuyMallGoodsSubmitQueue")]
    public class ParentBuyMallGoodsSubmitConsumer : ConsumerBase<ParentBuyMallGoodsSubmitEvent>
    {
        protected override async Task Receive(ParentBuyMallGoodsSubmitEvent eEvent)
        {
            var _lockKey = new ParentBuyMallGoodsSubmitToken(eEvent.TenantId, eEvent.MyStudent.Id);
            var studentContractsSelfHelpBLL = CustomServiceLocator.GetInstance<IStudentContractsSelfHelpBLL>();
            studentContractsSelfHelpBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<ParentBuyMallGoodsSubmitToken, ParentBuyMallGoodsSubmitEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await studentContractsSelfHelpBLL.ParentBuyMallGoodsSubmitConsumerEvent(eEvent)
                , false);
            await lockTakeHandler.Process();
        }
    }
}
