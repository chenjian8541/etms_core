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

        void HomeworkExpireRemind(HomeworkExpireRemindRequest request);

        void HomeworkComment(HomeworkCommentRequest request);

        void GrowthRecordAdd(GrowthRecordAddRequest request);

        void WxMessage(WxMessageRequest request);

        void StudentEvaluate(StudentEvaluateRequest request);

        void StudentCourseSurplus(StudentCourseSurplusRequest request);

        void StudentMakeup(StudentMakeupRequest request);

        void NoticeStudentCourseNotEnough(NoticeStudentCourseNotEnoughRequest request);

        void NoticeUserOfClassToday(NoticeUserOfClassTodayRequest request);

        void NoticeTeacherOfHomeworkFinish(NoticeTeacherOfHomeworkFinishRequest request);
    }
}
