using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeStudentsOfClassTodayGenerateQueue")]
    public class NoticeStudentsOfClassTodayGenerateConsumer : ConsumerBase<NoticeStudentsOfClassTodayGenerateEvent>
    {
        protected override async Task Receive(NoticeStudentsOfClassTodayGenerateEvent eEvent)
        {
            var studentSendNoticeBLL = CustomServiceLocator.GetInstance<IStudentSendNoticeBLL>();
            studentSendNoticeBLL.InitTenantId(eEvent.TenantId);
            await studentSendNoticeBLL.NoticeStudentsOfClassTodayGenerate(eEvent);
        }
    }
}
