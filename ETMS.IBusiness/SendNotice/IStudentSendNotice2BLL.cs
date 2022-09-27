using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.SendNotice
{
    public interface IStudentSendNotice2BLL : IBaseBLL
    {
        Task NoticeStudentsOfHomeworkAddConsumeEvent(NoticeStudentsOfHomeworkAddEvent request);

        Task NoticeStudentsOfHomeworkEditConsumeEvent(NoticeStudentsOfHomeworkEditEvent request);

        Task NoticeStudentsOfHomeworkAddCommentConsumeEvent(NoticeStudentsOfHomeworkAddCommentEvent request);

        Task NoticeStudentsOfGrowthRecordConsumeEvent(NoticeStudentsOfGrowthRecordEvent request);

        Task NoticeStudentsOfGrowthRecordEditConsumerEvent(NoticeStudentsOfGrowthRecordEditEvent request);

        Task NoticeStudentsOfStudentEvaluateConsumeEvent(NoticeStudentsOfStudentEvaluateEvent request);

        Task NoticeStudentsOfHomeworkExDateConsumeEvent(NoticeStudentsOfHomeworkExDateEvent request);

        Task NoticeStudentsOfHomeworkNotAnswerConsumeEvent(NoticeStudentsOfHomeworkNotAnswerEvent request);

        Task NoticeStudentCourseSurplusConsumerEvent(NoticeStudentCourseSurplusEvent request);

        Task NoticeStudentsOfMakeupConsumerEvent(NoticeStudentsOfMakeupEvent request);

        Task NoticeStudentCourseNotEnoughConsumerEvent(NoticeStudentCourseNotEnoughEvent request);

        Task NoticeStudentsCheckOnConsumerEvent(NoticeStudentsCheckOnEvent request);

        Task NoticeSendArrearageBatchConsumerEvent(NoticeSendArrearageBatchEvent request);

        void NoticeStudentCourseNotEnoughBatchConsumerEvent(NoticeStudentCourseNotEnoughBatchEvent request);
    }
}
