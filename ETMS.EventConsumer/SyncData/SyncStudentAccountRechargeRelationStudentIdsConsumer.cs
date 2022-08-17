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
    [QueueConsumerAttribution("SyncStudentAccountRechargeRelationStudentIdsQueue")]
    public class SyncStudentAccountRechargeRelationStudentIdsConsumer : ConsumerBase<SyncStudentAccountRechargeRelationStudentIdsEvent>
    {
        protected override async Task Receive(SyncStudentAccountRechargeRelationStudentIdsEvent eEvent)
        {
            var evStudentAccountRechargeBLL = CustomServiceLocator.GetInstance<IEvStudentAccountRechargeBLL>();
            evStudentAccountRechargeBLL.InitTenantId(eEvent.TenantId);
            await evStudentAccountRechargeBLL.SyncStudentAccountRechargeRelationStudentIdsConsumerEvent(eEvent);
        }
    }
}
