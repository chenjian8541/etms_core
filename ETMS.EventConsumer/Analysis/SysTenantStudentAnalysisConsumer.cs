using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("SysTenantStudentAnalysisQueue")]
    public class SysTenantStudentAnalysisConsumer : ConsumerBase<SysTenantStudentAnalysisEvent>
    {
        protected override async Task Receive(SysTenantStudentAnalysisEvent eEvent)
        {
            var sysTenantPeopleAnalysisBLL = CustomServiceLocator.GetInstance<ISysTenantPeopleAnalysisBLL>();
            sysTenantPeopleAnalysisBLL.InitTenantId(eEvent.TenantId);
            await sysTenantPeopleAnalysisBLL.SysTenantStudentAnalysisConsumerEvent(eEvent);
        }
    }
}