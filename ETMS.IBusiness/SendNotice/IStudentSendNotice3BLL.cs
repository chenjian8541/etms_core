using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.SendNotice
{
    public interface IStudentSendNotice3BLL : IBaseBLL
    {
        Task NoticeStudentCouponsGetConsumerEvent(NoticeStudentCouponsGetEvent request);

        Task NoticeStudentCouponsExplainConsumetEvent(NoticeStudentCouponsExplainEvent request);

        Task NoticeStudentAccountRechargeChangedConsumerEvent(NoticeStudentAccountRechargeChangedEvent request);

        Task NoticeStudentReservationConsumerEvent(NoticeStudentReservationEvent request);

        Task NoticeStudentClassCheckSignRevokeConsumerEvent(NoticeStudentClassCheckSignRevokeEvent request);

        Task NoticeStudentActiveGrowthCommentConsumerEvent(NoticeStudentActiveGrowthCommentEvent request);

        Task NoticeStudentAlbumConsumerEvent(NoticeStudentAlbumEvent request);
    }
}
