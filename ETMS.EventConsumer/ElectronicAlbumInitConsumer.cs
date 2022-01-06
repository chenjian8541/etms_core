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
    [QueueConsumerAttribution("ElectronicAlbumInitQueue")]
    public class ElectronicAlbumInitConsumer : ConsumerBase<ElectronicAlbumInitEvent>
    {
        protected override async Task Receive(ElectronicAlbumInitEvent eEvent)
        {
            var _lockKey = new ElectronicAlbumInitToken(eEvent.TenantId, eEvent.MyElectronicAlbum.Id);
            var evInteractionBLL = CustomServiceLocator.GetInstance<IEvInteractionBLL>();
            evInteractionBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<ElectronicAlbumInitToken, ElectronicAlbumInitEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await evInteractionBLL.ElectronicAlbumInitConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();
        }
    }
}
