using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("SysTenantStudentAnalysisAddQueue")]
    public class SysTenantStudentAnalysisAddConsumer : ConsumerBase<SysTenantStudentAnalysisAddEvent>
    {
        protected override async Task Receive(SysTenantStudentAnalysisAddEvent eEvent)
        {
            var sysTenantPeopleAnalysisBLL = CustomServiceLocator.GetInstance<ISysTenantPeopleAnalysisBLL>();
            sysTenantPeopleAnalysisBLL.InitTenantId(eEvent.TenantId);
            await sysTenantPeopleAnalysisBLL.SysTenantStudentAnalysisAddConsumerEvent(eEvent);
        }
    }
}
