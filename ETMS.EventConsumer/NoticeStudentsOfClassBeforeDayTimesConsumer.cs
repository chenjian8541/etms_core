using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeStudentsOfClassBeforeDayTimesQueue")]
    public class NoticeStudentsOfClassBeforeDayTimesConsumer : ConsumerBase<NoticeStudentsOfClassBeforeDayTimesEvent>
    {
        protected override async Task Receive(NoticeStudentsOfClassBeforeDayTimesEvent eEvent)
        {
            var studentSendNoticeBLL = CustomServiceLocator.GetInstance<IStudentSendNoticeBLL>();
            studentSendNoticeBLL.InitTenantId(eEvent.TenantId);
            await studentSendNoticeBLL.NoticeStudentsOfClassBeforeDayClassTimes(eEvent);
        }
    }
}
