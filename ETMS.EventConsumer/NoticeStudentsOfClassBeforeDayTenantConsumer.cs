using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeStudentsOfClassBeforeDayTenantQueue")]
    public class NoticeStudentsOfClassBeforeDayTenantConsumer : ConsumerBase<NoticeStudentsOfClassBeforeDayTenantEvent>
    {
        protected override async Task Receive(NoticeStudentsOfClassBeforeDayTenantEvent eEvent)
        {
            var studentSendNoticeBLL = CustomServiceLocator.GetInstance<IStudentSendNoticeBLL>();
            studentSendNoticeBLL.InitTenantId(eEvent.TenantId);
            await studentSendNoticeBLL.NoticeStudentsOfClassBeforeDayTenant(eEvent);
        }
    }
}
