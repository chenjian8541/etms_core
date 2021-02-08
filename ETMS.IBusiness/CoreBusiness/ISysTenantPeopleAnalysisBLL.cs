using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface ISysTenantPeopleAnalysisBLL: IBaseBLL
    {
        Task SysTenantUserAnalysisConsumerEvent(SysTenantUserAnalysisEvent request);

        Task SysTenantUserAnalysisAddConsumerEvent(SysTenantUserAnalysisAddEvent request);

        Task SysTenantUserAnalysisRemoveConsumerEvent(SysTenantUserAnalysisRemoveEvent request);

        Task SysTenantStudentAnalysisConsumerEvent(SysTenantStudentAnalysisEvent request);

        Task SysTenantStudentAnalysisAddConsumerEvent(SysTenantStudentAnalysisAddEvent request);

        Task SysTenantStudentAnalysisRemoveConsumerEvent(SysTenantStudentAnalysisRemoveEvent request);
    }
}
