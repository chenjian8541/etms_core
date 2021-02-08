using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("SysTenantUserAnalysisRemoveQueue")]
    public class SysTenantUserAnalysisRemoveConsumer : ConsumerBase<SysTenantUserAnalysisRemoveEvent>
    {
        protected override async Task Receive(SysTenantUserAnalysisRemoveEvent eEvent)
        {
            var sysTenantPeopleAnalysisBLL = CustomServiceLocator.GetInstance<ISysTenantPeopleAnalysisBLL>();
            sysTenantPeopleAnalysisBLL.InitTenantId(eEvent.TenantId);
            await sysTenantPeopleAnalysisBLL.SysTenantUserAnalysisRemoveConsumerEvent(eEvent);
        }
    }
}
