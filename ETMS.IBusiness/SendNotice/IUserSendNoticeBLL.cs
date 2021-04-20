using ETMS.Entity.Database.Source;
using ETMS.Entity.ExternalService.Dto.Request;
using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.SendNotice
{
    public interface IUserSendNoticeBLL : IBaseBLL
    {
        Task NoticeUserOfClassTodayGenerateConsumerEvent(NoticeUserOfClassTodayGenerateEvent request);

        Task NoticeTeacherOfClassTodayTenantConsumerEvent(NoticeTeacherOfClassTodayTenantEvent request);

        Task NoticeTeacherOfClassTodayClassTimesConsumerEvent(NoticeTeacherOfClassTodayClassTimesEvent request);

        Task NoticeTeacherOfHomeworkFinishConsumerEvent(NoticeTeacherOfHomeworkFinishEvent request);

        Task NoticeUserOfStudentTryClassFinishConsumerEvent(NoticeUserOfStudentTryClassFinishEvent request);

        Task NoticeTeacherStudentReservation(NoticeStudentReservationEvent request, NoticeStudentOrUserReservationRequest req, EtClassTimes classTimes, EtStudent student);

        Task NoticeUserStudentLeaveApplyConsumerEvent(NoticeUserStudentLeaveApplyEvent request);

        Task NoticeUserContractsNotArrivedConsumerEvent(NoticeUserContractsNotArrivedEvent request);

        Task NoticeUserTryCalssApplyConsumerEvent(NoticeUserTryCalssApplyEvent request);
    }
}
