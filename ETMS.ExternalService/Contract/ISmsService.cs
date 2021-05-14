using ETMS.Entity.ExternalService.Dto.Output;
using ETMS.Entity.ExternalService.Dto.Request;
using ETMS.Entity.ExternalService.Dto.Request.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.ExternalService.Contract
{
    public interface ISmsService
    {
        Task<SmsOutput> AddSmsSign(AddSmsSignRequest request);

        Task<SmsOutput> UserLogin(SmsUserLoginRequest request);

        Task<SmsOutput> ParentLogin(SmsParentLoginRequest request);

        Task<SmsOutput> SysSafe(SmsSysSafeRequest request);

        Task<SmsOutput> NoticeStudentsOfClassBeforeDay(NoticeStudentsOfClassBeforeDayRequest request);

        Task<SmsOutput> NoticeStudentsOfClassToday(NoticeStudentsOfClassTodayRequest request);

        Task<SmsOutput> NoticeClassCheckSign(NoticeClassCheckSignRequest request);

        Task<SmsOutput> NoticeStudentLeaveApply(NoticeStudentLeaveApplyRequest request);

        Task<SmsOutput> NoticeStudentContracts(NoticeStudentContractsRequest request);

        Task<SmsOutput> NoticeStudentCourseNotEnough(NoticeStudentCourseNotEnoughRequest request);

        Task<SmsOutput> NoticeUserOfClassToday(NoticeUserOfClassTodayRequest request);

        Task<SmsOutput> NoticeStudentCheckIn(NoticeStudentCheckInRequest request);

        Task<SmsOutput> NoticeStudentCheckOut(NoticeStudentCheckOutRequest request);

        Task<SmsOutput> NoticeStudentAccountRechargeChanged(NoticeStudentAccountRechargeChangedRequest request);
    }
}
