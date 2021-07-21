using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.SendNotice
{
    public interface IUserSendNotice2BLL : IBaseBLL
    {
        Task NoticeUserActiveGrowthCommentConsumerEvent(NoticeUserActiveGrowthCommentEvent request);

        Task NoticeUserAboutStudentCheckOnConsumerEvent(NoticeUserAboutStudentCheckOnEvent request);
    }
}
