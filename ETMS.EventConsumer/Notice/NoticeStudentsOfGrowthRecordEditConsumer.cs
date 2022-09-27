using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IBusiness.SendNotice;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeStudentsOfGrowthRecordEditQueue")]
    public class NoticeStudentsOfGrowthRecordEditConsumer : ConsumerBase<NoticeStudentsOfGrowthRecordEditEvent>
    {
        protected override async Task Receive(NoticeStudentsOfGrowthRecordEditEvent eEvent)
        {
            var studentSendNoticeBLL = CustomServiceLocator.GetInstance<IStudentSendNotice2BLL>();
            studentSendNoticeBLL.InitTenantId(eEvent.TenantId);
            await studentSendNoticeBLL.NoticeStudentsOfGrowthRecordEditConsumerEvent(eEvent);
        }
    }
}
