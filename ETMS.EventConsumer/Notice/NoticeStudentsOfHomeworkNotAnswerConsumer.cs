using ETMS.Event.DataContract;
using ETMS.IBusiness.EventConsumer;
using ETMS.IBusiness.SendNotice;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeStudentOfHomeworkNotAnswerQueue")]
    public class NoticeStudentsOfHomeworkNotAnswerConsumer : ConsumerBase<NoticeStudentsOfHomeworkNotAnswerEvent>
    {
        protected override async Task Receive(NoticeStudentsOfHomeworkNotAnswerEvent request)
        {
            var studentSendNotice2BLL = CustomServiceLocator.GetInstance<IStudentSendNotice2BLL>();
            studentSendNotice2BLL.InitTenantId(request.TenantId);
            await studentSendNotice2BLL.NoticeStudentsOfHomeworkNotAnswerConsumeEvent(request);
        }
    }
}
