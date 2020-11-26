using ETMS.Entity.ExternalService.Dto.Output;
using ETMS.Entity.ExternalService.Dto.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.ExternalService.Contract
{
    public interface ISmsService
    {
        Task<SmsOutput> UserLogin(SmsUserLoginRequest request);

        Task<SmsOutput> ParentLogin(SmsParentLoginRequest request);

        Task<SmsOutput> ClearData(SmsClearDataRequest request);

        Task<SmsOutput> NoticeStudentsOfClassBeforeDay(NoticeStudentsOfClassBeforeDayRequest request);

        Task<SmsOutput> NoticeStudentsOfClassToday(NoticeStudentsOfClassTodayRequest request);

        Task<SmsOutput> NoticeClassCheckSign(NoticeClassCheckSignRequest request);

        Task<SmsOutput> NoticeStudentLeaveApply(NoticeStudentLeaveApplyRequest request);

        Task<SmsOutput> NoticeStudentContracts(NoticeStudentContractsRequest request);
    }
}
