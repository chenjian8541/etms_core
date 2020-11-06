using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IStudentSendNoticeBLL : IBaseBLL
    {
        Task NoticeStudentsOfClassBeforeDayTenant(NoticeStudentsOfClassBeforeDayTenantEvent request);

        Task NoticeStudentsOfClassBeforeDayClassTimes(NoticeStudentsOfClassBeforeDayTimesEvent request);

        Task NoticeStudentsOfClassTodayGenerate(NoticeStudentsOfClassTodayGenerateEvent request);

        Task NoticeStudentsOfClassTodayTenant(NoticeStudentsOfClassTodayTenantEvent request);

        Task NoticeStudentsOfClassTodayClassTimes(NoticeStudentsOfClassTodayClassTimesEvent request);

        Task NoticeStudentsCheckSign(NoticeStudentsCheckSignEvent request);

        Task NoticeStudentLeaveApply(NoticeStudentLeaveApplyEvent request);

        Task NoticeStudentContracts(NoticeStudentContractsEvent request);

        Task NoticeStudentsOfWxMessageConsumerEvent(NoticeStudentsOfWxMessageEvent request);
    }
}
