using ETMS.Event.DataContract.Activity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.EventConsumer
{
    public interface IEvActivityBLL: IBaseBLL
    {
        Task SyncActivityBehaviorCountConsumerEvent(SyncActivityBehaviorCountEvent request);

        Task SyncActivityEffectCountConsumerEvent(SyncActivityEffectCountEvent request);

        Task SyncActivityRouteFinishCountConsumerEvent(SyncActivityRouteFinishCountEvent request);

        Task SyncActivityBascInfoConsumerEvent(SyncActivityBascInfoEvent request);
    }
}
