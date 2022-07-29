using ETMS.Event.DataContract.Achievement;
using ETMS.IBusiness.EventConsumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EventConsumer
{
    public class EvAchievementBLL : IEvAchievementBLL
    {
        public void InitTenantId(int tenantId)
        { }

        public async Task SyncAchievementAllConsumerEvent(SyncAchievementAllEvent request)
        { }
    }
}
