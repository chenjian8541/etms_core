using ETMS.Entity.ExternalService.Dto.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.ExternalService.Contract
{
    public interface IWxService
    {
        void NoticeStudentsOfClassBeforeDay(NoticeStudentsOfClassBeforeDayRequest request);

        void NoticeStudentsOfClassToday(NoticeStudentsOfClassTodayRequest request);

        void NoticeClassCheckSign(NoticeClassCheckSignRequest request);

        void NoticeStudentLeaveApply(NoticeStudentLeaveApplyRequest request);

        void NoticeStudentContracts(NoticeStudentContractsRequest request);
    }
}
