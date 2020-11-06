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

        Task NoticeStudentsOfHomeworkAddCommentConsumeEvent(NoticeStudentsOfHomeworkAddCommentEvent request);

        Task NoticeStudentsOfGrowthRecordConsumeEvent(NoticeStudentsOfGrowthRecordEvent request);
    }
}
