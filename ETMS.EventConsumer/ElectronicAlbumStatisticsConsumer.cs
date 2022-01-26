using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.EventConsumer.Lib;
using ETMS.IBusiness;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("ElectronicAlbumStatisticsQueue")]
    public class ElectronicAlbumStatisticsConsumer : ConsumerBase<ElectronicAlbumStatisticsEvent>
    {
        protected override async Task Receive(ElectronicAlbumStatisticsEvent eEvent)
        {
            var _lockKey = new ElectronicAlbumStatisticsToken(eEvent.TenantId, eEvent.ElectronicAlbumDetailId);
            var evInteractionBLL = CustomServiceLocator.GetInstance<IEvInteractionBLL>();
            evInteractionBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<ElectronicAlbumStatisticsToken, ElectronicAlbumStatisticsEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await evInteractionBLL.ElectronicAlbumStatisticsConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();
        }
    }
}
