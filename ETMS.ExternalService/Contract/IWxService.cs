using ETMS.Entity.ExternalService.Dto.Request;
using ETMS.Entity.ExternalService.Dto.Request.User;
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

        void HomeworkAdd(HomeworkAddRequest request);

        void HomeworkEdit(HomeworkEditRequest request);

        void HomeworkExpireRemind(HomeworkExpireRemindRequest request);

        void HomeworkComment(HomeworkCommentRequest request);

        void GrowthRecordAdd(GrowthRecordAddRequest request);

        void WxMessage(WxMessageRequest request);

        void StudentEvaluate(StudentEvaluateRequest request);

        void StudentCourseSurplus(StudentCourseSurplusRequest request);

        void StudentMakeup(StudentMakeupRequest request);

        //void NoticeStudentCourseNotEnough(NoticeStudentCourseNotEnoughRequest request);

        void NoticeStudentCourseNotEnough2(NoticeStudentCourseNotEnoughRequest request);

        void NoticeStudentCourseNotEnough3(NoticeStudentCourseNotEnoughRequest request);

        void NoticeUserOfClassToday(NoticeUserOfClassTodayRequest request);

        void NoticeTeacherOfHomeworkFinish(NoticeTeacherOfHomeworkFinishRequest request);

        void NoticeStudentCheckIn(NoticeStudentCheckInRequest request);

        void NoticeStudentCheckOut(NoticeStudentCheckOutRequest request);

        void NoticeStudentCouponsGet(NoticeStudentCouponsGetRequest request);

        void NoticeStudentCouponsExplain(NoticeStudentCouponsExplainRequest request);

        void NoticeUserOfStudentTryClassFinish(NoticeUserOfStudentTryClassFinishRequest request);

        void NoticeStudentAccountRechargeChanged(NoticeStudentAccountRechargeChangedRequest request);

        void NoticeStudentOrUserReservation(NoticeStudentOrUserReservationRequest request);

        void NoticeStudentCustomizeMsg(NoticeStudentCustomizeMsgRequest request);

        void NoticeUserMessage(NoticeUserMessageRequest request);

        void NoticeStudentMessage(NoticeStudentMessageRequest request);

        void NoticeStudentArrearage(NoticeStudentArrearageRequest request);
    }
}
