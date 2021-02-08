using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("SysTenantUserAnalysisQueue")]
    public class SysTenantUserAnalysisConsumer : ConsumerBase<SysTenantUserAnalysisEvent>
    {
        protected override async Task Receive(SysTenantUserAnalysisEvent eEvent)
        {
            var sysTenantPeopleAnalysisBLL = CustomServiceLocator.GetInstance<ISysTenantPeopleAnalysisBLL>();
            sysTenantPeopleAnalysisBLL.InitTenantId(eEvent.TenantId);
            await sysTenantPeopleAnalysisBLL.SysTenantUserAnalysisConsumerEvent(eEvent);
        }
    }
}