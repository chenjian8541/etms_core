using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeStudentsOfClassTodayTenantQueue")]
    public class NoticeStudentsOfClassTodayTenantConsumer : ConsumerBase<NoticeStudentsOfClassTodayTenantEvent>
    {
        protected override async Task Receive(NoticeStudentsOfClassTodayTenantEvent eEvent)
        {
            var studentSendNoticeBLL = CustomServiceLocator.GetInstance<IStudentSendNoticeBLL>();
            studentSendNoticeBLL.InitTenantId(eEvent.TenantId);
            await studentSendNoticeBLL.NoticeStudentsOfClassTodayTenant(eEvent);
        }
    }
}
