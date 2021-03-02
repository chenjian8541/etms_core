using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("SyncStudentAccountRechargeLogPhoneQueue")]
    public class SyncStudentAccountRechargeLogPhoneConsumer : ConsumerBase<SyncStudentAccountRechargeLogPhoneEvent>
    {
        protected override async Task Receive(SyncStudentAccountRechargeLogPhoneEvent request)
        {
            var jobAnalyzeBLL = CustomServiceLocator.GetInstance<IJobAnalyzeBLL>();
            jobAnalyzeBLL.InitTenantId(request.TenantId);
            await jobAnalyzeBLL.SyncStudentAccountRechargeLogPhoneConsumerEvent(request);
        }
    }
}