using ETMS.Event.DataContract.Achievement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.EventConsumer
{
    public interface IEvAchievementBLL : IBaseBLL
    {
        Task SyncAchievementAllConsumerEvent(SyncAchievementAllEvent request);
    }
}
