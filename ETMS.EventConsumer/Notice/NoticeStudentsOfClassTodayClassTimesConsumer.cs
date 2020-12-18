using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeStudentsOfClassTodayClassTimesQueue")]
    public class NoticeStudentsOfClassTodayClassTimesConsumer : ConsumerBase<NoticeStudentsOfClassTodayClassTimesEvent>
    {
        protected override async Task Receive(NoticeStudentsOfClassTodayClassTimesEvent eEvent)
        {
            var studentSendNoticeBLL = CustomServiceLocator.GetInstance<IStudentSendNoticeBLL>();
            studentSendNoticeBLL.InitTenantId(eEvent.TenantId);
            await studentSendNoticeBLL.NoticeStudentsOfClassTodayClassTimes(eEvent);
        }
    }
}
