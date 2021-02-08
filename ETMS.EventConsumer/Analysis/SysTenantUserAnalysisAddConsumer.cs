using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("SysTenantUserAnalysisAddQueue")]
    public class SysTenantUserAnalysisAddConsumer : ConsumerBase<SysTenantUserAnalysisAddEvent>
    {
        protected override async Task Receive(SysTenantUserAnalysisAddEvent eEvent)
        {
            var sysTenantPeopleAnalysisBLL = CustomServiceLocator.GetInstance<ISysTenantPeopleAnalysisBLL>();
            sysTenantPeopleAnalysisBLL.InitTenantId(eEvent.TenantId);
            await sysTenantPeopleAnalysisBLL.SysTenantUserAnalysisAddConsumerEvent(eEvent);
        }
    }
}