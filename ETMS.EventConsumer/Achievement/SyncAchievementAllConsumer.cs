using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Achievement;
using ETMS.EventConsumer.Lib;
using ETMS.IBusiness;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer.Achievement
{
    [QueueConsumerAttribution("SyncAchievementAllQueue")]
    public class SyncAchievementAllConsumer : ConsumerBase<SyncAchievementAllEvent>
    {
        protected override async Task Receive(SyncAchievementAllEvent eEvent)
        {
            var _lockKey = new SyncAchievementAllToken(eEvent.TenantId, eEvent.AchievementId);
            var evAchievementBLL = CustomServiceLocator.GetInstance<IEvAchievementBLL>();
            evAchievementBLL.InitTenantId(eEvent.TenantId);
            var lockTakeHandler = new LockTakeHandler<SyncAchievementAllToken, SyncAchievementAllEvent>(_lockKey, eEvent, this.ClassName,
                async () =>
                await evAchievementBLL.SyncAchievementAllConsumerEvent(eEvent)
                );
            await lockTakeHandler.Process();

        }
    }
}
