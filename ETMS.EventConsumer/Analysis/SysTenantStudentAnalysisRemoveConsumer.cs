using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("SysTenantStudentAnalysisRemoveQueue")]
    public class SysTenantStudentAnalysisRemoveConsumer : ConsumerBase<SysTenantStudentAnalysisRemoveEvent>
    {
        protected override async Task Receive(SysTenantStudentAnalysisRemoveEvent eEvent)
        {
            var sysTenantPeopleAnalysisBLL = CustomServiceLocator.GetInstance<ISysTenantPeopleAnalysisBLL>();
            sysTenantPeopleAnalysisBLL.InitTenantId(eEvent.TenantId);
            await sysTenantPeopleAnalysisBLL.SysTenantStudentAnalysisRemoveConsumerEvent(eEvent);
        }
    }
}
